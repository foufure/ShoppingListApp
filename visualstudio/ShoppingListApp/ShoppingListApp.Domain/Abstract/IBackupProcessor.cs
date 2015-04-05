using System.Collections.Generic;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IBackupProcessor
    {  
        void ProcessBackup(List<string> filesToBackup);
    }
}
