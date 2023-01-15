using MarketPlace.DataLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.Entities.Site;

public class SiteBanner : BaseEntity
{
    #region properties

    [Display(Name = "نام تصویر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ImageName { get; set; }

    [Display(Name = "آدرس")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string Url { get; set; }

    [Display(Name = "(سایز) کلاس های نمایشی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ColSize { get; set; }

    public BannerPlacement BannerPlacement { get; set; }

    #endregion
}
