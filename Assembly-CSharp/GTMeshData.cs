using System;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000DF3 RID: 3571
public class GTMeshData
{
	// Token: 0x0600575F RID: 22367 RVA: 0x001C4344 File Offset: 0x001C2544
	public GTMeshData(Mesh m)
	{
		this.mesh = m;
		this.subMeshCount = m.subMeshCount;
		this.vertices = m.vertices;
		this.triangles = m.triangles;
		this.normals = m.normals;
		this.tangents = m.tangents;
		this.colors32 = m.colors32;
		this.boneWeights = m.boneWeights;
		this.uv = m.uv;
		this.uv2 = m.uv2;
		this.uv3 = m.uv3;
		this.uv4 = m.uv4;
		this.uv5 = m.uv5;
		this.uv6 = m.uv6;
		this.uv7 = m.uv7;
		this.uv8 = m.uv8;
	}

	// Token: 0x06005760 RID: 22368 RVA: 0x001C4414 File Offset: 0x001C2614
	public Mesh ExtractSubmesh(int subMeshIndex, bool optimize = false)
	{
		if (subMeshIndex < 0 || subMeshIndex >= this.subMeshCount)
		{
			throw new IndexOutOfRangeException("subMeshIndex");
		}
		SubMeshDescriptor subMesh = this.mesh.GetSubMesh(subMeshIndex);
		int firstVertex = subMesh.firstVertex;
		int vertexCount = subMesh.vertexCount;
		MeshTopology topology = subMesh.topology;
		int[] indices = this.mesh.GetIndices(subMeshIndex, false);
		for (int i = 0; i < indices.Length; i++)
		{
			indices[i] -= firstVertex;
		}
		Mesh mesh = new Mesh();
		mesh.indexFormat = ((vertexCount > 65535) ? IndexFormat.UInt32 : IndexFormat.UInt16);
		mesh.SetVertices(this.vertices, firstVertex, vertexCount);
		mesh.SetIndices(indices, topology, 0);
		mesh.SetNormals(this.normals, firstVertex, vertexCount);
		mesh.SetTangents(this.tangents, firstVertex, vertexCount);
		if (!this.uv.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(0, this.uv, firstVertex, vertexCount);
		}
		if (!this.uv2.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(1, this.uv2, firstVertex, vertexCount);
		}
		if (!this.uv3.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(2, this.uv3, firstVertex, vertexCount);
		}
		if (!this.uv4.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(3, this.uv4, firstVertex, vertexCount);
		}
		if (!this.uv5.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(4, this.uv5, firstVertex, vertexCount);
		}
		if (!this.uv6.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(5, this.uv6, firstVertex, vertexCount);
		}
		if (!this.uv7.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(6, this.uv7, firstVertex, vertexCount);
		}
		if (!this.uv8.IsNullOrEmpty<Vector2>())
		{
			mesh.SetUVs(7, this.uv8, firstVertex, vertexCount);
		}
		if (optimize)
		{
			mesh.Optimize();
			mesh.OptimizeIndexBuffers();
		}
		mesh.RecalculateBounds();
		return mesh;
	}

	// Token: 0x06005761 RID: 22369 RVA: 0x001C45E2 File Offset: 0x001C27E2
	public static GTMeshData Parse(Mesh mesh)
	{
		if (mesh == null)
		{
			throw new ArgumentNullException("mesh");
		}
		return new GTMeshData(mesh);
	}

	// Token: 0x04006759 RID: 26457
	public Mesh mesh;

	// Token: 0x0400675A RID: 26458
	public Vector3[] vertices;

	// Token: 0x0400675B RID: 26459
	public Vector3[] normals;

	// Token: 0x0400675C RID: 26460
	public Vector4[] tangents;

	// Token: 0x0400675D RID: 26461
	public Color32[] colors32;

	// Token: 0x0400675E RID: 26462
	public int[] triangles;

	// Token: 0x0400675F RID: 26463
	public BoneWeight[] boneWeights;

	// Token: 0x04006760 RID: 26464
	public Vector2[] uv;

	// Token: 0x04006761 RID: 26465
	public Vector2[] uv2;

	// Token: 0x04006762 RID: 26466
	public Vector2[] uv3;

	// Token: 0x04006763 RID: 26467
	public Vector2[] uv4;

	// Token: 0x04006764 RID: 26468
	public Vector2[] uv5;

	// Token: 0x04006765 RID: 26469
	public Vector2[] uv6;

	// Token: 0x04006766 RID: 26470
	public Vector2[] uv7;

	// Token: 0x04006767 RID: 26471
	public Vector2[] uv8;

	// Token: 0x04006768 RID: 26472
	public int subMeshCount;
}
