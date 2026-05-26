using System;
using Photon.Voice;
using Photon.Voice.Unity;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011EF RID: 4591
	[RequireComponent(typeof(Recorder))]
	public class VoiceToLoudness : MonoBehaviour
	{
		// Token: 0x06007332 RID: 29490 RVA: 0x00257025 File Offset: 0x00255225
		protected void Awake()
		{
			this._recorder = base.GetComponent<Recorder>();
		}

		// Token: 0x06007333 RID: 29491 RVA: 0x00257033 File Offset: 0x00255233
		protected void PhotonVoiceCreated(PhotonVoiceCreatedParams photonVoiceCreatedParams)
		{
			this.CreateProcessVoiceData(photonVoiceCreatedParams.Voice);
		}

		// Token: 0x06007334 RID: 29492 RVA: 0x00257044 File Offset: 0x00255244
		private void CreateProcessVoiceData(LocalVoice voice)
		{
			LocalVoiceAudioFloat localVoiceAudioFloat = voice as LocalVoiceAudioFloat;
			if (localVoiceAudioFloat != null)
			{
				this._photonVoiceCreated = true;
				localVoiceAudioFloat.AddPostProcessor(new IProcessor<float>[]
				{
					new ProcessVoiceDataToLoudness(this)
				});
			}
		}

		// Token: 0x06007335 RID: 29493 RVA: 0x00257077 File Offset: 0x00255277
		private void Update()
		{
			if (this._photonVoiceCreated)
			{
				return;
			}
			if (this._recorder != null && this._recorder.Voice != null)
			{
				this.CreateProcessVoiceData(this._recorder.Voice);
			}
		}

		// Token: 0x0400839A RID: 33690
		[NonSerialized]
		public float Loudness;

		// Token: 0x0400839B RID: 33691
		private Recorder _recorder;

		// Token: 0x0400839C RID: 33692
		private bool _photonVoiceCreated;

		// Token: 0x0400839D RID: 33693
		private float _checkVoice;
	}
}
