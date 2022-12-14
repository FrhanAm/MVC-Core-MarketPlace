using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.DataLayer.Entities.ProductOrder;
using MarketPlace.DataLayer.Entities.Wallet;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations;

public class OrderService : IOrderServcie
{
    #region constructor

    private readonly IGenericRepository<Order> _orderRepository;
    private readonly IGenericRepository<OrderDetail> _orderDetailRepository;
    private readonly ISellerWalletService _sellerWalletService;

    public OrderService(IGenericRepository<Order> orderRepository,
        IGenericRepository<OrderDetail> orderDetailRepository,
        ISellerWalletService sellerWalletService)
    {
        _orderDetailRepository = orderDetailRepository;
        _orderRepository = orderRepository;
        _sellerWalletService = sellerWalletService;
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
            .ThenInclude(x => x.ProductColor)
            .Include(x => x.OrderDetails)
            .ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(x => x.UserId == userId && !x.IsPaid);

        return userOpenOrder;
    }

    public async Task<int> GetTotlaOrderPriceForPayment(long userId)
    {
        var userOpenOrder = await GetUserLatestOpenOrder(userId);
        int totalPrice = 0;

        foreach (var detail in userOpenOrder.OrderDetails)
        {
            var oneProductPrice = detail.ProductColor != null
                ? detail.Product.Price + detail.ProductColor.Price
                : detail.Product.Price;
            totalPrice += detail.Count * oneProductPrice;
        }

        return totalPrice;
    }

    public async Task PayOrderProductPriceToSeller(long userId)
    {
        var openOrder = await GetUserLatestOpenOrder(userId);

        foreach (var detail in openOrder.OrderDetails)
        {
            var productPrice = detail.Product.Price;
            var productColorPrice = detail.ProductColor?.Price ?? 0;
            var discount = 0;
            var totalPrice = detail.Count * (productPrice + productColorPrice) - discount;

            await _sellerWalletService.AddWallet(new SellerWallet
            {
                SellerId = detail.Product.SellerId,
                Price = (int)Math.Ceiling(totalPrice * detail.Product.SiteProfit / (double)100),
                TransactionType = TransactionType.Deposit,
                Description = $"پرداخت مبلغ {totalPrice} تومان جهت فروش {detail.Product.Title} به تعداد {detail.Count} عدد با سهم تایین شده ی {100 - detail.Product.SiteProfit} درصد"
            });

            detail.ProductPrice = totalPrice;
            detail.ProductColorPrice = productColorPrice;
            _orderDetailRepository.EditEntity(detail);
        }

        openOrder.IsPaid = true;
        // todo: set description and tracking code in order
        _orderRepository.EditEntity(openOrder);
        await _orderRepository.SaveChanges();
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
        }
        else
        {
            similarOrder.Count += order.Count;
            await _orderDetailRepository.SaveChanges();
        }

    }

    public async Task<UserOpenOrderDTO> GetUserOpenOrderDetail(long userId)
    {
        var userOpenOrder = await GetUserLatestOpenOrder(userId);
        return new UserOpenOrderDTO
        {
            UserId = userOpenOrder.Id,
            Description = userOpenOrder.Description,
            Details = userOpenOrder.OrderDetails.Select(x => new UserOpenOrderDetailItemDTO
            {
                Count = x.Count,
                ColorName = x.ProductColor?.ColorName,
                ProductColorId = x.ProductColorId,
                ProductColorPrice = x.ProductColor?.Price ?? 0,
                ProductId = x.ProductId,
                ProductPrice = x.Product.Price,
                ProductTitle = x.Product.Title,
                ProductImageName = x.Product.ImageName,
            }).ToList(),
        };
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
