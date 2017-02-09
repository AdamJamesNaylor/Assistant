namespace Imbick.Assistant.Core.Steps {
    public class RunResult {

        public static RunResult Passed = new RunResult();
        public static RunResult Failed = new RunResult(false);

        public bool Continue { get; set; }

        public RunResult(bool @continue = true) {
            Continue = @continue;
        }
    }

    public class WorkflowState {
        public object Payload { get; set; }
    }
}