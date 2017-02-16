namespace Imbick.Assistant.Core.Steps.Samplers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Steps;
    using System.Net.Http;
    using System.Threading.Tasks;
    using NLog;

    public class MinecraftChatMessage {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class MinecraftServerChatSampler
        : Step {

        public MinecraftServerChatSampler(string host)
            : base("Minecraft server chat sampler") {
            _host = host;
            _client = new HttpClient {
                BaseAddress = new Uri(_host)
            };
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override async Task<RunResult> Run(WorkflowState workflowState) {
            var response = await GetResponse();
            if (!response.IsSuccessStatusCode) {
                _logger.Error($"Error checking for messages from {response.RequestMessage.RequestUri.AbsoluteUri} ({response.StatusCode}).");
                return RunResult.Failed;
            }

            var chatMessages = await DeserialiseChatMessages(response);

            if (!chatMessages.Any())
                return RunResult.Failed;

            workflowState.Payload =
                chatMessages.Select(update => new MinecraftChatMessage {
                    Message = update.message,
                    Name = update.name
                });

            return RunResult.Passed;
        }

        private async Task<HttpResponseMessage> GetResponse() {
            string requestUri = $"/up/world/world/{_ticks}";
            _logger.Trace($"Checking for Minecraft chat messages at {_ticks} against {_host + requestUri}.");
            return await _client.GetAsync(requestUri);
        }

        private async Task<IEnumerable<DynMapResponseUpdate>> DeserialiseChatMessages(HttpResponseMessage response) {
            var serialisedResponse = await response.Content.ReadAsStringAsync();
            var dynMapResponse = JsonConvert.DeserializeObject<DynMapResponse>(serialisedResponse);
            _ticks = dynMapResponse.timestamp;
            var chatMessages = dynMapResponse.updates.Where(u => u.type == "chat");
            _logger.Trace($"{chatMessages.Count()} chat messages returned out of {dynMapResponse.updates.Count} updates.");
            return chatMessages;
        }

        private readonly HttpClient _client;
        private readonly string _host;
        private readonly Logger _logger;
        private long _ticks = 0;
    }

    public class DynMapResponsePlayer {
        public string world { get; set; }
        public int armor { get; set; }
        public string name { get; set; }
        public double x { get; set; }
        public double y { get; set; }
        public double health { get; set; }
        public double z { get; set; }
        public int sort { get; set; }
        public string type { get; set; }
        public string account { get; set; }
    }

    public class DynMapResponseUpdate {
        public string type { get; set; }
        public string name { get; set; }
        public object timestamp { get; set; }
        public string msg { get; set; }
        public double? x { get; set; }
        public double? y { get; set; }
        public double? z { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public string icon { get; set; }
        public string set { get; set; }
        public bool? markup { get; set; }
        public object desc { get; set; }
        public string dim { get; set; }
        public int? minzoom { get; set; }
        public int? maxzoom { get; set; }
        public string ctype { get; set; }
        public string source { get; set; }
        public string playerName { get; set; }
        public string message { get; set; }
        public string account { get; set; }
        public string channel { get; set; }
    }

    public class DynMapResponse {
        public int currentcount { get; set; }
        public bool hasStorm { get; set; }
        public List<DynMapResponsePlayer> players { get; set; }
        public bool isThundering { get; set; }
        public int confighash { get; set; }
        public int servertime { get; set; }
        public List<DynMapResponseUpdate> updates { get; set; }
        public long timestamp { get; set; }
    }
}