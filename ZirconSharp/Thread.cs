//
// Thread.cs: API bindings for the Zircon Thread class
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
	/// Runnable / computation entity
	/// </summary>
	/// <remarks>
	/// <para>The thread object is the construct that represents a time-shared CPU execution context. Thread objects 
	/// live associated to a particular Process Object which provides the memory and the handles to other objects 
	/// necessary for I/O and computation.</para>
	/// <para>
	/// Threads are created by calling Create(), but only start executing when either Start() or Process.Start() 
	/// are called. Both syscalls take as an argument the entrypoint of the initial routine to execute.
	/// </para>
	/// <para>The thread passed to zx_process_start() should be the first thread to start execution on a process.</para>
	/// <para>A thread terminates execution: </para>
	///    <list type="bullet">
	///    <item>
	///      <description>
	/// 	   By calling Thread.Exit ().
	///      </description>
	///    </item>
	///    <item>
	///      <description>
	/// 	    By calling VirtualMemoryAddressRegion.UmapHandleCloseTheadExit ()
	///      </description>
	///    </item>
	///    <item>
	///      <description>
	///         zx_futex_wake_handle_close_thread_exit()
	///      </description>
	///    </item>
	///    <item>
	///      <description>
	///        when the parent process terminates
	///      </description>
	///    </item>
	///    <item>
	///      <description>
	///        by calling zx_task_kill() with the thread's handle
	///      </description>
	///    </item>
	///    <item>
	///      <description>
	///        after generating an exception for which there is no handler or the handler decides to terminate the thread.
	///      </description>
	///    </item>
	/// </list>
	/// <para>Returning from the entrypoint routine does not terminate execution. The last action of the entrypoint 
	/// should be to call Thread.Exit() or one of the above mentioned Exit() variants.</para>
    	/// <para>Closing the last handle to a thread does not terminate execution. In order to forcefully kill a 
    	/// thread for which there is no available handle, use zx_object_get_child() to obtain a handle to 
	/// the thread. This method is strongly discouraged. Killing a thread that is executing might 
	/// leave the process in a corrupt state.
    	/// </para>
	/// <para>Fuchsia native threads are always detached. That is, there is no join() operation needed to do a clean termination. However, some runtimes above the kernel, such as C11 or POSIX might require threads to be joined.</para>
	/// </remarks>
	public class Thread : Task {
		internal Thread (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}

		[DllImport (Library)]
		extern static void zx_thread_exit ();

		/// <summary>
		/// Terminate the current running thread
		/// </summary>
		/// <remarks>
		/// <para>Exit() causes the currently running thread to cease running and exit.</para>
		/// <para>The signal ZX_THREAD_TERMINATED will be asserted on the thread object upon exit and may be observed via zx_object_wait_one() or zx_object_wait_many() on a handle to the thread.</para>
		/// </remarks>
		public void Exit ()
		{
			zx_thread_exit ();
		}

		[DllImport (Library)]
		extern static ZxStatus zx_thread_start (uint handle,
			    IntPtr thread_entry,
			    IntPtr stack,
			    UIntPtr arg1,
			    UIntPtr arg2);

		/// <summary>
		/// Start execution on a thread
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
		///      <term>ErrBadHandle</term>
		///      <description>
		///        thread is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        thread is not a thread handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        The handle thread lacks ZX_RIGHT_WRITE.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        thread is not ready to run or the process thread is part of is no longer alive.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="threadEntry">Should point to the code to execute.</param>
		/// <param name="stackStart">Initial value for the stack pointer</param>
		/// <param name="arg1">First argument to pass to the function, in the register expected by the architecture.</param>
		/// <param name="arg2">Second argument to pass to the function, in the register expected by the architecture.</param>
		/// <remarks>
		/// <para>The arg1 and arg2 values are passed on the first two registers of the ABI of the platform.  The other registers are set to zero.</para>
		/// <para>When the last handle to a thread is closed, the thread is destroyed.</para>
		/// <para>Thread handles may be waited on and will assert the signal ZX_THREAD_TERMINATED when the thread stops executing (due to zx_thread_exit() being called).</para>
		/// </remarks>
		public ZxStatus Start (IntPtr threadEntry, IntPtr stackStart, UIntPtr arg1, UIntPtr arg2)
		{
			return zx_thread_start ((uint)handle, threadEntry, stackStart, arg1, arg2);
		}

		// TODO: thread_read_state
		// TODO: thread_write_state
	}
}
