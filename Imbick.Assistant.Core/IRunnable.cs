namespace Imbick.Assistant.Core
{
    using Steps;
    using System.Threading.Tasks;

    public interface IRunnable {
        Task<RunResult> Run(WorkflowState workflowState);
    }
}