namespace Imbick.Assistant.Core
{
    using System.Collections.Generic;

    public class StringEqualsCondition
        : Condition, IRunnable {
        
        public StringEqualsCondition(string paramName, string operand) {
            _paramName = paramName;
            _operand = operand;
        }

        public StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameter) {
            var triggerParam = workflowParameter[_paramName];
            if (triggerParam.Type != typeof (string))
                throw new InvalidWorkflowParameterTypeException(triggerParam, typeof(string));
            var result = (string)workflowParameter[_paramName].Value == _operand;
            return new StepRunResult(result);
        }

        private readonly string _paramName;
        private readonly string _operand;
    }
}