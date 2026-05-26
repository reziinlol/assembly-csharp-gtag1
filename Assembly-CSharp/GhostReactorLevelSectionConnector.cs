using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000702 RID: 1794
public class GhostReactorLevelSectionConnector : MonoBehaviour
{
	// Token: 0x06002D27 RID: 11559 RVA: 0x000F5BB0 File Offset: 0x000F3DB0
	private void Awake()
	{
		this.prePlacedGameEntities = new List<GameEntity>(128);
		base.GetComponentsInChildren<GameEntity>(this.prePlacedGameEntities);
		for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
		{
			this.prePlacedGameEntities[i].gameObject.SetActive(false);
		}
		this.renderers = new List<Renderer>(512);
		this.hidden = false;
		base.GetComponentsInChildren<Renderer>(this.renderers);
		if (this.boundingCollider == null)
		{
			Debug.LogWarningFormat("Missing Bounding Collider for section {0}", new object[]
			{
				base.gameObject.name
			});
		}
	}

	// Token: 0x06002D28 RID: 11560 RVA: 0x000F5C58 File Offset: 0x000F3E58
	public void Init(GhostReactorManager grManager)
	{
		if (grManager.IsAuthority())
		{
			if (this.gateEntity != null)
			{
				grManager.gameEntityManager.RequestCreateItem(this.gateEntity.name.GetStaticHash(), this.gateSpawnPoint.position, this.gateSpawnPoint.rotation, 0L);
			}
			for (int i = 0; i < this.prePlacedGameEntities.Count; i++)
			{
				if (!this.prePlacedGameEntities[i].isBuiltIn)
				{
					int staticHash = this.prePlacedGameEntities[i].gameObject.name.GetStaticHash();
					if (!grManager.gameEntityManager.FactoryHasEntity(staticHash))
					{
						Debug.LogErrorFormat("Cannot Find Entity in Factory {0} {1}", new object[]
						{
							this.prePlacedGameEntities[i].gameObject.name,
							staticHash
						});
					}
					else
					{
						GameEntityCreateData item = new GameEntityCreateData
						{
							entityTypeId = staticHash,
							position = this.prePlacedGameEntities[i].transform.position,
							rotation = this.prePlacedGameEntities[i].transform.rotation,
							createData = 0L,
							createdByEntityId = -1,
							slotIndex = -1
						};
						GhostReactorLevelSection.tempCreateEntitiesList.Add(item);
					}
				}
			}
			grManager.gameEntityManager.RequestCreateItems(GhostReactorLevelSection.tempCreateEntitiesList);
			GhostReactorLevelSection.tempCreateEntitiesList.Clear();
		}
	}

	// Token: 0x06002D29 RID: 11561 RVA: 0x000F5DCC File Offset: 0x000F3FCC
	public void Hide(bool hide)
	{
		for (int i = 0; i < this.renderers.Count; i++)
		{
			if (!(this.renderers[i] == null))
			{
				this.renderers[i].enabled = !hide;
			}
		}
	}

	// Token: 0x06002D2A RID: 11562 RVA: 0x000F5E18 File Offset: 0x000F4018
	public void UpdateDisable(Vector3 playerPos)
	{
		if (this.boundingCollider == null)
		{
			return;
		}
		float sqrMagnitude = (this.boundingCollider.ClosestPoint(playerPos) - playerPos).sqrMagnitude;
		float num = 324f;
		float num2 = 484f;
		if (this.hidden && sqrMagnitude < num)
		{
			this.hidden = false;
			this.Hide(false);
			return;
		}
		if (!this.hidden && sqrMagnitude > num2)
		{
			this.hidden = true;
			this.Hide(true);
		}
	}

	// Token: 0x040039E9 RID: 14825
	public Transform hubAnchor;

	// Token: 0x040039EA RID: 14826
	public Transform sectionAnchor;

	// Token: 0x040039EB RID: 14827
	public Transform gateSpawnPoint;

	// Token: 0x040039EC RID: 14828
	public GameEntity gateEntity;

	// Token: 0x040039ED RID: 14829
	public GhostReactorLevelSectionConnector.Direction direction;

	// Token: 0x040039EE RID: 14830
	public BoxCollider boundingCollider;

	// Token: 0x040039EF RID: 14831
	public List<Transform> pathNodes;

	// Token: 0x040039F0 RID: 14832
	private const float SHOW_DIST = 18f;

	// Token: 0x040039F1 RID: 14833
	private const float HIDE_DIST = 22f;

	// Token: 0x040039F2 RID: 14834
	private List<GameEntity> prePlacedGameEntities;

	// Token: 0x040039F3 RID: 14835
	private List<Renderer> renderers;

	// Token: 0x040039F4 RID: 14836
	private bool hidden;

	// Token: 0x02000703 RID: 1795
	public enum Direction
	{
		// Token: 0x040039F6 RID: 14838
		Down = -1,
		// Token: 0x040039F7 RID: 14839
		Forward,
		// Token: 0x040039F8 RID: 14840
		Up
	}
}
