using System;

namespace RegistrarWebApiClient
{
    public class WebApiException : Exception
    {
        private int responseCode;
        private string responseMessage;

        public WebApiException(int responseCode, string responseMessage)
        {
            this.responseCode = responseCode;
            this.responseMessage = responseMessage;
        }
    }
}
