namespace Imbick.Assistant.Service {
    using System;
    using System.Collections.Generic;
    using Core;
    using Core.Steps.Actions;
    using Core.Steps.Branching;
    using Core.Steps.Conditions;
    using Core.Steps.Loops;
    using Core.Steps.Samplers;

    public class WorkflowConfigurator {
        private IntervalConditionStep _fiveSecondInterval;
        private string _minecraftHost = "https://mc.selea.se";
        private readonly RedisStateProvider _redisStateProvider = new RedisStateProvider("localhost");

        public IEnumerable<Workflow> GetWorkflows() {
            const int fiveSecondsInMilliseconds = 5000;
            _fiveSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(fiveSecondsInMilliseconds));

            //var workflow1 = BuildPlayerConnectedWorkflow();

            //var workflow2 = BuildTravelEmailWorkflow();

            var workflow3 = BuildMinecraftChatWorkflow();

            return new[] { workflow3 };
        }

        private Workflow BuildMinecraftChatWorkflow() {
            var workflow = new Workflow("Respond to chat messages");

            const int oneSecondsInMilliseconds = 1000;
            var oneSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(oneSecondsInMilliseconds));
            workflow.AddStep(oneSecondInterval);

            var chatSampler = new MinecraftServerChatSampler(_minecraftHost);
            workflow.AddStep(chatSampler);
            //todo may need to check state at this point in case messages are responded to more than once - also check messages aren't lost
            var loopStep = new ForeachLoopStep<MinecraftChatMessage>();
            {
                //loopStep.Steps.Add(new StringDoesNotEqualConditionStep(w => ((MinecraftChatMessage) w.Payload).Name, "Imbick"));
                loopStep.Steps.Add(new StringStartsWithConditionStep(w => ((MinecraftChatMessage)w.Payload).Message.ToLower(), "thaddeus,"));
                var fuzzyTextMatchAction = new FuzzyTextMatchAction(w => ((MinecraftChatMessage)w.Payload).Message.Substring(9).Trim());
                {
                    fuzzyTextMatchAction.Matches.Add(new FuzzyTextMatch
                    {
                        Terms = { "ping" },
                        Steps = { new SetPayloadStep("pong") }
                    });

                    fuzzyTextMatchAction.Matches.Add(new FuzzyTextMatch
                    {
                        Terms = { "who are you?", "tell me about yourself" },
                        Steps = { new SetPayloadStep("My name is Thaddeus, and I am here to help.") }
                    });

                    fuzzyTextMatchAction.Matches.Add(new FuzzyTextMatch
                    {
                        Terms = { "when where you born?" },
                        Steps = { new SetPayloadStep("I said my first words at midnight on Valentines day 2017.") }
                    });

                    fuzzyTextMatchAction.Matches.Add(new FuzzyTextMatch
                    {
                        Terms = { "cya" },
                        Steps = { new SetPayloadStep("See you soon.") }
                    });

                    fuzzyTextMatchAction.FailureSteps.Add(new SetPayloadStep("Sorry, I'm not sure what you mean?"));
                }
                loopStep.Steps.Add(fuzzyTextMatchAction);
                loopStep.Steps.Add(new MinecraftServerChatSender(_minecraftHost));
            }
            workflow.AddStep(loopStep);

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