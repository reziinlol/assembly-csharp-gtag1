using System;
using UnityEngine;

// Token: 0x020006D4 RID: 1748
public class GameLight : MonoBehaviour
{
	// Token: 0x1700044B RID: 1099
	// (get) Token: 0x06002BF4 RID: 11252 RVA: 0x000EDCCC File Offset: 0x000EBECC
	public bool IsRegistered
	{
		get
		{
			return this.lightId != -1;
		}
	}

	// Token: 0x1700044C RID: 1100
	// (get) Token: 0x06002BF5 RID: 11253 RVA: 0x000EDCDA File Offset: 0x000EBEDA
	// (set) Token: 0x06002BF6 RID: 11254 RVA: 0x000EDCE2 File Offset: 0x000EBEE2
	public float InitialIntensity { get; private set; }

	// Token: 0x06002BF7 RID: 11255 RVA: 0x000EDCEB File Offset: 0x000EBEEB
	public void Awake()
	{
		this.intensityMult = 1;
		this.lightId = -1;
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x000EDCFB File Offset: 0x000EBEFB
	protected void OnEnable()
	{
		if (this.initialized)
		{
			this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		}
	}

	// Token: 0x06002BF9 RID: 11257 RVA: 0x000EDD19 File Offset: 0x000EBF19
	protected void Start()
	{
		this.lightId = GameLightingManager.instance.AddGameLight(this, false);
		this.initialized = true;
	}

	// Token: 0x06002BFA RID: 11258 RVA: 0x000EDD36 File Offset: 0x000EBF36
	protected void OnDisable()
	{
		if (this.initialized)
		{
			GameLightingManager.instance.RemoveGameLight(this);
		}
	}

	// Token: 0x06002BFB RID: 11259 RVA: 0x000EDD50 File Offset: 0x000EBF50
	public void UpdateCachedLightColorAndIntensity()
	{
		this.cachedColorAndIntensity = (float)this.intensityMult * this.light.intensity * (this.negativeLight ? -1f : 1f) * this.light.color;
	}

	// Token: 0x0400385E RID: 14430
	public Light light;

	// Token: 0x0400385F RID: 14431
	public bool negativeLight;

	// Token: 0x04003860 RID: 14432
	public bool isHighPriorityPlayerLight;

	// Token: 0x04003861 RID: 14433
	public Vector3 cachedPosition;

	// Token: 0x04003862 RID: 14434
	public Vector4 cachedColorAndIntensity;

	// Token: 0x04003863 RID: 14435
	public int lightId = -1;

	// Token: 0x04003864 RID: 14436
	public int intensityMult = 1;

	// Token: 0x04003865 RID: 14437
	private bool initialized;
}
