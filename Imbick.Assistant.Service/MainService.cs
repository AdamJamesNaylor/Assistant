
namespace Imbick.Assistant.Service {
    using Core;
    using Core.Conditions;
    using Core.Samplers;
    using System;
    using System.ServiceProcess;
    
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
            var fiveSecondInterval = new IntervalCondition(TimeSpan.FromMilliseconds(fiveSecondsInMilliseconds));
            workflow.AddStep(fiveSecondInterval);

            var mcSampler = new MinecraftServerListPingSampler("mc.selea.se");
            workflow.AddStep(mcSampler);

            var playerConnected = new MinecraftPlayerConnectedCondition(new RedisStateProvider("localhost"));
            workflow.AddStep(playerConnected);

            var isMe = new StringEqualsCondition("MinecraftPlayerConnected", "Imbick");
            workflow.AddStep(isMe);

            var printSuccess = new WriteStringToConsoleStep("Found Imbick!");
            workflow.AddStep(printSuccess);

            var runner = new WorkflowRunner(100);
            runner.Register(workflow);
            runner.RunAllWorkflows();

        }

        protected override void OnStop() {
        }
    }
}