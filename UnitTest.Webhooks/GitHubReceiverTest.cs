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
    //TODO: Use Microsoft.AspNetCore.TestHost
    public class GitHubReceiverTest
    {
        string playload = "{\"message\":\"Is a Test\", \"Created\":12/05/2017}";
        const string ApiKey = "1234";

        private readonly ITestOutputHelper output;
        GithubReceiver _githubReceiver;
        HttpContext _httpContextMock;
        public GitHubReceiverTest(ITestOutputHelper output)
    {
            this.output = output;
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextMock = new DefaultHttpContext();
            _httpContextMock.Request.Headers.Add("X-Hub-Signature", "sha1=D84EC1692152800054B90555F8705FA0F03ECC8B");
            var bytes = Encoding.ASCII.GetBytes(playload);
            _httpContextMock.Request.Body = new MemoryStream(bytes);
            httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock);
            _githubReceiver = new GithubReceiver(httpContextAccessorMock.Object, new GithubOptions
            {
                ApiKey = ApiKey,
                WebHookAction = (ctx, message) =>
                {
                    output.WriteLine($"In test {nameof(GitHubReceiverTest)}");
                }
            });
        }

        [Fact]
        public void AssertSignatureIsTrue()
        {
            var res = _githubReceiver.AssertSignature(_httpContextMock);
            Assert.True(res);
        }

        [Fact]
        public void WebHookBadSignatureExpcetion()
        {
            _httpContextMock.Request.Headers["X-Hub-Signature"] = "sha1=D84EC1155152800054B90555F8705FA0F03ECC8B";
            Assert.Throws<WebHookBadSignatureExpcetion>(() => _githubReceiver.AssertSignature(_httpContextMock));
        }

    }

}
