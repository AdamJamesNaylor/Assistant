
namespace Imbick.Assistant.Core.Steps.Branching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class SwitchBranchStep
        : Step {

        public List<SwitchCase> Cases { get; set; }

        public SwitchBranchStep()
            : base("Switch branch step") {
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            foreach (var @case in Cases) {
                var allConditionsSatisfied = true;
                foreach (var condition in @case.Conditions) {
                    var conditionResult = await condition.Run(workflowParameters);
                    if (!conditionResult.Continue) {
                        allConditionsSatisfied = false;
                        break;
                    }
                }

                if (allConditionsSatisfied) {
                    return await @case.Steps.Run(workflowParameters);
                }
            }

            return RunResult.Failed;
        }
    }
}
