//
// Objet.cs: ZirconObject is the base class for the various handle-based objects in the Zircon kernel.
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
	public class ZirconObject : SafeHandle {
		// zx_obj_props_t
		/// <summary>
		/// Zx handle properties.
		/// </summary>
		public enum ZxObjProps : uint {
			None,
		    	Waitable = 1
		}

		/// <summary>
		/// Basic information about a handle (zx_info_handle_basic_t)
		/// </summary>
		[StructLayout (LayoutKind.Sequential)]
		public struct BasicInfoHandle {
			/// <summary>
			/// The unique id assigned by kernel to the object referenced by the
			/// handle.
			/// </summary>
			public ZxKernelObjectId Koid;

			/// <summary>
			/// The immutable rights assigned to the handle. Two handles that
			/// have the same koid and the same rights are equivalent and
			/// interchangeable.
			/// </summary>		
			public ZxRights Rights;

			// 
			/// <summary>
			/// The object type: channel, event, socket, etc.
			/// </summary>
			public ZxObjectType Type;                // zx_obj_type_t;

			/// <summary>
			/// 
			/// If the object referenced by the handle is related to another (such
			/// as the other end of a channel, or the parent of a job) then
			/// |related_koid| is the koid of that object, otherwise it is zero.
			/// This relationship is immutable: an object's |related_koid| does
			/// not change even if the related object no longer exists.
			/// </summary>
			public ZxKernelObjectId RelatedKoid;

			/// <summary>
			/// Set to Waitable if the object referenced by the handle can be waited on; zero otherwise
			/// </summary>
			public ZxObjProps Props;               // zx_obj_props_t;
		}

		internal enum ZxObjectInfoTopic : uint {
			None = 0,
			HandleValid = 1,
			HandleBasic = 2,
			Process = 3,
			ProcessThreads = 4,
			Vmar = 7,
			JobChildren = 8,
			JobProcesses = 9,
			Thread = 10,
			ThreadExceptionReport = 11,
			TaskStats = 12,
			ProcessMaps = 13,
			ProcessVmos = 14,
			ThreadStats = 15,
			CpuStats = 16,
			KmemStats = 17,
			Resource = 18,
			HandleCount = 19,
			Bti = 20,
			ProcessHandleStats = 21,
			Socket = 22,
			Vmo = 23,
		}

		// Name of the zircon dynamic library to call, available to all ZirconSharp types
		internal const string Library = "zircon";

		public ZirconObject (uint preexistingHandle, bool ownsHandle) : base (IntPtr.Zero, ownsHandle)
		{
			SetHandle ((IntPtr) preexistingHandle);
		}

		public override bool IsInvalid => handle == IntPtr.Zero;

		// This is just for marshalling
		internal ZirconObject () : base (IntPtr.Zero, true)
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

		[DllImport (Library)]
		internal extern static ZxStatus zx_object_get_info (uint handle, ZxObjectInfoTopic topic, IntPtr buffer, IntPtr buffer_size, out IntPtr actual, out IntPtr avail);

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:ZirconSharp.ZirconObject"/> is valid.
		/// </summary>
		/// <value><c>true</c> if is valid; otherwise, <c>false</c>.</value>
		public bool IsValid => zx_object_get_info ((uint)handle, ZxObjectInfoTopic.HandleValid, IntPtr.Zero, IntPtr.Zero, out var ignore1, out var ignore2) == ZxStatus.Ok;

		/// <summary>
		/// Gets the Basic handle information for the object.
		/// </summary>
		/// <value>The handle information.</value>
		public BasicInfoHandle HandleInformation {
			get {
				unsafe {
					BasicInfoHandle buffer = new BasicInfoHandle ();
					var code = zx_object_get_info ((uint)handle, ZxObjectInfoTopic.HandleBasic, (IntPtr)(&buffer), (IntPtr) sizeof(BasicInfoHandle), out var ignore1, out var ignore2);

					return buffer;
				}
			}
		}

		/// <summary>
		/// Gets the type of the handle
		/// </summary>
		/// <value>The type of the object.</value>
		public ZxObjectType ObjectType => HandleInformation.Type;

		// MISSING:
		// object_get_child - find the child of an object by its koid
		// object_get_cookie - read an object cookie
		// object_get_property - read an object property
		// object_set_cookie - write an object cookie
		// object_set_property - modify an object property
		// object_signal - set or clear the user signals on an object
		// object_signal_peer - set or clear the user signals in the opposite end
		// object_wait_many - wait for signals on multiple objects
		// object_wait_one - wait for signals on one object
		// object_wait_async - asynchronous notifications on signal change
		// handle_close_many

		// MISSING:
		// Per type, the ZX_INFO_TOPIC
	}
}