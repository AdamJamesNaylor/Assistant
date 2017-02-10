namespace Imbick.Assistant.Core.Steps.Actions {
    using System;
    using Newtonsoft.Json;
    using Steps;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using NLog;
    using Samplers;

    public class MinecraftServerChatSender
        : Step {

        public MinecraftServerChatSender(string host)
            : base("Minecraft server chat sender") {
            _host = host;
            _client = new HttpClient {
                BaseAddress = new Uri(_host)
            };
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            _logger.Trace($"Sending message {workflowState.Payload} to Minecraft server {_host}");
            var message = new MinecraftChatMessage {
                Name = "Thaddeus",
                Message = workflowState.Payload.ToString()
            };
            var serialisedMessage = JsonConvert.SerializeObject(message);
            var content = new StringContent(serialisedMessage, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/up/sendmessage", content);
            if (!response.IsSuccessStatusCode) {
                _logger.Error($"Failed to send chat message to {response.RequestMessage.RequestUri.AbsoluteUri} ({response.StatusCode})");
                return RunResult.Failed;
            }

            _logger.Trace("Sent");
            return RunResult.Passed;
        }

        private readonly HttpClient _client;
        private readonly string _host;
        private readonly Logger _logger;
    }
}