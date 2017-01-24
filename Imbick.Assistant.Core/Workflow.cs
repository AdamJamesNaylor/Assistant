namespace Imbick.Assistant.Core {
    using System.Collections.Generic;
    using NLog;

    public class Workflow {
        public string Name { get; private set; }
        public IReadOnlyCollection<IRunnable> Steps => _steps.AsReadOnly();

        public Workflow(string name) {
            Name = name;
            _steps = new List<IRunnable>();
            _parameters = new Dictionary<string, WorkflowParameter>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Run() {
            foreach (var step in Steps) {
                var result = step.Run(_parameters);
                if (!result.Continue)
                    break;
            }
            _parameters.Clear();
        }

        public void AddStep(IRunnable step) {
            //_logger.Trace();
            _steps.Add(step);
        }

        private readonly Dictionary<string, WorkflowParameter> _parameters;
        private readonly List<IRunnable> _steps;
        private Logger _logger;
    }
}