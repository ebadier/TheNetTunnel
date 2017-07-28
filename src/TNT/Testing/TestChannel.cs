﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TNT.Exceptions.Local;
using TNT.Presentation;
using TNT.Transport;

namespace TNT.Testing
{
    public class TestChannel: IChannel
    {
        private readonly bool _threadQueue;
        private bool _wasConnected;
        private bool _allowReceive;
        ConcurrentQueue<byte[]> _receiveQueue = new ConcurrentQueue<byte[]>();
        private Task _receiveQueueHandlerTask = new Task(() => { });

        public TestChannel(bool threadQueue = true)
        {
            _threadQueue = threadQueue;
        }
        public void ImmitateReceive(byte[] message)
        {
            
            _receiveQueue.Enqueue(message);
            if(_threadQueue)
                _receiveQueueHandlerTask = _receiveQueueHandlerTask.ContinueWith((t) => HandleReceiveQueue());
            else
                HandleReceiveQueue();
        }

        void HandleReceiveQueue()
        {
            byte[] msg;
            _receiveQueue.TryDequeue(out msg);
            if(msg==null)
                return;
            if(!IsConnected)
                return;
            OnReceive?.Invoke(this, msg);
        }
        public void ImmitateConnect()
        {
            if(IsConnected)
                throw  new InvalidOperationException("Cannot to immitate connect while IsConnected = true");
            _wasConnected = true;
            IsConnected = true;
        }

        public void ImmitateDisconnect()
        {
            if (!IsConnected)
                throw new InvalidOperationException("Cannot to immitate disconnect while IsConnected = false");
            IsConnected = false;
            OnDisconnect?.Invoke(this, null);
        }

        public event Action<IChannel, byte[]> OnWrited;

        public bool IsConnected { get; private set; }

        public bool AllowReceive
        {
            get { return _allowReceive; }
            set
            {
                if(_allowReceive==value)
                    return;
                
                _allowReceive = value;

                if (_allowReceive)
                {
                    if(!_threadQueue)
                        HandleReceiveQueue();
                    else if (_receiveQueueHandlerTask.Status == TaskStatus.Created)
                        _receiveQueueHandlerTask.Start();
                }
                AllowReceiveChanged?.Invoke(this,value);
            }
        }

        public event Action<IChannel, bool> AllowReceiveChanged; 
        public event Action<IChannel, byte[]> OnReceive;
        public event Action<IChannel, ErrorMessage> OnDisconnect;
        public void Disconnect()
        {
          DisconnectBecauseOf(null);
        }
        public void DisconnectBecauseOf(ErrorMessage error)
        {
            if (IsConnected)
            {
                IsConnected = false;
                OnDisconnect?.Invoke(this, error);
            }
        }
        public Task<bool> TryWriteAsync(byte[] array)
        {
            return Task.Run(
                () => {
                    if (IsConnected)
                        Write(array);
                    return IsConnected;
                }
            );
        }

        public void Write(byte[] array)
        {
            if (!_wasConnected)
                throw new ConnectionIsNotEstablishedYet();
            if (!IsConnected)
                throw new ConnectionIsLostException();
            OnWrited?.Invoke(this, array);
        }

        public int BytesReceived { get; }
        public int BytesSent { get; }
        public string RemoteEndpointName { get; }
        public string LocalEndpointName { get; }
        public Task WriteAsync(byte[] data)
        {
            return Task.Run(() => Write(data));
        }
    }
}
