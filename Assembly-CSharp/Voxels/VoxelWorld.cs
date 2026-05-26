using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace Voxels
{
	// Token: 0x020012F0 RID: 4848
	[DefaultExecutionOrder(5)]
	public class VoxelWorld : MonoBehaviour
	{
		// Token: 0x17000B9F RID: 2975
		// (get) Token: 0x060078CE RID: 30926 RVA: 0x0027BAF6 File Offset: 0x00279CF6
		public IEnumerable<Chunk> Chunks
		{
			get
			{
				return this.chunks.Values;
			}
		}

		// Token: 0x17000BA0 RID: 2976
		// (get) Token: 0x060078CF RID: 30927 RVA: 0x0027BB03 File Offset: 0x00279D03
		// (set) Token: 0x060078D0 RID: 30928 RVA: 0x0027BB0B File Offset: 0x00279D0B
		public bool Initialized { get; private set; }

		// Token: 0x17000BA1 RID: 2977
		// (get) Token: 0x060078D1 RID: 30929 RVA: 0x0027BB14 File Offset: 0x00279D14
		public bool IsInfinite
		{
			get
			{
				return this.worldType == VoxelWorld.WorldType.Infinite;
			}
		}

		// Token: 0x17000BA2 RID: 2978
		// (get) Token: 0x060078D2 RID: 30930 RVA: 0x0027BB1F File Offset: 0x00279D1F
		public UnityEngine.BoundsInt WorldBounds
		{
			get
			{
				return this.worldBounds;
			}
		}

		// Token: 0x17000BA3 RID: 2979
		// (get) Token: 0x060078D3 RID: 30931 RVA: 0x0027BB27 File Offset: 0x00279D27
		// (set) Token: 0x060078D4 RID: 30932 RVA: 0x0027BB2F File Offset: 0x00279D2F
		public int Id { get; private set; }

		// Token: 0x17000BA4 RID: 2980
		// (get) Token: 0x060078D5 RID: 30933 RVA: 0x0027BB38 File Offset: 0x00279D38
		// (set) Token: 0x060078D6 RID: 30934 RVA: 0x0027BB40 File Offset: 0x00279D40
		public bool UpdateWorld
		{
			get
			{
				return this._updateWorld;
			}
			set
			{
				this._updateWorld = value;
			}
		}

		// Token: 0x17000BA5 RID: 2981
		// (get) Token: 0x060078D7 RID: 30935 RVA: 0x0027BB49 File Offset: 0x00279D49
		// (set) Token: 0x060078D8 RID: 30936 RVA: 0x0027BB51 File Offset: 0x00279D51
		public int3 ChunkSize { get; private set; }

		// Token: 0x17000BA6 RID: 2982
		// (get) Token: 0x060078D9 RID: 30937 RVA: 0x0027BB5A File Offset: 0x00279D5A
		// (set) Token: 0x060078DA RID: 30938 RVA: 0x0027BB62 File Offset: 0x00279D62
		public int VoxelDimension { get; private set; }

		// Token: 0x17000BA7 RID: 2983
		// (get) Token: 0x060078DB RID: 30939 RVA: 0x0027BB6B File Offset: 0x00279D6B
		// (set) Token: 0x060078DC RID: 30940 RVA: 0x0027BB73 File Offset: 0x00279D73
		public int VoxelCount { get; private set; }

		// Token: 0x17000BA8 RID: 2984
		// (get) Token: 0x060078DD RID: 30941 RVA: 0x0027BB7C File Offset: 0x00279D7C
		public MeshGenerationMode MeshGenerationMode
		{
			get
			{
				return this.generationParameters.MeshGenerationMode;
			}
		}

		// Token: 0x17000BA9 RID: 2985
		// (get) Token: 0x060078DE RID: 30942 RVA: 0x0027BB89 File Offset: 0x00279D89
		public bool WorldGenerationComplete
		{
			get
			{
				return this.chunks.Count > 0 && this.chunksToGenerate.Count == 0;
			}
		}

		// Token: 0x060078DF RID: 30943 RVA: 0x0027BBA9 File Offset: 0x00279DA9
		public static bool ExistsFor(Scene scene)
		{
			return VoxelWorld.WorldLookup.ContainsKey(scene.GetHashCode());
		}

		// Token: 0x060078E0 RID: 30944 RVA: 0x0027BBC2 File Offset: 0x00279DC2
		public static bool ExistsFor(GameObject gameObject)
		{
			return VoxelWorld.ExistsFor(gameObject.scene);
		}

		// Token: 0x060078E1 RID: 30945 RVA: 0x0027BBCF File Offset: 0x00279DCF
		public static bool ExistsFor(Component component)
		{
			return VoxelWorld.ExistsFor(component.gameObject.scene);
		}

		// Token: 0x060078E2 RID: 30946 RVA: 0x0027BBE1 File Offset: 0x00279DE1
		public static void SetFor(Scene scene, VoxelWorld voxelWorld)
		{
			if (!VoxelWorld.WorldLookup.TryAdd(scene.GetHashCode(), voxelWorld))
			{
				throw new InvalidOperationException(string.Format("Scene {0} already has a VoxelWorld set.", scene));
			}
		}

		// Token: 0x060078E3 RID: 30947 RVA: 0x0027BC13 File Offset: 0x00279E13
		public static void SetFor(GameObject gameObject, VoxelWorld voxelWorld)
		{
			VoxelWorld.SetFor(gameObject.scene, voxelWorld);
		}

		// Token: 0x060078E4 RID: 30948 RVA: 0x0027BC21 File Offset: 0x00279E21
		public static void SetFor(Component component, VoxelWorld voxelWorld)
		{
			VoxelWorld.SetFor(component.gameObject.scene, voxelWorld);
		}

		// Token: 0x060078E5 RID: 30949 RVA: 0x0027BC34 File Offset: 0x00279E34
		public static VoxelWorld GetFor(Scene scene)
		{
			VoxelWorld result;
			if (!VoxelWorld.WorldLookup.TryGetValue(scene.GetHashCode(), out result))
			{
				Debug.LogError(string.Format("No VoxelWorld found for scene {0}", scene));
			}
			return result;
		}

		// Token: 0x060078E6 RID: 30950 RVA: 0x0027BC72 File Offset: 0x00279E72
		public static VoxelWorld GetFor(GameObject gameObject)
		{
			return VoxelWorld.GetFor(gameObject.scene);
		}

		// Token: 0x060078E7 RID: 30951 RVA: 0x0027BC7F File Offset: 0x00279E7F
		public static VoxelWorld GetFor(Component component)
		{
			return VoxelWorld.GetFor(component.gameObject.scene);
		}

		// Token: 0x060078E8 RID: 30952 RVA: 0x0027BC94 File Offset: 0x00279E94
		private void Awake()
		{
			if (this.registerAsSceneWorld && !VoxelWorld.ExistsFor(this))
			{
				VoxelWorld.SetFor(base.gameObject, this);
			}
			MeshGenerationMode meshGenerationMode = this.generationParameters.MeshGenerationMode;
			if (meshGenerationMode != MeshGenerationMode.MarchingCubes)
			{
				if (meshGenerationMode != MeshGenerationMode.SurfaceNets)
				{
					throw new ArgumentOutOfRangeException();
				}
				Chunk.Pad = 2;
			}
			else
			{
				Chunk.Pad = 1;
			}
			if (!this.target)
			{
				this.target = base.transform;
			}
			this.Id = this.GenerateHashcodeFromPath();
		}

		// Token: 0x060078E9 RID: 30953 RVA: 0x0027BD10 File Offset: 0x00279F10
		private void Start()
		{
			Chunk.DefaultSize = this.chunkSize;
			this.ChunkSize = Chunk.DefaultSize;
			this.VoxelDimension = this.chunkSize + Chunk.Pad;
			this.VoxelCount = this.VoxelDimension * this.VoxelDimension * this.VoxelDimension;
			int num = this.viewDistance * 2 + 1;
			this.chunksToGenerate = new NativeHashSet<int3>(num * num * num, Allocator.Persistent);
			this.sortedChunks = new NativeList<int3>(Allocator.Persistent);
			this.ConfigurePools();
			this.Initialized = true;
		}

		// Token: 0x060078EA RID: 30954 RVA: 0x0027BDA3 File Offset: 0x00279FA3
		private void OnEnable()
		{
			VoxelManager.Register(this);
		}

		// Token: 0x060078EB RID: 30955 RVA: 0x0027BDAB File Offset: 0x00279FAB
		private void OnDisable()
		{
			if (ApplicationQuittingState.IsQuitting)
			{
				return;
			}
			VoxelManager.Unregister(this);
		}

		// Token: 0x060078EC RID: 30956 RVA: 0x0027BDBC File Offset: 0x00279FBC
		private void OnDestroy()
		{
			if (this.persistChanges)
			{
				this.SaveChunks();
			}
			foreach (Chunk chunk in this.chunks.Values)
			{
				chunk.Dispose();
			}
			if (this.sortedChunks.IsCreated)
			{
				this.sortedChunks.Dispose();
			}
			if (this.chunksToGenerate.IsCreated)
			{
				this.chunksToGenerate.Dispose();
			}
			this.sortedChunks = default(NativeList<int3>);
			this.chunksToGenerate = default(NativeHashSet<int3>);
		}

		// Token: 0x060078ED RID: 30957 RVA: 0x0027BE68 File Offset: 0x0027A068
		private void Update()
		{
			if (!this._updateWorld)
			{
				return;
			}
			if (this.generationQueueChanged)
			{
				this.sortJobHandle.Complete();
				this.sortedChunkCount = this.sortedChunks.Length;
				this.chunkSortIndex = 0;
				this.generationQueueChanged = false;
			}
			foreach (ChunkTaskSet chunkTaskSet in this.chunkJobs.Values)
			{
				if (chunkTaskSet.CompleteIfReady())
				{
					this.HandleJobCompletion(chunkTaskSet);
				}
			}
			foreach (int3 key in this.completedJobs)
			{
				this.chunkJobs.Remove(key);
			}
			this.completedJobs.Clear();
			using (Dictionary<int3, Chunk>.ValueCollection.Enumerator enumerator3 = this.chunks.Values.GetEnumerator())
			{
				while (enumerator3.MoveNext())
				{
					Chunk chunk = enumerator3.Current;
					if (chunk.IsDirty)
					{
						this.chunksToGenerate.Add(chunk.Id);
						this.generationQueueChanged = true;
					}
				}
				goto IL_155;
			}
			IL_11E:
			int num = this.chunkSortIndex;
			this.chunkSortIndex = num + 1;
			int3 @int = this.sortedChunks[num];
			this.chunksToGenerate.Remove(@int);
			this.ProcessChunk(@int);
			IL_155:
			if (this.chunkSortIndex >= this.sortedChunks.Length || this.chunkJobs.Count >= this.maxJobs)
			{
				this.UpdateVisibleChunks(false);
				return;
			}
			goto IL_11E;
		}

		// Token: 0x060078EE RID: 30958 RVA: 0x0027C020 File Offset: 0x0027A220
		private void SaveChunks()
		{
			Debug.Log("Saving chunks...");
			foreach (Chunk chunk in this.chunks.Values)
			{
				if (chunk.IsDataChanged)
				{
					ChunkIO.SaveChunk(new ChunkDTO(chunk));
				}
			}
		}

		// Token: 0x060078EF RID: 30959 RVA: 0x0027C090 File Offset: 0x0027A290
		private void ConfigurePools()
		{
			this._chunkPool = new UnityEngine.Pool.ObjectPool<Chunk>(() => new Chunk(int3.zero, this.ChunkSize, -1), delegate(Chunk chunk)
			{
			}, delegate(Chunk chunk)
			{
				if (chunk.Component)
				{
					this._chunkComponentPool.Release(chunk.Component);
					chunk.SetComponent(null);
				}
				chunk.Clear();
			}, delegate(Chunk chunk)
			{
				chunk.Dispose();
			}, true, 100, 100);
			this._chunkComponentPool = new UnityEngine.Pool.ObjectPool<ChunkComponent>(() => Object.Instantiate<ChunkComponent>(this.chunkPrefab), delegate(ChunkComponent chunkComponent)
			{
				chunkComponent.gameObject.SetActive(false);
				chunkComponent.transform.SetParent(base.transform, false);
			}, delegate(ChunkComponent chunkComponent)
			{
				if (chunkComponent.meshFilter.sharedMesh)
				{
					Mesh sharedMesh = chunkComponent.meshFilter.sharedMesh;
					chunkComponent.meshFilter.sharedMesh = null;
					chunkComponent.meshCollider.sharedMesh = null;
					this._meshPool.Release(sharedMesh);
				}
				chunkComponent.gameObject.SetActive(false);
			}, delegate(ChunkComponent chunkComponent)
			{
				if (chunkComponent)
				{
					Object.Destroy(chunkComponent.gameObject);
				}
			}, true, 100, 100);
			this._meshPool = new UnityEngine.Pool.ObjectPool<Mesh>(() => new Mesh(), null, delegate(Mesh mesh)
			{
				mesh.Clear(false);
			}, null, true, 100, 100);
		}

		// Token: 0x060078F0 RID: 30960 RVA: 0x0027C1A6 File Offset: 0x0027A3A6
		public bool TryGetChunk(int3 chunkId, out Chunk chunk)
		{
			return this.chunks.TryGetValue(chunkId, out chunk);
		}

		// Token: 0x060078F1 RID: 30961 RVA: 0x0027C1B5 File Offset: 0x0027A3B5
		private Chunk GetPooledChunk(int3 chunkId)
		{
			Chunk chunk = this._chunkPool.Get();
			chunk.World = this;
			chunk.Id = chunkId;
			return chunk;
		}

		// Token: 0x060078F2 RID: 30962 RVA: 0x0027C1D0 File Offset: 0x0027A3D0
		private Chunk CreateOrLoadChunk(int3 chunkId)
		{
			Chunk pooledChunk = this.GetPooledChunk(chunkId);
			ChunkDTO from;
			if (this.persistChanges && ChunkIO.TryLoadChunk(chunkId, out from))
			{
				pooledChunk.SetFrom(from);
			}
			else
			{
				pooledChunk.Id = chunkId;
			}
			return pooledChunk;
		}

		// Token: 0x060078F3 RID: 30963 RVA: 0x0027C208 File Offset: 0x0027A408
		public void SetChunkFrom(ChunkDTO dto)
		{
			Chunk pooledChunk;
			if (!this.chunks.TryGetValue(dto.Id, out pooledChunk))
			{
				pooledChunk = this.GetPooledChunk(dto.Id);
				this.chunks[dto.Id] = pooledChunk;
			}
			pooledChunk.SetFrom(dto);
		}

		// Token: 0x060078F4 RID: 30964 RVA: 0x0027C250 File Offset: 0x0027A450
		public void UpdateChunkFrom(ChunkDTO dto)
		{
			Chunk pooledChunk;
			if (!this.chunks.TryGetValue(dto.Id, out pooledChunk))
			{
				pooledChunk = this.GetPooledChunk(dto.Id);
				this.chunks[dto.Id] = pooledChunk;
				pooledChunk.SetFrom(dto);
				return;
			}
			pooledChunk.UpdateFrom(dto);
		}

		// Token: 0x060078F5 RID: 30965 RVA: 0x0027C2A0 File Offset: 0x0027A4A0
		private void Save(Chunk chunk)
		{
			if (chunk.IsDataChanged)
			{
				ChunkIO.SaveChunk(new ChunkDTO(chunk));
			}
		}

		// Token: 0x060078F6 RID: 30966 RVA: 0x0027C2B5 File Offset: 0x0027A4B5
		private void Unload(Chunk chunk)
		{
			if (this.persistChanges)
			{
				this.Save(chunk);
			}
			this._chunkPool.Release(chunk);
		}

		// Token: 0x060078F7 RID: 30967 RVA: 0x0027C2D4 File Offset: 0x0027A4D4
		private void UpdateVisibleChunks(bool isFirstTime = false)
		{
			int3 chunkIdForWorldPosition = this.GetChunkIdForWorldPosition(this.target.position);
			if (chunkIdForWorldPosition.Equals(this.playerChunk) && !this.generationQueueChanged)
			{
				return;
			}
			this.playerChunk = chunkIdForWorldPosition;
			this.generationQueueChanged = true;
			VoxelWorld.WorldType worldType = this.worldType;
			if (worldType != VoxelWorld.WorldType.Infinite)
			{
				if (worldType != VoxelWorld.WorldType.Bounded)
				{
					throw new ArgumentOutOfRangeException();
				}
				ValueTuple<int3, int3> chunkBoundsForLocalBounds = this.GetChunkBoundsForLocalBounds(this.worldBounds, false);
				int3 item = chunkBoundsForLocalBounds.Item1;
				int3 item2 = chunkBoundsForLocalBounds.Item2;
				for (int i = item.x; i <= item2.x; i++)
				{
					for (int j = item.y; j <= item2.y; j++)
					{
						for (int k = item.z; k <= item2.z; k++)
						{
							int3 @int = new int3(i, j, k);
							if (!this.chunks.ContainsKey(@int) && !this.chunksToGenerate.Contains(@int))
							{
								this.chunksToGenerate.Add(@int);
							}
						}
					}
				}
			}
			else
			{
				for (int l = -this.viewDistance; l <= this.viewDistance; l++)
				{
					for (int m = -this.viewDistance; m <= this.viewDistance; m++)
					{
						for (int n = -this.viewDistance; n <= this.viewDistance; n++)
						{
							int3 int2 = this.playerChunk + new int3(l, m, n);
							if (!this.chunks.ContainsKey(int2) && !this.chunksToGenerate.Contains(int2))
							{
								this.chunksToGenerate.Add(int2);
							}
						}
					}
				}
			}
			SortChunksJob jobData = new SortChunksJob
			{
				ChunkSet = this.chunksToGenerate,
				SortedChunks = this.sortedChunks
			};
			this.sortJobHandle = jobData.Schedule(default(JobHandle));
			if (this.worldType == VoxelWorld.WorldType.Infinite)
			{
				int num = this.viewDistance + 2;
				this.chunksToRemove.Clear();
				foreach (int3 int3 in this.chunks.Keys)
				{
					if (Mathf.Abs(int3.x - this.playerChunk.x) > num || Mathf.Abs(int3.y - this.playerChunk.y) > num || Mathf.Abs(int3.z - this.playerChunk.z) > num)
					{
						this.chunksToRemove.Add(int3);
					}
				}
				foreach (int3 key in this.chunksToRemove)
				{
					Chunk chunk;
					if (this.chunks.TryGetValue(key, out chunk))
					{
						ChunkTaskSet chunkTaskSet;
						if (this.chunkJobs.TryGetValue(key, out chunkTaskSet))
						{
							chunkTaskSet.Complete();
							this.chunkJobs.Remove(key);
						}
						this.Unload(chunk);
						this.chunks.Remove(key);
					}
				}
			}
		}

		// Token: 0x060078F8 RID: 30968 RVA: 0x0027C608 File Offset: 0x0027A808
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int3 GetChunkIdForWorldPosition(Vector3 worldPosition)
		{
			return this.GetLocalPosition(worldPosition).LocalPositionToChunkId(this.ChunkSize);
		}

		// Token: 0x060078F9 RID: 30969 RVA: 0x0027C61C File Offset: 0x0027A81C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int3 GetChunkIdForLocalPosition(Vector3 voxelWorldPosition)
		{
			return voxelWorldPosition.LocalPositionToChunkId(this.ChunkSize);
		}

		// Token: 0x060078FA RID: 30970 RVA: 0x0027C62A File Offset: 0x0027A82A
		public void SetWorldType(VoxelWorld.WorldType newWorldType, bool force = false)
		{
			if (!force && this.worldType == newWorldType)
			{
				return;
			}
			this.worldType = newWorldType;
			this.RegenerateAllChunks();
		}

		// Token: 0x060078FB RID: 30971 RVA: 0x0027C646 File Offset: 0x0027A846
		public void SetWorldBounds(UnityEngine.BoundsInt bounds)
		{
			this.worldBounds = bounds;
			this.SetWorldType(VoxelWorld.WorldType.Bounded, true);
		}

		// Token: 0x060078FC RID: 30972 RVA: 0x0027C658 File Offset: 0x0027A858
		public static void SaveWorld(Scene scene)
		{
			VoxelWorld @for = VoxelWorld.GetFor(scene);
			foreach (Chunk chunk in @for.chunks.Values)
			{
				@for.Save(chunk);
			}
		}

		// Token: 0x060078FD RID: 30973 RVA: 0x0027C6B8 File Offset: 0x0027A8B8
		public static void ResetWorld(Scene scene)
		{
			ChunkIO.DeleteWorld();
			VoxelWorld @for = VoxelWorld.GetFor(scene);
			if (@for)
			{
				@for.RegenerateAllChunks();
			}
		}

		// Token: 0x060078FE RID: 30974 RVA: 0x0027C6E0 File Offset: 0x0027A8E0
		private void RegenerateAllChunks()
		{
			if (!this.Initialized)
			{
				return;
			}
			foreach (ChunkTaskSet chunkTaskSet in this.chunkJobs.Values)
			{
				chunkTaskSet.Complete();
			}
			this.chunkJobs.Clear();
			this.completedJobs.Clear();
			this.chunksToGenerate.Clear();
			this.sortedChunks.Clear();
			foreach (Chunk chunk in this.chunks.Values)
			{
				chunk.Clear();
			}
			this.generationQueueChanged = true;
		}

		// Token: 0x060078FF RID: 30975 RVA: 0x0027C7B8 File Offset: 0x0027A9B8
		public void ResetChunk(int3 chunkId)
		{
			Chunk chunk;
			if (!this.chunks.TryGetValue(chunkId, out chunk))
			{
				return;
			}
			if (!chunk.IsDataChanged)
			{
				return;
			}
			ChunkTaskSet chunkTaskSet;
			if (this.chunkJobs.TryGetValue(chunkId, out chunkTaskSet))
			{
				chunkTaskSet.Complete();
				this.chunkJobs.Remove(chunkId);
			}
			chunk.IsDataGenerated = false;
			chunk.IsDirty = true;
		}

		// Token: 0x06007900 RID: 30976 RVA: 0x0027C810 File Offset: 0x0027AA10
		private void ProcessChunk(int3 chunkId)
		{
			Chunk chunk;
			if (!this.chunks.TryGetValue(chunkId, out chunk))
			{
				chunk = this.CreateOrLoadChunk(chunkId);
				this.chunks[chunkId] = chunk;
			}
			if (!chunk.IsDirty)
			{
				Debug.LogWarning(string.Format("{0} is not dirty, skipping processing", chunk));
				return;
			}
			chunk.IsDirty = false;
			ChunkTaskSet chunkTaskSet = new ChunkTaskSet(chunk, this.generationParameters, Array.Empty<ValueTuple<ChunkTaskSet.ChunkTaskDelegate, Action<Chunk>>>());
			ChunkState state = chunk.State;
			if (state < ChunkState.VoxelDataGenerated)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateVoxelDataJob), null);
			}
			if (state < ChunkState.MeshDataGenerated)
			{
				if (this.generationParameters.MeshGenerationMode == MeshGenerationMode.MarchingCubes)
				{
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateMeshDataJob), new Action<Chunk>(this.CreateChunkMesh));
				}
				else if (this.generationParameters.MeshGenerationMode == MeshGenerationMode.SurfaceNets)
				{
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateMeshDataJob), null);
					chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateSurfaceNetsPostProcessingJob), new Action<Chunk>(this.CreateChunkMesh));
				}
				else
				{
					Debug.LogError(string.Format("Unknown mesh generation mode: {0}", this.generationParameters.MeshGenerationMode));
				}
			}
			else if (state < ChunkState.MeshCreated)
			{
				chunkTaskSet.AddTask(null, new Action<Chunk>(this.CreateChunkMesh));
			}
			if (state < ChunkState.CollisionBaked)
			{
				chunkTaskSet.AddTask(new ChunkTaskSet.ChunkTaskDelegate(ChunkTask.CreateCollisionJob), new Action<Chunk>(this.AssignMesh));
			}
			else if (state < ChunkState.MeshAssigned)
			{
				chunkTaskSet.AddTask(null, new Action<Chunk>(this.AssignMesh));
			}
			if (!chunkTaskSet.IsEmpty)
			{
				this.chunkJobs.Add(chunk.Id, chunkTaskSet);
				chunkTaskSet.Start();
				return;
			}
			Debug.LogWarning(string.Format("{0} was dirty but nothing to do?", chunk));
		}

		// Token: 0x06007901 RID: 30977 RVA: 0x0027C9AC File Offset: 0x0027ABAC
		private void MeshChunkImmediately(Chunk chunk)
		{
			ChunkTask.CreateMeshDataJob(chunk, this.generationParameters).Complete();
			if (this.generationParameters.MeshGenerationMode == MeshGenerationMode.SurfaceNets)
			{
				ChunkTask.CreateSurfaceNetsPostProcessingJob(chunk, this.generationParameters).Complete();
			}
			chunk.Mesh = this.CreateMesh(chunk);
			if (chunk.Mesh)
			{
				ChunkTask.CreateCollisionJob(chunk, default(GenerationParameters)).Complete();
			}
			this.AssignMesh(chunk);
			this.chunkJobs.Remove(chunk.Id);
		}

		// Token: 0x06007902 RID: 30978 RVA: 0x0027CA39 File Offset: 0x0027AC39
		private void CreateChunkMesh(Chunk chunk)
		{
			chunk.Mesh = this.CreateMesh(chunk);
		}

		// Token: 0x06007903 RID: 30979 RVA: 0x0027CA48 File Offset: 0x0027AC48
		private Mesh CreateMesh(Chunk chunk)
		{
			if (chunk.VertexCount == 0)
			{
				chunk.DisposeMeshData();
				chunk.IsMeshCreated = true;
				return null;
			}
			int vertexCount = chunk.VertexCount;
			Mesh mesh = this._meshPool.Get();
			if (vertexCount > chunk.VertexData.Length)
			{
				Debug.LogError(string.Format("Vertex count {0} exceeds allocated vertex data length {1} for chunk {2}. This is likely a bug in the meshing job.", vertexCount, chunk.VertexData.Length, chunk.Id));
				return null;
			}
			mesh.SetVertexBufferParams(vertexCount, MeshVertexData.VertexBufferMemoryLayout);
			mesh.SetIndexBufferParams(vertexCount, IndexFormat.UInt16);
			mesh.SetVertexBufferData<MeshVertexData>(chunk.VertexData, 0, 0, vertexCount, 0, MeshUpdateFlags.DontValidateIndices);
			mesh.SetIndexBufferData<ushort>(chunk.TriangleData, 0, 0, vertexCount, MeshUpdateFlags.DontValidateIndices);
			mesh.subMeshCount = 1;
			mesh.SetSubMesh(0, new SubMeshDescriptor(0, vertexCount, MeshTopology.Triangles), MeshUpdateFlags.Default);
			mesh.RecalculateBounds();
			chunk.DisposeMeshData();
			chunk.IsMeshCreated = true;
			return mesh;
		}

		// Token: 0x06007904 RID: 30980 RVA: 0x0027CB20 File Offset: 0x0027AD20
		private void AssignMesh(Chunk chunk)
		{
			Mesh mesh = chunk.Mesh;
			if (mesh)
			{
				if (!chunk.Component)
				{
					ChunkComponent chunkComponent = this._chunkComponentPool.Get();
					chunkComponent.transform.SetParent(base.transform, false);
					chunkComponent.transform.localScale = Vector3.one * this.worldScale;
					chunkComponent.transform.localPosition = (chunk.Id * this.ChunkSize).ToVector3() * this.worldScale;
					chunk.SetComponent(chunkComponent);
				}
				chunk.MeshFilter.sharedMesh = mesh;
				chunk.MeshCollider.sharedMesh = mesh;
				chunk.GameObject.SetActive(true);
			}
			else if (chunk.Component)
			{
				this._chunkComponentPool.Release(chunk.Component);
				chunk.SetComponent(null);
			}
			chunk.IsMeshAssigned = true;
			chunk.IsDirty = false;
		}

		// Token: 0x06007905 RID: 30981 RVA: 0x0027CC14 File Offset: 0x0027AE14
		public void SetVoxelDensityCustom(UnityEngine.BoundsInt worldBounds, Func<int3, byte, byte> setDensityFunction, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass102_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass102_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.setDensityFunction = setDensityFunction;
			this.ForEachChunkInBounds(worldBounds, new Action(CS$<>8__locals1.<SetVoxelDensityCustom>g__SetVoxelDensityInChunk|0));
		}

		// Token: 0x06007906 RID: 30982 RVA: 0x0027CC50 File Offset: 0x0027AE50
		public void SetVoxelDataCustom(UnityEngine.BoundsInt worldBounds, [TupleElementNames(new string[]
		{
			"density",
			"material",
			"density",
			"material"
		})] Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> setDataFunction, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass103_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass103_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.setDataFunction = setDataFunction;
			this.ForEachChunkInBounds(worldBounds, new Action(CS$<>8__locals1.<SetVoxelDataCustom>g__SetVoxelDataInChunk|0));
		}

		// Token: 0x06007907 RID: 30983 RVA: 0x0027CC8C File Offset: 0x0027AE8C
		public void SetVoxelDataCustom(int3[] voxels, [TupleElementNames(new string[]
		{
			"density",
			"material",
			"density",
			"material"
		})] Func<int3, ValueTuple<byte, byte>, ValueTuple<byte, byte>> setDataFunction, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass104_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass104_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.voxels = voxels;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.setDataFunction = setDataFunction;
			UnityEngine.BoundsInt boundsFor = VoxelWorld.GetBoundsFor(CS$<>8__locals1.voxels);
			this.ForEachChunkInBounds(boundsFor, new Action(CS$<>8__locals1.<SetVoxelDataCustom>g__SetVoxelDataInChunk|0));
		}

		// Token: 0x06007908 RID: 30984 RVA: 0x0027CCDC File Offset: 0x0027AEDC
		public void SetVoxels(UnityEngine.BoundsInt bounds, Voxel[] voxels, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass105_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass105_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.voxels = voxels;
			bounds.GetVoxelCount();
			this.ForEachChunkInBounds(bounds, new Action(CS$<>8__locals1.<SetVoxels>g__SetVoxelDataInChunk|0));
		}

		// Token: 0x06007909 RID: 30985 RVA: 0x0027CD20 File Offset: 0x0027AF20
		public void SetVoxelDensity(UnityEngine.BoundsInt bounds, byte[] data, bool immediate = true)
		{
			VoxelWorld.<>c__DisplayClass106_0 CS$<>8__locals1 = new VoxelWorld.<>c__DisplayClass106_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.immediate = immediate;
			CS$<>8__locals1.data = data;
			this.ForEachChunkInBounds(bounds, new Action(CS$<>8__locals1.<SetVoxelDensity>g__SetVoxelDensityInChunk|0));
		}

		// Token: 0x0600790A RID: 30986 RVA: 0x0027CD5C File Offset: 0x0027AF5C
		public byte GetVoxelMaterial(int3 voxelId)
		{
			int3 key = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(key, out chunk))
			{
				return 0;
			}
			int3 @int = voxelId - chunk.Id * chunk.Size;
			int index = @int.x + this.VoxelDimension * (@int.y + this.VoxelDimension * @int.z);
			return chunk.Material[index];
		}

		// Token: 0x0600790B RID: 30987 RVA: 0x0027CDD0 File Offset: 0x0027AFD0
		public byte GetVoxelDensity(int3 voxelId)
		{
			int3 key = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(key, out chunk))
			{
				return 0;
			}
			int3 @int = voxelId - chunk.Id * chunk.Size;
			int index = @int.x + this.VoxelDimension * (@int.y + this.VoxelDimension * @int.z);
			return chunk.Density[index];
		}

		// Token: 0x0600790C RID: 30988 RVA: 0x0027CE44 File Offset: 0x0027B044
		public Voxel GetVoxelData(int3 voxelId)
		{
			int3 key = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(key, out chunk))
			{
				return default(Voxel);
			}
			int3 @int = voxelId - chunk.Id * chunk.Size;
			int index = @int.x + this.VoxelDimension * (@int.y + this.VoxelDimension * @int.z);
			return new Voxel(chunk.Material[index], chunk.Density[index]);
		}

		// Token: 0x0600790D RID: 30989 RVA: 0x0027CED4 File Offset: 0x0027B0D4
		public void SetVoxelMaterial(int3 voxelId, byte material)
		{
			int3 key = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(key, out chunk))
			{
				return;
			}
			int3 @int = voxelId - chunk.Id * chunk.Size;
			int index = @int.x + this.VoxelDimension * (@int.y + this.VoxelDimension * @int.z);
			chunk.Material[index] = material;
		}

		// Token: 0x0600790E RID: 30990 RVA: 0x0027CF48 File Offset: 0x0027B148
		public void SetVoxelDensity(int3 voxelId, byte density)
		{
			int3 key = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(key, out chunk))
			{
				return;
			}
			int3 @int = voxelId - chunk.Id * chunk.Size;
			int index = @int.x + this.VoxelDimension * (@int.y + this.VoxelDimension * @int.z);
			chunk.Density[index] = density;
		}

		// Token: 0x0600790F RID: 30991 RVA: 0x0027CFBC File Offset: 0x0027B1BC
		public void SetVoxelData(int3 voxelId, Voxel data)
		{
			int3 key = voxelId.LocalPositionToChunkId(this.ChunkSize);
			Chunk chunk;
			if (!this.chunks.TryGetValue(key, out chunk))
			{
				return;
			}
			int3 @int = voxelId - chunk.Id * chunk.Size;
			int index = @int.x + this.VoxelDimension * (@int.y + this.VoxelDimension * @int.z);
			chunk.Material[index] = data.Material;
			chunk.Density[index] = data.Density;
		}

		// Token: 0x06007910 RID: 30992 RVA: 0x0027D048 File Offset: 0x0027B248
		public static UnityEngine.BoundsInt GetBoundsFor(int3[] voxels)
		{
			int3 @int = new int3(int.MaxValue, int.MaxValue, int.MaxValue);
			int3 int2 = new int3(int.MinValue, int.MinValue, int.MinValue);
			foreach (int3 x in voxels)
			{
				@int = math.min(x, @int);
				int2 = math.max(x, int2);
			}
			return new UnityEngine.BoundsInt
			{
				min = @int.ToVectorInt(),
				max = int2.ToVectorInt()
			};
		}

		// Token: 0x06007911 RID: 30993 RVA: 0x0027D0CC File Offset: 0x0027B2CC
		[return: TupleElementNames(new string[]
		{
			"min",
			"max"
		})]
		public ValueTuple<int3, int3> GetChunkBoundsForLocalBounds(UnityEngine.BoundsInt worldBounds, bool includeLLC = false)
		{
			return new ValueTuple<int3, int3>((worldBounds.min - (includeLLC ? (Vector3Int.one * Chunk.Pad) : Vector3Int.zero)).LocalPositionToChunkId(this.ChunkSize), worldBounds.max.LocalPositionToChunkId(this.ChunkSize));
		}

		// Token: 0x06007912 RID: 30994 RVA: 0x0027D120 File Offset: 0x0027B320
		public bool BoundsChunksLoaded(UnityEngine.BoundsInt localWorldBounds, bool includeLLC = false)
		{
			if (!this.Initialized)
			{
				return false;
			}
			ValueTuple<int3, int3> chunkBoundsForLocalBounds = this.GetChunkBoundsForLocalBounds(localWorldBounds, includeLLC);
			int3 item = chunkBoundsForLocalBounds.Item1;
			int3 item2 = chunkBoundsForLocalBounds.Item2;
			for (int i = item.x; i <= item2.x; i++)
			{
				for (int j = item.y; j <= item2.y; j++)
				{
					for (int k = item.z; k <= item2.z; k++)
					{
						int3 key = new int3(i, j, k);
						Chunk chunk;
						if (!this.chunks.TryGetValue(key, out chunk))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		// Token: 0x06007913 RID: 30995 RVA: 0x0027D1B1 File Offset: 0x0027B3B1
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Vector3Int ClampToWorldBounds(Vector3Int coord)
		{
			if (this.worldType == VoxelWorld.WorldType.Bounded)
			{
				coord.Clamp(this.worldBounds.min, this.worldBounds.max);
			}
			return coord;
		}

		// Token: 0x06007914 RID: 30996 RVA: 0x0027D1DC File Offset: 0x0027B3DC
		private void ForEachChunkInBounds(UnityEngine.BoundsInt bounds, Action action)
		{
			VoxelWorld._opBounds = bounds;
			int3 @int = this.ClampToWorldBounds(bounds.min - Vector3Int.one * Chunk.Pad).LocalPositionToChunkId(this.ChunkSize);
			int3 int2 = this.ClampToWorldBounds(bounds.max).LocalPositionToChunkId(this.ChunkSize);
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int3 = new int3(i, j, k);
						Chunk opChunk;
						if (this.chunks.TryGetValue(int3, out opChunk))
						{
							VoxelWorld._opChunk = opChunk;
							action();
						}
						else
						{
							Debug.LogError(string.Format("Couldn't find chunk {0} to perform operation", int3));
						}
					}
				}
			}
		}

		// Token: 0x06007915 RID: 30997 RVA: 0x0027D2BC File Offset: 0x0027B4BC
		public void GetChunksForBounds(UnityEngine.BoundsInt worldBounds, ref List<Chunk> list)
		{
			if (list == null)
			{
				list = new List<Chunk>();
			}
			list.Clear();
			int3 @int = this.ClampToWorldBounds(worldBounds.min - Vector3Int.one * Chunk.Pad).LocalPositionToChunkId(this.ChunkSize);
			int3 int2 = this.ClampToWorldBounds(worldBounds.max).LocalPositionToChunkId(this.ChunkSize);
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 key = new int3(i, j, k);
						Chunk item;
						if (this.chunks.TryGetValue(key, out item))
						{
							list.Add(item);
						}
					}
				}
			}
		}

		// Token: 0x06007916 RID: 30998 RVA: 0x0027D38C File Offset: 0x0027B58C
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk GetChunkForLocalPosition(int3 worldPosition)
		{
			return this.chunks.GetValueOrDefault(worldPosition.LocalPositionToChunkId(this.ChunkSize));
		}

		// Token: 0x06007917 RID: 30999 RVA: 0x0027D3A5 File Offset: 0x0027B5A5
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Chunk GetChunkForLocalPosition(Vector3 worldPosition)
		{
			return this.chunks.GetValueOrDefault(worldPosition.LocalPositionToChunkId(this.ChunkSize));
		}

		// Token: 0x06007918 RID: 31000 RVA: 0x0027D3C0 File Offset: 0x0027B5C0
		private void ForEachVoxelInChunkInBounds(UnityEngine.BoundsInt worldBounds, Chunk chunk, Action<int3, int3, int, byte> action)
		{
			int3 @int = new int3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
			int3 int2 = new int3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);
			int3 int3 = chunk.Id * chunk.Size;
			int3 int4 = int3 + chunk.Dimensions - 1;
			@int.x = math.max(@int.x, int3.x);
			@int.y = math.max(@int.y, int3.y);
			@int.z = math.max(@int.z, int3.z);
			int2.x = math.min(int2.x, int4.x);
			int2.y = math.min(int2.y, int4.y);
			int2.z = math.min(int2.z, int4.z);
			if (@int.x > int2.x || @int.y > int2.y || @int.z > int2.z)
			{
				Debug.LogWarning(string.Format("No overlap between chunk {0} and bounds {1}", chunk.Id, worldBounds));
				return;
			}
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int5 = new int3(i, j, k);
						int3 int6 = int5 - chunk.Id * chunk.Size;
						int num = int6.x + this.VoxelDimension * (int6.y + this.VoxelDimension * int6.z);
						byte arg = chunk.Density[num];
						action(int5, int6, num, arg);
					}
				}
			}
		}

		// Token: 0x06007919 RID: 31001 RVA: 0x0027D600 File Offset: 0x0027B800
		private void ForEachVoxelInChunkInBounds(UnityEngine.BoundsInt worldBounds, Chunk chunk, Action<int3, int3, int, byte, byte> action)
		{
			int3 @int = new int3(worldBounds.min.x, worldBounds.min.y, worldBounds.min.z);
			int3 int2 = new int3(worldBounds.max.x, worldBounds.max.y, worldBounds.max.z);
			int3 int3 = chunk.Id * chunk.Size;
			int3 int4 = int3 + chunk.Dimensions - 1;
			@int.x = math.max(@int.x, int3.x);
			@int.y = math.max(@int.y, int3.y);
			@int.z = math.max(@int.z, int3.z);
			int2.x = math.min(int2.x, int4.x);
			int2.y = math.min(int2.y, int4.y);
			int2.z = math.min(int2.z, int4.z);
			if (@int.x > int2.x || @int.y > int2.y || @int.z > int2.z)
			{
				Debug.LogWarning(string.Format("No overlap between chunk {0} and bounds {1}", chunk.Id, worldBounds));
				return;
			}
			for (int i = @int.x; i <= int2.x; i++)
			{
				for (int j = @int.y; j <= int2.y; j++)
				{
					for (int k = @int.z; k <= int2.z; k++)
					{
						int3 int5 = new int3(i, j, k);
						int3 int6 = int5 - chunk.Id * chunk.Size;
						int num = int6.x + this.VoxelDimension * (int6.y + this.VoxelDimension * int6.z);
						byte arg = chunk.Density[num];
						byte arg2 = chunk.Material[num];
						action(int5, int6, num, arg, arg2);
					}
				}
			}
		}

		// Token: 0x0600791A RID: 31002 RVA: 0x0027D858 File Offset: 0x0027BA58
		private void ForEachSpecifiedVoxelInChunk(int3[] voxels, Chunk chunk, Action<int3, int3, int, byte, byte> action)
		{
			int3 @int = chunk.Id * chunk.Size;
			int3 max = @int + chunk.Dimensions - 1;
			foreach (int3 int2 in voxels)
			{
				if (int2.IsInBounds(@int, max))
				{
					int3 int3 = int2 - chunk.Id * chunk.Size;
					int num = int3.x + this.VoxelDimension * (int3.y + this.VoxelDimension * int3.z);
					byte arg = chunk.Density[num];
					byte arg2 = chunk.Material[num];
					action(int2, int3, num, arg, arg2);
				}
			}
		}

		// Token: 0x0600791B RID: 31003 RVA: 0x0027D924 File Offset: 0x0027BB24
		private void HandleJobCompletion(ChunkTaskSet chunkTask)
		{
			if (!chunkTask.HasChunks)
			{
				Debug.LogError("Chunk is null in HandleJobCompletion");
			}
			Chunk chunk = chunkTask.Chunk;
			if (chunk.State < ChunkState.MeshAssigned || chunk.IsDirty)
			{
				Debug.LogWarning(string.Format("{0} job completed with state {1} and dirty {2}", chunk, chunk.State, chunk.IsDirty));
			}
			this.completedJobs.Add(chunkTask.Chunk.Id);
		}

		// Token: 0x0600791C RID: 31004 RVA: 0x0027D997 File Offset: 0x0027BB97
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public byte GetDensityAt(Vector3 voxelWorldPosition)
		{
			return this.GetDensityAt(voxelWorldPosition.ToInt3(), 0);
		}

		// Token: 0x0600791D RID: 31005 RVA: 0x0027D9A8 File Offset: 0x0027BBA8
		public byte GetDensityAt(int3 voxelWorldPosition, byte defaultDensity = 0)
		{
			Chunk chunkForLocalPosition = this.GetChunkForLocalPosition(voxelWorldPosition);
			if (chunkForLocalPosition != null && chunkForLocalPosition.IsDataGenerated)
			{
				int3 localPosition = chunkForLocalPosition.GetLocalPosition(voxelWorldPosition);
				return chunkForLocalPosition.Density[localPosition.x + this.VoxelDimension * (localPosition.y + this.VoxelDimension * localPosition.z)];
			}
			return defaultDensity;
		}

		// Token: 0x0600791E RID: 31006 RVA: 0x0027D9FF File Offset: 0x0027BBFF
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetDensityAt(Vector3 voxelWorldPosition, byte density)
		{
			this.SetDensityAt(voxelWorldPosition.ToInt3(), density);
		}

		// Token: 0x0600791F RID: 31007 RVA: 0x0027DA10 File Offset: 0x0027BC10
		public void SetDensityAt(int3 voxelWorldPosition, byte density)
		{
			Chunk chunkForLocalPosition = this.GetChunkForLocalPosition(voxelWorldPosition);
			if (chunkForLocalPosition != null && chunkForLocalPosition.IsDataGenerated)
			{
				int3 localPosition = chunkForLocalPosition.GetLocalPosition(voxelWorldPosition);
				int index = localPosition.x + this.VoxelDimension * (localPosition.y + this.VoxelDimension * localPosition.z);
				if (chunkForLocalPosition.Density[index] != density)
				{
					chunkForLocalPosition.Density[index] = density;
					chunkForLocalPosition.IsDataChanged = true;
					chunkForLocalPosition.IsMeshCreated = false;
					chunkForLocalPosition.IsDirty = true;
					return;
				}
			}
			else
			{
				Debug.LogWarning(string.Format("No chunk found for world position {0}, cannot set density.", voxelWorldPosition));
			}
		}

		// Token: 0x06007920 RID: 31008 RVA: 0x0027DAA4 File Offset: 0x0027BCA4
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetLocalPosition(Vector3 worldPosition)
		{
			return Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one * this.worldScale).inverse.MultiplyPoint(worldPosition);
		}

		// Token: 0x06007921 RID: 31009 RVA: 0x0027DAF0 File Offset: 0x0027BCF0
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldPosition(Vector3 localPosition)
		{
			return Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one * this.worldScale).MultiplyPoint(localPosition);
		}

		// Token: 0x06007922 RID: 31010 RVA: 0x0027DB31 File Offset: 0x0027BD31
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetWorldPosition(int3 localPosition)
		{
			return this.GetWorldPosition(localPosition.ToVector3());
		}

		// Token: 0x06007923 RID: 31011 RVA: 0x0027DB3F File Offset: 0x0027BD3F
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetVoxelForWorldPosition(Vector3 worldPosition)
		{
			return this.GetLocalPosition(worldPosition).RoundToInt();
		}

		// Token: 0x06007924 RID: 31012 RVA: 0x0027DB4D File Offset: 0x0027BD4D
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetVoxelForLocalPosition(Vector3 localPosition)
		{
			return localPosition.RoundToInt();
		}

		// Token: 0x17000BAA RID: 2986
		// (get) Token: 0x06007925 RID: 31013 RVA: 0x0027DB55 File Offset: 0x0027BD55
		public float Scale
		{
			get
			{
				return this.worldScale;
			}
		}

		// Token: 0x04008BEA RID: 35818
		private static readonly Dictionary<int, VoxelWorld> WorldLookup = new Dictionary<int, VoxelWorld>();

		// Token: 0x04008BEB RID: 35819
		[Header("World Settings")]
		public GenerationParameters generationParameters = new GenerationParameters
		{
			NoiseScale = 0.01f,
			GroundLevel = 0f,
			HeightScale = 0.01f,
			Octaves = 4,
			Persistence = 0.5f,
			IsoLevel = 0f,
			Seed = 12345,
			NormalThreshold = 60f,
			AreaWeightedNormals = true
		};

		// Token: 0x04008BEC RID: 35820
		[SerializeField]
		private VoxelWorld.WorldType worldType;

		// Token: 0x04008BED RID: 35821
		[SerializeField]
		private UnityEngine.BoundsInt worldBounds;

		// Token: 0x04008BEE RID: 35822
		[SerializeField]
		private float worldScale = 1f;

		// Token: 0x04008BEF RID: 35823
		[SerializeField]
		private int chunkSize = 32;

		// Token: 0x04008BF0 RID: 35824
		[SerializeField]
		private int viewDistance = 5;

		// Token: 0x04008BF1 RID: 35825
		[SerializeField]
		private int maxJobs = 10;

		// Token: 0x04008BF2 RID: 35826
		[SerializeField]
		private bool registerAsSceneWorld = true;

		// Token: 0x04008BF3 RID: 35827
		[SerializeField]
		private bool persistChanges = true;

		// Token: 0x04008BF4 RID: 35828
		[Header("References")]
		public ChunkComponent chunkPrefab;

		// Token: 0x04008BF5 RID: 35829
		public Transform target;

		// Token: 0x04008BF6 RID: 35830
		protected Dictionary<int3, Chunk> chunks = new Dictionary<int3, Chunk>();

		// Token: 0x04008BF7 RID: 35831
		private Dictionary<int3, ChunkTaskSet> chunkJobs = new Dictionary<int3, ChunkTaskSet>();

		// Token: 0x04008BF8 RID: 35832
		private List<int3> completedJobs = new List<int3>();

		// Token: 0x04008BF9 RID: 35833
		protected NativeHashSet<int3> chunksToGenerate;

		// Token: 0x04008BFA RID: 35834
		protected NativeList<int3> sortedChunks;

		// Token: 0x04008BFB RID: 35835
		protected int chunkSortIndex;

		// Token: 0x04008BFC RID: 35836
		protected JobHandle sortJobHandle;

		// Token: 0x04008BFD RID: 35837
		protected int sortedChunkCount;

		// Token: 0x04008BFE RID: 35838
		protected int3 playerChunk = new int3(int.MaxValue, int.MaxValue, int.MaxValue);

		// Token: 0x04008BFF RID: 35839
		protected bool generationQueueChanged;

		// Token: 0x04008C00 RID: 35840
		private List<int3> chunksToRemove = new List<int3>();

		// Token: 0x04008C01 RID: 35841
		private UnityEngine.Pool.ObjectPool<Chunk> _chunkPool;

		// Token: 0x04008C02 RID: 35842
		private UnityEngine.Pool.ObjectPool<ChunkComponent> _chunkComponentPool;

		// Token: 0x04008C03 RID: 35843
		private UnityEngine.Pool.ObjectPool<Mesh> _meshPool;

		// Token: 0x04008C06 RID: 35846
		private bool _updateWorld = true;

		// Token: 0x04008C0A RID: 35850
		private static UnityEngine.BoundsInt _opBounds;

		// Token: 0x04008C0B RID: 35851
		private static Chunk _opChunk;

		// Token: 0x04008C0C RID: 35852
		private static bool _opAnyChanged;

		// Token: 0x020012F1 RID: 4849
		public enum WorldType
		{
			// Token: 0x04008C0E RID: 35854
			Infinite,
			// Token: 0x04008C0F RID: 35855
			Bounded
		}
	}
}
