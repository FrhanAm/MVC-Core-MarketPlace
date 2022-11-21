using MarketPlace.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarketPlace.DataLayer.Entities.Products;

public class ProductGallery : BaseEntity
{
    #region properties

    public long ProductId { get; set; }

    public int DisplayPriority { get; set; }

    [Display(Name = "نام تصویر")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ImageName { get; set; }

    #endregion

    #region relations

    public Product Product { get; set; }

    #endregion
}
