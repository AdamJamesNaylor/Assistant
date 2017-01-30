
namespace Imbick.Assistant.Core {
    using System.Collections.Generic;
    using System.Timers;
    using NLog;

    public class WorkflowRunner {
        private readonly List<Workflow> _workflows = new List<Workflow>();
        private bool _enabled;
        private Logger _logger;
        private readonly int _interval;
        private readonly Timer _timer;

        public WorkflowRunner(int interval) {
            _interval = interval;
            _timer = new Timer(_interval);
        }

        public void Register(IEnumerable<Workflow> workflows) {
            _workflows.AddRange(workflows);
            _enabled = true;
            _logger = LogManager.GetCurrentClassLogger();
        }

        private void Process(object sender, ElapsedEventArgs args) {
            if (!_enabled) {
                _logger.Trace("Runner disabled, not processing.");

                _timer.Enabled = false;
                _timer.Stop();
                return;
            }
            _logger.Trace("Runner enabled, processing.");

            _timer.Enabled = true;
            _timer.Stop();
            foreach (var workflow in _workflows) {
                _logger.Debug($"Running workflow '{workflow.Name}' consisting of {workflow.Steps.Count} steps.");
                workflow.Run();
            }
            _timer.Start();
        }

        public void RunAllWorkflows() {
            _logger.Info($"Running all workflows at {_interval}ms intervals.");

            _timer.Elapsed += Process;
            _timer.Enabled = true;
        }

    }
}