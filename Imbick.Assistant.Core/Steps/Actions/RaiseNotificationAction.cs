namespace Imbick.Assistant.Core.Steps.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Commands;
    using Newtonsoft.Json;
    using SmartFormat;

    public class RaiseNotificationAction
        : ActionStep {
        private readonly IState _state;
        private readonly IEnumerable<Guid> _deviceIds;
        private readonly string _messageFormat;

        public RaiseNotificationAction(IState state, IEnumerable<Guid> deviceIds, string messageFormat)
            : base("Raise notification action") {
            _state = state;
            _deviceIds = deviceIds;
            _messageFormat = messageFormat;
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            var message = Smart.Format(_messageFormat, workflowParameters);
            foreach (var deviceId in _deviceIds) {
                var command = new RaiseNotificationCommand(message);
                var serialisedCommand = JsonConvert.SerializeObject(command);
                _state.ListAdd(deviceId.ToString(), serialisedCommand);
            }
            return new RunResult();
        }
    }

}
