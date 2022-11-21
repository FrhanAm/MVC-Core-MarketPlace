using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Products;

public class CreateProductFeatureDTO
{
    public long ProductId { get; set; }

    [Display(Name = "عنوان ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Feature { get; set; }

    [Display(Name = "مقدار ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string FeatureValue { get; set; }
}
