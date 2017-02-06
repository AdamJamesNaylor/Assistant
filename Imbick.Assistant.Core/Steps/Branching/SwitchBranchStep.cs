
namespace Imbick.Assistant.Core.Steps.Branching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Conditions;

    public class SwitchCase {
        public string Name { get; set; }
        public List<ConditionStep> Conditions { get; set; }
        public List<Step> Steps { get; set; } 
    }

    public class SwitchBranchStep
        : Step {

        public List<SwitchCase> Cases { get; set; }

        public SwitchBranchStep()
            : base("Switch branch step") {
        }

        public override Task<StepRunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            foreach (var @case in Cases) {
                
            }
        }
    }
}
