using System;
using System.Collections.Generic;
using System.Text;

namespace AspNet.Core.Webhooks.Exceptions
{
    public class WebHookMissedSignatureException : WebHookException
    {
        public WebHookMissedSignatureException(string message) : base(message)
        {

        }
    }
}
