using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Account;

public class ChangePasswordDTO
{
    [Display(Name = "کلمه عبور فعلی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string CurrentPassword { get; set; }

    [Display(Name = "کلمه عبور جدید")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string NewPassword { get; set; }

    [Display(Name = "تکرار کلمه عبور جدید")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ConfirmNewPassword { get; set; }
}
