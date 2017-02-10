
namespace Imbick.Assistant.Core.Steps.Loops {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NLog;

    public class ForeachLoopStep<T>
        : Step {

        public StepCollection Steps { get; set; }

        public ForeachLoopStep()
            : base("Foreach loop step") {
            Steps = new StepCollection();
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            _logger.Trace("Looping!!!!!!!!");
            var values = (IEnumerable<T>)workflowState.Payload;
            _logger.Trace($"Enumerating {values.Count()} items.");
            foreach (var value in values) {
                var subWorkflowState = new WorkflowState {
                    Payload = value
                };
                await Steps.Run(subWorkflowState);
            }
            return RunResult.Passed;
        }

        private Logger _logger;
    }
}