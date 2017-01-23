
namespace ConsoleApp1 {
    using Imbick.Assistant.Core;
    using Imbick.Assistant.Core.Samplers;

    internal class Program {
        private static void Main(string[] args) {
            var workflow = new Workflow();

            //var emailTrigger = new Pop3EmailReceivedTrigger("adamnaylor@gmail.com", "", "pop.gmail.com");
            //var subjectTrigger = new StringEqualsTrigger("email_subject", "test email");
            //var emailNotify = new Workflow();
            //emailNotify.AddTrigger(emailTrigger);
            //emailNotify.SetAction(new WriteLineToConsoleAction("email_subject"));
            ////emailNotify.AddTrigger(subjectTrigger);
            //var runner = new WorkflowRunner();
            //runner.Register(emailNotify);
            //runner.RunAllWorkflows();

            var mcSampler = new MinecraftServerListPingSampler("mc.selea.se");
            workflow.AddStep(mcSampler);

            var isMe = new StringEqualsCondition("MinecraftPlayerConnected", "Imbick");
            workflow.AddStep(isMe);

            var printSuccess = new WriteStringToConsoleStep("Found Imbick!");
            workflow.AddStep(printSuccess);

            var runner = new WorkflowRunner();
            runner.Register(workflow);
            runner.RunAllWorkflows();
        }
    }
}