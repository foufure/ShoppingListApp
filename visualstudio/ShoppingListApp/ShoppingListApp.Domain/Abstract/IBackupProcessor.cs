using System.Collections.Generic;
using System.IO;

namespace ShoppingListApp.Domain.Abstract
{
    public interface IBackupProcessor
    {
        void CreateBackup();
        void SecureBackup();
        void RestoreBackup(string backupFileToRestore);
    }
}
