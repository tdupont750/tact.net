using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WebSocket4Net;
using Xunit;

namespace Tact.AspNetCore.WebSockets.Server.Tests
{
    public class WebSocketHandlerTests
    {
        [Fact]
        public async Task IntegrationTestAsync()
        {
            const string url = "://localhost:8283";
            using (var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http{url}")
                .UseStartup<Startup>()
                .Build())
            {
                host.Start();

                var queue = host.Services.GetService<Queue<string>>();
                var tcs = new TaskCompletionSource<bool>();

                using (var client = new WebSocket($"ws{url}/test"))
                {
                    client.MessageReceived += (s, e) =>
                    {
                        queue.Enqueue(e.Message);
                        tcs.SetResult(true);
                    };

                    client.Open();

                    await Task.Delay(500).ConfigureAwait(false);

                    client.Send("hi");

                    var delay = Task.Delay(2000);
                    await Task.WhenAny(tcs.Task, delay).ConfigureAwait(false);

                    Assert.False(delay.IsCompleted);
                    Assert.Equal(2, queue.Count);
                    Assert.Equal("hi", queue.Dequeue());
                    Assert.Equal("bye", queue.Dequeue());
                }
            }
        }
        
        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddSingleton(new Queue<string>());
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseWebSockets();
                app.UseWebSocketHandler("test", connection =>
                {
                    var queue = (Queue<string>)app.ApplicationServices.GetService(typeof(Queue<string>));

                    connection.OnMessage = m =>
                    {
                        queue.Enqueue(m);
                        connection.SendAsync("bye");
                    };
                });
            }
        }
    }
}
