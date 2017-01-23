namespace Imbick.Assistant.Core
{
    using System;

    public class WorkflowParameter<T>
        : WorkflowParameter {
        public override Type Type => typeof (T);

        public WorkflowParameter(string name, T value) {
            Value = value;
            Name = name;
        }
    }

    public abstract class WorkflowParameter {
        public abstract Type Type { get; }
        public string Name { get; set; }
        public object Value { get; set;  }
    }
}