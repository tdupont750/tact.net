using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tact.Diagnostics;
using Tact.Net.WebSockets;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Rpc.Configuration;
using Tact.Rpc.Hosts;
using Tact.Rpc.Models;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Server.WebSocket.Hosts.Implementation
{
    public class WebSocketHost : IHost
    {
        private readonly IWebHost _webHost;
        private readonly ISerializer _serializer;
        private readonly IReadOnlyList<IWebSocketEndpoint> _endpoints;
        private readonly ILog _log;

        public WebSocketHost(IResolver resolver, IReadOnlyList<IWebSocketEndpoint> endpoints, WebSocketHostConfig hostConfig, ILog log)
        {
            _log = log;

            _endpoints = endpoints;

            _serializer = resolver.Resolve<ISerializer>(hostConfig.Serializer);

            _webHost = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(hostConfig.Urls.ToArray())
                .ConfigureServices(services =>
                {
                    services.AddSingleton(resolver);
                })
                .Configure(app =>
                {
                    var handler = new WebSocketHandler("rpc/ws", OnConnection);
                    app.UseWebSockets().UseWebSocketHandler(handler);
                })
                .Build();
        }

        public void Dispose()
        {
            _webHost.Dispose();
        }

        public Task InitializeAsync(CancellationToken cancelToken)
        {
            _webHost.Start();
            return Task.CompletedTask;
        }

        public class RegisterConditionAttribute : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(IContainer container, Type toType)
            {
                var config = container.Resolve<WebSocketHostConfig>();
                return config.IsEnabled;
            }
        }

        private void OnConnection(IWebSocketConnection connection)
        {
            connection.OnBinary = b => Task.Run(() => HandleRequestAsync(connection, b), connection.HttpContext.RequestAborted);
            connection.OnClose = (s, m) => _log.Debug("OnClose - {0}: {1}", s, m);
            connection.OnError = ex =>
            {
                _log.Error(ex, "OnError");
                return false;
            };
            connection.OnFatalError = ex => _log.Fatal(ex, "OnFatalError");
            connection.OnOpen = ws => _log.Debug("OnOpen");
            connection.OnMessage = m => Task.Run(() => HandleRequestAsync(connection, m), connection.HttpContext.RequestAborted);
        }

        private Task HandleRequestAsync(IWebSocketConnection connection, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            return HandleRequestAsync(connection, bytes);
        }

        private async Task HandleRequestAsync(IWebSocketConnection connection, byte[] requestBytes)
        {
            using (var stream = new MemoryStream(requestBytes))
            {
                var callInfo = await _serializer
                    .DeserializeAsync<RemoteCallInfo>(stream)
                    .ConfigureAwait(false);

                var callInfoPosition = stream.Position;

                foreach (var endpoint in _endpoints)
                    if (endpoint.CanHandle(callInfo, out Type type))
                    {
                        var model = await _serializer
                            .DeserializeAsync(type, stream)
                            .ConfigureAwait(false);

                        var result = await endpoint
                            .HandleAsync(callInfo, model)
                            .ConfigureAwait(false);

                        stream.Position = callInfoPosition;

                        await _serializer
                            .SerializeToStreamAsync(result, stream)
                            .ConfigureAwait(false);

                        var resultBytes = stream.ToArray();

                        await connection
                            .SendAsync(resultBytes, connection.HttpContext.RequestAborted)
                            .ConfigureAwait(false);

                        break;
                    }
            }
        }
    }
}
