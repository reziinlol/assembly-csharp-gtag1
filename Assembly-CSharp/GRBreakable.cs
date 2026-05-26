using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200074E RID: 1870
public class GRBreakable : MonoBehaviour, IGameHittable
{
	// Token: 0x1700046E RID: 1134
	// (get) Token: 0x06002F5E RID: 12126 RVA: 0x00102150 File Offset: 0x00100350
	public bool BrokenLocal
	{
		get
		{
			return this.brokenLocal;
		}
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x00102158 File Offset: 0x00100358
	private void OnEnable()
	{
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x00102171 File Offset: 0x00100371
	private void OnDisable()
	{
		if (this.gameEntity != null)
		{
			this.gameEntity.OnStateChanged -= this.OnEntityStateChanged;
		}
	}

	// Token: 0x06002F61 RID: 12129 RVA: 0x00102198 File Offset: 0x00100398
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		GRBreakable.BreakableState breakableState = (GRBreakable.BreakableState)nextState;
		if (breakableState == GRBreakable.BreakableState.Broken)
		{
			this.BreakLocal();
			return;
		}
		if (breakableState == GRBreakable.BreakableState.Unbroken)
		{
			this.RestoreLocal();
		}
	}

	// Token: 0x06002F62 RID: 12130 RVA: 0x001021BC File Offset: 0x001003BC
	public void BreakLocal()
	{
		if (!this.brokenLocal)
		{
			this.brokenLocal = true;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = false;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(false);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(true);
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.breakSound, this.breakSoundVolume);
			}
			GameEntity gameEntity;
			if (this.gameEntity.IsAuthority() && this.holdsRandomItem && this.itemSpawnProbability.TryForRandomItem(this.gameEntity, out gameEntity, 0))
			{
				this.gameEntity.manager.RequestCreateItem(gameEntity.gameObject.name.GetStaticHash(), this.itemSpawnLocation.position, this.itemSpawnLocation.rotation, 0L);
			}
		}
	}

	// Token: 0x06002F63 RID: 12131 RVA: 0x001022D8 File Offset: 0x001004D8
	public void RestoreLocal()
	{
		if (this.brokenLocal)
		{
			this.brokenLocal = false;
			if (this.breakableCollider != null)
			{
				this.breakableCollider.enabled = true;
			}
			for (int i = 0; i < this.disableWhenBroken.Count; i++)
			{
				this.disableWhenBroken[i].gameObject.SetActive(true);
			}
			for (int j = 0; j < this.enableWhenBroken.Count; j++)
			{
				this.enableWhenBroken[j].gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06002F64 RID: 12132 RVA: 0x00102368 File Offset: 0x00100568
	public bool IsHitValid(GameHitData hit)
	{
		return !this.brokenLocal && hit.hitTypeId == 0;
	}

	// Token: 0x06002F65 RID: 12133 RVA: 0x00102380 File Offset: 0x00100580
	public void OnHit(GameHitData hit)
	{
		if (hit.hitTypeId == 0 && (int)this.gameEntity.GetState() != 1)
		{
			this.gameEntity.RequestState(this.gameEntity.id, 1L);
			GameEntity gameEntity = this.gameEntity.manager.GetGameEntity(hit.hitByEntityId);
			if (gameEntity != null && gameEntity.IsHeldByLocalPlayer())
			{
				PlayerGameEvents.MiscEvent("GRSmashBreakable", 1);
			}
		}
	}

	// Token: 0x04003CD0 RID: 15568
	public GameEntity gameEntity;

	// Token: 0x04003CD1 RID: 15569
	public List<Transform> enableWhenBroken;

	// Token: 0x04003CD2 RID: 15570
	public List<Transform> disableWhenBroken;

	// Token: 0x04003CD3 RID: 15571
	public Collider breakableCollider;

	// Token: 0x04003CD4 RID: 15572
	public bool holdsRandomItem = true;

	// Token: 0x04003CD5 RID: 15573
	public Transform itemSpawnLocation;

	// Token: 0x04003CD6 RID: 15574
	public GRBreakableItemSpawnConfig itemSpawnProbability;

	// Token: 0x04003CD7 RID: 15575
	public AudioSource audioSource;

	// Token: 0x04003CD8 RID: 15576
	public AudioClip breakSound;

	// Token: 0x04003CD9 RID: 15577
	public float breakSoundVolume;

	// Token: 0x04003CDA RID: 15578
	private bool brokenLocal;

	// Token: 0x0200074F RID: 1871
	public enum BreakableState
	{
		// Token: 0x04003CDC RID: 15580
		Unbroken,
		// Token: 0x04003CDD RID: 15581
		Broken
	}
}
