using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200039B RID: 923
public class SpeakerVoiceLoudnessAudioOut : UnityAudioOut
{
	// Token: 0x0600165A RID: 5722 RVA: 0x00081970 File Offset: 0x0007FB70
	public SpeakerVoiceLoudnessAudioOut(SpeakerVoiceToLoudness speaker, AudioSource audioSource, AudioOutDelayControl.PlayDelayConfig playDelayConfig, Photon.Voice.ILogger logger, string logPrefix, bool debugInfo) : base(audioSource, playDelayConfig, logger, logPrefix, debugInfo)
	{
		this.voiceToLoudness = speaker;
	}

	// Token: 0x0600165B RID: 5723 RVA: 0x00081988 File Offset: 0x0007FB88
	public override void OutWrite(float[] data, int offsetSamples)
	{
		float num = 0f;
		for (int i = 0; i < data.Length; i++)
		{
			float num2 = data[i];
			if (!float.IsFinite(num2))
			{
				num2 = 0f;
				data[i] = num2;
			}
			else if (num2 > 1f)
			{
				num2 = 1f;
				data[i] = num2;
			}
			else if (num2 < -1f)
			{
				num2 = -1f;
				data[i] = num2;
			}
			num += Mathf.Abs(num2);
		}
		if (num > 0f)
		{
			float num3 = num / (float)data.Length;
			this.voiceToLoudness.loudness = num3;
			if (SpeakerVoiceToLoudnessConfig.EnableLoudnessLimit && num3 > SpeakerVoiceToLoudnessConfig.LoudnessLimitThreshold)
			{
				data = SpeakerVoiceToLoudnessConfig.StaticArrays.GetStaticArray(data.Length);
			}
		}
		base.OutWrite(data, offsetSamples);
	}

	// Token: 0x04002076 RID: 8310
	private SpeakerVoiceToLoudness voiceToLoudness;
}
