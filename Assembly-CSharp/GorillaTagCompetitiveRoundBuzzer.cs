using System;
using UnityEngine;

// Token: 0x02000898 RID: 2200
public class GorillaTagCompetitiveRoundBuzzer : MonoBehaviour
{
	// Token: 0x060039A6 RID: 14758 RVA: 0x0013A253 File Offset: 0x00138453
	private void OnEnable()
	{
		GorillaTagCompetitiveManager.onStateChanged += this.OnStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime += this.OnUpdateRemainingTime;
	}

	// Token: 0x060039A7 RID: 14759 RVA: 0x0013A277 File Offset: 0x00138477
	private void OnDisable()
	{
		GorillaTagCompetitiveManager.onStateChanged -= this.OnStateChanged;
		GorillaTagCompetitiveManager.onUpdateRemainingTime -= this.OnUpdateRemainingTime;
	}

	// Token: 0x060039A8 RID: 14760 RVA: 0x0013A29C File Offset: 0x0013849C
	private void OnStateChanged(GorillaTagCompetitiveManager.GameState newState)
	{
		switch (newState)
		{
		case GorillaTagCompetitiveManager.GameState.WaitingForPlayers:
			this.PlaySFX(this.needMorePlayerClip);
			break;
		case GorillaTagCompetitiveManager.GameState.Playing:
			this.PlaySFX(this.roundStartClip);
			break;
		case GorillaTagCompetitiveManager.GameState.PostRound:
			this.PlaySFX(this.roundEndClip);
			break;
		}
		this.lastState = newState;
	}

	// Token: 0x060039A9 RID: 14761 RVA: 0x0013A2F4 File Offset: 0x001384F4
	private void OnUpdateRemainingTime(float remainingTime)
	{
		int num = Mathf.CeilToInt(remainingTime);
		int num2 = Mathf.CeilToInt(this.lastStateRemainingTime);
		if (num != num2)
		{
			GorillaTagCompetitiveManager.GameState gameState = this.lastState;
			if (gameState != GorillaTagCompetitiveManager.GameState.StartingCountdown)
			{
				if (gameState == GorillaTagCompetitiveManager.GameState.Playing)
				{
					if (num > 0 && num <= this.roundEndCountdownDuration)
					{
						this.PlaySFX(this.roundEndingCountdownClip);
					}
				}
			}
			else if (num > 0)
			{
				this.PlaySFX(this.roundCountdownClip);
			}
		}
		this.lastStateRemainingTime = remainingTime;
	}

	// Token: 0x060039AA RID: 14762 RVA: 0x0013A35B File Offset: 0x0013855B
	private void PlaySFX(AudioClip clip)
	{
		this.PlaySFX(clip, 1f);
	}

	// Token: 0x060039AB RID: 14763 RVA: 0x0013A369 File Offset: 0x00138569
	private void PlaySFX(AudioClip clip, float volume)
	{
		this.audioSource.PlayOneShot(clip, volume);
	}

	// Token: 0x04004985 RID: 18821
	public AudioSource audioSource;

	// Token: 0x04004986 RID: 18822
	public AudioClip roundCountdownClip;

	// Token: 0x04004987 RID: 18823
	public AudioClip roundStartClip;

	// Token: 0x04004988 RID: 18824
	public AudioClip roundEndingCountdownClip;

	// Token: 0x04004989 RID: 18825
	public int roundEndCountdownDuration = 5;

	// Token: 0x0400498A RID: 18826
	public AudioClip roundEndClip;

	// Token: 0x0400498B RID: 18827
	public AudioClip needMorePlayerClip;

	// Token: 0x0400498C RID: 18828
	private GorillaTagCompetitiveManager.GameState lastState;

	// Token: 0x0400498D RID: 18829
	private float lastStateRemainingTime = -1f;
}
