using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using University.Interfaces;
using University.Services;

namespace University.Tests
{
    [TestClass]
    public class ValidationTest
    {
        private IValidationService validationService;

        [TestInitialize]
        public void Initialize()
        {
            validationService = new ValidationService();
        }

        [TestMethod]
        public void ValidateNullDate()
        {
            // Arrange
            DateTime? nullDate = null;

            // Act
            bool result = validationService.ValidateBirthDate(nullDate);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateFutureDate()
        {
            // Arrange
            DateTime futureDate = DateTime.Now.AddDays(1);

            // Act
            bool result = validationService.ValidateBirthDate(futureDate);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateCorrectDate()
        {
            // Arrange
            DateTime pastDate = DateTime.Now.AddYears(-1);

            // Act
            bool result = validationService.ValidateBirthDate(pastDate);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateCorrectPESEL()
        {
            // Arrange
            string correctPESEL = "88012471472";

            // Act
            bool result = validationService.ValidatePESEL(correctPESEL);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateIncorrectPESEL()
        {
            // Arrange
            string incorrectPESEL = "88012471471";

            // Act
            bool result = validationService.ValidatePESEL(incorrectPESEL);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
