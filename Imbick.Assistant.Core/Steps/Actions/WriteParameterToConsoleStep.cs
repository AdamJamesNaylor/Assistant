namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using System.Threading.Tasks;
    using Steps;

    public class WriteParameterToConsoleStep
        : IRunnable {
        private readonly Func<WorkflowState, string> _valueResolver;

        public bool IncludeNewline { get; set; }

        public WriteParameterToConsoleStep(Func<WorkflowState, string> valueResolver, bool includeNewline = true) {
            _valueResolver = valueResolver;
            IncludeNewline = includeNewline;
        }

        public async Task<RunResult> Run(WorkflowState workflowState) {
            if (IncludeNewline)
                Console.WriteLine(_valueResolver(workflowState));
            else
                Console.Write(_valueResolver(workflowState));
            return new RunResult();
        }
    }
}