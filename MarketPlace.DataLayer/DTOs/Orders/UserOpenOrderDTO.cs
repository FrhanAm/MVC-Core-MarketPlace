using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketPlace.DataLayer.DTOs.Orders;

public class UserOpenOrderDTO
{
    public long UserId { get; set; }

    public string? Description { get; set; }

    public List<UserOpenOrderDetailItemDTO> Details { get; set; }

    public int GetTotalPrice() => Details.Sum(x => x.Count * (x.ProductPrice + x.ProductColorPrice));

    public int GetTotalDiscounts() => Details.Sum(x => x.GetOrderDetailWithDiscountPriceAmount());
}
