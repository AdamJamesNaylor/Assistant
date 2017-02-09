namespace Imbick.Assistant.Core.ValueResolvers {
    using System;
    using System.Collections.Generic;
    using Steps.Conditions;

    public class WorkflowParameterValueResolver<T> {
        private readonly string _parameterName;
        private readonly IDictionary<string, WorkflowParameter> _workflowParameters;

        public WorkflowParameterValueResolver(string parameterName, IDictionary<string, WorkflowParameter> workflowParameters) {
            _parameterName = parameterName;
            _workflowParameters = workflowParameters;
        }

        public T Resolve() {
            if (!_workflowParameters.ContainsKey(_parameterName))
                throw new ValueResolutionException(
                    $"Expected a parameter called {_parameterName} in parameter collection.");

            var parameter = _workflowParameters["MinecraftChatMessages"];
            if (parameter.Type != typeof(T))
                throw new ValueResolutionException(
                    $"Expected {_parameterName} parameter to be of type {typeof(T).FullName}");

            return (T)parameter.Value;
        }
    }

    public class ValueResolutionException : Exception {
        public ValueResolutionException(string s) {
            throw new NotImplementedException();
        }
    }

    public interface IValueResolver<T> {
        T Resolve();
    }

}