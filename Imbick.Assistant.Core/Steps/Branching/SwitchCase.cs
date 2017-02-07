namespace Imbick.Assistant.Core.Steps.Branching {
    using Conditions;

    public class SwitchCase {
        public string Name { get; set; }
        public StepCollection<ConditionStep> Conditions { get; set; }
        public StepCollection Steps { get; set; }

        public SwitchCase() {
            Conditions = new StepCollection<ConditionStep>();
            Steps = new StepCollection();
        }
    }
}