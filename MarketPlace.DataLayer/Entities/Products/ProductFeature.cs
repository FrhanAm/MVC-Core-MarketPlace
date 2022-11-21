using MarketPlace.DataLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MarketPlace.DataLayer.Entities.Products;

public class ProductFeature : BaseEntity
{

	#region properties

	public long ProductId { get; set; }

    [Display(Name = "عنوان ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string FeatureTitle { get; set; }

    [Display(Name = "مقدار ویژگی")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string FeatureValue { get; set; }

	#endregion

	#region relations

	public Product Product { get; set; }

	#endregion
}
