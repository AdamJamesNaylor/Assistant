
namespace Imbick.Assistant.Core.Steps.Loops {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ForeachLoopStep<T>
        : Step {
    
        public StepCollection Steps { get; set; }

        public ForeachLoopStep()
            : base("Foreach loop step") {
        }

        public async override Task<RunResult> Run(WorkflowState workflowState) {
            var values = (IEnumerable<T>)workflowState.Payload;
            foreach (var value in values) {
                var subWorkflowState = new WorkflowState {
                    Payload = value
                };
                await Steps.Run(subWorkflowState);
            }
            return RunResult.Passed;
        }
    }
}