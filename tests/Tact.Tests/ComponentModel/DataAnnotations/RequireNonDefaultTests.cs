using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tact.ComponentModel.DataAnnotations;
using Xunit;

namespace Tact.Tests.ComponentModel.DataAnnotations
{
    public class RequireNonDefaultTests
    {
        [Fact]
        public void AllErrors()
        {
            var model = new TestModel();
            var ex = Assert.Throws<AggregateException>(() =>
            {
                var validationContext = new ValidationContext(model);
                validationContext.ValidateObject();
            });
            Assert.Equal(4, ex.InnerExceptions.Count);
        }

        [Fact]
        public void NoErrors()
        {
            var model = new TestModel
            {
                NullableInt = 0,
                Int = 1,
                Bool = true,
                Object = new object()
            };

            var validationContext = new ValidationContext(model);
            validationContext.ValidateObject();
        }

        [Fact]
        public void Strings()
        {
            var model = new StringModel();
            var ex1 = Assert.Throws<AggregateException>(() =>
            {
                var validationContext1 = new ValidationContext(model);
                validationContext1.ValidateObject();
            });
            Assert.Equal(1, ex1.InnerExceptions.Count);

            model.String = string.Empty;
            var ex2 = Assert.Throws<AggregateException>(() =>
            {
                var validationContext2 = new ValidationContext(model);
                validationContext2.ValidateObject();
            });
            Assert.Equal(1, ex2.InnerExceptions.Count);

            model.String = "Test";
            var validationContext3 = new ValidationContext(model);
            validationContext3.ValidateObject();
        }

        [Fact]
        public void NoAttributes()
        {
            var model = new NoAttributesModel();
            var validationContext = new ValidationContext(model);
            validationContext.ValidateObject();
        }

        public class NoAttributesModel
        {
            public string String { get; set; }
        }

        public class StringModel
        {
            [RequireNonDefault]
            public string String { get; set; }
        }

        public class TestModel
        {
            [RequireNonDefault]
            public int? NullableInt { get; set; }

            [RequireNonDefault]
            public int Int { get; set; }

            [RequireNonDefault]
            public bool Bool { get; set; }

            [RequireNonDefault]
            public object Object { get; set; }
        }
    }
}
