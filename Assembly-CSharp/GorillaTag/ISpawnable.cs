using System;
using GorillaTag.CosmeticSystem;

namespace GorillaTag
{
	// Token: 0x02001138 RID: 4408
	public interface ISpawnable
	{
		// Token: 0x17000AB3 RID: 2739
		// (get) Token: 0x06006FF2 RID: 28658
		// (set) Token: 0x06006FF3 RID: 28659
		bool IsSpawned { get; set; }

		// Token: 0x17000AB4 RID: 2740
		// (get) Token: 0x06006FF4 RID: 28660
		// (set) Token: 0x06006FF5 RID: 28661
		ECosmeticSelectSide CosmeticSelectedSide { get; set; }

		// Token: 0x06006FF6 RID: 28662
		void OnSpawn(VRRig rig);

		// Token: 0x06006FF7 RID: 28663
		void OnDespawn();
	}
}
