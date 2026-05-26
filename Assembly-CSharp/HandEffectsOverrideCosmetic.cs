using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020003D1 RID: 977
public class HandEffectsOverrideCosmetic : MonoBehaviour, ISpawnable
{
	// Token: 0x17000237 RID: 567
	// (get) Token: 0x06001741 RID: 5953 RVA: 0x000861C4 File Offset: 0x000843C4
	// (set) Token: 0x06001742 RID: 5954 RVA: 0x000861CC File Offset: 0x000843CC
	public bool IsSpawned { get; set; }

	// Token: 0x17000238 RID: 568
	// (get) Token: 0x06001743 RID: 5955 RVA: 0x000861D5 File Offset: 0x000843D5
	// (set) Token: 0x06001744 RID: 5956 RVA: 0x000861DD File Offset: 0x000843DD
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001745 RID: 5957 RVA: 0x000861E6 File Offset: 0x000843E6
	public void OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDespawn()
	{
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x000861EF File Offset: 0x000843EF
	public void OnEnable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Add(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Add(this);
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x0008621C File Offset: 0x0008441C
	public void OnDisable()
	{
		if (!this.isLeftHand)
		{
			this._rig.CosmeticHandEffectsOverride_Right.Remove(this);
			return;
		}
		this._rig.CosmeticHandEffectsOverride_Left.Remove(this);
	}

	// Token: 0x0400227C RID: 8828
	public HandEffectsOverrideCosmetic.HandEffectType handEffectType;

	// Token: 0x0400227D RID: 8829
	public bool isLeftHand;

	// Token: 0x0400227E RID: 8830
	public HandEffectsOverrideCosmetic.EffectsOverride firstPerson;

	// Token: 0x0400227F RID: 8831
	public HandEffectsOverrideCosmetic.EffectsOverride thirdPerson;

	// Token: 0x04002280 RID: 8832
	private VRRig _rig;

	// Token: 0x020003D2 RID: 978
	[Serializable]
	public class EffectsOverride
	{
		// Token: 0x04002283 RID: 8835
		public GameObject effectVFX;

		// Token: 0x04002284 RID: 8836
		public bool playHaptics;

		// Token: 0x04002285 RID: 8837
		public float hapticStrength = 0.5f;

		// Token: 0x04002286 RID: 8838
		public float hapticDuration = 0.5f;

		// Token: 0x04002287 RID: 8839
		public bool parentEffect;
	}

	// Token: 0x020003D3 RID: 979
	public enum HandEffectType
	{
		// Token: 0x04002289 RID: 8841
		None,
		// Token: 0x0400228A RID: 8842
		FistBump,
		// Token: 0x0400228B RID: 8843
		HighFive
	}
}
