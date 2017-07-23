﻿using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Expirements.General;
using TNT.Api;
using TNT.Presentation;
using TNT.Tcp;

namespace Expirements
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Thread.Sleep(500);
            var connection = TntBuilder
                .UseContract<ITestContract>()
                .UseContractInitalization((c,t) =>
                {
                    Console.WriteLine("Contract initialization");
                    c.AskCallBack += OnAskCallBack;
                    c.SayCallBack += OnSayCallBack;
                })
                .UseContractFinalization((c,t) =>
                {
                    Console.WriteLine("Contract deinitialization");
                    c.AskCallBack -= OnAskCallBack;
                    c.SayCallBack -= OnSayCallBack;
                })
                .CreateTcpClientConnection(IPAddress.Loopback, 17171);

            while (true)
            {
                var line = Console.ReadLine();
                if (line.ToLower() == "exit")
                    break;
                 var returned = connection.Contract.Ask(DateTime.Now, "theClient", line);
                 Console.WriteLine($"returned: {returned}");
            }
            connection.Channel.Disconnect();
            Console.ReadLine();
        }

        private static int OnAskCallBack(string s)
        {
            Console.WriteLine($"AskCallback: {s}");
            return 42;
        }

        private static void OnSayCallBack(int i)
        {
            Console.WriteLine("SayCallBack " + i);
        }
    }

}
