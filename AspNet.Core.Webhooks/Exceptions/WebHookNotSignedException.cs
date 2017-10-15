using System;
using System.Collections.Generic;
using System.Text;

namespace AspNet.Core.Webhooks.Exceptions
{
    public class WebHookNotSignedException : WebHookException
    {
        public WebHookNotSignedException(string message) : base(message)
        {

        }
    }
}
