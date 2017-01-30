
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
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var configurator = new WorkflowConfigurator();
            var workflows = configurator.GetWorkflows();
            var runner = new WorkflowRunner(100);
            runner.Register(workflows);
            runner.RunAllWorkflows();
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args) {
            _logger.Error(args.ExceptionObject);
        }

        protected override void OnStop() {
            _logger.Info("Service stopping.");
        }

        private readonly Logger _logger;
    }
}