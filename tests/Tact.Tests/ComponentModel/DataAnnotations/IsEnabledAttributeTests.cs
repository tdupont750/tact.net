using System;
using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel;
using Tact.ComponentModel.DataAnnotations;
using Xunit;

namespace Tact.Tests.ComponentModel.DataAnnotations
{
    public class IsEnabledAttributeTests
    {
        [Fact]
        public void ValidIsEnabled()
        {
            var model = new IsEnabledModel { IsEnabled = true, String = "Hi"};
            var validationContext = new ValidationContext(model);
            validationContext.ValidateObject();
        }

        [Fact]
        public void InvalidIsEnabled()
        {
            var model = new IsEnabledModel { IsEnabled = true };
            Assert.Throws<AggregateException>(() =>
            {
                var validationContext = new ValidationContext(model);
                validationContext.ValidateObject();
            });
        }

        [Fact]
        public void ValidNotEnabled()
        {
            var model = new IsEnabledModel { String = "Hi" };
            var validationContext = new ValidationContext(model);
            validationContext.ValidateObject();
        }

        [Fact]
        public void InvalidNotEnabled()
        {
            var model = new IsEnabledModel();
            var validationContext = new ValidationContext(model);
            validationContext.ValidateObject();
        }

        [Fact]
        public void InvalidIsEnabledModelTest()
        {
            var model = new InvalidIsEnabledModel();
            Assert.Throws<InvalidOperationException>(() =>
            {
                var validationContext = new ValidationContext(model);
                validationContext.ValidateObject();
            });
        }

        public class IsEnabledModel
        {
            [IsValidationEnabled]
            public bool IsEnabled { get; set; }

            [RequireNonDefault]
            public string String { get; set; }
        }

        public class InvalidIsEnabledModel
        {
            [IsValidationEnabled]
            public string String { get; set; }
        }
    }
}