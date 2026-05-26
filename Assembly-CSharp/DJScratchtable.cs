using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x020002BE RID: 702
public class DJScratchtable : MonoBehaviour
{
	// Token: 0x0600121E RID: 4638 RVA: 0x00060FC7 File Offset: 0x0005F1C7
	public void SetPlaying(bool playing)
	{
		this.isPlaying = playing;
	}

	// Token: 0x0600121F RID: 4639 RVA: 0x00060FD0 File Offset: 0x0005F1D0
	private void OnTriggerStay(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		GorillaTriggerColliderHandIndicator componentInParent = collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
		if (componentInParent == null)
		{
			return;
		}
		Vector3 vector = (base.transform.parent.InverseTransformPoint(collider.transform.position) - base.transform.localPosition).WithY(0f);
		float target = Mathf.Atan2(vector.z, vector.x) * 57.29578f;
		if (this.isTouching)
		{
			base.transform.localRotation = Quaternion.LookRotation(vector) * this.firstTouchRotation;
			if (this.isPlaying)
			{
				float num = Mathf.DeltaAngle(this.lastScratchSoundAngle, target);
				if (num > this.scratchMinAngle)
				{
					if (Time.time > this.cantForwardScratchUntilTimestamp)
					{
						this.scratchPlayer.Play(ScratchSoundType.Forward, this.isLeft);
						this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
						this.lastScratchSoundAngle = target;
						GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
					}
				}
				else if (num < -this.scratchMinAngle && Time.time > this.cantBackScratchUntilTimestamp)
				{
					this.scratchPlayer.Play(ScratchSoundType.Back, this.isLeft);
					this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
					this.lastScratchSoundAngle = target;
					GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, this.hapticStrength, this.hapticDuration);
				}
			}
		}
		else
		{
			this.firstTouchRotation = Quaternion.Inverse(Quaternion.LookRotation(base.transform.InverseTransformPoint(collider.transform.position).WithY(0f)));
			if (this.isPlaying)
			{
				this.PauseTrack();
				this.scratchPlayer.Play(ScratchSoundType.Pause, this.isLeft);
				this.lastScratchSoundAngle = target;
				this.cantForwardScratchUntilTimestamp = Time.time + this.scratchCooldown;
				this.cantBackScratchUntilTimestamp = Time.time + this.scratchCooldown;
			}
		}
		this.isTouching = true;
	}

	// Token: 0x06001220 RID: 4640 RVA: 0x000611D4 File Offset: 0x0005F3D4
	private void OnTriggerExit(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		if (this.isPlaying)
		{
			this.ResumeTrack();
			this.scratchPlayer.Play(ScratchSoundType.Resume, this.isLeft);
		}
		this.isTouching = false;
	}

	// Token: 0x06001221 RID: 4641 RVA: 0x00061220 File Offset: 0x0005F420
	public void SelectTrack(int track)
	{
		this.lastSelectedTrack = track;
		if (track == 0)
		{
			this.turntableVisual.Stop();
			this.isPlaying = false;
		}
		else
		{
			this.turntableVisual.Run();
			this.isPlaying = true;
		}
		int num = track - 1;
		for (int i = 0; i < this.tracks.Length; i++)
		{
			if (num == i)
			{
				float time = (float)(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time)) % this.trackDuration;
				this.tracks[i].Play();
				this.tracks[i].time = time;
			}
			else
			{
				this.tracks[i].Stop();
			}
		}
	}

	// Token: 0x06001222 RID: 4642 RVA: 0x000612C0 File Offset: 0x0005F4C0
	public void PauseTrack()
	{
		for (int i = 0; i < this.tracks.Length; i++)
		{
			this.tracks[i].Stop();
		}
		this.pausedUntilTimestamp = Time.time + 1f;
	}

	// Token: 0x06001223 RID: 4643 RVA: 0x000612FE File Offset: 0x0005F4FE
	public void ResumeTrack()
	{
		this.SelectTrack(this.lastSelectedTrack);
		this.pausedUntilTimestamp = 0f;
	}

	// Token: 0x040015EA RID: 5610
	[SerializeField]
	private bool isLeft;

	// Token: 0x040015EB RID: 5611
	[SerializeField]
	private DJScratchSoundPlayer scratchPlayer;

	// Token: 0x040015EC RID: 5612
	[SerializeField]
	private float scratchCooldown;

	// Token: 0x040015ED RID: 5613
	[SerializeField]
	private float scratchMinAngle;

	// Token: 0x040015EE RID: 5614
	[SerializeField]
	private AudioSource[] tracks;

	// Token: 0x040015EF RID: 5615
	[SerializeField]
	private CosmeticFan turntableVisual;

	// Token: 0x040015F0 RID: 5616
	[SerializeField]
	private float trackDuration;

	// Token: 0x040015F1 RID: 5617
	[SerializeField]
	private float hapticStrength;

	// Token: 0x040015F2 RID: 5618
	[SerializeField]
	private float hapticDuration;

	// Token: 0x040015F3 RID: 5619
	private int lastSelectedTrack;

	// Token: 0x040015F4 RID: 5620
	private bool isPlaying;

	// Token: 0x040015F5 RID: 5621
	private bool isTouching;

	// Token: 0x040015F6 RID: 5622
	private Quaternion firstTouchRotation;

	// Token: 0x040015F7 RID: 5623
	private float lastScratchSoundAngle;

	// Token: 0x040015F8 RID: 5624
	private float cantForwardScratchUntilTimestamp;

	// Token: 0x040015F9 RID: 5625
	private float cantBackScratchUntilTimestamp;

	// Token: 0x040015FA RID: 5626
	private float pausedUntilTimestamp;
}
