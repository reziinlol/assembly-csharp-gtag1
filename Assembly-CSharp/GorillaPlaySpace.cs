using System;
using UnityEngine;

// Token: 0x020005CC RID: 1484
public class GorillaPlaySpace : MonoBehaviour
{
	// Token: 0x170003E9 RID: 1001
	// (get) Token: 0x0600252F RID: 9519 RVA: 0x000C5D1C File Offset: 0x000C3F1C
	public static GorillaPlaySpace Instance
	{
		get
		{
			return GorillaPlaySpace._instance;
		}
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x000C5D23 File Offset: 0x000C3F23
	private void Awake()
	{
		if (GorillaPlaySpace._instance != null && GorillaPlaySpace._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GorillaPlaySpace._instance = this;
	}

	// Token: 0x04003084 RID: 12420
	[OnEnterPlay_SetNull]
	private static GorillaPlaySpace _instance;

	// Token: 0x04003085 RID: 12421
	public Collider headCollider;

	// Token: 0x04003086 RID: 12422
	public Collider bodyCollider;

	// Token: 0x04003087 RID: 12423
	public Transform rightHandTransform;

	// Token: 0x04003088 RID: 12424
	public Transform leftHandTransform;

	// Token: 0x04003089 RID: 12425
	public Vector3 headColliderOffset;

	// Token: 0x0400308A RID: 12426
	public Vector3 bodyColliderOffset;

	// Token: 0x0400308B RID: 12427
	private Vector3 lastLeftHandPosition;

	// Token: 0x0400308C RID: 12428
	private Vector3 lastRightHandPosition;

	// Token: 0x0400308D RID: 12429
	private Vector3 lastLeftHandPositionForTag;

	// Token: 0x0400308E RID: 12430
	private Vector3 lastRightHandPositionForTag;

	// Token: 0x0400308F RID: 12431
	private Vector3 lastBodyPositionForTag;

	// Token: 0x04003090 RID: 12432
	private Vector3 lastHeadPositionForTag;

	// Token: 0x04003091 RID: 12433
	private Rigidbody playspaceRigidbody;

	// Token: 0x04003092 RID: 12434
	public Transform headsetTransform;

	// Token: 0x04003093 RID: 12435
	public Vector3 rightHandOffset;

	// Token: 0x04003094 RID: 12436
	public Vector3 leftHandOffset;

	// Token: 0x04003095 RID: 12437
	public VRRig vrRig;

	// Token: 0x04003096 RID: 12438
	public VRRig offlineVRRig;

	// Token: 0x04003097 RID: 12439
	public float vibrationCooldown = 0.1f;

	// Token: 0x04003098 RID: 12440
	public float vibrationDuration = 0.05f;

	// Token: 0x04003099 RID: 12441
	private float leftLastTouchedSurface;

	// Token: 0x0400309A RID: 12442
	private float rightLastTouchedSurface;

	// Token: 0x0400309B RID: 12443
	public VRRig myVRRig;

	// Token: 0x0400309C RID: 12444
	private float bodyHeight;

	// Token: 0x0400309D RID: 12445
	public float tagCooldown;

	// Token: 0x0400309E RID: 12446
	public float taggedTime;

	// Token: 0x0400309F RID: 12447
	public float disconnectTime = 60f;

	// Token: 0x040030A0 RID: 12448
	public float maxStepVelocity = 2f;

	// Token: 0x040030A1 RID: 12449
	public float hapticWaitSeconds = 0.05f;

	// Token: 0x040030A2 RID: 12450
	public float tapHapticDuration = 0.05f;

	// Token: 0x040030A3 RID: 12451
	public float tapHapticStrength = 0.5f;

	// Token: 0x040030A4 RID: 12452
	public float tagHapticDuration = 0.15f;

	// Token: 0x040030A5 RID: 12453
	public float tagHapticStrength = 1f;

	// Token: 0x040030A6 RID: 12454
	public float taggedHapticDuration = 0.35f;

	// Token: 0x040030A7 RID: 12455
	public float taggedHapticStrength = 1f;
}
