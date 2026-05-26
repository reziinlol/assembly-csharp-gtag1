using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006AA RID: 1706
public class GameDockable : MonoBehaviour
{
	// Token: 0x06002A8E RID: 10894 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x06002A8F RID: 10895 RVA: 0x000E4740 File Offset: 0x000E2940
	public GameEntityId BestDock()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return GameEntityId.Invalid;
		}
		SnapJointType snapJointType = GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.HandL : SnapJointType.HandR;
		SnapJointType snapJointType2 = GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.ForearmL : SnapJointType.ForearmR;
		GamePlayer gamePlayer = GamePlayerLocal.instance.gamePlayer;
		List<SuperInfectionSnapPoint> snapPoints = gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		GameDock gameDock = null;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2)
			{
				GameEntity snappedEntity = snapPoints[i].GetSnappedEntity();
				if (!(snappedEntity == null))
				{
					GameDock component = snappedEntity.GetComponent<GameDock>();
					if (!(component == null) && component.CanDock(this))
					{
						Transform transform = component.dockMarker.transform;
						Vector3 zero = Vector3.zero;
						Quaternion identity = Quaternion.identity;
						float num2 = Vector3.Distance(transform.TransformPoint(identity * zero), base.transform.position);
						float num3 = this.dockableRadius + component.dockRadius;
						if (num2 < num && num2 < num3)
						{
							num = num2;
							gameDock = component;
						}
					}
				}
			}
		}
		for (int j = 0; j < 2; j++)
		{
			GameEntity grabbedGameEntity = gamePlayer.GetGrabbedGameEntity(j);
			if (!(grabbedGameEntity == null))
			{
				GameDock component2 = grabbedGameEntity.GetComponent<GameDock>();
				if (!(component2 == null) && component2.CanDock(this))
				{
					Transform transform2 = component2.dockMarker.transform;
					Vector3 zero2 = Vector3.zero;
					Quaternion identity2 = Quaternion.identity;
					float num2 = Vector3.Distance(transform2.TransformPoint(identity2 * zero2), base.transform.position);
					float num4 = this.dockableRadius + component2.dockRadius;
					if (num2 < num && num2 < num4)
					{
						num = num2;
						gameDock = component2;
					}
				}
			}
		}
		if (gameDock == null)
		{
			return GameEntityId.Invalid;
		}
		return gameDock.gameEntity.id;
	}

	// Token: 0x06002A90 RID: 10896 RVA: 0x000E4943 File Offset: 0x000E2B43
	public Transform GetDockablePoint()
	{
		if (!(this.dockablePoint == null))
		{
			return this.dockablePoint;
		}
		return base.transform;
	}

	// Token: 0x06002A91 RID: 10897 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
	}

	// Token: 0x06002A92 RID: 10898 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnUndock(GameEntity gameEntity, GameEntity attachedToGameEntity)
	{
	}

	// Token: 0x04003744 RID: 14148
	public GameEntity gameEntity;

	// Token: 0x04003745 RID: 14149
	public float dockableRadius = 0.15f;

	// Token: 0x04003746 RID: 14150
	public Transform dockablePoint;
}
