//
// Core.cs: Core types and definitions, mostly a C# wrapper around fuchsia/zircon/system/public/zircon/types.h
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//
using System;
namespace ZirconSharp {
	public enum ZxStatus {

	}

	/// <summary>
	/// Absolute time in nanoseconds (generally with respect to the monotonic clock)
	/// </summary>
	public struct ZxTime {
		// TODO: Will have to adjust this offset, once I figure out what the baseline offset is in Zircon
		const long offset = 0;

		public static ZxTime Infinite => new ZxTime (Int64.MaxValue);
		public static ZxTime InfinitePast => new ZxTime (Int64.MinValue);
		public long NanoSeconds;

		public ZxTime (long nanoseconds)
		{
			NanoSeconds = nanoseconds;
		}

		public ZxTime (DateTime dateTime)
		{
			NanoSeconds = dateTime.Ticks * 100 + offset;
		}

		public DateTime DateTime => new DateTime (NanoSeconds / 100, DateTimeKind.Utc);

		public override string ToString () => $"[{DateTime},nanoseconds={NanoSeconds}]";

	}

	/// <summary>
	/// A duration in nanoseconds
	/// </summary>
	public struct ZxDuration {
		long NanoSeconds;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ZirconSharp.ZxDuration"/> based on the specified number of Nanoseconds.
		/// </summary>
		/// <param name="nanoseconds">Nanoseconds.</param>
		public ZxDuration (long nanoseconds)
		{
			this.NanoSeconds = nanoseconds;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ZirconSharp.ZxDuration"/> struct from a TimeSpan.
		/// </summary>
		/// <param name="timeSpan">Time span.</param>
		public ZxDuration (TimeSpan timeSpan)
		{
			NanoSeconds = timeSpan.Ticks * 100;
		}

		/// <summary>
		/// Creates a ZxDuration from the specified number of microseconds.
		/// </summary>
		/// <returns>A new instance of ZxDuration representing the specified duration in microseconds.</returns>
		/// <param name="x">The x coordinate.</param>
		public ZxDuration FromMicroSeconds (long x) => new ZxDuration (x * 1000);
		public ZxDuration FromMilliSeconds (long x) => new ZxDuration (x * 1000000);
		public ZxDuration FromSeconds (long x) => new ZxDuration (x * 1000000000);
		public ZxDuration FromMinutes (long x) => new ZxDuration (x * 60000000000);
		public ZxDuration FromHours (long x) => new ZxDuration (x * 3600000000000);

		/// <summary>
		/// Returns the ZxDuration as a TimeSpan
		/// </summary>
		/// <value>The time span.</value>
		public TimeSpan TimeSpan => new TimeSpan (NanoSeconds / 100);

		public override string ToString () => $"[{TimeSpan},nanoseconds={NanoSeconds}]";
	}

	/// <summary>
	/// A duration in hardware ticks
	/// </summary>
	public struct ZxTicks {
		ulong HardwareTicks;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ZirconSharp.ZxTicks"/> from the specified hardware ticks.
		/// </summary>
		/// <param name="hardwareTicks">Hardware ticks.</param>
		public ZxTicks (ulong hardwareTicks)
		{
			this.HardwareTicks = hardwareTicks;
		}

		public override string ToString () => $"{HardwareTicks}";
	}

	/// <summary>
	/// Global Kernel Object identifier.
	/// </summary>
	/// <remarks>
	/// kernel object ids use 63 bits, with the most significant bit being zero.
	/// The remaining values (msb==1) are for use by programs and tools that wish to
	/// create koids for artificial objets.
	/// </remarks>
	public struct ZxKernelObjectId {
		public ulong KOID;

		public ZxKernelObjectId Invalid => new ZxKernelObjectId (0);
		public ZxKernelObjectId Kernel => new ZxKernelObjectId (1);
		public ZxKernelObjectId (ulong v)
		{
			KOID = v;
		}

		public override string ToString () => $"{KOID}";
	}

	/// <summary>
	/// Transaction ID and argument types for zx_channel_call.
	/// </summary>
	public struct ZxTxId {
		public uint TxId;
		public ZxTxId (uint id)
		{
			TxId = id;
		}

		public override string ToString () => $"{TxId}";
	}

	/// <summary>
	/// Clock IDs
	/// </summary>
	public enum ZxClock : uint {
		Monotonic,
		Utc,
		Thread
	}

	/// <summary>
	/// Zx object signals, the generic version, per-object versions exist as well
	/// </summary>
	[Flags]
	public enum ZxObjectSignal : uint {
		None,
		UserSignalAll = 0xff000000,
		SignalAll = 0x00ffffff,
		Readable = 1u << 0,
		Writable = 1u << 1,
		PeerClosed = 1u << 2,
		Signaled = 1u << 3,
		_Signal4 = 1u << 4,
		_Signal5 = 1u << 5,
		_Signal6 = 1u << 6,
		_Signal7 = 1u << 7,
		_Signal8 = 1u << 8,
		_Signal9 = 1u << 9,
		_Signal10 = 1u << 10,
		_Signal11 = 1u << 11,
		_Signal12 = 1u << 12,
		_Signal13 = 1u << 13,
		_Signal14 = 1u << 14,
		_Signal15 = 1u << 15,
		_Signal16 = 1u << 16,
		_Signal17 = 1u << 17,
		_Signal18 = 1u << 18,
		_Signal19 = 1u << 19,
		_Signal20 = 1u << 20,
		_Signal21 = 1u << 21,
		_Signal22 = 1u << 22,
		HandleClosed = 1u << 23,

		_UserSignal0 = 1u << 24,
		_UserSignal1 = 1u << 25,
		_UserSignal2 = 1u << 26,
		_UserSignal3 = 1u << 27,
		_UserSignal4 = 1u << 28,
		_UserSignal5 = 1u << 29,
		_UserSignal6 = 1u << 30,
		_UserSignal7 = 1u << 31,
	}

	/// <summary>
	/// Zx event signal values
	/// </summary>
	[Flags]
	public enum ZxEventSignal : uint {
		Signaled = ZxObjectSignal.Signaled,
		SignalMask = ZxObjectSignal.SignalAll | ZxObjectSignal.Signaled
	}

	/// <summary>
	/// Zx EventPair signal values.
	/// </summary>
	[Flags]
	public enum ZxEventPairSignal : uint {
		Signaled = ZxObjectSignal.Signaled,
		PeerClosed = ZxObjectSignal.PeerClosed,
		SignalMask = ZxObjectSignal.UserSignalAll | Signaled | PeerClosed
	}

	/// <summary>
	/// Zx Channel signal values.
	/// </summary>
	[Flags]
	public enum ZxChannelSignal : uint {
		Readable = ZxObjectSignal.Readable,
		Writable = ZxObjectSignal.Writable,
		PeerClosed = ZxObjectSignal.PeerClosed
	}

	/// <summary>
	/// Zx socket signal values
	/// </summary>
    	[Flags]
	public enum ZxSocketSignal : uint {
		Readable = ZxObjectSignal.Readable,
		Writable = ZxObjectSignal.Writable,
		PeerClosed = ZxObjectSignal.PeerClosed,
		PeerWriteDisabled = ZxObjectSignal._Signal4,
		WriteDisabled = ZxObjectSignal._Signal5,
		ControlReadable = ZxObjectSignal._Signal6,
		ControlWritable = ZxObjectSignal._Signal7,
		Accept = ZxObjectSignal._Signal8,
		Share = ZxObjectSignal._Signal9,
		ReadThreshold = ZxObjectSignal._Signal10,
		WriteThreshold = ZxObjectSignal._Signal11,
	}

	/// <summary>
	/// Zx fifo signal values
	/// </summary>
    	[Flags]
	public enum ZxFifoSignal : uint {
		Readable = ZxObjectSignal.Readable,
		Writable = ZxObjectSignal.Writable,
		PeerClosed = ZxObjectSignal.PeerClosed,
	}

	/// <summary>
	/// Zx task signal values
	/// </summary>
    	[Flags]
	public enum ZxTaskSignal : uint {
		Terminated = ZxObjectSignal.Signaled
	}

	/// <summary>
	/// Zx job signal values
	/// </summary>
	[Flags]
	public enum ZxJobSignal : uint {
		NoProcess = ZxObjectSignal.Signaled,
		NoJobs = ZxObjectSignal._Signal4
	}

	/// <summary>
	/// Zx process signal values
	/// </summary>
	[Flags]
	public enum ZxProcessSignal : uint {
		Terminated = ZxObjectSignal.Signaled,
	}

	/// <summary>
	/// Zx thread signal values
	/// </summary>
	[Flags]
	public enum ZxThreadSignal : uint {
		Terminated = ZxObjectSignal.Signaled,
		Running = ZxObjectSignal._Signal4,
		Suspended = ZxObjectSignal._Signal5
	}

	/// <summary>
	/// Zx log signal values
	/// </summary>
	[Flags]
	public enum ZxLogSignal : uint {
		Readable = ZxObjectSignal.Readable,
		Writable = ZxObjectSignal.Writable
	}

	/// <summary>
	/// Zx timer signal values
	/// </summary>
	[Flags]
	public enum ZxTimerSignal : uint {
		Signaled = ZxObjectSignal.Signaled
	}

	/// <summary>
	/// Zx VMO signal values
	/// </summary>
	[Flags]
	public enum ZxVmoSignal : uint {
		ZeroChildren = ZxObjectSignal.Signaled
	}
}