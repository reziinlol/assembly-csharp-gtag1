using System;
using System.Collections;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000BD5 RID: 3029
public class LightningManager : MonoBehaviour
{
	// Token: 0x06004BAA RID: 19370 RVA: 0x00194AEA File Offset: 0x00192CEA
	private void Start()
	{
		this.lightningAudio = base.GetComponent<AudioSource>();
		GorillaComputer instance = GorillaComputer.instance;
		instance.OnServerTimeUpdated = (Action)Delegate.Combine(instance.OnServerTimeUpdated, new Action(this.OnTimeChanged));
	}

	// Token: 0x06004BAB RID: 19371 RVA: 0x00194B20 File Offset: 0x00192D20
	private void OnTimeChanged()
	{
		this.InitializeRng();
		if (this.lightningRunner != null)
		{
			base.StopCoroutine(this.lightningRunner);
		}
		this.lightningRunner = base.StartCoroutine(this.LightningEffectRunner());
	}

	// Token: 0x06004BAC RID: 19372 RVA: 0x00194B50 File Offset: 0x00192D50
	private void GetHourStart(out long seed, out float timestampRealtime)
	{
		DateTime serverTime = GorillaComputer.instance.GetServerTime();
		DateTime d = new DateTime(serverTime.Year, serverTime.Month, serverTime.Day, serverTime.Hour, 0, 0);
		timestampRealtime = Time.realtimeSinceStartup - (float)(serverTime - d).TotalSeconds;
		seed = d.Ticks;
	}

	// Token: 0x06004BAD RID: 19373 RVA: 0x00194BB0 File Offset: 0x00192DB0
	private void InitializeRng()
	{
		long seed;
		float num;
		this.GetHourStart(out seed, out num);
		this.currentHourlySeed = seed;
		this.rng = new SRand(seed);
		this.lightningTimestampsRealtime.Clear();
		this.nextLightningTimestampIndex = -1;
		float num2 = num;
		float num3 = 0f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (num3 < 3600f)
		{
			float num4 = this.rng.NextFloat(this.minTimeBetweenFlashes, this.maxTimeBetweenFlashes);
			num3 += num4;
			num2 += num4;
			if (this.nextLightningTimestampIndex == -1 && num2 > realtimeSinceStartup)
			{
				this.nextLightningTimestampIndex = this.lightningTimestampsRealtime.Count;
			}
			this.lightningTimestampsRealtime.Add(num2);
		}
		this.lightningTimestampsRealtime[this.lightningTimestampsRealtime.Count - 1] = num + 3605f;
	}

	// Token: 0x06004BAE RID: 19374 RVA: 0x00194C74 File Offset: 0x00192E74
	internal void DoLightningStrike()
	{
		BetterDayNightManager.instance.AnimateLightFlash(this.lightMapIndex, this.flashFadeInDuration, this.flashHoldDuration, this.flashFadeOutDuration);
		this.lightningAudio.clip = (ZoneManagement.IsInZone(GTZone.cave) ? this.muffledLightning : this.regularLightning);
		this.lightningAudio.GTPlay();
	}

	// Token: 0x06004BAF RID: 19375 RVA: 0x00194CD1 File Offset: 0x00192ED1
	private IEnumerator LightningEffectRunner()
	{
		for (;;)
		{
			if (this.lightningTimestampsRealtime.Count <= this.nextLightningTimestampIndex)
			{
				this.InitializeRng();
			}
			if (this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
			{
				yield return new WaitForSecondsRealtime(this.lightningTimestampsRealtime[this.nextLightningTimestampIndex] - Time.realtimeSinceStartup);
				float num = this.lightningTimestampsRealtime[this.nextLightningTimestampIndex];
				this.nextLightningTimestampIndex++;
				if (Time.realtimeSinceStartup - num < 1f && this.lightningTimestampsRealtime.Count > this.nextLightningTimestampIndex)
				{
					this.DoLightningStrike();
				}
			}
			yield return null;
		}
		yield break;
	}

	// Token: 0x04005EE7 RID: 24295
	public int lightMapIndex;

	// Token: 0x04005EE8 RID: 24296
	public float minTimeBetweenFlashes;

	// Token: 0x04005EE9 RID: 24297
	public float maxTimeBetweenFlashes;

	// Token: 0x04005EEA RID: 24298
	public float flashFadeInDuration;

	// Token: 0x04005EEB RID: 24299
	public float flashHoldDuration;

	// Token: 0x04005EEC RID: 24300
	public float flashFadeOutDuration;

	// Token: 0x04005EED RID: 24301
	private AudioSource lightningAudio;

	// Token: 0x04005EEE RID: 24302
	private SRand rng;

	// Token: 0x04005EEF RID: 24303
	private long currentHourlySeed;

	// Token: 0x04005EF0 RID: 24304
	private List<float> lightningTimestampsRealtime = new List<float>();

	// Token: 0x04005EF1 RID: 24305
	private int nextLightningTimestampIndex;

	// Token: 0x04005EF2 RID: 24306
	public AudioClip regularLightning;

	// Token: 0x04005EF3 RID: 24307
	public AudioClip muffledLightning;

	// Token: 0x04005EF4 RID: 24308
	private Coroutine lightningRunner;
}
