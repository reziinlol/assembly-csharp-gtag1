using System;
using System.Collections.Generic;
using GorillaTag;
using UnityEngine;

// Token: 0x020006EE RID: 1774
public class SuperInfectionSnapPointManager : MonoBehaviour
{
	// Token: 0x06002CBB RID: 11451 RVA: 0x000F1DF4 File Offset: 0x000EFFF4
	public void Awake()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		ISpawnable[] componentsInChildren = base.GetComponentsInChildren<ISpawnable>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnSpawn(componentInParent);
		}
	}

	// Token: 0x06002CBC RID: 11452 RVA: 0x000F1E28 File Offset: 0x000F0028
	public void Start()
	{
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.SnapPoints)
		{
			superInfectionSnapPoint.Initialize();
			this.snapPointDict[superInfectionSnapPoint.jointType] = superInfectionSnapPoint;
		}
	}

	// Token: 0x06002CBD RID: 11453 RVA: 0x000F1E8C File Offset: 0x000F008C
	public void Clear()
	{
		foreach (SuperInfectionSnapPoint superInfectionSnapPoint in this.SnapPoints)
		{
			superInfectionSnapPoint.Clear();
		}
		this.snapPointDict.Clear();
	}

	// Token: 0x06002CBE RID: 11454 RVA: 0x000F1EE8 File Offset: 0x000F00E8
	public SuperInfectionSnapPoint FindSnapPoint(SnapJointType jointType)
	{
		if (jointType == SnapJointType.None)
		{
			return null;
		}
		if (this.snapPointDict.ContainsKey(jointType))
		{
			return this.snapPointDict[jointType];
		}
		return null;
	}

	// Token: 0x06002CBF RID: 11455 RVA: 0x000F1F0B File Offset: 0x000F010B
	public static SuperInfectionSnapPoint FindSnapPoint(GamePlayer player, SnapJointType jointType)
	{
		if (player == null)
		{
			return null;
		}
		return player.snapPointManager.FindSnapPoint(jointType);
	}

	// Token: 0x06002CC0 RID: 11456 RVA: 0x000F1F24 File Offset: 0x000F0124
	public void DropAllSnappedAuthority()
	{
		for (int i = 0; i < this.SnapPoints.Count; i++)
		{
			GameEntity snappedEntity = this.SnapPoints[i].GetSnappedEntity();
			if (!(snappedEntity == null))
			{
				Vector3 position = snappedEntity.transform.position;
				snappedEntity.manager.RequestGrabEntity(snappedEntity.id, false, Vector3.zero, Quaternion.identity);
				snappedEntity.manager.RequestThrowEntity(snappedEntity.id, false, position, Vector3.zero, Vector3.zero);
			}
		}
	}

	// Token: 0x0400392B RID: 14635
	public List<SuperInfectionSnapPoint> SnapPoints;

	// Token: 0x0400392C RID: 14636
	private Dictionary<SnapJointType, SuperInfectionSnapPoint> snapPointDict = new Dictionary<SnapJointType, SuperInfectionSnapPoint>();
}
