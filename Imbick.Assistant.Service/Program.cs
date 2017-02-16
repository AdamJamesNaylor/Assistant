
namespace Imbick.Assistant.Service {
    using System;
    using System.ServiceProcess;

    internal static class Program {
        private static void Main() {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);

            var ServicesToRun = new ServiceBase[] {
                new MainService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //todo this dependancy on NLog could be moved back into core.
            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Error(e.ExceptionObject);
        }
    }
}