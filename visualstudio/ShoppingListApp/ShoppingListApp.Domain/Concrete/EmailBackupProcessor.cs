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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed.")]
        public void ProcessBackup(string fileToBackup) 
        {
                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.EnableSsl = emailSettings.UseSsl;
                    smtpClient.Host = emailSettings.ServerName;
                    smtpClient.Port = emailSettings.ServerPort;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password);

                    StringBuilder body = new StringBuilder()
                        .AppendLine("A new backup is available")
                        .AppendLine("---")
                        .AppendLine(fileToBackup);

                    MailMessage mailMessage = new MailMessage(
                                           emailSettings.MailFromAddress,   // From
                                           emailSettings.MailToAddress,     // To
                                          "New backup" + fileToBackup,        // Subject
                                           body.ToString());                // Body

                    mailMessage.Attachments.Add(new Attachment(fileToBackup, MediaTypeNames.Application.Octet));

                    smtpClient.Send(mailMessage);
                }
        }
    }
}
