using System;
using GorillaTag.Audio;
using UnityEngine;

// Token: 0x020006B5 RID: 1717
public class GameEntityDelayedReturn : MonoBehaviour, IGameEntityComponent, IDelayedExecListener
{
	// Token: 0x06002AEA RID: 10986 RVA: 0x000E5804 File Offset: 0x000E3A04
	public void OnEntityInit()
	{
		Transform transform = base.transform;
		this.initialPosition = transform.position;
		this.initialRotation = transform.rotation;
		this.initialScale = transform.localScale;
		Rigidbody componentInParent = base.GetComponentInParent<Rigidbody>();
		this.initialIsKinematic = (componentInParent != null && componentInParent.isKinematic);
		this.initialized = true;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnInteractionStarted));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Combine(gameEntity2.OnSnapped, new Action(this.OnInteractionStarted));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnAttached = (Action)Delegate.Combine(gameEntity3.OnAttached, new Action(this.OnInteractionStarted));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnReleased = (Action)Delegate.Combine(gameEntity4.OnReleased, new Action(this.OnInteractionEnded));
		GameEntity gameEntity5 = this.gameEntity;
		gameEntity5.OnUnsnapped = (Action)Delegate.Combine(gameEntity5.OnUnsnapped, new Action(this.OnInteractionEnded));
		GameEntity gameEntity6 = this.gameEntity;
		gameEntity6.OnDetached = (Action)Delegate.Combine(gameEntity6.OnDetached, new Action(this.OnInteractionEnded));
		if (!this.IsCurrentlyInteracting())
		{
			this.StartTimer();
		}
	}

	// Token: 0x06002AEB RID: 10987 RVA: 0x000E595C File Offset: 0x000E3B5C
	public void OnEntityDestroy()
	{
		if (!this.initialized)
		{
			return;
		}
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnInteractionStarted));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnSnapped = (Action)Delegate.Remove(gameEntity2.OnSnapped, new Action(this.OnInteractionStarted));
		GameEntity gameEntity3 = this.gameEntity;
		gameEntity3.OnAttached = (Action)Delegate.Remove(gameEntity3.OnAttached, new Action(this.OnInteractionStarted));
		GameEntity gameEntity4 = this.gameEntity;
		gameEntity4.OnReleased = (Action)Delegate.Remove(gameEntity4.OnReleased, new Action(this.OnInteractionEnded));
		GameEntity gameEntity5 = this.gameEntity;
		gameEntity5.OnUnsnapped = (Action)Delegate.Remove(gameEntity5.OnUnsnapped, new Action(this.OnInteractionEnded));
		GameEntity gameEntity6 = this.gameEntity;
		gameEntity6.OnDetached = (Action)Delegate.Remove(gameEntity6.OnDetached, new Action(this.OnInteractionEnded));
		this.CancelTimer();
	}

	// Token: 0x06002AEC RID: 10988 RVA: 0x000E5A62 File Offset: 0x000E3C62
	public void OnEntityStateChange(long prevState, long newState)
	{
		this.RestartTimer();
	}

	// Token: 0x06002AED RID: 10989 RVA: 0x000E5A6A File Offset: 0x000E3C6A
	private void OnDestroy()
	{
		this.CancelDelayedFx();
	}

	// Token: 0x06002AEE RID: 10990 RVA: 0x000E5A72 File Offset: 0x000E3C72
	private void OnInteractionStarted()
	{
		this.CancelTimer();
	}

	// Token: 0x06002AEF RID: 10991 RVA: 0x000E5A7A File Offset: 0x000E3C7A
	private void OnInteractionEnded()
	{
		if (!this.IsCurrentlyInteracting())
		{
			this.StartTimer();
		}
	}

	// Token: 0x06002AF0 RID: 10992 RVA: 0x000E5A8C File Offset: 0x000E3C8C
	public void StartTimer()
	{
		this.CancelDelayedFx();
		this._callGenerationId++;
		int callGenerationId = this._callGenerationId;
		GameEntityDelayedReturn.Options options = this.m_options;
		GTDelayedExec.Add(this, options.delay, callGenerationId << 2 | 1);
		GTDelayedExec.Add(this, options.delay + options.reappearDelay, callGenerationId << 2 | 2);
		this._timerRunning = true;
		if (options.disappearSound != null)
		{
			this._delayedDisappearAudioIndex = GTAudioOneShot.PlayDelayed(options.disappearSound, base.transform.parent, base.transform.localPosition, options.delay, options.disappearVolume, 1f);
		}
		else
		{
			this._delayedDisappearAudioIndex = -1;
		}
		if (options.pooledDisappearPrefab != null)
		{
			this._delayedDisappearPoolIndex = ObjectPools.InstantiateDelayed(options.pooledDisappearPrefab, base.transform.parent, base.transform.localPosition, options.delay);
		}
		else
		{
			this._delayedDisappearPoolIndex = -1;
		}
		if (options.beepSound == null || options.beepPhases == null || options.beepPhases.Length == 0)
		{
			return;
		}
		int contextId = callGenerationId << 2 | 0;
		for (int i = 0; i < options.beepPhases.Length; i++)
		{
			float interval = options.beepPhases[i].interval;
			if (interval > 0f)
			{
				float num = (i + 1 < options.beepPhases.Length) ? options.beepPhases[i + 1].timeRemaining : 0f;
				float num2 = Mathf.Min(options.beepPhases[i].timeRemaining, options.delay);
				if (num2 > num)
				{
					float num3 = options.delay - num2;
					float num4 = options.delay - num;
					for (float num5 = num3; num5 < num4; num5 += interval)
					{
						GTDelayedExec.Add(this, num5, contextId);
					}
				}
			}
		}
	}

	// Token: 0x06002AF1 RID: 10993 RVA: 0x000E5A7A File Offset: 0x000E3C7A
	public void RestartTimer()
	{
		if (!this.IsCurrentlyInteracting())
		{
			this.StartTimer();
		}
	}

	// Token: 0x06002AF2 RID: 10994 RVA: 0x000E5C54 File Offset: 0x000E3E54
	public void CancelTimer()
	{
		if (!this._timerRunning)
		{
			return;
		}
		this._callGenerationId++;
		this._timerRunning = false;
		this.CancelDelayedFx();
		if (!this.gameEntity.gameObject.activeSelf)
		{
			this.gameEntity.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002AF3 RID: 10995 RVA: 0x000E5CA8 File Offset: 0x000E3EA8
	private void CancelDelayedFx()
	{
		if (this._delayedDisappearAudioIndex >= 0)
		{
			GTAudioOneShot.CancelDelayed(this._delayedDisappearAudioIndex);
			this._delayedDisappearAudioIndex = -1;
		}
		if (this._delayedDisappearPoolIndex >= 0)
		{
			ObjectPools.CancelDelayedInstantiate(this._delayedDisappearPoolIndex);
			this._delayedDisappearPoolIndex = -1;
		}
	}

	// Token: 0x06002AF4 RID: 10996 RVA: 0x000E5CE0 File Offset: 0x000E3EE0
	void IDelayedExecListener.OnDelayedAction(int contextId)
	{
		if (contextId >> 2 != this._callGenerationId)
		{
			return;
		}
		if (this.gameEntity == null)
		{
			return;
		}
		int num = contextId & 3;
		GameEntityDelayedReturn.Options options = this.m_options;
		switch (num)
		{
		case 0:
			if (this._delayedDisappearAudioIndex >= 0)
			{
				GTAudioOneShot.UpdateDelayed(this._delayedDisappearAudioIndex, base.transform.parent, base.transform.localPosition);
			}
			if (this._delayedDisappearPoolIndex >= 0)
			{
				ObjectPools.UpdateDelayedInstantiate(this._delayedDisappearPoolIndex, base.transform.parent, base.transform.localPosition);
			}
			if (options.beepSound != null)
			{
				GTAudioOneShot.Play(options.beepSound, base.transform.position, options.beepVolume, 1f);
				return;
			}
			break;
		case 1:
			this.Disappear();
			return;
		case 2:
			this.Reappear();
			break;
		default:
			return;
		}
	}

	// Token: 0x06002AF5 RID: 10997 RVA: 0x000E5DB7 File Offset: 0x000E3FB7
	private bool IsCurrentlyInteracting()
	{
		return this.gameEntity.IsHeld() || this.gameEntity.snappedByActorNumber != -1 || this.gameEntity.attachedToEntityId != GameEntityId.Invalid;
	}

	// Token: 0x06002AF6 RID: 10998 RVA: 0x000E5DEB File Offset: 0x000E3FEB
	private void Disappear()
	{
		this.gameEntity.gameObject.SetActive(false);
	}

	// Token: 0x06002AF7 RID: 10999 RVA: 0x000E5E00 File Offset: 0x000E4000
	private void Reappear()
	{
		this._timerRunning = false;
		if (this.resetTarget != null)
		{
			base.transform.SetPositionAndRotation(this.resetTarget.position, this.resetTarget.rotation);
			base.transform.localScale = this.resetTarget.localScale;
		}
		else
		{
			base.transform.SetPositionAndRotation(this.initialPosition, this.initialRotation);
			base.transform.localScale = this.initialScale;
		}
		Rigidbody componentInParent = base.GetComponentInParent<Rigidbody>();
		if (componentInParent != null)
		{
			componentInParent.linearVelocity = Vector3.zero;
			componentInParent.angularVelocity = Vector3.zero;
			componentInParent.isKinematic = (this.forceKinematicOnReset || this.initialIsKinematic);
		}
		this.gameEntity.gameObject.SetActive(true);
		Vector3 position = base.transform.position;
		GameEntityDelayedReturn.Options options = this.m_options;
		if (options.reappearSound != null)
		{
			GTAudioOneShot.Play(options.reappearSound, position, options.reappearVolume, 1f);
		}
		if (options.pooledReappearPrefab != null)
		{
			ObjectPools.instance.Instantiate(options.pooledReappearPrefab, position, true);
		}
	}

	// Token: 0x06002AF8 RID: 11000 RVA: 0x000E5F2A File Offset: 0x000E412A
	public void ReturnNow()
	{
		this.CancelTimer();
		this.Disappear();
		this.Reappear();
	}

	// Token: 0x06002AF9 RID: 11001 RVA: 0x000E5F3E File Offset: 0x000E413E
	public void SetResetTarget(Transform target)
	{
		this.resetTarget = target;
	}

	// Token: 0x06002AFA RID: 11002 RVA: 0x000E5F47 File Offset: 0x000E4147
	internal void Configure(GameEntityDelayedReturn.Options options)
	{
		this.m_options = options;
	}

	// Token: 0x0400378E RID: 14222
	private const int k_actionBits = 2;

	// Token: 0x0400378F RID: 14223
	private const int k_actionMask = 3;

	// Token: 0x04003790 RID: 14224
	private const int k_actionBeep = 0;

	// Token: 0x04003791 RID: 14225
	private const int k_actionDisappear = 1;

	// Token: 0x04003792 RID: 14226
	private const int k_actionReappear = 2;

	// Token: 0x04003793 RID: 14227
	public GameEntity gameEntity;

	// Token: 0x04003794 RID: 14228
	[SerializeField]
	private GameEntityDelayedReturn.Options m_options = new GameEntityDelayedReturn.Options
	{
		delay = 30f,
		reappearDelay = 0.5f,
		disappearSound = null,
		disappearVolume = 1f,
		pooledDisappearPrefab = null,
		reappearSound = null,
		reappearVolume = 1f,
		pooledReappearPrefab = null,
		beepSound = null,
		beepVolume = 1f,
		beepPhases = null
	};

	// Token: 0x04003795 RID: 14229
	[Tooltip("If set, the entity teleports here instead of its initial position.")]
	public Transform resetTarget;

	// Token: 0x04003796 RID: 14230
	[Tooltip("If true, the Rigidbody is forced kinematic after return regardless of its initial state.")]
	public bool forceKinematicOnReset;

	// Token: 0x04003797 RID: 14231
	private Vector3 initialPosition;

	// Token: 0x04003798 RID: 14232
	private Quaternion initialRotation;

	// Token: 0x04003799 RID: 14233
	private Vector3 initialScale;

	// Token: 0x0400379A RID: 14234
	private bool initialIsKinematic;

	// Token: 0x0400379B RID: 14235
	private bool initialized;

	// Token: 0x0400379C RID: 14236
	private int _callGenerationId;

	// Token: 0x0400379D RID: 14237
	private int _delayedDisappearAudioIndex = -1;

	// Token: 0x0400379E RID: 14238
	private int _delayedDisappearPoolIndex = -1;

	// Token: 0x0400379F RID: 14239
	private bool _timerRunning;

	// Token: 0x020006B6 RID: 1718
	[Serializable]
	public struct Options
	{
		// Token: 0x040037A0 RID: 14240
		public float delay;

		// Token: 0x040037A1 RID: 14241
		[Tooltip("Seconds the entity stays hidden between disappear and reappear.")]
		public float reappearDelay;

		// Token: 0x040037A2 RID: 14242
		public AudioClip disappearSound;

		// Token: 0x040037A3 RID: 14243
		public float disappearVolume;

		// Token: 0x040037A4 RID: 14244
		public GameObject pooledDisappearPrefab;

		// Token: 0x040037A5 RID: 14245
		public AudioClip reappearSound;

		// Token: 0x040037A6 RID: 14246
		public float reappearVolume;

		// Token: 0x040037A7 RID: 14247
		public GameObject pooledReappearPrefab;

		// Token: 0x040037A8 RID: 14248
		public AudioClip beepSound;

		// Token: 0x040037A9 RID: 14249
		public float beepVolume;

		// Token: 0x040037AA RID: 14250
		[Tooltip("Beep phases keyed by seconds remaining. Must be ordered from most to least time remaining.")]
		public GameEntityDelayedReturn.BeepPhase[] beepPhases;
	}

	// Token: 0x020006B7 RID: 1719
	[Serializable]
	public struct BeepPhase
	{
		// Token: 0x040037AB RID: 14251
		[Tooltip("Beeping starts when this many seconds remain.")]
		public float timeRemaining;

		// Token: 0x040037AC RID: 14252
		[Tooltip("Seconds between beeps during this phase.")]
		public float interval;
	}
}
