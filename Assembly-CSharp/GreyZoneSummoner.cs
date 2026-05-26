using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000233 RID: 563
public class GreyZoneSummoner : MonoBehaviour
{
	// Token: 0x17000175 RID: 373
	// (get) Token: 0x06000EF8 RID: 3832 RVA: 0x00052020 File Offset: 0x00050220
	public Vector3 SummoningFocusPoint
	{
		get
		{
			return this.summoningFocusPoint.position;
		}
	}

	// Token: 0x17000176 RID: 374
	// (get) Token: 0x06000EF9 RID: 3833 RVA: 0x0005202D File Offset: 0x0005022D
	public float SummonerMaxDistance
	{
		get
		{
			return this.areaTriggerCollider.radius + 1f;
		}
	}

	// Token: 0x06000EFA RID: 3834 RVA: 0x00052040 File Offset: 0x00050240
	private void OnEnable()
	{
		this.greyZoneManager = GreyZoneManager.Instance;
		if (this.greyZoneManager == null)
		{
			return;
		}
		this.greyZoneManager.RegisterSummoner(this);
		this.areaTriggerNotifier.TriggerEnterEvent += this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent += this.ColliderExitedArea;
	}

	// Token: 0x06000EFB RID: 3835 RVA: 0x000520A4 File Offset: 0x000502A4
	private void OnDisable()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager.Instance.DeregisterSummoner(this);
		}
		this.areaTriggerNotifier.TriggerEnterEvent -= this.ColliderEnteredArea;
		this.areaTriggerNotifier.TriggerExitEvent -= this.ColliderExitedArea;
	}

	// Token: 0x06000EFC RID: 3836 RVA: 0x000520FC File Offset: 0x000502FC
	public void UpdateProgressFeedback(bool greyZoneAvailable)
	{
		if (this.greyZoneManager == null)
		{
			return;
		}
		if (greyZoneAvailable && !this.candlesParent.gameObject.activeSelf)
		{
			this.candlesParent.gameObject.SetActive(true);
		}
		this.candlesTimeline.time = (double)Mathf.Clamp01(this.greyZoneManager.SummoningProgress) * this.candlesTimeline.duration;
		this.candlesTimeline.Evaluate();
		if (!this.greyZoneManager.GreyZoneActive)
		{
			float value = (float)this.summoningTones.Count * this.greyZoneManager.SummoningProgress;
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				float num = Mathf.InverseLerp((float)i, (float)i + 1f + this.summoningTonesFadeOverlap, value);
				this.summoningTones[i].volume = num * this.summoningTonesMaxVolume;
			}
		}
		this.greyZoneActivationButton.isOn = this.greyZoneManager.GreyZoneActive;
		this.greyZoneActivationButton.UpdateColor();
		for (int j = 0; j < this.greyZoneGravityFactorButtons.Count; j++)
		{
			this.greyZoneGravityFactorButtons[j].isOn = (this.greyZoneManager.GravityFactorSelection == j);
			this.greyZoneGravityFactorButtons[j].UpdateColor();
		}
	}

	// Token: 0x06000EFD RID: 3837 RVA: 0x00052245 File Offset: 0x00050445
	public void OnGreyZoneActivated()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.FadeOutSummoningTones());
	}

	// Token: 0x06000EFE RID: 3838 RVA: 0x0005225A File Offset: 0x0005045A
	private IEnumerator FadeOutSummoningTones()
	{
		float fadeStartTime = Time.time;
		float fadeRate = 1f / this.summoningTonesFadeTime;
		while (Time.time < fadeStartTime + this.summoningTonesFadeTime)
		{
			for (int i = 0; i < this.summoningTones.Count; i++)
			{
				this.summoningTones[i].volume = Mathf.MoveTowards(this.summoningTones[i].volume, 0f, this.summoningTonesMaxVolume * fadeRate * Time.deltaTime);
			}
			yield return null;
		}
		for (int j = 0; j < this.summoningTones.Count; j++)
		{
			this.summoningTones[j].volume = 0f;
		}
		yield break;
	}

	// Token: 0x06000EFF RID: 3839 RVA: 0x0005226C File Offset: 0x0005046C
	public void ColliderEnteredArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntityBSP component = other.GetComponent<ZoneEntityBSP>();
		VRRig vrrig = (component != null) ? component.entityRig : null;
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigEnteredSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x06000F00 RID: 3840 RVA: 0x000522B8 File Offset: 0x000504B8
	public void ColliderExitedArea(TriggerEventNotifier notifier, Collider other)
	{
		ZoneEntityBSP component = other.GetComponent<ZoneEntityBSP>();
		VRRig vrrig = (component != null) ? component.entityRig : null;
		if (vrrig != null && this.greyZoneManager != null)
		{
			this.greyZoneManager.VRRigExitedSummonerProximity(vrrig, this);
		}
	}

	// Token: 0x0400121B RID: 4635
	[SerializeField]
	private Transform summoningFocusPoint;

	// Token: 0x0400121C RID: 4636
	[SerializeField]
	private Transform candlesParent;

	// Token: 0x0400121D RID: 4637
	[SerializeField]
	private PlayableDirector candlesTimeline;

	// Token: 0x0400121E RID: 4638
	[SerializeField]
	private TriggerEventNotifier areaTriggerNotifier;

	// Token: 0x0400121F RID: 4639
	[SerializeField]
	private SphereCollider areaTriggerCollider;

	// Token: 0x04001220 RID: 4640
	[SerializeField]
	private GorillaPressableButton greyZoneActivationButton;

	// Token: 0x04001221 RID: 4641
	[SerializeField]
	private List<AudioSource> summoningTones = new List<AudioSource>();

	// Token: 0x04001222 RID: 4642
	[SerializeField]
	private float summoningTonesMaxVolume = 1f;

	// Token: 0x04001223 RID: 4643
	[SerializeField]
	private float summoningTonesFadeOverlap = 0.5f;

	// Token: 0x04001224 RID: 4644
	[SerializeField]
	private float summoningTonesFadeTime = 4f;

	// Token: 0x04001225 RID: 4645
	[SerializeField]
	private List<GorillaPressableButton> greyZoneGravityFactorButtons = new List<GorillaPressableButton>();

	// Token: 0x04001226 RID: 4646
	private GreyZoneManager greyZoneManager;
}
