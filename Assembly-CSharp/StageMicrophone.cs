using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020009D7 RID: 2519
public class StageMicrophone : MonoBehaviour
{
	// Token: 0x06004078 RID: 16504 RVA: 0x00158643 File Offset: 0x00156843
	private void Awake()
	{
		StageMicrophone.Instance = this;
	}

	// Token: 0x06004079 RID: 16505 RVA: 0x0015864B File Offset: 0x0015684B
	public bool IsPlayerAmplified(VRRig player)
	{
		return (player.GetMouthPosition() - base.transform.position).IsShorterThan(this.PickupRadius);
	}

	// Token: 0x0600407A RID: 16506 RVA: 0x0015866E File Offset: 0x0015686E
	public float GetPlayerSpatialBlend(VRRig player)
	{
		if (!this.IsPlayerAmplified(player))
		{
			return 0.9f;
		}
		return this.AmplifiedSpatialBlend;
	}

	// Token: 0x04005110 RID: 20752
	public static StageMicrophone Instance;

	// Token: 0x04005111 RID: 20753
	[SerializeField]
	private float PickupRadius;

	// Token: 0x04005112 RID: 20754
	[SerializeField]
	private float AmplifiedSpatialBlend;
}
