using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020006E9 RID: 1769
public class GameSnappable : MonoBehaviour
{
	// Token: 0x06002C9E RID: 11422 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x06002C9F RID: 11423 RVA: 0x000F1598 File Offset: 0x000EF798
	public void GetSnapOffset(SnapJointType jointType, out Vector3 positionOffset, out Quaternion rotationOffset)
	{
		foreach (GameSnappable.SnapJointOffset snapJointOffset in this.snapOffsets)
		{
			if ((snapJointOffset.jointType & jointType) != SnapJointType.None)
			{
				positionOffset = snapJointOffset.positionOffset;
				rotationOffset = Quaternion.Euler(snapJointOffset.rotationOffset);
				return;
			}
		}
		positionOffset = Vector3.zero;
		rotationOffset = Quaternion.identity;
	}

	// Token: 0x06002CA0 RID: 11424 RVA: 0x000F1624 File Offset: 0x000EF824
	public SuperInfectionSnapPoint BestSnapPoint()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return null;
		}
		bool flag = GamePlayer.IsLeftHand(heldByHandIndex);
		SnapJointType snapJointType = flag ? SnapJointType.HandL : SnapJointType.HandR;
		SnapJointType snapJointType2 = flag ? SnapJointType.ForearmL : SnapJointType.ForearmR;
		List<SuperInfectionSnapPoint> snapPoints = GamePlayerLocal.instance.gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2 && (snapPoints[i].jointType & this.snapLocationTypes) != SnapJointType.None && !snapPoints[i].HasSnapped())
			{
				Vector3 point;
				Quaternion rotation;
				this.GetSnapOffset(snapPoints[i].jointType, out point, out rotation);
				float num3 = Vector3.Distance(snapPoints[i].transform.TransformPoint(rotation * point), base.transform.position);
				float num4 = this.snapRadius + snapPoints[i].snapPointRadius;
				if (num3 < num && num3 < num4)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		if (num2 >= 0)
		{
			return snapPoints[num2];
		}
		if ((this.snapLocationTypes & SnapJointType.Holster) != SnapJointType.None)
		{
			GameEntityManager currGameEntityManager = GamePlayerLocal.instance.currGameEntityManager;
			IEnumerable<SuperInfectionSnapPoint> points = ((currGameEntityManager != null) ? currGameEntityManager.superInfectionManager : null).GetPoints(SnapJointType.Holster);
			SuperInfectionSnapPoint superInfectionSnapPoint = null;
			float num5 = this.snapRadius;
			foreach (SuperInfectionSnapPoint superInfectionSnapPoint2 in points)
			{
				if (!superInfectionSnapPoint2.HasSnapped())
				{
					Vector3 point2;
					Quaternion rotation2;
					this.GetSnapOffset(superInfectionSnapPoint2.jointType, out point2, out rotation2);
					float num6 = Vector3.Distance(superInfectionSnapPoint2.transform.TransformPoint(rotation2 * point2), base.transform.position);
					if (num6 < num5)
					{
						superInfectionSnapPoint = superInfectionSnapPoint2;
						num5 = num6;
					}
				}
			}
			if (superInfectionSnapPoint != null)
			{
				return superInfectionSnapPoint;
			}
		}
		return null;
	}

	// Token: 0x06002CA1 RID: 11425 RVA: 0x000F1834 File Offset: 0x000EFA34
	public GameEntityId BestSnapPointDock()
	{
		int heldByHandIndex = this.gameEntity.heldByHandIndex;
		if (heldByHandIndex < 0)
		{
			return GameEntityId.Invalid;
		}
		SnapJointType snapJointType = GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.HandL : SnapJointType.HandR;
		SnapJointType snapJointType2 = GamePlayer.IsLeftHand(heldByHandIndex) ? SnapJointType.ForearmL : SnapJointType.ForearmR;
		List<SuperInfectionSnapPoint> snapPoints = GamePlayerLocal.instance.gamePlayer.snapPointManager.SnapPoints;
		float num = float.MaxValue;
		int num2 = -1;
		for (int i = 0; i < snapPoints.Count; i++)
		{
			if (snapPoints[i].jointType != snapJointType && snapPoints[i].jointType != snapJointType2 && (snapPoints[i].jointType & this.snapLocationTypes) != SnapJointType.None && snapPoints[i].HasSnapped())
			{
				Vector3 point;
				Quaternion rotation;
				this.GetSnapOffset(snapPoints[i].jointType, out point, out rotation);
				float num3 = Vector3.Distance(snapPoints[i].transform.TransformPoint(rotation * point), base.transform.position);
				float num4 = this.snapRadius + snapPoints[i].snapPointRadius;
				if (num3 < num && num3 < num4)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		if (num2 < 0)
		{
			return GameEntityId.Invalid;
		}
		return snapPoints[num2].GetSnappedEntity().id;
	}

	// Token: 0x06002CA2 RID: 11426 RVA: 0x000F198C File Offset: 0x000EFB8C
	public bool CanGrabWithHand(bool leftHand)
	{
		if (this.snappedToJoint == null)
		{
			return true;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return (leftHand && jointType != SnapJointType.HandL && jointType != SnapJointType.ForearmL) || (!leftHand && jointType != SnapJointType.HandR && jointType != SnapJointType.ForearmR);
	}

	// Token: 0x06002CA3 RID: 11427 RVA: 0x000F19DA File Offset: 0x000EFBDA
	public void OnSnap()
	{
		this.snapSound.Play(null);
		this.snapHaptic.PlayIfSnappedLocal(this.gameEntity);
	}

	// Token: 0x06002CA4 RID: 11428 RVA: 0x000F19FC File Offset: 0x000EFBFC
	public bool IsSnappedToLeftArm()
	{
		if (this.snappedToJoint == null)
		{
			return false;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return jointType == SnapJointType.HandL || jointType == SnapJointType.ForearmL;
	}

	// Token: 0x06002CA5 RID: 11429 RVA: 0x000F1A34 File Offset: 0x000EFC34
	public bool IsSnappedToRightArm()
	{
		if (this.snappedToJoint == null)
		{
			return false;
		}
		SnapJointType jointType = this.snappedToJoint.jointType;
		return jointType == SnapJointType.HandR || jointType == SnapJointType.ForearmR;
	}

	// Token: 0x06002CA6 RID: 11430 RVA: 0x000F1A6B File Offset: 0x000EFC6B
	public void OnUnsnap()
	{
		this.unsnapSound.Play(null);
	}

	// Token: 0x06002CA7 RID: 11431 RVA: 0x000F1A79 File Offset: 0x000EFC79
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool TryGetJointToSnapIndex(SnapJointType jointType, out int out_slot)
	{
		out_slot = GameSnappable.GetJointToSnapIndex(jointType);
		return out_slot != -1;
	}

	// Token: 0x06002CA8 RID: 11432 RVA: 0x000F1A8C File Offset: 0x000EFC8C
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int GetJointToSnapIndex(SnapJointType jointType)
	{
		int result;
		if (jointType != SnapJointType.HandL)
		{
			if (jointType != SnapJointType.HandR)
			{
				result = -1;
			}
			else
			{
				result = 3;
			}
		}
		else
		{
			result = 2;
		}
		return result;
	}

	// Token: 0x06002CA9 RID: 11433 RVA: 0x000F1AB0 File Offset: 0x000EFCB0
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SnapJointType GetSnapIndexToJoint(int snapIndex)
	{
		SnapJointType result;
		if (snapIndex != 2)
		{
			if (snapIndex != 3)
			{
				result = SnapJointType.None;
			}
			else
			{
				result = SnapJointType.HandR;
			}
		}
		else
		{
			result = SnapJointType.HandL;
		}
		return result;
	}

	// Token: 0x0400390B RID: 14603
	public GameEntity gameEntity;

	// Token: 0x0400390C RID: 14604
	public float snapRadius = 0.15f;

	// Token: 0x0400390D RID: 14605
	public SuperInfectionSnapPoint snappedToJoint;

	// Token: 0x0400390E RID: 14606
	public AbilitySound snapSound;

	// Token: 0x0400390F RID: 14607
	public AbilitySound unsnapSound;

	// Token: 0x04003910 RID: 14608
	public AbilityHaptic snapHaptic;

	// Token: 0x04003911 RID: 14609
	public SnapJointType snapLocationTypes;

	// Token: 0x04003912 RID: 14610
	public List<GameSnappable.SnapJointOffset> snapOffsets;

	// Token: 0x020006EA RID: 1770
	[Serializable]
	public struct SnapJointOffset
	{
		// Token: 0x04003913 RID: 14611
		public SnapJointType jointType;

		// Token: 0x04003914 RID: 14612
		public Vector3 positionOffset;

		// Token: 0x04003915 RID: 14613
		public Vector3 rotationOffset;
	}
}
