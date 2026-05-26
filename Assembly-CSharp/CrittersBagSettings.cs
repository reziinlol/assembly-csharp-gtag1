using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class CrittersBagSettings : CrittersActorSettings
{
	// Token: 0x060001AB RID: 427 RVA: 0x0000A51C File Offset: 0x0000871C
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersBag crittersBag = (CrittersBag)this.parentActor;
		crittersBag.attachableCollider = this.attachableCollider;
		crittersBag.dropCube = this.dropCube;
		crittersBag.anchorLocation = this.anchorLocation;
		crittersBag.attachDisableColliders = this.attachDisableColliders;
		crittersBag.attachSound = this.attachSound;
		crittersBag.detachSound = this.detachSound;
		crittersBag.blockAttachTypes = this.blockAttachTypes;
	}

	// Token: 0x040001D1 RID: 465
	public Collider attachableCollider;

	// Token: 0x040001D2 RID: 466
	public BoxCollider dropCube;

	// Token: 0x040001D3 RID: 467
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001D4 RID: 468
	public List<Collider> attachDisableColliders;

	// Token: 0x040001D5 RID: 469
	public AudioClip attachSound;

	// Token: 0x040001D6 RID: 470
	public AudioClip detachSound;

	// Token: 0x040001D7 RID: 471
	public List<CrittersActor.CrittersActorType> blockAttachTypes;
}
