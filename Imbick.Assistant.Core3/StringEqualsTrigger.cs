namespace Imbick.Assistant.Core
{
    using System.Collections.Generic;

    public class StringEqualsTrigger
        : Trigger, IFireable {
        
        public StringEqualsTrigger(string paramName, string operand) {
            _paramName = paramName;
            _operand = operand;
        }

        public bool HasFired(IDictionary<string, TriggerParameter> triggerParameters) {
            var triggerParam = triggerParameters[_paramName];
            if (triggerParam.Type != typeof (string))
                throw new InvalidTriggerParameterTypeException(triggerParam, typeof(string));
            return (string)triggerParameters[_paramName].Value == _operand;
        }

        private readonly string _paramName;
        private readonly string _operand;
    }
}