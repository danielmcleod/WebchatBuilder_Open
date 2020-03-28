using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace WcbWebSetupCustomAction
{
    [RunInstaller(true)]
    public class CustomSetupAction : Installer
    {
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            string dbConnectionString = Context.Parameters["DBCONN"];
            string wcbPort = Context.Parameters["WCBPORT"];
            string cicPort = Context.Parameters["CICPORT"];
            string cicProtocol = Context.Parameters["CICPROTOCOL"];
            string wcbDomain = Context.Parameters["WCBDOMAIN"];
            string allowedIps = Context.Parameters["ALLOWEDIPS"];
            string allowedDomains = Context.Parameters["ALLOWEDDOMAINS"];

            string path = Context.Parameters["assemblypath"];
            path = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));
            path = Path.Combine(path, "Web.config");
            var config = ConfigurationManager.OpenExeConfiguration(path);

            if (dbConnectionString != null)
            {
                var cs = config.ConnectionStrings;
                cs.ConnectionStrings["Repository"].ConnectionString = dbConnectionString;
            }
            var appSettings = config.AppSettings;
            if (cicPort != null)
            {
                appSettings.Settings["CicServerPort"].Value = cicPort;
            }
            if (cicProtocol != null)
            {
                appSettings.Settings["CicServerProtocol"].Value = cicProtocol;
            }
            if (wcbPort != null)
            {
                appSettings.Settings["WcbServicePort"].Value = wcbPort;
            }
            if (wcbDomain != null)
            {
                appSettings.Settings["WcbDomain"].Value = wcbDomain;
            }
            if (allowedIps != null)
            {
                appSettings.Settings["AllowedIPAddresses"].Value = allowedIps;
            }
            if (allowedDomains != null)
            {
                appSettings.Settings["AllowedDomains"].Value = allowedDomains;
            }
            config.Save();
            base.Install(stateSaver);
        }
    }
}
