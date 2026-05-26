using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class CrittersVoiceNoise : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x0600030B RID: 779 RVA: 0x00011DC9 File Offset: 0x0000FFC9
	private void Start()
	{
		this.speaker = base.GetComponent<GorillaSpeakerLoudness>();
	}

	// Token: 0x0600030C RID: 780 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600030D RID: 781 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600030E RID: 782 RVA: 0x00011DEC File Offset: 0x0000FFEC
	public void SliceUpdate()
	{
		float num = 0f;
		if (this.speaker.IsSpeaking)
		{
			num = this.speaker.Loudness;
		}
		if (num > this.minTriggerThreshold && CrittersManager.instance.IsNotNull())
		{
			CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.rigSetupByRig[this.rig].rigActors[4].actorSet;
			if (crittersLoudNoise.IsNotNull() && !crittersLoudNoise.soundEnabled)
			{
				float volume = Mathf.Lerp(this.noiseVolumeMin, this.noisVolumeMax, Mathf.Clamp01((num - this.minTriggerThreshold) / this.maxTriggerThreshold));
				crittersLoudNoise.PlayVoiceSpeechLocal(PhotonNetwork.InRoom ? PhotonNetwork.Time : ((double)Time.time), 0.016666668f, volume);
			}
		}
	}

	// Token: 0x0400036B RID: 875
	[SerializeField]
	private GorillaSpeakerLoudness speaker;

	// Token: 0x0400036C RID: 876
	[SerializeField]
	private VRRig rig;

	// Token: 0x0400036D RID: 877
	[SerializeField]
	private float minTriggerThreshold = 0.01f;

	// Token: 0x0400036E RID: 878
	[SerializeField]
	private float maxTriggerThreshold = 0.3f;

	// Token: 0x0400036F RID: 879
	[SerializeField]
	private float noiseVolumeMin = 1f;

	// Token: 0x04000370 RID: 880
	[SerializeField]
	private float noisVolumeMax = 9f;
}
