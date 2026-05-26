using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x020010F2 RID: 4338
	public class WaterSplashOverride : MonoBehaviour
	{
		// Token: 0x04007E3A RID: 32314
		public bool suppressWaterEffects;

		// Token: 0x04007E3B RID: 32315
		public bool playBigSplash;

		// Token: 0x04007E3C RID: 32316
		public bool playDrippingEffect = true;

		// Token: 0x04007E3D RID: 32317
		public bool scaleByPlayersScale;

		// Token: 0x04007E3E RID: 32318
		public bool overrideBoundingRadius;

		// Token: 0x04007E3F RID: 32319
		public float boundingRadiusOverride = 1f;
	}
}
