using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200090C RID: 2316
public class SizeLayerChanger : MonoBehaviour
{
	// Token: 0x17000578 RID: 1400
	// (get) Token: 0x06003C8F RID: 15503 RVA: 0x00149CF8 File Offset: 0x00147EF8
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

	// Token: 0x06003C90 RID: 15504 RVA: 0x00149D38 File Offset: 0x00147F38
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
	}

	// Token: 0x06003C91 RID: 15505 RVA: 0x00149D50 File Offset: 0x00147F50
	public void OnTriggerEnter(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerEnter)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x06003C92 RID: 15506 RVA: 0x00149DD0 File Offset: 0x00147FD0
	public void OnTriggerExit(Collider other)
	{
		if (!this.triggerWithBodyCollider && !other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig vrrig;
		if (this.triggerWithBodyCollider)
		{
			if (other != GTPlayer.Instance.bodyCollider)
			{
				return;
			}
			vrrig = GorillaTagger.Instance.offlineVRRig;
		}
		else
		{
			vrrig = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		}
		if (vrrig == null)
		{
			return;
		}
		if (this.applyOnTriggerExit)
		{
			vrrig.sizeManager.currentSizeLayerMaskValue = this.SizeLayerMask;
		}
	}

	// Token: 0x04004D32 RID: 19762
	public float maxScale;

	// Token: 0x04004D33 RID: 19763
	public float minScale;

	// Token: 0x04004D34 RID: 19764
	public bool isAssurance;

	// Token: 0x04004D35 RID: 19765
	public bool affectLayerA = true;

	// Token: 0x04004D36 RID: 19766
	public bool affectLayerB = true;

	// Token: 0x04004D37 RID: 19767
	public bool affectLayerC = true;

	// Token: 0x04004D38 RID: 19768
	public bool affectLayerD = true;

	// Token: 0x04004D39 RID: 19769
	[SerializeField]
	private bool applyOnTriggerEnter = true;

	// Token: 0x04004D3A RID: 19770
	[SerializeField]
	private bool applyOnTriggerExit;

	// Token: 0x04004D3B RID: 19771
	[SerializeField]
	private bool triggerWithBodyCollider;
}
