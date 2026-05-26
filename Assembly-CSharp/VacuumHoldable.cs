using System;
using UnityEngine;

// Token: 0x02000213 RID: 531
public class VacuumHoldable : TransferrableObject
{
	// Token: 0x06000DEA RID: 3562 RVA: 0x0004C284 File Offset: 0x0004A484
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06000DEB RID: 3563 RVA: 0x0004C294 File Offset: 0x0004A494
	internal override void OnEnable()
	{
		base.OnEnable();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.hasAudioSource = (this.audioSource != null && this.audioSource.clip != null);
	}

	// Token: 0x06000DEC RID: 3564 RVA: 0x0004C2CC File Offset: 0x0004A4CC
	internal override void OnDisable()
	{
		base.OnDisable();
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000DED RID: 3565 RVA: 0x0004C320 File Offset: 0x0004A520
	private void InitToDefault()
	{
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.particleFX.isPlaying)
		{
			this.particleFX.Stop();
		}
		if (this.hasAudioSource && this.audioSource.isPlaying)
		{
			this.audioSource.GTStop();
		}
	}

	// Token: 0x06000DEE RID: 3566 RVA: 0x0004C36C File Offset: 0x0004A56C
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
	}

	// Token: 0x06000DEF RID: 3567 RVA: 0x0004C37C File Offset: 0x0004A57C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (!this.IsMyItem() && base.myOnlineRig != null && base.myOnlineRig.muted)
		{
			this.itemState = TransferrableObject.ItemStates.State0;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			if (this.particleFX.isPlaying)
			{
				this.particleFX.Stop();
			}
			if (this.hasAudioSource && this.audioSource.isPlaying)
			{
				this.audioSource.GTStop();
				return;
			}
		}
		else
		{
			if (!this.particleFX.isEmitting)
			{
				this.particleFX.Play();
			}
			if (this.hasAudioSource && !this.audioSource.isPlaying)
			{
				this.audioSource.GTPlay();
			}
			if (this.IsMyItem() && Time.time > this.activationStartTime + this.activationVibrationStartDuration)
			{
				GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationLoopStrength, Time.deltaTime);
			}
		}
	}

	// Token: 0x06000DF0 RID: 3568 RVA: 0x0004C470 File Offset: 0x0004A670
	public override void OnActivate()
	{
		base.OnActivate();
		this.itemState = TransferrableObject.ItemStates.State1;
		if (this.IsMyItem())
		{
			this.activationStartTime = Time.time;
			GorillaTagger.Instance.StartVibration(this.currentState == TransferrableObject.PositionState.InLeftHand, this.activationVibrationStartStrength, this.activationVibrationStartDuration);
		}
	}

	// Token: 0x06000DF1 RID: 3569 RVA: 0x0004C4BC File Offset: 0x0004A6BC
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x040010AE RID: 4270
	[Tooltip("Emission rate will be increase when the trigger button is pressed.")]
	public ParticleSystem particleFX;

	// Token: 0x040010AF RID: 4271
	[Tooltip("Sound will loop and fade in/out volume when trigger pressed.")]
	public AudioSource audioSource;

	// Token: 0x040010B0 RID: 4272
	private float activationVibrationStartStrength = 0.8f;

	// Token: 0x040010B1 RID: 4273
	private float activationVibrationStartDuration = 0.05f;

	// Token: 0x040010B2 RID: 4274
	private float activationVibrationLoopStrength = 0.005f;

	// Token: 0x040010B3 RID: 4275
	private float activationStartTime;

	// Token: 0x040010B4 RID: 4276
	private bool hasAudioSource;

	// Token: 0x02000214 RID: 532
	private enum VacuumState
	{
		// Token: 0x040010B6 RID: 4278
		None = 1,
		// Token: 0x040010B7 RID: 4279
		Active
	}
}
