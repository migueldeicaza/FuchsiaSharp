using System;
using System.Runtime.InteropServices;

namespace ZirconSharp {
	/// <summary>
	/// Options for creating Virtual Memory Objects.
	/// </summary>
	[Flags]
	public enum VmoOptions : uint {
		/// <summary>
		/// The default.
		/// </summary>
		Default = 0,
		/// <summary>
		/// Use to create a VirtualMemoryObject that is non resizable
		/// </summary>
		NonResizable = 1
	}

	public enum VmoOperation : uint {
		/// <summary>
		/// Commit size bytes worth of pages starting at byte offset for the VMO. Requires the ZX_RIGHT_WRITE right.
		/// </summary>
		Commit = 1,
		/// <summary>
		/// Release a range of pages previously committed to the VMO from offset to offset+size. Requires the ZX_RIGHT_WRITE right.
		/// </summary>
		Decommit = 2,
		/// <summary>
		/// Presently unsupported.
		/// </summary>
		Lock = 3,
		/// <summary>
		/// Presently unsupported.
		/// </summary>
		Unlock = 4,
		/// <summary>
		/// Performs a cache sync operation. Requires the ZX_RIGHT_READ right.
		/// </summary>
		CacheSync = 6,
		/// <summary>
		///  Performs a cache invalidation operation. Requires the ZX_RIGHT_WRITE right.
		/// </summary>
		CacheInvalidate = 7,
		/// <summary>
		/// Performs a cache clean operation. Requires the ZX_RIGHT_READ right.
		/// </summary>
		CacheClean = 8,
		/// <summary>
		/// Performs cache clean and invalidate operations together. Requires the ZX_RIGHT_READ right.
		/// </summary>
		CacheCleanInvalidate = 9
	}

	public enum ZxCachePolicy : uint {
		/// <summary>
		/// Use hardware caching.
		/// </summary>
		Cached = 0,
		/// <summary>
		/// Disable caching.
		/// </summary>
		Uncached = 1,
		/// <summary>
		/// Disable cache and treat as device memory. This is architecture dependent and may be equivalent to Uncached on some devices.
		/// </summary>
		UncachedDevice = 2,
		/// <summary>
		/// Uncached with write combining.
		/// </summary>
		WriteCombining = 3,

		PolicyMask = 3
	}

	/// <summary>
	/// A Virtual Memory Object (VMO) represents a contiguous region of virtual memory that may be mapped into multiple address spaces.
	/// </summary>
	/// <remarks>
	/// <para> 
	///   VMOs are used in by the kernel and userspace to represent
	///   both paged and physical memory. They are the standard method of
	///   sharing memory between processes, as well as between the kernel
	///   and userspace.
	/// </para>
	/// 
	/// <para> 
	///   VMOs are created with vmo_create and basic I/O can be
	///   performed on them with vmo_read and vmo_write. A VMO‘s size may
	///   be set using vmo_set_size. Conversely, vmo_get_size will
	///   retrieve a VMO’s current size.
	/// </para>
	/// 
	/// <para> 
	///   The size of a VMO will be rounded up to the next page size
	///   boundary by the kernel.
	/// </para>
	/// 
	/// <para> 
	///   Pages are committed (allocated) for VMOs on demand through
	///   vmo_read, vmo_write, or by writing to a mapping of the VMO
	///   created using vmar_map. Pages can be committed and decommitted
	///   from a VMO manually by calling vmo_op_range with the
	///   ZX_VMO_OP_COMMIT and ZX_VMO_OP_DECOMMIT operations, but this
	///   should be considered a low level operation. vmo_op_range can
	///   also be used for cache and locking operations against pages a
	///   VMO holds.
	/// </para>
	/// 
	/// <para> 
	///   Processes with special purpose use cases involving cache
	///   policy can use vmo_set_cache_policy to change the policy of a
	///   given VMO. This use case typically applies to device
	///   drivers.
	/// </para>
	/// </remarks>
	public class VirtualMemoryObject : ZirconHandle {

		VirtualMemoryObject (uint handle) : base (handle, ownsHandle: true) { }

		[DllImport (Library)]
		extern static ZxStatus zx_vmo_create (ulong size, uint options, out uint result);

		/// <summary>
		/// creates a new virtual memory object (VMO), which represents a container of zero to size bytes of memory managed by the operating system.
		/// </summary>
		/// <remarks>
		///   <para>
		///      The size of the VMO will be rounded up to the next page size boundary.
		///      Use the Size proeprty to return the current size of the VMO.
		///   </para>
		///   <para>
		///      One handle is returned on success, representing an object with the requested size.
		///   </para>
		///   <para>The following rights will be set on the handle by default:</para>
		///   <list type="bullet">
		///     <item></item>
		///   </list>
		/// <para>
		///   The ZX_VMO_ZERO_CHILDREN signal is active on a newly created VMO. It becomes inactive whenever a 
		///   clone of the VMO is created and becomes active again when all clones have been destroyed and 
		///   no mappings of those clones into address spaces exist.
		/// </para>
		/// </remarks>
		/// <param name="size">The desired size</param>
		/// <param name="result">On success, this will be set to the VirtualMemoryObject created, or null on error.</param>
		/// <param name="options">Flags for creating the virtual memory object.</param>
		/// <returns>
		///   ZxStatus.Ok on success, otherwise the error.
		/// </returns>
		public static ZxStatus Create (ulong size, out VirtualMemoryObject result, VmoOptions options = VmoOptions.Default)
		{
			uint handle;
			var ret = zx_vmo_create (size, (uint) VmoOptions.Default, out handle);
			if (ret != ZxStatus.Ok) {
				result = null;
				return ret;
			}
			result = new VirtualMemoryObject (handle);
			return ret;
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmo_read (uint handle, IntPtr buffer, ulong offset, IntPtr buffer_size);

		/// <summary>
		/// Read bytes from the VMO
		/// </summary>
		/// <returns>The Ok status or ErrBadHandle, ErrWrongType, ErrAccessDeined, ErrInvalidArgs, ErrOutOfRange, ErrBadState.</returns>
		/// <param name="destination">Destination where the data will be copied to.</param>
		/// <param name="offset">Offset inside the VMO to read from.</param>
		/// <param name="count">Number of bytes to read.</param>
		public ZxStatus Read (IntPtr destination, ulong offset, IntPtr count)
		{
			return zx_vmo_read ((uint) handle, destination, offset, count);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmo_write (uint handle, IntPtr buffer, ulong offset, IntPtr buffer_size);

		/// <summary>
		/// Write bytes to the VMO
		/// </summary>
		/// <returns>The Ok status or ErrBadHandle, ErrWrongType, ErrAccessDenied, ErrInvalidArgs, ErrNoMemory, ErrOutOfRange, ErrBadState.</returns>
		/// <param name="source">Source of data to copy bytes from.</param>
		/// <param name="offset">Offset inside the VMO to write to.</param>
		/// <param name="count">Number of bytes to write.</param>
		public ZxStatus Write(IntPtr source, ulong offset, IntPtr count)
		{
			return zx_vmo_write ((uint)handle, source, offset, count);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmo_get_size (uint handle, out ulong size);

		/// <summary>
		/// Read the current size of the Virtual Memory Object
		/// </summary>
		/// <value>The size.</value>
		public ulong Size {
			get {
				ulong size;
				return zx_vmo_get_size ((uint)handle, out size) == ZxStatus.Ok ? size : 0;
			}
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmo_set_size (uint handle, ulong size);

		/// <summary>
		/// Resize the virtual memory object to the specified size
		/// </summary>
		/// <returns>Ok on Success, or one of ErrBadHandle, ErrWrongType, ErrAccessDeined, ErrUnavailable, ErrOutOfRange, ErrNoMemory.</returns>
		/// <param name="newSize">New Size for the VMO.</param>
		public ZxStatus Resize (ulong newSize)
		{
			return zx_vmo_set_size ((uint)handle, newSize);
		}


		[DllImport (Library)]
		extern static ZxStatus zx_vmo_op_range (uint handle, VmoOperation operation, ulong offset, ulong size, IntPtr buffer, IntPtr bufferSize);

		/// <summary>
		/// Perform an operation on a range of a Virtual Memory Object
		/// </summary>
		/// <returns>Result of the operation Ok, ErrBadHandle, ErrOutOfRange, ErrNoMemory, ErrWrongType, ErrAccessDeined, ErrNotSupported or ErrInvalidArgs.</returns>
		/// <param name="operation">Operation to perform on the range.</param>
		/// <param name="startOffset">Starting offset inside the VMO.</param>
		/// <param name="size">Size on which to perform the operation, in bytes.</param>
		public ZxStatus PerformOperation (VmoOperation operation, ulong startOffset, ulong size)
		{
			return zx_vmo_op_range ((uint)handle, operation, startOffset, size, IntPtr.Zero, IntPtr.Zero);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmo_set_cache_policy (uint handle, uint cache_policy);

		/// <summary>
		/// Sets the cache policy for the Vmo.
		/// </summary>
		/// <returns>Status code: Ok, ErrAccessDeined, ErrInvalidArgs, ErrBadHandle, ErrNotSupported, or ErrBadState.</returns>
		/// <param name="policy">The cache policy for the Vmo.</param>
		public ZxStatus SetCachePolicy (ZxCachePolicy policy)
		{
			return zx_vmo_set_cache_policy ((uint) handle, (uint) policy);
		}
	}
}
