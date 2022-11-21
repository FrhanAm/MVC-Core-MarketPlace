﻿using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.Web.PresentationExtensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.User.Controllers;

public class AccountController : UserBaseController
{
    #region constructor

    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    #endregion

    #region change password

    [HttpGet("change-password")]
    public async Task<IActionResult> ChangePassword()
    {
        return View();
    }
    
    [HttpPost("change-password"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordDTO passwordDTO)
    {
        if (ModelState.IsValid)
        {
            var res = await _userService.ChangeUserPassword(passwordDTO, User.GetUserId());
            if (res)
            {
                TempData[SuccessMessage] = "کلمه ی عبور شما تغییر یافت";
                TempData[InfoMessage] = "لطفا جهت تکمیل فرایند تغییر کلمه ی عبور، مجددا وارد شوید";
                await HttpContext.SignOutAsync();
                return RedirectToAction("Login", "Account", new { area = "" });
            }
            else
            {
                TempData[ErrorMessage] = "لطفا از کلمه ی عبور جدیدی استفاده کنید";
                ModelState.AddModelError("NewPassword", "لطفا از کلمه ی عبور جدیدی استفاده کنید");
            }

        }

        return View(passwordDTO);
    }

    #endregion

    #region edit profile

    [HttpGet("edit-profile")]
    public async Task<IActionResult> EditProfile()
    {
        var userProfile = await _userService.GetProfileForEdit(User.GetUserId());
        if (userProfile == null) return NotFound();
        return View(userProfile);
    }
    
    [HttpPost("edit-profile"), ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditUserProfileDTO profile, IFormFile? avatarImage)
    {
        if (ModelState.IsValid)
        {
            var result = await _userService.EditUserProfile(profile, User.GetUserId(), avatarImage);
            switch (result)
            {
                case EditUserProfileResult.IsBlocked:
                    TempData[ErrorMessage] = "حساب کاربری شما مسدود شده است";
                    break;
                case EditUserProfileResult.IsNotActive:
                    TempData[ErrorMessage] = "حساب کاربری شما فعال نیست";
                    break;
                case EditUserProfileResult.NotFound:
                    TempData[ErrorMessage] = "کاربری با مشخصات وارد شده یافت نشد";
                    break;
                case EditUserProfileResult.Success:
                    TempData[SuccessMessage] = $"جناب {profile.FirstName} {profile.LastName} پروفایل شما با موفقیت ویرایش شد";
                    return RedirectToAction("EditProfile");
                default:
                    break;
            }
        }

        return View(profile);
    }

    #endregion
}
