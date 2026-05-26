using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x020007A6 RID: 1958
public class GRHazardousMaterial : MonoBehaviour
{
	// Token: 0x06003201 RID: 12801 RVA: 0x00112F2B File Offset: 0x0011112B
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x06003202 RID: 12802 RVA: 0x00112F34 File Offset: 0x00111134
	public void OnLocalPlayerOverlap()
	{
		GRPlayer component = VRRig.LocalRig.GetComponent<GRPlayer>();
		if (component != null && component.State == GRPlayer.GRPlayerState.Alive)
		{
			this.reactor.grManager.RequestPlayerStateChange(component, GRPlayer.GRPlayerState.Ghost);
		}
	}

	// Token: 0x06003203 RID: 12803 RVA: 0x00112F6F File Offset: 0x0011116F
	private void OnTriggerEnter(Collider collider)
	{
		if (collider == GTPlayer.Instance.headCollider || collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x06003204 RID: 12804 RVA: 0x00112F9B File Offset: 0x0011119B
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider == GTPlayer.Instance.headCollider || collision.collider == GTPlayer.Instance.bodyCollider)
		{
			this.OnLocalPlayerOverlap();
		}
	}

	// Token: 0x040040F5 RID: 16629
	private GhostReactor reactor;
}
