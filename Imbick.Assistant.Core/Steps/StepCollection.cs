namespace Imbick.Assistant.Core.Steps {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StepCollection<T>
        : List<Step>, IRunnable where T : Step {

        public async Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters)
        {
            foreach (var step in this)
            {
                var stepResult = await step.Run(workflowParameters);
                if (!stepResult.Continue)
                    return RunResult.Failed;
            }
            return RunResult.Passed;
        }
    }

    public class StepCollection
        : StepCollection<Step> { }
}