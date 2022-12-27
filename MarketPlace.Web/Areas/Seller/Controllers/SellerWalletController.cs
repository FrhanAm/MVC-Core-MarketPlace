using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.SellerWallet;
using Microsoft.AspNetCore.Mvc;

namespace MarketPlace.Web.Areas.Seller.Controllers
{
	public class SellerWalletController : SellerBaseController
	{
		#region constructor

		private readonly ISellerWalletService _sellerWalletService;

		public SellerWalletController(ISellerWalletService sellerWalletService)
		{
			_sellerWalletService= sellerWalletService;
		}

        #endregion

        #region index

        [HttpGet("seller-wallet")]
        public async Task<IActionResult> Index(FilterSellerWalletDTO filter)
        {
            filter.TakeEntity = 5;
            return View(await _sellerWalletService.FilterSellerWallet(filter));
        }

        #endregion
    }
}
