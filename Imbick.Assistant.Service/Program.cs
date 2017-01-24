
namespace Imbick.Assistant.Service {
    using System.ServiceProcess;

    internal static class Program {
        private static void Main() {
            var ServicesToRun = new ServiceBase[] {
                new MainService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}