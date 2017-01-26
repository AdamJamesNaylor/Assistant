
namespace Imbick.Assistant.Service {
    using System.ServiceProcess;
    using System.ComponentModel;
    using System.Configuration.Install;

    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer {
        public ProjectInstaller() {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }

        private void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e) {
            using (ServiceController sc = new ServiceController(serviceInstaller1.ServiceName)) {
                sc.Start();
            }
        }
    }
}