using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace ShoppingListApp.Web.UI.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public ActionResult LogOn(string returnUrl)
        {
            // Request a redirect to the external logOn provider
            return new ChallengeResult("Google", Url.Action("ExternalLogOnCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        public RedirectToRouteResult LogOff()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ExternalLogOnCallback(string returnUrl)
        {
            return new RedirectResult(returnUrl);
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LogOnProvider = provider;
                RedirectUri = redirectUri;
            }

            public string LogOnProvider { get; set; }
            
            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                if (context != null)
                { 
                    var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                    context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LogOnProvider);
                }
            }
        }
    }
}