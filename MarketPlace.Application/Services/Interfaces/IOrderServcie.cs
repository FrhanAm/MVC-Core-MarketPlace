using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.DataLayer.Entities.ProductOrder;

namespace MarketPlace.Application.Services.Interfaces;

public interface IOrderServcie : IAsyncDisposable
{
	#region order

	Task<long> AddOrderForUser(long userId);
	Task<Order> GetUserLatestOpenOrder(long userId);
	Task<int> GetTotlaOrderPriceForPayment(long userId);
	Task PayOrderProductPriceToSeller(long userId);

	#endregion

	#region order detail

	Task AddProductToOpenOrder(long userId, AddProductToOrderDTO order);

	Task<UserOpenOrderDTO> GetUserOpenOrderDetail(long userId);

	Task ChangeOrderDetailCount(long detailId, long userId, int count);

	Task<bool> RemoveOrderDetail(long detailId, long userId);

	#endregion
}
