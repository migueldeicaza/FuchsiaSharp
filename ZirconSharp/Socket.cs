//
// Socket.cs: API bindings for the Zircon Socket class
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//
using System;
using System.Runtime.InteropServices;

namespace ZirconSharp {
	/// <summary>
	/// Sockets are a bidirectional stream transport. Unlike channels, sockets only move data (not handles).
	/// </summary>
	/// <remarks>
	/// <para>Data is written into one end of a socket via Write() and read from the opposing end via Read()</para>
	/// <para>Upon creation, both ends of the socket are writable and readable.</para>
	/// <para>Via the ShutdownOptions.Read and ShutdownOptions.Write options to Shutdown, one end of the socket can be closed for reading and/or writing.</para>
	/// <para>The following signals may be set for a socket object:</para>
	///    <list type="bullet">
	///    <item>
	///      <term>SocketReadable</term>
	///      <description>
	///        data is available to read from the socket
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketWritable</term>
	///      <description>
	///        data may be written to the socket
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketPeerClosed</term>
	///      <description>
	///        the other endpoint of this socket has been closed.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketPeerWriteDisabled</term>
	///      <description>
	///        writing is disabled permanently for the other endpoint either because of passing ZX_SOCKET_SHUTDOWN_READ to this endpoint or passing ZX_SOCKET_SHUTDOWN_WRITE to the peer. Reads on a socket endpoint with this signal raised will succeed so long as there is data in the socket that was written before writing was disabled.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketWriteDisabled</term>
	///      <description>
	///        writing is disabled permanently for this endpoint either because of passing ZX_SOCKET_SHUTDOWN_WRITE to this endpoint or passing ZX_SOCKET_SHUTDOWN_READ to the peer.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketControlReadable</term>
	///      <description>
	///        data is available to read from the socket control plane.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketControlWritable</term>
	///      <description>
	///        data may be written to the socket control plane.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketShare</term>
	///      <description>
	///        a socket may be sent via zx_socket_share.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketAccept</term>
	///      <description>
	///        a socket may be received via zx_socket_accept.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketReadThreshold</term>
	///      <description>
	///        data queued up on socket for reading exceeds the read threshold.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketWriteThreshold</term>
	///      <description>
	///        space available on the socket for writing exceeds the write threshold.
	///      </description>
	///    </item>
	/// </list>
	/// <para>The following properties may be queried from a socket object:</para>
	///    <list type="bullet">
	///    <item>
	///      <term>SocketReadable</term>
	///      <description>
	///        data is available to read from the socket
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketWritable</term>
	///      <description>
	///        data may be written to the socket
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketPeerClosed</term>
	///      <description>
	///        the other endpoint of this socket has been closed.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketPeerWriteDisabled</term>
	///      <description>
	///        writing is disabled permanently for the other endpoint either because of passing ZX_SOCKET_SHUTDOWN_READ to this endpoint or passing ZX_SOCKET_SHUTDOWN_WRITE to the peer. Reads on a socket endpoint with this signal raised will succeed so long as there is data in the socket that was written before writing was disabled.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketWriteDisabled</term>
	///      <description>
	///        writing is disabled permanently for this endpoint either because of passing ZX_SOCKET_SHUTDOWN_WRITE to this endpoint or passing ZX_SOCKET_SHUTDOWN_READ to the peer.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketControlReadable</term>
	///      <description>
	///        data is available to read from the socket control plane.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketControlWritable</term>
	///      <description>
	///        data may be written to the socket control plane.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketShare</term>
	///      <description>
	///        a socket may be sent via zx_socket_share.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketAccept</term>
	///      <description>
	///        a socket may be received via zx_socket_accept.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketReadThreshold</term>
	///      <description>
	///        data queued up on socket for reading exceeds the read threshold.
	///      </description>
	///    </item>
	///    <item>
	///      <term>SocketWriteThreshold</term>
	///      <description>
	///        space available on the socket for writing exceeds the write threshold.
	///      </description>
	///    </item>
	/// </list>
	/// </remarks>
	public class Socket : ZirconObject {
		internal Socket (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}

		/// <summary>
		/// Information about a socket.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct InfoSocket {
			/// <summary>
			/// The options passed to Socket.Create.
			/// </summary>
			public Options CreationOptions;
			/// <summary>
			/// <para>The maximum size of the receive buffer of a socket, in bytes.</para>
			/// <para>The receive buffer may become full at a capacity less than the maximum due to overhead</para>
			/// </summary>
			public ulong RxBufMax;
			/// <summary>
			/// <para>The size of the receive buffer of a socket, in bytes.</para>
			/// </summary>
			public ulong RxBufSize;
			/// <summary>
			/// <para>The amount of data, in bytes, that is available for reading in a single Socket.Read call.</para>
			/// <para>For stream sockets, this value will match |rx_buf_size|. For datagram 
			/// sockets, this value will be the size of the next datagram in the receive buffer.</para>
			/// </summary>			
			public ulong RxBufAvailable;
			/// <summary>
			/// <para>The maximum size of the transmit buffer of a socket, in bytes.</para>
			/// <para>The transmit buffer may become full at a capacity less than the maximum due to overhead</para>
			/// <para>Will be zero if the peer endpoint is closed.</para>
			/// </summary>
			public ulong TxBufMax;
			/// <summary>
			/// The size of the transmit buffer of a socket, in bytes.  Will be zero if the peer endpoint is closed.
			/// </summary>
			public ulong TxBufSize;
		}

		/// <summary>
		/// Returns information about this socket.
		/// </summary>
		/// <value>The socket info.</value>
		public InfoSocket SocketInfo {
			get {
				unsafe {
					InfoSocket buffer = new InfoSocket ();
					var code = zx_object_get_info ((uint)handle, ZxObjectInfoTopic.Socket, (IntPtr)(&buffer), (IntPtr)sizeof (InfoSocket), out var ignore1, out var ignore2);
					return buffer;
				}
			}
		}

		/// <summary>
		/// Options for socket creation
		/// </summary>
		[Flags]
		public enum Options : uint {
			/// <summary>
			/// Creates a stream socket.
			/// </summary>
			Stream = 0,
			/// <summary>
			///  Creates a datagram socket
			/// </summary>
			Datagram = 1 << 0,
			/// <summary>
			/// flag may be set to enable the socket control plane.
			/// </summary>
			HasControl = 1 << 1,
			/// <summary>
			/// flag may be set to enable transfer of sockets over this socket via Share()() and Accept()
			/// </summary>
			HasAccept = 1 << 2,
		}

		/// <summary>
		/// Options for the Read and Write operations.
		/// </summary>
		public enum RWOptions : uint {
			None = 0,
			/// <summary>
			/// attempts to read from the socket control plane.
			/// </summary>
			Control = 1 << 2
		}

		[DllImport (Library)]
		extern static ZxStatus zx_socket_create (Options options, out uint handle0, out uint handle1);

		/// <summary>
		/// Create a pair of stream sockets with the specified options, returned in handle0 and handle1.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success and the handle0 and handle1 point to the two socket pairs. In the event of failure, one of the following values is returned.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        out0 or out1 is an invalid pointer or NULL or options is any value other than ZX_SOCKET_STREAM or ZX_SOCKET_DATAGRAM.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future build this error will no longer occur.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="handle0">Returns the first socket pair.</param>
		/// <param name="handle1">Returns the first socket pair.</param>
		/// <remarks>
		///  creates a socket, a connected pair of bidirectional stream transports, that can move only data, and that have a maximum capacity.
		/// Data written to one handle may be read from the opposite.
		/// </remarks>
		public static ZxStatus CreateStream (out Socket handle0, out Socket handle1, bool hasControl = false, bool hasAccept = false)
		{
			return Create (Options.Stream | (hasControl ? Options.HasControl : 0) | (hasAccept ? Options.HasAccept : 0), out handle0, out handle1);
		}

		/// <summary>
		/// Create a pair of datagram sockets with the specified options, returned in handle0 and handle1.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success and the handle0 and handle1 point to the two socket pairs. In the event of failure, one of the following values is returned.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        out0 or out1 is an invalid pointer or NULL or options is any value other than ZX_SOCKET_STREAM or ZX_SOCKET_DATAGRAM.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future build this error will no longer occur.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="handle0">Returns the first socket pair.</param>
		/// <param name="handle1">Returns the first socket pair.</param>
		/// <remarks>
		///  creates a socket, a connected pair of bidirectional stream transports, that can move only data, and that have a maximum capacity.
		/// Data written to one handle may be read from the opposite.
		/// </remarks>
		public static ZxStatus CreateDatagram (out Socket handle0, out Socket handle1, bool hasControl = false, bool hasAccept = false)
		{
			return Create (Options.Datagram | (hasControl ? Options.HasControl : 0) | (hasAccept ? Options.HasAccept : 0), out handle0, out handle1);
		}

		/// <summary>
		/// Create a pair of sockets with the specified options, returned in handle0 and handle1.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success and the handle0 and handle1 point to the two socket pairs. In the event of failure, one of the following values is returned.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        out0 or out1 is an invalid pointer or NULL or options is any value other than ZX_SOCKET_STREAM or ZX_SOCKET_DATAGRAM.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future build this error will no longer occur.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="options">Use to select stream or datagram, and whether to enable a socket control plane and whether sockets can he transferred over this socket.</param>
		/// <param name="handle0">Returns the first socket pair.</param>
		/// <param name="handle1">Returns the first socket pair.</param>
		/// <remarks>
		///  creates a socket, a connected pair of bidirectional stream transports, that can move only data, and that have a maximum capacity.
		/// Data written to one handle may be read from the opposite.
		/// </remarks>
		public static ZxStatus Create (Options options, out Socket handle0, out Socket handle1)
		{
			var res = zx_socket_create (options, out var rhandle0, out var rhandle1);
			if (res == ZxStatus.Ok) {
				handle0 = new Socket (rhandle0, ownsHandle: true);
				handle1 = new Socket (rhandle1, ownsHandle: true);
			} else {
				handle0 = null;
				handle1 = null;
			}
			return res;
		}

		[DllImport (Library)]
		extern static ZxStatus zx_socket_accept (uint handle, out uint result);

		/// <summary>
		/// Receive another socket object via a socket
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>	
		///        on success and the received handle is returned via out_socket. In the event of failure, one of the following values is returned.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        socket is invalid.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        socket is not a socket handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        socket lacks ZX_RIGHT_READ.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        out_socket is an invalid pointer.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrShouldWait</term>
		///      <description>
		///        There is no new socket ready to be accepted.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNotSupported</term>
		///      <description>
		///        This socket does not support the transfer of sockets. It was not created with the ZX_SOCKET_HAS_ACCEPT option.
		///      </description>
		///    </item>
		/// </list>
		///</returns>
		/// <param name="result">The resulting socket on success</param>
		/// <remarks> attempts to receive a new socket via an existing socket connection. The signal ZX_SOCKET_ACCEPT is asserted when there is a new socket available.</remarks>
		public ZxStatus Accept (out Socket result)
		{
			var res = zx_socket_accept ((uint)handle, out var rhandle);
			if (res == ZxStatus.Ok)
				result = new Socket (rhandle, ownsHandle: true);
			else
				result = null;
			
			return res;
		}


		[DllImport (Library)]
		extern static ZxStatus zx_socket_read (uint handle,
					   RWOptions options,
					   IntPtr buffer,
					   /* size_t */ UIntPtr buffer_size,
					   /* size_t* */ out UIntPtr actual);

		[DllImport (Library, EntryPoint="zx_socket_read")]
		extern static ZxStatus zx_socket_read2 (uint handle,
					   RWOptions options,
					   IntPtr buffer,
					   /* size_t */ UIntPtr buffer_size,
					   UIntPtr actualIgnored);

		public ZxStatus Read (IntPtr buffer, ulong bufferSize, out ulong readBytes, RWOptions options = RWOptions.None)
		{
			var ret = zx_socket_read ((uint)handle, options, buffer, (UIntPtr)bufferSize, out var lreadBytes);
			readBytes = (ulong)lreadBytes;
			return ret;
		}

		public ZxStatus Read (IntPtr buffer, ulong bufferSize, RWOptions options = RWOptions.None)
		{
			var ret = zx_socket_read2 ((uint)handle, options, buffer, (UIntPtr)bufferSize, UIntPtr.Zero);
			return ret;
		}

		[DllImport (Library)]
		extern static ZxStatus zx_socket_share (uint handle, uint socketToShare);

		public ZxStatus Share (Socket socketToShare)
		{
			if (socketToShare == null)
				throw new ArgumentNullException (nameof (socketToShare));
			return zx_socket_share ((uint)handle, (uint) socketToShare.handle);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_socket_write (uint handle,
					   RWOptions options,
					   IntPtr buffer,
					   /* size_t */ UIntPtr buffer_size,
					   /* size_t* */ out UIntPtr actual);
		[DllImport (Library, EntryPoint="zx_socket_write")]
		extern static ZxStatus zx_socket_write2 (uint handle,
					   RWOptions options,
					   IntPtr buffer,
					   /* size_t */ UIntPtr buffer_size,
					   /* size_t* */ UIntPtr actual);

		public ZxStatus Write (IntPtr buffer, ulong bufferSize, out ulong writtenBytes, RWOptions options = RWOptions.None)
		{
			var ret = zx_socket_write ((uint)handle, options, buffer, (UIntPtr)bufferSize, out var lwrittenBytes);
			writtenBytes = (ulong)lwrittenBytes;
			return ret;
		}

		public ZxStatus Write (IntPtr buffer, ulong bufferSize, RWOptions options = RWOptions.None)
		{
			var ret = zx_socket_write2 ((uint)handle, options, buffer, (UIntPtr)bufferSize, UIntPtr.Zero);
			return ret;
		}

		[Flags]
		public enum ShutdownOptions : uint {
			None = 0,
			Write = 1,
	    		Read = 2
		}

		[DllImport (Library)]
		extern static ZxStatus zx_socket_shutdown (uint handle, ShutdownOptions options);

		public ZxStatus Shutdown (ShutdownOptions options = ShutdownOptions.None)
		{
			return zx_socket_shutdown ((uint)handle, options);
		}
	}
}