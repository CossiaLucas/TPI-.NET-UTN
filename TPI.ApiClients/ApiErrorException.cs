using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TPI.ApiClients
{
    public class ApiErrorException : HttpRequestException
    {
        public ApiErrorException(string message, HttpStatusCode statusCode)
            : base(message, null, statusCode) { }
    }
}
