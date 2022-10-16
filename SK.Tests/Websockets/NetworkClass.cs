using System;
using WebSocketSharp.Server;

namespace Tests
{
    public class NetworkClass
    {
        public string buffer = string.Empty;
        public const string DEFAULT_IP = "127.0.0.1";
        public const string DEFAULT_PORT = "8080";
        public const string URL_FORMAT = "ws://{0}:{1}/";
        private WebSocketServer server;
        public NetworkClass(string port = "", string ip = "")
        {
            if (string.IsNullOrEmpty(port))
            {
                port = DEFAULT_PORT;
            }
            
            if (string.IsNullOrEmpty(ip))
            {
                ip = DEFAULT_IP;
            }

            server = new WebSocketServer(string.Format(URL_FORMAT, new string[] { ip , port }));

            server.AddWebSocketService("/", 
                () => new IndexRoute(this)
                { IgnoreExtensions = true });
            server.Start();

            Console.WriteLine("Server started");
        }

        public void broadcastData()
        {
            server.WebSocketServices["/"].Sessions.Broadcast(buffer);
        }

        public void Stop()
        {
            server.Stop();
        }
    }
}
