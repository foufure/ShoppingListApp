using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using NLog;
using NLog.Interface;
using Quartz;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.JobsScheduler
{
    public class BackupAllJob : IJob
    {
        private IBackupProcessor backupProcessor;
        private string baseSystemPath;
        private string zippedBackupFileName;

        private ILogger backupAllJobLogger;

        public BackupAllJob(IBackupProcessor backupProcessor, IDataPathProvider dataPathProvider, ILogger backupAllJobLogger)
        {
            this.backupProcessor = backupProcessor;
            this.baseSystemPath = dataPathProvider.DataPath;
            this.zippedBackupFileName = this.baseSystemPath + @"\backupAll.bak";

            this.backupAllJobLogger = backupAllJobLogger;
        }

        public void Execute(IJobExecutionContext context)
        {
            backupAllJobLogger.Trace("BackupAllJob Start Execution: " + DateTime.Now.ToString());

            CreateZippedBackupFile(zippedBackupFileName);

            try
            {
                backupProcessor.ProcessBackup(new List<string>() { zippedBackupFileName });
            }
            catch (System.NullReferenceException)
            {
                backupAllJobLogger.Error("No Zip File given as argument!");
            }
            catch (System.ArgumentNullException)
            {
                backupAllJobLogger.Error("Zip File does not exists!");
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException)
            {
                backupAllJobLogger.Error("Email Recipients not reachable!");
            }
            catch (System.Net.Mail.SmtpException)
            {
                backupAllJobLogger.Error("Email could not be sent! Smtp Server down!");
            }

            System.IO.File.Delete(zippedBackupFileName);
            
            backupAllJobLogger.Trace("BackupAllJob End Execution: " + DateTime.Now.ToString());
        }

        private void CreateZippedBackupFile(string backupFileName)
        {
            using (ZipFile zippedBackupFile = new ZipFile(backupFileName))
            {
                foreach (string fileName in Directory.GetFiles(baseSystemPath, "*.xml"))
                {
                    zippedBackupFile.AddFile(baseSystemPath + @"\" + Path.GetFileName(fileName), string.Empty);
                }

                zippedBackupFile.Save();
            }
        }
    }
}
