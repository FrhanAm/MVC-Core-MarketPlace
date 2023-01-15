using MarketPlace.DataLayer.DTOs.Site;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Account;

public class ForgotPasswordDTO : CaptchaViewModel
{
    //[Display(Name = "تلفن همراه")]
    //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    //[MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    //public string Mobile { get; set; }
    
    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Email { get; set; }
}

public enum ForgotPasswordResult
{
    Success,
    NotFound,
    Error
}
