using System;
using UnityEngine;

namespace GorillaLocomotion.Swimming
{
	// Token: 0x020010F1 RID: 4337
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WaterParameters", order = 1)]
	public class WaterParameters : ScriptableObject
	{
		// Token: 0x04007E24 RID: 32292
		[Header("Splash Effect")]
		public bool playSplashEffect = true;

		// Token: 0x04007E25 RID: 32293
		public GameObject splashEffect;

		// Token: 0x04007E26 RID: 32294
		public float splashEffectScale = 1f;

		// Token: 0x04007E27 RID: 32295
		public bool sendSplashEffectRPCs;

		// Token: 0x04007E28 RID: 32296
		public float splashSpeedRequirement = 0.8f;

		// Token: 0x04007E29 RID: 32297
		public float bigSplashSpeedRequirement = 1.9f;

		// Token: 0x04007E2A RID: 32298
		public Gradient splashColorBySpeedGradient;

		// Token: 0x04007E2B RID: 32299
		[Header("Ripple Effect")]
		public bool playRippleEffect = true;

		// Token: 0x04007E2C RID: 32300
		public GameObject rippleEffect;

		// Token: 0x04007E2D RID: 32301
		public float rippleEffectScale = 1f;

		// Token: 0x04007E2E RID: 32302
		public float defaultDistanceBetweenRipples = 0.75f;

		// Token: 0x04007E2F RID: 32303
		public float minDistanceBetweenRipples = 0.2f;

		// Token: 0x04007E30 RID: 32304
		public float minTimeBetweenRipples = 0.75f;

		// Token: 0x04007E31 RID: 32305
		public Color rippleSpriteColor = Color.white;

		// Token: 0x04007E32 RID: 32306
		[Header("Drip Effect")]
		public bool playDripEffect = true;

		// Token: 0x04007E33 RID: 32307
		public float postExitDripDuration = 1.5f;

		// Token: 0x04007E34 RID: 32308
		public float perDripTimeDelay = 0.2f;

		// Token: 0x04007E35 RID: 32309
		public float perDripTimeRandRange = 0.15f;

		// Token: 0x04007E36 RID: 32310
		public float perDripDefaultRadius = 0.01f;

		// Token: 0x04007E37 RID: 32311
		public float perDripRadiusRandRange = 0.01f;

		// Token: 0x04007E38 RID: 32312
		[Header("Misc")]
		public float recomputeSurfaceForColliderDist = 0.2f;

		// Token: 0x04007E39 RID: 32313
		public bool allowBubblesInVolume;
	}
}
