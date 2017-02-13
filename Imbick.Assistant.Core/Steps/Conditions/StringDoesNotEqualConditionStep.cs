namespace Imbick.Assistant.Core.Steps.Conditions {
    using System;
    using System.Threading.Tasks;

    public class StringDoesNotEqualConditionStep
        : ConditionStep {

        public StringDoesNotEqualConditionStep(Func<WorkflowState, string> valueResolver, string operand)
            : base("String does not equal condition") {
            _valueResolver = valueResolver;
            _operand = operand;
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            var value = _valueResolver(workflowState);

            return value != _operand ? RunResult.Passed : RunResult.Failed;
        }

        private readonly Func<WorkflowState, string> _valueResolver;
        private readonly string _operand;
    }

    public class StringStartsWithConditionStep
        : ConditionStep {

        public StringStartsWithConditionStep(Func<WorkflowState, string> valueResolver, string operand)
            : base("String starts with condition") {
            _valueResolver = valueResolver;
            _operand = operand;
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            var value = _valueResolver(workflowState);

            return value.StartsWith(_operand) ? RunResult.Passed : RunResult.Failed;
        }

        private readonly Func<WorkflowState, string> _valueResolver;
        private readonly string _operand;
    }
}