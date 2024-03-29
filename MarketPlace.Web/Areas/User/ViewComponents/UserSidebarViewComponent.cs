﻿using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.ViewComponents;

public class UserSidebar : ViewComponent
{
    private ISellerService _sellerService;

    public UserSidebar(ISellerService sellerService)
    {
        _sellerService = sellerService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        ViewBag.hasUserAnyActiveSellerPanel = await _sellerService.HasUserAnyActiveSellerPanel(User.GetUserId());

        return View("UserSidebar");
    }
}
