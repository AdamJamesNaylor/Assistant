
namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using NLog;
    using Steps;

    public class WorkflowRunner
    : IDisposable {
        public WorkflowRunner(int interval) {
            _interval = interval;
        }

        public void Register(IEnumerable<Workflow> workflows) {
            _workflows.AddRange(workflows);
            _logger = LogManager.GetCurrentClassLogger();
        }

        private void Process(object stateInfo) {
            _logger.Trace("Runner processing all workflows.");

            foreach (var workflow in _workflows) {
                if (workflow.IsRunning || !workflow.IsEnabled)
                    continue;

                _logger.Debug($"Running workflow '{workflow.Name}' consisting of {workflow.Steps.Count} steps.");
                workflow.Run(new WorkflowState()); //todo where is the best place to create this state?
            }
        }

        public void RunAllWorkflows() {
            _logger.Info($"Running all workflows at {_interval}ms intervals.");
            var autoEvent = new AutoResetEvent(false);
            _timer = new Timer(Process, autoEvent, _interval, _interval);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private readonly List<Workflow> _workflows = new List<Workflow>();
        private Logger _logger;
        private readonly int _interval;
        private Timer _timer;
    }
}