﻿using System;
using System.Runtime.InteropServices;

namespace ZirconSharp {
	/// <summary>
	/// Bidirectional interprocess communication via messages consisting of some amount of byte data and some number of handles.
	/// </summary>
	/// <remarks>
	/// <para>
	///   Channels maintain an ordered queue of messages to be delivered in
	///   either direction. A message consists of some amount of data and some
	///   number of handles. A call to Write() enqueues one message,
	///   and a call to Read() dequeues one message (if any are
	///   queued). A thread can block until messages are pending via
	///   WaitOne() or other waiting mechanisms.
	/// </para>
	/// 
	/// <para>
	///   Alternatively, a call to Call() enqueues a message in one
	///   direction of the channel, waits for a corresponding response, and
	///   dequeues the response message. In call mode, corresponding responses
	///   are identified via the first 4 bytes of the message, called the
	///   transaction ID. The kernel supplies distinct transaction IDs (always
	///   with the high bit set) for messages written with Call().
	/// </para>
	/// 
	/// <para>
	///   The process of sending a message via a channel has two steps. The
	///   first is to atomically write the data into the channel and move
	///   ownership of all handles in the message into this channel. This
	///   operation always consumes the handles: at the end of the call, all
	///   handles either are all in the channel or are all discarded. The second
	///   operation is similar: after a channel read, all the handles in the
	///   next message to read are either atomically moved into the process's
	///   handle table, all remain in the channel, or are discarded (only when
	///   the ZxChannelRead.MayDiscard option is given).
	/// </para>
	/// 
	/// <para>
	///   Unlike many other kernel object types, channels are not
	///   duplicatable. Thus there is only ever one handle associated to a
	///   handle endpoint and the process holding that handle is considered the
	///   owner. Only the owner can write messages or send the channel endpoint
	///   to another process.
	/// </para>
	/// 
	/// <para>
	///   Furthermore, when ownership of a channel endpoint goes from one
	///   process to another, even if a write was in progress, the ordering of
	///   messages is guaranteed to be parsimonious; packets before the transfer
	///   event originate from the previous owner and packets after the transfer
	///   belong to the new owner. The same applies if a read was in progress
	///   when the endpoint was transferred.
	/// </para>
	/// 
	/// <para>
	///   The above sequential guarantee is not provided for other kernel
	///   objects, even if the last remaining handle is stripped of the
	///   ZX_RIGHT_DUPLICATE right.
	/// </para>
	/// </remarks>
	public class Channel : ZirconObject {
		public Channel ()
		{
		}
	}
}
