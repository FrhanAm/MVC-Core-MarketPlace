﻿using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.Seller;
using System.Net.NetworkInformation;

namespace MarketPlace.DataLayer.DTOs.SellerWallet;

public class FilterSellerWalletDTO : BasePaging
{
	public long? SellerId { get; set; }

	public int? PriceFrom { get; set; }

	public int? PriceTo { get; set; }

	public List<Entities.Wallet.SellerWallet> SellerWallets { get; set; }

	#region methods

	public FilterSellerWalletDTO SetSellerWallets(List<Entities.Wallet.SellerWallet> wallets)
	{
		SellerWallets = wallets;
		return this;
	}

	public FilterSellerWalletDTO SetPaging(BasePaging paging)
	{
		this.PageId = paging.PageId;
		this.AllEntitiesCount = paging.AllEntitiesCount;
		this.StartPage = paging.StartPage;
		this.EndPage = paging.EndPage;
		this.HowManyShowPageAfterAndBefore = paging.HowManyShowPageAfterAndBefore;
		this.TakeEntity = paging.TakeEntity;
		this.SkipEntity = paging.SkipEntity;
		this.PageCount = paging.PageCount;
		return this;
	}

	#endregion
}
