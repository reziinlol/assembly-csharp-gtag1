using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class CritterVisuals : MonoBehaviour
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000326 RID: 806 RVA: 0x0001318B File Offset: 0x0001138B
	public CritterAppearance Appearance
	{
		get
		{
			return this._appearance;
		}
	}

	// Token: 0x06000327 RID: 807 RVA: 0x00013194 File Offset: 0x00011394
	public void SetAppearance(CritterAppearance appearance)
	{
		this._appearance = appearance;
		float num = this._appearance.size.ClampSafe(0.25f, 1.5f);
		this.bodyRoot.localScale = new Vector3(num, num, num);
		if (!string.IsNullOrEmpty(appearance.hatName))
		{
			foreach (GameObject gameObject in this.hats)
			{
				gameObject.SetActive(gameObject.name == this._appearance.hatName);
			}
			this.hatRoot.gameObject.SetActive(true);
			return;
		}
		this.hatRoot.gameObject.SetActive(false);
	}

	// Token: 0x06000328 RID: 808 RVA: 0x00013239 File Offset: 0x00011439
	public void ApplyMesh(Mesh newMesh)
	{
		this.myMeshFilter.sharedMesh = newMesh;
	}

	// Token: 0x06000329 RID: 809 RVA: 0x00013247 File Offset: 0x00011447
	public void ApplyMaterial(Material mat)
	{
		this.myRenderer.sharedMaterial = mat;
	}

	// Token: 0x040003BB RID: 955
	public int critterType;

	// Token: 0x040003BC RID: 956
	[Header("Visuals")]
	public Transform bodyRoot;

	// Token: 0x040003BD RID: 957
	public MeshRenderer myRenderer;

	// Token: 0x040003BE RID: 958
	public MeshFilter myMeshFilter;

	// Token: 0x040003BF RID: 959
	public Transform hatRoot;

	// Token: 0x040003C0 RID: 960
	public GameObject[] hats;

	// Token: 0x040003C1 RID: 961
	private CritterAppearance _appearance;
}
