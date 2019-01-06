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
using System.Runtime.InteropServices;

namespace ZirconSharp {
	/// <summary>
	///   Zircon statuses are signed 32 bit integers. The space of values is
	///   divided as follows:  Zero value indicates an OK status; negative values are defined by the system, 
    	///   in this enumeration; positive values are reserved for protocol-specific error values,
	///   and will never be defined by the system.
	/// </summary>
	public enum ZxStatus {
		Ok = 0,

		/// <summary>
		/// The system encountered an otherwise unspecified error while performing the operation.
		/// </summary>
		ErrInternal = -1,

		/// <summary>
		/// The operation is not implemented, supported, or enabled.
		/// </summary>
		ErrNotSupported = -2,

		/// <summary>The system was not able to allocate some resource needed for the
		/// operation.</summary>
		ErrNoResources = (-3),

		/// <summary>The system was not able to allocate memory needed for the operation.</summary>
		ErrNoMemory = (-4),

		/// <summary>The system call was interrupted, but should be retried.  This should not be
		/// seen outside of the VDSO.</summary>
		ErrInternalIntrRetry = (-6),

		/// <summary>an argument is invalid, ex. null pointer</summary>
		ErrInvalidArgs = (-10),

		/// <summary>A specified handle value does not refer to a handle.</summary>
		ErrBadHandle = (-11),

		/// <summary>The subject of the operation is the wrong type to perform the
		/// operation. Example: Attempting a message_read on a thread handle.</summary>
		ErrWrongType = (-12),

		/// <summary>The specified syscall number is invalid.</summary>
		ErrBadSyscall = (-13),

		/// <summary>An argument is outside the valid range for this operation.</summary>
		ErrOutOfRange = (-14),

		/// <summary>A caller provided buffer is too small for this operation.</summary>
		ErrBufferTooSmall = (-15),

		/// <summary>operation failed because the current state of the object does not allow it, or
		/// a precondition of the operation is not satisfied</summary>
		ErrBadState = (-20),

		/// <summary>The time limit for the operation elapsed before the operation
		/// completed.</summary>
		ErrTimedOut = (-21),

		/// <summary>The operation cannot be performed currently but potentially could succeed if
		/// the caller waits for a prerequisite to be satisfied, for example waiting for a handle to
		/// be readable or writable. Example: Attempting to read from a channel that has no messages
		/// waiting but has an open remote will return ZX_ERR_SHOULD_WAIT. Attempting to read from a
		/// channel that has no messages waiting and has a closed remote end will return
		/// ZX_ERR_PEER_CLOSED.</summary>

		ErrShouldWait = (-22),

		/// <summary>The in-progress operation (e.g. a wait) has been canceled.</summary>
		ErrCanceled = (-23),

		/// <summary>The operation failed because the remote end of the subject of the operation was
		/// closed.</summary>
		ErrPeerClosed = (-24),

		/// <summary>The requested entity is not found.</summary>
		ErrNotFound = (-25),

		/// <summary>An object with the specified identifier already exists. Example: Attempting to
		/// create a file when a file already exists with that name.</summary>
		ErrAlreadyExists = (-26),

		/// <summary>The operation failed because the named entity is already owned or controlled by
		/// another entity. The operation could succeed later if the current owner releases the
		/// entity.
		ErrAlreadyBound = (-27),

		/// <summary>The subject of the operation is currently unable to perform the
		/// operation. Note: This is used when there's no direct way for the caller to observe when
		/// the subject will be able to perform the operation and should thus retry.</summary>
		ErrUnavailable = (-28),


		/// <summary>The caller did not have permission to perform the specified
		/// operation.</summary>
		ErrAccessDenied = (-30),

		/// <summary>Otherwise unspecified error occurred during I/O.</summary>
		ErrIo = (-40),

		/// <summary>The entity the I/O operation is being performed on rejected the
		/// operation. Example: an I2C device NAK'ing a transaction or a disk controller rejecting
		/// an invalid command, or a stalled USB endpoint.</summary>
		ErrIoRefused = (-41),

		/// <summary>The data in the operation failed an integrity check and is possibly
		/// corrupted. Example: CRC or Parity error.</summary>
		ErrIoDataIntegrity = (-42),

		/// <summary>The data in the operation is currently unavailable and may be permanently
		/// lost. Example: A disk block is irrecoverably damaged.</summary>
		ErrIoDataLoss = (-43),

		/// <summary>The device is no longer available (has been unplugged from the system, powered
		/// down, or the driver has been unloaded)</summary>
		ErrIoNotPresent = (-44),

		/// <summary>More data was received from the device than expected. Example: a USB "babble"
		/// error due to a device sending more data than the host queued to receive.</summary>
		ErrIoOverrun = (-45),

		/// <summary>An operation did not complete within the required timeframe. Example: A USB
		/// isochronous transfer that failed to complete due to an overrun or underrun.</summary>
		ErrIoMissedDeadline = (-46),

		/// <summary>The data in the operation is invalid parameter or is out of range. Example: A
		/// USB transfer that failed to complete with TRB Error</summary>
		ErrIoInvalid = (-47),

		/// <summary>Path name is too long.</summary>
		ErrBadPath = (-50),

		/// <summary>Object is not a directory or does not support directory operations. Example:
		/// Attempted to open a file as a directory or attempted to do directory operations on a
		/// file.</summary>
		ErrNotDir = (-51),

		/// <summary>Object is not a regular file.</summary>
		ErrNotFile = (-52),

		/// <summary>This operation would cause a file to exceed a filesystem-specific size
		/// limit</summary>
		ErrFileBig = (-53),

		/// <summary>Filesystem or device space is exhausted.</summary>
		ErrNoSpace = (-54),

		/// <summary>Directory is not empty.</summary>
		ErrNotEmpty = (-55),

		/// <summary>Do not call again. Example: A notification callback will be called on every
		/// event until it returns something other than ZX_OK. This status allows differentiation
		/// between "stop due to an error" and "stop because the work is done."</summary>
		ErrStop = (-60),

		/// <summary>Advance to the next item. Example: A notification callback will use this
		/// response to indicate it did not "consume" an item passed to it, but by choice, not due
		/// to an error condition.</summary>
		ErrNext = (-61),

		/// <summary>Ownership of the item has moved to an asynchronous worker.</summary>
		/// <remarks>
		/// Unlike ZX_ERR_STOP, which implies that iteration on an object
		/// should stop, and ZX_ERR_NEXT, which implies that iteration
		/// should continue to the next item, ZX_ERR_ASYNC implies
		/// that an asynchronous worker is responsible for continuing iteration.
		///
		/// Example: A notification callback will be called on every
		/// event, but one event needs to handle some work asynchronously
		/// before it can continue. ZX_ERR_ASYNC implies the worker is
		/// responsible for resuming iteration once its work has completed.
		/// </remarks>
		ErrAsync = (-62),

		/// <summary>Specified protocol is not supported.</summary>
		ErrProtocolNotSupported = (-70),

		/// <summary>Host is unreachable.</summary>
		ErrAddressUnreachable = (-71),

		/// <summary>Address is being used by someone else.</summary>
		ErrAddressInUse = (-72),

		/// <summary>Socket is not connected.</summary>
		ErrNotConnected = (-73),

		/// <summary>Remote peer rejected the connection.</summary>
		ErrConnectionRefused = (-74),

		/// <summary>Connection was reset.</summary>
		ErrConnectionReset = (-75),

		/// <summary>Connection was aborted.</summary>
		ErrConnectionAborted = (-76),

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

	[StructLayout (LayoutKind.Sequential)]
	internal unsafe struct zx_channel_call_args {
		internal void *wr_bytes;
		internal uint *wr_handles;
		internal void *rd_bytes;
		internal uint *rd_handles;
		internal uint wr_num_bytes;
		internal uint wr_num_handles;
		internal uint rd_num_bytes;
		internal uint rd_num_handles;
	}

	[StructLayout (LayoutKind.Sequential)]
    	internal struct zx_wait_item {
		uint handle;
		ZxObjectSignal waitfor;
		ZxObjectSignal pending;
	}

	/// <summary>
	/// Options for mapping Virtual Memory Objects into the address space of the process
	/// </summary>
	[Flags]
	public enum ZxVmOption : uint {
		/// <summary>
		/// Map vmo as readable. It is an error if handle does not have VmCanMapRead permissions, the handle 
		/// does not have the ZX_RIGHT_READ right, or the vmo handle does not have the ZX_RIGHT_READ right.
		/// </summary>
		PermRead = 1u << 0,
		/// <summary>
		/// Map vmo as writable. It is an error if handle does not have VmCanMapWrite permissions, the handle 
		/// does not have the ZX_RIGHT_WRITE right, or the vmo handle does not have the ZX_RIGHT_WRITE right.
		/// </summary>
		PermWrite = 1u << 1,
		/// <summary>
		/// Map vmo as executable. It is an error if handle does not have VmCanMapExecute permissions, 
		/// the handle handle does not have the ZX_RIGHT_EXECUTE right, or the vmo handle does not have the 
		/// ZX_RIGHT_EXECUTE right.
		/// </summary>
		PermExecute = 1u << 2,
		VmCompact = 1u << 3,
		/// <summary>
		/// Use the vmar_offset to place the mapping, invalid if handle does not have the VmCanMapSpecific permission. 
		/// vmar_offset is an offset relative to the base address of the given VMAR. It is an error to specify a range 
		/// that overlaps with another VMAR or mapping.
		/// </summary>
		VmSpecific = 1u << 4,
		/// <summary>
		/// Same as VmSpecific, but can overlap another mapping. It is still an error to partially-overlap another VMAR. 
		/// If the range meets these requirements, it will atomically (with respect to all other map/unmap/protect 
		/// operations) replace existing mappings in the area.
		/// </summary>
		VmSpecificOverwrite = 1u << 5,
		/// <summary>
		/// The new VMAR can have subregions/mappings created with VmSpecific. It is NOT an error if the parent 
		/// does not have VmCanMapSpecific permissions.
		/// </summary>
		VmCanMapSpecific = 1u << 6,
		/// <summary>
		/// The new VMAR can contain readable mappings. It is an error if the parent does not have 
		/// VmCanMapRead permissions.
		/// </summary>
		VmCanMapRead = 1u << 7,
		/// <summary>
		/// The new VMAR can contain writable mappings. It is an error if the parent does not have 
		/// VmCanMapWrite permissions.
		/// </summary>
		VmCanMapWrite = 1u << 8,
		/// <summary>
		///  The new VMAR can contain executable mappings. It is an error if the parent does not have 
		/// VmCanMapExecute permissions.
		/// </summary>
		VmCanMapExecute = 1u << 9,
		/// <summary>
		/// Immediately page into the new mapping all backed regions of the VMO. This cannot be specified if VmSpecificOverwrite is used.
		/// </summary>
		VmMapRange = 1u << 10,
		/// <summary>
		/// Maps the VMO only if the VMO is non-resizable, that is, it was created with the VmoOptions.NonResizable option.
		/// </summary>
		VmRequireNonResizable = 1u << 11,
	}
}