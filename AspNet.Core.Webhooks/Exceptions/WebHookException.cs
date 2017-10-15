using System;
using System.Collections.Generic;
using System.Text;

namespace AspNet.Core.Webhooks.Exceptions
{
    public class WebHookException : Exception
    {
        public WebHookException(string message) : base(message)
        {

        }

        public WebHookException(string message, Exception innerException):base(message, innerException)
        {

        }

    }
}
