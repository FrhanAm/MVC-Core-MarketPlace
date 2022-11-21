using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Seller;

public class RequestSellerDTO
{
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

    //[Display(Name = "یادداشت های ادمین")]
    //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    //public string AdminDescription { get; set; }
}

public enum RequestSellerResult
{
    Success,
    HasUnderProgressRequest,
    HasNotPermission
}
