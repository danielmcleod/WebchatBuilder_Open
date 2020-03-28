using log4net;
using log4net.Config;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.IO;
using System.ServiceProcess;
using WebChatBuilderModels;
using WebChatBuilderModels.Helper;
using WebChatBuilderService.Icelib;

namespace WebChatBuilderService
{
    public partial class WcbService : ServiceBase
    {
        public static IcelibInitializer IcelibInitializer;
        private static readonly ILog Logger = LogManager.GetLogger("WCB");

        public WcbService()
        {
            InitializeComponent();
        }

        public void OnDebug()
        {
            OnStart(null);
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();
            Logger.Info("Starting WCB Service...");

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            Database.SetInitializer<Repository>(new RepositoryInitializer());
            using (var db = new Repository())
            {
                db.Database.Initialize(true);
            }

            DbInterception.Add(new CustomEFInterceptor());

            IcelibInitializer = new IcelibInitializer();
            IcelibInitializer.Start();
            var webApi = new WebApiStartup();
            webApi.Start();
        }

        protected override void OnStop()
        {
            if (IcelibInitializer != null)
            {
                IcelibInitializer.Stop();
            }
        }
    }
}
