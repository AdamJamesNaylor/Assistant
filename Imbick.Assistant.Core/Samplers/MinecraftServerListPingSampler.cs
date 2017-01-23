namespace Imbick.Assistant.Core.Samplers {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using Newtonsoft.Json;

    public class MinecraftPlayer {
        public string Name { get; set; }
        public string Id { get; set; }
    }

    public class MinecraftServerListPingSampler
        : IRunnable {

        private readonly List<byte> _buffer;
        private NetworkStream _stream;
        private int _offset;
        private readonly string _host;
        private readonly short _port;
        private TcpClient _client;

        public MinecraftServerListPingSampler(string host, short port = 25565) {
            _host = host;
            _port = port;
            _buffer = new List<byte>();
            _offset = 0;
        }

        public StepRunResult Run(IDictionary<string, WorkflowParameter> workflowParameters) {

            using (_client = new TcpClient()) {
                if (!Connect())
                    return new StepRunResult(false);

                SendHandshakePacket();

                //Send a "Status Request" packet
                //http://wiki.vg/Server_List_Ping#Ping_Process
                Flush(0);

                var buffer = new byte[4096];
                _stream.Read(buffer, 0, buffer.Length);

                try {
                    var length = ReadVarInt(buffer);
                    var packet = ReadVarInt(buffer);
                    var jsonLength = ReadVarInt(buffer);

                    var json = ReadString(buffer, jsonLength);
                    var ping = JsonConvert.DeserializeObject<PingPayload>(json);

                    workflowParameters.Add("MinecraftServerPlayerCount",
                        new WorkflowParameter<int>("MinecraftServerPlayerCount", ping.Players.Online));
                    if (ping.Players.Online > 0) {
                        var players = ping.Players.Sample.Select(t => new MinecraftPlayer {Name = t.Name, Id = t.Id}).ToList();
                        var param = new WorkflowParameter<MinecraftPlayer[]>("MinecraftServerPlayers", players.ToArray()); //may need to be a intrinsic workflow param for each player so that subsequent steps don't need to understand the MinecraftPlayer type.
                        workflowParameters.Add("MinecraftServerPlayers", param);
                    }
                } catch (IOException) {
                    //If an IOException is thrown then the server didn't 
                    //send us a VarInt or sent us an invalid one.
                    return new StepRunResult(false);
                } finally {
                    Disconnect();
                }
                return new StepRunResult();
            }
        }

        private void SendHandshakePacket() {
            //http://wiki.vg/Server_List_Ping#Ping_Process
            WriteVarInt(47);
            WriteString(_host);
            WriteShort(_port);
            WriteVarInt(1);
            Flush(0);
        }

        private bool Connect() {
            try {
                _client.Connect(_host, _port);
            } catch (SocketException ex) {
                //log unable to connect
                return false;
            }
            if (!_client.Connected) {
                //log unable to connect
                return false;
            }

            _stream = _client.GetStream();
            return true;
        }

        private void Disconnect() {
            _client.Close();
            _offset = 0;
        }

        internal byte ReadByte(byte[] buffer) {
            var b = buffer[_offset];
            _offset += 1;
            return b;
        }

        internal byte[] Read(byte[] buffer, int length) {
            var data = new byte[length];
            Array.Copy(buffer, _offset, data, 0, length);
            _offset += length;
            return data;
        }

        internal int ReadVarInt(byte[] buffer) {
            var value = 0;
            var size = 0;
            int b;
            while (((b = ReadByte(buffer)) & 0x80) == 0x80) {
                value |= (b & 0x7F) << (size++*7);
                if (size > 5) {
                    throw new IOException("This VarInt is an imposter!");
                }
            }
            return value | ((b & 0x7F) << (size*7));
        }

        internal string ReadString(byte[] buffer, int length) {
            var data = Read(buffer, length);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        internal void WriteVarInt(int value) {
            while ((value & 128) != 0) {
                _buffer.Add((byte) (value & 127 | 128));
                value = (int) ((uint) value) >> 7;
            }
            _buffer.Add((byte) value);
        }

        internal void WriteShort(short value) {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        internal void WriteString(string data) {
            var buffer = System.Text.Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer.Length);
            _buffer.AddRange(buffer);
        }

        internal void Write(byte b) {
            _stream.WriteByte(b);
        }

        internal void Flush(int id = -1) {
            var buffer = _buffer.ToArray();
            _buffer.Clear();

            var add = 0;
            var packetData = new[] {(byte) 0x00};
            if (id >= 0) {
                WriteVarInt(id);
                packetData = _buffer.ToArray();
                add = packetData.Length;
                _buffer.Clear();
            }

            WriteVarInt(buffer.Length + add);
            var bufferLength = _buffer.ToArray();
            _buffer.Clear();

            _stream.Write(bufferLength, 0, bufferLength.Length);
            _stream.Write(packetData, 0, packetData.Length);
            _stream.Write(buffer, 0, buffer.Length);
        }
    }

    /// <summary>
    /// C# represenation of the following JSON file
    /// https://gist.github.com/thinkofdeath/6927216
    /// </summary>
    internal class PingPayload {
        /// <summary>
        /// Protocol that the server is using and the given name
        /// </summary>
        [JsonProperty(PropertyName = "version")]
        public VersionPayload Version { get; set; }

        [JsonProperty(PropertyName = "players")]
        public PlayersPayload Players { get; set; }

        [JsonProperty(PropertyName = "description")]
        public DescriptionPayload Description { get; set; }

        /// <summary>
        /// Server icon, important to note that it's encoded in base 64
        /// </summary>
        [JsonProperty(PropertyName = "favicon")]
        public string Icon { get; set; }
    }

    internal class VersionPayload {
        [JsonProperty(PropertyName = "protocol")]
        public int Protocol { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    internal class PlayersPayload {
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }

        [JsonProperty(PropertyName = "online")]
        public int Online { get; set; }

        [JsonProperty(PropertyName = "sample")]
        public List<Player> Sample { get; set; }
    }

    internal class Player {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    internal class DescriptionPayload {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
    }
}