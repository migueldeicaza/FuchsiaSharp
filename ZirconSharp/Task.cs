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
	public enum ZxExceptionOptions : uint {
		None,
		/// <summary>
		/// When binding an exception port to a process, set the process's debugger exception port.
		/// </summary>
		PortDebugger = 1,
	}

	/// <summary>
	/// “Runnable” subclass of kernel objects (threads, processes, and jobs)
	/// </summary>
	public class Task : ZirconObject {

		internal Task (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}

		[DllImport (Library)]
		extern static ZxStatus zx_task_kill (uint handle);

		/// <summary>
		/// Kill the provided task (job, process, or thread).
		/// </summary>
		/// <returns></returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success.  If a process or thread uses this syscall to kill itself, this syscall does not return
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not a task handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have the ZX_RIGHT_DESTROY right.
		///      </description>
		///    </item>
		/// </list>
		/// <remarks>
		/// <para>This asynchronously kills the given process, thread or job and its children recursively, until the entire task tree rooted at handle is dead.</para>
		/// <para>It is possible to wait for the task to be dead via the ZX_TASK_TERMINATED signal. When the procedure completes, as observed by the signal, the task and all its children are considered to be in the dead state and most operations will no longer succeed.</para>
		/// <para>If handle is a job and the syscall is successful, the job cannot longer be used to create new processes.</para>
		/// </remarks>
		public ZxStatus Kill ()
		{
			return zx_task_kill ((uint)handle);
		}

		[DllImport (Library)]
		extern static ZxStatus task_bind_exception_port (uint handle, uint port, ulong key, ZxExceptionOptions options);

		/// <summary>
		/// Binds the exception port corresponding to a given job, process, or thread.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAlreadyBound</term>
		///      <description>
		///        handle already has its exception port bound.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not a valid handle, or port is not a valid handle. Note that when unbinding from an exception port port is ZX_HANDLE_INVALID.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        Unbinding a port that is not currently bound.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not that of a job, process, or thread, and is not ZX_HANDLE_INVALID, or port is not that of a port and is not ZX_HANDLE_INVALID.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        A bad value has been passed in options.
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
		/// <param name="port">an IO port created by Port.Create(). The same IO port can be bound to multiple objects.</param>
		/// <param name="key">is passed back in exception reports, and is part of the port message protocol.</param>
		/// <param name="options">Options.</param>
		/// <remarks>When a port is bound to the exception port of an object it participates in exception processing. See below for how exceptions are processed.</remarks>
		public ZxStatus BindExceptionPort (Port port, ulong key, ZxExceptionOptions options = ZxExceptionOptions.None)
		{
			return task_bind_exception_port ((uint)handle, (uint)port.DangerousGetHandle (), key, options);
		}

		/// <summary>
		/// Unbind from, the exception port corresponding to a given job, process, or thread.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAlreadyBound</term>
		///      <description>
		///        handle already has its exception port bound.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not a valid handle, or port is not a valid handle. Note that when unbinding from an exception port port is ZX_HANDLE_INVALID.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        Unbinding a port that is not currently bound.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not that of a job, process, or thread, and is not ZX_HANDLE_INVALID, or port is not that of a port and is not ZX_HANDLE_INVALID.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        A bad value has been passed in options.
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
		public ZxStatus UnbindExceptionPort ()
		{
			return task_bind_exception_port ((uint)handle, (uint)0, 0, 0);
		}
	}
}
