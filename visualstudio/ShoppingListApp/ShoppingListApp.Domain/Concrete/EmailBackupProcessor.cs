using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class EmailBackupProcessor : IBackupProcessor
    {
        private IEmailSettings emailSettings;

        public EmailBackupProcessor(IEmailSettings settings) 
        {
            emailSettings = settings;
        }

        public void ProcessBackup(string fileToBackup)
        {
            if (File.Exists(fileToBackup))
            {
                using (SmtpClient smtpClient = new SmtpClient())
                using (Attachment backupAttachment = new Attachment(fileToBackup, MediaTypeNames.Application.Octet))
                {
                    smtpClient.EnableSsl = emailSettings.UseSsl;
                    smtpClient.Host = emailSettings.ServerName;
                    smtpClient.Port = emailSettings.ServerPort;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);

                    // Conditional - only for tests
                    GenerateEmailAsAFileForUnitTesting(smtpClient);

                    StringBuilder body = new StringBuilder()
                        .AppendLine("A new backup is available")
                        .AppendLine("---")
                        .AppendLine(fileToBackup);

                    using (MailMessage mailMessage = new MailMessage(
                                           emailSettings.MailFromAddress,   // From
                                           emailSettings.MailToAddress,     // To
                                           "New backup " + fileToBackup,    // Subject
                                           body.ToString()))
                    {
                        mailMessage.Attachments.Add(backupAttachment);
                        smtpClient.Send(mailMessage);
                    }
                }
            }
            else
            {
                throw new System.ArgumentNullException(fileToBackup);
            }
        }

        // [Conditional("NUNIT_UNITTEST")]
        private static void GenerateEmailAsAFileForUnitTesting(SmtpClient smtpClient)
        {
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            smtpClient.PickupDirectoryLocation = @"c:\temp";
            smtpClient.EnableSsl = false;
        }
    }
}
