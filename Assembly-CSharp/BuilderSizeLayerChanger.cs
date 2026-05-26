using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000610 RID: 1552
public class BuilderSizeLayerChanger : MonoBehaviour
{
	// Token: 0x170003F2 RID: 1010
	// (get) Token: 0x060026A1 RID: 9889 RVA: 0x000CC680 File Offset: 0x000CA880
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x060026A2 RID: 9890 RVA: 0x000CC6C0 File Offset: 0x000CA8C0
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x000CC6D8 File Offset: 0x000CA8D8
	public void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000CC764 File Offset: 0x000CA964
	public void OnTriggerExit(Collider other)
	{
		if (other != GTPlayer.Instance.bodyCollider)
		{
			return;
		}
		VRRig offlineVRRig = GorillaTagger.Instance.offlineVRRig;
		if (offlineVRRig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			if (offlineVRRig.sizeManager.currentSizeLayerMaskValue != this.SizeLayerMask && this.fxForLayerChange != null)
			{
				ObjectPools.instance.Instantiate(this.fxForLayerChange, offlineVRRig.transform.position, true);
			}
			offlineVRRig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x0400320F RID: 12815
	public float maxScale;

	// Token: 0x04003210 RID: 12816
	public float minScale;

	// Token: 0x04003211 RID: 12817
	public bool isAssurance;

	// Token: 0x04003212 RID: 12818
	public bool affectLayerA = true;

	// Token: 0x04003213 RID: 12819
	public bool affectLayerB = true;

	// Token: 0x04003214 RID: 12820
	public bool affectLayerC = true;

	// Token: 0x04003215 RID: 12821
	public bool affectLayerD = true;

	// Token: 0x04003216 RID: 12822
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04003217 RID: 12823
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04003218 RID: 12824
	[SerializeField]
	private GameObject fxForLayerChange;
}
