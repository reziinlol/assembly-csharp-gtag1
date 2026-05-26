using System;
using UnityEngine;

namespace GorillaTag.Audio
{
	// Token: 0x020011EB RID: 4587
	internal static class GTAudioOneShot
	{
		// Token: 0x17000B0D RID: 2829
		// (get) Token: 0x06007321 RID: 29473 RVA: 0x00256C48 File Offset: 0x00254E48
		// (set) Token: 0x06007322 RID: 29474 RVA: 0x00256C4F File Offset: 0x00254E4F
		internal static bool isInitialized { get; private set; }

		// Token: 0x06007323 RID: 29475 RVA: 0x00256C58 File Offset: 0x00254E58
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			if (GTAudioOneShot.isInitialized)
			{
				return;
			}
			AudioSource audioSource = Resources.Load<AudioSource>("AudioSourceSingleton_Prefab");
			if (audioSource == null)
			{
				Debug.LogError("GTAudioOneShot: Failed to load AudioSourceSingleton_Prefab from resources!!!");
				return;
			}
			GTAudioOneShot.audioSource = Object.Instantiate<AudioSource>(audioSource);
			GTAudioOneShot.defaultCurve = GTAudioOneShot.audioSource.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
			Object.DontDestroyOnLoad(GTAudioOneShot.audioSource);
			GTAudioOneShot.isInitialized = true;
		}

		// Token: 0x06007324 RID: 29476 RVA: 0x00256CB7 File Offset: 0x00254EB7
		internal static void Play(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.pitch = pitch;
			GTAudioOneShot.audioSource.transform.position = position;
			GTAudioOneShot.audioSource.GTPlayOneShot(clip, volume);
		}

		// Token: 0x06007325 RID: 29477 RVA: 0x00256CEF File Offset: 0x00254EEF
		internal static void Play(AudioClip clip, Vector3 position, AnimationCurve curve, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return;
			}
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
			GTAudioOneShot.Play(clip, position, volume, pitch);
			GTAudioOneShot.audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, GTAudioOneShot.defaultCurve);
		}

		// Token: 0x06007326 RID: 29478 RVA: 0x00256D26 File Offset: 0x00254F26
		internal static int PlayDelayed(AudioClip sound, Vector3 pos, float delay, float volume = 1f, float pitch = 1f)
		{
			return GTAudioOneShot.PlayDelayed(sound, null, pos, delay, volume, pitch);
		}

		// Token: 0x06007327 RID: 29479 RVA: 0x00256D34 File Offset: 0x00254F34
		internal static int PlayDelayed(AudioClip sound, Transform xform, Vector3 pos, float delay, float volume = 1f, float pitch = 1f)
		{
			if (ApplicationQuittingState.IsQuitting || !GTAudioOneShot.isInitialized)
			{
				return -1;
			}
			int num;
			if (GTAudioOneShot._delayedFreeHead >= 0)
			{
				num = GTAudioOneShot._delayedFreeHead;
				GTAudioOneShot._delayedFreeHead = GTAudioOneShot._delayedFreeNext[num];
			}
			else
			{
				if (GTAudioOneShot._delayedHighWater >= GTAudioOneShot._delayedData.Length)
				{
					int newSize = GTAudioOneShot._delayedData.Length * 2;
					Array.Resize<GTAudioOneShot.DelayedPlayData>(ref GTAudioOneShot._delayedData, newSize);
					Array.Resize<int>(ref GTAudioOneShot._delayedFreeNext, newSize);
				}
				num = GTAudioOneShot._delayedHighWater++;
			}
			GTAudioOneShot._delayedData[num] = new GTAudioOneShot.DelayedPlayData
			{
				sound = sound,
				xform = xform,
				pos = pos,
				volume = volume,
				pitch = pitch
			};
			GTDelayedExec.Add(GTAudioOneShot._delayedListener, delay, num);
			return num;
		}

		// Token: 0x06007328 RID: 29480 RVA: 0x00256DF4 File Offset: 0x00254FF4
		internal static void CancelDelayed(int idx)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			GTAudioOneShot._delayedData[idx].sound = null;
		}

		// Token: 0x06007329 RID: 29481 RVA: 0x00256E10 File Offset: 0x00255010
		internal static void UpdateDelayed(int idx, Transform xform)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.xform = xform;
		}

		// Token: 0x0600732A RID: 29482 RVA: 0x00256E48 File Offset: 0x00255048
		internal static void UpdateDelayed(int idx, Vector3 pos)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.pos = pos;
		}

		// Token: 0x0600732B RID: 29483 RVA: 0x00256E80 File Offset: 0x00255080
		internal static void UpdateDelayed(int idx, Transform xform, Vector3 pos)
		{
			if (idx >= GTAudioOneShot._delayedHighWater)
			{
				return;
			}
			ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[idx];
			if (ptr.sound == null)
			{
				return;
			}
			ptr.xform = xform;
			ptr.pos = pos;
		}

		// Token: 0x04008389 RID: 33673
		[OnEnterPlay_SetNull]
		internal static AudioSource audioSource;

		// Token: 0x0400838A RID: 33674
		[OnEnterPlay_SetNull]
		internal static AnimationCurve defaultCurve;

		// Token: 0x0400838B RID: 33675
		private const int k_initialDelayedCount = 32;

		// Token: 0x0400838C RID: 33676
		[OnEnterPlay_Set(0)]
		private static int _delayedHighWater;

		// Token: 0x0400838D RID: 33677
		[OnEnterPlay_Set(-1)]
		private static int _delayedFreeHead = -1;

		// Token: 0x0400838E RID: 33678
		[OnEnterPlay_SetNew]
		private static GTAudioOneShot.DelayedPlayData[] _delayedData = new GTAudioOneShot.DelayedPlayData[32];

		// Token: 0x0400838F RID: 33679
		[OnEnterPlay_SetNew]
		private static int[] _delayedFreeNext = new int[32];

		// Token: 0x04008390 RID: 33680
		[OnEnterPlay_SetNew]
		private static readonly GTAudioOneShot.DelayedPlayListener _delayedListener = new GTAudioOneShot.DelayedPlayListener();

		// Token: 0x020011EC RID: 4588
		private struct DelayedPlayData
		{
			// Token: 0x04008391 RID: 33681
			public AudioClip sound;

			// Token: 0x04008392 RID: 33682
			public Transform xform;

			// Token: 0x04008393 RID: 33683
			public Vector3 pos;

			// Token: 0x04008394 RID: 33684
			public float volume;

			// Token: 0x04008395 RID: 33685
			public float pitch;
		}

		// Token: 0x020011ED RID: 4589
		private class DelayedPlayListener : IDelayedExecListener
		{
			// Token: 0x0600732D RID: 29485 RVA: 0x00256EEC File Offset: 0x002550EC
			public void OnDelayedAction(int contextId)
			{
				if (contextId >= GTAudioOneShot._delayedHighWater)
				{
					return;
				}
				ref GTAudioOneShot.DelayedPlayData ptr = ref GTAudioOneShot._delayedData[contextId];
				if (ptr.sound != null)
				{
					Vector3 position = (ptr.xform != null) ? ptr.xform.TransformPoint(ptr.pos) : ptr.pos;
					GTAudioOneShot.Play(ptr.sound, position, ptr.volume, ptr.pitch);
				}
				ptr = default(GTAudioOneShot.DelayedPlayData);
				GTAudioOneShot._delayedFreeNext[contextId] = GTAudioOneShot._delayedFreeHead;
				GTAudioOneShot._delayedFreeHead = contextId;
			}
		}
	}
}
