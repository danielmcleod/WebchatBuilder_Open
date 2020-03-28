using System;
using System.Configuration;
using System.IO;
using System.Security.AccessControl;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;


namespace WcbSetupCustomAction
{
    public class CustomAction
    {
        [CustomAction]
        public static ActionResult OnInstall(Session session)
        {
            MessageBox.Show("OnInstall");

            //System.Diagnostics.Debugger.Launch();
            session.Log("Running Post Install Task");
            session.Log("------------- " + session.Property("INSTALLDIR"));

            var installDir = session.Property("INSTALLDIR");

            //APP_CONFIG_FILE=[INSTALLDIR]WCB Service\WebChatBuilderService.exe.config, WEB_CONFIG_FILE=[INSTALLDIR]WCB Web\WebChatBuilder.exe.config

            var appConfigFile = installDir + "WCB Service\\WebChatBuilderService.exe.config";
            var webConfigFile = installDir + "WCB Web\\WebChatBuilder.exe.config";

            session.Log("------------- " + appConfigFile);
            session.Log("------------- " + webConfigFile);

            var siteName = session.Property("IIS_WEBSITENAME");
            session.Log("------------- " + siteName);
            var appPoolName = session.Property("IIS_POOL_NAME");
            session.Log("------------- " + appPoolName);
            var domain = session.Property("WCB_DOMAIN");
            session.Log("------------- " + domain);
            var allowedDomains = session.Property("WCB_ALLOWED_DOMAINS");
            session.Log("------------- " + allowedDomains);
            var ipAddr = session.Property("IIS_WEBSITEIPADDRESS");
            session.Log("------------- " + ipAddr);
            var port = session.Property("IIS_WEBSITEPORT");
            session.Log("------------- " + port);
            var cicUser = session.Property("CIC_USER");
            session.Log("------------- " + cicUser);
            var cicPass = session.Property("CIC_PASS");
            var cicPrimary = session.Property("CIC_PRIMARY");
            session.Log("------------- " + cicPrimary);
            var cicSecondary = session.Property("CIC_SECONDARY");
            session.Log("------------- " + cicSecondary);
            var cicPort = session.Property("CIC_PORT");
            session.Log("------------- " + cicPort);
            var cicProtocol = session.Property("CIC_PROTOCOL");
            session.Log("------------- " + cicProtocol);
            var licenseKey = session.Property("WCB_LICENSE_KEY");
            var wcbPort = session.Property("WCB_PORT");
            session.Log("------------- " + wcbPort);
            var connectionString = session.Property("WCB_CONNECTION_STRING");

            return session.HandleErrors(() =>
            {
                UpdateServiceAppConfig(appConfigFile, connectionString, cicUser, cicPass, cicPrimary, cicSecondary, wcbPort, session);
                UpdateSiteWebConfig(webConfigFile, connectionString, cicPort, cicProtocol, wcbPort, domain, allowedDomains, session);
                //SetPermissionForAppPool(installDir + "WCB Web", appPoolName);
            });

        }

        static void SetPermissionForAppPool(string webDir, string appPoolName)
        {
            var directorySecurity = Directory.GetAccessControl(webDir);

            var rule = new FileSystemAccessRule(
                @"IIS AppPool\" + appPoolName,
                FileSystemRights.FullControl,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow);
            directorySecurity.AddAccessRule(rule);
            Directory.SetAccessControl(webDir, directorySecurity);
        }

        public static void UpdateServiceAppConfig(string configFile, string connectionString, string cicUser, string cicPass, string primary, string secondary, string wcbPort, Session session)
        {
            try
            {
                var config = ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap { ExeConfigFilename = configFile }, ConfigurationUserLevel.None);

                config.AppSettings.Settings["CicUser"].Value = cicUser;
                config.AppSettings.Settings["CicPassword"].Value = cicPass;
                config.AppSettings.Settings["PrimaryServer"].Value = primary;
                config.AppSettings.Settings["SecondaryServer"].Value = secondary;
                config.AppSettings.Settings["ServicePort"].Value = wcbPort;

                var section = config.ConnectionStrings;
                section.ConnectionStrings["Repository"].ConnectionString = connectionString;

                config.Save();
            }
            catch (Exception e)
            {
                session.Log("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    session.Log("Inner Exception: " + e.InnerException.Message);
                }
            }
        }

        public static void UpdateSiteWebConfig(string configFile, string connectionString, string cicPort, string cicProtocol, string wcbPort, string domain, string allowedDomains, Session session)
        {
            try
            {
                var config = ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap { ExeConfigFilename = configFile }, ConfigurationUserLevel.None);

                config.AppSettings.Settings["CicServerPort"].Value = cicPort;
                config.AppSettings.Settings["CicServerProtocol"].Value = cicProtocol;
                config.AppSettings.Settings["WcbServicePort"].Value = wcbPort;
                config.AppSettings.Settings["AllowedDomains"].Value = allowedDomains;
                config.AppSettings.Settings["WcbDomain"].Value = domain;

                var section = config.ConnectionStrings;
                section.ConnectionStrings["Repository"].ConnectionString = connectionString;

                config.Save();
            }
            catch (Exception e)
            {
                session.Log("Exception: " + e.Message);
                if (e.InnerException != null)
                {
                    session.Log("Inner Exception: " + e.InnerException.Message);
                }
            }
        }
    }
}
