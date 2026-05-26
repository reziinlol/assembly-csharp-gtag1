using System;
using BoingKit;
using UnityEngine;

// Token: 0x0200001C RID: 28
public class ImplosionExplosionMain : MonoBehaviour
{
	// Token: 0x0600006B RID: 107 RVA: 0x000039D8 File Offset: 0x00001BD8
	public void Start()
	{
		this.m_aaInstancedDiamondMatrix = new Matrix4x4[(this.NumDiamonds + ImplosionExplosionMain.kNumInstancedBushesPerDrawCall - 1) / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][];
		for (int i = 0; i < this.m_aaInstancedDiamondMatrix.Length; i++)
		{
			this.m_aaInstancedDiamondMatrix[i] = new Matrix4x4[ImplosionExplosionMain.kNumInstancedBushesPerDrawCall];
		}
		for (int j = 0; j < this.NumDiamonds; j++)
		{
			float d = Random.Range(0.1f, 0.4f);
			Vector3 pos = new Vector3(Random.Range(-3.5f, 3.5f), Random.Range(0.5f, 7f), Random.Range(-3.5f, 3.5f));
			Quaternion q = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));
			this.m_aaInstancedDiamondMatrix[j / ImplosionExplosionMain.kNumInstancedBushesPerDrawCall][j % ImplosionExplosionMain.kNumInstancedBushesPerDrawCall].SetTRS(pos, q, d * Vector3.one);
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00003AE8 File Offset: 0x00001CE8
	public void Update()
	{
		Mesh sharedMesh = this.Diamond.GetComponent<MeshFilter>().sharedMesh;
		Material sharedMaterial = this.Diamond.GetComponent<MeshRenderer>().sharedMaterial;
		if (this.m_diamondMaterialProps == null)
		{
			this.m_diamondMaterialProps = new MaterialPropertyBlock();
		}
		if (this.ReactorField.UpdateShaderConstants(this.m_diamondMaterialProps, 1f, 1f))
		{
			foreach (Matrix4x4[] array in this.m_aaInstancedDiamondMatrix)
			{
				Graphics.DrawMeshInstanced(sharedMesh, 0, sharedMaterial, array, array.Length, this.m_diamondMaterialProps);
			}
		}
	}

	// Token: 0x0400005F RID: 95
	public BoingReactorField ReactorField;

	// Token: 0x04000060 RID: 96
	public GameObject Diamond;

	// Token: 0x04000061 RID: 97
	public int NumDiamonds;

	// Token: 0x04000062 RID: 98
	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	// Token: 0x04000063 RID: 99
	private Matrix4x4[][] m_aaInstancedDiamondMatrix;

	// Token: 0x04000064 RID: 100
	private MaterialPropertyBlock m_diamondMaterialProps;
}
