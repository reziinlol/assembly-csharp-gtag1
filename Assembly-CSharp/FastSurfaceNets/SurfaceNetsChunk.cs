using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Voxels;

namespace FastSurfaceNets
{
	// Token: 0x020012C0 RID: 4800
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
	public class SurfaceNetsChunk : MonoBehaviour
	{
		// Token: 0x0600780F RID: 30735 RVA: 0x0027667A File Offset: 0x0027487A
		private void Awake()
		{
			if (this.autoGenerate)
			{
				this.BuildChunk();
			}
		}

		// Token: 0x06007810 RID: 30736 RVA: 0x0027668A File Offset: 0x0027488A
		private void OnDestroy()
		{
			if (this.sdf.IsCreated)
			{
				this.sdf.Dispose();
				this.sdf = default(NativeArray<byte>);
			}
		}

		// Token: 0x06007811 RID: 30737 RVA: 0x002766B0 File Offset: 0x002748B0
		public void BuildChunk()
		{
			SurfaceNetsChunk.<BuildChunk>d__13 <BuildChunk>d__;
			<BuildChunk>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<BuildChunk>d__.<>4__this = this;
			<BuildChunk>d__.<>1__state = -1;
			<BuildChunk>d__.<>t__builder.Start<SurfaceNetsChunk.<BuildChunk>d__13>(ref <BuildChunk>d__);
		}

		// Token: 0x06007812 RID: 30738 RVA: 0x002766E8 File Offset: 0x002748E8
		private void FillChunk()
		{
			if (this.parameters.generateShape)
			{
				int x = this.shape.x;
				int num = this.shape.x * this.shape.y;
				int3 @int = this.parameters.shapeMin - this.chunkPosition + this.min;
				int3 int2 = this.parameters.shapeMax - this.chunkPosition + this.min;
				for (int i = 0; i < this.shape.z; i++)
				{
					for (int j = 0; j < this.shape.y; j++)
					{
						int num2 = i * num + j * x;
						for (int k = 0; k < this.shape.x; k++)
						{
							float value;
							if (this.parameters.generateShape)
							{
								value = ((k >= @int.x && k <= int2.x && j >= @int.y && j <= int2.y && i >= @int.z && i <= int2.z) ? 1f : -1f);
							}
							else
							{
								float3 @float = (this.chunkPosition + new int3(k, j, i)).ToFloat3();
								value = noise.snoise(@float * this.parameters.noiseScale) - @float.y / this.parameters.heightScale;
							}
							this.sdf[num2 + k] = value.ToByte();
						}
					}
				}
				return;
			}
			new FillChunkJob
			{
				sdf = this.sdf,
				shape = this.shape,
				chunkPosition = this.chunkPosition,
				shapeMin = this.parameters.shapeMin,
				shapeMax = this.parameters.shapeMax,
				noiseScale = this.parameters.noiseScale,
				heightScale = this.parameters.heightScale,
				min = this.min,
				max = this.max,
				strideY = this.shape.x,
				strideZ = this.shape.x * this.shape.y
			}.Schedule(this.shape.x * this.shape.y * this.shape.z, 64, default(JobHandle)).Complete();
		}

		// Token: 0x06007813 RID: 30739 RVA: 0x0027699C File Offset: 0x00274B9C
		private void OnDrawGizmosSelected()
		{
			if (!this.mesh || this.mesh.vertexCount < 3)
			{
				return;
			}
			Gizmos.color = Color.green;
			int vertexCount = this.mesh.vertexCount;
			Vector3[] vertices = this.mesh.vertices;
			Vector3[] normals = this.mesh.normals;
			for (int i = 0; i < vertexCount; i++)
			{
				Gizmos.DrawLine(base.transform.position + vertices[i], base.transform.position + vertices[i] + normals[i] * 0.25f);
			}
		}

		// Token: 0x04008AF4 RID: 35572
		public int3 Id;

		// Token: 0x04008AF5 RID: 35573
		public GenerationParameters parameters;

		// Token: 0x04008AF6 RID: 35574
		public const int ChunkSize = 32;

		// Token: 0x04008AF7 RID: 35575
		public bool autoGenerate = true;

		// Token: 0x04008AF8 RID: 35576
		private const int Pad = 1;

		// Token: 0x04008AF9 RID: 35577
		private int3 chunkPosition;

		// Token: 0x04008AFA RID: 35578
		private NativeArray<byte> sdf;

		// Token: 0x04008AFB RID: 35579
		private int3 min;

		// Token: 0x04008AFC RID: 35580
		private int3 max;

		// Token: 0x04008AFD RID: 35581
		private int3 shape;

		// Token: 0x04008AFE RID: 35582
		private Mesh mesh;
	}
}
