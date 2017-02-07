namespace Imbick.Assistant.Service {
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Steps.Actions;
    using Core.Steps.Branching;
    using Core.Steps.Conditions;
    using Core.Steps.Samplers;

    public class WorkflowConfigurator {
        private IntervalConditionStep _fiveSecondInterval;
        private string _minecraftHost = "mc.selea.se";
        private RedisStateProvider _redisStateProvider = new RedisStateProvider("localhost");

        public IEnumerable<Workflow> GetWorkflows() {
            const int fiveSecondsInMilliseconds = 5000;
            _fiveSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(fiveSecondsInMilliseconds));

            var workflow1 = BuildPlayerConnectedWorkflow();

            var workflow2 = BuildTravelEmailWorkflow();

            var workflow3 = BuildMinecraftChatWorkflow();

            return new[] { workflow3 };
        }

        private Workflow BuildMinecraftChatWorkflow() {
            const int oneSecondsInMilliseconds = 1000;
            var oneSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(oneSecondsInMilliseconds));

            var workflow = new Workflow("Respond to chat messages.");
            workflow.AddStep(oneSecondInterval);

            var chatSampler = new MinecraftServerChatSampler(_minecraftHost, _redisStateProvider);
            workflow.AddStep(chatSampler);

            var switchStep = new SwitchBranchStep();
            var case1 = new SwitchCase {
                Name = "Hello"
            };
            case1.Conditions.Add(new StringEqualsConditionStep("", "hello"));
            switchStep.Cases.Add(case1);

            return workflow;
        }

        private Workflow BuildTravelEmailWorkflow() {
            var workflow2 = new Workflow("Check if travel has been authorised.");
            workflow2.AddStep(_fiveSecondInterval);
            var exchangeSampler = new ExchangeEmailSampler("", "", "");
            workflow2.AddStep(exchangeSampler);
            return workflow2;
        }

        private Workflow BuildPlayerConnectedWorkflow() {
            var adamsPhone = Guid.Parse("ef57afc0-9fea-45fc-93c9-a0ec8ada8546");

            var workflow1 = new Workflow("Check if Imbick is connected to mc.selea.se.");

            workflow1.AddStep(_fiveSecondInterval);

            var mcSampler = new MinecraftServerListPingSampler(_minecraftHost);
            workflow1.AddStep(mcSampler);

            var playerConnected = new MinecraftPlayerConnectedConditionStep(_redisStateProvider);
            workflow1.AddStep(playerConnected);

            //var notMe = new StringDoesNotEqualCondition("MinecraftPlayerConnected", "Imbick");
            //workflow.AddStep(notMe);

            //var printSuccess = new WriteStringToConsoleAction("Found Imbick!");
            var raiseNotification = new RaiseNotificationAction(_redisStateProvider, new[] {adamsPhone},
                "Player {MinecraftPlayerConnected} has just joined " + _minecraftHost);
            workflow1.AddStep(raiseNotification);
            return workflow1;
        }
    }
}