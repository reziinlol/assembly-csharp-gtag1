using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200006B RID: 107
public class CrittersNoiseMaker : CrittersToolThrowable
{
	// Token: 0x06000268 RID: 616 RVA: 0x0000E6DA File Offset: 0x0000C8DA
	protected override void OnImpact(Vector3 hitPosition, Vector3 hitNormal)
	{
		if (CrittersManager.instance.LocalAuthority())
		{
			if (this.destroyOnImpact || this.playOnce)
			{
				this.PlaySingleNoise();
				return;
			}
			this.StartPlayingRepeatNoise();
		}
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000E707 File Offset: 0x0000C907
	protected override void OnImpactCritter(CrittersPawn impactedCritter)
	{
		this.OnImpact(impactedCritter.transform.position, impactedCritter.transform.up);
	}

	// Token: 0x0600026A RID: 618 RVA: 0x0000E725 File Offset: 0x0000C925
	protected override void OnPickedUp()
	{
		this.StopPlayRepeatNoise();
	}

	// Token: 0x0600026B RID: 619 RVA: 0x0000E730 File Offset: 0x0000C930
	private void PlaySingleNoise()
	{
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
		CrittersManager.instance.TriggerEvent(CrittersManager.CritterEvent.NoiseMakerTriggered, this.actorId, base.transform.position);
	}

	// Token: 0x0600026C RID: 620 RVA: 0x0000E7AD File Offset: 0x0000C9AD
	private void StartPlayingRepeatNoise()
	{
		this.StopPlayRepeatNoise();
		this.repeatPlayNoise = base.StartCoroutine(this.PlayRepeatNoise());
	}

	// Token: 0x0600026D RID: 621 RVA: 0x0000E7C7 File Offset: 0x0000C9C7
	private void StopPlayRepeatNoise()
	{
		if (this.repeatPlayNoise != null)
		{
			base.StopCoroutine(this.repeatPlayNoise);
			this.repeatPlayNoise = null;
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x0000E7E4 File Offset: 0x0000C9E4
	private IEnumerator PlayRepeatNoise()
	{
		int num = Mathf.FloorToInt(this.repeatNoiseDuration / this.repeatNoiseRate);
		int num2;
		for (int i = num; i > 0; i = num2 - 1)
		{
			this.PlaySingleNoise();
			yield return new WaitForSeconds(this.repeatNoiseRate);
			num2 = i;
		}
		if (this.destroyAfterPlayingRepeatNoise)
		{
			this.shouldDisable = true;
		}
		yield break;
	}

	// Token: 0x040002B8 RID: 696
	[Header("Noise Maker")]
	public int soundSubIndex = 3;

	// Token: 0x040002B9 RID: 697
	public bool playOnce = true;

	// Token: 0x040002BA RID: 698
	public float repeatNoiseDuration;

	// Token: 0x040002BB RID: 699
	public float repeatNoiseRate;

	// Token: 0x040002BC RID: 700
	public bool destroyAfterPlayingRepeatNoise = true;

	// Token: 0x040002BD RID: 701
	private Coroutine repeatPlayNoise;
}
