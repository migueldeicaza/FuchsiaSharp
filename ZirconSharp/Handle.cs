using System;
using System.Runtime.InteropServices;

namespace ZirconSharp {

	public class ZirconHandle : SafeHandle {

		// Name of the zircon dynamic library to call, available to all ZirconSharp types
		internal const string Library = "zircon";

		public ZirconHandle (IntPtr preexistingHandle, bool ownsHandle) : base (IntPtr.Zero, ownsHandle)
		{
			SetHandle (preexistingHandle);
		}

		public override bool IsInvalid => handle == IntPtr.Zero;

		// This is just for marshalling
		internal ZirconHandle () : base (IntPtr.Zero, true)
		{
		}

		[DllImport (Library)]
		extern static void zx_handle_close (IntPtr handle);


		protected override bool ReleaseHandle ()
		{
			zx_handle_close (handle);
			return true;
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing) {
				ReleaseHandle ();
			}
		}
	}
}
