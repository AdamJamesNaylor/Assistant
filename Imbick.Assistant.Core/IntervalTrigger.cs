namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;

    public class IntervalCondition
        : Condition, IRunnable {

        public IntervalCondition(TimeSpan interval) {
            _interval = interval;
            _lastFired = DateTime.Now;
        }

        public StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameter) {
            var now = DateTime.Now;
            if (_lastFired + _interval < now)
                return new StepRunResult(false);

            _lastFired = now;
            return new StepRunResult();
        }

        private readonly TimeSpan _interval;
        private DateTime _lastFired;
    }
}