using MarketPlace.DataLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MarketPlace.DataLayer.Entities.Products;

public class ProductColor : BaseEntity
{

	#region properties

	public long ProductId { get; set; }

    [Display(Name = "رنگ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ColorName { get; set; }

    [Display(Name = "کد رنگ")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string ColorCode { get; set; }

	public int Price { get; set; }

	#endregion

	#region relations

	public Product Product { get; set; }

	#endregion
}
