using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Channels
{
    /// <summary>
    /// An awaitable object that represents an asynchronous read operation
    /// </summary>
    public struct ReadableChannelAwaitable : ICriticalNotifyCompletion
    {
        private readonly IReadableBufferAwaiter _awaiter;
        private readonly CancellationToken _cancellationToken;
        private IDisposable _cancellationRegistration;

        public ReadableChannelAwaitable(IReadableBufferAwaiter awaiter, CancellationToken cancellationToken)
        {
            _awaiter = awaiter;
            _cancellationToken = cancellationToken;
            _cancellationRegistration = null;
        }

        public bool IsCompleted => _cancellationToken.IsCancellationRequested || _awaiter.IsCompleted;

        public ChannelReadResult GetResult()
        {
            _cancellationRegistration?.Dispose();
            _cancellationToken.ThrowIfCancellationRequested();
            return _awaiter.GetResult();
        }

        public ReadableChannelAwaitable GetAwaiter() => this;

        public void UnsafeOnCompleted(Action continuation) => OnCompleted(continuation);

        public void OnCompleted(Action continuation)
        {
            _cancellationRegistration = _cancellationToken.Register(continuation);
            _awaiter.OnCompleted(continuation);
        }
    }
}
