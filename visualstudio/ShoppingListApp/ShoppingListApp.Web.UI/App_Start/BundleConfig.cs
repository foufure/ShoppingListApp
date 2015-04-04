using System.Web.Optimization;

namespace ShoppingListApp.Web.UI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/Content/*.css"));

            bundles.Add(new ScriptBundle("~/bundles/shoppinglistappscripts")
                .Include(
                    "~/Scripts/jquery-{version}.js",
                    "~/Scripts/jquery.validate.js",
                    "~/Scripts/jquery.validate.unobtrusive.js",
                    "~/Scripts/jquery.unobtrusive-ajax.js",
                    "~/Scripts/jquery.datetimepicker.js",
                    "~/Scripts/DatePickerShoppingListApp.js",
                    "~/Scripts/StrikeThroughToggler.js",
                    "~/Scripts/bootstrap-filestyle.js",
                    "~/Scripts/jquery.bootstrap-touchspin.js",
                    "~/Scripts/QuantitySpinner.js",
                    "~/Scripts/bootstrap.js",
                    "~/Scripts/jquery-ui-{version}.js",
                    "~/Scripts/jquery.ui.touch-punch.js",
                    "~/Scripts/jquery.dataTables.js",
                    "~/Scripts/jquery.dataTables.rowReordering.js",
                    "~/Scripts/TooltipConfig.js"));
        }
    }
}