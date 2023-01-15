using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Account;

public class EditUserProfileDTO
{
    [Display(Name = "نام")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string FirstName { get; set; }

    [Display(Name = "نام خانوادگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string LastName { get; set; }

    [Display(Name = "تصویر آواتار")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string? Avatar { get; set; }
}

public enum EditUserProfileResult
{
    IsBlocked,
    IsNotActive,
    NotFound,
    Error,
    Success
}
