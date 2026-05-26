using System;
using UnityEngine;

namespace GorillaLocomotion.Gameplay
{
	// Token: 0x02001101 RID: 4353
	internal interface IGorillaGrabable
	{
		// Token: 0x17000A96 RID: 2710
		// (get) Token: 0x06006D9F RID: 28063
		string name { get; }

		// Token: 0x06006DA0 RID: 28064
		bool MomentaryGrabOnly();

		// Token: 0x06006DA1 RID: 28065
		bool CanBeGrabbed(GorillaGrabber grabber);

		// Token: 0x06006DA2 RID: 28066
		void OnGrabbed(GorillaGrabber grabber, out Transform grabbedTransform, out Vector3 localGrabbedPosition);

		// Token: 0x06006DA3 RID: 28067
		void OnGrabReleased(GorillaGrabber grabber);
	}
}
