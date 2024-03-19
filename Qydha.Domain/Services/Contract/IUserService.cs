﻿namespace Qydha.Domain.Services.Contracts;
public interface IUserService
{
    Task<Result<IEnumerable<User>>> GetAllRegularUsers();
    Task<Result<User>> GetUserById(Guid userId);
    Task<Result<User>> GetUserWithSettingsByIdAsync(Guid userId);

    Task<Result> IsUserNameAvailable(string username);

    Task<Result<User>> UpdateUser(User user);
    Task<Result<User>> UpdateUserPassword(Guid userId, string oldPassword, string newPassword);
    Task<Result<User>> UpdateUserPassword(Guid userId, Guid phoneAuthReqId, string newPassword);
    Task<Result<User>> UpdateUserUsername(Guid userId, string password, string newUsername);
    Task<Result<UpdatePhoneRequest>> UpdateUserPhone(Guid userId, string password, string newPhone);
    Task<Result<User>> ConfirmPhoneUpdate(Guid userId, string code, Guid requestId);
    Task<Result<UpdateEmailRequest>> UpdateUserEmail(Guid userId, string password, string newEmail);
    Task<Result<User>> ConfirmEmailUpdate(Guid userId, string code, Guid requestId);
    Task<Result<User>> UploadUserPhoto(Guid userId, IFormFile file);
    Task<Result> UpdateFCMToken(Guid userId, string token);


    Task<Result<User>> DeleteUser(Guid userId, string password);
    Task<Result<User>> DeleteAnonymousUser(Guid userId);

    Task<Result<UserGeneralSettings>> GetUserGeneralSettings(Guid userId);
    Task<Result<UserGeneralSettings>> UpdateUserGeneralSettings(UserGeneralSettings settings);

    Task<Result<UserHandSettings>> GetUserHandSettings(Guid userId);
    Task<Result<UserHandSettings>> UpdateUserHandSettings(UserHandSettings settings);

    Task<Result<UserBalootSettings>> GetUserBalootSettings(Guid userId);
    Task<Result<UserBalootSettings>> UpdateUserBalootSettings(UserBalootSettings settings);

}
