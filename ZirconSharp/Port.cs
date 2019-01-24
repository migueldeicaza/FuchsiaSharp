//
// Port.cs: API bindings for the Zircon Port base class
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
	/// Options to pass to the Port.Create method to create a new port.
	/// </summary>
	public enum ZxPortOptions : uint {
		None = 0,
		/// <summary>
		/// This binds the port to an interrupt.
		/// </summary>
		BindToInterrupt = 1,
	}

	public class Port : ZirconObject {
		internal Port (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}

		[DllImport (Library)]
		extern static ZxStatus zx_port_create (ZxPortOptions options, out uint result);

		/// <summary>
		/// Creates a new IO Port: a waitable object that can be used to read packets queued by kernel or by user-mode.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        and a valid IO port via result on success.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        options has an invalid value, or out is an invalid pointer or NULL.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future builds this error will no longer occur.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="result">Resulting created Port on success or null on error.</param>
		/// <param name="options">Options for the port.</param>
		/// <remarks>
		/// <para>If you need this port to be bound to an interrupt, pass BindToInterrupt to options, otherwise it should be 0.</para>
		/// <para>In the case where a port is bound to an interrupt, the interrupt packets are delivered via a dedicated queue on ports and are higher priority than other non-interrupt packets.</para>
		/// <para>The returned handle will have the following ZxRights: Transfer, Read, Write and Duplicate.</para>
		/// </remarks>
		public static ZxStatus Create (out Port result, ZxPortOptions options = ZxPortOptions.None)
		{
			uint nh;
			var ret = zx_port_create (options, out nh);
			if (ret == ZxStatus.Ok)
				result = new Port (nh, ownsHandle: true);
			else
				result = null;
			return ret;
		}
	}
}
