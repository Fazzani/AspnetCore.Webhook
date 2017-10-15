[![Build status](https://ci.appveyor.com/api/projects/status/ti7qlv5ekc535k85?svg=true)](https://ci.appveyor.com/project/Fazzani/aspnetcore-webhook)

## Description
<p>Generic AspNet Webhook middleware.</p>
<p>A Github and appveyor was implemented by default.</p>

## Setup
<pre>Install-Package AspNet.Core.Webhooks -Version 1.0.0</pre>

1. Add this code in ConfigureServices method of Startup class
<pre>
<code>
  services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
  services.UseGithubWebhook(() => new GithubOptions
            {
                ApiKey = "test",
                WebHookAction = async (context, message) =>
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { source = nameof(GithubOptions), message = message }), CancellationToken.None);
                }
            }).UseAppVeyorWebhook(() => new AppveyorOptions
            {
                ApiKey = "qwertyuiopasdfghjklzxcvbnm123456",
                WebHookAction = async (context, message) =>
                                 {
                                     context.Response.ContentType = "application/json";
                                     context.Response.StatusCode = StatusCodes.Status200OK;
                                     await context.Response.WriteAsync(JsonConvert.SerializeObject(new { source = nameof(AppveyorOptions), message = message }), CancellationToken.None);
                                 }
            });
</code>
</pre>

2. In Configure method Add this code :
<pre>
<code>
 app.UseWebHooks(typeof(AppveyorReceiver));
 app.UseWebHooks(typeof(GithubReceiver));
</code>
</pre>
### TODO

* Async
* auto Resolver assemblies by interface IWebHookHandler
* config from settings files
* Log
* Auto upload to nuget
* Add capability to user app.UseMiddleware<T>