using System;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Audio;
using Oculus.VoiceSDK.Utilities;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

// Token: 0x0200088F RID: 2191
public class GorillaSpeakerLoudness : MonoBehaviour, IGorillaSliceableSimple, IDynamicFloat
{
	// Token: 0x17000512 RID: 1298
	// (get) Token: 0x06003934 RID: 14644 RVA: 0x001388E1 File Offset: 0x00136AE1
	public bool IsSpeaking
	{
		get
		{
			return this.isSpeaking;
		}
	}

	// Token: 0x17000513 RID: 1299
	// (get) Token: 0x06003935 RID: 14645 RVA: 0x001388E9 File Offset: 0x00136AE9
	public float Loudness
	{
		get
		{
			return this.loudness;
		}
	}

	// Token: 0x17000514 RID: 1300
	// (get) Token: 0x06003936 RID: 14646 RVA: 0x001388F1 File Offset: 0x00136AF1
	public float LoudnessNormalized
	{
		get
		{
			return Mathf.Min(this.loudness / this.normalizedMax, 1f);
		}
	}

	// Token: 0x17000515 RID: 1301
	// (get) Token: 0x06003937 RID: 14647 RVA: 0x0013890A File Offset: 0x00136B0A
	public float floatValue
	{
		get
		{
			return this.LoudnessNormalized;
		}
	}

	// Token: 0x17000516 RID: 1302
	// (get) Token: 0x06003938 RID: 14648 RVA: 0x00138912 File Offset: 0x00136B12
	public bool IsMicEnabled
	{
		get
		{
			return this.isMicEnabled;
		}
	}

	// Token: 0x17000517 RID: 1303
	// (get) Token: 0x06003939 RID: 14649 RVA: 0x0013891A File Offset: 0x00136B1A
	public float SmoothedLoudness
	{
		get
		{
			return this.smoothedLoudness;
		}
	}

	// Token: 0x0600393A RID: 14650 RVA: 0x00138922 File Offset: 0x00136B22
	private void Start()
	{
		this.rigContainer = base.GetComponent<RigContainer>();
		this.timeLastUpdated = Time.time;
		this.deltaTime = Time.deltaTime;
	}

	// Token: 0x0600393B RID: 14651 RVA: 0x00011DD7 File Offset: 0x0000FFD7
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600393C RID: 14652 RVA: 0x00011DE0 File Offset: 0x0000FFE0
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.LateUpdate);
	}

	// Token: 0x0600393D RID: 14653 RVA: 0x00138946 File Offset: 0x00136B46
	public void SliceUpdate()
	{
		this.deltaTime = Time.time - this.timeLastUpdated;
		this.timeLastUpdated = Time.time;
		this.UpdateMicEnabled();
		this.UpdateLoudness();
		this.UpdateSmoothedLoudness();
	}

	// Token: 0x0600393E RID: 14654 RVA: 0x00138978 File Offset: 0x00136B78
	private void UpdateMicEnabled()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig)
		{
			this.isMicEnabled = this.CheckMicConnection();
			rig.IsMicEnabled = this.isMicEnabled;
			return;
		}
		this.isMicEnabled = rig.IsMicEnabled;
	}

	// Token: 0x0600393F RID: 14655 RVA: 0x001389D0 File Offset: 0x00136BD0
	private bool CheckMicConnection()
	{
		this.permission = (this.permission || MicPermissionsManager.HasMicPermission());
		if (this.permission && !this.micConnected && Microphone.devices != null)
		{
			this.micConnected = (Microphone.devices.Length != 0);
		}
		return this.permission && this.micConnected;
	}

	// Token: 0x06003940 RID: 14656 RVA: 0x00138A2C File Offset: 0x00136C2C
	private void UpdateLoudness()
	{
		if (this.rigContainer == null)
		{
			return;
		}
		PhotonVoiceView voice = this.rigContainer.Voice;
		if (voice != null && this.speaker == null)
		{
			this.speaker = voice.SpeakerInUse;
		}
		if (this.recorder == null)
		{
			this.recorder = ((voice != null) ? voice.RecorderInUse : null);
		}
		if (this.recorder != null && this.offlineMic != null)
		{
			Microphone.End(UnityMicrophone.devices[0]);
			Object.Destroy(this.offlineMic);
			this.offlineMic = null;
			this.recorder.RestartRecording(true);
		}
		VRRig rig = this.rigContainer.Rig;
		if (rig.isOfflineVRRig && this.recorder == null && this.isMicEnabled && !Microphone.IsRecording(UnityMicrophone.devices[0]))
		{
			this.offlineMic = Microphone.Start(UnityMicrophone.devices[0], true, 1, 16000);
		}
		if ((rig.remoteUseReplacementVoice || rig.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && rig.SpeakingLoudness > 0f && !this.rigContainer.ForceMute && !this.rigContainer.Muted)
		{
			this.isSpeaking = true;
			this.loudness = rig.SpeakingLoudness;
			return;
		}
		if (voice != null && voice.IsSpeaking)
		{
			this.isSpeaking = true;
			if (!(this.speaker != null))
			{
				this.loudness = 0f;
				return;
			}
			if (this.speakerVoiceToLoudness == null)
			{
				this.speakerVoiceToLoudness = this.speaker.GetComponent<SpeakerVoiceToLoudness>();
			}
			if (this.speakerVoiceToLoudness != null)
			{
				this.loudness = this.speakerVoiceToLoudness.loudness;
				return;
			}
		}
		else if (voice != null && this.recorder != null && NetworkSystem.Instance.IsObjectLocallyOwned(voice.gameObject) && this.recorder.IsCurrentlyTransmitting)
		{
			if (this.voiceToLoudness == null)
			{
				this.voiceToLoudness = this.recorder.GetComponent<VoiceToLoudness>();
				if (this.voiceToLoudness == null)
				{
					this.recorder.AddComponent<VoiceToLoudness>();
				}
			}
			this.isSpeaking = true;
			if (this.voiceToLoudness != null)
			{
				this.loudness = this.voiceToLoudness.Loudness;
				return;
			}
			this.loudness = 0f;
			return;
		}
		else if (this.offlineMic != null && this.recorder == null && this.isMicEnabled && Microphone.IsRecording(UnityMicrophone.devices[0]))
		{
			this.isSpeaking = true;
			int num = Mathf.Min(Mathf.CeilToInt(this.deltaTime * 16000f), 16000);
			if (num > this.voiceSampleBuffer.Length)
			{
				Array.Resize<float>(ref this.voiceSampleBuffer, num);
			}
			if (this.offlineMic.samples >= num && this.offlineMic.GetData(this.voiceSampleBuffer, this.offlineMic.samples - num))
			{
				float num2 = 0f;
				for (int i = 0; i < this.voiceSampleBuffer.Length; i++)
				{
					num2 += Mathf.Abs(this.voiceSampleBuffer[i]);
				}
				this.loudness = num2 / (float)this.voiceSampleBuffer.Length;
				return;
			}
		}
		else
		{
			this.isSpeaking = false;
			this.loudness = 0f;
		}
	}

	// Token: 0x06003941 RID: 14657 RVA: 0x00138DA8 File Offset: 0x00136FA8
	private void UpdateSmoothedLoudness()
	{
		if (!this.isSpeaking)
		{
			this.smoothedLoudness = 0f;
			return;
		}
		if (!Mathf.Approximately(this.loudness, this.lastLoudness))
		{
			this.timeSinceLoudnessChange = 0f;
			this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
			this.lastLoudness = this.loudness;
			return;
		}
		if (this.timeSinceLoudnessChange > this.loudnessUpdateCheckRate)
		{
			this.smoothedLoudness = 0.001f;
			return;
		}
		this.smoothedLoudness = Mathf.Lerp(this.smoothedLoudness, this.loudness, Mathf.Clamp01(this.loudnessBlendStrength * this.deltaTime));
		this.timeSinceLoudnessChange += this.deltaTime;
	}

	// Token: 0x0400493E RID: 18750
	private bool isSpeaking;

	// Token: 0x0400493F RID: 18751
	private float loudness;

	// Token: 0x04004940 RID: 18752
	[SerializeField]
	private float normalizedMax = 0.175f;

	// Token: 0x04004941 RID: 18753
	private bool isMicEnabled;

	// Token: 0x04004942 RID: 18754
	private RigContainer rigContainer;

	// Token: 0x04004943 RID: 18755
	private Speaker speaker;

	// Token: 0x04004944 RID: 18756
	private SpeakerVoiceToLoudness speakerVoiceToLoudness;

	// Token: 0x04004945 RID: 18757
	private Recorder recorder;

	// Token: 0x04004946 RID: 18758
	private VoiceToLoudness voiceToLoudness;

	// Token: 0x04004947 RID: 18759
	private float smoothedLoudness;

	// Token: 0x04004948 RID: 18760
	private float lastLoudness;

	// Token: 0x04004949 RID: 18761
	private float timeSinceLoudnessChange;

	// Token: 0x0400494A RID: 18762
	private float loudnessUpdateCheckRate = 0.2f;

	// Token: 0x0400494B RID: 18763
	private float loudnessBlendStrength = 2f;

	// Token: 0x0400494C RID: 18764
	private bool permission;

	// Token: 0x0400494D RID: 18765
	private bool micConnected;

	// Token: 0x0400494E RID: 18766
	private float timeLastUpdated;

	// Token: 0x0400494F RID: 18767
	private float deltaTime;

	// Token: 0x04004950 RID: 18768
	private AudioClip offlineMic;

	// Token: 0x04004951 RID: 18769
	private float[] voiceSampleBuffer = new float[128];
}
