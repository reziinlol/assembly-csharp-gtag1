using System;
using UnityEngine;

// Token: 0x020004D6 RID: 1238
public class RigDuplicationZone : MonoBehaviour
{
	// Token: 0x14000042 RID: 66
	// (add) Token: 0x06001E21 RID: 7713 RVA: 0x000A1790 File Offset: 0x0009F990
	// (remove) Token: 0x06001E22 RID: 7714 RVA: 0x000A17C4 File Offset: 0x0009F9C4
	public static event RigDuplicationZone.RigDuplicationZoneAction OnEnabled;

	// Token: 0x17000329 RID: 809
	// (get) Token: 0x06001E23 RID: 7715 RVA: 0x000A17F7 File Offset: 0x0009F9F7
	public string Id
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06001E24 RID: 7716 RVA: 0x000A17FF File Offset: 0x0009F9FF
	private void OnEnable()
	{
		RigDuplicationZone.OnEnabled += this.RigDuplicationZone_OnEnabled;
		if (RigDuplicationZone.OnEnabled != null)
		{
			RigDuplicationZone.OnEnabled(this);
		}
	}

	// Token: 0x06001E25 RID: 7717 RVA: 0x000A1824 File Offset: 0x0009FA24
	private void OnDisable()
	{
		RigDuplicationZone.OnEnabled -= this.RigDuplicationZone_OnEnabled;
	}

	// Token: 0x06001E26 RID: 7718 RVA: 0x000A1837 File Offset: 0x0009FA37
	private void RigDuplicationZone_OnEnabled(RigDuplicationZone z)
	{
		if (z == this)
		{
			return;
		}
		if (z.id != this.id)
		{
			return;
		}
		this.SetOtherZone(z);
		z.SetOtherZone(this);
	}

	// Token: 0x06001E27 RID: 7719 RVA: 0x000A1865 File Offset: 0x0009FA65
	private void SetOtherZone(RigDuplicationZone z)
	{
		this.otherZone = z;
		this.offsetToOtherZone = z.transform.position - base.transform.position;
	}

	// Token: 0x06001E28 RID: 7720 RVA: 0x000A1890 File Offset: 0x0009FA90
	private void OnTriggerEnter(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = true;
			return;
		}
		component.SetDuplicationZone(this);
	}

	// Token: 0x06001E29 RID: 7721 RVA: 0x000A18C8 File Offset: 0x0009FAC8
	private void OnTriggerExit(Collider other)
	{
		VRRig component = other.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		if (component.isLocal)
		{
			this.playerInZone = false;
			return;
		}
		component.ClearDuplicationZone(this);
	}

	// Token: 0x06001E2A RID: 7722 RVA: 0x000A18FD File Offset: 0x0009FAFD
	public Vector3 GetVisualOffsetForRigs(Vector3 cachedOffset)
	{
		if (this.otherZone == null)
		{
			Debug.LogError("RigDuplicationZone doesn't have an other zone!", base.gameObject);
			return cachedOffset;
		}
		if (!this.otherZone.playerInZone)
		{
			return cachedOffset;
		}
		return this.offsetToOtherZone + cachedOffset;
	}

	// Token: 0x1700032A RID: 810
	// (get) Token: 0x06001E2B RID: 7723 RVA: 0x000A193A File Offset: 0x0009FB3A
	public bool IsApplyingDisplacement
	{
		get
		{
			return this.otherZone.playerInZone;
		}
	}

	// Token: 0x04002851 RID: 10321
	private RigDuplicationZone otherZone;

	// Token: 0x04002852 RID: 10322
	[SerializeField]
	private string id;

	// Token: 0x04002853 RID: 10323
	private bool playerInZone;

	// Token: 0x04002854 RID: 10324
	private Vector3 offsetToOtherZone;

	// Token: 0x020004D7 RID: 1239
	// (Invoke) Token: 0x06001E2E RID: 7726
	public delegate void RigDuplicationZoneAction(RigDuplicationZone z);
}
