using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x02000364 RID: 868
public class RaceCheckpoint : MonoBehaviour
{
	// Token: 0x06001534 RID: 5428 RVA: 0x000707F3 File Offset: 0x0006E9F3
	public void Init(RaceCheckpointManager manager, int index)
	{
		this.manager = manager;
		this.checkpointIndex = index;
		this.SetIsCorrectCheckpoint(index == 0);
	}

	// Token: 0x06001535 RID: 5429 RVA: 0x0007080D File Offset: 0x0006EA0D
	public void SetIsCorrectCheckpoint(bool isCorrect)
	{
		this.isCorrect = isCorrect;
		this.banner.sharedMaterial = (isCorrect ? this.activeCheckpointMat : this.wrongCheckpointMat);
	}

	// Token: 0x06001536 RID: 5430 RVA: 0x00070832 File Offset: 0x0006EA32
	private void OnTriggerEnter(Collider other)
	{
		if (other != GTPlayer.Instance.headCollider)
		{
			return;
		}
		if (this.isCorrect)
		{
			this.manager.OnCheckpointReached(this.checkpointIndex, this.checkpointSound);
			return;
		}
		this.wrongCheckpointSound.Play();
	}

	// Token: 0x04001A06 RID: 6662
	[SerializeField]
	private MeshRenderer banner;

	// Token: 0x04001A07 RID: 6663
	[SerializeField]
	private Material activeCheckpointMat;

	// Token: 0x04001A08 RID: 6664
	[SerializeField]
	private Material wrongCheckpointMat;

	// Token: 0x04001A09 RID: 6665
	[SerializeField]
	private SoundBankPlayer checkpointSound;

	// Token: 0x04001A0A RID: 6666
	[SerializeField]
	private SoundBankPlayer wrongCheckpointSound;

	// Token: 0x04001A0B RID: 6667
	private RaceCheckpointManager manager;

	// Token: 0x04001A0C RID: 6668
	private int checkpointIndex;

	// Token: 0x04001A0D RID: 6669
	private bool isCorrect;
}
