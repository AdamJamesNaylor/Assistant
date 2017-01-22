namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;

    public class WriteParameterToConsoleStep
        : IRunnable {
        private readonly string _paramName;

        public WriteParameterToConsoleStep(string paramName) {
            _paramName = paramName;
        }

        public StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameter) {
            Console.WriteLine(workflowParameter[_paramName].Value);
            return new StepRunResult();
        }
    }
}