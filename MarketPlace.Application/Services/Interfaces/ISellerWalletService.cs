using MarketPlace.DataLayer.DTOs.SellerWallet;

namespace MarketPlace.Application.Services.Interfaces;

public interface ISellerWalletService
{
	#region wallet

	Task<FilterSellerWalletDTO> FilterSellerWallet(FilterSellerWalletDTO filter);

	#endregion
}
