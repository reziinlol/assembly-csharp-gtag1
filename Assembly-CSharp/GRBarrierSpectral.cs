using System;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x02000748 RID: 1864
public class GRBarrierSpectral : MonoBehaviour, IGameEntityComponent, IGameHittable
{
	// Token: 0x06002F3E RID: 12094 RVA: 0x00101578 File Offset: 0x000FF778
	public void Awake()
	{
		this.hitFx.SetActive(false);
		this.destroyedFx.SetActive(false);
	}

	// Token: 0x06002F3F RID: 12095 RVA: 0x00101594 File Offset: 0x000FF794
	public void OnEntityInit()
	{
		this.entity.SetState((long)this.health);
		Vector3 localScale = BitPackUtils.UnpackWorldPosFromNetwork(this.entity.createData);
		base.transform.localScale = localScale;
	}

	// Token: 0x06002F40 RID: 12096 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x06002F41 RID: 12097 RVA: 0x001015D0 File Offset: 0x000FF7D0
	public void OnEntityStateChange(long prevState, long newState)
	{
		int nextHealth = (int)newState;
		this.ChangeHealth(nextHealth);
	}

	// Token: 0x06002F42 RID: 12098 RVA: 0x001015E8 File Offset: 0x000FF7E8
	public void OnImpact(GameHitType hitType)
	{
		if (hitType == GameHitType.Flash)
		{
			int nextHealth = Mathf.Max(this.health - 1, 0);
			this.ChangeHealth(nextHealth);
			if (this.entity.IsAuthority())
			{
				this.entity.RequestState(this.entity.id, (long)this.health);
			}
		}
	}

	// Token: 0x06002F43 RID: 12099 RVA: 0x0010163C File Offset: 0x000FF83C
	private void ChangeHealth(int nextHealth)
	{
		if (this.health != nextHealth)
		{
			this.health = nextHealth;
			if (this.health == 0)
			{
				this.collider.enabled = false;
				this.visualMesh.enabled = false;
				this.audioSource.PlayOneShot(this.onDestroyedClip, this.onDestroyedVolume);
				this.destroyedFx.SetActive(false);
				this.destroyedFx.SetActive(true);
			}
			else
			{
				this.audioSource.PlayOneShot(this.onDamageClip, this.onDamageVolume);
				this.hitFx.SetActive(false);
				this.hitFx.SetActive(true);
			}
			this.RefreshVisuals();
		}
	}

	// Token: 0x06002F44 RID: 12100 RVA: 0x00023994 File Offset: 0x00021B94
	public bool IsHitValid(GameHitData hit)
	{
		return true;
	}

	// Token: 0x06002F45 RID: 12101 RVA: 0x001016E4 File Offset: 0x000FF8E4
	public void OnHit(GameHitData hit)
	{
		GameHitType hitTypeId = (GameHitType)hit.hitTypeId;
		if (this.entity.manager.GetGameComponent<GRTool>(hit.hitByEntityId) != null)
		{
			this.OnImpact(hitTypeId);
		}
	}

	// Token: 0x06002F46 RID: 12102 RVA: 0x00101720 File Offset: 0x000FF920
	public void RefreshVisuals()
	{
		if (this.lastVisualUpdateHealth != this.health)
		{
			this.lastVisualUpdateHealth = this.health;
			Color color = this.visualMesh.material.GetColor("_BaseColor");
			color.a = (float)this.health / (float)this.maxHealth;
			this.visualMesh.material.SetColor("_BaseColor", color);
		}
	}

	// Token: 0x04003C9E RID: 15518
	public GameEntity entity;

	// Token: 0x04003C9F RID: 15519
	public MeshRenderer visualMesh;

	// Token: 0x04003CA0 RID: 15520
	public Collider collider;

	// Token: 0x04003CA1 RID: 15521
	public AudioSource audioSource;

	// Token: 0x04003CA2 RID: 15522
	public AudioClip onDamageClip;

	// Token: 0x04003CA3 RID: 15523
	public float onDamageVolume;

	// Token: 0x04003CA4 RID: 15524
	public AudioClip onDestroyedClip;

	// Token: 0x04003CA5 RID: 15525
	public float onDestroyedVolume;

	// Token: 0x04003CA6 RID: 15526
	[SerializeField]
	private GameObject hitFx;

	// Token: 0x04003CA7 RID: 15527
	[SerializeField]
	private GameObject destroyedFx;

	// Token: 0x04003CA8 RID: 15528
	public int maxHealth = 3;

	// Token: 0x04003CA9 RID: 15529
	[ReadOnly]
	public int health = 3;

	// Token: 0x04003CAA RID: 15530
	private int lastVisualUpdateHealth = -1;
}
