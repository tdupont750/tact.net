using System;
using System.Net.WebSockets;

namespace Tact.Net.WebSockets
{
    public class WebSocketException : Exception
    {
        public WebSocketException(WebSocketCloseStatus status)
        {
            Status = status;
        }

        public WebSocketCloseStatus Status { get; }
    }
}