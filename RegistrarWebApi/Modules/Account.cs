using RegistrarCommon;
using RegistrarWebApiClient.Interfaces;
using RegistrarWebApiClient.Models.Account;

namespace RegistrarWebApi.Modules
{
    public class Account : ModuleBase, IAccount
    {
        private readonly ISessionCache _cache;

        public Account(ISessionCache cache)
        {
            _cache = cache;
        }

        public RegisterAccountResponse Register(RegisterAccountRequest data)
        {
            return new RegisterAccountResponse()
            {
                Message = $"A new account for \"{data.UserID.ToLower()}\" was created."
            };
        }

        public LoginResponse Login(LoginRequest request)
        {
            var session = _cache.CreateSession(request.UserID.ToLower());
            return new LoginResponse()
            {
                SessionID = session.SessionID
            };
        }

        public LogoutResponse Logout(LogoutRequest request)
        {
            _cache.CloseSession(request.SessionID);
            return new LogoutResponse();
        }
    }
}
