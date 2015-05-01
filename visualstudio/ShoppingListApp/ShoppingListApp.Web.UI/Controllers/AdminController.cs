using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Ninject;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Concrete;
using ShoppingListApp.I18N.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class AdminController : Controller
    {
        private IBackupProcessor backupProcessor;
        private IItemsRepository itemRepository;
        private IShoppingListRepository shoppingListsRepository;
        private ICategoryRepository categoriesRepository;
        private IDataPathProvider dataPathProvider;

        public AdminController(IBackupProcessor backupProcessor, IDataPathProvider dataPathProvider, IItemsRepository itemRepository, IShoppingListRepository shoppingListsRepository, ICategoryRepository categoriesRepository)
        {
            this.backupProcessor = backupProcessor;
            this.itemRepository = itemRepository;
            this.shoppingListsRepository = shoppingListsRepository;
            this.categoriesRepository = categoriesRepository;
            this.dataPathProvider = dataPathProvider;
        }

        [Authorize]
        public ActionResult Admin()
        {
            return View();
        }

        [Authorize]
        public RedirectToRouteResult Backup()
        {
            TempData["backup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.BackupMessage + " " + DateTime.Now.ToString("u", CurrentCultureConfiguration.GetCurrentUICulture);

            try
            {
                backupProcessor.CreateBackup();
                backupProcessor.SecureBackup();
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
        public RedirectToRouteResult RestoreDefaults()
        {
            itemRepository.ResetToDefault();
            categoriesRepository.ResetToDefault();

            TempData["defaults"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.DefaultsRestored;

            return RedirectToAction("Admin");
        }

        [Authorize]
        public RedirectToRouteResult DeleteData()
        {
            itemRepository.ResetToEmpty();
            categoriesRepository.ResetToEmpty();
            shoppingListsRepository.ResetToEmpty();

            TempData["deleted"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.DataDeleted;

            return RedirectToAction("Admin");
        }

        [Authorize]
        public RedirectToRouteResult RestoreBackup(HttpPostedFileBase fileToRestore)
        {
            TempData["restoreBackup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.NoFilesToRestore;

            if (fileToRestore != null && fileToRestore.ContentLength > 0)
            {
                string fileNameAndPath = dataPathProvider.DataPath + @"\" + fileToRestore.FileName;
                fileToRestore.SaveAs(fileNameAndPath);

                try
                {
                    backupProcessor.RestoreBackup(fileNameAndPath);
                    TempData["restoreBackup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("u", CurrentCultureConfiguration.GetCurrentUICulture);
                }
                catch (System.Exception)
                {
                    TempData["restoreBackup"] = ShoppingListApp.I18N.Resources.Views.Home.IndexCommon.RestoreItemsFailure;
                }

                System.IO.File.Delete(fileNameAndPath);
            }

            return RedirectToAction("Admin");
        }
    }
}