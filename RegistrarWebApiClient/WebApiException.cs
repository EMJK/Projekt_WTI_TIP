using System;

namespace WebApiClient
{
    public class WebApiException : Exception
    {
        public int ResponseCode { get; }
        public string ResponseMessage { get; }

        public WebApiException(int responseCode, string responseMessage)
        {
            this.ResponseCode = responseCode;
            this.ResponseMessage = responseMessage;
        }
    }
}
