﻿using MarketPlace.Web.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers;

[Authorize]
[Area("Seller")]
[Route("seller")]
[CheckSellerStateAttriburte]
public class SellerBaseController : Controller
{
    protected string ErrorMessage = "ErrorMessage";
    protected string SuccessMessage = "SuccessMessage";
    protected string InfoMessage = "InfoMessage";
    protected string WarningMessage = "WarningMessage";
}
