//
// Process.cs: API bindings for the Zircon Process class
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
	/// A zircon process is an instance of a program in the traditional sense: a set of instructions which 
	/// will be executed by one or more threads, along with a collection of resources.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The process object is a container of Handles, VirtualMemoryAddressRegions and Threads.   In general, 
	/// it is associated with code which it is executing until it is forcefully terminated or the program exits.
	/// </para>
	/// <para>
	/// Processes are owned by jobs and allow an application that is composed by more than one process to be 
	/// treated as a single entity, from the perspective of resource and permission limits, as well as lifetime control.
	/// </para>
	/// <para>
	/// <para>
	/// A process is created via a call to Create() and its execution begins with Start();
	/// The process stops execution when the last thread is terminated or exits, the process calls zx_process_exit(),
	/// the parent job terminates the process, or the parent job is destroyed.
	/// </para>
	/// <para>
    	/// The call to Start() cannot be issued twice. New threads cannot be added to a process that was started 
    	/// and then its last thread has exited.
    	/// </para>
    	/// </remarks>
	public class Process : Task {
		internal Process (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}

		[DllImport (Library)]
		extern static ZxStatus zx_process_read_memory (uint handle, IntPtr vaddr, IntPtr buffer, ulong buffer_size, out ulong actual);

		/// <summary>
		/// Read from the given process's address space.
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
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have the ZX_RIGHT_READ right or ZX_WRITE_RIGHT is needed for historical reasons.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        the process's memory is not accessible (e.g., the process is being terminated), or the requested memory is not cacheable.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        buffer is an invalid pointer or NULL, or buffer_size is zero or greater than 64MB.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        the process does not have any memory at the requested address.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not a process handle.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="vaddr">the address of the block of memory to read..</param>
		/// <param name="destination">pointer to a user buffer to read bytes into..</param>
		/// <param name="size">number of bytes to attempt to read.  The buffer must be large enough for at least this many bytes. The size must be greater than zero and less than or equal to 64MB.</param>
		/// <param name="actual">the actual number of bytes read is stored here. Less bytes than requested may be returned if vaddr+buffer_size extends beyond the memory mapped in the process.</param>
		public ZxStatus ReadMemory (IntPtr vaddr, IntPtr destination, ulong size, out ulong actual)
		{
			return zx_process_read_memory ((uint)handle, vaddr, destination, size, out actual);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_process_start (uint handle, uint thread, IntPtr entry, IntPtr stack, uint arg1, UIntPtr arg2);

		/// <summary>
		/// Start execution on a process
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
		///        process or thread or arg1 is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        process is not a process handle or thread is not a thread handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        The handle thread lacks ZX_RIGHT_WRITE or thread does not belong to process, or the handle process lacks ZX_RIGHT_WRITE or arg1 lacks ZX_RIGHT_TRANSFER.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        process is already running or has exited.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="thread">Thread.</param>
		/// <param name="startAddress">Start address.</param>
		/// <param name="startStack">Start stack.</param>
		/// <param name="firstArg">This handle will be transferred from the current process to the process being started, and an appropriate handle value will be placed for the newly started thread.   If this method returns an error, the handle is closed rather than being transferred to the process being started.</param>
		/// <param name="secondArg">Second argument passed to the startup method.</param>
		/// <remarks>
		/// <para>Start s similar to ZirconSharp.Thread.Start(), but is used for the purpose of starting the first thread in a process.</para>
		/// <para>Start causes a thread to begin execution at the program counter specified by entry and with the stack pointer set to stack. 
		/// The arguments firstArg (the handle for it) and secondArg are arranged to be in the architecture specific registers used for the 
		/// first two arguments of a function call before the thread is started. All other registers are zero upon start.</para>
		/// <para>The first argument (arg1) is a handle, which will be transferred from the process of the caller to the process which is being started, and an appropriate handle value will be placed in arg1 for the newly started thread. If zx_process_start() returns an error, arg1 is closed rather than transferred to the process being started.</para>
		/// </remarks>
		public ZxStatus Start (Thread thread, IntPtr startAddress, IntPtr startStack, ZirconObject firstArg, UIntPtr secondArg)
		{
			if (thread == null)
				throw new ArgumentNullException (nameof (thread));
			if (firstArg == null)
				throw new ArgumentNullException (nameof (firstArg));
			return zx_process_start ((uint)handle, (uint)thread.DangerousGetHandle (), startAddress, startStack, (uint) firstArg.DangerousGetHandle (), secondArg);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_process_write_memory (uint handle, IntPtr vaddr, IntPtr buffer, IntPtr buffer_size, out IntPtr actual);

		/// <summary>
		/// Write into the given process's address space.
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
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have the ZX_RIGHT_READ right or ZX_WRITE_RIGHT is needed for historical reasons.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        the process's memory is not accessible (e.g., the process is being terminated), or the requested memory is not cacheable.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        buffer is an invalid pointer or NULL, or buffer_size is zero or greater than 64MB.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        the process does not have any memory at the requested address.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not a process handle.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="destination">the address of the block of memory to write in the target process.</param>
		/// <param name="source">Pointer to the buffer in our address space that will be copied into the target.</param>
		/// <param name="size">Size that will be copied.</param>
		/// <param name="actual">the actual number of bytes written is stored here. Less bytes than requested may be returned if vaddr+buffer_size extends beyond the memory mapped in the process.</param>
		public ZxStatus WriteMemory (IntPtr destination, IntPtr source, ulong size, out ulong actual)
		{
			return zx_process_read_memory ((uint)handle, destination, source, size, out actual);
		}

		[DllImport (Library)]
		extern static void zx_process_exit (ulong retcode);

		/// <summary>
		///  Exits the currently running process.
		/// </summary>
		/// <param name="code">Status code to return.  The exit status can be probed using the Object APIs.</param>
		public void Exit (ulong code)
		{
			zx_process_exit (code);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_thread_create (uint process, string name, IntPtr name_size, uint options, out uint result);

		/// <summary>
		/// Creates a new thread.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success, and the Thread object is set to refer to the newly created thread
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        process is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        process is not a process handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        process does not have the ZX_RIGHT_MANAGE_THREAD right.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        name or out was an invalid pointer, or options was non-zero.
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
		/// <param name="name">Name for the thread.</param>
		/// <param name="thread">On success, this contains the newly created Thread.</param>
		/// <remarks>
		/// <para>CreateThread creates a thread within the specified process.</para>
		/// <para>
		/// Upon success the thread outgoing parameters is not null. The thread will not start executing until Thread.Start() is called.
		/// </para>
		/// <para>Thread handles may be waited on and will assert the signal ZX_THREAD_TERMINATED when the thread stops executing (due to Thread.Exit() being called).</para>
		/// <para>process is the controlling process object for the new thread, which will become a child of that process.</para>
		/// </remarks>
		public ZxStatus CreateThread (string name, out Thread thread)
		{
			if (name == null)
				throw new ArgumentNullException (nameof (name));
			uint hresult;
			var ret = zx_thread_create ((uint)handle, name, (IntPtr)System.Text.Encoding.UTF8.GetBytes (name).Length, 0, out hresult);
			if (ret == ZxStatus.Ok)
				thread = new Thread (hresult, ownsHandle: true);
			else
				thread = null;
			return ret;
		}

	}
}
