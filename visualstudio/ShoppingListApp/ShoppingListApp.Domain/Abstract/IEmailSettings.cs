namespace ShoppingListApp.Domain.Abstract
{
    public interface IEmailSettings
    {
        string MailToAddress { get; }
        
        string MailFromAddress { get; }
        
        bool UseSsl { get; }
        
        string UserName { get; }
        
        string Password { get; }
        
        string ServerName { get; }
       
        int ServerPort { get; }
    }
}
