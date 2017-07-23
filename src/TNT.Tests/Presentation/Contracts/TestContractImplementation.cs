﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TNT.Tests.Presentation.FullStack
{
    public class TestContractImplementation:ITestContract
    {
        public const int AskReturns = 42;

        public int SayCalledCount { get; set; }

        public void Say()
        {
            SayCalledCount++;
        }
        public List<string> SaySCalled { get; } = new List<string>();
        public void Say(string s)
        {
            SaySCalled.Add(s);
        }

        public void Say(string s, int i, long l)
        {
        }

        public int Ask()
        {
            return AskReturns;
        }

        public string Ask(string s)
        {
            return "not implemented";
        }

        public int Ask(string s, int i, long l)
        {
            return 0;
        }

        public Action OnSay { get; set; }
        public Action<string> OnSayS { get; set; }
        public Action<string, int, long> OnSaySIL { get; set; }
        public Func<int> OnAsk { get; set; }
        public Func<string, string> OnAskS { get; set; }
        public Func<string, int, long, string> OnAskSIL { get; set; }
    }
}
