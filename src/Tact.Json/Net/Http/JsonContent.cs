using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tact.Collections;

namespace Tact.Net.Http
{
    public class JsonContent : HttpContent
    {
        private static readonly Lazy<JsonSerializer> DefaultJsonSerializer
            = new Lazy<JsonSerializer>(JsonSerializer.CreateDefault);

        private PooledStream _pooledStream;

        public JsonContent(object obj, JsonSerializer jsonSerializer = null)
        {
            _pooledStream = PooledStream.Acquire();

            (jsonSerializer ?? DefaultJsonSerializer.Value)
                .Serialize(_pooledStream.StreamWriter, obj);

            _pooledStream.StreamWriter.Flush();

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            ArraySegment<byte> buffer;
            if (!_pooledStream.MemoryStream.TryGetBuffer(out buffer))
                throw new InvalidOperationException("No Buffer Found");

            return stream.WriteAsync(buffer.Array, 0, buffer.Count);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _pooledStream.MemoryStream.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _pooledStream != null)
            {
                PooledStream.Return(_pooledStream);
                _pooledStream = null;
            }

            base.Dispose(disposing);
        }

        private class PooledStream
        {
            private static readonly ObjectPool<PooledStream> Pool
                = new ObjectPool<PooledStream>(100, () => new PooledStream());

            public readonly StreamWriter StreamWriter;
            public readonly MemoryStream MemoryStream;

            private PooledStream()
            {
                MemoryStream = new MemoryStream();
                StreamWriter = new StreamWriter(MemoryStream);
            }

            public static PooledStream Acquire()
            {
                return Pool.Acquire();
            }

            public static void Return(PooledStream pooledStream)
            {
                pooledStream.MemoryStream.SetLength(0);
                Pool.Release(pooledStream);
            }
        }
    }
}
