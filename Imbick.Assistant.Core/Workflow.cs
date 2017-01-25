namespace Imbick.Assistant.Core {
    using System.Collections.Generic;
    using NLog;
    using Steps;

    public class Workflow {
        public string Name { get; }
        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();

        public Workflow(string name) {
            Name = name;
            _steps = new List<Step>();
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

        public void AddStep(Step step) {
            _logger.Trace($"Added step {step.Name} to workflow {Name}.");
            _steps.Add(step);
        }

        private readonly Dictionary<string, WorkflowParameter> _parameters;
        private readonly List<Step> _steps;
        private readonly Logger _logger;
    }
}