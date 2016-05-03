namespace RegistrarWebApiClient.Models.Account
{
    public class LoginRequest
    {
        public string UserID { get; set; }
    }

    public class LoginResponse
    {
        public string SessionID { get; set; }
    }

    public class LogoutRequest
    {
        public string SessionID { get; set; }
    }

    public class LogoutResponse
    {
        
    }
}
