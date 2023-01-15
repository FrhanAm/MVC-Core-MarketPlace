using MarketPlace.DataLayer.DTOs.Site;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Contacts;

public class CreateContactUsDTO : CaptchaViewModel
{
    [Display(Name = "ایمیل")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Email { get; set; }


    [Display(Name = "نام و نام خانوادگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string FullName { get; set; }


    [Display(Name = "عنوان")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Subject { get; set; }


    [Display(Name = "متن پیام")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string Text { get; set; }

}
