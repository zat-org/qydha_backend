
using static Dapper.SqlMapper;

namespace Qydha.Infrastructure.Repositories;

public class UserRepo(IDbConnection dbConnection, ILogger<UserRepo> logger) : GenericRepository<User>(dbConnection, logger), IUserRepo
{

    #region  add User
    public override async Task<Result<User>> AddAsync<IdT>(User entity, bool excludeKey = true)
    {

        var insertUserParameters = new DynamicParameters();
        var insertSettingsParameters = new DynamicParameters();
        var userTableTuple = User.GetInsertQueryData(entity, excludeKey: true);
        var generalTableTuple = UserGeneralSettings.GetInsertQueryData(new UserGeneralSettings(), excludeKey: true);
        var handTableTuple = UserHandSettings.GetInsertQueryData(new UserHandSettings(), excludeKey: true);
        var balootTableTuple = UserBalootSettings.GetInsertQueryData(new UserBalootSettings(), excludeKey: true);

        insertUserParameters.AddDynamicParams(userTableTuple.Item3);
        insertSettingsParameters.AddDynamicParams(generalTableTuple.Item3);
        insertSettingsParameters.AddDynamicParams(handTableTuple.Item3);
        insertSettingsParameters.AddDynamicParams(balootTableTuple.Item3);

        string insertUserQuery = @$"
            INSERT INTO {User.GetTableName()}  ({userTableTuple.Item1}) 
            VALUES ({userTableTuple.Item2}) 
            RETURNING {User.GetKeyColumnName()};
            ";
        string insertSettingsQuery = @$"
                INSERT INTO {UserGeneralSettings.GetTableName()} 
                ( {UserGeneralSettings.GetKeyColumnName()} , {generalTableTuple.Item1})
                VALUES ( @new_user_id, {generalTableTuple.Item2} );

                INSERT INTO {UserHandSettings.GetTableName()} 
                ( {UserHandSettings.GetKeyColumnName()} ,{handTableTuple.Item1})
                VALUES  (@new_user_id , {handTableTuple.Item2}) ;

                INSERT INTO {UserBalootSettings.GetTableName()} 
                ( {UserBalootSettings.GetKeyColumnName()}  , {balootTableTuple.Item1})
                VALUES  (@new_user_id , {balootTableTuple.Item2}) ;
            ";
        using var transaction = dbConnection.BeginTransaction();

        try
        {

            Guid entityId = await _dbConnection.QuerySingleAsync<Guid>(insertUserQuery, insertUserParameters, transaction);
            entity.Id = entityId;
            insertSettingsParameters.Add("@new_user_id", entityId);
            await _dbConnection.ExecuteAsync(insertSettingsQuery, insertSettingsParameters, transaction);
            transaction.Commit();
            return Result.Ok(entity);
        }
        catch (Exception exp)
        {
            transaction.Rollback();
            _logger.LogCritical(exp, $"error from db : {exp.Message} ");
            return Result.Fail<User>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }

    }
    #endregion

    #region getUser
    public async Task<Result<Tuple<User, UserGeneralSettings?, UserHandSettings?, UserBalootSettings?>>> GetUserWithSettingsByIdAsync(Guid userId)
    {
        try
        {
            string sql = @$"
            SELECT {User.GetColumnsAndPropsForGet(excludeKey: false)}
            from {User.GetTableName()}
            where {User.GetKeyColumnName()} = @userId;

            SELECT {UserGeneralSettings.GetColumnsAndPropsForGet(excludeKey: false)}
            from {UserGeneralSettings.GetTableName()}
            where {UserGeneralSettings.GetKeyColumnName()} = @userId;

            SELECT {UserHandSettings.GetColumnsAndPropsForGet(excludeKey: false)}
            from {UserHandSettings.GetTableName()}
            where {UserHandSettings.GetKeyColumnName()} = @userId;

            SELECT {UserBalootSettings.GetColumnsAndPropsForGet(excludeKey: false)}
            from {UserBalootSettings.GetTableName()}
            where {UserBalootSettings.GetKeyColumnName()} = @userId;
            ";
            using GridReader multi = await _dbConnection.QueryMultipleAsync(sql, new { userId });
            User? user = await multi.ReadSingleOrDefaultAsync<User>();
            UserGeneralSettings? userGeneralSettings = await multi.ReadSingleOrDefaultAsync<UserGeneralSettings>();
            UserHandSettings? userHandSettings = await multi.ReadSingleOrDefaultAsync<UserHandSettings>();
            UserBalootSettings? userBalootSettings = await multi.ReadSingleOrDefaultAsync<UserBalootSettings>();
            if (user is null)
                return Result.Fail<Tuple<User, UserGeneralSettings?, UserHandSettings?, UserBalootSettings?>>(new()
                {
                    Code = _notFoundError,
                    Message = $"{_notFoundError} :: Entity not found"
                });
            if (userGeneralSettings is null || userHandSettings is null || userBalootSettings is null)
            {
                _logger.LogError("user with {id} do not have one of this settings => general settings : {hasGS} , hand settings : {hasHS} , baloot settings : {hasBS}", user.Id, userGeneralSettings is null, userHandSettings is null, userBalootSettings is null);
            }
            return Result.Ok(new Tuple<User, UserGeneralSettings?, UserHandSettings?, UserBalootSettings?>(user, userGeneralSettings, userHandSettings, userBalootSettings));
        }
        catch (Exception exp)
        {
            _logger.LogCritical(exp, "error from db : {msg} ", exp.Message);
            return Result.Fail<Tuple<User, UserGeneralSettings?, UserHandSettings?, UserBalootSettings?>>(new()
            {
                Code = ErrorType.ServerErrorOnDB,
                Message = exp.Message
            });
        }
    }

    public async Task<Result<IEnumerable<User>>> GetAllRegularUsers() =>
         await GetAllAsync(filterCriteria: $"{User.GetColumnName(nameof(User.IsAnonymous))} = false",
                                parameters: new { },
                                orderCriteria: "");

    public async Task<Result<User>> GetByIdAsync(Guid id) =>
        await GetByUniquePropAsync(nameof(User.Id), id);

    public async Task<Result<User>> GetByPhoneAsync(string phone) =>
        await GetByUniquePropAsync(nameof(User.Phone), phone);

    public async Task<Result<User>> GetByEmailAsync(string email) =>
        await GetByUniquePropAsync(nameof(User.NormalizedEmail), email.ToUpper());

    public async Task<Result<User>> GetByUsernameAsync(string username) =>
        await GetByUniquePropAsync(nameof(User.NormalizedUsername), username.ToUpper());

    public async Task<Result> IsUsernameAvailable(string username, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "اسم المستخدم موجود بالفعل"
            });
        return Result.Ok();
    }

    public async Task<Result> IsPhoneAvailable(string phone)
    {
        Result<User> getUserRes = await GetByPhoneAsync(phone);
        if (getUserRes.IsSuccess)
            return Result.Fail(new Error
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "رقم الجوال موجود بالفعل"
            });
        return Result.Ok();
    }

    public async Task<Result> IsEmailAvailable(string email, Guid? userId = null)
    {
        Result<User> getUserRes = await GetByEmailAsync(email);
        if (getUserRes.IsSuccess && ((userId is null) || (userId is not null && getUserRes.Value.Id != userId)))
            return Result.Fail(new Error
            {
                Code = ErrorType.DbUniqueViolation,
                Message = "البريد الالكتروني موجود بالفعل."
            });
        return Result.Ok();
    }

    #endregion

    #region editUser    
    public async Task<Result> UpdateUserLastLoginToNow(Guid userId) =>
                await PatchById(userId,
                                nameof(User.LastLogin),
                                 DateTime.UtcNow);

    public async Task<Result> UpdateUserFCMToken(Guid userId, string fcmToken) =>
                await PatchById(userId,
                                nameof(User.FCMToken),
                               fcmToken);

    public async Task<Result> UpdateUserPassword(Guid userId, string passwordHash) =>
                await PatchById(userId,
                                nameof(User.PasswordHash),
                                passwordHash);
    public async Task<Result> UpdateUserUsername(Guid userId, string username) =>
                await PatchById(userId,
                    new Dictionary<string, object>(){
                        { nameof(User.Username) ,username},
                        { nameof(User.NormalizedUsername),username.ToUpper()}
                    });
    public async Task<Result> UpdateUserPhone(Guid userId, string phone) =>
                   await PatchById(userId,
                                nameof(User.Phone),
                                phone);

    public async Task<Result> UpdateUserEmail(Guid userId, string email) =>
                await PatchById(userId,
                new Dictionary<string, object>(){
                        { nameof(User.Email),email},
                        { nameof(User.NormalizedEmail),email.ToUpper()},
                        { nameof(User.IsEmailConfirmed) ,true}
                    });


    public async Task<Result> UpdateUserAvatarData(Guid userId, string avatarPath, string avatarUrl) =>
                await PatchById(userId,
                 new Dictionary<string, object>(){
                        { nameof(User.AvatarPath),avatarPath},
                        { nameof(User.AvatarUrl),avatarUrl}
                    });

    #endregion

    public async Task<Result<User>> CheckUserCredentials(Guid userId, string password)
    {
        Result<User> getUserRes = await GetByIdAsync(userId);
        return getUserRes.OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidCredentials,
                    Message = "كلمة المرور غير صحيحة"
                });
            return Result.Ok(user);
        });
    }

    public async Task<Result<User>> CheckUserCredentials(string username, string password)
    {
        Result<User> getUserRes = await GetByUsernameAsync(username);
        return getUserRes.OnSuccess<User>((user) =>
        {
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return Result.Fail<User>(new()
                {
                    Code = ErrorType.InvalidCredentials,
                    Message = "كلمة المرور غير صحيحة"
                });
            return Result.Ok(user);
        });
    }


}