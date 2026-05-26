using System;
using UnityEngine;

// Token: 0x020009E3 RID: 2531
public class TappableDent : Tappable
{
	// Token: 0x060040D5 RID: 16597 RVA: 0x0015ACE8 File Offset: 0x00158EE8
	private void Start()
	{
		if (this.parent == null)
		{
			this.parent = base.gameObject;
		}
		this.offsetPerTap = base.transform.parent.InverseTransformVector(base.transform.TransformVector(this.finalLocalOffset / (float)this.numTapsToDestroy));
		this.scaleOffsetPerTap = (this.finalLocalScale - base.transform.localScale) / (float)this.numTapsToDestroy;
	}

	// Token: 0x060040D6 RID: 16598 RVA: 0x0015AD6C File Offset: 0x00158F6C
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this.numTapsSoFar++;
		if (this.numTapsSoFar >= this.numTapsToDestroy)
		{
			this.parent.SetActive(false);
			return;
		}
		base.transform.localPosition += this.offsetPerTap;
		base.transform.localScale += this.scaleOffsetPerTap;
	}

	// Token: 0x04005168 RID: 20840
	[SerializeField]
	private int numTapsToDestroy = 3;

	// Token: 0x04005169 RID: 20841
	[SerializeField]
	private Vector3 finalLocalOffset;

	// Token: 0x0400516A RID: 20842
	[SerializeField]
	private Vector3 finalLocalScale;

	// Token: 0x0400516B RID: 20843
	[SerializeField]
	private GameObject parent;

	// Token: 0x0400516C RID: 20844
	private int numTapsSoFar;

	// Token: 0x0400516D RID: 20845
	private Vector3 offsetPerTap;

	// Token: 0x0400516E RID: 20846
	private Vector3 scaleOffsetPerTap;
}
