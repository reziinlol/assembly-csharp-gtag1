using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200029A RID: 666
public class GTAnimator : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x170001B4 RID: 436
	// (get) Token: 0x06001189 RID: 4489 RVA: 0x0005E277 File Offset: 0x0005C477
	public Animation animationComponent
	{
		get
		{
			return this.m_animationComponent;
		}
	}

	// Token: 0x170001B5 RID: 437
	// (get) Token: 0x0600118A RID: 4490 RVA: 0x0005E27F File Offset: 0x0005C47F
	// (set) Token: 0x0600118B RID: 4491 RVA: 0x0005E287 File Offset: 0x0005C487
	public bool hasAnimationComponent { get; private set; }

	// Token: 0x0600118C RID: 4492 RVA: 0x0005E290 File Offset: 0x0005C490
	protected void Awake()
	{
		this.Init();
	}

	// Token: 0x0600118D RID: 4493 RVA: 0x0005E298 File Offset: 0x0005C498
	public void Init()
	{
		if (this._wasInitCalled)
		{
			return;
		}
		this._wasInitCalled = true;
		this.hasAnimationComponent = (this.m_animationComponent != null);
		bool hasAnimationComponent = this.hasAnimationComponent;
		this.m_animationMap.Init();
		foreach (GTAnimator.AnimClipAndGObjs animClipAndGObjs in this.m_animationMap.Values)
		{
			this._allStaticGobjs.UnionWith(animClipAndGObjs.endStaticGameObjects);
		}
	}

	// Token: 0x0600118E RID: 4494 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEnable()
	{
	}

	// Token: 0x170001B6 RID: 438
	// (get) Token: 0x0600118F RID: 4495 RVA: 0x0005E328 File Offset: 0x0005C528
	public bool IsPlaying
	{
		get
		{
			return this.m_animationComponent.isPlaying;
		}
	}

	// Token: 0x06001190 RID: 4496 RVA: 0x0005E335 File Offset: 0x0005C535
	public void SetState(long enumValueAsLong)
	{
		if (!this._wasInitCalled)
		{
			this.Init();
		}
		if (this._currentStateAsLong != enumValueAsLong)
		{
			this.TryPlay(enumValueAsLong);
		}
	}

	// Token: 0x06001191 RID: 4497 RVA: 0x0005E358 File Offset: 0x0005C558
	public bool TryPlay(long enumValueAsLong)
	{
		GTAnimator.AnimClipAndGObjs animClipAndGObjs;
		if (!this.hasAnimationComponent || !this.m_animationMap.TryGet(enumValueAsLong, out animClipAndGObjs))
		{
			return false;
		}
		foreach (GameObject gameObject in this._allStaticGobjs)
		{
			gameObject.SetActive(false);
		}
		GameObject[] animatedGameObjects = this.m_animatedGameObjects;
		for (int i = 0; i < animatedGameObjects.Length; i++)
		{
			animatedGameObjects[i].SetActive(true);
		}
		this._currentStateAsLong = enumValueAsLong;
		this.m_animationComponent.clip = animClipAndGObjs.animClip;
		this.m_animationComponent.Play();
		if (animClipAndGObjs.soundBankToPlayOnStart)
		{
			animClipAndGObjs.soundBankToPlayOnStart.Play();
		}
		if (!animClipAndGObjs.animClip.isLooping)
		{
			this._frameCountWhenLastPlayed = Time.frameCount;
			GTDelayedExec.Add(this, animClipAndGObjs.animClip.length, this._frameCountWhenLastPlayed);
		}
		return true;
	}

	// Token: 0x06001192 RID: 4498 RVA: 0x0005E450 File Offset: 0x0005C650
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (!base.enabled || this._frameCountWhenLastPlayed != contextId)
		{
			return;
		}
		this.m_animationComponent.Stop();
		for (int i = 0; i < this.m_animatedGameObjects.Length; i++)
		{
			if (this.m_animatedGameObjects[i] != null)
			{
				this.m_animatedGameObjects[i].SetActive(false);
			}
		}
		GTAnimator.AnimClipAndGObjs animClipAndGObjs;
		GameObject[] array;
		if (this.m_animationMap.TryGet(this._currentStateAsLong, out animClipAndGObjs) && animClipAndGObjs.endStaticGameObjects != null && animClipAndGObjs.endStaticGameObjects.Length != 0)
		{
			array = animClipAndGObjs.endStaticGameObjects;
		}
		else
		{
			array = this.m_defaultStaticGameObjects;
		}
		if (array != null)
		{
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j] != null)
				{
					array[j].SetActive(true);
				}
			}
		}
		if (this._queuedStateAsLong != -9223372036854775808L)
		{
			long queuedStateAsLong = this._queuedStateAsLong;
			this._queuedStateAsLong = long.MinValue;
			this.TryPlay(queuedStateAsLong);
		}
	}

	// Token: 0x06001193 RID: 4499 RVA: 0x0005E537 File Offset: 0x0005C737
	public void Stop()
	{
		if (this.m_animationComponent != null)
		{
			this.m_animationComponent.Stop();
		}
	}

	// Token: 0x06001194 RID: 4500 RVA: 0x0005E554 File Offset: 0x0005C754
	public void QueueState(long enumValueAsLong)
	{
		if (!this._wasInitCalled)
		{
			this.Init();
		}
		if (this._queuedStateAsLong == enumValueAsLong || this._currentStateAsLong == enumValueAsLong)
		{
			return;
		}
		if (!this.IsPlaying || this._IsCurrentClipLoopable())
		{
			this.TryPlay(enumValueAsLong);
			return;
		}
		this._queuedStateAsLong = enumValueAsLong;
	}

	// Token: 0x06001195 RID: 4501 RVA: 0x0005E5A4 File Offset: 0x0005C7A4
	private bool _IsCurrentClipLoopable()
	{
		if (this.m_animationComponent == null)
		{
			return false;
		}
		AnimationClip clip = this.m_animationComponent.clip;
		if (clip == null)
		{
			return false;
		}
		WrapMode wrapMode = clip.wrapMode;
		return wrapMode == WrapMode.Loop || wrapMode == WrapMode.PingPong;
	}

	// Token: 0x04001506 RID: 5382
	private const string preLog = "[GTAnimator]  ";

	// Token: 0x04001507 RID: 5383
	private const string preErr = "[GTAnimator]  ERROR!!!  ";

	// Token: 0x04001508 RID: 5384
	private const string preErrBeta = "[GTAnimator]  ERROR!!!  (beta only log)  ";

	// Token: 0x04001509 RID: 5385
	[Tooltip("Assign a unity Animation component (not to be confused with less performant Animator Component).")]
	[SerializeField]
	private Animation m_animationComponent;

	// Token: 0x0400150B RID: 5387
	[Tooltip("These will be activated when animation starts playing and deactivated when any anim finishes playing.")]
	[SerializeField]
	private GameObject[] m_animatedGameObjects;

	// Token: 0x0400150C RID: 5388
	[Tooltip("If an enum map value is not defined then these will be activated.")]
	[SerializeField]
	private GameObject[] m_defaultStaticGameObjects;

	// Token: 0x0400150D RID: 5389
	[Header("Enum To Animation Mapping")]
	[Tooltip("Map an enum's values to specific AnimationClips.")]
	[SerializeField]
	internal GTEnumValueMap<GTAnimator.AnimClipAndGObjs> m_animationMap;

	// Token: 0x0400150E RID: 5390
	private readonly HashSet<GameObject> _allStaticGobjs = new HashSet<GameObject>();

	// Token: 0x0400150F RID: 5391
	private const long _k_invalidState = -9223372036854775808L;

	// Token: 0x04001510 RID: 5392
	private long _currentStateAsLong = long.MinValue;

	// Token: 0x04001511 RID: 5393
	private int _frameCountWhenLastPlayed;

	// Token: 0x04001512 RID: 5394
	private bool _wasInitCalled;

	// Token: 0x04001513 RID: 5395
	private long _queuedStateAsLong = long.MinValue;

	// Token: 0x0200029B RID: 667
	[Serializable]
	public struct AnimClipAndGObjs
	{
		// Token: 0x04001514 RID: 5396
		public AnimationClip animClip;

		// Token: 0x04001515 RID: 5397
		public SoundBankPlayer soundBankToPlayOnStart;

		// Token: 0x04001516 RID: 5398
		[Tooltip("These GameObjects will be activated when the animation clip finishes playing.")]
		public GameObject[] endStaticGameObjects;
	}
}
