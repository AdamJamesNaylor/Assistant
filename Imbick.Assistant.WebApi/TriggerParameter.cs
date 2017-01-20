namespace Imbick.Assistant.WebApi {
    using System;

    public class TriggerParameter<T>
        : TriggerParameter {
        public override Type Type => typeof (T);
    }

    public abstract class TriggerParameter {
        public abstract Type Type { get; }
        public string Name { get; set; }
        public object Value { get; set;  }
    }
}