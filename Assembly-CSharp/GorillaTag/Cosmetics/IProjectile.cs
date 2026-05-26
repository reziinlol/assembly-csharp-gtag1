using System;
using UnityEngine;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001290 RID: 4752
	public interface IProjectile
	{
		// Token: 0x060076FB RID: 30459
		void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progressStep = -1);
	}
}
