using AspNet.Core.Webhooks.Exceptions;
using AspNet.Core.Webhooks.Receivers;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace UnitTest.Webhooks
{
    public class AppveyorReceiverTest
    {
        string playload = "{\"message\":\"Is a Test\", \"Created\":12/05/2017}";
        const string ApiKey = "1234";

        private readonly ITestOutputHelper output;
        AppveyorReceiver _appveyorReceiver;
        HttpContext _httpContextMock;
        public AppveyorReceiverTest(ITestOutputHelper output)
        {
            this.output = output;
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new DefaultHttpContext();
            _httpContextMock.Request.Headers.Add("X-Hub-Signature", "D6B45E6C267330022E1E4FA70D88134A7E5B25A0470541A40A48BD53C5980A08");
            var bytes = Encoding.ASCII.GetBytes(playload);
            _httpContextMock.Request.Body = new MemoryStream(bytes);
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock);
            _appveyorReceiver = new AppveyorReceiver(httpContextAccessorMock.Object, new AppveyorOptions
            {
                ApiKey = ApiKey,
                WebHookAction = (ctx, message) =>
                {
                    output.WriteLine($"In test {nameof(AppveyorReceiverTest)}");
                }
            });
        }

        [Fact]
        public void AssertSignatureIsTrue()
        {
            var res = _appveyorReceiver.AssertSignature(_httpContextMock);
            Assert.True(res);
        }

        [Fact]
        public void WebHookBadSignatureExpcetion()
        {
            _httpContextMock.Request.Headers["X-Hub-Signature"] = "D6B45E6C267330022E1E4FA70D88134A7E5B25A0499541A40A48BD53C5980A08";
            Assert.Throws<WebHookBadSignatureExpcetion>(() => _appveyorReceiver.AssertSignature(_httpContextMock));
        }
    }
}
