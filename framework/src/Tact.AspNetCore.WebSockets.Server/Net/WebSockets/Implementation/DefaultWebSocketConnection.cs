using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Tact.Net.WebSockets.Implementation
{
    public class DefaultWebSocketConnection : IWebSocketConnection
    {
        private const string NotOpenMessage = "WebSocket not open";

        private WebSocket _webSocket;
        private Action<WebSocket> _onOpen;

        public DefaultWebSocketConnection(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; }

        public Action<WebSocket> OnOpen
        {
            get { return OnOpenWrapper; }
            set { _onOpen = value; }
        }

        public Action<string> OnMessage { get; set; }
        public Action<byte[]> OnBinary { get; set; }
        public Func<Exception, bool> OnError { get; set; }
        public Action<Exception> OnFatalError { get; set; }
        public Action<WebSocketCloseStatus, string> OnClose { get; set; }

        public Task SendAsync(string message, CancellationToken cancelToken)
        {
            if (_webSocket == null) throw new InvalidOperationException(NotOpenMessage);
            if (message == null) throw new ArgumentNullException(nameof(message));
            var bytes = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(bytes);
            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancelToken);
        }

        public Task SendAsync(byte[] bytes, CancellationToken cancelToken)
        {
            if (_webSocket == null) throw new InvalidOperationException(NotOpenMessage);
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            var segment = new ArraySegment<byte>(bytes);
            return _webSocket.SendAsync(segment, WebSocketMessageType.Binary, true, cancelToken);
        }

        public Task CloseAsync(CancellationToken cancelToken)
        {
            if (_webSocket == null) throw new InvalidOperationException(NotOpenMessage);
            return _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancelToken);
        }

        private void OnOpenWrapper(WebSocket webSocket)
        {
            if (webSocket == null) throw new ArgumentNullException(nameof(webSocket));
            _webSocket = webSocket;
            _onOpen?.Invoke(webSocket);
        }
    }
}