namespace Imbick.Assistant.Core.Steps {
    using System.Threading.Tasks;

    public class CompoundStep
        : Step {

        public StepCollection Steps { get; set; }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            return await Steps.Run(workflowState);
        }

        public CompoundStep()
            : base("Compound step") {
        }
    }
}