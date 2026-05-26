using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x0200066B RID: 1643
public abstract class CosmeticCritter : MonoBehaviour
{
	// Token: 0x1700041D RID: 1053
	// (get) Token: 0x0600290E RID: 10510 RVA: 0x000DE683 File Offset: 0x000DC883
	// (set) Token: 0x0600290F RID: 10511 RVA: 0x000DE68B File Offset: 0x000DC88B
	public int Seed { get; protected set; }

	// Token: 0x1700041E RID: 1054
	// (get) Token: 0x06002910 RID: 10512 RVA: 0x000DE694 File Offset: 0x000DC894
	// (set) Token: 0x06002911 RID: 10513 RVA: 0x000DE69C File Offset: 0x000DC89C
	public CosmeticCritterSpawner Spawner { get; protected set; }

	// Token: 0x1700041F RID: 1055
	// (get) Token: 0x06002912 RID: 10514 RVA: 0x000DE6A5 File Offset: 0x000DC8A5
	// (set) Token: 0x06002913 RID: 10515 RVA: 0x000DE6AD File Offset: 0x000DC8AD
	public Type CachedType { get; private set; }

	// Token: 0x06002914 RID: 10516 RVA: 0x000DE6B6 File Offset: 0x000DC8B6
	public int GetGlobalMaxCritters()
	{
		return this.globalMaxCritters;
	}

	// Token: 0x06002915 RID: 10517 RVA: 0x000DE6BE File Offset: 0x000DC8BE
	public void SetSeedSpawnerTypeAndTime(int seed, CosmeticCritterSpawner spawner, Type type, double time)
	{
		this.Seed = seed;
		this.Spawner = spawner;
		this.CachedType = type;
		this.startTime = time;
	}

	// Token: 0x06002916 RID: 10518 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnSpawn()
	{
	}

	// Token: 0x06002917 RID: 10519 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void OnDespawn()
	{
	}

	// Token: 0x06002918 RID: 10520 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void SetRandomVariables()
	{
	}

	// Token: 0x06002919 RID: 10521
	public abstract void Tick();

	// Token: 0x0600291A RID: 10522 RVA: 0x000DE6DD File Offset: 0x000DC8DD
	protected double GetAliveTime()
	{
		if (!PhotonNetwork.InRoom)
		{
			return Time.timeAsDouble - this.startTime;
		}
		return PhotonNetwork.Time - this.startTime;
	}

	// Token: 0x0600291B RID: 10523 RVA: 0x000DE6FF File Offset: 0x000DC8FF
	public virtual bool Expired()
	{
		return this.GetAliveTime() > (double)this.lifetime || this.GetAliveTime() < 0.0;
	}

	// Token: 0x04003598 RID: 13720
	[Tooltip("After this many seconds the critter will forcibly despawn.")]
	[SerializeField]
	protected float lifetime;

	// Token: 0x04003599 RID: 13721
	[Tooltip("The maximum number of this kind of critter that can be in the room at any given time.")]
	[SerializeField]
	private int globalMaxCritters;

	// Token: 0x0400359D RID: 13725
	protected double startTime;
}
