//
// Port.cs: API bindings for the Zircon Port base class
//
// Author:
//   Miguel de Icaza (miguel@gnome.org)
//
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.
//
using System;
namespace ZirconSharp {

	public class Port : ZirconObject {
		internal Port (uint handle, bool ownsHandle) : base (handle, ownsHandle)
		{
		}
	}
}
