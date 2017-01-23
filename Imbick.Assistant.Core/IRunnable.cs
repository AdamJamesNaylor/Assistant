namespace Imbick.Assistant.Core
{
    using System.Collections.Generic;

    public interface IRunnable {
        StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters);
    }
}