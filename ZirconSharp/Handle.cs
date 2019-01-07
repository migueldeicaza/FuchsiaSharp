using System;
using System.Runtime.InteropServices;

namespace ZirconSharp {
	/// <summary>
	/// Zircon handle reference kernel objects.
	/// </summary>
	/// <remarks>
	/// <para>
	///   Handles are kernel constructs that allows user-mode programs to
	///   reference a kernel object. A handle can be thought as a session or
	///   connection to a particular kernel object.   They are created by calling
	///   the constructor or factory methods in one of the various subclasses of 
	///   ZirconHandle.
	/// </para>
	/// 
	/// <para>
	///   It is often the case that multiple processes concurrently access the
	///   same object via different handles. However, a single handle can only
	///   be either bound to a single process or be bound to kernel.
	/// </para>
	/// 
	/// <para>
	///   When it is bound to kernel we say it's ‘in-transit’.
	/// </para>
	/// 
	/// <para>
	///   In user-mode a handle is simply a specific number returned by some
	///   syscall. Only handles that are not in-transit are visible to
	///   user-mode.
	/// </para>
	/// 
	/// <para>
	///   The integer that represents a handle is only meaningful for that
	///   process. The same number in another process might not map to any
	///   handle or it might map to a handle pointing to a completely different
	///   kernel object.
	/// </para>
	/// 
	/// <para>
	///   The integer value for a handle is any 32-bit number except the value
	///   corresponding to ZX_HANDLE_INVALID.
	/// </para>
	/// 
	/// <para> For kernel-mode, a handle is a C++ object that contains three
	///   logical fields: A reference to a kernel object, the rights to the
	///   kernel object or the process it is bound to (or if it's bound to
	///   kernel)
	/// </para>
	/// 
	/// <para>
	///   The ‘rights’ specify what operations on the kernel object are
	///   allowed. It is possible for a single process to have two different
	///   handles to the same kernel object with different rights.
	/// </para>
    /// <para>
    /// </para>
	/// </remarks>
	public class ZirconHandle : SafeHandle {

		// Name of the zircon dynamic library to call, available to all ZirconSharp types
		internal const string Library = "zircon";

		public ZirconHandle (uint preexistingHandle, bool ownsHandle) : base (IntPtr.Zero, ownsHandle)
		{
			SetHandle ((IntPtr) preexistingHandle);
		}

		public override bool IsInvalid => handle == IntPtr.Zero;

		// This is just for marshalling
		internal ZirconHandle () : base (IntPtr.Zero, true)
		{
		}

		[DllImport (Library)]
		extern static void zx_handle_close (uint handle);


		protected override bool ReleaseHandle ()
		{
			zx_handle_close ((uint) handle);
			return true;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				ReleaseHandle ();
			}
		}

		// Used by subclasses to duplicate a handle
		[DllImport (Library)]
		internal extern static ZxStatus zx_handle_duplicate (uint handle, ZxRights rights, out uint handleResult);

		[DllImport (Library)]
		internal extern static ZxStatus zx_handle_replace (uint handle,
			      ZxRights rights,
			      out uint handleResult);
	}
}
