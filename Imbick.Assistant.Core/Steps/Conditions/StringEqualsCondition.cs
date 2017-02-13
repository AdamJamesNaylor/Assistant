namespace Imbick.Assistant.Core.Steps.Conditions {
    using System;
    using System.Threading.Tasks;
    using Steps;

    public class StringEqualsConditionStep
        : ConditionStep {

        public StringEqualsConditionStep(Func<WorkflowState, string> valueResolver, string operand)
            : base("String equals condition") {
            _valueResolver = valueResolver;
            _operand = operand;
        }

        public async override Task<RunResult> Run(WorkflowState workflowState) {
            var value = _valueResolver(workflowState);

            return value != _operand ? RunResult.Failed : RunResult.Passed;
        }

        private readonly Func<WorkflowState, string> _valueResolver;
        private readonly string _operand;
    }
}