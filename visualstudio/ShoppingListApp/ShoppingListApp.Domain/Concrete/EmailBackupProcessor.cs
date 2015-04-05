using System.Collections.Generic;
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

        public void ProcessBackup(List<string> filesToBackup)
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
                    foreach (string fileToBackup in filesToBackup)
                    { 
                        if (File.Exists(fileToBackup))
                        {
                            mailMessage.Attachments.Add(new Attachment(fileToBackup, MediaTypeNames.Application.Octet));
                        }
                        else
                        {
                            throw new System.ArgumentNullException(fileToBackup);
                        }
                    }
                
                    smtpClient.Send(mailMessage);
                }
            }     
        }
    }
}
