namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using System.Threading.Tasks;
    using Steps;

    public class WriteStringToConsoleAction
        : ActionStep {
        private readonly string _value;

        public WriteStringToConsoleAction(string value)
            : base("Write string to console action") {
            _value = value;
        }

        public async override Task<RunResult> Run(WorkflowState workflowState) {
            Console.WriteLine(_value);
            return new RunResult();
        }
    }
}