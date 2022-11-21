using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Seller;
using MarketPlace.Web.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Admin.Controllers;

public class SellerController : AdminBaseController
{
	#region constructor

	private readonly ISellerService _sellerService;

	public SellerController(ISellerService sellerService)
	{
		_sellerService = sellerService;
	}

	#endregion

	#region seller requests

	public async Task<IActionResult> SellerRequests(FilterSellerDTO filter)
	{
		return View(await _sellerService.FilterSellers(filter));
	}

	#endregion

	#region accept seller request

	public async Task<IActionResult> AcceptSellerRequest(long requestId)
	{
		var result = await _sellerService.AcceptSellerRequest(requestId);

		if (result)
		{
			return JsonResponseStatus.SendStatus(
				JsonResponseStatusType.Success,
				"درخواست مورد نظر با موفقیت تایید شد"
				);
		}

		return JsonResponseStatus.SendStatus(
			JsonResponseStatusType.Danger,
			"اطلاعاتی با این مشخصات یافت نشد"
			);
	}

	#endregion

	#region reject seller request

	[HttpPost, ValidateAntiForgeryToken]
	public async Task<IActionResult> RejectSellerRequest(RejectItemDTO reject)
	{
		if (ModelState.IsValid)
		{
            var result = await _sellerService.RejectSellerRequest(reject);

            if (result)
            {
                return JsonResponseStatus.SendStatus(
                    JsonResponseStatusType.Success,
                    "درخواست مورد نظر با موفقیت رد شد",
                    reject
                    );
            }

            return JsonResponseStatus.SendStatus(
                JsonResponseStatusType.Danger,
                "اطلاعاتی با این مشخصات یافت نشد"
                );
        }

		return JsonResponseStatus.SendStatus(
			JsonResponseStatusType.Danger,
			"اطلاعاتی با این مشخصات یافت نشد"
			);
	}

	#endregion
}
