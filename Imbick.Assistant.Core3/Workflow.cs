namespace Imbick.Assistant.Core
{
    using System.Collections.Generic;
    using System.Linq;

    public interface IActionable {
        IRunnable Action { get; }
    }

    public interface ITriggerable {
        IEnumerable<IFireable> Triggers { get; }

        bool HaveAllTriggersFired();
    }

    public class Workflow
        : ITriggerable, IActionable {
        public IEnumerable<IFireable> Triggers => _triggers;

        public IRunnable Action { get; }

        public Workflow() {
            _triggers = new List<IFireable>();
        }
        public bool HaveAllTriggersFired() {
            return Triggers.All(trigger => trigger.HasFired(_triggerParameters));
        }

        public void AddTrigger(IFireable trigger) {
            _triggers.Add(trigger);
        }

        private readonly Dictionary<string, TriggerParameter> _triggerParameters = new Dictionary<string, TriggerParameter>();
        private readonly List<IFireable> _triggers;
    }
}