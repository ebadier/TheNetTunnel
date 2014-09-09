﻿using System;
using WhAlpaTest;
using System.Linq;
using NUnit.Framework;

namespace Try
{
	[TestFixture ()]
	public class qAssembleQueueTest
	{
		[Test ()]
		public void Test ()
		{
			msgdone = 0;
			whSender sender = new whSender()
			{
				MaxQuantumSize = 123,
			};

			int concurentMessagesCount = 10;

			for (int i = 0; i < concurentMessagesCount; i++)
			{
				var msg = new byte[1000];
				msg [0] = (byte)i;
				for (int j = 10; j < 1000; j++)
				{
					msg[j] = (byte)(i * 10 + j % 10);
				}
				sender.Send(msg);
			}
			whReceiver receiver = new whReceiver();
			receiver.OnMsg += new Action<whReceiver, whMsg>(receiver_OnMsg);
			int chanid = 0;
			byte[] nmsg;
			while(sender.Next(out nmsg, out chanid))
			{
				receiver.Set(nmsg);
			}
			if (msgdone != concurentMessagesCount)
				throw new Exception("incorrect concurent message count");
			//Check that all data is sended
			if(sender.Lenght!=0)
				throw new Exception("Sender lenght is not empty");
		}
		int msgdone = 0;
		void receiver_OnMsg(whReceiver arg1, whMsg arg2)
		{
			int num = arg2.body [0];

			//Counting income data:
			int c1 = arg2.body.Count (a => a == num * 10 + 1);
			int c2 = arg2.body.Count (a => a == num * 10 + 2);
			int c3 = arg2.body.Count (a => a == num * 10 + 3);

			Console.WriteLine ("msg num: "+ msgdone+ " id: " + arg2.id + " b0: " + num + " c1: " + c1 + " c2: " + c2 + " c3: " + c3);

			//checking income data values
			if (!(c1 == c2 && c2 == c3 && c3 == 99))
				throw new Exception ("wrong income data values");
			if(!(num == msgdone && num == arg2.id))
				throw new Exception ("wrong income msg order");

			msgdone++;
		}
	}

}

