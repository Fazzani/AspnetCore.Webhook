using AspNet.Core.Webhooks.Exceptions;
using AspNet.Core.Webhooks.Receivers;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AspNet.Core.Webhooks
{
    public abstract class AbstractWebHookHandler<TOptions, TWebHookMessage> : IWebHookHandler 
        where TOptions : WebhookOptions<TWebHookMessage> where TWebHookMessage : new()
    {
        protected TWebHookMessage webHookMessage;

        protected readonly TOptions _options;
        protected string _requestBody;
        public abstract string KeyToken { get; set; }
       // public HttpContext HttpContext { get; }

        public AbstractWebHookHandler(TOptions options)
        {
           // HttpContext = httpContext.HttpContext;
            _options = options;
            _requestBody = string.Empty;
        }


        public async Task<string> RequestBody(HttpContext httpContext)
        {
            if (httpContext.Request.Body.CanRead && string.IsNullOrEmpty(_requestBody))
                _requestBody = await new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 4096, false).ReadToEndAsync();
            return _requestBody;
        }

        /// <summary>
        /// The action trigred when Webhook handled
        /// </summary>
        public virtual void Invoke(HttpContext httpContext) =>
          httpContext.Response.OnStarting(() => Task.Run(() => {
              webHookMessage = (TWebHookMessage)JsonConvert.DeserializeObject(RequestBody(httpContext).GetAwaiter().GetResult(), typeof(TWebHookMessage));
              _options.WebHookAction?.Invoke(httpContext, webHookMessage);
          }));

        /// <summary>
        /// Is Post method and request path start with webhook
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public virtual bool IsWebHookRequest(HttpContext context) =>
              context.Request.Method.Equals("Post", StringComparison.InvariantCultureIgnoreCase)
                 && context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/webhook");

        /// <summary>
        /// Get webhook signature
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public abstract string GetSignature(HttpContext httpContext);

        /// <summary>
        /// Assert matching signature
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public virtual bool AssertSignature(HttpContext httpContext)
        {
            var signature = GetSignature(httpContext);
            if (!string.IsNullOrEmpty(signature))
            {
                var requestBody = RequestBody(httpContext).GetAwaiter().GetResult();

                var hash = new HMACSHA256(Encoding.ASCII.GetBytes(_options.ApiKey))
                    .ComputeHash(Encoding.ASCII.GetBytes(requestBody))
                    .Aggregate(string.Empty, (s, e) => s + String.Format("{0:X2}", e), s => s);

                if (!hash.Equals(signature, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new WebHookBadSignatureExpcetion("WebHook Signatures didn't match!");
                }
            }
            else
            {
                throw new WebHookMissedSignatureException("WebHook must be signed");
            }
            return true;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
