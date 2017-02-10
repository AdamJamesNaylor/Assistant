
namespace Imbick.Assistant.Core.Steps.Branching
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;

    public class SwitchBranchStep
        : Step {

        public List<SwitchCase> Cases { get; set; }

        public SwitchBranchStep()
            : base("Switch branch step") {
            Cases = new List<SwitchCase>();
            _logger = LogManager.GetCurrentClassLogger();
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

                if (!allConditionsSatisfied)
                    continue;

                _logger.Trace($"All conditions satisified for case {@case.Name}.");
                return await @case.Steps.Run(workflowState);
            }

            _logger.Trace("No cases were fully satisified.");
            return RunResult.Failed;
        }
        private readonly Logger _logger;
    }
}
