﻿using System;
using TNT.Exceptions.ContractImplementation;
using TNT.Exceptions.Local;

namespace TNT.Exceptions.Remote
{
    /// <summary>
    /// Exception, happens as the result of remote promblems, 
    /// like serialization, 
    /// wrong contractImplementation, 
    /// user code exceptions etc...
    /// </summary>
    public abstract class RemoteExceptionBase: Exception, ITntCallException
    {
        protected RemoteExceptionBase(
            
            RemoteExceptionId id, 
            string message = null, 
            Exception innerException = null)
            :base($"[{id}]"+ (message??(" tnt call exception")), innerException)
        {
            IsFatal = false;
            MessageId = null;
            AskId = null;
            Id = id;
        }

        public RemoteExceptionId Id { get; }

        public bool IsFatal { get; protected set; }

        public short? MessageId { get; protected set; }
        public short? AskId { get; protected set; }

        public static RemoteExceptionBase Create(RemoteExceptionId type, string additionalInfo, short? messageId,
            short? askId, bool isFatal = false)
        {
            switch (type)
            {
              case RemoteExceptionId.RemoteSideUnhandledException:
                    return new RemoteUnhandledException(messageId,askId, null, additionalInfo);
                case RemoteExceptionId.RemoteSideSerializationException:
                    return new RemoteSerializationException(messageId.Value, askId, isFatal, additionalInfo);
                case RemoteExceptionId.RemoteContractImplementationException:
                    return new RemoteContractImplementationException(messageId.Value, askId, isFatal, additionalInfo);
                default:
                    throw new InvalidOperationException(
                        $"Exception type {type} is unknown. Exception message: {additionalInfo}"); 
            }
        }
    }
}