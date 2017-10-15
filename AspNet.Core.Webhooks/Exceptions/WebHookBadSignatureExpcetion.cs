using System;
using System.Collections.Generic;
using System.Text;

namespace AspNet.Core.Webhooks.Exceptions
{
    public class WebHookBadSignatureExpcetion : WebHookException
    {
        public WebHookBadSignatureExpcetion(string message) : base(message)
        {

        }
    }
}
