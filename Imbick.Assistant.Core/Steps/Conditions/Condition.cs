namespace Imbick.Assistant.Core.Steps.Conditions {
    using Steps;

    public abstract class Condition
        : Step {
        protected Condition(string name)
            : base(name) {
            
        }
    }
}