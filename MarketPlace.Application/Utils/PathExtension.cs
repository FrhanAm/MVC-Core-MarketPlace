namespace MarketPlace.Application.Utils;

public static class PathExtension
{
    #region domain address

    public static string DomainAddress = "https://localhost:7224";

    #endregion

    #region default images

    public static string DefaultAvatar = "/img/defaults/avatar.jpg";

    #endregion
    
    #region uploader

    public static string UploadImage = "/img/upload/";
    public static string UploadImageServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/upload/");

    #endregion

    #region product

    public static string ProductImage = "/content/images/product/origin/";
    public static string ProductImageServer = Path
        .Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/product/origin/");

    public static string ProductThumbnailImage = "/content/images/product/thumb/";
    public static string ProductThumbnailImageServer = Path
        .Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/product/thumb/");

    #endregion

    #region product gallery

    public static string ProductGalleryImage = "/content/images/product-gallery/origin/";
    public static string ProductGalleryImageServer = Path
        .Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/product-gallery/origin/");

    public static string ProductGalleryThumbnailImage = "/content/images/product-gallery/thumb/";
    public static string ProductGalleryThumbnailImageServer = Path
        .Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/product-gallery/thumb/");

    #endregion


    #region user avatar

    public static string UserAvatarOrigin = "/content/images/userAvatar/origin/";
    public static string UserAvatarOriginServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/userAvatar/origin/");

    public static string UserAvatarThumb = "/content/images/userAvatar/thumb/";
    public static string UserAvatarThumbServer = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/content/images/userAvatar/thumb/");

    #endregion

    #region slider

    public static string SliderOrigin = "/img/slider/";

    #endregion

    #region banner

    public static string BannerOrigin = "/img/bg/";

    #endregion
}
