using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000550 RID: 1360
public class UmbrellaItem : TransferrableObject
{
	// Token: 0x0600229B RID: 8859 RVA: 0x000B9C2A File Offset: 0x000B7E2A
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State1;
	}

	// Token: 0x0600229C RID: 8860 RVA: 0x000B9C3C File Offset: 0x000B7E3C
	public override void OnActivate()
	{
		base.OnActivate();
		float hapticStrength = GorillaTagger.Instance.tapHapticStrength / 4f;
		float fixedDeltaTime = Time.fixedDeltaTime;
		float soundVolume = 0.08f;
		int soundIndex;
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			soundIndex = this.SoundIdOpen;
			this.itemState = TransferrableObject.ItemStates.State0;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Add(this.umbrellaRainDestroyTrigger);
		}
		else
		{
			soundIndex = this.SoundIdClose;
			this.itemState = TransferrableObject.ItemStates.State1;
			BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		}
		base.ActivateItemFX(hapticStrength, fixedDeltaTime, soundIndex, soundVolume);
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x0600229D RID: 8861 RVA: 0x000B9CD4 File Offset: 0x000B7ED4
	internal override void OnEnable()
	{
		base.OnEnable();
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x0600229E RID: 8862 RVA: 0x000B9CE2 File Offset: 0x000B7EE2
	internal override void OnDisable()
	{
		base.OnDisable();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
	}

	// Token: 0x0600229F RID: 8863 RVA: 0x000B9D02 File Offset: 0x000B7F02
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		BetterDayNightManager.instance.collidersToAddToWeatherSystems.Remove(this.umbrellaRainDestroyTrigger);
		this.itemState = TransferrableObject.ItemStates.State1;
		this.OnUmbrellaStateChanged();
	}

	// Token: 0x060022A0 RID: 8864 RVA: 0x000B9D2F File Offset: 0x000B7F2F
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.OnActivate();
		}
		return true;
	}

	// Token: 0x060022A1 RID: 8865 RVA: 0x000B9D58 File Offset: 0x000B7F58
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		UmbrellaItem.UmbrellaStates itemState = (UmbrellaItem.UmbrellaStates)this.itemState;
		if (itemState != this.previousUmbrellaState)
		{
			this.OnUmbrellaStateChanged();
		}
		this.UpdateAngles((itemState == UmbrellaItem.UmbrellaStates.UmbrellaOpen) ? this.startingAngles : this.endingAngles, this.lerpValue);
		this.previousUmbrellaState = itemState;
	}

	// Token: 0x060022A2 RID: 8866 RVA: 0x000B9DA8 File Offset: 0x000B7FA8
	protected virtual void OnUmbrellaStateChanged()
	{
		bool flag = this.itemState == TransferrableObject.ItemStates.State0;
		GameObject[] array = this.gameObjectsActivatedOnOpen;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(flag);
		}
		ParticleSystem[] array2;
		if (flag)
		{
			array2 = this.particlesEmitOnOpen;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			return;
		}
		array2 = this.particlesEmitOnOpen;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Stop();
		}
	}

	// Token: 0x060022A3 RID: 8867 RVA: 0x000B9E1C File Offset: 0x000B801C
	protected virtual void UpdateAngles(Quaternion[] toAngles, float t)
	{
		for (int i = 0; i < this.umbrellaBones.Length; i++)
		{
			this.umbrellaBones[i].localRotation = Quaternion.Lerp(this.umbrellaBones[i].localRotation, toAngles[i], t);
		}
	}

	// Token: 0x060022A4 RID: 8868 RVA: 0x000B9E64 File Offset: 0x000B8064
	protected void GenerateAngles()
	{
		this.startingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int i = 0; i < this.endingAngles.Length; i++)
		{
			this.startingAngles[i] = this.umbrellaToCopy.startingAngles[i];
		}
		this.endingAngles = new Quaternion[this.umbrellaBones.Length];
		for (int j = 0; j < this.endingAngles.Length; j++)
		{
			this.endingAngles[j] = this.umbrellaToCopy.endingAngles[j];
		}
	}

	// Token: 0x060022A5 RID: 8869 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool CanActivate()
	{
		return true;
	}

	// Token: 0x060022A6 RID: 8870 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool CanDeactivate()
	{
		return true;
	}

	// Token: 0x04002DA5 RID: 11685
	[AssignInCorePrefab]
	public Transform[] umbrellaBones;

	// Token: 0x04002DA6 RID: 11686
	[AssignInCorePrefab]
	public Quaternion[] startingAngles;

	// Token: 0x04002DA7 RID: 11687
	[AssignInCorePrefab]
	public Quaternion[] endingAngles;

	// Token: 0x04002DA8 RID: 11688
	[AssignInCorePrefab]
	[Tooltip("Assign to use the 'Generate Angles' button")]
	private UmbrellaItem umbrellaToCopy;

	// Token: 0x04002DA9 RID: 11689
	[AssignInCorePrefab]
	public float lerpValue = 0.25f;

	// Token: 0x04002DAA RID: 11690
	[AssignInCorePrefab]
	public Collider umbrellaRainDestroyTrigger;

	// Token: 0x04002DAB RID: 11691
	[AssignInCorePrefab]
	public GameObject[] gameObjectsActivatedOnOpen;

	// Token: 0x04002DAC RID: 11692
	[AssignInCorePrefab]
	public ParticleSystem[] particlesEmitOnOpen;

	// Token: 0x04002DAD RID: 11693
	[GorillaSoundLookup]
	public int SoundIdOpen = 64;

	// Token: 0x04002DAE RID: 11694
	[GorillaSoundLookup]
	public int SoundIdClose = 65;

	// Token: 0x04002DAF RID: 11695
	private UmbrellaItem.UmbrellaStates previousUmbrellaState = UmbrellaItem.UmbrellaStates.UmbrellaOpen;

	// Token: 0x02000551 RID: 1361
	private enum UmbrellaStates
	{
		// Token: 0x04002DB1 RID: 11697
		UmbrellaOpen = 1,
		// Token: 0x04002DB2 RID: 11698
		UmbrellaClosed
	}
}
