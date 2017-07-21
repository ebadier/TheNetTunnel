﻿using System;
using TNT.Exceptions;

namespace TNT.Cord
{
    public interface ICordMessenger
    {
        void Ask(short id, short askId, object[] arguments);
        void Say(int id, object[] values);
        void Ans(short id, short askId, object value);

        void HandleCallException(RemoteCallException rcException);

        event Action<ICordMessenger, CordRequestMessage> OnRequest;

        event Action<ICordMessenger, int, int, object> OnAns;

        event Action<ICordMessenger, ExceptionMessage> OnException;
    }
}
