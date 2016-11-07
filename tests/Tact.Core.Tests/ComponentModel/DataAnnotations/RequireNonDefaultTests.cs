using Tact.ComponentModel;
using Tact.ComponentModel.DataAnnotations;
using Xunit;

namespace Tact.Core.Tests.ComponentModel.DataAnnotations
{
    public class RequireNonDefaultTests
    {
        [Fact]
        public void AllErrors()
        {
            var model = new TestModel();
            var ex = Assert.Throws<ModelValidationException>(() => ModelValidation.Validate(model));
            Assert.Equal(4, ex.Errors.Count);
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

             ModelValidation.Validate(model);
        }

        [Fact]
        public void Strings()
        {
            var model = new StringModel();
            var ex = Assert.Throws<ModelValidationException>(() => ModelValidation.Validate(model));
            Assert.Equal(1, ex.Errors.Count);

            model.String = string.Empty;
            ex = Assert.Throws<ModelValidationException>(() => ModelValidation.Validate(model));
            Assert.Equal(1, ex.Errors.Count);

            model.String = "Test";
            ModelValidation.Validate(model);
        }

        [Fact]
        public void NoAttributes()
        {
            var model = new NoAttributesModel();
            ModelValidation.Validate(model);
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
