using System;
using Microsoft.AspNetCore.Http;
using Tact.Net.WebSockets;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseWebSocketHandler(this IApplicationBuilder app, Func<HttpContext, IWebSocketConnection> connectionFactory)
        {
            app.UseWebSocketHandler(string.Empty, connectionFactory);
        }

        public static void UseWebSocketHandler(this IApplicationBuilder app, string path, Func<HttpContext, IWebSocketConnection> connectionFactory)
        {
            var handler = new WebSocketHandler(path, connectionFactory);
            app.UseWebSocketHandler(handler);
        }

        public static void UseWebSocketHandler(this IApplicationBuilder app, Action<IWebSocketConnection> connectionInitializer)
        {
            app.UseWebSocketHandler(string.Empty, connectionInitializer);
        }

        public static void UseWebSocketHandler(this IApplicationBuilder app, string path, Action<IWebSocketConnection> connectionInitializer)
        {
            var handler = new WebSocketHandler(path, connectionInitializer);
            app.UseWebSocketHandler(handler);
        }

        public static void UseWebSocketHandler(this IApplicationBuilder app, params WebSocketHandler[] handlers)
        {
            app.Use((http, next) =>
            {
                if (http.WebSockets.IsWebSocketRequest)
                    for (var i = 0; i < handlers.Length; i++)
                        if (handlers[i].IsMatch(http.Request.Path))
                            return handlers[i].ProcessWebSocketAsync(http);

                return next();
            });
        }
    }
}