namespace Imbick.Assistant.Core {
    public abstract class Step {
        public string Name { get; protected set; }

        protected Step(string name) {
            Name = name;
        }
    }
}