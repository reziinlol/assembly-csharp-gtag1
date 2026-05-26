using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Voxels
{
	// Token: 0x020012C6 RID: 4806
	[Serializable]
	public class Chunk : IDisposable
	{
		// Token: 0x17000B8D RID: 2957
		// (get) Token: 0x0600781D RID: 30749 RVA: 0x002770F6 File Offset: 0x002752F6
		// (set) Token: 0x0600781E RID: 30750 RVA: 0x002770FE File Offset: 0x002752FE
		public ChunkComponent Component { get; private set; }

		// Token: 0x17000B8E RID: 2958
		// (get) Token: 0x0600781F RID: 30751 RVA: 0x00277107 File Offset: 0x00275307
		// (set) Token: 0x06007820 RID: 30752 RVA: 0x0027710F File Offset: 0x0027530F
		public GameObject GameObject { get; private set; }

		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x06007821 RID: 30753 RVA: 0x00277118 File Offset: 0x00275318
		// (set) Token: 0x06007822 RID: 30754 RVA: 0x00277120 File Offset: 0x00275320
		public MeshFilter MeshFilter { get; private set; }

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x06007823 RID: 30755 RVA: 0x00277129 File Offset: 0x00275329
		// (set) Token: 0x06007824 RID: 30756 RVA: 0x00277131 File Offset: 0x00275331
		public MeshRenderer MeshRenderer { get; private set; }

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x06007825 RID: 30757 RVA: 0x0027713A File Offset: 0x0027533A
		// (set) Token: 0x06007826 RID: 30758 RVA: 0x00277142 File Offset: 0x00275342
		public MeshCollider MeshCollider { get; private set; }

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x06007827 RID: 30759 RVA: 0x0027714B File Offset: 0x0027534B
		// (set) Token: 0x06007828 RID: 30760 RVA: 0x00277152 File Offset: 0x00275352
		public static int3 DefaultSize { get; set; } = 32;

		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x06007829 RID: 30761 RVA: 0x0027715A File Offset: 0x0027535A
		// (set) Token: 0x0600782A RID: 30762 RVA: 0x00277161 File Offset: 0x00275361
		public static int Pad { get; set; } = 1;

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x0600782B RID: 30763 RVA: 0x00277169 File Offset: 0x00275369
		public ChunkState State
		{
			get
			{
				if (!this.IsDataGenerated)
				{
					return ChunkState.Created;
				}
				if (!this.IsMeshGenerated)
				{
					return ChunkState.VoxelDataGenerated;
				}
				if (!this.IsMeshCreated)
				{
					return ChunkState.MeshDataGenerated;
				}
				if (!this.IsCollisionBaked)
				{
					return ChunkState.MeshCreated;
				}
				if (!this.IsMeshAssigned)
				{
					return ChunkState.CollisionBaked;
				}
				return ChunkState.MeshAssigned;
			}
		}

		// Token: 0x0600782C RID: 30764 RVA: 0x002771A0 File Offset: 0x002753A0
		public Chunk(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.VertexData = default(NativeArray<MeshVertexData>);
			this.TriangleData = default(NativeArray<ushort>);
			this.GenericMeshData = null;
		}

		// Token: 0x0600782D RID: 30765 RVA: 0x00277240 File Offset: 0x00275440
		public Chunk(int3 id, int3 size, int padding = -1)
		{
			if (padding < 0)
			{
				padding = Chunk.Pad;
			}
			this.Id = id;
			this.Size = size;
			this.Dimensions = size + padding;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Density = default(NativeArray<byte>);
			this.Material = default(NativeArray<byte>);
			this.VertexData = default(NativeArray<MeshVertexData>);
			this.TriangleData = default(NativeArray<ushort>);
			this.GenericMeshData = null;
		}

		// Token: 0x0600782E RID: 30766 RVA: 0x002772E0 File Offset: 0x002754E0
		public void SetFrom(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.Dispose();
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.IsDataGenerated = true;
			this.IsDataChanged = true;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		// Token: 0x0600782F RID: 30767 RVA: 0x00277398 File Offset: 0x00275598
		public void UpdateFrom(ChunkDTO dto)
		{
			this.Id = dto.Id;
			this.Size = dto.Size;
			this.Dimensions = dto.Dimensions;
			this.VoxelCount = this.Dimensions.x * this.Dimensions.y * this.Dimensions.z;
			this.DisposeAllExceptComponent();
			this.Density = dto.Density;
			this.Material = dto.Material;
			this.IsDataGenerated = true;
			this.IsDataChanged = true;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		// Token: 0x06007830 RID: 30768 RVA: 0x00277450 File Offset: 0x00275650
		public void Clear()
		{
			this.DisposeMeshData();
			this.IsDataGenerated = false;
			this.IsDataChanged = false;
			this.IsMeshGenerated = false;
			this.IsMeshCreated = false;
			this.IsCollisionBaked = false;
			this.IsMeshAssigned = false;
			this.IsDirty = true;
			this.VertexCount = 0;
			this.Mesh = null;
		}

		// Token: 0x06007831 RID: 30769 RVA: 0x002774A4 File Offset: 0x002756A4
		public void SetComponent(ChunkComponent chunkComponent)
		{
			this.Component = chunkComponent;
			if (chunkComponent)
			{
				this.GameObject = chunkComponent.gameObject;
				this.MeshFilter = chunkComponent.meshFilter;
				this.MeshRenderer = chunkComponent.meshRenderer;
				this.MeshCollider = chunkComponent.meshCollider;
				this.Component.name = Chunk.GetChunkName(this.Id);
				this.Component.World = this.World;
				return;
			}
			this.GameObject = null;
			this.MeshFilter = null;
			this.MeshRenderer = null;
			this.MeshCollider = null;
		}

		// Token: 0x06007832 RID: 30770 RVA: 0x00277534 File Offset: 0x00275734
		public void Dispose()
		{
			if (this.Density.IsCreated)
			{
				this.Density.Dispose();
				this.Density = default(NativeArray<byte>);
			}
			if (this.Material.IsCreated)
			{
				this.Material.Dispose();
				this.Material = default(NativeArray<byte>);
			}
			this.DisposeMeshData();
			if (this.Component)
			{
				Object.Destroy(this.Component.gameObject);
			}
		}

		// Token: 0x06007833 RID: 30771 RVA: 0x002775AC File Offset: 0x002757AC
		public void DisposeAllExceptComponent()
		{
			if (this.Density.IsCreated)
			{
				this.Density.Dispose();
				this.Density = default(NativeArray<byte>);
			}
			if (this.Material.IsCreated)
			{
				this.Material.Dispose();
				this.Material = default(NativeArray<byte>);
			}
			this.DisposeMeshData();
		}

		// Token: 0x06007834 RID: 30772 RVA: 0x00277608 File Offset: 0x00275808
		public void AllocateVertexData(int length)
		{
			if (this.VertexData.IsCreated)
			{
				if (this.VertexData.Length == length)
				{
					return;
				}
				NativeArrayPool<MeshVertexData>.Return(this.VertexData);
				this.VertexData = default(NativeArray<MeshVertexData>);
			}
			this.VertexData = NativeArrayPool<MeshVertexData>.Get(length);
		}

		// Token: 0x06007835 RID: 30773 RVA: 0x00277654 File Offset: 0x00275854
		public void AllocateTriangleData(int length)
		{
			if (this.TriangleData.IsCreated)
			{
				if (this.TriangleData.Length == length)
				{
					return;
				}
				NativeArrayPool<ushort>.Return(this.TriangleData);
				this.TriangleData = default(NativeArray<ushort>);
			}
			this.TriangleData = NativeArrayPool<ushort>.Get(length);
		}

		// Token: 0x06007836 RID: 30774 RVA: 0x002776A0 File Offset: 0x002758A0
		public void DisposeMeshData()
		{
			if (this.VertexData.IsCreated)
			{
				NativeArrayPool<MeshVertexData>.Return(this.VertexData);
				this.VertexData = default(NativeArray<MeshVertexData>);
			}
			if (this.TriangleData.IsCreated)
			{
				NativeArrayPool<ushort>.Return(this.TriangleData);
				this.TriangleData = default(NativeArray<ushort>);
			}
			IDisposable disposable = this.GenericMeshData as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
				this.GenericMeshData = null;
			}
		}

		// Token: 0x06007837 RID: 30775 RVA: 0x00277711 File Offset: 0x00275911
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int3 GetLocalPosition(int3 voxelPosition)
		{
			return voxelPosition - this.Id * this.Size;
		}

		// Token: 0x06007838 RID: 30776 RVA: 0x0027772C File Offset: 0x0027592C
		public override string ToString()
		{
			return string.Format("Chunk ({0}, {1}, {2}) [{3}{4}{5}{6}{7}{8}]", new object[]
			{
				this.Id.x,
				this.Id.y,
				this.Id.z,
				this.IsDataGenerated ? "D" : "_",
				this.IsDataChanged ? "C" : "_",
				this.IsMeshGenerated ? "G" : "_",
				this.IsMeshCreated ? "M" : "_",
				this.IsCollisionBaked ? "B" : "_",
				this.IsMeshAssigned ? "A" : "_"
			});
		}

		// Token: 0x06007839 RID: 30777 RVA: 0x0027780D File Offset: 0x00275A0D
		public static string GetChunkName(int3 id)
		{
			return string.Format("Chunk_{0}_{1}_{2}", id.x, id.y, id.z);
		}

		// Token: 0x04008B26 RID: 35622
		public VoxelWorld World;

		// Token: 0x04008B27 RID: 35623
		public int3 Id;

		// Token: 0x04008B28 RID: 35624
		public int3 Size;

		// Token: 0x04008B29 RID: 35625
		public int3 Dimensions;

		// Token: 0x04008B2A RID: 35626
		public int VoxelCount;

		// Token: 0x04008B2B RID: 35627
		public NativeArray<byte> Density;

		// Token: 0x04008B2C RID: 35628
		public NativeArray<byte> Material;

		// Token: 0x04008B2D RID: 35629
		public NativeArray<MeshVertexData> VertexData;

		// Token: 0x04008B2E RID: 35630
		public NativeArray<ushort> TriangleData;

		// Token: 0x04008B2F RID: 35631
		public object GenericMeshData;

		// Token: 0x04008B30 RID: 35632
		public bool IsDataGenerated;

		// Token: 0x04008B31 RID: 35633
		public bool IsDataChanged;

		// Token: 0x04008B32 RID: 35634
		public bool IsMeshGenerated;

		// Token: 0x04008B33 RID: 35635
		public bool IsMeshCreated;

		// Token: 0x04008B34 RID: 35636
		public bool IsCollisionBaked;

		// Token: 0x04008B35 RID: 35637
		public bool IsMeshAssigned;

		// Token: 0x04008B36 RID: 35638
		public bool IsDirty = true;

		// Token: 0x04008B37 RID: 35639
		public int VertexCount;

		// Token: 0x04008B38 RID: 35640
		public Mesh Mesh;
	}
}
