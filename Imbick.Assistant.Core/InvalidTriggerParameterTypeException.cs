namespace Imbick.Assistant.Core {
    using System;

    public class InvalidWorkflowParameterTypeException
        : Exception {

        public InvalidWorkflowParameterTypeException(WorkflowParameter workflowParameter, Type expectedType)
            : base($"Expected parameter of type {expectedType.FullName} but was {workflowParameter.Type.FullName} for parameter named {workflowParameter.Name}.") {
        }
    }
}