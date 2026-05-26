using System;
using UnityEngine;

// Token: 0x02000078 RID: 120
public class CrittersStickyGoo : CrittersActor
{
	// Token: 0x060002ED RID: 749 RVA: 0x0001174B File Offset: 0x0000F94B
	public override void Initialize()
	{
		base.Initialize();
		this.readyToDisable = false;
	}

	// Token: 0x060002EE RID: 750 RVA: 0x0001175C File Offset: 0x0000F95C
	public bool CanAffect(Vector3 position)
	{
		return (base.transform.position - position).magnitude < this.range;
	}

	// Token: 0x060002EF RID: 751 RVA: 0x0001178A File Offset: 0x0000F98A
	public void EffectApplied(CrittersPawn critter)
	{
		if (this.destroyOnApply)
		{
			this.readyToDisable = true;
		}
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.StickyTriggered, this.actorId, critter.transform.position, Quaternion.LookRotation(critter.transform.up));
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x000117CC File Offset: 0x0000F9CC
	public override bool ProcessLocal()
	{
		bool result = base.ProcessLocal();
		if (this.readyToDisable)
		{
			base.gameObject.SetActive(false);
			return true;
		}
		return result;
	}

	// Token: 0x04000357 RID: 855
	[Header("Sticky Goo")]
	public float range = 1f;

	// Token: 0x04000358 RID: 856
	public float slowModifier = 0.3f;

	// Token: 0x04000359 RID: 857
	public float slowDuration = 3f;

	// Token: 0x0400035A RID: 858
	public bool destroyOnApply = true;

	// Token: 0x0400035B RID: 859
	private bool readyToDisable;
}
