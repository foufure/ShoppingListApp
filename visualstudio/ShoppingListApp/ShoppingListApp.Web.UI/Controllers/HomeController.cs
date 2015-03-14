using System;
using System.Web;
using System.Web.Mvc;
using Ninject;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.I18N.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private IBackupProcessor backupProcessor;
        private IRepositoryNameProvider itemRepositoryName;
        private IRepositoryNameProvider shoppingListsRepositoryName;

        public HomeController(IBackupProcessor backupProcessor, [Named("ItemRepositoryName")] IRepositoryNameProvider itemRepositoryName, [Named("ShoppingListRepositoryName")] IRepositoryNameProvider shoppingListsRepositoryName)
        {
            this.backupProcessor = backupProcessor;
            this.itemRepositoryName = itemRepositoryName;
            this.shoppingListsRepositoryName = shoppingListsRepositoryName;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult LogOn()
        {
            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize]
        public RedirectToRouteResult Backup()
        {
            TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.BackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.GetCurrentUICulture);

            try
            {
                backupProcessor.ProcessBackup(itemRepositoryName.RepositoryName);
                backupProcessor.ProcessBackup(shoppingListsRepositoryName.RepositoryName);
            }
            catch (System.ArgumentNullException)
            {
                TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToBackup;
            }
            catch (System.Net.Mail.SmtpFailedRecipientsException)
            {
                TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.DeliveryFailed;
            }
            catch (System.Net.Mail.SmtpException)
            {
                TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.ConnectionFailed;
            }
            
            return RedirectToAction("Admin");
        }

        [Authorize]
        public RedirectToRouteResult RestoreBackup(HttpPostedFileBase itemsToRestoreFile, HttpPostedFileBase shoppingListsToRestoreFile)
        {
            Restore(itemsToRestoreFile, "restoreitems", itemRepositoryName.RepositoryName, ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreItemsFailure);
            Restore(shoppingListsToRestoreFile, "restoreshoppinglists", shoppingListsRepositoryName.RepositoryName, ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreShoppingListsFailure);

            return RedirectToAction("Admin");
        }

        private void Restore(HttpPostedFileBase fileToRestore, string typeToRestore, string repositoryName, string failureMessage)
        {
            TempData[typeToRestore] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;

            if (fileToRestore != null && fileToRestore.ContentLength > 0)
            {
                fileToRestore.SaveAs(repositoryName);
                if (XmlRepositoryValidationExtensions.XmlRepositoryValidation(RepositoriesXsd.Items(), repositoryName))
                {
                    TempData[typeToRestore] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.GetCurrentUICulture);
                }
                else
                {
                    TempData[typeToRestore] = failureMessage;
                }
            }
        }
    }
}