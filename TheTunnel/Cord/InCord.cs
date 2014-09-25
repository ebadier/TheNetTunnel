﻿using System;

namespace TheTunnel
{
	public class InCord<T> : IInCord<T>
	{
		public InCord(short cid, IDeserializer deserializer)
		{
			this.INCid = cid;
			this.Deserializer = deserializer;
			this.DeserializerT = deserializer as IDeserializer<T>;
		}
		#region IInCord implementation

		public event Action<IInCord, T> OnReceiveT;

		public IDeserializer<T> DeserializerT {	get; protected set; }

		#endregion

		#region IInCord implementation

		public event Action<IInCord, object> OnReceive;

		public bool Parse (byte[] msg, int offset)
		{
			T res;
			object ores;
			if (Deserializer.TryDeserialize (msg, offset,out ores)) {
				res = (T)ores;
				if (OnReceiveT != null)
					OnReceiveT (this, res);
				if (OnReceive != null)
					OnReceive (this, res);
				return true;
			}
			return false;
		}

		public short INCid { get;	protected set;}

		public IDeserializer Deserializer {	get; protected set;	}
		#endregion
	}
}

