namespace Imbick.Assistant.WebApi {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Authentication;

    public class EmailReceivedTrigger
        : Trigger, IFireable {

        public bool HasFired(IDictionary<string, TriggerParameter> triggerParameters) {
            var param = new TriggerParameter<string> {
                Name = "email_subject",
                Value = "test email"
            };

            // Create our client and a SslStream variable (notice it is no longer NetworkStream)
            TcpClient mail = new TcpClient();
            SslStream netStream;

            // Connect to gmail using 995 port
            mail.Connect("pop.gmail.com", 995);

            // Get the stream and toss it into the constructor of SslStream
            // Now netStream is a SSL secured stream
            netStream = new SslStream(mail.GetStream());

            // Last thing you need is to authenticate as a client (you are connecting to them so you are the client and they are the server). 
            // You have to pass in the name on their certificate, which so happens to be the same host they are using for connecting. "pop.gmail.com"
            netStream.AuthenticateAsClient("pop.gmail.com");

            //  Log in to the account
            response = streamReader.ReadLine();
                if (response.StartsWith("+OK"))
                {
                    response = SendReceive("USER ", "username" + "@" + "Domain.Trim()");
                    if (response.StartsWith("+OK"))
                    {
                        response = SendReceive("PASS ", "Password");
                    }
                }

                var stream = tcpClient.GetStream();

                    var ssl = new SslStream(stream);
                    ssl.AuthenticateAsClient("host", null, SslProtocols.Tls12, true);
                    stream = ssl;

                streamReader = new StreamReader(stream);
                if (response.StartsWith("+OK"))
                    result = true;

            triggerParameters.Add(param.Name, param);
            return true;
        }
    }
}