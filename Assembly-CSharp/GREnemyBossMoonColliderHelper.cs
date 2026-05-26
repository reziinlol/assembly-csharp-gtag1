using System;
using UnityEngine;

// Token: 0x0200077E RID: 1918
public class GREnemyBossMoonColliderHelper : MonoBehaviour
{
	// Token: 0x060030B2 RID: 12466 RVA: 0x00109092 File Offset: 0x00107292
	public void Awake()
	{
		if (this.ResizeOnAwake)
		{
			base.transform.localScale = this.ResizeCollider;
		}
	}

	// Token: 0x060030B3 RID: 12467 RVA: 0x001090B0 File Offset: 0x001072B0
	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("GorillaPlayer"))
		{
			VRRig component = other.attachedRigidbody.GetComponent<VRRig>();
			if (component != null && component == VRRigCache.Instance.localRig.Rig && Time.time - this.lastTriggered > 0.5f)
			{
				if (this.localPlayer == null)
				{
					this.localPlayer = VRRig.LocalRig.GetComponent<GRPlayer>();
				}
				this.lastTriggered = Time.time;
				this.boss.HitPlayer(this.localPlayer, true);
				this.boss.ShockPlayer();
			}
		}
	}

	// Token: 0x04003E8E RID: 16014
	public bool ResizeOnAwake = true;

	// Token: 0x04003E8F RID: 16015
	public Vector3 ResizeCollider = new Vector3(1.025f, 1.025f, 1.025f);

	// Token: 0x04003E90 RID: 16016
	[SerializeField]
	private GREnemyBossMoon boss;

	// Token: 0x04003E91 RID: 16017
	[SerializeField]
	private GRPlayer localPlayer;

	// Token: 0x04003E92 RID: 16018
	private float lastTriggered;
}
