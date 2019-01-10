//
// Job.cs: API bindings for the Job type
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
	/// Controls a group of processes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// A job is a group of processes and possibly other (child) jobs. Jobs are used to track privileges 
	/// to perform kernel operations (i.e., make various syscalls, with various options), and track
	/// and limit basic resource (e.g., memory, CPU) consumption. Every process belongs to a single 
	/// job. Jobs can also be nested, and every job except the root job also belongs to a single (parent) job.
	/// </para>
	/// <para>
	/// A job is an object consisting of the following: a reference to a parent job,
	/// a set of child jobs (each of whom has this job as parent), 
	/// a set of member processes, a set of policies.
	/// </para>
	/// <para>
	/// Jobs control “applications” that are composed of more than one process to be controlled as a 
	/// single entity.
	/// </para>
	/// <para>
	/// Jobs can be created by calling the Create() method, the Default Job can be retrieved using
    	/// the Default property.
	/// </para>
	/// <para>
	/// Processes can be created by calling the CreateProcess() method on your Job instance.
	/// </para>
	/// </remarks>
	public class Job : Task {
		Job (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}

		[DllImport (Library)]
		extern static ZxStatus zx_job_create (uint parent_job, uint options, out uint result);

		/// <summary>
		/// Creates a new child job object given a parent job.
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        and a handle to the new job (via out) on success.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        parent_job is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        parent_job is not a job handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        options is nonzero, or out is an invalid pointer.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        parent_job does not have the ZX_RIGHT_WRITE or ZX_RIGHT_MANAGE_JOB right.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrOutOfRange</term>
		///      <description>
		///        The height of parent_job is too large to create a child job.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future build this error will no longer occur.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        The parent job object is in the dead state.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="parent">Parent.</param>
		/// <param name="options">Options.</param>
		/// <param name="result">Result on success.</param>
		/// <remarks>
		/// The kernel keeps track of and restricts the “height” of a job, which is its distance 
		/// from the root job. It is illegal to create a job under a parent whose height exceeds 
		/// an internal “max height” value. (It is, however, legal to create a process under such a job.)
		/// </remarks>
		public static ZxStatus Create (Job parent, uint options, out Job result)
		{
			uint res;
			var code = zx_job_create ((uint)parent.handle, options, out res);
			if (code == ZxStatus.Ok)
				result = new Job (res, ownsHandle: true);
			else
				result = null;
			return code;
		}

		[DllImport (Library)]
		extern static uint job_default ();

		/// <summary>
		/// The default Job received at startup.
		/// </summary>
		/// <value>The default.</value>
		public static Job Default => new Job (job_default (), ownsHandle: false);

		[DllImport (Library)]
		extern static ZxStatus zx_job_set_policy (uint handle, PolicyOption options, uint topic, BasicPolicy [] policy, uint count);


		public enum PolicyCondition : uint {
			/// <summary>
			/// a process under this job is attempting to issue a syscall with an invalid handle. 
	    		/// In this case, PolicyAction.Allow and PolicyAction.Deny are equivalent: if the syscall returns, 
	    		/// it will always return the error ErrBadHandle.
			/// </summary>
			BadHandle = 0,
			/// <summary>
			///  process under this job is attempting to issue a syscall with a handle that does not support such operation.
			/// </summary>
			WrongObject = 1,
			/// <summary>
			///  a process under this job is attempting to map an address region with write-execute access.
			/// </summary>
			VmarWx = 2,
			/// <summary>
			/// is a special condition that stands for the various New condtions. 
	    		///  This will include any new kernel objects which do not require a parent object for creation.
			/// </summary>
			NewAny = 3,
			/// <summary>
			/// a process under this job is attempting to create a new vm object.
			/// </summary>
			NewVmo = 4,
			/// <summary>
			/// a process under this job is attempting to create a new channel.
			/// </summary>
			NewChannel = 5,
			/// <summary>
			///  a process under this job is attempting to create a new event.
			/// </summary>
			NewEvent = 6,
			/// <summary>
			/// a process under this job is attempting to create a new event pair.
			/// </summary>
			NewEventpair = 7,
			/// <summary>
			///  a process under this job is attempting to create a new port.
			/// </summary>
			NewPort = 8,
			/// <summary>
			/// a process under this job is attempting to create a new socket.
			/// </summary>
			NewSocket = 9,
			/// <summary>
			/// a process under this job is attempting to create a new fifo.
			/// </summary>
			NewFifo = 10,
			/// <summary>
			/// a process under this job is attempting to create a new timer.
			/// </summary>
			NewTimer = 11,
			/// <summary>
			/// a process under this job is attempting to create a new process.
			/// </summary>
			NewProcess = 12,
		}

		/// <summary>
		/// Policy action, can be one of Allow or Deny, and can be augmented by OR-ing Exception or Kill.
		/// </summary>
		[Flags]
		public enum PolicyAction : uint {
			/// <summary>
			/// Allow condition.
			/// </summary>
			Allow = 0,
		    	/// <summary>
		    	/// Deny condition
		    	/// </summary>
		    	Deny = 1,
			/// <summary>
			/// generate an exception via the debug port. An exception generated this way acts as a breakpoint. The thread may be resumed after the exception.
			/// </summary>
			Exception = 2,
			/// <summary>
			/// terminate the process. It also implies Deny
			/// </summary>
			Kill = 5
		}

		/// <summary>
		/// Policy is applied for the conditions that are not
		/// specified by the parent job policy.
		/// </summary>
		public enum PolicyOption : uint {
			/// <summary>
			/// policy is applied for the conditions not specifically overridden by the parent policy.
			/// </summary>
			Relative = 0,
			/// <summary>
			/// policy is applied for all conditions in policy or the syscall fails.
			/// </summary>
			Absolute = 1,
		}

		/// <summary>
		/// Basic Policy structure (zx_policy_basic)
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		public struct BasicPolicy {
			public PolicyCondition Condition;
			public PolicyAction Policy;

			public BasicPolicy (PolicyCondition condition, PolicyAction policyAction)
			{
				this.Condition = condition;
				this.Policy = policyAction;
			}
		}

		const int ZX_JOB_POL_BASIC = 0;

		/// <summary>
		/// Sets one or more security and/or resource policies to an empty job. 
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        policy was not a valid pointer, or count was 0, or policy was not ZX_JOB_POL_RELATIVE or ZX_JOB_POL_ABSOLUTE, or topic was not ZX_JOB_POL_BASIC.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle is not valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle is not a job handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have ZX_POL_RIGHT_SET right.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        the job has existing jobs or processes alive.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrOutOfRange</term>
		///      <description>
		///        count is bigger than ZX_POL_MAX or condition is bigger than ZX_POL_MAX.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAlreadyExists</term>
		///      <description>
		///        existing policy conflicts with the new policy.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNotSupported</term>
		///      <description>
		///        an entry in policy has an invalid value.
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
		/// <param name="options">Options.</param>
		/// <param name="policy">Policy.</param>
		/// <remarks>
		/// <para>The job‘s effective policies is the combination of the parent’s effective policies and the policies specified in policy. 
		/// The effect in the case of conflict between the existing policies and the new policies is controlled by options values
		/// </para>
		/// <para>
		/// After this call succeeds any new child process or child job will have the new effective policy applied to it.
		/// </para>
		/// </remarks>
		public ZxStatus job_set_policy (PolicyOption options, BasicPolicy [] policy)
		{
			if (policy == null)
				throw new ArgumentNullException (nameof (policy));

			return zx_job_set_policy ((uint) handle, options, ZX_JOB_POL_BASIC, policy, (uint) policy.Length);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_process_create (uint job, string name, IntPtr name_size, uint options, out uint proc_handle, out uint vmar_handle);

		/// <summary>
		/// Creates a new process.  Upon success, handles for the new process and the root of its address space are returned.   To start you must call Start().
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        On success, a handle to the new process (via proc_handle), and a handle to the root of its address space (via vmar_handle).
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        job is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        job is not a job handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        job does not have the ZX_RIGHT_WRITE right (only when not ZX_HANDLE_INVALID).
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        name, proc_handle, or vmar_handle was an invalid pointer, or options was non-zero.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future build this error will no longer occur.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        The job object is in the dead state.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="name">Name for the new process.</param>
		/// <param name="process">On success, this contains the created Process, null on error.</param>
		/// <param name="vmar">On success, this contains the handle to the root address space, null on error..</param>
		/// <remarks>
		/// When the last handle to a process is closed, the process is destroyed.
		///
		/// Process handles may be waited on and will assert the signal ZX_PROCESS_TERMINATED when the process exits.
		/// job is the controlling job object for the new process, which will become a child of that job.
		/// </remarks>
		public ZxStatus CreateProcess (string name, out Process process, out VirtualMemoryAddressRegion vmar)
		{
			if (name == null)
				throw new ArgumentNullException (nameof (name));
			uint processHandle, vmarHandle;
			var ret = zx_process_create ((uint)handle, name, (IntPtr)System.Text.Encoding.UTF8.GetBytes (name).Length, 0, out processHandle, out vmarHandle);
			if (ret != ZxStatus.Ok) {
				process = null;
				vmar = null;
				return ret;
			}
			process = new Process (processHandle, ownsHandle: true);
			vmar = new VirtualMemoryAddressRegion (vmarHandle, (IntPtr)(-1));
			return ZxStatus.Ok;
		}
	}
}
