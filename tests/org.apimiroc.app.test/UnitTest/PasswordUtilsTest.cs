using FluentAssertions;
using org.apimiroc.core.shared.Utils;

namespace org.apimiroc.app.test.UnitTest
{
    public class PasswordUtilsTest
    {

        [Fact]
        public void GenerateRandomSalt_ShouldReturnValidSalt()
        {

            // Act
            var salt = PasswordUtils.GenerateRandomSalt();

            // Asser
            salt.Should().NotBeNull();
            salt.Should().HaveLength(6);

            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            salt.All(c => validChars.Contains(c))
                .Should().BeTrue("el salt generado debe contener solo caracteres válidos");

        }

        [Fact]
        public void HashPasswordWithSalt_ShoudGenerateDifferentHashes_WhenSaltChanges()
        {

            // Arrange
            string password = "TestPassword123$";
            var salt1 = PasswordUtils.GenerateRandomSalt();
            var salt2 = PasswordUtils.GenerateRandomSalt();

            // Act
            var hash1 = PasswordUtils.HashPasswordWithSalt(password, salt1);
            var hash2 = PasswordUtils.HashPasswordWithSalt(password, salt2);

            // Assert
            hash1.Should().NotBe(hash2, "los hashes deben ser diferentes cuando las sales son diferentes");

        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_WhenPasswordIsCorrect()
        {

            // Arrange
            string password = "TestPassword123$";
            string salt1 = PasswordUtils.GenerateRandomSalt();  
            string storedHash = PasswordUtils.HashPasswordWithSalt(password, salt1);

            // Act
            var result = PasswordUtils.VerifyPassword(password, storedHash, salt1);

            // Arrange
            result.Should().BeTrue("la verificación debe ser exitosa cuando la contraseña es correcta");

        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_WhenPasswordIsIncorrect()
        {

            // Arrange
            string correctPassword = "TestPassword123$";
            string incorrectPassword = "WrongPassword456$";
            string salt = PasswordUtils.GenerateRandomSalt();
            string storedHash = PasswordUtils.HashPasswordWithSalt(correctPassword, salt);

            // Act
            var result = PasswordUtils.VerifyPassword(incorrectPassword, storedHash, salt);

            // Arrange
            result.Should().BeFalse("la verificación debe fallar cuando la contraseña es incorrecta");

        }

    }
}
