using System;
using UnityEngine;

// Token: 0x020007CB RID: 1995
public class GRReviveMeter : MonoBehaviourTick
{
	// Token: 0x060032E8 RID: 13032 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Awake()
	{
	}

	// Token: 0x060032E9 RID: 13033 RVA: 0x0011706C File Offset: 0x0011526C
	public override void Tick()
	{
		float num = 0f;
		if (this.reviveStation != null && VRRig.LocalRig.OwningNetPlayer != null && this.reviveStation.GetReviveCooldownSeconds() > 0.0)
		{
			num = (float)this.reviveStation.CalculateRemainingReviveCooldownSeconds(VRRig.LocalRig.OwningNetPlayer.ActorNumber) / (float)this.reviveStation.GetReviveCooldownSeconds();
		}
		num = Mathf.Clamp(num, 0f, 1f);
		num = 1f - num;
		this.meter.localScale = new Vector3(1f, num, 1f);
	}

	// Token: 0x0400422C RID: 16940
	[SerializeField]
	private GRReviveStation reviveStation;

	// Token: 0x0400422D RID: 16941
	[SerializeField]
	private Transform meter;
}
