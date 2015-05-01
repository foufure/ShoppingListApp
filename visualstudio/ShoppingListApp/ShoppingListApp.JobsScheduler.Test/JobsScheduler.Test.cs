using System.Collections.Generic;
using System.IO;
using System.Threading;
using Ionic.Zip;
using Moq;
using Ninject;
using NLog.Interface;
using NUnit.Framework;
using Quartz;
using Quartz.Impl;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.JobsScheduler.Test
{
    [TestFixture]
    public class JobsSchedulerTests
    {
        private BackupAllJob backupAllJob;
        private Mock<IBackupProcessor> backupProcessor;
        private Mock<IDataPathProvider> dataPathProvider;
        private Mock<ILogger> loggerFake;

        [SetUp]
        public void Init()
        {
            dataPathProvider = new Mock<IDataPathProvider>();
            dataPathProvider.Setup(provider => provider.DataPath).Returns(@".");
            backupProcessor = new Mock<IBackupProcessor>();

            loggerFake = new Mock<ILogger>();
        }

        [TearDown]
        public static void Dispose()
        {
            if (File.Exists(@".\backupAll.bak.successful")) 
            { 
                File.Delete(@".\backupAll.bak.successful"); 
            }
        }

        [Test]
        public static void JobIsExecuted_WhenCronJobIsStarted()
        {
            // Arrange
            Mock<IJob> mockJob = new Mock<IJob>();
            mockJob.Setup(x => x.Execute(null));

            using (IKernel kernel = new StandardKernel())
            { 
                kernel.Bind<IJob>().ToConstant(mockJob.Object);

                CronJobsScheduler cronJobScheduler = new CronJobsScheduler(new StdSchedulerFactory(), new NinjectJobFactory(kernel));

                // Act
                cronJobScheduler.StartJobScheduler();
                cronJobScheduler.AddJob("* * * ? * *", JobBuilder.Create(mockJob.Object.GetType()).Build());

                // Assert
                Thread.Sleep(3000);
                cronJobScheduler.StandbyJobScheduler();
            }

            Thread.Sleep(3000);
            Assert.DoesNotThrow(() => mockJob.Verify(job => job.Execute(It.IsAny<IJobExecutionContext>()), Times.Between(1,3,Moq.Range.Inclusive)));
        }

        [Test]
        public void BackupIsDone_WhenJobIsExecuted()
        {
            // Arrange
            backupProcessor.Setup(processor => processor.CreateBackup());
            backupProcessor.Setup(processor => processor.SecureBackup())
                .Callback(() => File.Copy(@".\backupAll.bak", @".\backupAll.bak.successful"));
            backupAllJob = new BackupAllJob(backupProcessor.Object, dataPathProvider.Object, loggerFake.Object);

            // Act
            backupAllJob.Execute(null);

            // Assert
            Assert.True(File.Exists(@".\backupAll.bak.successful"));
        }

        [Test]
        public void BackupExceptionArgumentNullExceptionHandledCorrectly_WhenSomethingGoesWrong()
        {
            // Arrange
            backupProcessor.Setup(processor => processor.SecureBackup())
                .Throws(new System.ArgumentNullException());
            backupAllJob = new BackupAllJob(backupProcessor.Object, dataPathProvider.Object, loggerFake.Object);

            // Act
            backupAllJob.Execute(null);

            // Assert
            Assert.DoesNotThrow(() => loggerFake.Verify(logger => logger.Error("Zip File does not exists!"), Times.Exactly(1)));
        }

        [Test]
        public void BackupExceptionSmtpFailedRecipientsExceptionHandledCorrectly_WhenSomethingGoesWrong()
        {
            // Arrange
            backupProcessor.Setup(processor => processor.SecureBackup())
                .Throws(new System.Net.Mail.SmtpFailedRecipientsException());
            backupAllJob = new BackupAllJob(backupProcessor.Object, dataPathProvider.Object, loggerFake.Object);

            // Act
            backupAllJob.Execute(null);

            // Assert
            Assert.DoesNotThrow(() => loggerFake.Verify(logger => logger.Error("Email Recipients not reachable!"), Times.Exactly(1)));
        }

        [Test]
        public void BackupExceptionSmtpExceptionHandledCorrectly_WhenSomethingGoesWrong()
        {
            // Arrange
            backupProcessor.Setup(processor => processor.SecureBackup())
                .Throws(new System.Net.Mail.SmtpException());
            backupAllJob = new BackupAllJob(backupProcessor.Object, dataPathProvider.Object, loggerFake.Object);

            // Act
            backupAllJob.Execute(null);

            // Assert
            Assert.DoesNotThrow(() => loggerFake.Verify(logger => logger.Error("Email could not be sent! Smtp Server down!"), Times.Exactly(1)));
        }
    }
}
