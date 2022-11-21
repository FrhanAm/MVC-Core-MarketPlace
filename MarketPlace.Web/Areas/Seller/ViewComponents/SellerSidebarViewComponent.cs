using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.ViewComponents;

public class SellerSidebar : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View("SellerSidebar");
    }
}
