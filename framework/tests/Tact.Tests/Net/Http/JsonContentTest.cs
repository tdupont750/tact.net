using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tact.Net.Http;
using Xunit;

namespace Tact.Tests.Net.Http
{
    public class JsonContentTest
    {
        [Fact]
        public void ReadAsString()
        {
            var testObject = TestObject.Create();
            
            string s2, s1, s0 = JsonConvert.SerializeObject(testObject);

            var sw1 = Stopwatch.StartNew();
            var json = JsonConvert.SerializeObject(testObject);
            using (var content = new StringContent(json))
                s1 = content.ReadAsStringAsync().Result;

            sw1.Stop();

            var sw2 = Stopwatch.StartNew();
            using (var content = new JsonContent(testObject))
                s2 = content.ReadAsStringAsync().Result;

            sw2.Stop();

            Assert.Equal(s0, s1);
            Assert.Equal(s1, s2);
            Assert.True(sw1.Elapsed > sw2.Elapsed, $"{sw1.Elapsed} > {sw2.Elapsed}");
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
