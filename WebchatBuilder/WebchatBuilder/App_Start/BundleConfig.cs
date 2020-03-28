using System.Web.Optimization;

namespace WebchatBuilder
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/wcb").Include(
                 "~/Scripts/Local/main.js"));
            bundles.Add(new ScriptBundle("~/bundles/settings").Include(
                 "~/Scripts/Local/settings.js"));
            bundles.Add(new ScriptBundle("~/bundles/users").Include(
                 "~/Scripts/Local/users.js"));
            bundles.Add(new ScriptBundle("~/bundles/chat").Include(
                 "~/Scripts/chat.js"));
            //bundles.Add(new ScriptBundle("~/bundles/handlebars").Include(
            //     "~/Scripts/handlebars-v3.0.3.js"));

            // Code removed for clarity.
            BundleTable.EnableOptimizations = true;
        }
    }
}
