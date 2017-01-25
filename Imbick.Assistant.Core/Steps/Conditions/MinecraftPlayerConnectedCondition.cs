
namespace Imbick.Assistant.Core.Steps.Conditions {
    using System.Collections.Generic;
    using System.Linq;
    using Steps;
    using Steps.Samplers;

    public class MinecraftPlayerConnectedCondition
        : Condition {

        public MinecraftPlayerConnectedCondition(IState state)
        :base("Minecraft player connected condition") {
            _state = state;
        }

        public override StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            if (!workflowParameters.ContainsKey("MinecraftServerPlayers"))
                return new StepRunResult(false);

            var param = workflowParameters["MinecraftServerPlayers"];
            if (param.Type != typeof (MinecraftPlayer[]))
                return new StepRunResult(false);

            var currentlyConnectedPlayers = ((MinecraftPlayer[])param.Value).ToList();
            foreach (var currentlyConnectedPlayer in currentlyConnectedPlayers.Where(currentlyConnectedPlayer => !WasPreviouslyConnected(currentlyConnectedPlayer))) {
                workflowParameters.Add("MinecraftPlayerConnected", new WorkflowParameter<string>("MinecraftPlayerConnected", currentlyConnectedPlayer.Name));
                return new StepRunResult();
            }

            //stop tracking any disconnected players
            var listLength = _state.ListLength("MinecraftServerPlayers").Result;
            for (var i = 0; i < listLength; i++) {
                var previouslyConnectedPlayer = _state.ListGetByIndex<MinecraftPlayer>("MinecraftServerPlayers", i).Result;
                if (!currentlyConnectedPlayers.Contains(previouslyConnectedPlayer))
                    _state.ListRemove("MinecraftServerPlayers", 1, previouslyConnectedPlayer);
            }

            return new StepRunResult(false);
        }

        private bool WasPreviouslyConnected(MinecraftPlayer currentlyConnectedPlayer) {
            return _state.ListContains("MinecraftServerPlayers", currentlyConnectedPlayer, (p, x) => p.Id == x.Id).Result;
        }

        private readonly IState _state;
    }
}