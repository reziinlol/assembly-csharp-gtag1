using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Voxels
{
	// Token: 0x020012D8 RID: 4824
	[BurstCompile]
	public struct SortChunksJob : IJob
	{
		// Token: 0x06007888 RID: 30856 RVA: 0x00279508 File Offset: 0x00277708
		public void Execute()
		{
			int count = this.ChunkSet.Count;
			this.SortedChunks.ResizeUninitialized(count);
			NativeArray<int3> nativeArray = new NativeArray<int3>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			int num = 0;
			foreach (int3 value in this.ChunkSet)
			{
				nativeArray[num++] = value;
			}
			NativeArray<SortChunksJob.SortKey> array = new NativeArray<SortChunksJob.SortKey>(count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < count; i++)
			{
				uint num2 = (uint)math.distancesq(nativeArray[i], this.TargetPos);
				array[i] = new SortChunksJob.SortKey((ulong)num2 << 32 | (ulong)i);
			}
			array.Sort<SortChunksJob.SortKey>();
			for (int j = 0; j < count; j++)
			{
				int index = (int)(array[j].value & (ulong)-1);
				this.SortedChunks[j] = nativeArray[index];
			}
			array.Dispose();
			nativeArray.Dispose();
		}

		// Token: 0x04008B81 RID: 35713
		[ReadOnly]
		public NativeHashSet<int3> ChunkSet;

		// Token: 0x04008B82 RID: 35714
		[ReadOnly]
		public int3 TargetPos;

		// Token: 0x04008B83 RID: 35715
		public NativeList<int3> SortedChunks;

		// Token: 0x020012D9 RID: 4825
		private struct SortKey : IComparable<SortChunksJob.SortKey>
		{
			// Token: 0x06007889 RID: 30857 RVA: 0x0027962C File Offset: 0x0027782C
			public SortKey(ulong val)
			{
				this.value = val;
			}

			// Token: 0x0600788A RID: 30858 RVA: 0x00279635 File Offset: 0x00277835
			public int CompareTo(SortChunksJob.SortKey other)
			{
				return this.value.CompareTo(other.value);
			}

			// Token: 0x04008B84 RID: 35716
			public ulong value;
		}
	}
}
