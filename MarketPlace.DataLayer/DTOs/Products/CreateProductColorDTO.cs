using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MarketPlace.DataLayer.DTOs.Products;

public class CreateProductColorDTO
{
    [Display(Name = "رنگ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ColorName { get; set; }
    
    [Display(Name = "کد رنگ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ColorCode { get; set; }

    public int Price { get; set; }
}
