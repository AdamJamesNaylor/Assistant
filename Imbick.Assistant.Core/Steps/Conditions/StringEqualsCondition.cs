namespace Imbick.Assistant.Core.Steps.Conditions {
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;
    using Samplers;
    using Steps;

    public class MinecraftChatPlayerNameResolver {

        public MinecraftChatPlayerNameResolver(IDictionary<string, WorkflowParameter> workflowParameters) {
            _workflowParameters = workflowParameters;
        }

        public string Resolve() {
            if (!_workflowParameters.ContainsKey("MinecraftChatMessages"))
                throw new ValueResolutionException(
                    "Expected a parameter called MinecraftChatMessages in parameter collection.");

            var parameter = _workflowParameters["MinecraftChatMessages"];
            if (parameter.Type != typeof (IEnumerable<MinecraftChatMessage>))
                throw new ValueResolutionException(
                    $"Expected MinecraftChatMessages parameter to be of type {typeof (IEnumerable<MinecraftChatMessage>).FullName}");

            var chatMessages = (IEnumerable<MinecraftChatMessage>) parameter.Value;
            return chatMessages
        }

        private readonly IDictionary<string, WorkflowParameter> _workflowParameters;
    }

    [Serializable]
    public class ValueResolutionException
        : Exception {

        public ValueResolutionException() {
        }

        public ValueResolutionException(string message) : base(message) {
        }

        public ValueResolutionException(string message, Exception inner) : base(message, inner) {
        }

        protected ValueResolutionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {
        }
    }

    public abstract class ValueResolver {
        public abstract string Resolve();
    }

    public class StringEqualsConditionStep
        : ConditionStep {

        public StringEqualsConditionStep(string paramName, string operand)
            : base("String equals condition") {
            _paramName = paramName;
            _operand = operand;
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            var triggerParam = workflowParameters[_paramName];
            if (triggerParam.Type != typeof (string))
                throw new InvalidWorkflowParameterTypeException(triggerParam, typeof (string));
            var result = (string) workflowParameters[_paramName].Value == _operand;
            return new RunResult(result);
        }

        private readonly string _paramName;
        private readonly string _operand;
    }
}