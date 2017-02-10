namespace Imbick.Assistant.Core.Steps.Conditions {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NLog;
    using Steps;

    public class IntervalConditionStep
        : ConditionStep {

        public IntervalConditionStep(TimeSpan interval)
            : base($"{interval.ToReadableString()} interval condition") {
            _interval = interval;
            _lastFired = DateTime.Now;
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async override Task<RunResult> Run(WorkflowState workflowState) {

            var now = DateTime.Now;
            if (_lastFired + _interval > now) {
                return RunResult.Failed;
            }

            _logger.Debug($"IntervalCondition met after {_interval.Milliseconds}ms interval.");
            _lastFired = now;
            return RunResult.Passed;
        }

        private readonly TimeSpan _interval;
        private DateTime _lastFired;
        private readonly Logger _logger;
    }
}