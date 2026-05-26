using System;
using Photon.Voice;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011F0 RID: 4592
	internal class ProcessVoiceDataToLoudness : IProcessor<float>, IDisposable
	{
		// Token: 0x06007337 RID: 29495 RVA: 0x002570AE File Offset: 0x002552AE
		public ProcessVoiceDataToLoudness(VoiceToLoudness voiceToLoudness)
		{
			this._voiceToLoudness = voiceToLoudness;
		}

		// Token: 0x06007338 RID: 29496 RVA: 0x002570C0 File Offset: 0x002552C0
		public float[] Process(float[] buf)
		{
			float num = 0f;
			for (int i = 0; i < buf.Length; i++)
			{
				num += Mathf.Abs(buf[i]);
			}
			this._voiceToLoudness.Loudness = num / (float)buf.Length;
			return buf;
		}

		// Token: 0x06007339 RID: 29497 RVA: 0x000028C5 File Offset: 0x00000AC5
		public void Dispose()
		{
		}

		// Token: 0x0400839E RID: 33694
		private VoiceToLoudness _voiceToLoudness;
	}
}
