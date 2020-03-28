using System.ServiceProcess;

namespace WebChatBuilderService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            var wcbService = new WcbService();
            wcbService.OnDebug();
            Thread.Sleep(Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new WcbService() 
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
