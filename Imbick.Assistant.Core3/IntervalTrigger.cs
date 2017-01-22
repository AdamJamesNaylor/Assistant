namespace Imbick.Assistant.Core
{
    using System;
    using System.Collections.Generic;

    public class IntervalTrigger
        : IFireable {

        public IntervalTrigger(TimeSpan interval) {
            _interval = interval;
            _lastFired = DateTime.Now;
        }

        public bool HasFired(IDictionary<string, TriggerParameter> triggerParameters) {
            var now = DateTime.Now;
            if (_lastFired + _interval < now)
                return false;

            _lastFired = now;
            return true;
        }

        private readonly TimeSpan _interval;
        private DateTime _lastFired;
    }
}