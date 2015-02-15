using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.Domain.Abstract;
using ShoppingListApp.Domain.Entities;
using ShoppingListApp.i18n.Utils;
using System.IO;

namespace ShoppingListApp.Web.UI.Controllers
{
    public class HomeController : Controller
    {
        private IBackupProcessor backupProcessor;
        private IUserInformation userInformation;

        public HomeController(IBackupProcessor backupProcessorParam, IUserInformation userInformationParam)
        {
            backupProcessor = backupProcessorParam;
            userInformation = userInformationParam;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult LogIn()
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
            backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ItemRepository." + userInformation.UserName + @".xml");
            backupProcessor.ProcessBackup(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + ".xml");
            TempData["backup"] = ShoppingListApp.i18n.Resources.Views.Home.IndexCommon.BackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.getCurrentUICulture());
            return RedirectToAction("Admin");
        }

        [Authorize]
        public RedirectToRouteResult RestoreBackup(HttpPostedFileBase itemstorestorefile, HttpPostedFileBase shoppingliststorestorefile)
        {
            if (itemstorestorefile != null && itemstorestorefile.ContentLength > 0)
            {
                itemstorestorefile.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ItemRepository." + userInformation.UserName + @".xml");
                TempData["restore"] = ShoppingListApp.i18n.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.getCurrentUICulture());
            }
            
            if (shoppingliststorestorefile != null && shoppingliststorestorefile.ContentLength > 0)
            { 
                shoppingliststorestorefile.SaveAs(System.Web.HttpContext.Current.Server.MapPath("~/App_Data") + @"\ShoppingListRepository." + userInformation.UserName + ".xml");
                TempData["restore"] = ShoppingListApp.i18n.Resources.Views.Home.IndexCommon.RestoreBackupMessage + " " + DateTime.Now.ToString("d", ConfiguredCultures.getCurrentUICulture());
            }

            return RedirectToAction("Admin");
        }
    }
}