//
// ZxRights.cs: Rights definitions
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//
using System;
namespace ZirconSharp {
	/// <summary>
	/// Rights are associated with handles and convey privileges to perform actions on either the associated handle or the object associated with the handle.
    	/// </summary>
	[Flags]
	public enum ZxRights : uint {
		None = 0,
		/// <summary>
		/// Allows handle duplication via Duplicate
		/// </summary>
		Duplicate        = (1u << 0),
		/// <summary>
		/// Allows handle transfer via Channel.Write
		/// </summary>
		Transfer         = (1u << 1),
		/// <summary>
		/// Allows reading of data from containers (channels, sockets, VM objects, etc), allows mapping as readable if Map is also present.
		/// </summary>
		Read             = (1u << 2),
		/// <summary>
		/// Allows writing of data to containers (channels, sockets, VM objects, etc), Allows mapping as writeable if Map is also present
		/// </summary>
		Write = (1u << 3),
		/// <summary>
		/// Allows mapping as executable if Map is also present
		/// </summary>
		Execute = (1u << 4),
		/// <summary>
		/// Allows mapping of a VM object into an address space.
		/// </summary>
		Map = (1u << 5),
		/// <summary>
		/// Allows property inspection via zx_object_get_property
		/// </summary>
		GetProperty = (1u << 6),
		/// <summary>
		/// Allows property modification via zx_object_set_property
		/// </summary>
		SetProperty = (1u << 7),
		/// <summary>
		/// Allows enumerating child objects via zx_object_get_info and zx_object_get_child
		/// </summary>
		Enumerate = (1u << 8),
		/// <summary>
		/// Allows termination of task objects via zx_task_kill
		/// </summary>
		Destroy = (1u << 9),
		/// <summary>
		/// Allows policy modification via zx_job_set_policy
		/// </summary>
		SetPolicy = (1u << 10),
		/// <summary>
		/// Allows policy inspection via zx_job_get_policy
		/// </summary>
		GetPolicy = (1u << 11),
		/// <summary>
		/// Allows use of zx_object_signal
		/// </summary>
		Signal = (1u << 12),
		/// <summary>
		/// Allows use of zx_object_signal_peer
		/// </summary>
		SignalPeer = (1u << 13),
		/// <summary>
		/// Allows use of zx_object_wait_one, zx_object_wait_many, and other waiting primitives
		/// </summary>
		Wait = (1u << 14),
		/// <summary>
		///  Allows inspection via zx_object_get_info
		/// </summary>
		Inspect = (1u << 15),

		ManageJob        = (1u << 16),
		ManageProcess    = (1u << 17),
		ManageThread     = (1u << 18),
		ApplyProfile     = (1u << 19),
		SameRights       = (1u << 31),

	}
}
