namespace Imbick.Assistant.Core.Steps {
    public class StepRunResult {
        public bool Continue { get; set; }

        public StepRunResult(bool @continue = true) {
            Continue = @continue;
        }
    }
}