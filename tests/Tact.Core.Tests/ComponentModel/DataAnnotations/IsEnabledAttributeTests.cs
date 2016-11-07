using System;
using Tact.ComponentModel;
using Tact.ComponentModel.DataAnnotations;
using Xunit;

namespace Tact.Core.Tests.ComponentModel.DataAnnotations
{
    public class IsEnabledAttributeTests
    {
        [Fact]
        public void ValidIsEnabled()
        {
            var model = new IsEnabledModel { IsEnabled = true, String = "Hi"};
            ModelValidation.Validate(model);
        }

        [Fact]
        public void InvalidIsEnabled()
        {
            var model = new IsEnabledModel { IsEnabled = true };
            Assert.Throws<ModelValidationException>(() => ModelValidation.Validate(model));
        }

        [Fact]
        public void ValidNotEnabled()
        {
            var model = new IsEnabledModel { String = "Hi" };
            ModelValidation.Validate(model);
        }

        [Fact]
        public void InvalidNotEnabled()
        {
            var model = new IsEnabledModel();
            ModelValidation.Validate(model);
        }

        [Fact]
        public void InvalidIsEnabledModelTest()
        {
            var model = new InvalidIsEnabledModel();
            Assert.Throws<InvalidOperationException>(() => ModelValidation.Validate(model));
        }

        public class IsEnabledModel
        {
            [IsEnabled]
            public bool IsEnabled { get; set; }

            [RequireNonDefault]
            public string String { get; set; }
        }

        public class InvalidIsEnabledModel
        {
            [IsEnabled]
            public string String { get; set; }
        }
    }
}