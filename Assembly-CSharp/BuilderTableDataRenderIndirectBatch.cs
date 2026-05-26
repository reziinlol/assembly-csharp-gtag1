using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

// Token: 0x0200063E RID: 1598
public class BuilderTableDataRenderIndirectBatch
{
	// Token: 0x040033C1 RID: 13249
	public int totalInstances;

	// Token: 0x040033C2 RID: 13250
	public TransformAccessArray instanceTransform;

	// Token: 0x040033C3 RID: 13251
	public NativeArray<int> instanceTransformIndexToDataIndex;

	// Token: 0x040033C4 RID: 13252
	public List<int> pieceIDPerTransform;

	// Token: 0x040033C5 RID: 13253
	public NativeArray<Matrix4x4> instanceObjectToWorld;

	// Token: 0x040033C6 RID: 13254
	public NativeArray<int> instanceTexIndex;

	// Token: 0x040033C7 RID: 13255
	public NativeArray<float> instanceTint;

	// Token: 0x040033C8 RID: 13256
	public NativeArray<int> instanceLodLevel;

	// Token: 0x040033C9 RID: 13257
	public NativeArray<int> instanceLodLevelDirty;

	// Token: 0x040033CA RID: 13258
	public NativeList<BuilderTableMeshInstances> renderMeshes;

	// Token: 0x040033CB RID: 13259
	public GraphicsBuffer commandBuf;

	// Token: 0x040033CC RID: 13260
	public GraphicsBuffer matrixBuf;

	// Token: 0x040033CD RID: 13261
	public GraphicsBuffer texIndexBuf;

	// Token: 0x040033CE RID: 13262
	public GraphicsBuffer tintBuf;

	// Token: 0x040033CF RID: 13263
	public NativeArray<GraphicsBuffer.IndirectDrawIndexedArgs> commandData;

	// Token: 0x040033D0 RID: 13264
	public int commandCount;

	// Token: 0x040033D1 RID: 13265
	public RenderParams rp;
}
