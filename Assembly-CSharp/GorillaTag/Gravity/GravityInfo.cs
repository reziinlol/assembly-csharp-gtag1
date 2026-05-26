using System;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200118C RID: 4492
	[Serializable]
	public struct GravityInfo
	{
		// Token: 0x04008183 RID: 33155
		public Vector3 gravityUpDirection;

		// Token: 0x04008184 RID: 33156
		public Vector3 rotationDirection;

		// Token: 0x04008185 RID: 33157
		public float rotationSpeed;

		// Token: 0x04008186 RID: 33158
		public float gravityStrength;

		// Token: 0x04008187 RID: 33159
		public bool rotate;
	}
}
