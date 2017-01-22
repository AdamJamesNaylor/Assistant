namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;

    public class WriteStringToConsoleStep
        : IRunnable {
        private readonly string _value;

        public WriteStringToConsoleStep(string value) {
            _value = value;
        }

        public StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameter) {
            Console.WriteLine(_value);
            return new StepRunResult();
        }
    }
}