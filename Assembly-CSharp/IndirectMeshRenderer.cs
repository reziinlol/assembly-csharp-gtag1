using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000376 RID: 886
public static class IndirectMeshRenderer
{
	// Token: 0x060015AE RID: 5550 RVA: 0x000725B0 File Offset: 0x000707B0
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	private static void _Init()
	{
		IndirectMeshRenderer._DisposeAll();
		IndirectMeshRenderer._shader = Shader.Find("GorillaTag/IndirectLit");
		if (IndirectMeshRenderer._shader == null)
		{
			Debug.LogError("[IndirectMeshRenderer] Shader 'GorillaTag/IndirectLit' not found. Add it to Always Included Shaders.");
		}
		IndirectMeshRenderer._shaderEmissive = Shader.Find("GorillaTag/IndirectLitEmissive");
		if (IndirectMeshRenderer._shaderEmissive == null)
		{
			Debug.LogError("[IndirectMeshRenderer] Shader 'GorillaTag/IndirectLitEmissive' not found. Add it to Always Included Shaders.");
		}
		Application.quitting += IndirectMeshRenderer._DisposeAll;
		TickSystem<object>.AddPostTickCallback(new IndirectMeshRenderer.PostTickCallback());
	}

	// Token: 0x060015AF RID: 5551 RVA: 0x0007262C File Offset: 0x0007082C
	public static void Register(IndirectMeshInstance inst, int groupId = 0)
	{
		Mesh sharedMesh = inst.meshFilter.sharedMesh;
		if (sharedMesh.subMeshCount > 1)
		{
			Debug.LogError(string.Format("[IndirectMeshRenderer] Mesh '{0}' on '{1}' has {2} submeshes ", sharedMesh.name, inst.name, sharedMesh.subMeshCount) + "(likely from static batching). Disable Static on objects with IndirectMeshInstance.", inst);
			return;
		}
		Material sharedMaterial = inst.meshRenderer.sharedMaterial;
		Texture texture = sharedMaterial.HasTexture(ShaderProps._BaseMap) ? sharedMaterial.GetTexture(ShaderProps._BaseMap) : null;
		bool flag = sharedMaterial.IsKeywordEnabled("_EMISSION");
		Shader shader = flag ? IndirectMeshRenderer._shaderEmissive : IndirectMeshRenderer._shader;
		IndirectMeshRenderer.BatchKey key = new IndirectMeshRenderer.BatchKey
		{
			meshId = sharedMesh.GetInstanceID(),
			textureId = ((texture != null) ? texture.GetInstanceID() : 0),
			shaderId = shader.GetInstanceID()
		};
		int count;
		if (!IndirectMeshRenderer._batchLookup.TryGetValue(key, out count))
		{
			IndirectMeshRenderer.DrawBatch drawBatch = new IndirectMeshRenderer.DrawBatch
			{
				mesh = sharedMesh,
				submeshCount = sharedMesh.subMeshCount,
				layer = inst.gameObject.layer,
				matrices = new NativeList<Matrix4x4>(2048, Allocator.Persistent),
				groupIds = new NativeList<int>(2048, Allocator.Persistent),
				visibility = new NativeList<byte>(2048, Allocator.Persistent),
				material = new Material(shader)
				{
					name = sharedMaterial.name + " (Indirect)"
				}
			};
			if (texture != null)
			{
				drawBatch.material.SetTexture(ShaderProps._BaseMap, texture);
			}
			if (sharedMaterial.HasColor(ShaderProps._BaseColor))
			{
				drawBatch.material.SetColor(ShaderProps._BaseColor, sharedMaterial.GetColor(ShaderProps._BaseColor));
			}
			if (flag)
			{
				IndirectMeshRenderer._CopyEmissionProperties(drawBatch.material, sharedMaterial);
			}
			count = IndirectMeshRenderer._batchList.Count;
			IndirectMeshRenderer._batchLookup[key] = count;
			IndirectMeshRenderer._batchList.Add(drawBatch);
			Debug.Log(string.Format("[IndirectMeshRenderer] New batch #{0}: mesh='{1}' tex='{2}' shader='{3}' layer={4} submeshes={5}", new object[]
			{
				count,
				sharedMesh.name,
				(texture != null) ? texture.name : "null",
				shader.name,
				inst.gameObject.layer,
				sharedMesh.subMeshCount
			}));
		}
		IndirectMeshRenderer.DrawBatch drawBatch2 = IndirectMeshRenderer._batchList[count];
		int length = drawBatch2.matrices.Length;
		Matrix4x4 localToWorldMatrix = inst.transform.localToWorldMatrix;
		drawBatch2.matrices.Add(localToWorldMatrix);
		drawBatch2.groupIds.Add(groupId);
		byte b = 1;
		drawBatch2.visibility.Add(b);
		drawBatch2.visibleCount++;
		drawBatch2.dirty = true;
		if (inst.dynamic)
		{
			ref List<IndirectMeshRenderer.DynamicEntry> ptr = ref drawBatch2.dynamicEntries;
			if (ptr == null)
			{
				ptr = new List<IndirectMeshRenderer.DynamicEntry>();
			}
			drawBatch2.dynamicEntries.Add(new IndirectMeshRenderer.DynamicEntry
			{
				transform = inst.transform,
				matrixIndex = length
			});
		}
		IndirectMeshRenderer._batchList[count] = drawBatch2;
	}

	// Token: 0x060015B0 RID: 5552 RVA: 0x0007295C File Offset: 0x00070B5C
	private static void _CopyEmissionProperties(Material dst, Material src)
	{
		if (src.HasTexture(ShaderProps._EmissionMap))
		{
			dst.SetTexture(ShaderProps._EmissionMap, src.GetTexture(ShaderProps._EmissionMap));
		}
		if (src.HasColor(ShaderProps._EmissionColor))
		{
			dst.SetColor(ShaderProps._EmissionColor, src.GetColor(ShaderProps._EmissionColor));
		}
		if (src.HasVector(ShaderProps._EmissionUVScrollSpeed))
		{
			dst.SetVector(ShaderProps._EmissionUVScrollSpeed, src.GetVector(ShaderProps._EmissionUVScrollSpeed));
		}
		if (src.HasFloat(ShaderProps._EmissionDissolveEdgeSize))
		{
			dst.SetFloat(ShaderProps._EmissionDissolveEdgeSize, src.GetFloat(ShaderProps._EmissionDissolveEdgeSize));
		}
		if (src.HasFloat(ShaderProps._EmissionDissolveProgress))
		{
			dst.SetFloat(ShaderProps._EmissionDissolveProgress, src.GetFloat(ShaderProps._EmissionDissolveProgress));
		}
		if (src.HasVector(ShaderProps._EmissionDissolveAnimation))
		{
			dst.SetVector(ShaderProps._EmissionDissolveAnimation, src.GetVector(ShaderProps._EmissionDissolveAnimation));
		}
		if (src.HasFloat(ShaderProps._EmissionMaskByBaseMapAlpha))
		{
			dst.SetFloat(ShaderProps._EmissionMaskByBaseMapAlpha, src.GetFloat(ShaderProps._EmissionMaskByBaseMapAlpha));
		}
	}

	// Token: 0x060015B1 RID: 5553 RVA: 0x00072A60 File Offset: 0x00070C60
	public static void SetGroupVisible(int groupId, bool visible)
	{
		byte b = visible ? 1 : 0;
		for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
		{
			IndirectMeshRenderer.DrawBatch value = IndirectMeshRenderer._batchList[i];
			bool flag = false;
			int length = value.groupIds.Length;
			for (int j = 0; j < length; j++)
			{
				if (value.groupIds[j] == groupId && value.visibility[j] != b)
				{
					value.visibility[j] = b;
					value.visibleCount += (visible ? 1 : -1);
					flag = true;
				}
			}
			if (flag)
			{
				value.dirty = true;
				IndirectMeshRenderer._batchList[i] = value;
			}
		}
	}

	// Token: 0x060015B2 RID: 5554 RVA: 0x00072B1C File Offset: 0x00070D1C
	private static void _Render()
	{
		if (IndirectMeshRenderer._batchList.Count == 0)
		{
			return;
		}
		if (!IndirectMeshRenderer._loggedFirstRender)
		{
			IndirectMeshRenderer._loggedFirstRender = true;
			int num = 0;
			for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
			{
				num += IndirectMeshRenderer._batchList[i].visibleCount;
			}
			Debug.Log(string.Format("[IndirectMeshRenderer] First render: {0} batch(es), {1} visible instance(s), stereoMul={2}", IndirectMeshRenderer._batchList.Count, num, 2));
		}
		for (int j = 0; j < IndirectMeshRenderer._batchList.Count; j++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[j];
			if (drawBatch.dynamicEntries != null)
			{
				for (int k = drawBatch.dynamicEntries.Count - 1; k >= 0; k--)
				{
					IndirectMeshRenderer.DynamicEntry dynamicEntry = drawBatch.dynamicEntries[k];
					if (dynamicEntry.transform == null)
					{
						if (drawBatch.visibility[dynamicEntry.matrixIndex] != 0)
						{
							drawBatch.visibility[dynamicEntry.matrixIndex] = 0;
							drawBatch.visibleCount--;
							drawBatch.dirty = true;
						}
						int index = drawBatch.dynamicEntries.Count - 1;
						drawBatch.dynamicEntries[k] = drawBatch.dynamicEntries[index];
						drawBatch.dynamicEntries.RemoveAt(index);
					}
					else
					{
						drawBatch.matrices[dynamicEntry.matrixIndex] = dynamicEntry.transform.localToWorldMatrix;
					}
				}
				if (!drawBatch.dirty && drawBatch.dynamicEntries.Count > 0)
				{
					drawBatch.needsUpload = true;
				}
			}
			if (drawBatch.visibleCount == 0)
			{
				if (drawBatch.dirty)
				{
					IndirectMeshRenderer._DisposeBatchBuffers(ref drawBatch);
					drawBatch.dirty = false;
					drawBatch.needsUpload = false;
				}
				IndirectMeshRenderer._batchList[j] = drawBatch;
			}
			else
			{
				if (drawBatch.dirty)
				{
					IndirectMeshRenderer._RebuildBatch(ref drawBatch);
				}
				else if (drawBatch.needsUpload)
				{
					IndirectMeshRenderer._UploadBatch(ref drawBatch);
				}
				IndirectMeshRenderer._batchList[j] = drawBatch;
				Graphics.RenderMeshIndirect(drawBatch.renderParams, drawBatch.mesh, drawBatch.commandBuffer, drawBatch.submeshCount, 0);
			}
		}
	}

	// Token: 0x060015B3 RID: 5555 RVA: 0x00072D3C File Offset: 0x00070F3C
	private static void _RebuildBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		int length = batch.matrices.Length;
		int num = batch.visibleCount * 2;
		IndirectMeshRenderer._DisposeBatchBuffers(ref batch);
		batch.matrixBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, num, 64);
		batch.commandBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, batch.submeshCount, 20);
		if (!batch.gpuMatrices.IsCreated || batch.gpuMatrices.Length < num)
		{
			if (batch.gpuMatrices.IsCreated)
			{
				batch.gpuMatrices.Dispose();
			}
			batch.gpuMatrices = new NativeArray<Matrix4x4>(num, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		int num2 = 0;
		for (int i = 0; i < length; i++)
		{
			if (batch.visibility[i] != 0)
			{
				Matrix4x4 matrix4x = batch.matrices[i];
				Vector3 rhs = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
				vector = Vector3.Min(vector, rhs);
				vector2 = Vector3.Max(vector2, rhs);
				int num3 = num2 * 2;
				batch.gpuMatrices[num3] = matrix4x;
				batch.gpuMatrices[num3 + 1] = matrix4x;
				num2++;
			}
		}
		batch.matrixBuffer.SetData<Matrix4x4>(batch.gpuMatrices, 0, 0, num);
		Vector3 b = Vector3.one * 10f;
		Bounds worldBounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector + b);
		if (!batch.commandData.IsCreated || batch.commandData.Length != batch.submeshCount)
		{
			if (batch.commandData.IsCreated)
			{
				batch.commandData.Dispose();
			}
			batch.commandData = new NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs>(batch.submeshCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
		}
		for (int j = 0; j < batch.submeshCount; j++)
		{
			batch.commandData[j] = new GraphicsBuffer.IndirectDrawIndexedArgs
			{
				indexCountPerInstance = batch.mesh.GetIndexCount(j),
				startIndex = batch.mesh.GetIndexStart(j),
				baseVertexIndex = batch.mesh.GetBaseVertex(j),
				startInstance = 0U,
				instanceCount = (uint)num
			};
		}
		batch.commandBuffer.SetData<GraphicsBuffer.IndirectDrawIndexedArgs>(batch.commandData);
		batch.renderParams = new RenderParams(batch.material)
		{
			worldBounds = worldBounds,
			layer = batch.layer,
			shadowCastingMode = ShadowCastingMode.Off,
			receiveShadows = false,
			matProps = new MaterialPropertyBlock()
		};
		batch.renderParams.matProps.SetBuffer(IndirectMeshRenderer._spId_Matrices, batch.matrixBuffer);
		batch.dirty = false;
		batch.needsUpload = false;
	}

	// Token: 0x060015B4 RID: 5556 RVA: 0x00073018 File Offset: 0x00071218
	private static void _UploadBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		int length = batch.matrices.Length;
		Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			if (batch.visibility[i] != 0)
			{
				Matrix4x4 matrix4x = batch.matrices[i];
				Vector3 rhs = new Vector3(matrix4x.m03, matrix4x.m13, matrix4x.m23);
				vector = Vector3.Min(vector, rhs);
				vector2 = Vector3.Max(vector2, rhs);
				int num2 = num * 2;
				batch.gpuMatrices[num2] = matrix4x;
				batch.gpuMatrices[num2 + 1] = matrix4x;
				num++;
			}
		}
		batch.matrixBuffer.SetData<Matrix4x4>(batch.gpuMatrices, 0, 0, num * 2);
		Vector3 b = Vector3.one * 10f;
		batch.renderParams.worldBounds = new Bounds((vector + vector2) * 0.5f, vector2 - vector + b);
		batch.needsUpload = false;
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x00073141 File Offset: 0x00071341
	private static void _DisposeBatchBuffers(ref IndirectMeshRenderer.DrawBatch batch)
	{
		GraphicsBuffer matrixBuffer = batch.matrixBuffer;
		if (matrixBuffer != null)
		{
			matrixBuffer.Dispose();
		}
		batch.matrixBuffer = null;
		GraphicsBuffer commandBuffer = batch.commandBuffer;
		if (commandBuffer != null)
		{
			commandBuffer.Dispose();
		}
		batch.commandBuffer = null;
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x00073174 File Offset: 0x00071374
	private static void _DisposeBatch(ref IndirectMeshRenderer.DrawBatch batch)
	{
		IndirectMeshRenderer._DisposeBatchBuffers(ref batch);
		if (batch.matrices.IsCreated)
		{
			batch.matrices.Dispose();
		}
		if (batch.groupIds.IsCreated)
		{
			batch.groupIds.Dispose();
		}
		if (batch.visibility.IsCreated)
		{
			batch.visibility.Dispose();
		}
		if (batch.gpuMatrices.IsCreated)
		{
			batch.gpuMatrices.Dispose();
		}
		if (batch.commandData.IsCreated)
		{
			batch.commandData.Dispose();
		}
		if (batch.material != null)
		{
			Object.Destroy(batch.material);
		}
		batch.dynamicEntries = null;
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x00073220 File Offset: 0x00071420
	private static void _DisposeAll()
	{
		for (int i = 0; i < IndirectMeshRenderer._batchList.Count; i++)
		{
			IndirectMeshRenderer.DrawBatch drawBatch = IndirectMeshRenderer._batchList[i];
			IndirectMeshRenderer._DisposeBatch(ref drawBatch);
		}
		IndirectMeshRenderer._batchList.Clear();
		IndirectMeshRenderer._batchLookup.Clear();
	}

	// Token: 0x04001A73 RID: 6771
	private const string SHADER_NAME = "GorillaTag/IndirectLit";

	// Token: 0x04001A74 RID: 6772
	private const string SHADER_NAME_EMISSIVE = "GorillaTag/IndirectLitEmissive";

	// Token: 0x04001A75 RID: 6773
	private const int _k_instancesPerXform = 2;

	// Token: 0x04001A76 RID: 6774
	private static readonly int _spId_Matrices = Shader.PropertyToID("_Matrices");

	// Token: 0x04001A77 RID: 6775
	private static Shader _shader;

	// Token: 0x04001A78 RID: 6776
	private static Shader _shaderEmissive;

	// Token: 0x04001A79 RID: 6777
	private static readonly Dictionary<IndirectMeshRenderer.BatchKey, int> _batchLookup = new Dictionary<IndirectMeshRenderer.BatchKey, int>();

	// Token: 0x04001A7A RID: 6778
	private static readonly List<IndirectMeshRenderer.DrawBatch> _batchList = new List<IndirectMeshRenderer.DrawBatch>();

	// Token: 0x04001A7B RID: 6779
	private static bool _loggedFirstRender;

	// Token: 0x02000377 RID: 887
	private struct BatchKey : IEquatable<IndirectMeshRenderer.BatchKey>
	{
		// Token: 0x060015B9 RID: 5561 RVA: 0x0007328E File Offset: 0x0007148E
		public bool Equals(IndirectMeshRenderer.BatchKey other)
		{
			return this.meshId == other.meshId && this.textureId == other.textureId && this.shaderId == other.shaderId;
		}

		// Token: 0x060015BA RID: 5562 RVA: 0x000732BC File Offset: 0x000714BC
		public override int GetHashCode()
		{
			return (this.meshId * 397 ^ this.textureId) * 397 ^ this.shaderId;
		}

		// Token: 0x060015BB RID: 5563 RVA: 0x000732E0 File Offset: 0x000714E0
		public override bool Equals(object obj)
		{
			if (obj is IndirectMeshRenderer.BatchKey)
			{
				IndirectMeshRenderer.BatchKey other = (IndirectMeshRenderer.BatchKey)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x04001A7C RID: 6780
		public int meshId;

		// Token: 0x04001A7D RID: 6781
		public int textureId;

		// Token: 0x04001A7E RID: 6782
		public int shaderId;
	}

	// Token: 0x02000378 RID: 888
	private struct DynamicEntry
	{
		// Token: 0x04001A7F RID: 6783
		public Transform transform;

		// Token: 0x04001A80 RID: 6784
		public int matrixIndex;
	}

	// Token: 0x02000379 RID: 889
	private struct DrawBatch
	{
		// Token: 0x04001A81 RID: 6785
		public Mesh mesh;

		// Token: 0x04001A82 RID: 6786
		public Material material;

		// Token: 0x04001A83 RID: 6787
		public int submeshCount;

		// Token: 0x04001A84 RID: 6788
		public int layer;

		// Token: 0x04001A85 RID: 6789
		public NativeList<Matrix4x4> matrices;

		// Token: 0x04001A86 RID: 6790
		public NativeList<int> groupIds;

		// Token: 0x04001A87 RID: 6791
		public NativeList<byte> visibility;

		// Token: 0x04001A88 RID: 6792
		public int visibleCount;

		// Token: 0x04001A89 RID: 6793
		public NativeArray<Matrix4x4> gpuMatrices;

		// Token: 0x04001A8A RID: 6794
		public GraphicsBuffer matrixBuffer;

		// Token: 0x04001A8B RID: 6795
		public GraphicsBuffer commandBuffer;

		// Token: 0x04001A8C RID: 6796
		public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

		// Token: 0x04001A8D RID: 6797
		public RenderParams renderParams;

		// Token: 0x04001A8E RID: 6798
		public bool dirty;

		// Token: 0x04001A8F RID: 6799
		public bool needsUpload;

		// Token: 0x04001A90 RID: 6800
		public List<IndirectMeshRenderer.DynamicEntry> dynamicEntries;
	}

	// Token: 0x0200037A RID: 890
	private sealed class PostTickCallback : ITickSystemPost
	{
		// Token: 0x17000226 RID: 550
		// (get) Token: 0x060015BC RID: 5564 RVA: 0x00073305 File Offset: 0x00071505
		// (set) Token: 0x060015BD RID: 5565 RVA: 0x0007330D File Offset: 0x0007150D
		public bool PostTickRunning { get; set; }

		// Token: 0x060015BE RID: 5566 RVA: 0x00073316 File Offset: 0x00071516
		public void PostTick()
		{
			IndirectMeshRenderer._Render();
		}
	}
}
