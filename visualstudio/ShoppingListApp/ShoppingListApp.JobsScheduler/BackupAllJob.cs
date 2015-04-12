using System;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using NLog;
using Quartz;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;

namespace ShoppingListApp.JobsScheduler
{
    public class BackupAllJob : IJob
    {
        private static Logger backupAllJobLogger = LogManager.GetCurrentClassLogger();
        private static string baseSystemPath = System.Web.HttpRuntime.AppDomainAppPath + @"App_Data";


        public void Execute(IJobExecutionContext context)
        {
            backupAllJobLogger.Trace("BackupAllJob Start Execution: " + DateTime.Now.ToString());

            // Use Dependency Injection: read the following articles:
            // http://stackoverflow.com/questions/6741599/asp-net-mvc-3-ninject-and-quartz-net-how-to
            // http://stackoverflow.com/questions/22810793/how-to-inject-quartzs-job-with-ninject
            // http://stackoverflow.com/questions/25889255/quartz-net-and-ninject-how-to-bind-implementation-to-my-job-using-ninject
            EmailBackupProcessor backupProcessor = new EmailBackupProcessor(new GoogleEmailSettings(new GoogleUserInformation()));

            string backupZipFileName = baseSystemPath + @"\backupAll.bak";

            zipDirectoryContent(backupZipFileName);

            try
            {
                backupProcessor.ProcessBackup(new List<string>() { backupZipFileName });
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

            System.IO.File.Delete(backupZipFileName);
            
            backupAllJobLogger.Trace("BackupAllJob End Execution: " + DateTime.Now.ToString());
        }

        private void zipDirectoryContent(string zipBackupFileName)
        {
            using (ZipFile backupAll = new ZipFile(zipBackupFileName))
            {
                foreach (string fileName in Directory.GetFiles(baseSystemPath, "*.xml"))
                {
                    backupAll.AddFile(baseSystemPath + @"\" + Path.GetFileName(fileName), string.Empty);
                }

                backupAll.Save();
            }
        }
    }
}
