﻿namespace Imbick.Assistant.Core.Steps.Conditions {
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
            _logger.Trace($"IntervalCondition running with {_interval.Milliseconds}ms interval. Last fired {_lastFired}.");

            var now = DateTime.Now;
            if (_lastFired + _interval > now) {
                return new RunResult(false);
            }

            _logger.Debug($"IntervalCondition met after {_interval.Milliseconds}ms interval.");
            _lastFired = now;
            return new RunResult();
        }

        private readonly TimeSpan _interval;
        private DateTime _lastFired;
        private readonly Logger _logger;
    }
}