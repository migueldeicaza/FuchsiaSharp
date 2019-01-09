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
	public class VirtualMemoryAddressRegion : ZirconObject {
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


		[DllImport (Library)]
		extern static ZxStatus zx_vmar_map (uint handle,
						    ZxVmOption options,
						    ulong vmar_offset,
						    uint vmo_handle,
						    ulong vmo_offset,
						    ulong len,
						    out IntPtr mapped_addr);

		/// <summary>
		/// Adds a memory mapping - Maps the given VMO into the given virtual memory address region. 
		/// The mapping retains a reference to the underlying virtual memory object, which means 
		/// closing the VMO handle does not remove the mapping added by this function.
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        Success, and and the <paramref name="mapped_addr"/> set to the absolute base address of the mapping.
		///        The base address will be page-aligned and non-zero. In the event of failure, a negative error value is returned.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle or vmo is not a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrWrongType</term>
		///      <description>
		///        handle or vmo is not a VMAR or VMO handle, respectively.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        handle refers to a destroyed VMAR.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        mapped_addr or options are not valid, vmar_offset is non-zero when neither ZX_VM_SPECIFIC nor ZX_VM_SPECIFIC_OVERWRITE are given, ZX_VM_SPECIFIC_OVERWRITE and ZX_VM_MAP_RANGE are both given, vmar_offset and len describe an unsatisfiable allocation due to exceeding the region bounds, vmar_offset or vmo_offset or len are not page-aligned, vmo_offset + ROUNDUP(len, PAGE_SIZE) overflows.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        Insufficient privileges to make the requested mapping.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNotSupported</term>
		///      <description>
		///        The VMO is resizable and ZX_VM_REQUIRE_NON_RESIZABLE was requested.
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
		/// <param name="options"><para>Options that control how the mapping will work, these can be used:</para>
		///    <list type="bullet">
		///    <item>
		///      <term>VmSpecific</term>
		///      <description>
		///        Use the vmar_offset to place the mapping, invalid if handle does not have the VmCanMapSpecific permission. 
		///        vmar_offset is an offset relative to the base address of the given VMAR. It is an error to specify a range 
		///        that overlaps with another VMAR or mapping.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmSpecificOverwrite</term>
		///      <description>
		///        Same as VmSpecific, but can overlap another mapping. It is still an error to partially-overlap another
		///        VMAR. If the range meets these requirements, it will atomically (with respect to all other 
		///        map/unmap/protect operations) replace existing mappings in the area.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmPermRead</term>
		///      <description>
		///        Map vmo as readable. It is an error if handle does not have VmCanMapRead permissions, the handle does 
		///        not have the ZX_RIGHT_READ right, or the vmo handle does not have the ZX_RIGHT_READ right.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmPermWrite</term>
		///      <description>
		///        Map vmo as writable. It is an error if handle does not have VmCanMapWrite permissions, the handle does 
		///        not have the ZX_RIGHT_WRITE right, or the vmo handle does not have the ZX_RIGHT_WRITE right.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmPermExecute</term>
		///      <description>
		///        Map vmo as executable. It is an error if handle does not have VmCanMapExecute permissions, 
		///        the handle handle does not have the ZX_RIGHT_EXECUTE right, or the vmo handle does not have the 
		///        ZX_RIGHT_EXECUTE right.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmMapRange</term>
		///      <description>
		///        Immediately page into the new mapping all backed regions of the VMO. This cannot be
		///        specified if VmSpecificOverwrite is used.
		///      </description>
		///    </item>
		///    <item>
		///      <term>VmRequireNonResizable</term>
		///      <description>
		///        Maps the VMO only if the VMO is non-resizable, that is, it was created with the ZX_VMO_NON_RESIZABLE option.
		///      </description>
		///    </item>
		/// </list>
		/// </param>
		/// <param name="vmar_offset">Vmar offset.</param>
		/// <param name="vmo">Vmo.</param>
		/// <param name="vmo_offset"> must be 0 if options does not have VmSpecific or VmSpecificOverwrite set. If neither 
		/// of those are set, then the mapping will be assigned an offset at random by the kernel (with an allocator 
		/// determined by policy set on the target VMAR).</param>
		/// <param name="len">Length, must be page aligned.</param>
		/// <param name="mapped_addr">On success, the mapped address.</param>
		/// <remarks>
		/// The VMO that backs a memory mapping can be resized to a smaller size. This can cause the thread is reading or 
		/// writing to the VMAR region to fault. To avoid this hazard, services that receive VMOs from clients should use 
		/// VmRequireNonResizable when mapping the VMO
		/// </remarks>
		public ZxStatus Map (ZxVmOption options, ulong vmar_offset, VirtualMemoryObject vmo, ulong vmo_offset, ulong len, out IntPtr mapped_addr)
		{
			if (vmo == null)
				throw new ArgumentNullException (nameof (vmo));
			return zx_vmar_map ((uint)handle, options, vmar_offset, (uint)vmo.DangerousGetHandle (), vmo_offset, len, out mapped_addr);
		}

		[DllImport (Library)]
		extern static ZxStatus zx_vmar_unmap (uint handle, IntPtr addr, ulong len);

		/// <summary>
		/// unmap virtual memory pages between addr and addr+len.
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
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        addr is not page-aligned, len is 0 or not page-aligned, or the requested range partially overlaps a sub-region.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadState</term>
		///      <description>
		///        handle refers to a destroyed handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrNotFound</term>
		///      <description>
		///        Could not find the requested mapping.
		///      </description>
		///    </item>
		/// </list>
		/// </returns>
		/// <param name="addr">Address start</param>
		/// <param name="len">Length of the region to unmap, must be page aligned.</param>
		/// <remarks>This unmaps all VMO mappings and destroys (as if Destroy() were called) all sub-regions within the absolute 
		/// range including addr and ending before exclusively at addr + len. Any sub-region that is in the range must be fully 
		/// in the range (i.e. partial overlaps are an error). If a mapping is only partially in the range, the mapping is 
		/// split and the requested portion is unmapped.
		/// </remarks>
		public ZxStatus Unmap (IntPtr addr, ulong len)
		{
			return zx_vmar_unmap ((uint) handle, addr, len);
		}

		/// <summary>
		/// Creates a duplicate of the object with the specified rights, by requesting the kernel to duplicate the handle with those values
		/// </summary>
		/// <returns>
		///    <list type="bullet">
		///    <item>
		///      <term>Ok</term>
		///      <description>
		///        and the duplicate in result
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrBadHandle</term>
		///      <description>
		///        handle isn't a valid handle.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrInvalidArgs</term>
		///      <description>
		///        The rights requested are not a subset of handle rights or out is an invalid pointer.
		///      </description>
		///    </item>
		///    <item>
		///      <term>ErrAccessDenied</term>
		///      <description>
		///        handle does not have ZX_RIGHT_DUPLICATE and may not be duplicated.
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
		/// <param name="rights">New desired access rights.</param>
		public ZxStatus Duplicate (ZxRights rights, out VirtualMemoryAddressRegion result)
		{
			uint rhandle;
			var ret = zx_handle_duplicate ((uint)handle, rights, out rhandle);
			if (ret == ZxStatus.Ok)
				result = new VirtualMemoryAddressRegion (rhandle, address);
			else
				result = null;
			return ret;
		}
	}
}
