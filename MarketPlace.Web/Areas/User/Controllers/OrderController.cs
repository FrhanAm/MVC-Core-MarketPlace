﻿using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Orders;
using MarketPlace.Web.Http;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers;

public class OrderController : UserBaseController
{
	#region constructor

	private readonly IOrderServcie _orderServcie;
	private readonly IUserService _userService;

    public OrderController(IOrderServcie orderServcie, IUserService userService)
    {
        _orderServcie = orderServcie;
        _userService = userService;
    }

    #endregion

    #region add product to open order

    [AllowAnonymous]
    [HttpPost("add-product-to-order")]
    public async Task<IActionResult> AddProductToOrder(AddProductToOrderDTO order)
    {
        if (ModelState.IsValid)
        {
            if (User.Identity.IsAuthenticated)
            {
                await _orderServcie.AddProductToOpenOrder(User.GetUserId(), order);
                return JsonResponseStatus.SendStatus(
                    JsonResponseStatusType.Success,
                    "محصول مورد نظر با موفقیت ثبت شد");
            }
            else
            {
                return JsonResponseStatus.SendStatus(
                    JsonResponseStatusType.Danger,
                    "برای ثبت محصول در سبد خرید ابتدا باید وارد سایت شوید");
            }
        }

        return JsonResponseStatus.SendStatus(JsonResponseStatusType.Danger,
            "در ثبت اطلاعات خطایی رخ داد");
    }

    #endregion
}
