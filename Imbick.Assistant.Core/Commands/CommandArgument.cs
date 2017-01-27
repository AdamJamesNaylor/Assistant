namespace Imbick.Assistant.Core.Commands {
    using System;

    public abstract class CommandArgument {
        public abstract Type Type { get; }
    }

    public class CommandArgument<T>
        : CommandArgument {
        public override Type Type => typeof (T);
        public T Value { get; }

        public CommandArgument(T value) {
            Value = value;
        }
    }
}