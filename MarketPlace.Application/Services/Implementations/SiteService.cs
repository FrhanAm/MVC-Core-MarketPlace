using MarketPlace.Application.Services.Interfaces;
using MarketPlace.DataLayer.Entities.Site;
using MarketPlace.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

namespace MarketPlace.Application.Services.Implementations
{
    public class SiteService : ISiteService
    {
        #region constructor

        private readonly IGenericRepository<SiteSetting> _siteSettingRepository;
        private readonly IGenericRepository<Slider> _sliderRepository;
        private readonly IGenericRepository<SiteBanner> _bannerRepository;

        public SiteService(IGenericRepository<SiteSetting> siteSettingRepository
            , IGenericRepository<Slider> sliderRepository
            , IGenericRepository<SiteBanner> bannerRepository)
        {
            _siteSettingRepository = siteSettingRepository;
            _sliderRepository = sliderRepository;
            _bannerRepository = bannerRepository;
        }


        #endregion

        #region site settings

        public async Task<SiteSetting?> GetDefaultSiteSetting()
        {
            return await _siteSettingRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.IsDefault && !x.IsDeleted);
        }

        #endregion

        #region slider

        public async Task<List<Slider>> GetAllActiveSliders()
        {
            return await _sliderRepository.GetQuery().AsQueryable()
                .Where(x => x.IsActive && !x.IsDeleted).ToListAsync();
        }

        #endregion

        #region site banners

        public async Task<List<SiteBanner>> GetSiteBannerByPlacement(List<BannerPlacement> placements)
        {
            return await _bannerRepository.GetQuery().AsQueryable()
                .Where(x => placements.Contains(x.BannerPlacement)).ToListAsync();
        }

        #endregion

        #region dispose

        public async ValueTask DisposeAsync()
        {
            if (_siteSettingRepository != null) await _siteSettingRepository.DisposeAsync();
            if (_sliderRepository != null) await _sliderRepository.DisposeAsync();
            if (_bannerRepository != null) await _bannerRepository.DisposeAsync();
        }

        #endregion
    }
}
