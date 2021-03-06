using nng.Native;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace nng
{
    // public class NngContext
    // {
    //     public bool NngCheck(int error,
    //         [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
    //         [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    //     {
    //         if (error == 0)
    //         {
    //             return false;
    //         }
    //         var str = nng_strerror(error);
    //         Console.WriteLine($"{memberName}:{sourceLineNumber} failed: {str}");
    //         return true;
    //     }
    // }

    // public interface IFactory
    // {
    //     IReplySocket CreateRep();
    //     IRequestSocket CreateReq();
    // }

    // public class AsyncFactory : IFactory
    // {
    //     public IReplySocket CreateRep()
    //     {

    //     }

    //     public IRequestSocket CreateReq()
    //     {

    //     }
    // }

    /// <summary>
    /// Context for asynchronous nng operations.  Most likely involves nng_aio, only involves nng_ctx if supported by protocol.
    /// </summary>
    public interface IAsyncContext : IHasSocket, IDisposable
    {
        void SetTimeout(int msTimeout);
        void Cancel();
    }

    /// <summary>
    /// Context supporting asynchronously sending messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISendAsyncContext<T> : IAsyncContext
    {
        Task<bool> Send(T message);
    }

    /// <summary>
    /// Context supporting asynchronously receiving messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReceiveAsyncContext<T> : IAsyncContext
    {
        Task<T> Receive(CancellationToken token);
    }

    /// <summary>
    /// Context supporting both asynchronously sending and receiving messages.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISendReceiveAsyncContext<T> : ISendAsyncContext<T>, IReceiveAsyncContext<T>
    {
    }

    /// <summary>
    /// Context with asynchronous request half of request/reply protocol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReqRepAsyncContext<T> : IAsyncContext
    {
        Task<T> Send(T message);
    }

    /// <summary>
    /// Context with asynchronous reply half of request/reply protocol
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepReqAsyncContext<T> : IAsyncContext
    {
        Task<T> Receive();
        Task<bool> Reply(T message);
    }

    /// <summary>
    /// Context supporting nng_ctx
    /// </summary>
    public interface ICtx : IAsyncContext
    {
        nng_ctx NngCtx { get; }

        int GetCtxOpt(string name, out bool data);
        int GetCtxOpt(string name, out int data);
        int GetCtxOpt(string name, out nng_duration data);
        int GetCtxOpt(string name, out UIntPtr data);
        // int GetCtxOpt(string name, out string data);
        // int GetCtxOpt(string name, out UInt64 data);

        int SetCtxOpt(string name, byte[] data);
        int SetCtxOpt(string name, bool data);
        int SetCtxOpt(string name, int data);
        int SetCtxOpt(string name, nng_duration data);
        int SetCtxOpt(string name, UIntPtr data);
        // int SetCtxOpt(string name, string data);
        // int SetCtxOpt(string name, UInt64 data);
    }

    /// <summary>
    /// Context with subscribe half of publish/subscribe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// For publish half see <see cref="ISendAsyncContext{T}"/>
    /// </remarks>
    public interface ISubAsyncContext<T> : IReceiveAsyncContext<T>, ISubscriber
    {
    }

    public static class AsyncContextExt
    {
        #region ISocket.CreateAsyncContext
        public static INngResult<ISendAsyncContext<T>> CreateAsyncContext<T>(this IPushSocket socket, IAPIFactory<T> factory) => factory.CreateSendAsyncContext(socket);
        public static INngResult<IReceiveAsyncContext<T>> CreateAsyncContext<T>(this IPullSocket socket, IAPIFactory<T> factory) => factory.CreateReceiveAsyncContext(socket);
        public static INngResult<ISendReceiveAsyncContext<T>> CreateAsyncContext<T>(this IBusSocket socket, IAPIFactory<T> factory) => factory.CreateSendReceiveAsyncContext(socket);
        public static INngResult<ISendAsyncContext<T>> CreateAsyncContext<T>(this IPubSocket socket, IAPIFactory<T> factory) => factory.CreateSendAsyncContext(socket);
        public static INngResult<ISubAsyncContext<T>> CreateAsyncContext<T>(this ISubSocket socket, IAPIFactory<T> factory) => factory.CreateSubAsyncContext(socket);
        public static INngResult<IReqRepAsyncContext<T>> CreateAsyncContext<T>(this IReqSocket socket, IAPIFactory<T> factory) => factory.CreateReqRepAsyncContext(socket);
        public static INngResult<IRepReqAsyncContext<T>> CreateAsyncContext<T>(this IRepSocket socket, IAPIFactory<T> factory) => factory.CreateRepReqAsyncContext(socket);
        #endregion
    }
}