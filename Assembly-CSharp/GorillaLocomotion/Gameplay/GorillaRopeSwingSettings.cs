using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x020010FD RID: 4349
	[CreateAssetMenu(fileName = "GorillaRopeSwingSettings", menuName = "ScriptableObjects/GorillaRopeSwingSettings", order = 0)]
	public class GorillaRopeSwingSettings : ScriptableObject
	{
		// Token: 0x04007E90 RID: 32400
		public float inheritVelocityMultiplier = 1f;

		// Token: 0x04007E91 RID: 32401
		public float frictionWhenNotHeld = 0.25f;
	}
}
