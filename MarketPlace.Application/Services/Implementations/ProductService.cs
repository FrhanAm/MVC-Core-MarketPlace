using MarketPlace.Application.Extensions;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.Products;
using MarketPlace.DataLayer.Entities.Products;
using MarketPlace.DataLayer.Migrations;
using MarketPlace.DataLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations;

public class ProductService : IProductService
{
    #region constructor

    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<ProductCategory> _productCategoryRepository;
    private readonly IGenericRepository<ProductSelectedCategory> _productSelectedCategoryRepository;
    private readonly IGenericRepository<ProductColor> _productColorRepository;
    private readonly IGenericRepository<ProductGallery> _productGalleryRepository;
    private readonly IGenericRepository<ProductFeature> _productFeatureRepository;

    public ProductService(
        IGenericRepository<Product> productRepository,
        IGenericRepository<ProductCategory> productCategoryRepository,
        IGenericRepository<ProductSelectedCategory> productSelectedCategoryRepository,
        IGenericRepository<ProductColor> productColorRepository,
        IGenericRepository<ProductGallery> productGalleryRepository,
        IGenericRepository<ProductFeature> productFeatureRepository
        )
    {
        _productRepository = productRepository;
        _productCategoryRepository = productCategoryRepository;
        _productSelectedCategoryRepository = productSelectedCategoryRepository;
        _productColorRepository = productColorRepository;
        _productGalleryRepository = productGalleryRepository;
        _productFeatureRepository = productFeatureRepository;
    }

    #endregion

    #region products

    public async Task<FilterProductDTO> FilterProducts(FilterProductDTO filter)
    {
        var query = _productRepository.GetQuery()
            .Include(x => x.ProductSelectedCategories)
            .ThenInclude(x => x.ProductCategory)
            .AsQueryable();

        var mostExpensiveProduct = await query.OrderByDescending(x => x.Price).FirstOrDefaultAsync();
        filter.FilterMaxPrice = mostExpensiveProduct.Price;

        #region state

        switch (filter.FilterProductState)
        {
            case FilterProductState.All:
                break;
            case FilterProductState.UnderProgress:
                query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.UnderProgress);
                break;
            case FilterProductState.Accepted:
                query = query.Where(x => x.IsActive && x.ProductAcceptanceState == ProductAcceptanceState.Accepted);
                break;
            case FilterProductState.Rejected:
                query = query.Where(x => x.ProductAcceptanceState == ProductAcceptanceState.Rejected);
                break;
            case FilterProductState.Active:
                query = query.Where(x => x.IsActive);
                break;
            case FilterProductState.NotActive:
                query = query.Where(x => !x.IsActive && x.ProductAcceptanceState == ProductAcceptanceState.Accepted);
                break;
            default:
                break;
        }

        switch (filter.OrderBy)
        {
            case FilterProductOrderBy.CreateDate_Des:
                query = query.OrderByDescending(x => x.CreateDate);
                break;
            case FilterProductOrderBy.CreateDate_Asc:
                query = query.OrderBy(x => x.CreateDate);
                break;
            case FilterProductOrderBy.Price_Des:
                query = query.OrderByDescending(x => x.Price);
                break;
            case FilterProductOrderBy.Price_Asc:
                query = query.OrderBy(x => x.Price);
                break;
        }

        #endregion

        #region filter

        if (!string.IsNullOrEmpty(filter.ProductTitle))
            query = query.Where(x => EF.Functions.Like(x.Title, $"%{filter.ProductTitle}%"));

        if (filter.SellerId != null && filter.SellerId != 0)
            query = query.Where(x => x.SellerId == filter.SellerId.Value);

        if (filter.SelectedMaxPrice == 0) filter.SelectedMaxPrice = mostExpensiveProduct.Price;

        query = query.Where(x => x.Price >= filter.SelectedMinPrice);
        query = query.Where(x => x.Price <= filter.SelectedMaxPrice);

        if (!string.IsNullOrEmpty(filter.Category))
            query = query.Where(x => x.ProductSelectedCategories
            .Any(y => y.ProductCategory.UrlName == filter.Category));

        #endregion

        #region paging

        var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
        var allEntities = await query.Paging(pager).ToListAsync();

        #endregion

        return filter.SetProducts(allEntities).SetPaging(pager);
    }

    public async Task<CreateProductResult> CreateProduct(CreateProductDTO product, long sellerId, IFormFile productImage)
    {
        if (productImage == null) return CreateProductResult.HasNoImage;

        var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(productImage.FileName);

        var res = productImage.AddImageToServer(imageName, PathExtension.ProductImageServer, 150, 150, PathExtension.ProductThumbnailImageServer);

        if (res)
        {
            // create product
            var newProduct = new Product
            {
                Title = product.Title,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                IsActive = product.IsActive,
                SellerId = sellerId,
                ImageName = imageName,
                ProductAcceptanceState = ProductAcceptanceState.UnderProgress
            };

            await _productRepository.AddEntity(newProduct);
            await _productRepository.SaveChanges();

            // create product categories
            if (product.SelectedCategories != null)
            {
                await AddProductSelectedCategories(newProduct.Id, product.SelectedCategories);
                await _productSelectedCategoryRepository.SaveChanges();
            }
            // create product colors
            if (product.ProductColors != null)
            {
                await AddProductSelectedColors(newProduct.Id, product.ProductColors);
                await _productColorRepository.SaveChanges();
            }
            // create product colors
            if (product.ProductFeatures != null)
            {
                await CreateProductFeature(newProduct.Id, product.ProductFeatures);
                await _productFeatureRepository.SaveChanges();
            }

            return CreateProductResult.Success;
        }

        return CreateProductResult.Error;
    }

    public async Task<EditProductResult> EditSellerProduct(EditProductDTO product, long userId, IFormFile productImage)
    {
        var mainProduct = await _productRepository.GetQuery().AsQueryable()
            .Include(x => x.Seller)
            .SingleOrDefaultAsync(x => x.Id == product.Id);
        if (mainProduct == null) return EditProductResult.NotFound;
        if (mainProduct.Seller.UserId != userId) return EditProductResult.NotForUser;

        mainProduct.Title = product.Title;
        mainProduct.ShortDescription = product.ShortDescription;
        mainProduct.Description = product.Description;
        mainProduct.IsActive = product.IsActive;
        mainProduct.Price = product.Price;
        mainProduct.ProductAcceptanceState = ProductAcceptanceState.UnderProgress;

        if (productImage != null)
        {
            var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(productImage.FileName);

            var res = productImage.AddImageToServer(imageName, PathExtension.ProductImageServer, 150, 150,
                PathExtension.ProductThumbnailImageServer, mainProduct.ImageName);

            if (res)
            {
                mainProduct.ImageName = imageName;
            }
        }

        // remove all product categories and add new ones
        await RemoveAllProductSelectedCategories(product.Id);
        if (product.SelectedCategories != null)
        {
            await AddProductSelectedCategories(product.Id, product.SelectedCategories);
            await _productSelectedCategoryRepository.SaveChanges();
        }

        // remove all product features and add new ones
        await RemoveAllProductFeatures(product.Id);
        if (product.ProductFeatures != null)
        {
            await CreateProductFeature(product.Id, product.ProductFeatures);
            await _productFeatureRepository.SaveChanges();
        }

        // create product colors and add new ones
        await RemoveAllProductSelectedColors(product.Id);
        if (product.ProductColors != null)
        {
            await AddProductSelectedColors(product.Id, product.ProductColors);
            await _productColorRepository.SaveChanges();
        }

        return EditProductResult.Success;
    }

    public async Task<bool> AcceptSellerProduct(long productId)
    {
        var product = await _productRepository.GetEntityById(productId);
        if (product != null)
        {
            product.ProductAcceptanceState = ProductAcceptanceState.Accepted;
            product.ProductAcceptOrRejectDescription = $"محصول مورد نظر در تاریخ {DateTime.Now.ToShamsi()} مورد تایید سایت قرار گرفت";
            _productRepository.EditEntity(product);
            await _productRepository.SaveChanges();
            return true;
        }
        return false;
    }

    public async Task<bool> RejectSellerProduct(RejectItemDTO reject)
    {
        var product = await _productRepository.GetEntityById(reject.Id);
        if (product != null)
        {
            product.ProductAcceptanceState = ProductAcceptanceState.Rejected;
            product.ProductAcceptOrRejectDescription = reject.RejectMessage;
            _productRepository.EditEntity(product);
            await _productRepository.SaveChanges();
            return true;
        }
        return false;
    }

    public async Task<EditProductDTO> GetProductForEdit(long productId)
    {
        var product = await _productRepository.GetEntityById(productId);
        if (product == null) return null;

        return new EditProductDTO
        {
            Id = productId,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            Price = product.Price,
            IsActive = product.IsActive,
            ImageName = product.ImageName,
            Title = product.Title,
            ProductColors = await _productColorRepository.GetQuery().AsQueryable()
            .Where(x => !x.IsDeleted && x.ProductId == productId)
            .Select(x => new CreateProductColorDTO { Price = x.Price, ColorName = x.ColorName, ColorCode = x.ColorCode }).ToListAsync(),
            SelectedCategories = await _productSelectedCategoryRepository.GetQuery().AsQueryable()
            .Where(x => x.ProductId == productId).Select(x => x.ProductCategoryId).ToListAsync(),
            ProductFeatures = await _productFeatureRepository.GetQuery().AsQueryable()
            .Where(x => !x.IsDeleted && x.ProductId == productId)
            .Select(x => new CreateProductFeatureDTO
            {
                Feature = x.FeatureTitle,
                FeatureValue = x.FeatureValue
            }).ToListAsync(),

        };
    }

    public async Task RemoveAllProductSelectedCategories(long productId)
    {
        _productSelectedCategoryRepository
            .DeletePermanentEntities(await _productSelectedCategoryRepository.GetQuery().AsQueryable().Where(x => x.ProductId == productId).ToListAsync());
    }

    public async Task RemoveAllProductSelectedColors(long productId)
    {
        _productColorRepository
            .DeletePermanentEntities(await _productColorRepository.GetQuery().AsQueryable().Where(x => x.ProductId == productId).ToListAsync());
    }

    public async Task AddProductSelectedCategories(long productId, List<long> selectedCategories)
    {
        var productSelectedCategories = new List<ProductSelectedCategory>();

        foreach (var categoryId in selectedCategories)
        {
            productSelectedCategories.Add(new ProductSelectedCategory
            {
                ProductCategoryId = categoryId,
                ProductId = productId,
            });
        }
        await _productSelectedCategoryRepository.AddRangeEntities(productSelectedCategories);
    }

    public async Task AddProductSelectedColors(long productId, List<CreateProductColorDTO> colors)
    {
        var productSelectedColors = new List<ProductColor>();

        foreach (var productColor in colors)
        {
            if (productSelectedColors.All(x => x.ColorName != productColor.ColorName))
            {
                productSelectedColors.Add(new ProductColor
                {
                    ColorName = productColor.ColorName,
                    Price = productColor.Price,
                    ProductId = productId,
                    ColorCode = productColor.ColorCode,
                });
            }
        }

        await _productColorRepository.AddRangeEntities(productSelectedColors);
    }

    public async Task<ProductDetailDTO> GetProductDetailById(long productId)
    {
        var product = await _productRepository.GetQuery().AsQueryable()
            .Include(x => x.Seller)
            .ThenInclude(x => x.User)
            .Include(x => x.ProductSelectedCategories)
            .ThenInclude(x => x.ProductCategory)
            .Include(x => x.ProductGalleries)
            .Include(x => x.ProductColors)
            .Include(x => x.ProductFeatures)
            .SingleOrDefaultAsync(x => x.Id == productId);
            
        if (product == null) return null;

        var selectedCategoriesIds = product.ProductSelectedCategories.Select(x => x.ProductCategoryId).ToList();

        return new ProductDetailDTO
        {
            ProductId = productId,
            Title = product.Title,
            Price = product.Price,
            ImageName = product.ImageName,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            Seller = product.Seller,
            ProductCategories = product.ProductSelectedCategories.Select(x => x.ProductCategory).ToList(),
            ProductGalleries = product.ProductGalleries.ToList(),
            ProductColors = product.ProductColors.ToList(),
            SellerId = product.SellerId,
            ProductFeatures = product.ProductFeatures.ToList(),
            RelatedProducts = await _productRepository.GetQuery()
            .Include(m => m.ProductSelectedCategories)
            .Where(x => x.ProductSelectedCategories.Any(y => selectedCategoriesIds.Contains(y.ProductCategoryId)) 
                && x.Id != productId
                && x.ProductAcceptanceState == ProductAcceptanceState.Accepted).ToListAsync()
        };
    }

    #endregion

    #region product gallery

    public async Task<List<ProductGallery>> GetAllProductGalleries(long productId)
    {
        return await _productGalleryRepository.GetQuery()
            .AsQueryable().Where(x => x.ProductId == productId).ToListAsync();
    }

    public async Task<Product> GetProductBySellerOwnerId(long productId, long userId)
    {
        return await _productRepository.GetQuery()
            .Include(x => x.Seller)
            .SingleOrDefaultAsync(x => x.Id == productId && x.Seller.UserId == userId);
    }

    public async Task<List<ProductGallery>> GetAllProductGalleriesInSellerPanel(long productId, long sellerId)
    {
        return await _productGalleryRepository.GetQuery()
            .Include(x => x.Product)
            .Where(x => x.ProductId == productId && x.Product.SellerId == sellerId).ToListAsync();
    }

    public async Task<CreateOrEditProductGalleryResult> CreateProductGallery(CreateOrEditProductGalleryDTO gallery, long productId, long sellerId)
    {
        var product = await _productRepository.GetEntityById(productId);
        if (product == null) return CreateOrEditProductGalleryResult.GalleryNotFound;
        if (product.SellerId != sellerId) return CreateOrEditProductGalleryResult.NotForUserProduct;
        if (gallery.Image == null || !gallery.Image.IsImage()) return CreateOrEditProductGalleryResult.ImageIssNull;

        var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(gallery.Image.FileName);
        gallery.Image.AddImageToServer(imageName, PathExtension.ProductGalleryImageServer, 100, 100, PathExtension.ProductGalleryThumbnailImageServer);

        await _productGalleryRepository.AddEntity(new ProductGallery
        {
            ProductId = productId,
            ImageName = imageName,
            DisplayPriority = gallery.DisplayPriority,
        });

        await _productGalleryRepository.SaveChanges();

        return CreateOrEditProductGalleryResult.Success;
    }

    public async Task<CreateOrEditProductGalleryDTO> GetProductGalleryForEdit(long galleryId, long sellerId)
    {
        var gallery = await _productGalleryRepository.GetQuery()
            .Include(x => x.Product)
            .SingleOrDefaultAsync(x => x.Id == galleryId && x.Product.SellerId == sellerId);

        if (gallery == null) return null;
        return new CreateOrEditProductGalleryDTO
        {
            ImageName = gallery.ImageName,
            DisplayPriority = gallery.DisplayPriority
        };
    }

    public async Task<CreateOrEditProductGalleryResult> EditProductGallery(long galleryId, long sellerId, CreateOrEditProductGalleryDTO gallery)
    {
        var mainGallery = await _productGalleryRepository.GetQuery()
            .Include(x => x.Product)
            .SingleOrDefaultAsync(x => x.Id == galleryId);

        if (mainGallery == null) return CreateOrEditProductGalleryResult.GalleryNotFound;

        if (mainGallery.Product.SellerId != sellerId) return CreateOrEditProductGalleryResult.NotForUserProduct;

        if (gallery.Image != null && gallery.Image.IsImage())
        {
            var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(gallery.Image.FileName);
            var result = gallery.Image.AddImageToServer(imageName, PathExtension.ProductGalleryImageServer, 100, 100,
                PathExtension.ProductGalleryThumbnailImageServer, mainGallery.ImageName);
            mainGallery.ImageName = imageName;
        }

        mainGallery.DisplayPriority = gallery.DisplayPriority;
        _productGalleryRepository.EditEntity(mainGallery);
        await _productGalleryRepository.SaveChanges();
        return CreateOrEditProductGalleryResult.Success;
    }

    #endregion

    #region product categories

    public async Task<List<ProductCategory>> GetAllProductCategoriesByParentId(long? parentId)
    {
        if (parentId == null || parentId == 0)
        {
            return await _productCategoryRepository
                .GetQuery()
                .AsQueryable()
                .Where(x => !x.IsDeleted && x.IsActive && x.ParentId == null)
                .ToListAsync();
        }

        return await _productCategoryRepository
            .GetQuery()
            .AsQueryable()
            .Where(x => !x.IsDeleted && x.IsActive && x.ParentId == parentId)
            .ToListAsync();
    }

    public async Task<List<ProductCategory>> GetAllActiveProductCategories()
    {
        return await _productCategoryRepository.GetQuery().AsQueryable()
            .Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
    }

    #endregion

    #region product feature

    public async Task CreateProductFeature(long productId, List<CreateProductFeatureDTO> features)
    {
        var newFeatures = new List<ProductFeature>();
        if (features != null && features.Any())
        {
            foreach (var feature in features)
            {
                newFeatures.Add(new ProductFeature
                {
                    ProductId = productId,
                    FeatureTitle = feature.Feature,
                    FeatureValue = feature.FeatureValue,
                });
            }

            await _productFeatureRepository.AddRangeEntities(newFeatures);
            await _productFeatureRepository.SaveChanges();
        }
    }

    public async Task RemoveAllProductFeatures(long productId)
    {
        var productFeatures = await _productFeatureRepository
            .GetQuery().Where(x => x.ProductId == productId).ToListAsync();
        _productFeatureRepository.DeletePermanentEntities(productFeatures);
        _productFeatureRepository.SaveChanges();
    }

    #endregion

    #region dispose

    public async ValueTask DisposeAsync()
    {
        await _productCategoryRepository.DisposeAsync();
        await _productRepository.DisposeAsync();
        await _productSelectedCategoryRepository.DisposeAsync();
        await _productGalleryRepository.DisposeAsync();
        await _productFeatureRepository.DisposeAsync();
        await _productColorRepository.DisposeAsync();
    }

    #endregion
}
