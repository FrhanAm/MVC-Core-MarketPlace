using MarketPlace.DataLayer.DTOs.Orders;

namespace MarketPlace.Application.EntitiesExtensions;

public static class OrderExtensions
{
    public static string GetOrderDetailWithDiscountPrice(this UserOpenOrderDetailItemDTO detail) => 
        detail.DiscountPercentage != null 
        ? (detail.ProductPrice * detail.DiscountPercentage.Value / 100 * detail.Count).ToString("#,0 تومان")
        : "";
}
