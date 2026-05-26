using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Voxels
{
	// Token: 0x020012D7 RID: 4823
	public struct NativeCounter : IDisposable
	{
		// Token: 0x17000B9C RID: 2972
		// (get) Token: 0x06007883 RID: 30851 RVA: 0x002794AC File Offset: 0x002776AC
		// (set) Token: 0x06007884 RID: 30852 RVA: 0x002794B5 File Offset: 0x002776B5
		public unsafe int Count
		{
			get
			{
				return *this._counter;
			}
			set
			{
				*this._counter = value;
			}
		}

		// Token: 0x06007885 RID: 30853 RVA: 0x002794BF File Offset: 0x002776BF
		public unsafe NativeCounter(Allocator allocator)
		{
			this._allocator = allocator;
			this._counter = (int*)UnsafeUtility.Malloc(4L, 4, this._allocator);
			this.Count = 0;
		}

		// Token: 0x06007886 RID: 30854 RVA: 0x002794E3 File Offset: 0x002776E3
		public unsafe int Increment()
		{
			return Interlocked.Increment(ref *this._counter) - 1;
		}

		// Token: 0x06007887 RID: 30855 RVA: 0x002794F2 File Offset: 0x002776F2
		public unsafe void Dispose()
		{
			UnsafeUtility.Free((void*)this._counter, this._allocator);
		}

		// Token: 0x04008B7F RID: 35711
		private readonly Allocator _allocator;

		// Token: 0x04008B80 RID: 35712
		[NativeDisableUnsafePtrRestriction]
		private unsafe readonly int* _counter;
	}
}
