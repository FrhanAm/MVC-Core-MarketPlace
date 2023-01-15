using MarketPlace.DataLayer.DTOs.Site;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Account;

public class ActivateMobileDTO : CaptchaViewModel
{
    [Display(Name = "تلفن همراه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Mobile { get; set; }

    [Display(Name = "کد فعال سازی تلفن همراه")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(20, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string? MobileActiceCode { get; set; }
}
