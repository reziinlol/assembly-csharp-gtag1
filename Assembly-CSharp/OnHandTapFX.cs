using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000846 RID: 2118
internal struct OnHandTapFX : IFXEffectContext<HandEffectContext>
{
	// Token: 0x170004D5 RID: 1237
	// (get) Token: 0x060036B3 RID: 14003 RVA: 0x0012D1C8 File Offset: 0x0012B3C8
	public HandEffectContext effectContext
	{
		get
		{
			HandEffectContext handEffect = this.rig.GetHandEffect(this.isLeftHand, this.stiltID);
			this.rig.SetHandEffectData(handEffect, this.surfaceIndex, this.isDownTap, this.isLeftHand, this.stiltID, this.volume, this.speed, this.tapDir);
			return handEffect;
		}
	}

	// Token: 0x170004D6 RID: 1238
	// (get) Token: 0x060036B4 RID: 14004 RVA: 0x0012D224 File Offset: 0x0012B424
	public FXSystemSettings settings
	{
		get
		{
			return this.rig.fxSettings;
		}
	}

	// Token: 0x040046F2 RID: 18162
	public VRRig rig;

	// Token: 0x040046F3 RID: 18163
	public Vector3 tapDir;

	// Token: 0x040046F4 RID: 18164
	public bool isDownTap;

	// Token: 0x040046F5 RID: 18165
	public bool isLeftHand;

	// Token: 0x040046F6 RID: 18166
	public StiltID stiltID;

	// Token: 0x040046F7 RID: 18167
	public int surfaceIndex;

	// Token: 0x040046F8 RID: 18168
	public float volume;

	// Token: 0x040046F9 RID: 18169
	public float speed;
}
