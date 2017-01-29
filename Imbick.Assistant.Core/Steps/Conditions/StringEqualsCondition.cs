namespace Imbick.Assistant.Core.Steps.Conditions {
    using System.Collections.Generic;
    using Steps;

    public class StringEqualsConditionStep
        : ConditionStep {

        public StringEqualsConditionStep(string paramName, string operand)
            : base("String equals condition") {
            _paramName = paramName;
            _operand = operand;
        }

        public override StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            var triggerParam = workflowParameters[_paramName];
            if (triggerParam.Type != typeof (string))
                throw new InvalidWorkflowParameterTypeException(triggerParam, typeof (string));
            var result = (string) workflowParameters[_paramName].Value == _operand;
            return new StepRunResult(result);
        }

        private readonly string _paramName;
        private readonly string _operand;
    }
}