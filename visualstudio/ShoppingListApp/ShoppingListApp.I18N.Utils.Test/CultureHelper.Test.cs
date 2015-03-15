using System;
using Moq;
using NUnit.Framework;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    public class EmailBackupProcessorTest
    {
        [SetUp]
        public void Init()
        {
            return;
        }

        [TearDown]
        public void Dispose()
        {
            return;
        }

        [Test]
        public void EmailBackupProcessorThrowsException_WhenFileToBeAttachedIsNull()
        {
            // Arrange
            
            // Act

            // Assert
            return;
        }
    }
}
