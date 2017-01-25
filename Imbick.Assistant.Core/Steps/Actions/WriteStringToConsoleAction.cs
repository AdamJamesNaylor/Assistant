namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using System.Collections.Generic;
    using Steps;

    public class WriteStringToConsoleAction
        : ActionStep {
        private readonly string _value;

        public WriteStringToConsoleAction(string value)
            : base("Write string to console action") {
            _value = value;
        }

        public override StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameter) {
            Console.WriteLine(_value);
            return new StepRunResult();
        }
    }
}