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

		[Flags]
		public enum Options : uint {
			Stream = 0,
			Datagram = 1 << 0,
			HasControl = 1 << 1,
			HasAccept = 1 << 2,
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
	}
}