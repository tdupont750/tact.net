using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Formatters.Base;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Formatters.Implementation
{
    [RegisterSingleton(typeof(IOutputFormatter))]
    public class TactOutputFormatter : TactFormatterBase, IOutputFormatter
    {
        public TactOutputFormatter(IReadOnlyList<ISerializer> serializers)
            : base(serializers)
        {
        }

        public bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return HasSerializerMatch(context.HttpContext);
        }

        public async Task WriteAsync(OutputFormatterWriteContext context)
        {
            var serializer = GetSerializer(context.HttpContext.Request.ContentType);
            if (serializer == null)
                throw new InvalidOperationException("Serializer not found");

            await serializer
                .SerializeToStreamAsync(context.Object, context.HttpContext.Response.Body)
                .ConfigureAwait(false);
        }
    }
}
