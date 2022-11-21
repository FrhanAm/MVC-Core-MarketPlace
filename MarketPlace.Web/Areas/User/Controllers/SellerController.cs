using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Seller;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers;

public class SellerController : UserBaseController
{
    #region constructor

    private readonly ISellerService _sellerService;

    public SellerController(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    #endregion

    #region request seller

    [HttpGet("request-seller-panel")]
    public IActionResult RequestSellerPanel()
    {
        return View();
    }

    [HttpPost("request-seller-panel"), ValidateAntiForgeryTokenAttribute]
    public async Task<IActionResult> RequestSellerPanel(RequestSellerDTO seller)
    {
        if (ModelState.IsValid)
        {
            var res = await _sellerService.AddNewSellerRequest(seller, User.GetUserId());

            switch (res)
            {
                case RequestSellerResult.Success:
                    TempData[SuccessMessage] = "درخواست شما با موفقیت ثبت شد";
                    TempData[InfoMessage] = "فرایند تایید اطلاعات شما در حال پیگیری می باشد";
                    return RedirectToAction("SellerRequests");
                case RequestSellerResult.HasUnderProgressRequest:
                    TempData[WarningMessage] = "درخواست های قبلی شما در حال پیگیری می باشند";
                    break;
                case RequestSellerResult.HasNotPermission:
                    TempData[ErrorMessage] = "شما دسترسی لازم برای انجام فرایند مورد نظر را ندارید";
                    TempData[InfoMessage] = "در حال حاضر نمی توانید درخواست جدیدی ثبت کنید";
                    break;
                default:
                    break;
            }
        }

        return View(seller);
    }

    #endregion

    #region seller requests

    [HttpGet("seller-requests")]
    public async Task<IActionResult> SellerRequests(FilterSellerDTO filter)
    {
        filter.TakeEntity = 1;
        filter.UserId = User.GetUserId();
        filter.State = FilterSellerState.All;

        return View(await _sellerService.FilterSellers(filter));
    }

    #endregion

    #region edit request

    [HttpGet("edit-request-seller/{id}")]
    public async Task<IActionResult> EditRequestSeller(long id)
    {
        var requestSeller = await _sellerService.GetRequestSellerForEdit(id, User.GetUserId());
        if (requestSeller == null) return NotFound();

        return View(requestSeller);
    }

    [HttpPost("edit-request-seller/{id}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRequestSeller(EditRequestSellerDTO request)
    {
        if (ModelState.IsValid)
        {
            var res = await _sellerService.EditRequestSeller(request, User.GetUserId());

            switch (res)
            {
                case EditRequestSellerResult.NotFound:
                    TempData[ErrorMessage] = "اطلاعات مورد نظر یافت نشد";
                    break;
                case EditRequestSellerResult.Success:
                    TempData[SuccessMessage] = "اطلاعات مورد نظر با موفقیت ویرایش شد";
                    TempData[InfoMessage] = "فرایند تایید اطلاعات از سر گرفته شد";
                    return RedirectToAction("SellerRequests");
            }
        }

        return View(request);
    }

    #endregion
}
