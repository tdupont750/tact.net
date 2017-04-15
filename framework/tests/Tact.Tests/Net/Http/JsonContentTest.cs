using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using Tact.Net.Http;
using Xunit;

namespace Tact.Tests.Net.Http
{
    public class JsonContentTest
    {
        [Fact(Skip = "Needs optimization")]
        public void ReadAsString()
        {
            var testObject = TestObject.Create();
            
            string s2 = null, s1 = null, s0 = JsonConvert.SerializeObject(testObject);
            long t2 = 0, t1 = 0;

            for (var i = 0; i < 1000; i++)
            {
                var sw1 = Stopwatch.StartNew();
                var json = JsonConvert.SerializeObject(testObject);
                using (var content = new StringContent(json))
                    s1 = content.ReadAsStringAsync().Result;
                sw1.Stop();
                t1 += sw1.ElapsedTicks;
            }

            for (var i = 0; i < 1000; i++)
            {
                var sw2 = Stopwatch.StartNew();
                using (var content = new JsonContent(testObject))
                    s2 = content.ReadAsStringAsync().Result;

                sw2.Stop();
                t2 += sw2.ElapsedTicks;
            }

            Assert.Equal(s0, s1);
            Assert.Equal(s1, s2);
            Assert.True(t1 > t2, $"{t1} > {t2}");
        }
        
        private class TestObject
        {
            public static TestObject Create()
            {
                return new TestObject
                {
                    Guid = Guid.NewGuid(),
                    B = true,
                    C = 'C',
                    I = int.MaxValue,
                    Map = new Dictionary<int, string>
                {
                    { 1, "One" },
                    { 20, "Twenty" },
                    { 300, "Three Hundred" },
                }
                };
            }

            public Guid Guid { get; set; }
            public int I { get; set; }
            public bool B { get; set; }
            public char C { get; set; }
            public Dictionary<int, string> Map { get; set; }
        }
    }
}
