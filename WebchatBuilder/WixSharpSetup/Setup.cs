using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;
using WixSharpSetup.Dialogs;
using File = WixSharp.File;

namespace WixSharpSetup
{
    class Setup
    {
        static void Main()
        {
            var project = new ManagedProject("WebchatBuilder",
                new RegValueProperty("IISMAJORVERSION", RegistryHive.LocalMachine, @"SYSTEM\CurrentControlSet\Services\W3SVC\Parameters", "MajorVersion", "0"),
                new LaunchCondition("Installed OR (IISMAJORVERSION AND IISMAJORVERSION >= \"#7\")", "IIS 7 or higher is required."),
                new Dir(@"%ProgramFiles%\Qsect\Webchat Builder",
                //new Dir(@"D:\Qsect\Webchat Builder",
                    new Dir("WCB Service",
                    //new DirFiles(@"..\Release\Service\*.*", f => !f.EndsWith(".pdb") && !f.EndsWith(".obj"))),
                    new DirFiles(@"..\Release\Service_x86\*.*", f => !f.EndsWith(".pdb") && !f.EndsWith(".obj"))),
                    new Dir("WCB Web",
                    new Files(@"..\Release\Wcb\*.*", f => !f.EndsWith(".less") && !f.EndsWith(".map") && !f.EndsWith(".pdb") && !f.EndsWith(".obj")))))
                    {
                        GUID = new Guid("e862184a-8bce-4281-bca9-d81242902fb0"),
                        OutDir = @"..\Build",
                        LicenceFile = @"..\Resources\Wcb-License.rtf",
                        ControlPanelInfo = new ProductInfo
                        {
                            ProductIcon = @"..\Resources\icon.ico",
                            UrlInfoAbout = "https://qsect.com/webchatbuilder",
                            HelpLink = "https://qsect.com/support",
                            Manufacturer = "Qsect LLC"
                        },
                        BackgroundImage = @"..\Resources\SetupBackground.bmp",
                        BannerImage = @"..\Resources\SetupBanner.bmp",
                        Version = new Version("1.0.0.0"),
                        LocalizationFile = "localization.wxl"
                    };

            project.SetNetFxPrerequisite("NETFRAMEWORK45 >= '#378389'", "Please install the .Net Framework 4.5 first.");

            var service = project.ResolveWildCards().FindFile((f) => f.Name.EndsWith("WebChatBuilderService.exe")).First();
            service.ServiceInstaller = new ServiceInstaller
            {
                Name = "Webchat Builder Service",
                StartOn = SvcEvent.Install,
                StopOn = SvcEvent.InstallUninstall_Wait,
                RemoveOn = SvcEvent.Uninstall_Wait,
            };

            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add<WelcomeDialog>()
                                            .Add<LicenceDialog>()
                                            .Add<WebSettingsDialog>()
                                            .Add<CicSettingsDialog>()
                                            .Add<SharedSettingsDialog>()
                                            .Add<LicenseKeyDialog>()
                                            .Add<ProgressDialog>()
                                            .Add<ExitDialog>();

            project.ManagedUI.ModifyDialogs.Add<MaintenanceTypeDialog>()
                                           .Add<ProgressDialog>()
                                           .Add<ExitDialog>();

            project.AfterInstall += project_AfterInstall;
            //project.PreserveTempFiles = true;
            project.DefaultDeferredProperties = "INSTALLDIR, UILevel, IIS_WEBSITENAME, IIS_POOL_NAME, WCB_DOMAIN, WCB_ALLOWED_DOMAINS, IIS_WEBSITEIPADDRESS, IIS_WEBSITEPORT, CIC_USER, CIC_PASS, CIC_PRIMARY, CIC_SECONDARY, CIC_PORT, CIC_PROTOCOL, WCB_LICENSE_KEY, WCB_PORT, WCB_CONNECTION_STRING";
            project.OutFileName = "WebchatBuilderSetup";
            project.BuildMsi();
        }

        static void project_AfterInstall(SetupEventArgs e)
        {
            if (!e.IsInstalled)
            {
                OnInstall(e.Session);
            }
            else
            {
                //OnUninstall(e.Session);
            }
        }

        //public static ActionResult OnUninstall(Session session)
        //{
        //    session.Log("Running Post Uninstall Task");
        //    return session.HandleErrors(() =>
        //    {

        //    });
        //}

        public static ActionResult OnInstall(Session session)
        {
            //System.Diagnostics.Debugger.Launch();
            session.Log("Running Post Install Task");
            var installDir = session.Property("INSTALLDIR");
            session.Log("Install Directory: " + installDir);
            session.Log("UI Level" + session.Property("UILevel"));

            var servicePath = installDir + "WCB Service\\";
            var appConfigFile = servicePath + "WebChatBuilderService.exe.config";
            var webPath = installDir + "WCB Web\\";
            var webConfigFile = webPath + "Web.config";

            session.Log("------------- " + appConfigFile);
            session.Log("------------- " + webConfigFile);

            var siteName = session.Property("IIS_WEBSITENAME");
            var appPoolName = session.Property("IIS_POOL_NAME");
            var ipAddr = session.Property("IIS_WEBSITEIPADDRESS");
            var port = session.Property("IIS_WEBSITEPORT");

            if (String.IsNullOrEmpty(siteName))
            {
                siteName = "WebchatBuilder";
            }

            if (String.IsNullOrEmpty(appPoolName))
            {
                appPoolName = "WebchatBuilder";
            }

            if (String.IsNullOrEmpty(ipAddr))
            {
                ipAddr = "*";
            }
            else
            {
                if (ipAddr != "*")
                {
                    IPAddress ip;
                    if (!IPAddress.TryParse(ipAddr, out ip))
                    {
                        ipAddr = "*";
                    }

                }
            }

            if (String.IsNullOrEmpty(port))
            {
                port = "80";
            }
            else
            {
                int validatedPort;
                if (!int.TryParse(port, out validatedPort))
                {
                    port = "80";
                }
            }

            var domain = session.Property("WCB_DOMAIN");
            var allowedDomains = session.Property("WCB_ALLOWED_DOMAINS");

            var cicUser = session.Property("CIC_USER");
            var cicPass = session.Property("CIC_PASS");
            var cicPrimary = session.Property("CIC_PRIMARY");
            var cicSecondary = session.Property("CIC_SECONDARY");
            var cicPort = session.Property("CIC_PORT");
            var cicProtocol = session.Property("CIC_PROTOCOL");
            var licenseKey = session.Property("WCB_LICENSE_KEY");
            var wcbPort = session.Property("WCB_PORT");
            var connectionString = session.Property("WCB_CONNECTION_STRING");
            connectionString = connectionString.Replace("|~|", ";");
            //session.Log(connectionString);

            return session.HandleErrors(() =>
            {
                session.Log("Updating App Config");
                UpdateServiceAppConfig(appConfigFile, connectionString, cicUser, cicPass, cicPrimary, cicSecondary, wcbPort);
                session.Log("Updating Web Config");
                UpdateSiteWebConfig(webConfigFile, connectionString, cicPort, cicProtocol, wcbPort, domain, allowedDomains);
                session.Log("Configuring IIS");
                CreateAppPoolAndSite(siteName, appPoolName, webPath, port, ipAddr, domain);
                session.Log("Configuring IIS Permissions");
                SetPermissionForAppPool(installDir + "WCB Web", appPoolName);
                session.Log("Saving License File");
                CreateLicenseFile(servicePath, licenseKey);
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

        static void CreateAppPoolAndSite(string siteName, string appPoolName, string path, string port, string ip, string domain)
        {
            using (var sm = new ServerManager())
            {
                var invalidChars = SiteCollection.InvalidSiteNameCharacters();
                if (siteName.IndexOfAny(invalidChars) > -1)
                {
                    throw new Exception(String.Format("Invalid Site Name: {0}", siteName));
                }

                var bindingInfo = String.Format("{0}:{1}:{2}", ip, port, domain);
                var site = sm.Sites.Add(siteName, "http", bindingInfo, path);
                site.ServerAutoStart = true;
                
                var appPool = sm.ApplicationPools.Add(appPoolName);
                appPool.ManagedRuntimeVersion = "v4.0";
                appPool.ManagedPipelineMode = ManagedPipelineMode.Integrated;

                site.ApplicationDefaults.ApplicationPoolName = appPoolName;

                sm.CommitChanges();
            }
        }

        static void CreateLicenseFile(string path, string licenseKey)
        {
            if (!String.IsNullOrEmpty(licenseKey))
            {
                var licenseFilePath = path + "license.wcblic";
                using (var sr = new StreamWriter(licenseFilePath))
                {
                    sr.Write(licenseKey);
                }
            }
        }

        static void UpdateServiceAppConfig(string configFile, string connectionString, string cicUser, string cicPass, string primary, string secondary, string wcbPort)
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

        static void UpdateSiteWebConfig(string configFile, string connectionString, string cicPort, string cicProtocol, string wcbPort, string domain, string allowedDomains)
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
    }
}
