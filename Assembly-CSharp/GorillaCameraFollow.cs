using System;
using GorillaLocomotion;
using Unity.Cinemachine;
using UnityEngine;

// Token: 0x020005C6 RID: 1478
public class GorillaCameraFollow : MonoBehaviour
{
	// Token: 0x0600251C RID: 9500 RVA: 0x000C5A6C File Offset: 0x000C3C6C
	private void Start()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			this.cameraParent.SetActive(false);
		}
		if (this.cinemachineCamera != null)
		{
			this.cinemachineFollow = this.cinemachineCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
			this.baseCameraRadius = this.cinemachineFollow.CameraRadius;
			this.baseFollowDistance = this.cinemachineFollow.CameraDistance;
			this.baseVerticalArmLength = this.cinemachineFollow.VerticalArmLength;
			this.baseShoulderOffset = this.cinemachineFollow.ShoulderOffset;
		}
	}

	// Token: 0x0600251D RID: 9501 RVA: 0x000C5AF4 File Offset: 0x000C3CF4
	private void LateUpdate()
	{
		if (this.cinemachineFollow != null)
		{
			float scale = GTPlayer.Instance.scale;
			this.cinemachineFollow.CameraRadius = this.baseCameraRadius * scale;
			this.cinemachineFollow.CameraDistance = this.baseFollowDistance * scale;
			this.cinemachineFollow.VerticalArmLength = this.baseVerticalArmLength * scale;
			this.cinemachineFollow.ShoulderOffset = this.baseShoulderOffset * scale;
		}
	}

	// Token: 0x0400306D RID: 12397
	public Transform playerHead;

	// Token: 0x0400306E RID: 12398
	public GameObject cameraParent;

	// Token: 0x0400306F RID: 12399
	public Vector3 headOffset;

	// Token: 0x04003070 RID: 12400
	public Vector3 eulerRotationOffset;

	// Token: 0x04003071 RID: 12401
	public CinemachineVirtualCamera cinemachineCamera;

	// Token: 0x04003072 RID: 12402
	private Cinemachine3rdPersonFollow cinemachineFollow;

	// Token: 0x04003073 RID: 12403
	private float baseCameraRadius = 0.2f;

	// Token: 0x04003074 RID: 12404
	private float baseFollowDistance = 2f;

	// Token: 0x04003075 RID: 12405
	private float baseVerticalArmLength = 0.4f;

	// Token: 0x04003076 RID: 12406
	private Vector3 baseShoulderOffset = new Vector3(0.5f, -0.4f, 0f);
}
