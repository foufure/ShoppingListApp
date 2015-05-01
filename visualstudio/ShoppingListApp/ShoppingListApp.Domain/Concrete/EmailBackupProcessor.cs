using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ShoppingListApp.Domain.Abstract;
using Ionic.Zip;
using System.Text.RegularExpressions;

namespace ShoppingListApp.Domain.Concrete
{
    public class EmailBackupProcessor : IBackupProcessor
    {
        private IEmailSettings emailSettings;
        private IUserInformation userInformation;
        private IDataPathProvider dataPathProvider;

        private string backupFile;
        private string backupContentFilesSearchPattern;
        private string backupRestoreFilePattern;

        public EmailBackupProcessor(IEmailSettings emailSettings, IUserInformation userInformation, IDataPathProvider dataPathProvider) 
        {
            this.emailSettings = emailSettings;
            this.userInformation = userInformation;
            this.dataPathProvider = dataPathProvider;

            this.backupFile = dataPathProvider.DataPath + @"\" + userInformation.UserName + ".backup";
            this.backupContentFilesSearchPattern = userInformation.IsAdmin ? "*.*" : "*" + userInformation.UserName + ".xml";
            this.backupRestoreFilePattern = userInformation.IsAdmin ? ".xml" : userInformation.UserName + ".xml";
        }

        public void CreateBackup()
        {
            using (ZipFile backupZip = new ZipFile(backupFile))
            {
                foreach (string fileName in Directory.GetFiles(dataPathProvider.DataPath, backupContentFilesSearchPattern))
                {
                    backupZip.AddFile(dataPathProvider.DataPath + @"\" + Path.GetFileName(fileName), string.Empty);
                }

                backupZip.Save();
            }
        }

        public void SecureBackup()
        {
            using (SmtpClient smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);
                smtpClient.DeliveryMethod = emailSettings.DeliveryMethod;
                smtpClient.PickupDirectoryLocation = emailSettings.PickupDirectoryLocation;

                StringBuilder body = new StringBuilder()
                        .AppendLine("A new backup is available")
                        .AppendLine("---");

                using (MailMessage mailMessage = new MailMessage(
                                       emailSettings.MailFromAddress,   // From
                                       emailSettings.MailToAddress,     // To
                                       "New backup",    // Subject
                                       body.ToString()))
                {
                    if (File.Exists(backupFile))
                    {
                        mailMessage.Attachments.Add(new Attachment(backupFile, MediaTypeNames.Application.Octet));
                    }
                    else
                    {
                        throw new System.ArgumentNullException(backupFile);
                    }
                
                    smtpClient.Send(mailMessage);
                }
            }

            System.IO.File.Delete(backupFile);
        }

        public void RestoreBackup(string backupFileToRestore)
        {
            using (ZipFile backupZip = ZipFile.Read(backupFileToRestore))
            {
                foreach (ZipEntry zipEntry in backupZip)
                {
                    if (zipEntry.FileName.Contains(backupRestoreFilePattern))
                    {
                        zipEntry.Extract(dataPathProvider.DataPath, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
        }
    }
}
