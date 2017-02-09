namespace Imbick.Assistant.Core.Steps.Samplers {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Steps;
    using System.Net.Http;
    using System.Threading.Tasks;

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
        }

        public override async Task<RunResult> Run(WorkflowState workflowState)
        {
            var response = await _client.GetAsync("/up/world/world/1234");
            if (!response.IsSuccessStatusCode)
                return new RunResult(false);

            var serialisedResponse = await response.Content.ReadAsStringAsync();
            var dynMapResponse = JsonConvert.DeserializeObject<DynMapResponse>(serialisedResponse);
            workflowState.Payload =
                dynMapResponse.updates.Where(u => u.type == "chat").Select(update => new MinecraftChatMessage {
                    Message = update.message,
                    Name = update.name
                });

            return RunResult.Passed;
        }

        private readonly HttpClient _client;
        private readonly string _host;
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