namespace Imbick.Assistant.Core.Steps.Actions {
    using Steps;

    public abstract class ActionStep
        : Step {
        protected ActionStep(string name)
            : base(name) {
        }
    }
}