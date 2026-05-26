using System;
using GorillaTagScripts.GhostReactor;
using UnityEngine;

// Token: 0x020007C8 RID: 1992
public class GRRecycler : MonoBehaviourTick
{
	// Token: 0x060032C5 RID: 12997 RVA: 0x001164C4 File Offset: 0x001146C4
	public override void Tick()
	{
		if (this.closed && !this.anim.isPlaying)
		{
			if (!this.playedAudio)
			{
				this.audioSource.volume = this.recyclerRunningAudioVolume;
				this.audioSource.PlayOneShot(this.recyclerRunningAudio);
				this.playedAudio = true;
			}
			this.timeRemaining -= Time.deltaTime;
			if (this.timeRemaining <= 0f)
			{
				this.anim.PlayQueued("Recycler_Open", QueueMode.CompleteOthers);
				this.closed = false;
				if (this.closeEffects != null && this.openEffects != null)
				{
					this.closeEffects.Stop();
					this.openEffects.Play();
				}
			}
		}
	}

	// Token: 0x060032C6 RID: 12998 RVA: 0x00116587 File Offset: 0x00114787
	public void Init(GhostReactor reactor)
	{
		this.reactor = reactor;
	}

	// Token: 0x060032C7 RID: 12999 RVA: 0x00116590 File Offset: 0x00114790
	public int GetRecycleValue(GRTool.GRToolType type)
	{
		return this.reactor.toolProgression.GetRecycleShiftCredit(type);
	}

	// Token: 0x060032C8 RID: 13000 RVA: 0x001165A3 File Offset: 0x001147A3
	public void ScanItem(GameEntityId id)
	{
		this.scanner.ScanItem(id);
	}

	// Token: 0x060032C9 RID: 13001 RVA: 0x001165B4 File Offset: 0x001147B4
	public void RecycleItem()
	{
		if (this.anim != null)
		{
			this.anim.Play("Recycler_Close");
		}
		if (this.closeEffects != null && this.openEffects != null)
		{
			this.openEffects.Stop();
			this.closeEffects.Play();
		}
		this.closed = true;
		this.playedAudio = false;
		this.timeRemaining = this.closeDuration;
	}

	// Token: 0x060032CA RID: 13002 RVA: 0x0011662C File Offset: 0x0011482C
	private void OnTriggerEnter(Collider other)
	{
		if (this.reactor == null)
		{
			Debug.LogFormat("GRRecycler reactor is null?", Array.Empty<object>());
			return;
		}
		if (!this.reactor.grManager.IsAuthority())
		{
			Debug.LogFormat("GRRecycler is not authority.", Array.Empty<object>());
			return;
		}
		GRTool componentInParent = other.gameObject.GetComponentInParent<GRTool>();
		if (componentInParent == null)
		{
			Debug.LogFormat("GRRecycler Colliding Object is not a GRTool.", Array.Empty<object>());
			return;
		}
		GRTool.GRToolType toolType = other.gameObject.GetToolType();
		int recycleValue = this.GetRecycleValue(toolType);
		if (this.reactor != null)
		{
			int count = this.reactor.vrRigs.Count;
			for (int i = 0; i < count; i++)
			{
				GRPlayer grplayer = GRPlayer.Get(this.reactor.vrRigs[i]);
				if (grplayer != null)
				{
					grplayer.IncrementSynchronizedSessionStat(GRPlayer.SynchronizedSessionStat.EarnedCredits, (float)recycleValue);
				}
			}
		}
		Debug.LogFormat("GRRecycler Recycle Value is {0}", new object[]
		{
			recycleValue
		});
		if (GRPlayer.Get(componentInParent.gameEntity.lastHeldByActorNumber) == null)
		{
			Debug.LogFormat("GRRecycler Tool Not last held by a player (?), can't recycle.", Array.Empty<object>());
			return;
		}
		Debug.LogFormat("GRRecycler Refunding player {0} {1} Currency and Destroying Tool.", new object[]
		{
			componentInParent.gameEntity.lastHeldByActorNumber,
			recycleValue
		});
		if (toolType != GRTool.GRToolType.None)
		{
			this.reactor.grManager.RequestRecycleItem(componentInParent.gameEntity.lastHeldByActorNumber, componentInParent.gameEntity.id, toolType);
		}
	}

	// Token: 0x040041F9 RID: 16889
	private GameEntity gameEntity;

	// Token: 0x040041FA RID: 16890
	public ParticleSystem closeEffects;

	// Token: 0x040041FB RID: 16891
	public ParticleSystem openEffects;

	// Token: 0x040041FC RID: 16892
	[NonSerialized]
	public GhostReactor reactor;

	// Token: 0x040041FD RID: 16893
	public GRRecyclerScanner scanner;

	// Token: 0x040041FE RID: 16894
	public Animation anim;

	// Token: 0x040041FF RID: 16895
	public float closeDuration = 1f;

	// Token: 0x04004200 RID: 16896
	private float timeRemaining;

	// Token: 0x04004201 RID: 16897
	private bool closed;

	// Token: 0x04004202 RID: 16898
	private bool playedAudio;

	// Token: 0x04004203 RID: 16899
	public AudioSource audioSource;

	// Token: 0x04004204 RID: 16900
	public AudioClip recyclerRunningAudio;

	// Token: 0x04004205 RID: 16901
	public float recyclerRunningAudioVolume = 0.5f;
}
