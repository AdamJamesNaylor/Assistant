namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Steps;

    public class WriteParameterToConsoleStep
        : IRunnable {
        private readonly string _paramName;

        public bool IncludeNewline { get; set; }

        public WriteParameterToConsoleStep(string paramName, bool includeNewline = true) {
            IncludeNewline = includeNewline;
            _paramName = paramName;
        }

        public async Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameter) {
            if (IncludeNewline)
                Console.WriteLine(workflowParameter[_paramName].Value);
            else
                Console.Write(workflowParameter[_paramName].Value);
            return new RunResult();
        }
    }
}