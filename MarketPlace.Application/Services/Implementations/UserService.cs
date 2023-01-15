using MarketPlace.Application.Extensions;
using MarketPlace.Application.Services.Interfaces;
using MarketPlace.Application.Utils;
using MarketPlace.DataLayer.DTOs.Account;
using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace MarketPlace.Application.Services.Implementations;

public class UserService : IUserService
{
    #region constructor
    private readonly IGenericRepository<User> _userRepository;
    private readonly IPasswordHelper _passwordHelper;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;

    public UserService(IGenericRepository<User> userRepository, IPasswordHelper passwordHelper,
        ISmsService smsService, IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHelper = passwordHelper;
        _smsService = smsService;
        _emailService = emailService;
    }

    #endregion

    #region account

    public async Task<RegisterUserResult> RegisterUser(RegisterUserDTO register)
    {
        if (!await IsUserExistsByEmail(register.Email))
        {
            var user = new User
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                //Mobile = register.Mobile,
                Email = register.Email,
                Password = _passwordHelper.EncodePasswordMd5(register.Password),
                MobileActiceCode = new Random().Next(10000, 999999).ToString(),
                EmailActiveCode = Guid.NewGuid().ToString(format: "N"),
            };

            await _userRepository.AddEntity(user);
            await _userRepository.SaveChanges();
            await _emailService.SendVerificationEmail(user.Email, user.EmailActiveCode);
            //await _smsService.SendVerificationSms(user.Mobile, user.MobileActiceCode);

            return RegisterUserResult.Success;
        }
        //return RegisterUserResult.MobileExists;
        return RegisterUserResult.EmailExists;
    }

    public async Task<bool> IsUserExistsByMobileNumber(string mobile)
    {
        return await _userRepository.GetQuery().AsQueryable().AnyAsync(x => x.Mobile == mobile);
    }
    
    public async Task<bool> IsUserExistsByEmail(string email)
    {
        return await _userRepository.GetQuery().AsQueryable().AnyAsync(x => x.Email == email);
    }

    public async Task<LoginUserResult> GetUserForLogin(LoginUserDTO login)
    {
        //var user = await _userRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.Mobile == login.Mobile);
        var user = await _userRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.Email == login.Email);
        if (user == null) return LoginUserResult.NotFound;
        //if (!user.IsMobileActive) return LoginUserResult.NotActivated;
        if (!user.IsEmailActive) return LoginUserResult.NotActivated;
        if (user.Password != _passwordHelper.EncodePasswordMd5(login.Password)) return LoginUserResult.NotFound;
        return LoginUserResult.Success;
    }

    public async Task<User?> GetUserByMobile(string mobile)
    {
        return await _userRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.Mobile == mobile);
    }
    
    public async Task<User?> GetUserByEmail(string email)
    {
        return await _userRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.Email == email);
    }

    public async Task<ForgotPasswordResult> RecoverUserPassword(ForgotPasswordDTO forgot)
    {
        var user = await _userRepository.GetQuery().SingleOrDefaultAsync(x => x.Email == forgot.Email);
        if (user == null) return ForgotPasswordResult.NotFound;
        var newPassword = new Random().Next(1000000, 999999999).ToString();
        user.Password = _passwordHelper.EncodePasswordMd5(newPassword);
        _userRepository.EditEntity(user);
        // todo: send new pass to user via sms
        //await _smsService.SendUserPasswordSms(user.Mobile, newPassword);
        await _emailService.SendUserPasswordEmail(user.Email, newPassword);
        await _userRepository.SaveChanges();
        return ForgotPasswordResult.Success;
    }

    public async Task<bool> ActivateMobile(ActivateMobileDTO activate)
    {
        var user = await _userRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.Mobile == activate.Mobile);
        if (user != null)
        {
            if (user.MobileActiceCode == activate.MobileActiceCode)
            {
                user.IsMobileActive = true;
                user.MobileActiceCode = new Random().Next(10000, 999999).ToString();
                await _userRepository.SaveChanges();
                return true;
            }
        }

        return false;
    }

    public async Task<bool> ActivateEmail(ActivateEmailDTO activate)
    {
        var user = await _userRepository.GetQuery().AsQueryable().SingleOrDefaultAsync(x => x.Email == activate.Email);
        if (user != null)
        {
            if (user.EmailActiveCode == activate.EmailActiveCode)
            {
                user.IsEmailActive = true;
                user.EmailActiveCode = Guid.NewGuid().ToString(format: "N");
                await _userRepository.SaveChanges();
                return true;
            }
        }

        return false;
    }

    public async Task<bool> ChangeUserPassword(ChangePasswordDTO changePass, long currnetUserId)
    {
        var user = await _userRepository.GetEntityById(currnetUserId);
        if (user != null)
        {
            var newPassword = _passwordHelper.EncodePasswordMd5(changePass.NewPassword);
            if (newPassword != user.Password)
            {
                user.Password = newPassword;
                _userRepository.EditEntity(user);
                await _userRepository.SaveChanges();

                return true;
            }
        }

        return false;
    }

    public async Task<EditUserProfileDTO> GetProfileForEdit(long userId)
    {
        var user = await _userRepository.GetEntityById(userId);
        if (user == null)
        {
            return null;
        }

        return new EditUserProfileDTO
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Avatar = user.Avatar
        };
    }

    public async Task<EditUserProfileResult> EditUserProfile(EditUserProfileDTO profile, long userId, IFormFile avatarImage)
    {
        var user = await _userRepository.GetEntityById(userId);
        if (user == null) return EditUserProfileResult.NotFound;

        if (user.IsBlocked) return EditUserProfileResult.IsBlocked;
        if (!user.IsEmailActive) return EditUserProfileResult.IsNotActive;

        user.FirstName = profile.FirstName;
        user.LastName = profile.LastName;

        if (avatarImage != null && avatarImage.IsImage())
        {
            var imageName = Guid.NewGuid().ToString("N") + Path.GetExtension(avatarImage.FileName);

            avatarImage.AddImageToServer(
                imageName,
                PathExtension.UserAvatarOriginServer, 
                width: 100, height: 100,
                PathExtension.UserAvatarThumbServer, 
                user.Avatar);

            user.Avatar = imageName;
        }

        _userRepository.EditEntity(user);
        await _userRepository.SaveChanges();

        return EditUserProfileResult.Success;
    }

    #endregion

    #region dispose

    public async ValueTask DisposeAsync()
    {
        await _userRepository.DisposeAsync();
    }

    #endregion
}
