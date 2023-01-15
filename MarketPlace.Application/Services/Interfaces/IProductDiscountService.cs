using MarketPlace.DataLayer.DTOs.ProductDiscount;

namespace MarketPlace.Application.Services.Interfaces;

public interface IProductDiscountService : IAsyncDisposable
{
	#region product discount

	Task<FilterProductDiscountDTO> FilterProductDiscountDTO(FilterProductDiscountDTO filter);
	Task<CreateDiscountResult> CreateProductDiscount(CreateProductDiscountDTO discount, long sellerId);

	#endregion
}
