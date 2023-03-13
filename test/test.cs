using System;
using System.Runtime.InteropServices;

class Program {

	[DllImport("native.dll")]
	extern static bool Beep(uint dwFreq, uint dwDuration);

	[DllImport("native.dll")]
	extern static bool Beep2(uint dwFreq, uint dwDuration);

	[DllImport("native.dll")]
	extern static bool Beep3(uint dwFreq, uint dwDuration);

	static void Main() {
		bool ret = Beep(1, 2);
		ret = Beep2(1, 2);
		ret = Beep3(1, 2);
	}

}
