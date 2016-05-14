namespace RegistrarWebApiClient.Models.Account
{
    public class ChangePasswordRequest
    {
        public string UserID { get; set; }
        public string SessionID { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}