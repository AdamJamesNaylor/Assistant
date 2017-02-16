
namespace Imbick.Assistant.Core.Steps.Loops {
    using System.Collections.Generic;
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
            var values = (IEnumerable<T>)workflowState.Payload;
            foreach (var value in values) {
                var subWorkflowState = new WorkflowState {
                    Payload = value
                };
                var result = await Steps.Run(subWorkflowState);
                if (!result.Continue)
                    return RunResult.Failed;
            }
            return RunResult.Passed;
        }

        private Logger _logger;
    }
}