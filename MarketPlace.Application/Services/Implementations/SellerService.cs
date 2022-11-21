using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.DTOs.Common;
using MarketPlace.DataLayer.DTOs.Paging;
using MarketPlace.DataLayer.DTOs.Seller;
using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Entities.Store;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations;

public class SellerService : ISellerService
{
    #region constructor

    private readonly IGenericRepository<Seller> _sellerRepository;
    private readonly IGenericRepository<User> _userRepository;

    public SellerService(IGenericRepository<Seller> sellerRepository, IGenericRepository<User> userRepository)
    {
        _sellerRepository = sellerRepository;
        _userRepository = userRepository;
    }

    #endregion

    #region seller

    public async Task<RequestSellerResult> AddNewSellerRequest(RequestSellerDTO seller, long userId)
    {
        var user = await _userRepository.GetEntityById(userId);

        if (user.IsBlocked) return RequestSellerResult.HasNotPermission;

        var hasUnderProgressRequest = await _sellerRepository.GetQuery().AsQueryable()
            .AnyAsync(x => x.UserId == userId && x.StoreAcceptanceState == StoreAcceptanceState.UnderProgress);

        if (hasUnderProgressRequest) return RequestSellerResult.HasUnderProgressRequest;

        var newSeller = new Seller
        {
            UserId = userId,
            StoreName = seller.StoreName,
            Phone = seller.Phone,
            Address = seller.Address,
            StoreAcceptanceState = StoreAcceptanceState.UnderProgress,
        };

        await _sellerRepository.AddEntity(newSeller);
        await _sellerRepository.SaveChanges();

        return RequestSellerResult.Success;
    }

    public async Task<FilterSellerDTO> FilterSellers(FilterSellerDTO filter)
    {
        var query = _sellerRepository.GetQuery()
            .Include(x => x.User)
            .AsQueryable().Where(x => !x.IsDeleted);

        #region state

        switch (filter.State)
        {
            case FilterSellerState.All:
                break;
            case FilterSellerState.UnderProgress:
                query = query.Where(x => x.StoreAcceptanceState == StoreAcceptanceState.UnderProgress);
                break;
            case FilterSellerState.Accepted:
                query = query.Where(x => x.StoreAcceptanceState == StoreAcceptanceState.Accepted);
                break;
            case FilterSellerState.Rejected:
                query = query.Where(x => x.StoreAcceptanceState == StoreAcceptanceState.Rejected);
                break;
        }

        #endregion

        #region filter

        if (filter.UserId != null && filter.UserId != 0)
            query = query.Where(x => x.UserId == filter.UserId);

        if (!string.IsNullOrEmpty(filter.StoreName))
            query = query.Where(x => EF.Functions.Like(x.StoreName, $"%{filter.StoreName}%"));

        if (!string.IsNullOrEmpty(filter.Phone))
            query = query.Where(x => EF.Functions.Like(x.Phone, $"%{filter.Phone}%"));

        if (!string.IsNullOrEmpty(filter.Mobile))
            query = query.Where(x => EF.Functions.Like(x.Mobile, $"%{filter.Mobile}%"));

        if (!string.IsNullOrEmpty(filter.Address))
            query = query.Where(x => EF.Functions.Like(x.Address, $"%{filter.Address}%"));

        #endregion

        #region paging

        var pager = Pager.Build(filter.PageId, await query.CountAsync(), filter.TakeEntity, filter.HowManyShowPageAfterAndBefore);
        var allEntities = await query.Paging(pager).ToListAsync();

        #endregion

        return filter.SetPaging(pager).SetSellers(allEntities);
    }

    public async Task<EditRequestSellerDTO> GetRequestSellerForEdit(long id, long currentUserId)
    {
        var seller = await _sellerRepository.GetEntityById(id);
        if (seller == null || seller.UserId != currentUserId) return null;
        return new EditRequestSellerDTO
        {
            Id = seller.Id,
            Phone = seller.Phone,
            Address = seller.Address,
            StoreName = seller.StoreName
        };
    }

    public async Task<EditRequestSellerResult> EditRequestSeller(EditRequestSellerDTO request, long currentUserId)
    {
        var seller = await _sellerRepository.GetEntityById(request.Id);
        if (seller == null || seller.UserId != currentUserId) return EditRequestSellerResult.NotFound;

        seller.Phone = request.Phone;
        seller.Address = request.Address;
        seller.StoreName = request.StoreName;
        seller.StoreAcceptanceState = StoreAcceptanceState.UnderProgress;

        _sellerRepository.EditEntity(seller);
        await _sellerRepository.SaveChanges();

        return EditRequestSellerResult.Success;
    }

    public async Task<bool> AcceptSellerRequest(long requestId)
    {
        var sellerRequest = await _sellerRepository.GetEntityById(requestId);
        if (sellerRequest != null)
        {
            sellerRequest.StoreAcceptanceState = StoreAcceptanceState.Accepted;
            sellerRequest.StoreAcceptanceDescription = "اطلاعات پنل فروشندگی شما تایید شده است";
            _sellerRepository.EditEntity(sellerRequest);
            await _sellerRepository.SaveChanges();

            return true;
        }

        return false;
    }

    public async Task<bool> RejectSellerRequest(RejectItemDTO reject)
    {
        var seller = await _sellerRepository.GetEntityById(reject.Id);
        if (seller != null)
        {
            seller.StoreAcceptanceState = StoreAcceptanceState.Rejected;
            seller.StoreAcceptanceDescription = reject.RejectMessage;
            _sellerRepository.EditEntity(seller);
            await _sellerRepository.SaveChanges();

            return true;
        }

        return false;
    }

    public async Task<Seller> GetLastActiveSellerByUserId(long userId)
    {
        return await _sellerRepository.GetQuery()
            .OrderByDescending(x => x.CreateDate)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.StoreAcceptanceState == StoreAcceptanceState.Accepted);
    }

    public async Task<bool> HasUserAnyActiveSellerPanel(long userId)
    {
        return await _sellerRepository.GetQuery()
           .OrderByDescending(x => x.CreateDate)
           .AnyAsync(x => x.UserId == userId && x.StoreAcceptanceState == StoreAcceptanceState.Accepted);
    }

    #endregion

    #region dispose

    public async ValueTask DisposeAsync()
    {
        await _sellerRepository.DisposeAsync();
    }

    #endregion
}
