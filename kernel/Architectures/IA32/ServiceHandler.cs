using System;
//using CooS.Toolchains.IA32;
using System.Runtime.InteropServices;

namespace CooS.Architectures.IA32 {

	[StructLayout(LayoutKind.Sequential)]
	public struct RegisterSet {
		int edi;
		int esi;
		int ebp;
		int esp;
		int ebx;
		int edx;
		int ecx;
		int eax;
	}

	public delegate void ServiceHandler(ref RegisterSet regs);

}
