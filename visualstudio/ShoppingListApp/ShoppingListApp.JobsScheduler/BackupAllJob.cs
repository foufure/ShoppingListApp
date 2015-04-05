using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using NLog;
using Quartz;
using ShoppingListApp.Domain.Concrete;

namespace ShoppingListApp.JobsScheduler
{
    public class BackupAllJob : IJob
    {
        private static Logger backupAllJobLogger = LogManager.GetCurrentClassLogger();

        public void Execute(IJobExecutionContext context)
        {
            backupAllJobLogger.Trace("BackupAllJob Start Execution: " + DateTime.Now.ToString());

            EmailBackupProcessor backupProcessor = new EmailBackupProcessor(new GoogleEmailSettings(new GoogleUserInformation()));

            string zipFileName = System.Web.HttpRuntime.AppDomainAppPath + @"App_Data" + @"\backupAll.bak";

            using (ZipFile backupAll = new ZipFile(zipFileName))
            {
                foreach (string fileName in Directory.GetFiles(System.Web.HttpRuntime.AppDomainAppPath + @"App_Data", "*.xml"))
                {
                    backupAll.AddFile(System.Web.HttpRuntime.AppDomainAppPath + @"App_Data" + @"\" + Path.GetFileName(fileName), string.Empty);
                }

                backupAll.Save();
            }

            try
            {
                backupProcessor.ProcessBackup(new List<string>() {zipFileName});
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
                backupAllJobLogger.Error("Email Recipients does not answer!");
            }
            catch (System.Net.Mail.SmtpException)
            {
                backupAllJobLogger.Error("Email could not be sent! Smtp Server down!");
            }

            System.IO.File.Delete(zipFileName);
            backupAllJobLogger.Trace("BackupAllJob End Execution: " + DateTime.Now.ToString());
        }
    }
}
