using System;
using System.Collections.Generic;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020003A0 RID: 928
[DefaultExecutionOrder(-100)]
public class ThermalManager : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06001676 RID: 5750 RVA: 0x000822B4 File Offset: 0x000804B4
	public void OnEnable()
	{
		if (ThermalManager.instance != null)
		{
			Debug.LogError("ThermalManager already exists!");
			return;
		}
		ThermalManager.instance = this;
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
		this.lastTime = Time.time;
	}

	// Token: 0x06001677 RID: 5751 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x06001678 RID: 5752 RVA: 0x000822E8 File Offset: 0x000804E8
	public void SliceUpdate()
	{
		float num = Time.time - this.lastTime;
		this.lastTime = Time.time;
		for (int i = 0; i < ThermalManager.receivers.Count; i++)
		{
			ThermalReceiver thermalReceiver = ThermalManager.receivers[i];
			Transform transform = thermalReceiver.transform;
			Vector3 position = transform.position;
			float x = transform.lossyScale.x;
			float num2 = 20f;
			for (int j = 0; j < ThermalManager.sources.Count; j++)
			{
				ThermalSourceVolume thermalSourceVolume = ThermalManager.sources[j];
				if ((thermalSourceVolume.exclusionReceivers.Count <= 0 || !thermalSourceVolume.exclusionReceivers.Contains(thermalReceiver)) && (thermalReceiver.exclusionSources.Count <= 0 || !thermalReceiver.exclusionSources.Contains(thermalSourceVolume)))
				{
					Transform transform2 = thermalSourceVolume.transform;
					float x2 = transform2.lossyScale.x;
					float num3 = Vector3.Distance(transform2.position, position);
					float num4 = 1f - Mathf.InverseLerp(thermalSourceVolume.innerRadius * x2, thermalSourceVolume.outerRadius * x2, num3 - thermalReceiver.radius * x);
					num2 += thermalSourceVolume.celsius * num4;
				}
			}
			thermalReceiver.celsius = Mathf.Lerp(thermalReceiver.celsius, num2, num * thermalReceiver.conductivity);
			ContinuousPropertyArray continuousProperties = thermalReceiver.continuousProperties;
			if (continuousProperties != null)
			{
				continuousProperties.ApplyAll(thermalReceiver.celsius);
			}
			if (!thermalReceiver.wasAboveThreshold && thermalReceiver.celsius > thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = true;
				UnityEvent onAboveThreshold = thermalReceiver.OnAboveThreshold;
				if (onAboveThreshold != null)
				{
					onAboveThreshold.Invoke();
				}
			}
			else if (thermalReceiver.wasAboveThreshold && thermalReceiver.celsius < thermalReceiver.temperatureThreshold)
			{
				thermalReceiver.wasAboveThreshold = false;
				UnityEvent onBelowThreshold = thermalReceiver.OnBelowThreshold;
				if (onBelowThreshold != null)
				{
					onBelowThreshold.Invoke();
				}
			}
		}
	}

	// Token: 0x06001679 RID: 5753 RVA: 0x000824A9 File Offset: 0x000806A9
	public static void Register(ThermalSourceVolume source)
	{
		ThermalManager.sources.Add(source);
	}

	// Token: 0x0600167A RID: 5754 RVA: 0x000824B6 File Offset: 0x000806B6
	public static void Unregister(ThermalSourceVolume source)
	{
		ThermalManager.sources.Remove(source);
	}

	// Token: 0x0600167B RID: 5755 RVA: 0x000824C4 File Offset: 0x000806C4
	public static void Register(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Add(receiver);
	}

	// Token: 0x0600167C RID: 5756 RVA: 0x000824D1 File Offset: 0x000806D1
	public static void Unregister(ThermalReceiver receiver)
	{
		ThermalManager.receivers.Remove(receiver);
	}

	// Token: 0x04002089 RID: 8329
	public static readonly List<ThermalSourceVolume> sources = new List<ThermalSourceVolume>(256);

	// Token: 0x0400208A RID: 8330
	public static readonly List<ThermalReceiver> receivers = new List<ThermalReceiver>(256);

	// Token: 0x0400208B RID: 8331
	[NonSerialized]
	public static ThermalManager instance;

	// Token: 0x0400208C RID: 8332
	private float lastTime;
}
