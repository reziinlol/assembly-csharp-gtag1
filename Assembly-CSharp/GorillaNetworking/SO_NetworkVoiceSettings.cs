using System;
using ExitGames.Client.Photon;
using Photon.Voice;
using Photon.Voice.Unity;
using POpusCodec.Enums;
using UnityEngine;

namespace GorillaNetworking
{
	// Token: 0x02001015 RID: 4117
	[CreateAssetMenu(fileName = "VoiceSettings", menuName = "Gorilla Tag/VoiceSettings")]
	public class SO_NetworkVoiceSettings : ScriptableObject
	{
		// Token: 0x0400760A RID: 30218
		[Header("Voice settings")]
		public bool AutoConnectAndJoin = true;

		// Token: 0x0400760B RID: 30219
		public bool AutoLeaveAndDisconnect = true;

		// Token: 0x0400760C RID: 30220
		public bool WorkInOfflineMode = true;

		// Token: 0x0400760D RID: 30221
		public DebugLevel LogLevel = DebugLevel.ERROR;

		// Token: 0x0400760E RID: 30222
		public DebugLevel GlobalRecordersLogLevel = DebugLevel.INFO;

		// Token: 0x0400760F RID: 30223
		public DebugLevel GlobalSpeakersLogLevel = DebugLevel.INFO;

		// Token: 0x04007610 RID: 30224
		public bool CreateSpeakerIfNotFound;

		// Token: 0x04007611 RID: 30225
		public int UpdateInterval = 50;

		// Token: 0x04007612 RID: 30226
		public bool SupportLogger;

		// Token: 0x04007613 RID: 30227
		public int BackgroundTimeout = 60000;

		// Token: 0x04007614 RID: 30228
		[Header("Recorder Settings")]
		public bool RecordOnlyWhenEnabled;

		// Token: 0x04007615 RID: 30229
		public bool RecordOnlyWhenJoined = true;

		// Token: 0x04007616 RID: 30230
		public bool StopRecordingWhenPaused;

		// Token: 0x04007617 RID: 30231
		public bool TransmitEnabled = true;

		// Token: 0x04007618 RID: 30232
		public bool AutoStart = true;

		// Token: 0x04007619 RID: 30233
		public bool Encrypt;

		// Token: 0x0400761A RID: 30234
		public byte InterestGroup;

		// Token: 0x0400761B RID: 30235
		public bool DebugEcho;

		// Token: 0x0400761C RID: 30236
		public bool ReliableMode;

		// Token: 0x0400761D RID: 30237
		[Header("Recorder Codec Parameters")]
		public OpusCodec.FrameDuration FrameDuration = OpusCodec.FrameDuration.Frame60ms;

		// Token: 0x0400761E RID: 30238
		public SamplingRate SamplingRate = SamplingRate.Sampling16000;

		// Token: 0x0400761F RID: 30239
		[Range(6000f, 510000f)]
		public int Bitrate = 20000;

		// Token: 0x04007620 RID: 30240
		[Space]
		public SamplingRate SubsSamplingRate = SamplingRate.Sampling24000;

		// Token: 0x04007621 RID: 30241
		[Range(6000f, 510000f)]
		public int SubsBitrate = 30000;

		// Token: 0x04007622 RID: 30242
		[Header("Recorder Audio Source Settings")]
		public Recorder.InputSourceType InputSourceType;

		// Token: 0x04007623 RID: 30243
		public Recorder.MicType MicrophoneType;

		// Token: 0x04007624 RID: 30244
		public bool UseFallback = true;

		// Token: 0x04007625 RID: 30245
		public bool Detect = true;

		// Token: 0x04007626 RID: 30246
		[Range(0f, 1f)]
		public float Threshold = 0.07f;

		// Token: 0x04007627 RID: 30247
		public int Delay = 500;
	}
}
