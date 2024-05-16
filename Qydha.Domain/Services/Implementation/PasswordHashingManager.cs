namespace Qydha.Domain.Services.Implementation;

public static class PasswordHashingManager
{
    public static Result VerifyPassword(string password, string hash)
    {
        if (!BCrypt.Net.BCrypt.Verify(password, hash))
            return Result.Fail(new InvalidCredentialsError("كلمة المرور غير صحيحة"));
        return Result.Ok();
    }

    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}
