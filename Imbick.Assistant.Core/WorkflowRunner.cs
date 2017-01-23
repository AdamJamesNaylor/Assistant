
namespace Imbick.Assistant.Core {
    using System.Collections.Generic;
    using System.Timers;
    using NLog;

    public class WorkflowRunner {
        private readonly List<Workflow> _workflows = new List<Workflow>();
        private bool _enabled;
        private Logger _logger;
        private readonly int _interval;

        public WorkflowRunner(int interval) {
            _interval = interval;
        }

        public void Register(Workflow workflow) {
            _workflows.Add(workflow);
            _enabled = true;
            _logger = LogManager.GetCurrentClassLogger();
        }

        private void Process(object sender, ElapsedEventArgs args) {
            if (!_enabled)
                return;

            foreach (var workflow in _workflows) {
                _logger.Debug($"Running workflow '{workflow.Name}' consisting of {workflow.Steps.Count} steps.");
                workflow.Run();
            }
        }

        public void RunAllWorkflows() {
            _logger.Info($"Running all workflows at {_interval}ms intervals.");

            using (var timer = new Timer(_interval)) {
                timer.Elapsed += Process;
                timer.Enabled = true;

                while (_enabled) { }
            }
        }

    }
}