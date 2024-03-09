using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

[assembly: HostingStartup(typeof(DemoHostingStartupAssembly.StartupEnhancementHostingStartup))]

namespace DemoHostingStartupAssembly
{
    public class StartupEnhancementHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            Console.WriteLine($"[+] {nameof(StartupEnhancementHostingStartup)} executing...");

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IStartupFilter, MyStartupFilter>();
                services.AddSingleton<MyMiddleware>();
            });

            builder.ConfigureAppConfiguration((ctx, appConfiguration) =>
            {
            });
        }
    }

    public class MyStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            Console.WriteLine($"[+] {nameof(MyStartupFilter)} executing...");
            return builder =>
            {
                builder.UseMiddleware<MyMiddleware>();
                next(builder);
            };
        }
    }

    public class MyMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var start = Stopwatch.GetTimestamp();
            Console.WriteLine($"[+] {nameof(MyMiddleware)} executing...");

            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                httpContext.Response.Headers.Add("X-Response-Time-Milliseconds", Stopwatch.GetElapsedTime(start).ToString());
                return Task.CompletedTask;
            }, context);

            await next(context);
        }
    }
}
