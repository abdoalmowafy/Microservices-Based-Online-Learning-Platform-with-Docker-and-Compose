using auth.DTOs.Requests;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace auth.tests.DTOsTests
{
    public class RequestsTests
    {
        [Theory]
        [InlineData("        ", "Test@123")]
        [InlineData("testuser", "        ")]
        [InlineData("        ", "        ")]
        [InlineData("testuser", "Te@1")]
        [InlineData("testuser", "Test@123")]
        public void DTOs_Requests_RegisterRequest_ReturnsFalse(string email, string password)
        {
            // Arrange
            var request = new RegisterRequest
            {
                Email = email,
                Password = password
            };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("        ", "Test@123")]
        [InlineData("testuser", "        ")]
        [InlineData("        ", "        ")]
        [InlineData("testuser", "Te@1")]
        [InlineData("testuser", "Test@123")]
        public void DTOs_Requests_LoginRequest_ReturnsFalse(string email, string password)
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = email,
                Password = password
            };
            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(request, context, results, true);

            // Assert
            isValid.Should().BeFalse();
        }
    }
}
