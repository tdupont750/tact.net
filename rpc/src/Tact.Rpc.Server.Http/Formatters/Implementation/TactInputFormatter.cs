using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tact.Rpc.Serialization;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Formatters.Base;

namespace Tact.Rpc
{
    [RegisterSingleton(typeof(IInputFormatter))]
    public class TactInputFormatter : TactFormatterBase, IInputFormatter
    {
        public TactInputFormatter(IReadOnlyList<ISerializer> serializers)
            : base(serializers)
        {
        }

        public bool CanRead(InputFormatterContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return HasSerializerMatch(context.HttpContext);
        }
        
        public async Task<InputFormatterResult> ReadAsync(InputFormatterContext context)
        {
            var serializer = GetSerializer(context.HttpContext.Request.ContentType);
            if (serializer == null)
                throw new InvalidOperationException("Serializer not found");

            var result = await serializer
                .DeserializeAsync(context.ModelType, context.HttpContext.Request.Body)
                .ConfigureAwait(false);

            return await InputFormatterResult
                .SuccessAsync(result)
                .ConfigureAwait(false);                
        }
    }

}
