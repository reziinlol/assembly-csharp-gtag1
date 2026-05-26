using System;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200067F RID: 1663
public class ElderGorilla : MonoBehaviour
{
	// Token: 0x0600297A RID: 10618 RVA: 0x000DFD44 File Offset: 0x000DDF44
	private void Update()
	{
		if (GTPlayer.Instance == null)
		{
			return;
		}
		if (GTPlayer.Instance.inOverlay || !GTPlayer.Instance.isUserPresent)
		{
			return;
		}
		this.tHMD = GTPlayer.Instance.headCollider.transform;
		this.tLeftHand = GTPlayer.Instance.GetControllerTransform(true);
		this.tRightHand = GTPlayer.Instance.GetControllerTransform(false);
		if (Time.time - this.timeLastValidArmDist > 1f)
		{
			this.CheckHandDistance(this.tLeftHand);
			this.CheckHandDistance(this.tRightHand);
		}
		this.CheckHeight();
		this.CheckMicVolume();
	}

	// Token: 0x0600297B RID: 10619 RVA: 0x000DFDE8 File Offset: 0x000DDFE8
	private void CheckHandDistance(Transform hand)
	{
		float num = Vector3.Distance(hand.localPosition, this.tHMD.localPosition);
		if (num >= 1f)
		{
			return;
		}
		if (num >= 0.75f)
		{
			this.countValidArmDists++;
			this.timeLastValidArmDist = Time.time;
		}
	}

	// Token: 0x0600297C RID: 10620 RVA: 0x000DFE38 File Offset: 0x000DE038
	private void CheckHeight()
	{
		float y = this.tHMD.localPosition.y;
		if (!this.trackingHeadHeight)
		{
			this.trackedHeadHeight = y - 0.05f;
			this.timerTrackedHeadHeight = 0f;
		}
		else if (this.trackedHeadHeight < y)
		{
			this.trackingHeadHeight = false;
		}
		if (this.trackingHeadHeight)
		{
			if (this.timerTrackedHeadHeight >= 1f)
			{
				this.savedHeadHeight = y;
				this.trackingHeadHeight = false;
				return;
			}
			this.timerTrackedHeadHeight += Time.deltaTime;
		}
	}

	// Token: 0x0600297D RID: 10621 RVA: 0x000DFEBE File Offset: 0x000DE0BE
	private void CheckMicVolume()
	{
		float currentPeakAmp = GorillaTagger.Instance.myRecorder.LevelMeter.CurrentPeakAmp;
	}

	// Token: 0x040035FA RID: 13818
	private const float MAX_HAND_DIST = 1f;

	// Token: 0x040035FB RID: 13819
	private const float COOLDOWN_HAND_DIST = 1f;

	// Token: 0x040035FC RID: 13820
	private const float VALID_HAND_DIST = 0.75f;

	// Token: 0x040035FD RID: 13821
	private const float TIME_VALID_HEAD_HEIGHT = 1f;

	// Token: 0x040035FE RID: 13822
	private Transform tHMD;

	// Token: 0x040035FF RID: 13823
	private Transform tLeftHand;

	// Token: 0x04003600 RID: 13824
	private Transform tRightHand;

	// Token: 0x04003601 RID: 13825
	private int countValidArmDists;

	// Token: 0x04003602 RID: 13826
	private float timeLastValidArmDist;

	// Token: 0x04003603 RID: 13827
	private bool trackingHeadHeight;

	// Token: 0x04003604 RID: 13828
	private float trackedHeadHeight;

	// Token: 0x04003605 RID: 13829
	private float timerTrackedHeadHeight;

	// Token: 0x04003606 RID: 13830
	private float savedHeadHeight = 1.5f;
}
