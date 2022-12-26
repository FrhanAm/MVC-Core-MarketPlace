using MarketPlace.DataLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace MarketPlace.DataLayer.Entities.ProductOrder;

public class Order : BaseEntity
{
	#region properties

	public long UserId { get; set; }

	public DateTime? PaymentDate { get; set; }

	public bool IsPaid { get; set; }

    [Display(Name = "کد پیگیری")]
    [MaxLength(300, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکنر باشد")]
    public string? TracingCode { get; set; }

    [Display(Name = "کد پیگیری")]
    public string? Description { get; set; }

	#endregion

	#region reltions

	public ICollection<OrderDetail> OrderDetails { get; set; }

	#endregion
}
