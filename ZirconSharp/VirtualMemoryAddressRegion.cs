using System;
using System.Runtime.InteropServices;

namespace ZirconSharp {
	/// <summary>
	/// A contiguous region of a virtual memory address space
	/// </summary>
	/// <remarks>
	/// /// <para>
	///   VMARs are used by the kernel and userspace to represent the
	///   allocation of an address space.
	/// </para>
	/// 
	/// <para> 
	///   Every process starts with a single VMAR (the root VMAR) that spans
	///   the entire address space (see process_create). Each VMAR can be
	///   logically divided up into any number of non-overlapping parts, each
	///   representing a child VMARs, a virtual memory mapping, or a
	///   gap. Child VMARs are created using vmar_allocate. VM mappings are
	///   created using vmar_map.
	/// </para>
	/// 
	/// <para>
	///   VMARs have a hierarchical permission model for allowable mapping
	///   permissions. For example, the root VMAR allows read, write, and
	///   executable mapping. One could create a child VMAR that only allows
	///   read and write mappings, in which it would be illegal to create a
	///   child that allows executable mappings.
	/// </para>
	/// 
	/// <para>
	///   When a VMAR is created using vmar_allocate, its parent VMAR retains a
	///   reference to it. Because of this, if all handles to the child VMAR are
	///   closed, the child and its descendants will remain active in the
	///   address space. In order to disconnect the child from the address
	///   space, vmar_destroy must be called on a handle to the child.
	/// </para>
	/// 
	/// <para> 
	///   By default, all allocations of address space are randomized. At
	///   VMAR creation time, the caller can choose which randomization
	///   algorithm is used. The default allocator attempts to spread
	///   allocations widely across the full width of the VMAR. The alternate
	///   allocator, selected with ZX_VM_COMPACT, attempts to keep allocations
	///   close together within the VMAR, but at a random location within the
	///   range. It is recommended to use the default allocator.
	/// </para>
	/// 
	/// <para> 
	///   VMARs optionally support a fixed-offset mapping mode (called
	///   specific mapping). This mode can be used to create guard pages or
	///   ensure the relative locations of mappings. Each VMAR may have the
	///   ZxVmOptions.VmCanMapSpecific permission, regardless of whether or not its
	///   parent VMAR had that permission.
	/// </para>
	/// </remarks>
	public class VirtualMemoryAddressRegion : ZirconHandle {
		[DllImport (Library)]
		extern static ZxStatus zx_vmar_allocate (uint parentMvarHandle, ZxVmOption options, ulong offset, ulong size, out uint child_mvar_handle, out IntPtr child_addr);
		IntPtr address;

		internal VirtualMemoryAddressRegion (uint handle, IntPtr address) : base (handle, true)
		{
			this.address = address;
		}

		/// <summary>
		/// Returns the address for the VirtualMemoryAddressRegion
		/// </summary>
		/// <value>The address.</value>
		public IntPtr Address => address;

		/// <summary>
		/// Allocate a new subregion
		/// </summary>
		/// <returns>
		/// <para>
		/// On success, Ok and the result parameter is set to the VirtualMemoryAddressRegion, on error, 
		/// a different status code is returned and the result parameter is set to null.
		/// </para>
		///    <list type="bullet">
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        parent_vmar is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        parent_vmar is not a VMAR handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        parent_vmar refers to a destroyed VMAR.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        child_vmar or child_addr are not valid, offset is non-zero when ZX_VM_SPECIFIC is not given, offset and size describe an unsatisfiable allocation due to exceeding the region bounds, offset or size is not page-aligned, or size is 0.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNoMemory</term>
		///      <description>
		///        Failure due to lack of memory. There is no good way for userspace to handle this (unlikely) error. In a future build this error will no longer occur.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        Insufficient privileges to make the requested allocation.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="parentVmar">Parent VMAR that will host this new VMAR.</param>
		/// <param name="options">
		/// <para>Options for creating the region.</para>
		///    <list type="bullet">
		///    <item>
		///      <term>VmCompact</term>
		///      <description>
		///        A hint to the kernel that allocations and mappings within the newly created subregion should be kept close together. See the NOTES section below for discussion.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmSpecific</term>
		///      <description>
		///        Use the offset to place the mapping, invalid if vmar does not have the VmCanMapSpecific permission. offset is an offset relative to the base address of the parent region. It is an error to specify an address range that overlaps with another VMAR or mapping.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmCanMapSpecific</term>
		///      <description>
		///        The new VMAR can have subregions/mappings created with VmSpecific. It is NOT an error if the parent does not have VmCanMapSpecific permissions.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmCanMapRead</term>
		///      <description>
		///        The new VMAR can contain readable mappings. It is an error if the parent does not have VmCanMapRead permissions.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmCanMapWrite</term>
		///      <description>
		///        The new VMAR can contain writable mappings. It is an error if the parent does not have VmCanMapWrite permissions.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmCanMapExecute</term>
		///      <description>
		///        The new VMAR can contain executable mappings. It is an error if the parent does not have VmCanMapExecute permissions.
		///      </description>
		///    </item>
		/// </list>
		/// </param>
		/// <param name="offset">Offset, it must be zero if options does not have VmSpecific set.</param>
		/// <param name="size">Size for the region.</param>
		/// <param name="result">The newly created VirtualMemoryAddressRegion is returned in this parameter.</param>
		/// <remarks>
		/// <para>The address space occupied by a VMAR will remain allocated (within its parent VMAR) until the VMAR is destroyed by calling </para>
		/// <para>Note that just closing the VMAR's handle does not deallocate the address space occupied by the VMAR.</para>
		/// <para>Compact Flag: The kernel interprets this flag as a request to reduce sprawl in allocations. While this does not necessitate 
		/// reducing the absolute entropy of the allocated addresses, there will potentially be a very high correlation between allocations.
		/// This is a trade-off that the developer can make to increase locality of allocations and reduce the number of page tables necessary, 
		/// if they are willing to have certain addresses be more correlated.</para>
		/// </remarks>
		public static ZxStatus Allocate (VirtualMemoryAddressRegion parentVmar, ZxVmOption options, ulong offset, ulong size, out VirtualMemoryAddressRegion result)
		{
			uint resultHandle;
			IntPtr address;
			var ret = zx_vmar_allocate ((uint)parentVmar.handle, options, offset, size, out resultHandle, out address);
			if (ret == ZxStatus.Ok)
				result = new VirtualMemoryAddressRegion (resultHandle, address);
			result = null;
			return ret;
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmar_destroy (uint handle);

		/// <summary>
		/// destroy a virtual memory address region
		/// </summary>
		/// <returns>
		///  <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        on success.
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
		///        handle is not a VMAR handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        This region is already destroyed.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// This unmaps all mappings within the given region, and destroys all sub-regions of the region. Note that this operation is logically recursive.
		/// This operation does not close handle.Any outstanding handles to this VMAR will remain valid handles, but all VMAR operations on them will fail.
		/// </remarks>
		public ZxStatus Destroy ()
		{
			return zx_vmar_destroy ((uint) handle);
		}

#if false
		[DllImport (Library)]
		extern static ZxStatus zx_vmar_map (uint handle,
					ZxVmOption options,
					ulong vmar_offset,
					uint vmo_handle,
					ulong vmo_offset,
					ulong len,
					IntPtr mapped_addr);

		public void AddMemoryMapping (ZxVmOption options, ulong vmar_offset,
					uint vmo_handle,
					ulong vmo_offset,
					ulong len,
					IntPtr mapped_addr
#endif
	}
}
