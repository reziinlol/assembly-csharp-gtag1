using System;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200093E RID: 2366
public class PlayerAudioManager : MonoBehaviour
{
	// Token: 0x06003E0A RID: 15882 RVA: 0x0014F1A6 File Offset: 0x0014D3A6
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float transitionTime = 0.1f)
	{
		snapshot.TransitionTo(transitionTime);
	}

	// Token: 0x06003E0B RID: 15883 RVA: 0x0014F1AF File Offset: 0x0014D3AF
	public void UnsetMixerSnapshot(float transitionTime = 0.1f)
	{
		this.defaultSnapshot.TransitionTo(transitionTime);
	}

	// Token: 0x04004E40 RID: 20032
	public AudioMixerSnapshot defaultSnapshot;

	// Token: 0x04004E41 RID: 20033
	public AudioMixerSnapshot underwaterSnapshot;
}
