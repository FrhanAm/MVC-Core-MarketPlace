﻿using MarketPlace.DataLayer.Entities.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.DataLayer.DTOs.Products;

public class ProductDetailDTO
{
    public long ProductId { get; set; }

    public long SellerId { get; set; }

    [Display(Name = "نام محصول")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Title { get; set; }

    [Display(Name = "تصویر محصول")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ImageName { get; set; }

    [Display(Name = "قیمت محصول")]
    public int Price { get; set; }

    [Display(Name = "توضیحات کوتاه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ShortDescription { get; set; }

    [Display(Name = "توضیحات اصلی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Description { get; set; }

    public Entities.Store.Seller Seller { get; set; }

    public List<ProductGallery> ProductGalleries { get; set; }

    public List<ProductColor> ProductColors { get; set; }

    public List<ProductCategory> ProductCategories { get; set; }

    public List<ProductFeature> ProductFeatures { get; set; }

    public List<Product> RelatedProducts { get; set; }
}
