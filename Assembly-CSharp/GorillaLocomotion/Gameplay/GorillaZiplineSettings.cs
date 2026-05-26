using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001100 RID: 4352
	[CreateAssetMenu(fileName = "GorillaZiplineSettings", menuName = "ScriptableObjects/GorillaZiplineSettings", order = 0)]
	public class GorillaZiplineSettings : ScriptableObject
	{
		// Token: 0x04007EA2 RID: 32418
		public float minSlidePitch = 0.5f;

		// Token: 0x04007EA3 RID: 32419
		public float maxSlidePitch = 1f;

		// Token: 0x04007EA4 RID: 32420
		public float minSlideVolume;

		// Token: 0x04007EA5 RID: 32421
		public float maxSlideVolume = 0.2f;

		// Token: 0x04007EA6 RID: 32422
		public float maxSpeed = 10f;

		// Token: 0x04007EA7 RID: 32423
		public float gravityMulti = 1.1f;

		// Token: 0x04007EA8 RID: 32424
		[Header("Friction")]
		public float friction = 0.25f;

		// Token: 0x04007EA9 RID: 32425
		public float maxFriction = 1f;

		// Token: 0x04007EAA RID: 32426
		public float maxFrictionSpeed = 15f;
	}
}
