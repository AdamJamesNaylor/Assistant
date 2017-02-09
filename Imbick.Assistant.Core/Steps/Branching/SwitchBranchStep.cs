
namespace Imbick.Assistant.Core.Steps.Branching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SwitchBranchStep
        : Step {

        public List<SwitchCase> Cases { get; set; }

        public SwitchBranchStep()
            : base("Switch branch step") {
            Cases = new List<SwitchCase>();
        }

        public async override Task<RunResult> Run(WorkflowState workflowState) {
            foreach (var @case in Cases) {
                var allConditionsSatisfied = true;
                foreach (var condition in @case.Conditions) {
                    var conditionResult = await condition.Run(workflowState);
                    if (!conditionResult.Continue) {
                        allConditionsSatisfied = false;
                        break;
                    }
                }

                if (allConditionsSatisfied) {
                    return await @case.Steps.Run(workflowState);
                }
            }

            return RunResult.Failed;
        }
    }
}
