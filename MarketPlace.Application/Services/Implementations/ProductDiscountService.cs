using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.ProductDiscount;
using MarketPlace.DataLayer.Entities.Products;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations;

public class ProductDiscountService : IProductDiscountService
{
    #region constructor

    private readonly IGenericRepository<ProductDiscount> _productDiscountRepository;
    private readonly IGenericRepository<ProductDiscountUse> _productDiscountUseRepository;
    private readonly IGenericRepository<Product> _productRepository;

    public ProductDiscountService(IGenericRepository<ProductDiscount> productDiscountRepository,
        IGenericRepository<ProductDiscountUse> productDiscountUseRepository,
        IGenericRepository<Product> productRepository)
    {
        _productDiscountRepository = productDiscountRepository;
        _productDiscountUseRepository = productDiscountUseRepository;
        _productRepository = productRepository;
    }

    #endregion

    #region product discount

    public async Task<FilterProductDiscountDTO> FilterProductDiscountDTO(FilterProductDiscountDTO filter)
    {
        var query = _productDiscountRepository.GetQuery()
            .Include(x => x.Product)
            .AsQueryable();

        #region filter

        if (filter.ProductId != null && filter.ProductId != 0)
            query = query.Where(x => x.ProductId == filter.ProductId.Value);

        if (filter.SellerId != null && filter.SellerId != 0)
            query = query.Where(x => x.Product.SellerId == filter.SellerId.Value);

        #endregion

        #region paging

        var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
        var allEntities = await query.Paging(pager).ToListAsync();

        #endregion

        return filter.SetPaging(pager).SetDiscounts(allEntities);
    }

    public async Task<CreateDiscountResult> CreateProductDiscount(CreateProductDiscountDTO discount, long sellerId)
    {
        var product = await _productRepository.GetEntityById(discount.ProductId);
        if (product == null) return CreateDiscountResult.ProductNotFound;
        if (product.SellerId != sellerId) return CreateDiscountResult.ProductIsNotForSeller;

        var newDiscount = new ProductDiscount
        {
            ProductId = product.Id,
            DiscountNumber = discount.DiscountNumber,
            Percentage = discount.Percentage,
            ExpireDate = discount.ExpireDate.ToMiladiDateTime()
        };

        await _productDiscountRepository.AddEntity(newDiscount);
        await _productDiscountRepository.SaveChanges();

        return CreateDiscountResult.Success;
    }

    #endregion

    #region dispose

    public async ValueTask DisposeAsync()
    {
        _productDiscountRepository.DisposeAsync();
        _productDiscountUseRepository.DisposeAsync();
    }

    #endregion
}
