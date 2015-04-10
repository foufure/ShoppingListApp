using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
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
            this.categoriesRepositoryName = categoriesRepositoryName;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult LogOn()
        {
            if (!System.IO.File.Exists(itemRepositoryName.RepositoryName) && !System.IO.File.Exists(categoriesRepositoryName.RepositoryName))
            { 
                SetDefaults();
            }

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
                List<string> filesToBackup = new List<string>() { itemRepositoryName.RepositoryName, shoppingListsRepositoryName.RepositoryName, categoriesRepositoryName.RepositoryName };
                backupProcessor.ProcessBackup(filesToBackup);
            }
            catch (System.NullReferenceException)
            {
                TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToBackup;
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
        public RedirectToRouteResult RestoreBackup(HttpPostedFileBase itemsToRestoreFile, HttpPostedFileBase shoppingListsToRestoreFile, HttpPostedFileBase categoriesToRestoreFile)
        {
            Restore(itemsToRestoreFile, "restoreitems", itemRepositoryName.RepositoryName, ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreItemsFailure, RepositoriesXsd.Items());
            Restore(shoppingListsToRestoreFile, "restoreshoppinglists", shoppingListsRepositoryName.RepositoryName, ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreShoppingListsFailure, RepositoriesXsd.ShoppingLists());
            Restore(categoriesToRestoreFile, "restorecategories", categoriesRepositoryName.RepositoryName, ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreCategoriesFailure, RepositoriesXsd.Categories());

            return RedirectToAction("Admin");
        }

        [Authorize]
        public RedirectToRouteResult RestoreDefaults()
        {
            SetDefaults();

            TempData["defaults"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.DefaultsRestored;

            return RedirectToAction("Admin");
        }

        [Authorize]
        public RedirectToRouteResult DeleteData()
        {
            System.IO.File.Delete(itemRepositoryName.RepositoryName);
            System.IO.File.Delete(categoriesRepositoryName.RepositoryName);
            System.IO.File.Delete(shoppingListsRepositoryName.RepositoryName);

            TempData["deleted"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.DataDeleted;

            return RedirectToAction("Admin");
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
                backupProcessor.ProcessBackup(new List<string>() {System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\backupAll.bak"});
            }
            catch (System.NullReferenceException)
            {
                TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToBackup;
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

        private void Restore(HttpPostedFileBase fileToRestore, string typeToRestore, string repositoryName, string failureMessage, string repositoryType)
        {
            TempData[typeToRestore] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;

            if (fileToRestore != null && fileToRestore.ContentLength > 0)
            {
                fileToRestore.SaveAs(repositoryName);
                // if (XmlRepositoryValidationExtensions.XmlRepositoryValidation(repositoryType, repositoryName))
                // {
                    TempData[typeToRestore] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("u", ConfiguredCultures.GetCurrentUICulture);
                // }
                // else
                // {
                //     TempData[typeToRestore] = failureMessage;
                // }
            }
        }

        private void SetDefaults()
        {
            string languageSuffix = "";
            if (ConfiguredCultures.GetCurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                languageSuffix = "_" + ConfiguredCultures.GetCurrentUICulture.TwoLetterISOLanguageName;
            }

            System.IO.File.Copy(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\DefaultItemRepository" + languageSuffix + @".xml", itemRepositoryName.RepositoryName, true);
            System.IO.File.Copy(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\DefaultCategoryRepository" + languageSuffix + @".xml", categoriesRepositoryName.RepositoryName, true);
        }
    }
}