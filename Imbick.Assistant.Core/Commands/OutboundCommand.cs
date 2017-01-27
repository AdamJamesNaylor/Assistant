namespace Imbick.Assistant.Core.Commands {
    using System;
    using System.Collections.Generic;

    public class OutboundCommand {
        public OutboundCommand() {
            Arguments = new List<CommandArgument>();
            Processed = false;
        }

        public string Type { get; set; }
        public Guid TargetId { get; set; }

        public IList<CommandArgument> Arguments { get; private set; }

        public bool Processed { get; set; }

    }
}