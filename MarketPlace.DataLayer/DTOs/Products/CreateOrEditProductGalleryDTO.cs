using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MarketPlace.DataLayer.DTOs.Products;

public class CreateOrEditProductGalleryDTO
{
    [Display(Name = "اولویت نمایش")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int DisplayPriority { get; set; }

    [Display(Name = "تصویر")]
    public IFormFile Image { get; set; }

    public string ImageName { get; set; }
}

public enum CreateOrEditProductGalleryResult
{
    Success,
    NotForUserProduct,
    ImageIssNull,
    GalleryNotFound
}
