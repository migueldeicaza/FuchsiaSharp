//
// Task.cs: API bindings for the Zircon Task base class
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
					var code = object_get_info ((uint)handle, ZxObjectInfoTopic.Socket, (IntPtr)(&buffer), (IntPtr)sizeof (InfoSocket), out var ignore1, out var ignore2);
					return buffer;
				}
			}
		}

		[Flags]
		public enum Options : uint {
			Stream = 0,
			Datagram = 1 << 0,
			HasControl = 1 << 1,
			HasAccept = 1 << 2,
		}

		public enum RWOptions : uint {
			None = 0,
			Control = 1 << 2
		}

		[DllImport (Library)]
		extern static ZxStatus zx_socket_create (Options options, out uint handle0, out uint handle1);

		public static ZxStatus CreateStream (out Socket handle0, out Socket handle1, bool hasControl = false, bool hasAccept = false)
		{
			return Create (Options.Stream | (hasControl ? Options.HasControl : 0) | (hasAccept ? Options.HasAccept : 0), out handle0, out handle1);
		}

		public static ZxStatus CreateDatagram (out Socket handle0, out Socket handle1, bool hasControl = false, bool hasAccept = false)
		{
			return Create (Options.Datagram | (hasControl ? Options.HasControl : 0) | (hasAccept ? Options.HasAccept : 0), out handle0, out handle1);
		}

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

		public ZxStatus Read (IntPtr buffer, ulong bufferSize, out ulong readBytes)
		{
			var ret = zx_socket_read ((uint)handle, RWOptions.None, buffer, (UIntPtr)bufferSize, out var lreadBytes);
			readBytes = (ulong)lreadBytes;
			return ret;
		}

		public ZxStatus ReadControl (IntPtr buffer, ulong bufferSize, out ulong readBytes)
		{
			var ret = zx_socket_read ((uint)handle, RWOptions.Control, buffer, (UIntPtr) bufferSize, out var lreadBytes);
			readBytes = (ulong)lreadBytes;
			return ret;
		}
	}
}