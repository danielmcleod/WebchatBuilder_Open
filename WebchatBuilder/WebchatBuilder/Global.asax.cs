using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using log4net.Config;
using WebchatBuilder.Services;
using WebChatBuilderModels;
using WebChatBuilderModels.Helper;

namespace WebchatBuilder
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private readonly string _ignoreAllCertErrors = ConfigurationManager.AppSettings["IgnoreAllCertErrors"];
        private static readonly ILog Logger = LogManager.GetLogger("WCB");

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            XmlConfigurator.Configure();
            Logger.Info("Starting WCB Web...");

            //Database.SetInitializer<Repository>(null);
            Database.SetInitializer<Repository>(new RepositoryInitializer());
            using (var db = new Repository())
            {
                db.Database.Initialize(true);
            }

            DbInterception.Add(new CustomEFInterceptor());

            ServicePointManager.UseNagleAlgorithm = false;
            ServicePointManager.DefaultConnectionLimit = 1000000;
            ServicePointManager.ServerCertificateValidationCallback += ServerCertificateValidationCallback;// += (sender, certificate, chain, errors) => errors == SslPolicyErrors.None && validCerts.Contains(certificate);
        }

        private bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            LoggingService.GetInstance().LogNote("Certificate Callback - Certificate Received: " + certificate.Subject);
            var validCerts = new X509Certificate2Collection();
            
            var localMachineStore = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            localMachineStore.Open(OpenFlags.ReadOnly);
            validCerts.AddRange(localMachineStore.Certificates);
            localMachineStore.Close();

            var hasErrors = sslPolicyErrors != SslPolicyErrors.None;
            var certificateFound = false;
            foreach (var chainElement in chain.ChainElements)
            {
                if (validCerts.Contains(chainElement.Certificate))
                {
                    LoggingService.GetInstance().LogNote("Certificate Callback - Certificate Found: " + chainElement.Certificate.Subject);
                    certificateFound = true;
                }
            }
            LoggingService.GetInstance().LogNote("Certificate Callback - Has Errors?: " + hasErrors);
            if (hasErrors)
            {
                LoggingService.GetInstance().LogNote("Certificate Callback - Certificate Found?: " + certificateFound);
                if (!certificateFound)
                {
                    if (!String.IsNullOrWhiteSpace(_ignoreAllCertErrors) && _ignoreAllCertErrors.ToLowerInvariant() == "true")
                    {
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Context.Request.Path.Contains("signalr/"))
            {
                Context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                Context.Response.AppendHeader("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
            }
            else if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                var origin = Request.Headers.Get("Origin");
                if (ChatServices.AllowedDomains.Any(a => origin.ToLower().EndsWith(a.ToLower())))
                {
                    Context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
                    Context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
                    Context.Response.Headers.Add("Access-Control-Allow-Methods", "GET,POST,OPTIONS");
                    Context.Response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
                }
                Response.Flush();
            }
            //else if (Request.UrlReferrer != null)
            //{
            //    var referrer = Request.UrlReferrer.Host;
            //    if (!ChatServices.AllowedDomains.Any(a => referrer.ToLower().EndsWith(a.ToLower())))
            //    {
            //        Response.StatusCode = 404;
            //        Response.End();
            //    }
            //}
        }

        void Session_Start(object sender, EventArgs e)
        {
            try
            {
                string sessionId = Session.SessionID;
                HttpContext.Current.Session.Add("__WcbChatSession", string.Empty);
            }
            catch (Exception)
            {
            }
        }
    }
}
