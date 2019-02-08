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

	/// <summary>
	/// This determines the type of a packet in ZxPortPacket
	/// </summary>
	public enum ZxPortPacketType : uint {
		User,
		SignalOne,
		SignalRep,
		GuestBell,
		GuestMem,
		GuestIO,
		GuestVcpu,
		Interrupt,
		ExceptionBit = 8, // If this is set, the exception is a 16 bit value 8 bits shifted)
		TypeMask = 0xff
	}

	[StructLayout (LayoutKind.Sequential)]
	/// <summary>
	/// Structure used to pass data to a port, for ZxPortPacketType
	/// </summary>
	public struct ZxPacketUser {
		ulong a, b, c, d;
	}

	/// <summary>
	/// Structure used to pass signals over a port, for ZxPortPacketType.SignalOne and ZxPortPacketType.SignalRep
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	public struct ZxPacketSignal {
		uint signal_trigger;
		uint signal_observed;
		ulong count;
		ulong reserved0;
		ulong reserved1;
	}

	/// <summary>
	/// Structure used to pass exceptions over a port, for ZxPortPacketType.ExceptionBit 
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	public struct ZxPacketException {
		ulong pid;
		ulong tid;
		ulong reserved0;
		ulong reserved1;
	}

	/// <summary>
	/// Structure used to pass GuestBell over a port, for ZxPortPacketType.GuestBell
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	public struct ZxPacketGuestBell {
		ulong addr;
		ulong reserved0;
		ulong reserved1;
		ulong reserved2;
	}

	/// <summary>
	/// Structure used to pass Guest memory over a port, for ZxPortPacketType.GuestMem
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	public struct ZxPacketGuestMem {
		ulong addr;

		// The interpretation of the fields below is architecture specific aarch64 or x86064
		ulong reserved0;
		ulong reserved1;
		ulong reserved2;
	}

	/// <summary>
	/// Structure used to pass Guest IO over a port, for ZxPortPacketType.GuestIO
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	public struct ZxPacketGuestIO {
		ushort port;
		byte accessSize;
		byte input;
		uint data;
		ulong reserved0;
		ulong reserved1;
		ulong reserved2;
	}


	public enum ZxPacketGuestVcpuType : byte {
		Interrupt,
		Startup
	}

	/// <summary>
	/// Structure used to pass Guest Vcpu over a port, for ZxPortPacketType.GuestMem
	/// </summary>
	[StructLayout (LayoutKind.Explicit)]
	public struct ZxPacketGuestVcpu {
		[FieldOffset(0)]
		ulong interrupt_mask;
		[FieldOffset (8)]
		byte interrupt_vector;

		[FieldOffset (0)]
		ulong startup_id;
		[FieldOffset (8)]
		ulong startup_entry;

		[FieldOffset (16)]
		ZxPacketGuestVcpuType type;

		[FieldOffset (24)]
		ulong reserved;
	}

	/// <summary>
	/// Structure used to pass an interrupt over a port, for ZxPortPacketType.Interrupt
	/// </summary>
	[StructLayout (LayoutKind.Sequential)]
	public struct ZxPacketInterrupt {
		ZxTime timestamp;
		ulong reserved0;
		ulong reserved1;
		ulong reserved2;
	}
	[StructLayout (LayoutKind.Explicit)]
	public struct ZxPortPacket {
		[FieldOffset(0)]
		public long Key;
		[FieldOffset (8)]
		public ZxPortPacketType Type;

		[FieldOffset (12)]
		public ZxStatus Status;


		// Union
		[FieldOffset (16)]
		public ZxPacketUser User;

		[FieldOffset (16)]
		public ZxPacketSignal Signal;

		[FieldOffset (16)]
		public ZxPacketException Exception;

		[FieldOffset (16)]
		public ZxPacketGuestBell GuestBell;

		[FieldOffset (16)]
		public ZxPacketGuestMem GuestMem;
		[FieldOffset (16)]
		public ZxPacketGuestIO GuestIO;
		[FieldOffset (16)]
		public ZxPacketGuestVcpu GuestVcpu;
		[FieldOffset (16)]
		public ZxPacketInterrupt Interrupt;
	}

	/// <summary>
	/// Signaling and mailbox primitive.
	/// <remarks>
	/// Ports allow threads to wait for packets to be delivered from various events. These events include 
	/// explicit queueing on the port, asynchronous waits on other handles bound to the port, and asynchronous 
	/// message delivery from IPC transports.
	/// </remarks>
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

		[DllImport (Library)]
		extern static ZxStatus zx_port_queue (uint handle, ref ZxPortPacket data);

		/// <summary>
		/// Queue a packet to an port
		/// </summary>
		/// <returns>
		/// ///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on successful queue of a packet.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle isn't a valid handle
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        packet is an invalid pointer.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not a port handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have ZX_RIGHT_WRITE.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrShouldWait</term>
		///      <description>
		///        the port has too many pending packets. Once a thread has drained some packets a new zx_port_queue() call will likely succeed.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="packet">Packet to queue into the port.</param>
		/// <remarks>The queue is drained by calling the Wait method.</remarks>
		public ZxStatus Queue (ref ZxPortPacket packet)
		{
			return zx_port_queue ((uint) handle, ref packet);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_port_wait (uint handle, ulong deadline, out ZxPortPacket data);

		/// <summary>
		/// Wait the specified deadline and data.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on successful packet dequeuing.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        packet isn't a valid pointer
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have ZX_RIGHT_READ and may not be waited upon.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrTimedOut</term>
		///      <description>
		///        deadline passed and no packet was available.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="deadline">Maximum amount of time to wait for a packet to arrive, if the timeout passes, the ZxStatus.TimedOut value will be returned.</param>
		/// <param name="data">The data that was retrieved from the port's queue.</param>
		/// <remarks>
		/// <para>
		///   Wait is a blocking syscall which causes the caller to wait until at least one packet is available.
		/// </para>
		/// 
		/// <para>
		///   Upon return, if successful packet will contain the earliest (in FIFO order) available packet data.
		/// </para>
		/// 
		/// <para>
		///   The deadline indicates when to stop waiting for a packet (with respect
		///   to ZxClock.Monotonic) and will be automatically adjusted according to
		///   the job's timer slack policy. If no packet has arrived by the
		///   deadline, ZxStatus.ErrTimedOut is returned. The value ZxTime.Infinite
		///   will result in waiting forever. A value in the past will result in an
		///   immediate timeout, unless a packet is already available for reading.
		/// </para>
		/// 
		/// <para>
		///   Unlike Object.WaitOne and Object.WaitMany only one waiting thread is
		///   released (per available packet) which makes ports amenable to be
		///   serviced by thread pools.
		/// </para>
		/// 
		/// <para>
		///   There are two classes of packets: packets queued by userspace with
		///   Queue() and packets queued by the kernel when objects a port
		///   is registered with change state. In both cases the packet is always of
		///   type ZxPortPacket
		/// </para>
		/// 
		/// <para>
		///   In the case of packets generated via Queue(), type will be set
		///   to ZxPortPacketType.User, and the caller of Queue() controls all
		///   other values in the ZxPortPacket structure. Access to the packet
		///   data is provided by the user member, given by the ZxPacketUser.
		/// </para>
		/// 
		/// <para>
		///   For packets generated by the kernel, type can be one of the following values:
		/// </para>
		/// <list type="bullet">
		///    <item>
		///      <term>ZxPortPacketType.SignalOne, ZxPortPacketType.SignalRep</term>
		///      <description>
		///        generated by objects registered via zx_object_wait_async()
		///      </description>
		///    </item>
		///    <item>
		///      <term>PktTypeException</term>
		///      <description>
		///        generated by objects registered via zx_task_bind_exception_port()
		///      </description>
		///    </item>
		///    <item>
		///      <term>ZxPortPacketType.GuestBell, ZxPortPacketType.GuestMem, ZxPortPacketType.GuestVcpu, ZxPortPacketType.GuestIO</term>
		///      <description>
		///        generated by objects registered via zx_guest_set_trap().
		///      </description>
		///    </item>
		///    <item>
		///      <term>ZxPortPacketType.Interrupt</term>
		///      <description>
		///        generated by objects registered via zx_interrupt_bind().
		///      </description>
		///    </item>
		/// </list>
		/// <para>All kernel queued packets will have status set to ZxStatus.Ok and key set to the value provided to the registration syscall. 
		/// For details on how to interpret the union, see the corresponding registration syscall.
		/// </para>
		/// </remarks>
		public ZxStatus Wait (ZxTime deadline, out ZxPortPacket data)
		{
			return zx_port_wait ((uint) handle, (ulong) deadline.NanoSeconds, out data);
		}

	}
}
