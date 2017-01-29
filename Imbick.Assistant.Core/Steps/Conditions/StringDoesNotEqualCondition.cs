namespace Imbick.Assistant.Core.Steps.Conditions {
    using System.Collections.Generic;

    public class StringDoesNotEqualCondition
        : ConditionStep {
        private readonly string _paramName;
        private readonly string _operand;

        public StringDoesNotEqualCondition(string paramName, string operand) 
            : base("String does not equal condition") {
            _paramName = paramName;
            _operand = operand;
        }

        public override StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            var triggerParam = workflowParameters[_paramName];
            if (triggerParam.Type != typeof(string))
                throw new InvalidWorkflowParameterTypeException(triggerParam, typeof(string));
            var result = (string)workflowParameters[_paramName].Value != _operand;
            return new StepRunResult(result);
        }
    }
}