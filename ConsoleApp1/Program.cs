
namespace ConsoleApp1 {
    using System;
    using Imbick.Assistant.Core;
    using Imbick.Assistant.Core.Steps.Actions;
    using Imbick.Assistant.Core.Steps.Conditions;
    using Imbick.Assistant.Core.Steps.Samplers;

    internal class Program {
        private static void Main(string[] args) {
            var workflow = new Workflow("Check if Carl is connected to minecraft.");

            //var emailTrigger = new Pop3EmailReceivedTrigger("adamnaylor@gmail.com", "", "pop.gmail.com");
            //var subjectTrigger = new StringEqualsTrigger("email_subject", "test email");
            //var emailNotify = new Workflow();
            //emailNotify.AddTrigger(emailTrigger);
            //emailNotify.SetAction(new WriteLineToConsoleAction("email_subject"));
            ////emailNotify.AddTrigger(subjectTrigger);
            //var runner = new WorkflowRunner();
            //runner.Register(emailNotify);
            //runner.RunAllWorkflows();

            var fiveSecondsInMilliseconds = 5000;
            var fiveSecondsInTicks = fiveSecondsInMilliseconds * TimeSpan.TicksPerMillisecond;
            var fiveSecondInterval = new IntervalConditionStep(new TimeSpan(fiveSecondsInTicks));
            workflow.AddStep(fiveSecondInterval);

            var mcSampler = new MinecraftServerListPingSampler("mc.selea.se");
            workflow.AddStep(mcSampler);

            var playerConnected = new MinecraftPlayerConnectedConditionStep(new RedisStateProvider("localhost"));
            workflow.AddStep(playerConnected);

            var isMe = new StringEqualsConditionStep("MinecraftPlayerConnected", "Imbick");
            workflow.AddStep(isMe);

            var printSuccess = new WriteStringToConsoleAction("Found Imbick!");
            workflow.AddStep(printSuccess);

            var runner = new WorkflowRunner(100);
            runner.Register(workflow);
            runner.RunAllWorkflows();
        }
    }
}