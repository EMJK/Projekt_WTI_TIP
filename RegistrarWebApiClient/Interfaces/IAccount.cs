using RegistrarWebApiClient.Models.Account;

namespace RegistrarWebApiClient.Interfaces
{
    public interface IAccount
    {
        RegisterAccountResponse Register(RegisterAccountRequest data);
        LoginResponse Login(LoginRequest request);
        LogoutResponse Logout(LogoutRequest request);
    }
}