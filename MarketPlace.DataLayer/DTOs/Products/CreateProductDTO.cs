using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Products;

public class CreateProductDTO
{
    [Display(Name = "نام محصول")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Title { get; set; }

    [Display(Name = "قیمت محصول")]
    public int Price { get; set; }

    [Display(Name = "توضیحات کوتاه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ShortDescription { get; set; }

    [Display(Name = "توضیحات اصلی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Description { get; set; }

    [Display(Name = "فعال / غیر فعال")]
    public bool IsActive { get; set; }

    public List<CreateProductColorDTO>? ProductColors { get; set; }

    public List<CreateProductFeatureDTO>? ProductFeatures { get; set; }

    public List<long>? SelectedCategories { get; set; }
}

public enum CreateProductResult
{
    Success,
    HasNoImage,
    Error,
}