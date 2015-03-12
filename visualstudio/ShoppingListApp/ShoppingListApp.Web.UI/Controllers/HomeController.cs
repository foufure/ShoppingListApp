using System;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.I18N.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private IBackupProcessor backupProcessor;
        private IUserInformation userInformation;

        public HomeController(IBackupProcessor backupProcessor, IUserInformation userInformation)
        {
            this.backupProcessor = backupProcessor;
            this.userInformation = userInformation;
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
                backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ItemRepository." + userInformation.UserName + @".xml");
                backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + ".xml");
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
            TempData["restoreitems"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;
            TempData["restoreshoppinglists"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;

            if (itemsToRestoreFile != null && itemsToRestoreFile.ContentLength > 0)
            {
                itemsToRestoreFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ItemRepository." + userInformation.UserName + @".xml");
                if (XmlRepositoryValidationExtensions.XmlRepositoryValidation(RepositoriesXsd.Items(), System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ItemRepository." + userInformation.UserName + @".xml"))
                {   
                    TempData["restoreitems"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.GetCurrentUICulture);
                }
                else
                {
                    TempData["restoreitems"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreItemsFailure;
                }
            }
            
            if (shoppingListsToRestoreFile != null && shoppingListsToRestoreFile.ContentLength > 0)
            {
                shoppingListsToRestoreFile.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + ".xml");
                if (XmlRepositoryValidationExtensions.XmlRepositoryValidation(RepositoriesXsd.Items(), System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + @".xml"))
                {
                    TempData["restoreshoppinglists"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.GetCurrentUICulture);           
                }
                else
                {
                    TempData["restoreshoppinglists"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreShoppingListsFailure;
                }
            }

            return RedirectToAction("Admin");
        }
    }
}