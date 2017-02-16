
namespace Imbick.Assistant.Service {
    using System;
    using Core;
    using System.ServiceProcess;
    using NLog;

    public partial class MainService
        : ServiceBase {

        public MainService() {
            _logger = LogManager.GetCurrentClassLogger();
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            Start();
        }

        private void Start() {
            _logger.Info("Service starting.");
            try
            {
                var configurator = new WorkflowConfigurator();
                var workflows = configurator.GetWorkflows();
                var runner = new WorkflowRunner(100);
                runner.Register(workflows);
                runner.RunAllWorkflows();
            } catch (Exception ex) {
                _logger.Error(ex);
                StopService();
                throw ex;
            }
        }

        protected override void OnStop() {
            StopService();
        }

        private void StopService()
        {
            _logger.Info("Service stopping.");
        }

        private readonly Logger _logger;
    }
}