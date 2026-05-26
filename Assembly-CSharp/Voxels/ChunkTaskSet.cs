using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace Voxels
{
	// Token: 0x020012CA RID: 4810
	public class ChunkTaskSet
	{
		// Token: 0x17000B97 RID: 2967
		// (get) Token: 0x0600784F RID: 30799 RVA: 0x00277DF3 File Offset: 0x00275FF3
		public Chunk Chunk
		{
			get
			{
				return this.Chunks[0];
			}
		}

		// Token: 0x17000B98 RID: 2968
		// (get) Token: 0x06007850 RID: 30800 RVA: 0x00277E04 File Offset: 0x00276004
		public bool HasChunks
		{
			get
			{
				if (this.Chunks == null || this.Chunks.Count == 0)
				{
					return false;
				}
				using (List<Chunk>.Enumerator enumerator = this.Chunks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current == null)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x17000B99 RID: 2969
		// (get) Token: 0x06007851 RID: 30801 RVA: 0x00277E70 File Offset: 0x00276070
		public bool IsEmpty
		{
			get
			{
				return !this.Current.IsCreated && this.Callback == null && this.Tasks.Count == 0;
			}
		}

		// Token: 0x06007852 RID: 30802 RVA: 0x00277E97 File Offset: 0x00276097
		public ChunkTaskSet(GenerationParameters parameters)
		{
			this.Chunks = new List<Chunk>();
			this.Parameters = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>();
		}

		// Token: 0x06007853 RID: 30803 RVA: 0x00277EBC File Offset: 0x002760BC
		public ChunkTaskSet(Chunk chunk, GenerationParameters parameters, [TupleElementNames(new string[]
		{
			"task",
			"callback"
		})] params ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>[] tasks)
		{
			this.Chunks = new List<Chunk>
			{
				chunk
			};
			this.Parameters = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>(tasks);
		}

		// Token: 0x06007854 RID: 30804 RVA: 0x00277EE9 File Offset: 0x002760E9
		public ChunkTaskSet(IList<Chunk> chunks, GenerationParameters parameters, [TupleElementNames(new string[]
		{
			"task",
			"callback"
		})] params ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>[] tasks)
		{
			this.Chunks = new List<Chunk>(chunks);
			this.Parameters = parameters;
			this.Tasks = new Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>(tasks);
		}

		// Token: 0x06007855 RID: 30805 RVA: 0x00277F10 File Offset: 0x00276110
		public void AddTask(ChunkTaskSet.ChunkTaskDelegate task, Action<Chunk> callback = null)
		{
			this.Tasks.Enqueue(new ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>(task, callback));
		}

		// Token: 0x06007856 RID: 30806 RVA: 0x00277F24 File Offset: 0x00276124
		public void Start()
		{
			if (this.Current.IsCreated || this.Callback != null)
			{
				throw new InvalidOperationException("Cannot start a ChunkTaskSet that is already running.");
			}
			this.StartNext();
			this.UpdateDirty();
		}

		// Token: 0x06007857 RID: 30807 RVA: 0x00277F53 File Offset: 0x00276153
		public void Complete()
		{
			this.CompleteCurrent();
			while (this.StartNext())
			{
				this.CompleteCurrent();
			}
			this.UpdateDirty();
		}

		// Token: 0x06007858 RID: 30808 RVA: 0x00277F71 File Offset: 0x00276171
		public bool CompleteIfReady()
		{
			if (this.CompleteCurrentIfReady())
			{
				if (this.Tasks.Count == 0)
				{
					this.UpdateDirty();
					return true;
				}
				this.StartNext();
			}
			this.UpdateDirty();
			return false;
		}

		// Token: 0x06007859 RID: 30809 RVA: 0x00277F9E File Offset: 0x0027619E
		private bool CompleteCurrentIfReady()
		{
			if (this.Current.IsCompleted)
			{
				this.CompleteCurrent();
				return true;
			}
			return false;
		}

		// Token: 0x0600785A RID: 30810 RVA: 0x00277FB8 File Offset: 0x002761B8
		private void CompleteCurrent()
		{
			this.Current.Complete();
			foreach (Chunk obj in this.Chunks)
			{
				Action<Chunk> callback = this.Callback;
				if (callback != null)
				{
					callback(obj);
				}
			}
			this.Current = default(ChunkTask);
			this.Callback = null;
		}

		// Token: 0x0600785B RID: 30811 RVA: 0x00278034 File Offset: 0x00276234
		private bool StartNext()
		{
			if (this.Tasks.Count == 0)
			{
				return false;
			}
			ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>> task = this.Tasks.Dequeue();
			ValueTuple<ChunkTask, Action<Chunk>> valueTuple = this.CreateTask(task);
			this.Current = valueTuple.Item1;
			this.Callback = valueTuple.Item2;
			if (this.Current.IsCompleted)
			{
				this.CompleteCurrent();
				return this.StartNext();
			}
			return true;
		}

		// Token: 0x0600785C RID: 30812 RVA: 0x00278098 File Offset: 0x00276298
		private void UpdateDirty()
		{
			if (this.Current.IsCreated || this.Callback != null || this.Tasks.Count > 0)
			{
				using (List<Chunk>.Enumerator enumerator = this.Chunks.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Chunk chunk = enumerator.Current;
						chunk.IsDirty = false;
					}
					return;
				}
			}
			foreach (Chunk chunk2 in this.Chunks)
			{
				chunk2.IsDirty = (chunk2.State < ChunkState.MeshAssigned);
			}
		}

		// Token: 0x0600785D RID: 30813 RVA: 0x00278154 File Offset: 0x00276354
		private ValueTuple<ChunkTask, Action<Chunk>> CreateTask([TupleElementNames(new string[]
		{
			"task",
			"callback"
		})] ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>> task)
		{
			return this.CreateTask(task.Item1, task.Item2);
		}

		// Token: 0x0600785E RID: 30814 RVA: 0x00278168 File Offset: 0x00276368
		private ValueTuple<ChunkTask, Action<Chunk>> CreateTask(ChunkTaskSet.ChunkTaskDelegate task, Action<Chunk> callback = null)
		{
			if (this.Chunks.Count == 1)
			{
				return new ValueTuple<ChunkTask, Action<Chunk>>((task != null) ? task(this.Chunks[0], this.Parameters) : default(ChunkTask), callback);
			}
			NativeArray<JobHandle> jobs = new NativeArray<JobHandle>(this.Chunks.Count, Allocator.Temp, NativeArrayOptions.ClearMemory);
			for (int i = 0; i < this.Chunks.Count; i++)
			{
				jobs[i] = ((task != null) ? task(this.Chunks[i], this.Parameters).Handle : default(JobHandle));
			}
			JobHandle handle = JobHandle.CombineDependencies(jobs);
			jobs.Dispose();
			return new ValueTuple<ChunkTask, Action<Chunk>>(new ChunkTask(this.Chunks[0], handle, null), callback);
		}

		// Token: 0x04008B4D RID: 35661
		public List<Chunk> Chunks;

		// Token: 0x04008B4E RID: 35662
		public GenerationParameters Parameters;

		// Token: 0x04008B4F RID: 35663
		[TupleElementNames(new string[]
		{
			"task",
			"callback"
		})]
		public Queue<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>> Tasks;

		// Token: 0x04008B50 RID: 35664
		public ChunkTask Current;

		// Token: 0x04008B51 RID: 35665
		public Action<Chunk> Callback;

		// Token: 0x020012CB RID: 4811
		// (Invoke) Token: 0x06007860 RID: 30816
		public delegate ChunkTask ChunkTaskDelegate(Chunk chunk, GenerationParameters parameters);
	}
}
