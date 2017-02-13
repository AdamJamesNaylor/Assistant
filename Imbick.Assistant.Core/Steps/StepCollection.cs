namespace Imbick.Assistant.Core.Steps {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StepCollection<T>
        : List<Step>, IRunnable where T : Step {

        public async Task<RunResult> Run(WorkflowState workflowState) {
            foreach (var step in this) {
                var stepResult = await step.Run(workflowState);
                if (!stepResult.Continue)
                    return RunResult.Failed;
            }
            return RunResult.Passed;
        }
    }

    public class StepCollection
        : StepCollection<Step> {
    }
}