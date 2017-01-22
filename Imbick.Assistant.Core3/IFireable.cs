namespace Imbick.Assistant.Core {
    using System.Collections.Generic;

    public interface IFireable {
        bool HasFired(IDictionary<string, TriggerParameter> triggerParameters);
    }
}