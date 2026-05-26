using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
public class CrittersActorSettings : MonoBehaviour
{
	// Token: 0x06000188 RID: 392 RVA: 0x00009C6A File Offset: 0x00007E6A
	public virtual void OnEnable()
	{
		this.UpdateActorSettings();
	}

	// Token: 0x06000189 RID: 393 RVA: 0x00009C74 File Offset: 0x00007E74
	public virtual void UpdateActorSettings()
	{
		this.parentActor.usesRB = this.usesRB;
		this.parentActor.rb.isKinematic = !this.usesRB;
		this.parentActor.equipmentStorable = this.canBeStored;
		this.parentActor.storeCollider = this.storeCollider;
		this.parentActor.equipmentStoreTriggerCollider = this.equipmentStoreTriggerCollider;
	}

	// Token: 0x040001A5 RID: 421
	public CrittersActor parentActor;

	// Token: 0x040001A6 RID: 422
	public bool usesRB;

	// Token: 0x040001A7 RID: 423
	public bool canBeStored;

	// Token: 0x040001A8 RID: 424
	public CapsuleCollider storeCollider;

	// Token: 0x040001A9 RID: 425
	public CapsuleCollider equipmentStoreTriggerCollider;
}
