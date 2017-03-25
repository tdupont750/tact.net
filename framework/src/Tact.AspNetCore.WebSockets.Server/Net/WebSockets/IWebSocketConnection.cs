using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tact.Net.WebSockets
{
    public interface IWebSocketConnection
    {
        HttpContext HttpContext { get; }

        Action<WebSocket> OnOpen { get; set; }
        Action<string> OnMessage { get; set; }
        Action<byte[]> OnBinary { get; set; }
        Func<Exception, bool> OnError { get; set; }
        Action<Exception> OnFatalError { get; set; }
        Action<WebSocketCloseStatus, string> OnClose { get; set; }

        Task SendAsync(string message, CancellationToken cancelToken = default(CancellationToken));
        Task SendAsync(byte[] bytes, CancellationToken cancelToken = default(CancellationToken));
        Task CloseAsync(CancellationToken cancelToken = default(CancellationToken));
    }
}