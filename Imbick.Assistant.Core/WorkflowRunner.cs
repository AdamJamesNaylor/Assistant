
namespace Imbick.Assistant.Core {
    using System.Linq;
    using System.Collections.Generic;
    using System.Timers;

    public class WorkflowRunner {
        private readonly List<Workflow> _workflows = new List<Workflow>();
        private bool _enabled;

        public void Register(Workflow workflow) {
            _workflows.Add(workflow);
            _enabled = true;
        }

        private void Process(object sender, ElapsedEventArgs args) {
            if (!_enabled)
                return;

            foreach (var workflow in _workflows) {
                workflow.Run();
            }
        }

        public void RunAllWorkflows() {
            Process(null, null);
            return;
            var timer = new Timer(1000);

            timer.Elapsed += Process;

            timer.Enabled = true;

            while (_enabled) {
            }
        }

    }
}