using System;
using GorillaExtensions;
using GT_CustomMapSupportRuntime;
using UnityEngine;

// Token: 0x02000A56 RID: 2646
public class CustomMapsGrabbablesController : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060043EA RID: 17386 RVA: 0x0016BCD4 File Offset: 0x00169ED4
	private void Awake()
	{
		this.isGrabbed = false;
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Combine(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Combine(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x060043EB RID: 17387 RVA: 0x0016BD38 File Offset: 0x00169F38
	private void OnDestroy()
	{
		GameEntity gameEntity = this.entity;
		gameEntity.OnGrabbed = (Action)Delegate.Remove(gameEntity.OnGrabbed, new Action(this.OnGrabbed));
		GameEntity gameEntity2 = this.entity;
		gameEntity2.OnReleased = (Action)Delegate.Remove(gameEntity2.OnReleased, new Action(this.OnReleased));
	}

	// Token: 0x060043EC RID: 17388 RVA: 0x0016BD94 File Offset: 0x00169F94
	public void OnEntityInit()
	{
		GTDev.Log<string>("CustomMapsGrabbablesController::OnEntityInit", null);
		if (MapSpawnManager.instance == null)
		{
			return;
		}
		base.transform.parent = MapSpawnManager.instance.transform;
		byte enemyTypeIndex;
		GrabbableEntity.UnpackCreateData(this.entity.createData, out enemyTypeIndex, out this.luaAgentID);
		MapEntity mapEntity;
		if (!MapSpawnManager.instance.SpawnEntity((int)enemyTypeIndex, out mapEntity))
		{
			GTDev.LogError<string>("CustomMapsGrabbablesController::OnEntityInit could not spawn grabbable", null);
			Object.Destroy(base.gameObject);
			return;
		}
		GrabbableEntity grabbableEntity = (GrabbableEntity)mapEntity;
		if (grabbableEntity == null)
		{
			return;
		}
		grabbableEntity.gameObject.SetActive(true);
		grabbableEntity.transform.parent = this.entity.transform;
		grabbableEntity.transform.localPosition = Vector3.zero;
		grabbableEntity.transform.localRotation = Quaternion.identity;
		this.returnParent = this.entity.transform.parent;
		this.entity.audioSource = grabbableEntity.audioSource;
		this.entity.catchSound = grabbableEntity.catchSound;
		this.entity.catchSoundVolume = grabbableEntity.catchSoundVolume;
		this.entity.throwSound = grabbableEntity.throwSound;
		this.entity.throwSoundVolume = grabbableEntity.throwSoundVolume;
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("Prop");
		}
	}

	// Token: 0x060043ED RID: 17389 RVA: 0x0016BF01 File Offset: 0x0016A101
	public int GetGrabbingActor()
	{
		if (!this.isGrabbed)
		{
			return -1;
		}
		return this.entity.heldByActorNumber;
	}

	// Token: 0x060043EE RID: 17390 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060043EF RID: 17391 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x060043F0 RID: 17392 RVA: 0x0016BF18 File Offset: 0x0016A118
	private void OnGrabbed()
	{
		this.isGrabbed = true;
	}

	// Token: 0x060043F1 RID: 17393 RVA: 0x0016BF21 File Offset: 0x0016A121
	private void OnReleased()
	{
		this.isGrabbed = false;
		if (this.returnParent.IsNotNull())
		{
			this.entity.transform.parent = this.returnParent;
		}
	}

	// Token: 0x040055CA RID: 21962
	public GameEntity entity;

	// Token: 0x040055CB RID: 21963
	public short luaAgentID;

	// Token: 0x040055CC RID: 21964
	private bool isGrabbed;

	// Token: 0x040055CD RID: 21965
	private Transform returnParent;
}
