using System;
using JetBrains.Annotations;
using UnityEngine;

// Token: 0x020004CF RID: 1231
public class WorldTargetItem
{
	// Token: 0x06001DE0 RID: 7648 RVA: 0x000A0F6F File Offset: 0x0009F16F
	public bool IsValid()
	{
		return this.itemIdx != -1 && this.owner != null;
	}

	// Token: 0x06001DE1 RID: 7649 RVA: 0x000A0F88 File Offset: 0x0009F188
	[CanBeNull]
	public static WorldTargetItem GenerateTargetFromPlayerAndID(NetPlayer owner, int itemIdx)
	{
		VRRig vrrig = GorillaGameManager.StaticFindRigForPlayer(owner);
		if (vrrig == null)
		{
			Debug.LogError("Tried to setup a sharable object but the target rig is null...");
			return null;
		}
		Transform component = vrrig.myBodyDockPositions.TransferrableItem(itemIdx).gameObject.GetComponent<Transform>();
		return new WorldTargetItem(owner, itemIdx, component);
	}

	// Token: 0x06001DE2 RID: 7650 RVA: 0x000A0FD0 File Offset: 0x0009F1D0
	public static WorldTargetItem GenerateTargetFromWorldSharableItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		return new WorldTargetItem(owner, itemIdx, transform);
	}

	// Token: 0x06001DE3 RID: 7651 RVA: 0x000A0FDA File Offset: 0x0009F1DA
	private WorldTargetItem(NetPlayer owner, int itemIdx, Transform transform)
	{
		this.owner = owner;
		this.itemIdx = itemIdx;
		this.targetObject = transform;
		this.transferrableObject = transform.GetComponent<TransferrableObject>();
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x000A1003 File Offset: 0x0009F203
	public override string ToString()
	{
		return string.Format("Id: {0} ({1})", this.itemIdx, this.owner);
	}

	// Token: 0x04002836 RID: 10294
	public readonly NetPlayer owner;

	// Token: 0x04002837 RID: 10295
	public readonly int itemIdx;

	// Token: 0x04002838 RID: 10296
	public readonly Transform targetObject;

	// Token: 0x04002839 RID: 10297
	public readonly TransferrableObject transferrableObject;
}
