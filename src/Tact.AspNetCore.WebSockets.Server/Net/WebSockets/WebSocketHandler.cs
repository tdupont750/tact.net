using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Tact.Net.WebSockets.Implementation;

namespace Tact.Net.WebSockets
{
    public class WebSocketHandler
    {
        private readonly Action<IWebSocketConnection> _connectionInitializer;
        private readonly string _path;
        private readonly Func<HttpContext, IWebSocketConnection> _connectionFactory;

        public WebSocketHandler(string path, Func<HttpContext, IWebSocketConnection> connectionFactory)
        {
            _path = CleanPath(path);
            _connectionFactory = connectionFactory;
        }

        public WebSocketHandler(string path, Action<IWebSocketConnection> connectionInitializer)
        {
            _path = CleanPath(path);
            _connectionInitializer = connectionInitializer;
        }

        public bool IsMatch(string path)
        {
            var clean = CleanPath(path);
            return _path.Equals(clean, StringComparison.OrdinalIgnoreCase);
        }

        public async Task ProcessWebSocketAsync(HttpContext http)
        {
            IWebSocketConnection connection;
            if (_connectionFactory == null)
            {
                connection = new DefaultWebSocketConnection(http);
                _connectionInitializer(connection);
            }
            else
                connection = _connectionFactory(http);

            var buffer = new Buffer();
            var closeStatus = WebSocketCloseStatus.Empty;

            using (connection as IDisposable)
            using (var webSocket = await http.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false))
            {
                try
                {
                    connection.OnOpen?.Invoke(webSocket);

                    while (webSocket.State == WebSocketState.Open)
                    {
                        var segment = buffer.GetSegment();
                        var received = await webSocket.ReceiveAsync(segment, http.RequestAborted).ConfigureAwait(false);
                        OnReceive(buffer, connection, received);
                    }
                }
                catch (Exception ex)
                {
                    closeStatus = (ex as WebSocketException)?.Status ?? WebSocketCloseStatus.InternalServerError;
                    connection.OnFatalError?.Invoke(ex);
                }
                finally
                {
                    connection.OnClose?.Invoke(webSocket.CloseStatus ?? closeStatus, webSocket.CloseStatusDescription);
                }
            }
        }
        
        private static void OnReceive(Buffer buffer, IWebSocketConnection context, WebSocketReceiveResult received)
        {
            buffer.Offset += received.Count;

            if (buffer.Offset >= Buffer.Size)
                throw new WebSocketException(WebSocketCloseStatus.MessageTooBig);

            if (!received.EndOfMessage)
                return;

            try
            {
                switch (received.MessageType)
                {
                    case WebSocketMessageType.Binary:
                        if (context.OnBinary == null) return;
                        var result = new byte[buffer.Offset];
                        Array.Copy(buffer.Array, result, buffer.Offset);
                        context.OnBinary(result);
                        break;

                    case WebSocketMessageType.Close:
                        return;

                    case WebSocketMessageType.Text:
                        if (context.OnMessage == null) return;
                        var message = Encoding.UTF8.GetString(buffer.Array, 0, buffer.Offset);
                        context.OnMessage(message);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(received.MessageType));
                }
            }
            catch (Exception ex)
            {
                var isFatal = context.OnError?.Invoke(ex) ?? true;
                if (isFatal) throw;
            }

            buffer.Reset();
        }

        private static string CleanPath(string path)
        {
            return path?.Trim('/') ?? string.Empty;
        }

        private class Buffer
        {
            public const int Size = 1024 * 32;

            public readonly byte[] Array = new byte[Size];
            
            public int Offset;

            public ArraySegment<byte> GetSegment()
            {
                var count = Array.Length - Offset;
                return new ArraySegment<byte>(Array, Offset, count);
            }

            public void Reset()
            {
                Offset = 0;
                System.Array.Clear(Array, 0, Size);
            }
        }
    }
}