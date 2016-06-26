using WebApiClient.Models.Account;

namespace WebApiClient.Interfaces
{
    public interface IAccount
    {
        CreateAccountResponse CreateAccount(CreateAccountRequest request);
        LoginResponse Login(LoginRequest request);
        LogoutResponse Logout(LogoutRequest request);
        ChangePasswordResponse ChangePassword(ChangePasswordRequest request);
    }
}