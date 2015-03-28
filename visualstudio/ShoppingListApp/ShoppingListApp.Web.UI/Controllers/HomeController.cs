using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Ionic.Zip;
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
        private IRepositoryNameProvider categoriesRepositoryName;

        public HomeController(IBackupProcessor backupProcessor, [Named("ItemRepositoryName")] IRepositoryNameProvider itemRepositoryName, [Named("ShoppingListRepositoryName")] IRepositoryNameProvider shoppingListsRepositoryName, [Named("CategoryRepositoryName")] IRepositoryNameProvider categoriesRepositoryName)
        {
            this.backupProcessor = backupProcessor;
            this.itemRepositoryName = itemRepositoryName;
            this.shoppingListsRepositoryName = shoppingListsRepositoryName;
            this.categoriesRepositoryName = categoriesRepositoryName; //TODO: add backup/restore for categories!!!!
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
            TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.BackupMessage + " " + DateTime.Now.ToString("u", ConfiguredCultures.GetCurrentUICulture);

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

        [Authorize]
        public RedirectToRouteResult RestoreDefaults()
        {
            string languageSuffix = "";
            if (ConfiguredCultures.GetCurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                languageSuffix = "_" + ConfiguredCultures.GetCurrentUICulture.TwoLetterISOLanguageName;
            }

            System.IO.File.Copy(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\DefaultItemRepository" + languageSuffix + @".xml", itemRepositoryName.RepositoryName, true);
            System.IO.File.Copy(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\DefaultCategoryRepository" + languageSuffix + @".xml", categoriesRepositoryName.RepositoryName, true);

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult SuperAdmin()
        {
            return View();
        }

        [Authorize(Users = "Shopping List")]
        public RedirectToRouteResult BackupAll()
        {
            TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.BackupMessage + " " + DateTime.Now.ToString("u", ConfiguredCultures.GetCurrentUICulture);

            using (ZipFile backupAll = new ZipFile(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\backupAll.bak"))
            {
                foreach (string fileName in Directory.GetFiles(System.Web.HttpContext.Current.Server.MapPath("~/App_Data"), "*.xml"))
                {
                    backupAll.AddFile(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\" + Path.GetFileName(fileName), string.Empty);
                }

                backupAll.Save();
            }

            try
            {
                backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\backupAll.bak");
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

            System.IO.File.Delete(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\backupAll.bak");

            return RedirectToAction("SuperAdmin");
        }

        [Authorize(Users = "Shopping List")]
        public RedirectToRouteResult RestoreAllBackups(HttpPostedFileBase fileToRestore)
        {
            TempData["allBackupsToRestore"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;

            if (fileToRestore != null && fileToRestore.ContentLength > 0)
            {
                string fileNameAndPath = System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\" + fileToRestore.FileName;
                fileToRestore.SaveAs(fileNameAndPath);
                
                try
                {
                    using (ZipFile backupAll = ZipFile.Read(fileNameAndPath))
                    {
                        foreach (ZipEntry repositoryFile in backupAll)
                        {
                            repositoryFile.Extract(System.Web.HttpContext.Current.Server.MapPath("~/App_Data"), ExtractExistingFileAction.OverwriteSilently);
                        }
                    }

                    TempData["allBackupsToRestore"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("u", ConfiguredCultures.GetCurrentUICulture);
                }
                catch (System.Exception)
                {
                    TempData["allBackupsToRestore"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreItemsFailure;
                }

                System.IO.File.Delete(fileNameAndPath);
            }

            return RedirectToAction("SuperAdmin");
        }

        private void Restore(HttpPostedFileBase fileToRestore, string typeToRestore, string repositoryName, string failureMessage)
        {
            TempData[typeToRestore] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;

            if (fileToRestore != null && fileToRestore.ContentLength > 0)
            {
                fileToRestore.SaveAs(repositoryName);
                if (XmlRepositoryValidationExtensions.XmlRepositoryValidation(RepositoriesXsd.Items(), repositoryName))
                {
                    TempData[typeToRestore] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("u", ConfiguredCultures.GetCurrentUICulture);
                }
                else
                {
                    TempData[typeToRestore] = failureMessage;
                }
            }
        }
    }
}