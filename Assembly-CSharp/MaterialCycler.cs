using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000418 RID: 1048
public class MaterialCycler : MonoBehaviour
{
	// Token: 0x060018DF RID: 6367 RVA: 0x0008CF03 File Offset: 0x0008B103
	private void Awake()
	{
		this.materialCyclerNetworked = base.GetComponent<MaterialCyclerNetworked>();
		this.SetMaterials();
	}

	// Token: 0x060018E0 RID: 6368 RVA: 0x0008CF17 File Offset: 0x0008B117
	private void OnEnable()
	{
		if (this.materialCyclerNetworked != null)
		{
			this.materialCyclerNetworked.OnSynchronize += this.MaterialCyclerNetworked_OnSynchronize;
		}
	}

	// Token: 0x060018E1 RID: 6369 RVA: 0x0008CF3E File Offset: 0x0008B13E
	private void OnDisable()
	{
		if (this.materialCyclerNetworked != null)
		{
			this.materialCyclerNetworked.OnSynchronize -= this.MaterialCyclerNetworked_OnSynchronize;
		}
	}

	// Token: 0x060018E2 RID: 6370 RVA: 0x0008CF68 File Offset: 0x0008B168
	private void MaterialCyclerNetworked_OnSynchronize(int idx, int3 rgb)
	{
		if (idx < 0 || idx >= this.materials.Length)
		{
			return;
		}
		this.index = idx;
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.renderers[i].material = this.materials[this.index].Materials[i];
			this.renderers[i].material.SetColor(this.setColorTarget, new Color((float)rgb.x / 9f, (float)rgb.y / 9f, (float)rgb.z / 9f));
		}
		this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
	}

	// Token: 0x060018E3 RID: 6371 RVA: 0x0008D05C File Offset: 0x0008B25C
	private void SetMaterials()
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			if (this.materials[this.index].Materials.Length > i)
			{
				this.renderers[i].material = this.materials[this.index].Materials[i];
			}
			else
			{
				this.renderers[i].material = null;
			}
		}
		this.reset.Invoke(new Vector3(this.renderers[0].material.color.r, this.renderers[0].material.color.g, this.renderers[0].material.color.b));
	}

	// Token: 0x060018E4 RID: 6372 RVA: 0x0008D11B File Offset: 0x0008B31B
	public void NextMaterial()
	{
		this.index = (this.index + 1) % this.materials.Length;
		this.SetMaterials();
		this.SetDirty();
	}

	// Token: 0x060018E5 RID: 6373 RVA: 0x0008D140 File Offset: 0x0008B340
	private void SetDirty()
	{
		if (this.materialCyclerNetworked == null)
		{
			return;
		}
		this.synchTime = Time.time + this.materialCyclerNetworked.SyncTimeOut;
		if (this.crDirty == null)
		{
			this.crDirty = base.StartCoroutine(this.timeOutDirty());
		}
	}

	// Token: 0x060018E6 RID: 6374 RVA: 0x0008D18D File Offset: 0x0008B38D
	private IEnumerator timeOutDirty()
	{
		while (this.synchTime > Time.time)
		{
			yield return null;
		}
		this.synchronize();
		this.crDirty = null;
		yield break;
	}

	// Token: 0x060018E7 RID: 6375 RVA: 0x0008D19C File Offset: 0x0008B39C
	private void synchronize()
	{
		this.materialCyclerNetworked.Synchronize(this.index, this.renderers[0].material.color);
	}

	// Token: 0x060018E8 RID: 6376 RVA: 0x0008D1C4 File Offset: 0x0008B3C4
	public void SetColor(Vector3 rgb)
	{
		for (int i = 0; i < this.renderers.Length; i++)
		{
			this.renderers[i].material.SetColor(this.setColorTarget, new Color(rgb.x, rgb.y, rgb.z));
		}
		this.SetDirty();
	}

	// Token: 0x04002408 RID: 9224
	[SerializeField]
	private MaterialCycler.MaterialPack[] materials;

	// Token: 0x04002409 RID: 9225
	[SerializeField]
	private Renderer[] renderers;

	// Token: 0x0400240A RID: 9226
	private int index;

	// Token: 0x0400240B RID: 9227
	[SerializeField]
	private string setColorTarget = "_BaseColor";

	// Token: 0x0400240C RID: 9228
	[SerializeField]
	private UnityEvent<Vector3> reset;

	// Token: 0x0400240D RID: 9229
	private Coroutine crDirty;

	// Token: 0x0400240E RID: 9230
	private float synchTime;

	// Token: 0x0400240F RID: 9231
	private MaterialCyclerNetworked materialCyclerNetworked;

	// Token: 0x02000419 RID: 1049
	[Serializable]
	private class MaterialPack
	{
		// Token: 0x17000273 RID: 627
		// (get) Token: 0x060018EA RID: 6378 RVA: 0x0008D22C File Offset: 0x0008B42C
		public Material[] Materials
		{
			get
			{
				return this.materials;
			}
		}

		// Token: 0x04002410 RID: 9232
		[SerializeField]
		private Material[] materials;
	}
}
