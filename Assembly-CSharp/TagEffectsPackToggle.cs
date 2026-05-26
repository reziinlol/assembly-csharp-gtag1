using System;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using TagEffects;
using UnityEngine;

// Token: 0x020002F9 RID: 761
public class TagEffectsPackToggle : MonoBehaviour, ISpawnable
{
	// Token: 0x170001E9 RID: 489
	// (get) Token: 0x06001364 RID: 4964 RVA: 0x000666C9 File Offset: 0x000648C9
	// (set) Token: 0x06001365 RID: 4965 RVA: 0x000666D1 File Offset: 0x000648D1
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001EA RID: 490
	// (get) Token: 0x06001366 RID: 4966 RVA: 0x000666DA File Offset: 0x000648DA
	// (set) Token: 0x06001367 RID: 4967 RVA: 0x000666E2 File Offset: 0x000648E2
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x06001368 RID: 4968 RVA: 0x000666EB File Offset: 0x000648EB
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this._rig = rig;
	}

	// Token: 0x06001369 RID: 4969 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x0600136A RID: 4970 RVA: 0x000666F4 File Offset: 0x000648F4
	private void OnEnable()
	{
		this.Apply();
	}

	// Token: 0x0600136B RID: 4971 RVA: 0x000666FC File Offset: 0x000648FC
	private void OnDisable()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this.Remove();
	}

	// Token: 0x0600136C RID: 4972 RVA: 0x0006670C File Offset: 0x0006490C
	public void Apply()
	{
		this._rig.CosmeticEffectPack = this.tagEffectPack;
	}

	// Token: 0x0600136D RID: 4973 RVA: 0x0006671F File Offset: 0x0006491F
	public void Remove()
	{
		this._rig.CosmeticEffectPack = null;
	}

	// Token: 0x040017CC RID: 6092
	private VRRig _rig;

	// Token: 0x040017CD RID: 6093
	[SerializeField]
	private TagEffectPack tagEffectPack;
}
