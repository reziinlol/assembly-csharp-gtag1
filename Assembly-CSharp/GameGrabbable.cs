using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006CA RID: 1738
public class GameGrabbable : MonoBehaviour
{
	// Token: 0x06002BD3 RID: 11219 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x06002BD4 RID: 11220 RVA: 0x000ED298 File Offset: 0x000EB498
	public bool GetBestGrabPoint(Vector3 handPos, Quaternion handRot, int handIndex, out GameGrab grab)
	{
		float num = 0.15f;
		bool flag = false;
		grab = default(GameGrab);
		grab.position = base.transform.position;
		grab.rotation = base.transform.rotation;
		bool flag2 = GamePlayer.IsLeftHand(handIndex);
		if (this.snapGrabPoints != null)
		{
			for (int i = 0; i < this.snapGrabPoints.Count; i++)
			{
				GameGrabbable.SnapGrabPoints snapGrabPoints = this.snapGrabPoints[i];
				if (snapGrabPoints.isLeftHand == flag2 && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_UP, handRot * GameGrabbable.GRAB_UP) >= 0f && Vector3.Dot(snapGrabPoints.handTransform.rotation * GameGrabbable.GRAB_PALM, handRot * GameGrabbable.GRAB_PALM) >= 0f && (double)(handPos - snapGrabPoints.handTransform.position).sqrMagnitude <= 0.0225)
				{
					grab.position = handPos + handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation) * -snapGrabPoints.handTransform.localPosition;
					grab.rotation = handRot * Quaternion.Inverse(snapGrabPoints.handTransform.localRotation);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		Vector3 vector = grab.position - handPos;
		if (vector.sqrMagnitude > num * num)
		{
			grab.position = handPos + vector.normalized * num;
		}
		return true;
	}

	// Token: 0x04003838 RID: 14392
	public GameEntity gameEntity;

	// Token: 0x04003839 RID: 14393
	public List<GameGrabbable.SnapGrabPoints> snapGrabPoints;

	// Token: 0x0400383A RID: 14394
	private static readonly Vector3 GRAB_UP = new Vector3(0f, 0f, 1f);

	// Token: 0x0400383B RID: 14395
	private static readonly Vector3 GRAB_PALM = new Vector3(1f, 0f, 0f);

	// Token: 0x020006CB RID: 1739
	[Serializable]
	public class SnapGrabPoints
	{
		// Token: 0x0400383C RID: 14396
		public bool isLeftHand;

		// Token: 0x0400383D RID: 14397
		public Transform handTransform;
	}
}
