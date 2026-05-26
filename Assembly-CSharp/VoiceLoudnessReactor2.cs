using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class VoiceLoudnessReactor2 : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170003D1 RID: 977
	// (get) Token: 0x06002480 RID: 9344 RVA: 0x000C3F35 File Offset: 0x000C2135
	private float Loudness
	{
		get
		{
			return this.gsl.Loudness * this.sensitivity;
		}
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000C3F4C File Offset: 0x000C214C
	private void OnEnable()
	{
		if (this.continuousProperties.Count == 0)
		{
			return;
		}
		if (this.gsl == null)
		{
			this.gsl = base.GetComponentInParent<GorillaSpeakerLoudness>(true);
			if (this.gsl == null)
			{
				GorillaTagger componentInParent = base.GetComponentInParent<GorillaTagger>();
				if (componentInParent != null)
				{
					this.gsl = componentInParent.offlineVRRig.GetComponent<GorillaSpeakerLoudness>();
					if (this.gsl == null)
					{
						return;
					}
				}
			}
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x170003D2 RID: 978
	// (get) Token: 0x06002483 RID: 9347 RVA: 0x000C3FC6 File Offset: 0x000C21C6
	// (set) Token: 0x06002484 RID: 9348 RVA: 0x000C3FCE File Offset: 0x000C21CE
	public bool TickRunning { get; set; }

	// Token: 0x06002485 RID: 9349 RVA: 0x000C3FD7 File Offset: 0x000C21D7
	public void Tick()
	{
		this.continuousProperties.ApplyAll(this.Loudness);
	}

	// Token: 0x04002FE5 RID: 12261
	[Tooltip("Multiply the microphone input by this value. A good default is 15.")]
	public float sensitivity = 15f;

	// Token: 0x04002FE6 RID: 12262
	public ContinuousPropertyArray continuousProperties;

	// Token: 0x04002FE7 RID: 12263
	private GorillaSpeakerLoudness gsl;
}
