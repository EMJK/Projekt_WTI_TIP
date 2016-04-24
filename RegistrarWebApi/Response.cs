using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistrarWebApi
{
    public class Response<T>
    {
        public Response()
        {
            
        }

        public Response(int code, string message, T body)
        {
            ResponseCode = code;
            ResponseMessage = message;
            Body = body;
        } 
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

        public T Body { get; set; }
    }
}
