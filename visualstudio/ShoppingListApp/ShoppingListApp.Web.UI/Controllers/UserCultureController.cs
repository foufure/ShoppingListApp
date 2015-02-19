using System.Web.Mvc;
using ShoppingListApp.I18N.Utils;

namespace ShoppingListApp.Web.UI.Controllers
{
    [AllowAnonymous]
    public class UserCultureController : Controller
    {
        public ActionResult GetUserCulture(string userCulture, string returnUrl)
        {
            CultureHelper.SetWantedUserCulture(this.ControllerContext.HttpContext.Response.Cookies, userCulture);
            return Redirect(returnUrl);
        }
    }
}