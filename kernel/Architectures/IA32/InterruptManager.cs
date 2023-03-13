using System;
using CooS.Toolchains;
using IA32Toolkit.Assembler;
using System.Runtime.InteropServices;

namespace CooS.Architectures.IA32 {

#if true

	[StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
	public struct InterruptDescriptorTable {
		public ushort limit;
		public uint start;
	}

	[StructLayoutAttribute(LayoutKind.Sequential, Pack = 1)]
	public struct InterruptDescriptor {
		public ushort handler_l;
		public ushort segment;
		public ushort flags;
		/*
		uint16 flags		: 13;
		uint16 dpl			: 2;
		uint16 p			: 1;
		*/
		public ushort handler_h;
	}


	public sealed unsafe class Instruction {
		public static void int3() {
			throw new NotImplementedException();
		}
		public static void hlt() {
			throw new NotImplementedException();
		}
		public static void sti() {
			throw new NotImplementedException();
		}
		public static void cli() {
			throw new NotImplementedException();
		}
		public static void intn(int iv) {
			throw new NotImplementedException();
		}
		public static ushort ldcs() {
			throw new NotImplementedException();
		}
		public static void lidt(InterruptDescriptorTable idt) {
			throw new NotImplementedException();
		}
		public static void sidt(out InterruptDescriptorTable pidt) {
			throw new NotImplementedException();
		}
		public static void outb(ushort port, byte data) {
			throw new NotImplementedException();
		}
		public static void outw(ushort port, ushort data) {
			throw new NotImplementedException();
		}
		public static void outd(ushort port, uint data) {
			throw new NotImplementedException();
		}
		public static byte inb(ushort port) {
			throw new NotImplementedException();
		}
		public static ushort inw(ushort port) {
			throw new NotImplementedException();
		}
		public static uint ind(ushort port) {
			throw new NotImplementedException();
		}
		public static void inw(ushort port, void* p, int count, ushort watchport, byte mask, byte value) {
			throw new NotImplementedException();
		}
		public static ulong rdtsc() {
			throw new NotImplementedException();
		}
		public static IntPtr GetReturnAddress(int depth) {
			throw new NotImplementedException();
		}
	}

#endif

	public static class InterruptManager {

		static readonly InterruptDescriptor[] idt = new InterruptDescriptor[256];
		static readonly Delegate[] handlers = new Delegate[256];
		static readonly byte[][] proxies = new byte[256][];
		static readonly GCHandle[] pins = new GCHandle[256];

		static unsafe InterruptManager() {
			//if(!Engine.Privileged) throw new NotSupportedException("System is not priviledged.");
			for(int i=0; i<idt.Length; ++i) {
				idt[i].segment = 0x8;
				idt[i].handler_h = 0;
				idt[i].handler_l = 0;
				idt[i].flags = 0x8E00;
			}
			fixed(InterruptDescriptor* p = &idt[0]) {
				InterruptDescriptorTable buf;
				Instruction.sidt(out buf);
				//Tuning.Memory.Copy(p, (void*)buf.start, (uint)buf.limit+1);
				Console.WriteLine("Initialized InterruptManager: start=0x{0:X08}, limit=0x{1:X04}",buf.start,buf.limit);
			}
			ResetIDT();
		}

		static unsafe void ResetIDT() {
			int size = 8/*InterruptDescriptor.Size*/*idt.Length;
			fixed(InterruptDescriptor* p = &idt[0]) {
				InterruptDescriptorTable buf;
				buf.start = (uint)p;
				buf.limit = (ushort)(size-1);
				Instruction.lidt(buf);
			}
		}

		internal static void SetGate(ushort flags, byte intno, Delegate handler) {
			idt[intno] = new InterruptDescriptor();
			handlers[intno] = null;
			if(proxies[intno]!=null) {
				proxies[intno] = null;
				pins[intno].Free();
				pins[intno] = new GCHandle();
			}
			if(handler!=null) {
				Console.Write("Register Interrupt Handler: ");
				int target = Kernel.ObjectToValue(Kernel.GetDelegateTarget(handler)).ToInt32();
				int entrypoint = Kernel.GetFunctionPointer(handler).ToInt32();
				IA32Assembler assembler = new IA32Assembler();
				assembler.Pushad();
				assembler.Push(Register32.ESP);
				if(target==0) {
					assembler.Push(Register32.ESP);
				} else {
					assembler.Push(target);
					assembler.Push(Register32.ESP);
					assembler.Add(RegMem.Indirect(Register32.ESP), 4);
				}
				assembler.Mov(Register32.EAX, entrypoint);
				assembler.Call(Register32.EAX);
				assembler.Pop(Register32.ESP);
				assembler.Popad();
				assembler.Iretd();
				CodeInfo codeinfo = new CodeInfo(((System.IO.MemoryStream)assembler.BaseStream).ToArray(), 0);
				byte[] proxy = codeinfo.CodeBlock;
				proxies[intno] = proxy;
				pins[intno] = GCHandle.Alloc(proxy);
				IntPtr p = codeinfo.EntryPoint;
				InterruptDescriptor id;
				id.handler_l = (ushort)(uint)p.ToInt32();
				id.handler_h = (ushort)((uint)p.ToInt32()>>16);
				id.segment = Instruction.ldcs();
				id.flags = (ushort)(flags|0x8000);
				/*
				id.flags = flags;
				id.dpl = 0;
				id.p = 1;
				*/
				handlers[intno] = handler;
				idt[intno] = id;
				Console.WriteLine("#{3:X02}: 0x{0:X08} via proxy 0x{1:X04}{2:X04}", entrypoint, id.handler_h, id.handler_l, intno);
			}
			//
			ResetIDT();
		}

		public static unsafe int CopyContext(IntPtr sp, byte[] stack) {
			int size = 4+4*8+4*3;
			fixed(byte* p = &stack[stack.Length-size]) {
				//CooS.Tuning.Memory.Copy(p, sp.ToPointer(), size);
			}
			return size;
		}

	}

}
