namespace Imbick.Assistant.Core {
    using System;
    using System.Collections.Generic;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Text;

    public class EmailReceivedTrigger
        : Trigger, IFireable {

        public EmailReceivedTrigger(string username, string password) {
            _username = username;
            _password = password;
        }

        public bool HasFired(IDictionary<string, TriggerParameter> triggerParameters) {

            var mail = new TcpClient();
            mail.Connect("pop.gmail.com", 995);

            var netStream = new SslStream(mail.GetStream());

            // Last thing you need is to authenticate as a client (you are connecting to them so you are the client and they are the server). 
            // You have to pass in the name on their certificate, which so happens to be the same host they are using for connecting. "pop.gmail.com"
            netStream.AuthenticateAsClient("pop.gmail.com");

            var response = ReadResponse(netStream);
            if (!response.StartsWith("+OK"))
                throw new Exception();

            Authenticate(netStream);

            var param = new TriggerParameter<string> {
                Name = "email_subject",
                Value = "test email"
            };
            triggerParameters.Add(param.Name, param);
            return true;
        }

        private void Authenticate(SslStream netStream) {
            var response = SendReceive(netStream, $"USER {_username}");
            if (!response.StartsWith("+OK"))
                throw new Exception();
            response = SendReceive(netStream, $"PASS {_password}");
            if (!response.StartsWith("+OK"))
                throw new Exception();
        }

        private string SendReceive(SslStream netStream, string data) {
            var message = Encoding.UTF8.GetBytes(data);
            netStream.Write(message);
            return ReadResponse(netStream);
        }

        private string ReadResponse(SslStream sslStream) {
            // Read the  message sent by the client.
            // The client signals the end of the message using the
            // "<EOF>" marker.
            var buffer = new byte[2048];
            var messageData = new StringBuilder();
            int bytes;
            do {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                var decoder = Encoding.UTF8.GetDecoder();
                var chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                
                if (messageData.ToString().IndexOf("<EOF>") != -1) 
                    break;

            } while (bytes != 0);

            return messageData.ToString();
        }

        private readonly string _username;
        private readonly string _password;
    }
}