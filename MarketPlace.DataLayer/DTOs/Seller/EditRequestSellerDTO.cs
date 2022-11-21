using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MarketPlace.DataLayer.DTOs.Seller;

public class EditRequestSellerDTO
{
    public long Id { get; set; }

    [Display(Name = "نام فروشگاه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string StoreName { get; set; }

    [Display(Name = "تلفن فروشگاه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(50, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Phone { get; set; }

    [Display(Name = "آدرس")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Address { get; set; }
}

public enum EditRequestSellerResult
{
    NotFound,
    Success
}
