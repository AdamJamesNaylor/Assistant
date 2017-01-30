namespace Imbick.Assistant.Service {
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Steps.Actions;
    using Core.Steps.Conditions;
    using Core.Steps.Samplers;

    public class WorkflowConfigurator {
        public IEnumerable<Workflow> GetWorkflows() {
            var adamsPhone = Guid.Parse("ef57afc0-9fea-45fc-93c9-a0ec8ada8546");

            var workflow1 = new Workflow("Check if Imbick is connected to mc.selea.se.");

            const int fiveSecondsInMilliseconds = 5000;
            var fiveSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(fiveSecondsInMilliseconds));
            workflow1.AddStep(fiveSecondInterval);

            var minecraftHost = "mc.selea.se";
            var mcSampler = new MinecraftServerListPingSampler(minecraftHost);
            workflow1.AddStep(mcSampler);

            var redisStateProvider = new RedisStateProvider("localhost");
            var playerConnected = new MinecraftPlayerConnectedConditionStep(redisStateProvider);
            workflow1.AddStep(playerConnected);

            //var notMe = new StringDoesNotEqualCondition("MinecraftPlayerConnected", "Imbick");
            //workflow.AddStep(notMe);

            //var printSuccess = new WriteStringToConsoleAction("Found Imbick!");
            var raiseNotification = new RaiseNotificationAction(redisStateProvider, new[] { adamsPhone }, "Player {MinecraftPlayerConnected} has just joined " + minecraftHost);
            workflow1.AddStep(raiseNotification);

            var workflow2 = new Workflow("Check if travel has been authorised.");
            workflow2.AddStep(fiveSecondInterval);
            var exchangeSampler = new ExchangeEmailSampler("","","");
            workflow2.AddStep(exchangeSampler);

            return new[] { workflow2};
        }
    }
}