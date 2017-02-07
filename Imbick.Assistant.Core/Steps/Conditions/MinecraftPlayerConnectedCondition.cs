
namespace Imbick.Assistant.Core.Steps.Conditions {
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Steps;
    using Steps.Samplers;

    public class MinecraftPlayerConnectedConditionStep
        : ConditionStep {

        public MinecraftPlayerConnectedConditionStep(IState state)
        :base("Minecraft player connected condition") {
            _state = state;
        }

        public async override Task<RunResult> Run(IDictionary<string, WorkflowParameter> workflowParameters) {
            WorkflowParameter param = null;
            if (!AnyPlayersConnected(workflowParameters, out param))
                return new RunResult(false);

            var currentlyConnectedPlayers = ((MinecraftPlayer[])param.Value).ToList();

            UntrackOldPlayers(currentlyConnectedPlayers);

            var newlyConnectedPlayers = currentlyConnectedPlayers.Where(currentlyConnectedPlayer => !WasPreviouslyConnected(currentlyConnectedPlayer)).ToList();
            if (!newlyConnectedPlayers.Any())
                return new RunResult(false);

            var firstNewPlayer = newlyConnectedPlayers.First();
            workflowParameters.Add("MinecraftPlayerConnected", new WorkflowParameter<string>("MinecraftPlayerConnected", firstNewPlayer.Name));
            TrackNewPlayer(firstNewPlayer);
            return new RunResult();
        }

        private bool AnyPlayersConnected(IDictionary<string, WorkflowParameter> workflowParameters, out WorkflowParameter param) {
            param = null;
            if (!workflowParameters.ContainsKey("MinecraftServerPlayers"))
                return false;
            
            param = workflowParameters["MinecraftServerPlayers"];
            return param.Type == typeof (MinecraftPlayer[]);
        }

        private void TrackNewPlayer(MinecraftPlayer firstNewPlayer) {
            var serialisedNewPlayer = JsonConvert.SerializeObject(firstNewPlayer);
            _state.ListAdd("MinecraftServerPlayers", serialisedNewPlayer);
        }

        private void UntrackOldPlayers(ICollection<MinecraftPlayer> currentlyConnectedPlayers) {
            var listLength = _state.ListLength("MinecraftServerPlayers").Result;
            for (var i = 0; i < listLength; i++) {
                var previouslyConnectedPlayer = _state.ListGetByIndex<MinecraftPlayer>("MinecraftServerPlayers", i).Result;
                if (currentlyConnectedPlayers.All(p => p.Id != previouslyConnectedPlayer.Id))
                    _state.ListRemove("MinecraftServerPlayers", 1, previouslyConnectedPlayer);
            }
        }

        private bool WasPreviouslyConnected(MinecraftPlayer currentlyConnectedPlayer) {
            return _state.ListContains("MinecraftServerPlayers", currentlyConnectedPlayer, (p, x) => p.Id == x.Id).Result;
        }

        private readonly IState _state;
    }
}