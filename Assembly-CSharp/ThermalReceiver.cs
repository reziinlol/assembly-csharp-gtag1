using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A1 RID: 929
public class ThermalReceiver : MonoBehaviour, IDynamicFloat, IResettableItem
{
	// Token: 0x1700022D RID: 557
	// (get) Token: 0x0600167F RID: 5759 RVA: 0x000824FF File Offset: 0x000806FF
	public float Farenheit
	{
		get
		{
			return this.celsius * 1.8f + 32f;
		}
	}

	// Token: 0x1700022E RID: 558
	// (get) Token: 0x06001680 RID: 5760 RVA: 0x00082513 File Offset: 0x00080713
	public float floatValue
	{
		get
		{
			return this.celsius;
		}
	}

	// Token: 0x06001681 RID: 5761 RVA: 0x0008251B File Offset: 0x0008071B
	protected void Awake()
	{
		this.defaultCelsius = this.celsius;
		this.wasAboveThreshold = false;
	}

	// Token: 0x06001682 RID: 5762 RVA: 0x00082530 File Offset: 0x00080730
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06001683 RID: 5763 RVA: 0x00082538 File Offset: 0x00080738
	protected void OnDisable()
	{
		this.wasAboveThreshold = false;
		ThermalManager.Unregister(this);
	}

	// Token: 0x06001684 RID: 5764 RVA: 0x00082547 File Offset: 0x00080747
	public void ResetToDefaultState()
	{
		this.celsius = this.defaultCelsius;
	}

	// Token: 0x0400208D RID: 8333
	public float radius = 0.2f;

	// Token: 0x0400208E RID: 8334
	[Tooltip("How fast the temperature should change overtime. 1.0 would be instantly.")]
	public float conductivity = 0.3f;

	// Token: 0x0400208F RID: 8335
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04002090 RID: 8336
	[Tooltip("Optional: Fire events if temperature goes below or above this threshold - Celsius")]
	public float temperatureThreshold;

	// Token: 0x04002091 RID: 8337
	[Tooltip("Exclude these thermal sources from impacting this receiver")]
	public List<ThermalSourceVolume> exclusionSources = new List<ThermalSourceVolume>();

	// Token: 0x04002092 RID: 8338
	[Space]
	public UnityEvent OnAboveThreshold;

	// Token: 0x04002093 RID: 8339
	public UnityEvent OnBelowThreshold;

	// Token: 0x04002094 RID: 8340
	[DebugOption]
	public float celsius;

	// Token: 0x04002095 RID: 8341
	public bool wasAboveThreshold;

	// Token: 0x04002096 RID: 8342
	private float defaultCelsius;
}
