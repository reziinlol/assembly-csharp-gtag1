using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x02000753 RID: 1875
public class GRCollectibleDispenser : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x1700046F RID: 1135
	// (get) Token: 0x06002F73 RID: 12147 RVA: 0x001026DA File Offset: 0x001008DA
	public bool CollectibleAlreadySpawned
	{
		get
		{
			return this.currentCollectible != null;
		}
	}

	// Token: 0x17000470 RID: 1136
	// (get) Token: 0x06002F74 RID: 12148 RVA: 0x001026E8 File Offset: 0x001008E8
	public bool ReadyToDispenseNewCollectible
	{
		get
		{
			double num = (double)this.collectibleRespawnTimeMinutes * 60.0;
			bool flag = (ulong)this.collectiblesDispensed < (ulong)((long)this.maxDispenseCount);
			return !this.CollectibleAlreadySpawned && flag && Time.timeAsDouble - this.collectibleDispenseRequestTime > num && Time.timeAsDouble - this.collectibleDispenseTime > num && Time.timeAsDouble - this.collectibleCollectedTime > num;
		}
	}

	// Token: 0x06002F75 RID: 12149 RVA: 0x00102754 File Offset: 0x00100954
	public void OnEntityInit()
	{
		GhostReactor reactor = GhostReactorManager.Get(this.gameEntity).reactor;
		if (reactor != null)
		{
			reactor.collectibleDispensers.Add(this);
		}
	}

	// Token: 0x06002F76 RID: 12150 RVA: 0x00102788 File Offset: 0x00100988
	public void OnEntityDestroy()
	{
		GhostReactorManager ghostReactorManager = GhostReactorManager.Get(this.gameEntity);
		if (ghostReactorManager != null && ghostReactorManager.reactor != null)
		{
			ghostReactorManager.reactor.collectibleDispensers.Remove(this);
		}
	}

	// Token: 0x06002F77 RID: 12151 RVA: 0x001027CC File Offset: 0x001009CC
	public void OnEntityStateChange(long prevState, long nextState)
	{
		uint num = this.collectiblesDispensed;
		uint num2 = this.collectiblesCollected;
		this.collectiblesDispensed = (uint)(nextState >> 32);
		this.collectiblesCollected = (uint)(nextState & (long)((ulong)-1));
		if (num != this.collectiblesDispensed)
		{
			this.collectibleDispenseTime = Time.timeAsDouble;
		}
		if (num2 != this.collectiblesCollected)
		{
			this.collectibleCollectedTime = Time.timeAsDouble;
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
		}
	}

	// Token: 0x06002F78 RID: 12152 RVA: 0x00102858 File Offset: 0x00100A58
	public void RequestDispenseCollectible()
	{
		if (this.ReadyToDispenseNewCollectible && this.gameEntity.IsAuthority())
		{
			this.gameEntity.manager.RequestCreateItem(this.collectiblePrefab.name.GetStaticHash(), this.spawnLocation.position, this.spawnLocation.rotation, (long)this.gameEntity.manager.GetNetIdFromEntityId(this.gameEntity.id));
			this.collectiblesDispensed += 1U;
			this.collectibleDispenseTime = Time.timeAsDouble;
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long newState = num << 32 | num2;
			this.gameEntity.RequestState(this.gameEntity.id, newState);
		}
	}

	// Token: 0x06002F79 RID: 12153 RVA: 0x00102918 File Offset: 0x00100B18
	public void OnCollectibleConsumed()
	{
		if (this.currentCollectible != null && this.currentCollectible.IsNotNull())
		{
			GRCollectible grcollectible = this.currentCollectible;
			grcollectible.OnCollected = (Action)Delegate.Remove(grcollectible.OnCollected, new Action(this.OnCollectibleConsumed));
			GameEntity entity = this.currentCollectible.entity;
			entity.OnGrabbed = (Action)Delegate.Remove(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
			this.currentCollectible = null;
		}
		this.collectiblesCollected += 1U;
		this.collectibleCollectedTime = Time.timeAsDouble;
		if (this.gameEntity.IsAuthority())
		{
			long num = (long)((ulong)this.collectiblesDispensed);
			long num2 = (long)((ulong)this.collectiblesCollected);
			long newState = num << 32 | num2;
			this.gameEntity.RequestState(this.gameEntity.id, newState);
		}
		if ((ulong)this.collectiblesCollected >= (ulong)((long)this.maxDispenseCount))
		{
			this.dispenserExhaustedEffect.Play();
			this.audioSource.PlayOneShot(this.dispenserExhaustedClip, this.dispenserExhaustedVolume);
			this.stillDispensingModel.gameObject.SetActive(false);
			this.fullyConsumedModel.gameObject.SetActive(true);
			return;
		}
		this.collectibleTakenEffect.Play();
		this.audioSource.PlayOneShot(this.collectibleTakenClip, this.collectibleTakenVolume);
	}

	// Token: 0x06002F7A RID: 12154 RVA: 0x00102A64 File Offset: 0x00100C64
	public void GetSpawnedCollectible(GRCollectible collectible)
	{
		this.currentCollectible = collectible;
		collectible.OnCollected = (Action)Delegate.Combine(collectible.OnCollected, new Action(this.OnCollectibleConsumed));
		GameEntity entity = collectible.entity;
		entity.OnGrabbed = (Action)Delegate.Combine(entity.OnGrabbed, new Action(this.OnCollectibleConsumed));
	}

	// Token: 0x04003CE7 RID: 15591
	public GameEntity gameEntity;

	// Token: 0x04003CE8 RID: 15592
	public GameEntity collectiblePrefab;

	// Token: 0x04003CE9 RID: 15593
	public Transform spawnLocation;

	// Token: 0x04003CEA RID: 15594
	public LayerMask collectibleLayerMask;

	// Token: 0x04003CEB RID: 15595
	public float collectibleRespawnTimeMinutes = 1.5f;

	// Token: 0x04003CEC RID: 15596
	public int maxDispenseCount = 3;

	// Token: 0x04003CED RID: 15597
	public AudioSource audioSource;

	// Token: 0x04003CEE RID: 15598
	public Transform stillDispensingModel;

	// Token: 0x04003CEF RID: 15599
	public Transform fullyConsumedModel;

	// Token: 0x04003CF0 RID: 15600
	public ParticleSystem collectibleTakenEffect;

	// Token: 0x04003CF1 RID: 15601
	public AudioClip collectibleTakenClip;

	// Token: 0x04003CF2 RID: 15602
	public float collectibleTakenVolume;

	// Token: 0x04003CF3 RID: 15603
	public ParticleSystem dispenserExhaustedEffect;

	// Token: 0x04003CF4 RID: 15604
	public AudioClip dispenserExhaustedClip;

	// Token: 0x04003CF5 RID: 15605
	public float dispenserExhaustedVolume;

	// Token: 0x04003CF6 RID: 15606
	private GRCollectible currentCollectible;

	// Token: 0x04003CF7 RID: 15607
	private Coroutine getSpawnedCollectibleCoroutine;

	// Token: 0x04003CF8 RID: 15608
	private static Collider[] overlapColliders = new Collider[10];

	// Token: 0x04003CF9 RID: 15609
	private uint collectiblesDispensed;

	// Token: 0x04003CFA RID: 15610
	private uint collectiblesCollected;

	// Token: 0x04003CFB RID: 15611
	private double collectibleDispenseRequestTime = -10000.0;

	// Token: 0x04003CFC RID: 15612
	private double collectibleDispenseTime = -10000.0;

	// Token: 0x04003CFD RID: 15613
	private double collectibleCollectedTime = -10000.0;
}
