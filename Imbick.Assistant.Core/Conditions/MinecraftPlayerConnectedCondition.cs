﻿
namespace Imbick.Assistant.Core.Conditions {
    using System.Collections.Generic;
    using System.Linq;
    using Samplers;

    public class MinecraftPlayerConnectedCondition
        : Condition, IRunnable {

        public MinecraftPlayerConnectedCondition(IState state) {
            _state = state;
        }

        public StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters) {
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