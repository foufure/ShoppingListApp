using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingListApp.i18n.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    [AllowAnonymous]
    public class UserCultureController : Controller
    {
        public CultureHelper cultureHelper;

        public UserCultureController(CultureHelper cultureHelperParam)
        {
            cultureHelper = cultureHelperParam;
        }

        public ActionResult GetUserCulture(string userCulture, string returnUrl)
        {
            cultureHelper.SetWantedUserCulture(this.ControllerContext.HttpContext.Response.Cookies, userCulture);
            return Redirect(returnUrl);
        }
    }
}