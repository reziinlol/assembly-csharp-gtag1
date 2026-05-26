using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200028E RID: 654
public class GameStateFx : MonoBehaviour, IGameStateReceiver, IDelayedExecListener
{
	// Token: 0x0600117A RID: 4474 RVA: 0x0005DBD4 File Offset: 0x0005BDD4
	protected void Awake()
	{
		IGameStateProvider gameStateProvider = this.m_stateProvider as IGameStateProvider;
		if (gameStateProvider == null)
		{
			GTDev.LogError<string>("[GT/GameStateFx]  ERROR!!!  Awake: The supplied State Provider is not type `IGameStateProvider`. Path=" + base.transform.GetPathQ(), null);
			this._isValid = false;
			base.enabled = false;
			return;
		}
		this._stateProvider = gameStateProvider;
		if (!this._IsAllValid())
		{
			return;
		}
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			if (array != null)
			{
				Array.Sort<GameStateFx.StateReaction>(array, new Comparison<GameStateFx.StateReaction>(GameStateFx._DelaySortCompare));
			}
		}
	}

	// Token: 0x0600117B RID: 4475 RVA: 0x0005DC80 File Offset: 0x0005BE80
	private static int _DelaySortCompare(GameStateFx.StateReaction a, GameStateFx.StateReaction b)
	{
		return a.delay.CompareTo(b.delay);
	}

	// Token: 0x0600117C RID: 4476 RVA: 0x0005DC93 File Offset: 0x0005BE93
	protected void OnEnable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			base.enabled = false;
			return;
		}
		this._stateProvider.GameStateReceiverRegister(this);
	}

	// Token: 0x0600117D RID: 4477 RVA: 0x0005DCB8 File Offset: 0x0005BEB8
	protected void OnDisable()
	{
		if (!this._isValid || ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		this._stateProvider.GameStateReceiverUnregister(this);
	}

	// Token: 0x0600117E RID: 4478 RVA: 0x0005DCD8 File Offset: 0x0005BED8
	void IGameStateReceiver.GameStateReceiverOnStateChanged(long oldState, long newState)
	{
		GameStateFx.StateReaction[] array;
		if (!this.m_stateMap.TryGet(newState, out array))
		{
			return;
		}
		this._delayedExecContextFrameNum = Time.frameCount;
		this._reactionQueue.Clear();
		foreach (GameStateFx.StateReaction stateReaction in array)
		{
			if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Delay) != (GameStateFx.StateReaction.EOptions)0)
			{
				this._reactionQueue.Enqueue(stateReaction);
				GTDelayedExec.Add(this, stateReaction.delay, Time.frameCount);
			}
			else
			{
				GameStateFx._PerformReactions(stateReaction);
			}
		}
	}

	// Token: 0x0600117F RID: 4479 RVA: 0x0005DD4F File Offset: 0x0005BF4F
	void IDelayedExecListener.OnDelayedAction(int contextFrameNum)
	{
		if (contextFrameNum != this._delayedExecContextFrameNum || !base.isActiveAndEnabled)
		{
			return;
		}
		GameStateFx._PerformReactions(this._reactionQueue.Dequeue());
	}

	// Token: 0x06001180 RID: 4480 RVA: 0x0005DD74 File Offset: 0x0005BF74
	private static void _PerformReactions(GameStateFx.StateReaction reaction)
	{
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
		{
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Sound) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.resource = reaction.soundInfo.sound;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Volume) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.volume = reaction.soundInfo.volume;
			}
			if ((reaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Pitch) != (GameStateFx.SoundEntry.EOptions)0)
			{
				reaction.soundInfo.source.pitch = reaction.soundInfo.pitch;
			}
			reaction.soundInfo.source.GTPlay();
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.GameObjectInfo gameObjectInfo in reaction.gameObjectInfos)
			{
				gameObjectInfo.gameObject.SetActive(gameObjectInfo.activate);
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.BehaviourInfo behaviourInfo in reaction.behaviourInfos)
			{
				behaviourInfo.behaviour.enabled = behaviourInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
		{
			foreach (GameStateFx.RenderInfo renderInfo in reaction.renderers)
			{
				renderInfo.renderer.enabled = renderInfo.enable;
			}
		}
		if ((reaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
		{
			GameStateFx.MaterialInfo[] materialInfos = reaction.materialInfos;
			for (int i = 0; i < materialInfos.Length; i++)
			{
				foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[i].entries)
				{
					entry.slotInfo.renderer.GetSharedMaterials(GameStateFx._g_materialsCache);
					if (entry.slotInfo.slot >= 0 && entry.slotInfo.slot < GameStateFx._g_materialsCache.Count)
					{
						GameStateFx._g_materialsCache[entry.slotInfo.slot] = entry.material;
						entry.slotInfo.renderer.SetSharedMaterials(GameStateFx._g_materialsCache);
					}
				}
			}
		}
	}

	// Token: 0x06001181 RID: 4481 RVA: 0x0005DF9C File Offset: 0x0005C19C
	private bool _IsAllValid()
	{
		this._isValid = true;
		bool flag = false;
		this._hasDefaultAudioSource = (this.m_defaultAudioSource != null);
		foreach (GameStateFx.StateReaction[] array in this.m_stateMap.Values)
		{
			foreach (GameStateFx.StateReaction stateReaction in array)
			{
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Sound) != (GameStateFx.StateReaction.EOptions)0)
				{
					if ((stateReaction.soundInfo.options & GameStateFx.SoundEntry.EOptions.Source) != (GameStateFx.SoundEntry.EOptions)0)
					{
						if (!this._IsOneValid(stateReaction.soundInfo.source != null, "an AudioSource is unassigned."))
						{
							return false;
						}
					}
					else
					{
						flag = true;
						stateReaction.soundInfo.source = this.m_defaultAudioSource;
					}
					if (!this._IsOneValid(stateReaction.soundInfo.sound != null, "A sound is unassigned."))
					{
						return false;
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.GameObjects) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.GameObjectInfo gameObjectInfo in stateReaction.gameObjectInfos)
					{
						if (!this._IsOneValid(gameObjectInfo.gameObject != null, "A GameObject is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Behaviours) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.BehaviourInfo behaviourInfo in stateReaction.behaviourInfos)
					{
						if (!this._IsOneValid(behaviourInfo.behaviour != null, "A Behaviour is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Renderers) != (GameStateFx.StateReaction.EOptions)0)
				{
					foreach (GameStateFx.RenderInfo renderInfo in stateReaction.renderers)
					{
						if (!this._IsOneValid(renderInfo.renderer != null, "A Renderer is unassigned."))
						{
							return false;
						}
					}
				}
				if ((stateReaction.options & GameStateFx.StateReaction.EOptions.Materials) != (GameStateFx.StateReaction.EOptions)0)
				{
					GameStateFx.MaterialInfo[] materialInfos = stateReaction.materialInfos;
					for (int j = 0; j < materialInfos.Length; j++)
					{
						foreach (GameStateFx.MaterialInfo.Entry entry in materialInfos[j].entries)
						{
							if (!this._IsOneValid(entry.slotInfo.renderer != null, "A mat swap Renderer is unassigned"))
							{
								return false;
							}
						}
					}
				}
			}
		}
		if (flag && !this._hasDefaultAudioSource)
		{
			base.enabled = false;
			this._isValid = false;
			return false;
		}
		return true;
	}

	// Token: 0x06001182 RID: 4482 RVA: 0x0005E240 File Offset: 0x0005C440
	private bool _IsOneValid(bool isValidCondition, string msgFailReason)
	{
		if (isValidCondition)
		{
			return true;
		}
		this._isValid = false;
		base.enabled = false;
		return false;
	}

	// Token: 0x040014DA RID: 5338
	private const string preLog = "[GT/GameStateFx]  ";

	// Token: 0x040014DB RID: 5339
	private const string preErr = "[GT/GameStateFx]  ERROR!!!  ";

	// Token: 0x040014DC RID: 5340
	private bool _isValid;

	// Token: 0x040014DD RID: 5341
	[SerializeField]
	private MonoBehaviour m_stateProvider;

	// Token: 0x040014DE RID: 5342
	private IGameStateProvider _stateProvider;

	// Token: 0x040014DF RID: 5343
	[SerializeField]
	private AudioSource m_defaultAudioSource;

	// Token: 0x040014E0 RID: 5344
	private bool _hasDefaultAudioSource;

	// Token: 0x040014E1 RID: 5345
	[SerializeField]
	private GTEnumValueMap<GameStateFx.StateReaction[]> m_stateMap;

	// Token: 0x040014E2 RID: 5346
	private int _delayedExecContextFrameNum;

	// Token: 0x040014E3 RID: 5347
	private Queue<GameStateFx.StateReaction> _reactionQueue = new Queue<GameStateFx.StateReaction>(4);

	// Token: 0x040014E4 RID: 5348
	private static readonly List<Material> _g_materialsCache = new List<Material>(8);

	// Token: 0x0200028F RID: 655
	[Serializable]
	internal class StateReaction
	{
		// Token: 0x040014E5 RID: 5349
		[Tooltip("Options for what this reaction should do.")]
		public GameStateFx.StateReaction.EOptions options;

		// Token: 0x040014E6 RID: 5350
		public float delay;

		// Token: 0x040014E7 RID: 5351
		public GameStateFx.SoundEntry soundInfo;

		// Token: 0x040014E8 RID: 5352
		public GameStateFx.GameObjectInfo[] gameObjectInfos;

		// Token: 0x040014E9 RID: 5353
		public GameStateFx.BehaviourInfo[] behaviourInfos;

		// Token: 0x040014EA RID: 5354
		public GameStateFx.RenderInfo[] renderers;

		// Token: 0x040014EB RID: 5355
		public GameStateFx.MaterialInfo[] materialInfos;

		// Token: 0x02000290 RID: 656
		[Flags]
		public enum EOptions
		{
			// Token: 0x040014ED RID: 5357
			Delay = 1,
			// Token: 0x040014EE RID: 5358
			Sound = 2,
			// Token: 0x040014EF RID: 5359
			GameObjects = 4,
			// Token: 0x040014F0 RID: 5360
			Behaviours = 8,
			// Token: 0x040014F1 RID: 5361
			Renderers = 16,
			// Token: 0x040014F2 RID: 5362
			Materials = 32
		}
	}

	// Token: 0x02000291 RID: 657
	[Serializable]
	public struct SoundEntry
	{
		// Token: 0x040014F3 RID: 5363
		public GameStateFx.SoundEntry.EOptions options;

		// Token: 0x040014F4 RID: 5364
		public AudioSource source;

		// Token: 0x040014F5 RID: 5365
		public AudioResource sound;

		// Token: 0x040014F6 RID: 5366
		public float volume;

		// Token: 0x040014F7 RID: 5367
		public float pitch;

		// Token: 0x02000292 RID: 658
		[Flags]
		public enum EOptions
		{
			// Token: 0x040014F9 RID: 5369
			Source = 1,
			// Token: 0x040014FA RID: 5370
			Sound = 2,
			// Token: 0x040014FB RID: 5371
			Volume = 4,
			// Token: 0x040014FC RID: 5372
			Pitch = 8
		}
	}

	// Token: 0x02000293 RID: 659
	[Serializable]
	internal struct GameObjectInfo
	{
		// Token: 0x040014FD RID: 5373
		public bool activate;

		// Token: 0x040014FE RID: 5374
		public GameObject gameObject;
	}

	// Token: 0x02000294 RID: 660
	[Serializable]
	internal struct BehaviourInfo
	{
		// Token: 0x040014FF RID: 5375
		public bool enable;

		// Token: 0x04001500 RID: 5376
		public Behaviour behaviour;
	}

	// Token: 0x02000295 RID: 661
	[Serializable]
	internal struct RenderInfo
	{
		// Token: 0x04001501 RID: 5377
		public bool enable;

		// Token: 0x04001502 RID: 5378
		public Renderer renderer;
	}

	// Token: 0x02000296 RID: 662
	[Serializable]
	internal struct MaterialInfo
	{
		// Token: 0x04001503 RID: 5379
		public GameStateFx.MaterialInfo.Entry[] entries;

		// Token: 0x02000297 RID: 663
		[Serializable]
		internal struct Entry
		{
			// Token: 0x04001504 RID: 5380
			public GTRendererMatSlot slotInfo;

			// Token: 0x04001505 RID: 5381
			public Material material;
		}
	}
}
