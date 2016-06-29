namespace WebApiClient
{
    public class Response<T>
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public T Body { get; set; }

        public Response()
        {
            
        }

        public Response(int code, string message, T body)
        {
            ResponseCode = code;
            ResponseMessage = message;
            Body = body;
        }
    }
}
