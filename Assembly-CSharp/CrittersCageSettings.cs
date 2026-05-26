using System;
using UnityEngine;

// Token: 0x0200005C RID: 92
public class CrittersCageSettings : CrittersActorSettings
{
	// Token: 0x060001D1 RID: 465 RVA: 0x0000AEAC File Offset: 0x000090AC
	public override void UpdateActorSettings()
	{
		base.UpdateActorSettings();
		CrittersCage crittersCage = (CrittersCage)this.parentActor;
		crittersCage.cagePosition = this.cagePoint;
		crittersCage.grabPosition = this.grabPoint;
	}

	// Token: 0x04000213 RID: 531
	public Transform cagePoint;

	// Token: 0x04000214 RID: 532
	public Transform grabPoint;
}
