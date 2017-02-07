namespace Imbick.Assistant.Core.Steps.Conditions {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class StringDoesNotEqualCondition
        : ConditionStep {
        private readonly string _paramName;
        private readonly string _operand;

        public StringDoesNotEqualCondition(string paramName, string operand) 
            : base("String does not equal condition") {
            _paramName = paramName;
            _operand = operand;
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            var triggerParam = workflowParameters[_paramName];
            if (triggerParam.Type != typeof(string))
                throw new InvalidWorkflowParameterTypeException(triggerParam, typeof(string));
            var result = (string)workflowParameters[_paramName].Value != _operand;
            return new RunResult(result);
        }
    }
}