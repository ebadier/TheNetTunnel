﻿using System;

namespace TheTunnel
{
	public enum DisconnectReason:byte
	{
		ByContract = 0,
		UserWish = 1,
		ConnectionIsLost = 2,
	}
}
