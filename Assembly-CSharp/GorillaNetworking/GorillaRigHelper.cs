using System;

namespace GorillaNetworking
{
	// Token: 0x02001041 RID: 4161
	[Serializable]
	internal struct GorillaRigHelper : IComparable
	{
		// Token: 0x06006817 RID: 26647 RVA: 0x0021965A File Offset: 0x0021785A
		public int CompareTo(object obj)
		{
			return this.sqrDistance.CompareTo(((GorillaRigHelper)obj).sqrDistance);
		}

		// Token: 0x04007767 RID: 30567
		public VRRig rig;

		// Token: 0x04007768 RID: 30568
		public CosmeticsThrottler.RigDrawState state;

		// Token: 0x04007769 RID: 30569
		public float sqrDistance;

		// Token: 0x0400776A RID: 30570
		public float prevSqrDistance;
	}
}
