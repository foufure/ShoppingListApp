namespace ShoppingListApp.Domain.Abstract
{
    public interface IUserInformation
    {
        string UserName { get; }
        
        string UserEmail { get; }
    }
}
