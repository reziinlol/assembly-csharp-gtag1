using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x0200063F RID: 1599
public class BuilderTableDataRenderData
{
	// Token: 0x040033D2 RID: 13266
	public const int NUM_SPLIT_MESH_INSTANCE_GROUPS = 1;

	// Token: 0x040033D3 RID: 13267
	public int texWidth;

	// Token: 0x040033D4 RID: 13268
	public int texHeight;

	// Token: 0x040033D5 RID: 13269
	public TextureFormat textureFormat;

	// Token: 0x040033D6 RID: 13270
	public Dictionary<Material, int> materialToIndex;

	// Token: 0x040033D7 RID: 13271
	public List<Material> materials;

	// Token: 0x040033D8 RID: 13272
	public Material sharedMaterial;

	// Token: 0x040033D9 RID: 13273
	public Material sharedMaterialIndirect;

	// Token: 0x040033DA RID: 13274
	public Dictionary<Texture2D, int> textureToIndex;

	// Token: 0x040033DB RID: 13275
	public List<Texture2D> textures;

	// Token: 0x040033DC RID: 13276
	public List<Material> perTextureMaterial;

	// Token: 0x040033DD RID: 13277
	public List<MaterialPropertyBlock> perTexturePropertyBlock;

	// Token: 0x040033DE RID: 13278
	public Texture2DArray sharedTexArray;

	// Token: 0x040033DF RID: 13279
	public Dictionary<Mesh, int> meshToIndex;

	// Token: 0x040033E0 RID: 13280
	public List<Mesh> meshes;

	// Token: 0x040033E1 RID: 13281
	public List<int> meshInstanceCount;

	// Token: 0x040033E2 RID: 13282
	public NativeList<BuilderTableSubMesh> subMeshes;

	// Token: 0x040033E3 RID: 13283
	public Mesh sharedMesh;

	// Token: 0x040033E4 RID: 13284
	public BuilderTableDataRenderIndirectBatch dynamicBatch;

	// Token: 0x040033E5 RID: 13285
	public BuilderTableDataRenderIndirectBatch staticBatch;

	// Token: 0x040033E6 RID: 13286
	public JobHandle setupInstancesJobs;
}
