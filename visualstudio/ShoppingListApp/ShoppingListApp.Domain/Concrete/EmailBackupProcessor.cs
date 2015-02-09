using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;

namespace ShoppingListApp.Domain.Concrete
{
    public class EmailSettings 
    {
        public string MailToAddress { get { return System.Security.Claims.ClaimsPrincipal.Current.Claims.Where(type => type.Type.Contains("emailaddress")).First().Value; } }
        public string MailFromAddress = "shoppinglistappharbor@gmail.com";
        public bool UseSsl = true;
        public string Username = "shoppinglistappharbor@gmail.com";
        public string Password = "cambrai19821981siltzheim";
        public string ServerName = "smtp.gmail.com";
        public int ServerPort = 587;
    }

    public class EmailBackupProcessor : IBackupProcessor
    {
        private EmailSettings emailSettings;

        public EmailBackupProcessor(EmailSettings settings) 
        {
            emailSettings = settings;
        }

        public void ProcessBackup(string fileToBackup) {

            using (var smtpClient = new SmtpClient()) {

                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials= new NetworkCredential(emailSettings.Username, emailSettings.Password);

                StringBuilder body = new StringBuilder()
                    .AppendLine("A new backup is available")
                    .AppendLine("---")
                    .AppendLine(fileToBackup);

                MailMessage mailMessage = new MailMessage(
                                       emailSettings.MailFromAddress,   // From
                                       emailSettings.MailToAddress,     // To
                                      "New backup"+fileToBackup,        // Subject
                                       body.ToString());                // Body

                Attachment fileAttachment = new Attachment(fileToBackup, MediaTypeNames.Application.Octet);
                mailMessage.Attachments.Add(fileAttachment);

                smtpClient.Send(mailMessage);
            }
        }
    }
}
