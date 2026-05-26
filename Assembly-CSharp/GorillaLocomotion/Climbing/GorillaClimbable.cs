using System;
using UnityEngine;

namespace GorillaLocomotion.Climbing
{
	// Token: 0x0200110E RID: 4366
	public class GorillaClimbable : MonoBehaviour
	{
		// Token: 0x06006E02 RID: 28162 RVA: 0x0023FF8F File Offset: 0x0023E18F
		private void Awake()
		{
			this.colliderCache = base.GetComponent<Collider>();
		}

		// Token: 0x04007F11 RID: 32529
		public bool snapX;

		// Token: 0x04007F12 RID: 32530
		public bool snapY;

		// Token: 0x04007F13 RID: 32531
		public bool snapZ;

		// Token: 0x04007F14 RID: 32532
		public float maxDistanceSnap = 0.05f;

		// Token: 0x04007F15 RID: 32533
		public AudioClip clip;

		// Token: 0x04007F16 RID: 32534
		public AudioClip clipOnFullRelease;

		// Token: 0x04007F17 RID: 32535
		public Action<GorillaHandClimber, GorillaClimbableRef> onBeforeClimb;

		// Token: 0x04007F18 RID: 32536
		public bool climbOnlyWhileSmall;

		// Token: 0x04007F19 RID: 32537
		public bool IsPlayerAttached;

		// Token: 0x04007F1A RID: 32538
		[NonSerialized]
		public bool isBeingClimbed;

		// Token: 0x04007F1B RID: 32539
		[NonSerialized]
		public Collider colliderCache;
	}
}
