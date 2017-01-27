
namespace Imbick.Assistant.Service {
    using Core;
    using System;
    using System.ServiceProcess;
    using Core.Steps.Actions;
    using Core.Steps.Conditions;
    using Core.Steps.Samplers;

    public partial class MainService
        : ServiceBase {

        public MainService() {
            InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            Start();
        }

        private void Start() {
            var workflow = new Workflow("Check if Imbick is connected to mc.selea.se.");

            const int fiveSecondsInMilliseconds = 5000;
            var fiveSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(fiveSecondsInMilliseconds));
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

        protected override void OnStop() {
        }
    }
}