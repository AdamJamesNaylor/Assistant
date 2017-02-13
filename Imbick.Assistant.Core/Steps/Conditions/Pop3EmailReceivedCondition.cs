namespace Imbick.Assistant.Core.Steps.Conditions {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    using Steps;

    public class Pop3EmailReceivedConditionStep
        : ConditionStep, IDisposable {

        public Pop3EmailReceivedConditionStep(string username, string password, string domain)
            : base("Pop3 email received condition") {
            _username = username;
            _password = password;
            _tcpClient = null;
            _sslStream = null;
            _domain = domain;
            _state = new List<string>();
            _algorithm = MD5.Create();
        }

        public async override Task<RunResult> Run(WorkflowState workflowState) {
            Connect();

            Authenticate();

            var newMessage = RetrieveFirstNewMail();

            Disconnect();

            if (newMessage == null)
                return RunResult.Failed;

            workflowState.Payload = newMessage;
            return RunResult.Passed;
        }

        private Message RetrieveFirstNewMail() {
            var response = SendReceive("STAT\r\n");
            if (!response.StartsWith("+OK"))
                throw new Exception(); //log and return false
            var results = response.Split(' ');
            var count = int.Parse(results[1]);

            for (var i = 1; i <= count; i++) {
                response = SendReceive($"UIDL {i}\r\n");
                if (!response.StartsWith("+OK"))
                    throw new Exception(); //log and return false

                results = response.Split(' ');
                var uid = results[2];
                if (IsNewMessageId(uid)) {
                    RecordMessageId(uid);
                    return Retrieve(i);
                }
            }

            return null;
        }

        private Message Retrieve(int i) {
            var response = SendReceive($"RETR {i}\r\n");
            if (!response.StartsWith("+OK"))
                throw new Exception(); //log and return false

            using (var byteArrayBuilder = new MemoryStream()) {
                var first = true;
                byte[] lineRead;

                // Keep reading until we are at the end of the multi line response
                while (!IsLastLineInMultiLineResponse(lineRead = _sslStream.ReadLineAsBytes())) {
                    // We should not write CRLF on the very last line, therefore we do this
                    if (!first) {
                        // Write CRLF which was not included in the lineRead bytes of last line
                        byte[] crlfPair = System.Text.Encoding.ASCII.GetBytes("\r\n");
                        byteArrayBuilder.Write(crlfPair, 0, crlfPair.Length);
                    }
                    else {
                        // We are now not the first anymore
                        first = false;
                    }

                    // This is a multi-line. See http://tools.ietf.org/html/rfc1939#section-3
                    // It says that a line starting with "." and not having CRLF after it
                    // is a multi line, and the "." should be stripped
                    if (lineRead.Length > 0 && lineRead[0] == '.') {
                        // Do not write the first period
                        byteArrayBuilder.Write(lineRead, 1, lineRead.Length - 1);
                    }
                    else {
                        // Write everything
                        byteArrayBuilder.Write(lineRead, 0, lineRead.Length);
                    }
                }

                // Get out the bytes we have written to byteArrayBuilder
                return new Message(byteArrayBuilder.ToArray());
            }
        }

        public void Dispose() {
            Disconnect();
        }

        private void RecordMessageId(string uid) {
            _state.Add(uid);
        }

        private bool IsNewMessageId(string uid) {
            return !_state.Contains(uid);
        }

        private void Disconnect() {
            var response = SendReceive("QUIT\r\n");
            if (!response.StartsWith("+OK"))
                throw new Exception(); //a problem disconnecting

            if (_tcpClient != null && _tcpClient.Connected)
                _tcpClient.Close();

            if (_sslStream != null) {
                _sslStream.Close();
                _sslStream?.Dispose();
            }
        }

        private void Connect() {
            _tcpClient = new TcpClient(_domain, 995);
            _sslStream = new SslStream(_tcpClient.GetStream());

            _sslStream.AuthenticateAsClient(_domain);

            var response = ReadResponse();
            if (!response.StartsWith("+OK"))
                throw new Exception(); //log and return false
        }

        private void Authenticate() {
            var response = SendReceive($"USER {_username}\r\n");
            if (!response.StartsWith("+OK"))
                throw new Exception();
            response = SendReceive($"PASS {_password}\r\n");
            if (!response.StartsWith("+OK"))
                throw new Exception();
        }

        private string SendReceive(string data) {
            var message = System.Text.Encoding.ASCII.GetBytes(data);
            _sslStream.Write(message);
            return ReadResponse();
        }

        private string ReadResponse() {
            var buffer = new byte[2048];
            var messageData = new StringBuilder();
            int bytes;
            do {
                //potential for infinite loop
                bytes = _sslStream.Read(buffer, 0, buffer.Length);

                var decoder = System.Text.Encoding.ASCII.GetDecoder();
                var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);

                if (messageData.ToString().IndexOf("\r\n") != -1)
                    break;

            } while (bytes != 0);

            return messageData.ToString();
        }

        private static bool IsLastLineInMultiLineResponse(byte[] bytesReceived) {
            if (bytesReceived == null)
                throw new ArgumentNullException("bytesReceived");

            return bytesReceived.Length == 1 && bytesReceived[0] == '.';
        }

        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;
        private TcpClient _tcpClient;
        private SslStream _sslStream;
        private readonly HashAlgorithm _algorithm;
        private readonly List<string> _state;
    }
}