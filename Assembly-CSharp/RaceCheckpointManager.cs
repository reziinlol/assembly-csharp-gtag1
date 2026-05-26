using System;
using UnityEngine;

// Token: 0x02000365 RID: 869
public class RaceCheckpointManager : MonoBehaviour
{
	// Token: 0x06001538 RID: 5432 RVA: 0x00070874 File Offset: 0x0006EA74
	private void Start()
	{
		this.visual = base.GetComponent<RaceVisual>();
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].Init(this, i);
		}
		this.OnRaceEnd();
	}

	// Token: 0x06001539 RID: 5433 RVA: 0x000708B8 File Offset: 0x0006EAB8
	public void OnRaceStart()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(i == 0);
		}
	}

	// Token: 0x0600153A RID: 5434 RVA: 0x000708EC File Offset: 0x0006EAEC
	public void OnRaceEnd()
	{
		for (int i = 0; i < this.checkpoints.Length; i++)
		{
			this.checkpoints[i].SetIsCorrectCheckpoint(false);
		}
	}

	// Token: 0x0600153B RID: 5435 RVA: 0x0007091A File Offset: 0x0006EB1A
	public void OnCheckpointReached(int index, SoundBankPlayer checkpointSound)
	{
		this.checkpoints[index].SetIsCorrectCheckpoint(false);
		this.checkpoints[(index + 1) % this.checkpoints.Length].SetIsCorrectCheckpoint(true);
		this.visual.OnCheckpointPassed(index, checkpointSound);
	}

	// Token: 0x0600153C RID: 5436 RVA: 0x00070950 File Offset: 0x0006EB50
	public bool IsPlayerNearCheckpoint(VRRig player, int checkpointIdx)
	{
		return checkpointIdx >= 0 && checkpointIdx < this.checkpoints.Length && player.IsPositionInRange(this.checkpoints[checkpointIdx].transform.position, 6f);
	}

	// Token: 0x04001A0E RID: 6670
	[SerializeField]
	private RaceCheckpoint[] checkpoints;

	// Token: 0x04001A0F RID: 6671
	private RaceVisual visual;
}
