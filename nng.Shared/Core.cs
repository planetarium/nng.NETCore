using nng.Native;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace nng
{
    public class NngException : Exception
    {
        public NngException(string message)
        : base(message)
        {
        }
        public NngException(int errorCode)
        {
            error = errorCode;
        }

        public override string Message => string.Empty;//nng_strerror(error);

        int error = 0;
    }

    /// <summary>
    /// Operations on part of a message; either the header or body
    /// </summary>
    public interface IMessagePart
    {
        int Append(byte[] data);
        int Append(uint data);
        int Chop(UIntPtr size);
        int Chop(out uint data);
        void Clear();
        int Insert(byte[] data);
        int Insert(uint data);
        int Length { get; }
        int Trim(UIntPtr size);
        int Trim(out uint data);
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
        ReadOnlySpan<byte> Raw { get; }
    }

    /// <summary>
    /// Message sent/received via nng
    /// </summary>
    public interface IMessage : IMessagePart, IDisposable
    {
        /// <summary>
        /// Get the underlying nng_msg
        /// </summary>
        /// <value></value>
        nng_msg NngMsg { get; }
        /// <summary>
        /// Get the header portion of the message
        /// </summary>
        /// <value></value>
        IMessagePart Header { get; }
        /// <summary>
        /// Duplicates the message creating a new, identical message.
        /// </summary>
        /// <returns>The newly created identical message duplicate</returns>
        IMessage Dup();
    }

    public static class Extensions
    {
        public static void TrySetNngError<T>(this TaskCompletionSource<T> socket, int error)
        {
            if (error == 0)
            {
                return;
            }
            socket.TrySetException(new NngException(error));
        }
    }
}