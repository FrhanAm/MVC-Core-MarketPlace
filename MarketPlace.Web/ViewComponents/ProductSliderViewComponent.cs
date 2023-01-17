using MarketPlace.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.ViewComponents;

public class ProductSlider : ViewComponent
{
	#region constructor

	private readonly IProductService _productService;

	public ProductSlider(IProductService productService)
	{
		_productService = productService;
	}

	#endregion

	#region body

	public async Task<IViewComponentResult> InvokeAsync(string categoryName)
	{
		var category = await _productService.GetProductCategoryByUrlName(categoryName);
		var product = await _productService.GetCategoryProductsByCategoryName(categoryName);
		ViewBag.Title = category?.Title;
		return View("ProductSlider", product);
	}

	#endregion
}
