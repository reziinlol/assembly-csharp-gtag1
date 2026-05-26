using System;
using UnityEngine;

// Token: 0x0200093D RID: 2365
public class PitchShiftAudioPlayer : MonoBehaviour
{
	// Token: 0x06003E04 RID: 15876 RVA: 0x0014F0E6 File Offset: 0x0014D2E6
	private void Awake()
	{
		if (this._source == null)
		{
			this._source = base.GetComponent<AudioSource>();
		}
		if (this._pitch == null)
		{
			this._pitch = base.GetComponent<RangedFloat>();
		}
	}

	// Token: 0x06003E05 RID: 15877 RVA: 0x0014F11C File Offset: 0x0014D31C
	private void OnEnable()
	{
		this._pitchMixVars.Rent(out this._pitchMix);
		this._source.outputAudioMixerGroup = this._pitchMix.group;
	}

	// Token: 0x06003E06 RID: 15878 RVA: 0x0014F146 File Offset: 0x0014D346
	private void OnDisable()
	{
		this._source.Stop();
		this._source.outputAudioMixerGroup = null;
		AudioMixVar pitchMix = this._pitchMix;
		if (pitchMix == null)
		{
			return;
		}
		pitchMix.ReturnToPool();
	}

	// Token: 0x06003E07 RID: 15879 RVA: 0x0014F16F File Offset: 0x0014D36F
	private void Update()
	{
		if (this.apply)
		{
			this.ApplyPitch();
		}
	}

	// Token: 0x06003E08 RID: 15880 RVA: 0x0014F17F File Offset: 0x0014D37F
	private void ApplyPitch()
	{
		this._pitchMix.value = this._pitch.curved;
	}

	// Token: 0x04004E3B RID: 20027
	public bool apply = true;

	// Token: 0x04004E3C RID: 20028
	[SerializeField]
	private AudioSource _source;

	// Token: 0x04004E3D RID: 20029
	[SerializeField]
	private AudioMixVarPool _pitchMixVars;

	// Token: 0x04004E3E RID: 20030
	[SerializeReference]
	private AudioMixVar _pitchMix;

	// Token: 0x04004E3F RID: 20031
	[SerializeField]
	private RangedFloat _pitch;
}
