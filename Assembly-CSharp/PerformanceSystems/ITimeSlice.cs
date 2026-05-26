using System;

namespace PerformanceSystems
{
	// Token: 0x02000EA5 RID: 3749
	public interface ITimeSlice
	{
		// Token: 0x06005C10 RID: 23568
		void SliceUpdate();

		// Token: 0x06005C11 RID: 23569
		void SliceUpdateAlways(float deltaTime);

		// Token: 0x06005C12 RID: 23570
		void SliceUpdate(float deltaTime);
	}
}
