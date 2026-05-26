using System;
using UnityEngine;

// Token: 0x020002B7 RID: 695
public class Crossbow : ProjectileWeapon
{
	// Token: 0x060011EF RID: 4591 RVA: 0x00060350 File Offset: 0x0005E550
	protected override void Awake()
	{
		base.Awake();
		TransferrableObjectHoldablePart_Crank[] array = this.cranks;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetOnCrankedCallback(new Action<float>(this.OnCrank));
		}
		this.SetReloadFraction(0f);
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x00060398 File Offset: 0x0005E598
	public void SetReloadFraction(float newFraction)
	{
		this.loadFraction = Mathf.Clamp01(newFraction);
		this.animator.SetFloat(this.ReloadFractionHashID, this.loadFraction);
		if (this.loadFraction == 1f && !this.dummyProjectile.enabled)
		{
			this.shootSfx.GTPlayOneShot(this.reloadComplete_audioClip, 1f);
			this.dummyProjectile.enabled = true;
			return;
		}
		if (this.loadFraction < 1f && this.dummyProjectile.enabled)
		{
			this.dummyProjectile.enabled = false;
		}
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x00060430 File Offset: 0x0005E630
	private void OnCrank(float degrees)
	{
		if (this.loadFraction == 1f)
		{
			return;
		}
		this.totalCrankDegrees += degrees;
		this.crankSoundDegrees += degrees;
		if (Mathf.Abs(this.crankSoundDegrees) > this.crankSoundDegreesThreshold)
		{
			this.playingCrankSoundUntilTimestamp = Time.time + this.crankSoundContinueDuration;
			this.crankSoundDegrees = 0f;
		}
		if (!this.reloadAudio.isPlaying && Time.time < this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTPlay();
		}
		this.SetReloadFraction(Mathf.Abs(this.totalCrankDegrees / this.crankTotalDegreesToReload));
		if (this.loadFraction >= 1f)
		{
			this.totalCrankDegrees = 0f;
		}
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x000604EC File Offset: 0x0005E6EC
	protected override Vector3 GetLaunchPosition()
	{
		return this.launchPosition.position;
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x000604F9 File Offset: 0x0005E6F9
	protected override Vector3 GetLaunchVelocity()
	{
		return this.launchPosition.forward * this.launchSpeed * base.myRig.scaleFactor;
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x00060524 File Offset: 0x0005E724
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (!base.InHand())
		{
			this.wasPressingTrigger = false;
			return;
		}
		if ((base.InLeftHand() ? base.myRig.leftIndex.calcT : base.myRig.rightIndex.calcT) > 0.5f)
		{
			if (this.loadFraction == 1f && !this.wasPressingTrigger)
			{
				this.SetReloadFraction(0f);
				this.animator.SetTrigger(this.FireHashID);
				base.LaunchProjectile();
			}
			this.wasPressingTrigger = true;
		}
		else
		{
			this.wasPressingTrigger = false;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			if (this.loadFraction < 1f)
			{
				this.itemState &= (TransferrableObject.ItemStates)(-2);
				return;
			}
		}
		else if (this.loadFraction == 1f)
		{
			this.itemState |= TransferrableObject.ItemStates.State0;
		}
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x00060614 File Offset: 0x0005E814
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (!base.InHand())
		{
			return;
		}
		if (this.itemState.HasFlag(TransferrableObject.ItemStates.State0))
		{
			this.SetReloadFraction(1f);
			return;
		}
		if (this.loadFraction == 1f)
		{
			this.SetReloadFraction(0f);
		}
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x0006066C File Offset: 0x0005E86C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.reloadAudio.isPlaying && Time.time > this.playingCrankSoundUntilTimestamp)
		{
			this.reloadAudio.GTStop();
		}
	}

	// Token: 0x040015AE RID: 5550
	[SerializeField]
	private Transform launchPosition;

	// Token: 0x040015AF RID: 5551
	[SerializeField]
	private float launchSpeed;

	// Token: 0x040015B0 RID: 5552
	[SerializeField]
	private Animator animator;

	// Token: 0x040015B1 RID: 5553
	[SerializeField]
	private float crankTotalDegreesToReload;

	// Token: 0x040015B2 RID: 5554
	[SerializeField]
	private TransferrableObjectHoldablePart_Crank[] cranks;

	// Token: 0x040015B3 RID: 5555
	[SerializeField]
	private MeshRenderer dummyProjectile;

	// Token: 0x040015B4 RID: 5556
	[SerializeField]
	private AudioSource reloadAudio;

	// Token: 0x040015B5 RID: 5557
	[SerializeField]
	private AudioClip reloadComplete_audioClip;

	// Token: 0x040015B6 RID: 5558
	[SerializeField]
	private float crankSoundContinueDuration = 0.1f;

	// Token: 0x040015B7 RID: 5559
	[SerializeField]
	private float crankSoundDegreesThreshold = 0.1f;

	// Token: 0x040015B8 RID: 5560
	private AnimHashId FireHashID = "Fire";

	// Token: 0x040015B9 RID: 5561
	private AnimHashId ReloadFractionHashID = "ReloadFraction";

	// Token: 0x040015BA RID: 5562
	private float totalCrankDegrees;

	// Token: 0x040015BB RID: 5563
	private float loadFraction;

	// Token: 0x040015BC RID: 5564
	private float playingCrankSoundUntilTimestamp;

	// Token: 0x040015BD RID: 5565
	private float crankSoundDegrees;

	// Token: 0x040015BE RID: 5566
	private bool wasPressingTrigger;
}
