namespace Imbick.Assistant.Core {
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using Steps;

    public class Workflow
        : IRunnable {
        public string Name { get; }
        public IReadOnlyCollection<Step> Steps => _steps.AsReadOnly();

        public Workflow(string name) {
            Name = name;
            _steps = new StepCollection();
            _parameters = new Dictionary<string, WorkflowParameter>();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters = null)
        {
            foreach (var step in Steps) {
                var result = await step.Run(_parameters);
                if (!result.Continue)
                    break;
            }
            _parameters.Clear();
            return null;
        }

        public void AddStep(Step step) {
            _logger.Trace($"Added step {step.Name} to workflow {Name}.");
            _steps.Add(step);
        }

        private readonly Dictionary<string, WorkflowParameter> _parameters;
        private readonly StepCollection _steps;
        private readonly Logger _logger;
    }
}