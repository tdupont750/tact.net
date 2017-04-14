using System.Text;
using global::NLog.Targets;
using global::NLog.Layouts;
using global::NLog.Common;
using global::NLog;
using System.Net;
using System.Net.Sockets;
using global::NLog.LayoutRenderers;

namespace Tact.NLog.Targets
{
    [Target("Log4JUdp")]
    public class Log4JUdpTarget : TargetWithLayout
    {
        private readonly Socket _socket;
        private readonly Log4JXmlEventLayoutRenderer _render;

        private IPEndPoint _remoteEndPoint;

        public Log4JUdpTarget()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 0);
            _socket.Bind(localEP);
            
            _render = new Log4JXmlEventLayoutRenderer();
        }

        public string Ip { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 7071;

        protected override void Dispose(bool disposing)
        {
            _socket.Dispose();

            base.Dispose(disposing);
        }

        protected override void InitializeTarget()
        {
            var ipAddress = IPAddress.Parse(Ip);
            _remoteEndPoint = new IPEndPoint(ipAddress, Port);

            base.InitializeTarget();
        }

        protected override void Write(AsyncLogEventInfo asyncLogEvent)
        {
            var logEvent = asyncLogEvent.LogEvent;

            LogEventInfo renderedEvent;
            if (Layout != null)
            {
                var message = Layout.Render(logEvent);
                renderedEvent = new LogEventInfo(
                    logEvent.Level,
                    logEvent.LoggerName,
                    message);
            }
            else
                renderedEvent = logEvent;

            var renderedMessage = _render.Render(renderedEvent);
            var bytes = Encoding.UTF8.GetBytes(renderedMessage);
            _socket.SendTo(bytes, SocketFlags.None, _remoteEndPoint);
        }
    }
}