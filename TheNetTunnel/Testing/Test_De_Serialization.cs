﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using TNT.Serialization;
using TNT.Deserialization;

namespace Testing
{
	public class Test_De_Serialization
	{
		public void Primitive()
		{
			var io = 100;
			var ir = CheckAndRecreate (io);
			if (io != ir)
				throw new Exception ("io and ir are not equal");

		}

		public void Enum()
		{
			var e = testEn.third;
			var deserialized = CheckAndRecreate(e);
			if(e!=deserialized)
				throw new Exception ("original and deserizlized enums are not equal");
		}

		public void Unicode()
		{
			string origin = @"suppose I should be upset, even feel violated, but I'm not. No, in fact, I think this is a friendly message, like ""Hey, wanna play?"" and yes, I want to play. I really, really do. ";

			var deserialized = CheckAndRecreate (origin);

			if (String.Compare (origin, deserialized) != 0)
				throw new Exception ("original and deserizlized strings are not equal");
		}

		public void UTCFileTime()
		{
			DateTime origin = DateTime.Now;

			var deserialized = CheckAndRecreate (origin);

			if (DateTime.Compare (origin, deserialized)!= 0) {
				throw new Exception ("original and deserizlized timestamps are not equal");
			}
		}

	    public void ByteEnumeration(int EnnumerationSize)
	    {
            var bb = new List<byte>(EnnumerationSize);
            for (var i = 0; i < EnnumerationSize; i++)
	            bb.Add((byte) (i%255));
            
            IEnumerable<byte> enumb = bb;
	        var res = CheckAndRecreate<IEnumerable<byte>>(bb);
	        int j = 0;
            foreach (var r in res) {
                if (r != bb[j])
                    throw new Exception("original and deserialized byte enumeratior are not equal");
                j++;
            }
	    }

		public void ProtoBuf()
		{
			var origin = ToiletType.GetRandomType ();

			var deserialized = CheckAndRecreate (origin);

			if(!deserialized.IsEqual(origin))
				throw new Exception ("original and deserializerd toilets are not equal");
		}

		public void FixedSizeArrays()
		{
			List<double> doubles = new List<double> ();
			for (int i = 0; i < 1000; i++) {
				doubles.Add (Tools.Rnd.NextDouble ());
			}

			var origin = doubles.ToArray();
			var des = CheckAndRecreate (origin);

			if (!origin.SequenceEqual (des))
				throw new Exception ("Sent and received double[] are not equal");
		}

		public void DynamicSizeArrays()
		{
			string[] arr = new string[]{ "first", "second", "third" };
			var des = CheckAndRecreate (arr);
			if (!des.SequenceEqual (arr))
				throw new Exception ("Sent and received string[] are not equal ");
		}

		public void Sequence()
		{
			var origin = new TestSequence ();
			var objArr = CheckAndRecreateSequence (origin.Sequence);
			var des = new TestSequence ();
			des.Sequence = objArr;
			if(!origin.IsEqualTo(des))
				throw new Exception ("Sent and received sequences are not equal ");
		}

		public static T CheckAndRecreate<T>(T origin)
		{
			var ser = SerializersFactory.Create (typeof(T));
			var deser = DeserializersFactory.Create (typeof(T));
			var stream = new MemoryStream();
			ser.Serialize (origin, stream);

			var serT = ser as ISerializer<T>;

			var streamT = new MemoryStream ();

			serT.SerializeT (origin, streamT);

			var same = Tools.CompareStreams (stream, streamT);

			if (!same)
				throw new Exception ("serialized and seralizedT streams are not equal");

			stream.Position = 0;
			var transportStream = new MemoryStream ();

			TNT.StaticTools.CopyToAnotherStream (stream, transportStream, (int)stream.Length);

			transportStream.Position = 0;

			var deserialized = (deser as IDeserializer<T>).DeserializeT (transportStream, (int)transportStream.Length);

			return deserialized;

		}

		public static object[] CheckAndRecreateSequence(object[] origin)
		{
			var types = origin.Select (o => o.GetType ()).ToArray ();
			var ser = SerializersFactory.Create (types);
			var deser = DeserializersFactory.Create (types);

			var stream = new MemoryStream();
			ser.Serialize (origin, stream);

			var serT = ser as ISerializer<object[]>;

			var streamT = new MemoryStream ();

			serT.SerializeT (origin, streamT);

			var same = Tools.CompareStreams (stream, streamT);

			if (!same)
				throw new Exception ("serialized and seralizedT streams are not equal");

			stream.Position = 0;
			var transportStream = new MemoryStream ();

			TNT.StaticTools.CopyToAnotherStream (stream, transportStream, (int)stream.Length);

			transportStream.Position = 0;

			var deserialized = (deser as IDeserializer<object[]>).DeserializeT (transportStream, (int)transportStream.Length);

			return deserialized;
		}
	}

	public class TestSequence{

		public int i = Tools.Rnd.Next();

		public ToiletType toiletType = ToiletType.GetRandomType();

		public double d = Tools.Rnd.NextDouble();

		public string s = "str#"+Tools.Rnd.Next().ToString();

		public DateTime t = DateTime.Now;

		public Toilet toilet = Toilet.FindClosestToilet();

		public Type[] SequenceTypes{
			get{ return Sequence.Select (s => s.GetType()).ToArray (); }
		}

		public object[] Sequence
		{
			get{ return new object[]{ i, toiletType, d, s, t, toilet };}
			set{
				if (value.Length != Sequence.Length)
					throw new Exception ("Wrong input sequence lenght");
				if (!value.Select (v => v.GetType ()).SequenceEqual (SequenceTypes))
					throw new Exception ("wrong input types sequence");
				i = (int)value [0];
				toiletType = (ToiletType)value [1];
				d = (double)value [2];
				s = (string)value [3];
				t = (DateTime)value [4];
				toilet = (Toilet)value [5];
			}
		}
		public bool IsEqualTo(TestSequence ts)
		{
			return i == ts.i && d == ts.d && t.CompareTo (ts.t) == 0 && s.CompareTo (ts.s) == 0 && toilet.IsEqual (ts.toilet) && toiletType.IsEqual (ts.toiletType);
		}
	}


}
