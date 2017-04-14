using NLog.Config;
using NLog.LayoutRenderers;
using System;
using System.Text;

namespace Tact.NLog.LayoutRenderers
{
    [ThreadAgnostic, LayoutRenderer("cleanEx")]
    public class CleanExceptionLayoutRenderer : ExceptionLayoutRenderer
    {
        protected override void AppendStackTrace(StringBuilder sb, Exception ex)
        {
            // TODO
            base.AppendStackTrace(sb, ex);
        }
    }
}
