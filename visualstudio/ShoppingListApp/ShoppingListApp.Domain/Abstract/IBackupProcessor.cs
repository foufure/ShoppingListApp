namespace ShoppingListApp.Domain.Abstract
{
    public interface IBackupProcessor
    {
        void ProcessBackup(string fileToBackup);
    }
}
