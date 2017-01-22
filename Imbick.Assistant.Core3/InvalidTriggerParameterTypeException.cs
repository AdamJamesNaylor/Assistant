namespace Imbick.Assistant.Core
{
    using System;

    public class InvalidTriggerParameterTypeException
        : Exception {

        public InvalidTriggerParameterTypeException(TriggerParameter triggerParam, Type expectedType)
            : base($"Expected parameter of type {expectedType.FullName} but was {triggerParam.Type.FullName} for parameter named {triggerParam.Name}.") { }
    }
}