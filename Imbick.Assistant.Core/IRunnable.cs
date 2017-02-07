namespace Imbick.Assistant.Core
{
    using System.Collections.Generic;
    using Steps;
    using System.Threading.Tasks;

    public interface IRunnable {
        Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters);
    }
}