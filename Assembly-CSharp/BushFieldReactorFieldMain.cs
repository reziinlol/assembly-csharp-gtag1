using System;
using System.Collections.Generic;
using System.Linq;
using BoingKit;
using UnityEngine;

// Token: 0x02000021 RID: 33
public class BushFieldReactorFieldMain : MonoBehaviour
{
	// Token: 0x06000081 RID: 129 RVA: 0x0000432C File Offset: 0x0000252C
	public void Start()
	{
		Random.InitState(0);
		if (this.Bush.GetComponent<BoingReactorFieldGPUSampler>() == null)
		{
			for (int i = 0; i < this.NumBushes; i++)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.Bush);
				float num = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
				gameObject.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
				gameObject.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				gameObject.transform.localScale = num * Vector3.one;
				BoingReactorFieldCPUSampler component = gameObject.GetComponent<BoingReactorFieldCPUSampler>();
				if (component != null)
				{
					component.ReactorField = this.ReactorField;
				}
				BoingReactorFieldGPUSampler component2 = gameObject.GetComponent<BoingReactorFieldGPUSampler>();
				if (component2 != null)
				{
					component2.ReactorField = this.ReactorField;
				}
			}
		}
		else
		{
			this.m_aaInstancedBushMatrix = new Matrix4x4[(this.NumBushes + BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall - 1) / BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall][];
			for (int j = 0; j < this.m_aaInstancedBushMatrix.Length; j++)
			{
				this.m_aaInstancedBushMatrix[j] = new Matrix4x4[BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall];
			}
			for (int k = 0; k < this.NumBushes; k++)
			{
				float num2 = Random.Range(this.BushScaleRange.x, this.BushScaleRange.y);
				Vector3 pos = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.x), 0.2f * num2, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
				Quaternion q = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
				this.m_aaInstancedBushMatrix[k / BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall][k % BushFieldReactorFieldMain.kNumInstancedBushesPerDrawCall].SetTRS(pos, q, num2 * Vector3.one);
			}
		}
		for (int l = 0; l < this.NumBlossoms; l++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.Blossom);
			float num3 = Random.Range(this.BlossomScaleRange.x, this.BlossomScaleRange.y);
			gameObject2.transform.position = new Vector3(Random.Range(-0.5f * this.FieldBounds.x, 0.5f * this.FieldBounds.y), 0.2f * num3, Random.Range(-0.5f * this.FieldBounds.y, 0.5f * this.FieldBounds.y));
			gameObject2.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			gameObject2.transform.localScale = num3 * Vector3.one;
			gameObject2.GetComponent<BoingReactorFieldCPUSampler>().ReactorField = this.ReactorField;
		}
		this.m_aSphere = new List<BoingEffector>(this.NumSpheresPerCircle * this.NumCircles);
		for (int m = 0; m < this.NumCircles; m++)
		{
			for (int n = 0; n < this.NumSpheresPerCircle; n++)
			{
				GameObject gameObject3 = Object.Instantiate<GameObject>(this.Sphere);
				this.m_aSphere.Add(gameObject3.GetComponent<BoingEffector>());
			}
		}
		BoingReactorField component3 = this.ReactorField.GetComponent<BoingReactorField>();
		component3.Effectors = ((component3.Effectors != null) ? component3.Effectors.Concat(this.m_aSphere.ToArray()).ToArray<BoingEffector>() : this.m_aSphere.ToArray());
		this.m_basePhase = 0f;
	}

	// Token: 0x06000082 RID: 130 RVA: 0x0000474C File Offset: 0x0000294C
	public void Update()
	{
		int num = 0;
		for (int i = 0; i < this.NumCircles; i++)
		{
			float num2 = this.MaxCircleRadius / (float)(i + 1);
			for (int j = 0; j < this.NumSpheresPerCircle; j++)
			{
				float num3 = this.m_basePhase + (float)j / (float)this.NumSpheresPerCircle * 2f * 3.1415927f;
				num3 *= ((i % 2 == 0) ? 1f : -1f);
				this.m_aSphere[num].transform.position = new Vector3(num2 * Mathf.Cos(num3), 0.2f, num2 * Mathf.Sin(num3));
				num++;
			}
		}
		this.m_basePhase -= this.CircleSpeed / this.MaxCircleRadius * Time.deltaTime;
		if (this.m_aaInstancedBushMatrix != null)
		{
			Mesh sharedMesh = this.Bush.GetComponent<MeshFilter>().sharedMesh;
			Material sharedMaterial = this.Bush.GetComponent<MeshRenderer>().sharedMaterial;
			if (this.m_bushMaterialProps == null)
			{
				this.m_bushMaterialProps = new MaterialPropertyBlock();
			}
			if (this.ReactorField.UpdateShaderConstants(this.m_bushMaterialProps, 1f, 1f))
			{
				foreach (Matrix4x4[] array in this.m_aaInstancedBushMatrix)
				{
					Graphics.DrawMeshInstanced(sharedMesh, 0, sharedMaterial, array, array.Length, this.m_bushMaterialProps);
				}
			}
		}
	}

	// Token: 0x0400007F RID: 127
	public GameObject Bush;

	// Token: 0x04000080 RID: 128
	public GameObject Blossom;

	// Token: 0x04000081 RID: 129
	public GameObject Sphere;

	// Token: 0x04000082 RID: 130
	public BoingReactorField ReactorField;

	// Token: 0x04000083 RID: 131
	public int NumBushes;

	// Token: 0x04000084 RID: 132
	public Vector2 BushScaleRange;

	// Token: 0x04000085 RID: 133
	public int NumBlossoms;

	// Token: 0x04000086 RID: 134
	public Vector2 BlossomScaleRange;

	// Token: 0x04000087 RID: 135
	public Vector2 FieldBounds;

	// Token: 0x04000088 RID: 136
	public int NumSpheresPerCircle;

	// Token: 0x04000089 RID: 137
	public int NumCircles;

	// Token: 0x0400008A RID: 138
	public float MaxCircleRadius;

	// Token: 0x0400008B RID: 139
	public float CircleSpeed;

	// Token: 0x0400008C RID: 140
	private List<BoingEffector> m_aSphere;

	// Token: 0x0400008D RID: 141
	private float m_basePhase;

	// Token: 0x0400008E RID: 142
	private static readonly int kNumInstancedBushesPerDrawCall = 1000;

	// Token: 0x0400008F RID: 143
	private Matrix4x4[][] m_aaInstancedBushMatrix;

	// Token: 0x04000090 RID: 144
	private MaterialPropertyBlock m_bushMaterialProps;
}
