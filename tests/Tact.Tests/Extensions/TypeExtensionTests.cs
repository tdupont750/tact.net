using System;
using Xunit;

namespace Tact.Tests.Extensions
{
    public class TypeExtensionTests
    {
        [Fact]
        public void DefaultConstructor()
        {
            var type = typeof(TypeExtensionTests);
            type.EnsureSingleCostructor();
            type.EnsureSingleCostructor();
        }

        [Fact]
        public void OneConstructor()
        {
            var type = typeof(One);
            type.EnsureSingleCostructor();
            type.EnsureSingleCostructor();
        }

        [Fact]
        public void TwoConstructors()
        {
            var type = typeof(Two);
            Assert.Throws<InvalidOperationException>(() => type.EnsureSingleCostructor());
            Assert.Throws<InvalidOperationException>(() => type.EnsureSingleCostructor());
        }

        private class One
        {
            public One(int i)
            {
            }
        }

        private class Two
        {
            public Two(int i)
            {
            }

            public Two(int i, bool b)
            {
            }
        }
    }
}
