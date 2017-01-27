namespace Imbick.Assistant.Core.Steps.Actions
{
    using System;
    using System.Collections.Generic;

    class RaiseNotificationAction
        : ActionStep {
        private readonly IState _state;
        private readonly IEnumerable<Guid> _deviceIds;

        public RaiseNotificationAction(IState state, IEnumerable<Guid> deviceIds)
            : base("Raise notification action") {
            _state = state;
            _deviceIds = deviceIds;
        }

        public override StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            foreach (var deviceId in _deviceIds) {
                _state.S
            }
        }
    }
}
