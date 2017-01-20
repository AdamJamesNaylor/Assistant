namespace Imbick.Assistant.WebApi {
    using System.Collections.Generic;

    public interface IFireable {
        bool HasFired(IDictionary<string, TriggerParameter> triggerParameters);
    }
}