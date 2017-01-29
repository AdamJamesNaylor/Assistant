namespace Imbick.Assistant.Core {
    using System;

    public abstract class WorkflowParameter {

        public static implicit operator string(WorkflowParameter x) {
            return x.ToString();
        }

        public override string ToString() {
            if (Type == typeof(string))
                return (string)Value;
            //return Convert.ChangeType(x.Value, x.Type).ToString();
            //return x.Value.ToString();
            throw new InvalidOperationException("Did not expect type of " + Type.FullName);
        }


        public abstract Type Type { get; }
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class WorkflowParameter<T>
        : WorkflowParameter {
        public override Type Type => typeof (T);

        public WorkflowParameter(string name, T value) {
            Value = value;
            Name = name;
        }
    }
}