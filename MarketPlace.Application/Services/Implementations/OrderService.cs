using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.DataLayer.Entities.ProductOrder;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations;

public class OrderService : IOrderServcie
{
    #region constructor

    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderDetail> _orderDetailRepository;

    public OrderService(IGenericRepository<Order> orderRepository,
        IGenericRepository<OrderDetail> orderDetailRepository)
    {
        _orderDetailRepository = orderDetailRepository;
        _orderRepository = orderRepository;
    }

    #endregion

    #region order

    public async Task<long> AddOrderForUser(long userId)
    {
        var order = new Order { UserId = userId };

        await _orderRepository.AddEntity(order);

        await _orderRepository.SaveChanges();

        return order.Id;
    }

    public async Task<Order> GetUserLatestOpenOrder(long userId)
    {
        if (!await _orderRepository.GetQuery().AnyAsync(x => x.UserId == userId && !x.IsPaid))
            await AddOrderForUser(userId);

        var userOpenOrder = await _orderRepository.GetQuery()
            .Include(x => x.OrderDetails)
            .SingleOrDefaultAsync(x => x.UserId == userId && !x.IsPaid);

        if (userOpenOrder == null)
        {

        }

        return userOpenOrder;
    }

    #endregion

    #region order detail

    public async Task AddProductToOpenOrder(long userId, AddProductToOrderDTO order)
    {
        var openOrder = await GetUserLatestOpenOrder(userId);

        var similarOrder = openOrder.OrderDetails.SingleOrDefault(x => x.ProductId == order.ProductId 
                            && x.ProductColorId == order.ProductColorId);

        if (similarOrder == null)
        {
            var orderDetail = new OrderDetail
            {
                OrderId = openOrder.Id,
                ProductId = order.ProductId,
                ProductColorId = order.ProductColorId,
                Count = order.Count,
            };

            await _orderDetailRepository.AddEntity(orderDetail);
            await _orderDetailRepository.SaveChanges();
        } else
        {
            similarOrder.Count += order.Count;
            await _orderDetailRepository.SaveChanges();
        }

    }

    #endregion

    #region dispose

    public async ValueTask DisposeAsync()
    {
        await _orderRepository.DisposeAsync();
        await _orderDetailRepository.DisposeAsync();
    }

    #endregion
}
