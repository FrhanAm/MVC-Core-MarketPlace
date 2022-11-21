using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.DataLayer.Entities.Account;
using Microsoft.AspNetCore.Http;

namespace MarketPlace.Application.Services.Interfaces
{
    public interface IUserService : IAsyncDisposable
    {
        #region account

        Task<RegisterUserResult> RegisterUser(RegisterUserDTO register);
        Task<bool> IsUserExistsByMobileNumber(string mobile);
        Task<LoginUserResult> GetUserForLogin(LoginUserDTO login);
        Task<User?> GetUserByMobile(string mobile);
        Task<User?> GetUserByEmail(string email);
        Task<ForgotPasswordResult> RecoverUserPassword(ForgotPasswordDTO forgot); 
        Task<bool> ActivateMobile(ActivateMobileDTO activate);
        Task<bool> ActivateEmail(ActivateEmailDTO activate);
        Task<bool> ChangeUserPassword(ChangePasswordDTO changePass, long currnetUserId);
        Task<EditUserProfileDTO> GetProfileForEdit(long UserId);
        Task<EditUserProfileResult> EditUserProfile(EditUserProfileDTO profile, long userId, IFormFile avatarImage);

        #endregion
    }
}
