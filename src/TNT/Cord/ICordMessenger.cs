﻿using System;

namespace TNT.Cord
{
    public interface ICordMessenger
    {
        void Ask(short id, short askId, object[] arguments);
        void Say(int id, object[] values);
        void Ans(short id, short askId, object value);

        void HandleCallException(Exception ex);

        event Action<ICordMessenger, int, int, object[]> OnAsk;
        event Action<ICordMessenger, int, int, object> OnAns;

        event Action<ICordMessenger, int, object[]> OnSay;

        event Action<ICordMessenger, ExceptionMessage> OnException;
    }
}
