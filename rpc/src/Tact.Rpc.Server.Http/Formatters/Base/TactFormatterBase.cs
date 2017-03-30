using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Formatters.Base
{
    public abstract class TactFormatterBase
    {
        protected readonly IReadOnlyList<ISerializer> _serializers;

        public TactFormatterBase(IReadOnlyList<ISerializer> serializers)
        {
            _serializers = serializers;
        }

        protected bool HasSerializerMatch(HttpContext context)
        {
            var contentType = context.Request.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
                return false;

            return GetSerializer(contentType) != null;
        }

        protected ISerializer GetSerializer(string contentType)
        {
            return _serializers.FirstOrDefault(s => contentType.StartsWith(s.ContentType, StringComparison.OrdinalIgnoreCase));
        }
    }
}
