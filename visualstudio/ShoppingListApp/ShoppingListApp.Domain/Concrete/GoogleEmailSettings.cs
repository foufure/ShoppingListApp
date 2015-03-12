using System.Net.Mail;
using ShoppingListApp.Domain.Abstract;

namespace ShoppingListApp.Domain.Concrete
{
    public class GoogleEmailSettings : IEmailSettings
    {
        private IUserInformation userInformation;

        public GoogleEmailSettings(IUserInformation userInformation)
        {
            this.userInformation = userInformation;
        }

        public string MailToAddress
        {
            get { return userInformation.UserEmail; }
        }

        public string MailFromAddress
        {
            get { return "shoppinglistappharbor@gmail.com"; }
        }

        public bool UseSsl
        {
            get { return true; }
        }

        public string UserName
        {
            get { return "shoppinglistappharbor@gmail.com"; }
        }

        public string Password
        {
            get { return "cambrai19821981siltzheim"; }
        }

        public string ServerName
        {
            get { return "smtp.gmail.com"; }
        }

        public int ServerPort
        {
            get { return 587; }
        }

        public SmtpDeliveryMethod DeliveryMethod
        {
            get { return SmtpDeliveryMethod.Network; }
        }

        public string PickupDirectoryLocation 
        {
            get { return string.Empty; } 
        }
    }
}
