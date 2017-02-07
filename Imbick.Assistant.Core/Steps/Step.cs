namespace Imbick.Assistant.Core.Steps {
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class Step
        : IRunnable {
        public string Name { get; protected set; }

        protected Step(string name) {
            Name = name;
        }

        public abstract Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters);
    }
}