﻿using MarketPlace.Application.Extensions;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Products;
using MarketPlace.Web.Http;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers;

public class ProductController : SellerBaseController
{
    #region constructor

    private readonly IProductService _productService;
    private readonly ISellerService _sellerService;

    public ProductController(IProductService productService, ISellerService sellerService)
    {
        _productService = productService;
        _sellerService = sellerService;
    }

    #endregion

    #region product

    #region list

    [HttpGet("products-list")]
    public async Task<IActionResult> Index(FilterProductDTO filter)
    {
        var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
        filter.SellerId = seller.Id;
        filter.FilterProductState = FilterProductState.All;

        return View(await _productService.FilterProducts(filter));
    }

    #endregion

    #region create product

    [HttpGet("create-product")]
    public async Task<IActionResult> CreateProduct()
    {
        ViewBag.Categories = await _productService.GetAllActiveProductCategories();

        return View();
    }

    [HttpPost("create-product"), ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(CreateProductDTO product, IFormFile productImage)
    {
        if (ModelState.IsValid)
        {
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            var res = await _productService.CreateProduct(product, seller.Id, productImage);

            switch (res)
            {
                case CreateProductResult.Success:
                    TempData[SuccessMessage] = $"محصول مورد نظر با عنوان {product.Title} با موفقیت ثبت شد";
                    return RedirectToAction("Index");
                case CreateProductResult.HasNoImage:
                    TempData[WarningMessage] = "لطفا تصویر محصول را وارد نمایید";
                    break;
                case CreateProductResult.Error:
                    TempData[ErrorMessage] = "عملیات ثبت محصول با خطا مواجه شد";
                    break;
            }
        }

        ViewBag.MainCategories = await _productService.GetAllActiveProductCategories();
        return View(product);
    }

    #endregion

    #region edit product

    [HttpGet("edit-product/{productId}")]
    public async Task<IActionResult> EditProduct(long productId)
    {
        var product = await _productService.GetProductForEdit(productId);
        if (product == null) return NotFound();

        ViewBag.Categories = await _productService.GetAllActiveProductCategories();

        return View(product);
    }

    [HttpPost("edit-product/{productId}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(EditProductDTO product, IFormFile? productImage)
    {
        if (ModelState.IsValid)
        {
            var res = await _productService.EditSellerProduct(product, User.GetUserId(), productImage);

            switch (res)
            {
                case EditProductResult.NotFound:
                    TempData[WarningMessage] = "اطلاعات وارد شده یافت نشد";
                    break;
                case EditProductResult.NotForUser:
                    TempData[ErrorMessage] = "در ویرایش اطلاعات خطایی رخ داد";
                    break;
                case EditProductResult.Success:
                    TempData[SuccessMessage] = "عملیات با موفقیت انجام شد";
                    return RedirectToAction("Index");
            }
        }

        ViewBag.Categories = await _productService.GetAllActiveProductCategories();
        return View(product);
    }

    #endregion

    #region get product json

    [HttpGet("products-autocomplete")]
    public async Task<IActionResult> GetSellerProductsJson(string productName)
    {
        var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
        var data = await _productService.FilterGetProductsForSellerByProductName(productName, seller.Id);
        return new JsonResult(data);
    }

    #endregion

    #endregion

    #region product gallery

    #region list

    [HttpGet("product-galleries/{id}")]
    public async Task<IActionResult> GetProuctGalleries(long id)
    {
        ViewBag.productId = id;
        var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
        return View(await _productService.GetAllProductGalleriesInSellerPanel(id, seller.Id));
    }

    #endregion

    #region create

    [HttpGet("create-product-gallery/{productId}")]
    public async Task<IActionResult> CreateProductGallery(long productId)
    {
        var product = await _productService.GetProductBySellerOwnerId(productId, User.GetUserId());
        if (product == null) return NotFound();
        ViewBag.product = product;
        return View();
    }

    [HttpPost("create-product-gallery/{productId}")]
    public async Task<IActionResult> CreateProductGallery(long productId, CreateOrEditProductGalleryDTO gallery)
    {
        if (ModelState.IsValid)
        {
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            var result = await _productService.CreateProductGallery(gallery, productId, seller.Id);

            switch (result)
            {
                case CreateOrEditProductGalleryResult.Success:
                    TempData[SuccessMessage] = "عملیات ثبت گالری محصول با موفقیت انجام شد";
                    return RedirectToAction("GetProductGalleries", "Product", new { id = productId });
                case CreateOrEditProductGalleryResult.NotForUserProduct:
                    TempData[ErrorMessage] = "محصول مورد نظر در لیست محصولات شما یافت نشد"; 
                    break;
                case CreateOrEditProductGalleryResult.ImageIssNull:
                    TempData[WarningMessage] = "تصویر مربوطه را وارد نمایید";
                    break;
                case CreateOrEditProductGalleryResult.GalleryNotFound:
                    TempData[WarningMessage] = "محصول مورد نظر یافت نشد";
                    break;
                default:
                    break;
            }
        }

        var product = await _productService.GetProductBySellerOwnerId(productId, User.GetUserId());
        if (product == null) return NotFound();
        ViewBag.product = product;

        return View(gallery);
    }

    #endregion

    #region edit

    [HttpGet("product_{productId}/edit-gallery/{galleryId}")]
    public async Task<IActionResult> EditGallery(long productId, long galleryId)
    {
        var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
        var mainGallery = await _productService.GetProductGalleryForEdit(galleryId, seller.Id);
        if (mainGallery == null) return NotFound();
        
        return View(mainGallery);
    }
    
    [HttpPost("product_{productId}/edit-gallery/{galleryId}")]
    public async Task<IActionResult> EditGallery(long productId, long galleryId, CreateOrEditProductGalleryDTO gallery)
    {
        if (ModelState.IsValid)
        {
            var seller = await _sellerService.GetLastActiveSellerByUserId(User.GetUserId());
            var res = await _productService.EditProductGallery(galleryId, seller.Id, gallery);

            switch (res)
            {
                case CreateOrEditProductGalleryResult.Success:
                    TempData[SuccessMessage] = "اطلاعات مورد نظر با موفقیت ویرایش شد";
                    RedirectToAction("GetProductGalleries", "Product", new { id = productId });
                    break;
                case CreateOrEditProductGalleryResult.NotForUserProduct:
                    TempData[ErrorMessage] = "این اطلاعات برای شما غیر قابل دسترسی می باشد";
                    break;
                case CreateOrEditProductGalleryResult.ImageIssNull:
                    break;
                case CreateOrEditProductGalleryResult.GalleryNotFound:
                    TempData[WarningMessage] = "اطلاعات مورد نظر یافت نشد";
                    break;
                default:
                    break;
            }
        }

        return View();
    }

    #endregion

    #endregion

    #region product categories

    [HttpGet("product-categories/{parentId}")]
    public async Task<IActionResult> GetProductCategoriesByParent(long parentId)
    {
        var categories = await _productService.GetAllProductCategoriesByParentId(parentId);

        return JsonResponseStatus.SendStatus(JsonResponseStatusType.Success, "اطلاعات دسته بندی ها", categories);
    }

    #endregion
}
