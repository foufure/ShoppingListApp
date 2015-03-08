﻿#define NUNIT_UNITTEST

using System;
using System.IO;
using Moq;
using NUnit.Framework;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;

namespace ShoppingListApp.Domain.Test
{
    [TestFixture]
    public class EmailBackupProcessorTest
    {
        private Mock<IEmailSettings> emailSettingsMock;

        [SetUp]
        public void Init()
        {
            emailSettingsMock = new Mock<IEmailSettings>();
            emailSettingsMock.Setup(x => x.UseSsl).Returns(true);
            emailSettingsMock.Setup(x => x.MailFromAddress).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.MailToAddress).Returns("shoppinglistappharbor@gmail.com");
            emailSettingsMock.Setup(x => x.Password).Returns("InvalidPassword");
            emailSettingsMock.Setup(x => x.ServerName).Returns("smtp.gmail.com");
            emailSettingsMock.Setup(x => x.ServerPort).Returns(587);
            emailSettingsMock.Setup(x => x.UserName).Returns("shoppinglistappharbor@gmail.com");
        }

        [TearDown]
        public static void Dispose()
        {
            Array.ForEach(Directory.GetFiles(@"c:\temp\"), File.Delete);
        }

        [Test]
        public void EmailBackupProcessorThrowsException_WhenFileToBeAttachedIsNull()
        { 
            // Arrange
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object);

            // Act

            // Assert
            Assert.Throws(typeof(ArgumentNullException), () => testee.ProcessBackup(null));
        }

        [Test]
        public void EmailBackupProcessorBackupComplete_WhenEmailSent()
        {
            // Arrange
            EmailBackupProcessor testee = new EmailBackupProcessor(emailSettingsMock.Object);

            // Act
            testee.ProcessBackup(@"./ItemRepository.example.xml");
            string[] files = Directory.GetFiles(@"c:\temp", "*.eml");

            // Assert
            Assert.True(files.Length != 0);
            Assert.True(files[0].Contains(".eml"));
        } 
    }
}
