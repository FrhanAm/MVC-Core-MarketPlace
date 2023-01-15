using GoogleReCaptcha.V3.Interface;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MarketPlace.Web.Controllers;

public class AccountController : SiteBaseController
{
    #region constructor

    private readonly IUserService _userService;
    private readonly ICaptchaValidator _captchaValidator;

    public AccountController(IUserService userService, ICaptchaValidator captchaValidator)
    {
        _userService = userService;
        _captchaValidator = captchaValidator;
    }

    #endregion

    #region register

    [HttpGet("register")]
    public IActionResult Register()
    {
        if (User.Identity.IsAuthenticated) return Redirect("/");
        return View();
    }

    [HttpPost("register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterUserDTO register)
    {
        if (!await _captchaValidator.IsCaptchaPassedAsync(register.Captcha))
        {
            TempData[ErrorMessage] = "کد کپچای شما نا معتبر است";
            return View(register);
        }

        if (ModelState.IsValid)
        {
            var res = await _userService.RegisterUser(register);
            switch (res)
            {
                case RegisterUserResult.MobileExists:
                    TempData[ErrorMessage] = "تلفن همراه وارد شده تکراری می باشد";
                    ModelState.AddModelError("Mobile", "تلفن همراه وارد شده تکراری می باشد");
                    break;
                case RegisterUserResult.Success:
                    TempData[SuccessMessage] = "ثبت نام شما با موفقیت انجام شد";
                    TempData[InfoMessage] = "کد تایید تلفن همراه برای شما ارسال شد";
                    return RedirectToAction("ActivateEmail", "Account", new { email = register.Email });
            }
        }

        return View(register);
    }

    #endregion

    #region login

    [HttpGet("login")]
    public IActionResult Login()
    {
        if (User.Identity.IsAuthenticated) return Redirect("/");
        return View();
    }

    [HttpPost("login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginUserDTO login)
    {
        if (!await _captchaValidator.IsCaptchaPassedAsync(login.Captcha))
        {
            TempData[ErrorMessage] = "کد کپچای شما نا معتبر است";
            return View(login);
        }

        if (ModelState.IsValid)
        {
            var res = await _userService.GetUserForLogin(login);
            switch (res)
            {
                case LoginUserResult.Success:
                    //var user = await _userService.GetUserByMobile(login.Email);
                    var user = await _userService.GetUserByEmail(login.Email);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name,user.Email),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = login.RememberMe
                    };

                    await HttpContext.SignInAsync(principal, properties);

                    TempData[SuccessMessage] = "عملیات ورود با موفقیت انحام شد";
                    return Redirect("/");
                case LoginUserResult.NotFound:
                    TempData[ErrorMessage] = "کاربر مورد نظر یافت نشد";
                    break;
                case LoginUserResult.NotActivated:
                    TempData[WarningMessage] = "حساب کاربری شما فعال نشده است";
                    break;
            }
        }
        return View();
    }

    #endregion

    #region activate mobile

    [HttpGet("activate-mobile/{mobile}")]
    public IActionResult ActivateMobile(string mobile)
    {
        if (User.Identity.IsAuthenticated) return Redirect("/");
        var activateMobileDto = new ActivateMobileDTO { Mobile = mobile };
        return View(activateMobileDto);
    }

    [HttpPost("activate-mobile/{mobile}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateMobile(ActivateMobileDTO activate)
    {
        if (!await _captchaValidator.IsCaptchaPassedAsync(activate.Captcha))
        {
            TempData[ErrorMessage] = "کد کپچای شما نا معتبر است";
            return View(activate);
        }

        if (ModelState.IsValid)
        {
            var res = await _userService.ActivateMobile(activate);
            if (res)
            {
                TempData[SuccessMessage] = "حساب کاربری شما با موفقیت فعال شد";
                return RedirectToAction("Login");
            }
            TempData[ErrorMessage] = "کاربری با مشخصات وارد شده یافت نشد";
        }

        return View(activate);
    }

    #endregion

    #region activate email

    [HttpGet("activate-email/{email}")]
    public IActionResult ActivateEmail(string email)
    {
        if (User.Identity.IsAuthenticated) return Redirect("/");
        var activateEmailDto = new ActivateEmailDTO { Email = email };
        return View(activateEmailDto);
    }

    [HttpPost("activate-email/{email}"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ActivateEmail(ActivateEmailDTO activate)
    {
        if (!await _captchaValidator.IsCaptchaPassedAsync(activate.Captcha))
        {
            TempData[ErrorMessage] = "کد کپچای شما نا معتبر است";
            return View(activate);
        }

        if (ModelState.IsValid)
        {
            var res = await _userService.ActivateEmail(activate);
            if (res)
            {
                TempData[SuccessMessage] = "حساب کاربری شما با موفقیت فعال شد";
                return RedirectToAction("Login");
            }
            TempData[ErrorMessage] = "کاربری با مشخصات وارد شده یافت نشد";
        }

        return View(activate);
    }

    #endregion

    #region forgot password

    [HttpGet("forgot-pass")]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpPost("forgot-pass"), ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgot)
    {
        if (!await _captchaValidator.IsCaptchaPassedAsync(forgot.Captcha))
        {
            TempData[ErrorMessage] = "کد کپچای شما نا معتبر است";
            return View(forgot);
        }

        if (ModelState.IsValid)
        {
            var result = await _userService.RecoverUserPassword(forgot);
            switch (result)
            {
                case ForgotPasswordResult.Success:
                    TempData[SuccessMessage] = "کلمه ی عبور جدید برای شما ارسال شد";
                    TempData[InfoMessage] = "لطفا پس از ورود به حساب کاربری، کلمه ی عبور خود را تغییر دهید";
                    return Redirect("Login");
                case ForgotPasswordResult.NotFound:
                    TempData[WarningMessage] = "کاربر مورد نظر یافت نشد";
                    break;
            }
        }

        return View(forgot);
    }

    #endregion

    #region log out

    [HttpGet("log-out")]
    public async Task<IActionResult> LogOut()
    {
        await HttpContext.SignOutAsync();

        return Redirect("/");
    }

    #endregion
}
