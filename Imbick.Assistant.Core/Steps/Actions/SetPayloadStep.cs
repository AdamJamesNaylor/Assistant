
namespace Imbick.Assistant.Core.Steps.Actions {
    using System.Threading.Tasks;

    public class SetPayloadStep
        : Step {

        public SetPayloadStep(object payload)
            : base("Set payload step") {
            _payload = payload;
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            workflowState.Payload = _payload;
            return RunResult.Passed;
        }

        private readonly object _payload;
    }
}