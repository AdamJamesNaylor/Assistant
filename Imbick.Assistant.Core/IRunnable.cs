namespace Imbick.Assistant.Core
{
    using System.Collections.Generic;
    using Steps;

    public interface IRunnable {
        StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters);
    }
}