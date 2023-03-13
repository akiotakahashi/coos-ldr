using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CooS {

	static class Kernel {

		static unsafe void Main(string[] args) {
			byte* ptr = (byte*)0xb8000;
			ptr[0 + 77 * 2 + 24 * 160] = (byte)'k';
			ptr[1 + 77 * 2 + 24 * 160] = (byte)0x7;
			ptr[0 + 78 * 2 + 24 * 160] = (byte)'n';
			ptr[1 + 78 * 2 + 24 * 160] = (byte)0x7;
			ptr[0 + 79 * 2 + 24 * 160] = (byte)'l';
			ptr[1 + 79 * 2 + 24 * 160] = (byte)0x7;
		}

		public static void Delay(int ms, int us, int ns) {
		}

		internal static IntPtr ObjectToValue(object obj) {
			throw new NotImplementedException();
		}

		internal static IntPtr GetFunctionPointer(Delegate handler) {
			throw new NotImplementedException();
		}

		internal static object GetDelegateTarget(Delegate handler) {
			throw new NotImplementedException();
		}

	}

}
