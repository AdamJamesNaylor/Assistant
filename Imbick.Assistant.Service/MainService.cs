
namespace Imbick.Assistant.Service {
    using Core;
    using System;
    using System.ServiceProcess;
    using Core.Commands;
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
            var adamsPhone = Guid.Parse("ef57afc0-9fea-45fc-93c9-a0ec8ada8546");

            var workflow = new Workflow("Check if Imbick is connected to mc.selea.se.");

            const int fiveSecondsInMilliseconds = 5000;
            var fiveSecondInterval = new IntervalConditionStep(TimeSpan.FromMilliseconds(fiveSecondsInMilliseconds));
            workflow.AddStep(fiveSecondInterval);

            var minecraftHost = "mc.selea.se";
            var mcSampler = new MinecraftServerListPingSampler(minecraftHost);
            workflow.AddStep(mcSampler);

            var redisStateProvider = new RedisStateProvider("localhost");
            var playerConnected = new MinecraftPlayerConnectedConditionStep(redisStateProvider);
            workflow.AddStep(playerConnected);

            //var notMe = new StringDoesNotEqualCondition("MinecraftPlayerConnected", "Imbick");
            //workflow.AddStep(notMe);

            //var printSuccess = new WriteStringToConsoleAction("Found Imbick!");
            var raiseNotification = new RaiseNotificationAction(redisStateProvider, new [] { adamsPhone }, "Player {MinecraftPlayerConnected} has just joined " + minecraftHost);
            workflow.AddStep(raiseNotification);

            var runner = new WorkflowRunner(100);
            runner.Register(workflow);
            runner.RunAllWorkflows();

        }

        protected override void OnStop() {
        }
    }
}