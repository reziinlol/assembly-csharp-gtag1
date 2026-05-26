using System;
using UnityEngine;

// Token: 0x02000284 RID: 644
public class HatcheryEventFlashlight : MonoBehaviourTick, IGorillaSliceableSimple
{
	// Token: 0x06001151 RID: 4433 RVA: 0x0005CFF0 File Offset: 0x0005B1F0
	private void Awake()
	{
		this.parentRig = base.GetComponentInParent<VRRig>();
		this.playerLight = this.parentRig.isOfflineVRRig;
		this.currentEnergy = 10f;
		this.lightComponents = new Light[this.lights.Length];
		this.gameLightComponents = new GameLight[this.lights.Length];
		for (int i = 0; i < this.lights.Length; i++)
		{
			this.lightComponents[i] = this.lights[i].GetComponent<Light>();
			this.gameLightComponents[i] = this.lights[i].GetComponent<GameLight>();
		}
		this.startingBrightness = this.lightComponents[0].intensity;
		this.lightsParent.gameObject.SetActive(false);
	}

	// Token: 0x06001152 RID: 4434 RVA: 0x0005D0AC File Offset: 0x0005B2AC
	private new void OnEnable()
	{
		base.OnEnable();
		if (!this.playerLight)
		{
			GorillaSlicerSimpleManager.RegisterSliceable(this);
		}
	}

	// Token: 0x06001153 RID: 4435 RVA: 0x0005D0C2 File Offset: 0x0005B2C2
	private new void OnDisable()
	{
		base.OnDisable();
		if (!this.playerLight)
		{
			GorillaSlicerSimpleManager.UnregisterSliceable(this);
		}
	}

	// Token: 0x06001154 RID: 4436 RVA: 0x0005D0D9 File Offset: 0x0005B2D9
	private float MaxEnergy()
	{
		if (NetworkSystem.Instance.CurrentRoom == null)
		{
			return 10f;
		}
		return 10f * (1f / Mathf.Log((float)NetworkSystem.Instance.RoomPlayerCount + 1.72f));
	}

	// Token: 0x06001155 RID: 4437 RVA: 0x0005D10F File Offset: 0x0005B30F
	public override void Tick()
	{
		if (this.playerLight)
		{
			this.SliceUpdate();
		}
	}

	// Token: 0x06001156 RID: 4438 RVA: 0x0005D120 File Offset: 0x0005B320
	public void SliceUpdate()
	{
		if (GameLightingManager.instance.IsDynamicLightingEnabled != this.flashlight.gameObject.activeSelf)
		{
			this.flashlight.gameObject.SetActive(GameLightingManager.instance.IsDynamicLightingEnabled);
		}
		if (!GameLightingManager.instance.IsDynamicLightingEnabled)
		{
			return;
		}
		float time = Time.time;
		float num = this.MaxEnergy();
		if (this.wasLightEnabled)
		{
			this.currentEnergy -= (time - this.lastUpdated) * 1f;
		}
		else
		{
			this.currentEnergy += (time - this.lastUpdated) * 0.66f;
		}
		this.currentEnergy = Mathf.Clamp(this.currentEnergy, 0f, this.MaxEnergy());
		bool flag = this.parentRig.rightIndex.calcT >= 0.33f;
		bool flag2 = flag && (!this.wasLightSwitchedOn || this.wasLightEnabled) && this.currentEnergy > 0f;
		if (flag2 != this.wasLightEnabled)
		{
			this.lightsParent.gameObject.SetActive(flag2);
			this.clickSource.Play();
		}
		if (flag2)
		{
			this.UpdateLightPositioning();
			this.UpdateLightBrightness(num);
		}
		this.lastUpdated = Time.time;
		this.wasLightSwitchedOn = flag;
		this.wasLightEnabled = flag2;
	}

	// Token: 0x06001157 RID: 4439 RVA: 0x0005D26C File Offset: 0x0005B46C
	private void UpdateLightPositioning()
	{
		int num = Physics.RaycastNonAlloc(this.lightStart.position, this.lightStart.forward, this.hits, 6f, -1, QueryTriggerInteraction.Ignore);
		float num2 = 6f;
		for (int i = 0; i < num; i++)
		{
			if (this.hits[i].distance <= num2)
			{
				num2 = this.hits[i].distance;
			}
		}
		float num3 = (num2 >= 2f) ? (num2 - 1f) : (num2 / 2f);
		for (int j = 0; j < this.lights.Length; j++)
		{
			this.lights[j].position = this.lightStart.position + this.lightStart.forward * (num3 * (float)(j + 1) / (float)this.lights.Length);
		}
	}

	// Token: 0x06001158 RID: 4440 RVA: 0x0005D34C File Offset: 0x0005B54C
	private void UpdateLightBrightness(float _maxEnergy)
	{
		float intensity = this.startingBrightness / 5f * (1f + 4f * this.currentEnergy / _maxEnergy);
		for (int i = 0; i < this.lightComponents.Length; i++)
		{
			this.lightComponents[i].intensity = intensity;
			this.gameLightComponents[i].UpdateCachedLightColorAndIntensity();
		}
	}

	// Token: 0x040014A3 RID: 5283
	private const float lightMaxDistance = 6f;

	// Token: 0x040014A4 RID: 5284
	private const float surfaceOffset = 1f;

	// Token: 0x040014A5 RID: 5285
	private const float enableThresholdCurl = 0.33f;

	// Token: 0x040014A6 RID: 5286
	private const float maxEnergy = 10f;

	// Token: 0x040014A7 RID: 5287
	private const float energyUsageRate = 1f;

	// Token: 0x040014A8 RID: 5288
	private const float energyChargeRate = 0.66f;

	// Token: 0x040014A9 RID: 5289
	private RaycastHit[] hits = new RaycastHit[20];

	// Token: 0x040014AA RID: 5290
	private Light[] lightComponents;

	// Token: 0x040014AB RID: 5291
	private GameLight[] gameLightComponents;

	// Token: 0x040014AC RID: 5292
	private VRRig parentRig;

	// Token: 0x040014AD RID: 5293
	private float currentEnergy;

	// Token: 0x040014AE RID: 5294
	private float startingBrightness;

	// Token: 0x040014AF RID: 5295
	private float lastUpdated;

	// Token: 0x040014B0 RID: 5296
	private bool playerLight;

	// Token: 0x040014B1 RID: 5297
	private bool wasLightEnabled;

	// Token: 0x040014B2 RID: 5298
	private bool wasLightSwitchedOn;

	// Token: 0x040014B3 RID: 5299
	public Transform lightStart;

	// Token: 0x040014B4 RID: 5300
	public Transform lightsParent;

	// Token: 0x040014B5 RID: 5301
	public Transform flashlight;

	// Token: 0x040014B6 RID: 5302
	public Transform[] lights;

	// Token: 0x040014B7 RID: 5303
	public AudioSource clickSource;
}
