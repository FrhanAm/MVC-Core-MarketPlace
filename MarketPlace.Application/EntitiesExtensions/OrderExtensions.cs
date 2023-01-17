//using MarketPlace.DataLayer.DTOs.Orders;

//namespace MarketPlace.Application.EntitiesExtensions;

//public static class OrderExtensions
//{
//    public static int GetOrderDetailWithDiscountPriceAmount(this UserOpenOrderDetailItemDTO detail) => 
//        detail.DiscountPercentage != null 
//        ? ((detail.ProductPrice + detail.ProductColorPrice) * detail.DiscountPercentage.Value / 100 * detail.Count)
//        : 0;

//    public static string GetOrderDetailWithDiscountPrice(this UserOpenOrderDetailItemDTO detail) =>
//        detail.DiscountPercentage != null
//        ? detail.GetOrderDetailWithDiscountPriceAmount().ToString("#,0 تومان")
//        : "----";
//}
