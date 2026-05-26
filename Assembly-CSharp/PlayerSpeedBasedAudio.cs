using System;
using UnityEngine;

// Token: 0x020002AA RID: 682
public class PlayerSpeedBasedAudio : MonoBehaviour
{
	// Token: 0x060011BA RID: 4538 RVA: 0x0005EF47 File Offset: 0x0005D147
	private void Start()
	{
		this.fadeRate = 1f / this.fadeTime;
		this.baseVolume = this.audioSource.volume;
		this.localPlayerVelocityEstimator.TryResolve<GorillaVelocityEstimator>(out this.velocityEstimator);
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0005EF80 File Offset: 0x0005D180
	private void Update()
	{
		this.currentFadeLevel = Mathf.MoveTowards(this.currentFadeLevel, Mathf.InverseLerp(this.minVolumeSpeed, this.fullVolumeSpeed, this.velocityEstimator.linearVelocity.magnitude), this.fadeRate * Time.deltaTime);
		if (this.baseVolume == 0f || this.currentFadeLevel == 0f)
		{
			this.audioSource.volume = 0.0001f;
			return;
		}
		this.audioSource.volume = this.baseVolume * this.currentFadeLevel;
	}

	// Token: 0x04001544 RID: 5444
	[SerializeField]
	private float minVolumeSpeed;

	// Token: 0x04001545 RID: 5445
	[SerializeField]
	private float fullVolumeSpeed;

	// Token: 0x04001546 RID: 5446
	[SerializeField]
	private float fadeTime;

	// Token: 0x04001547 RID: 5447
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04001548 RID: 5448
	[SerializeField]
	private XSceneRef localPlayerVelocityEstimator;

	// Token: 0x04001549 RID: 5449
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x0400154A RID: 5450
	private float baseVolume;

	// Token: 0x0400154B RID: 5451
	private float fadeRate;

	// Token: 0x0400154C RID: 5452
	private float currentFadeLevel;
}
