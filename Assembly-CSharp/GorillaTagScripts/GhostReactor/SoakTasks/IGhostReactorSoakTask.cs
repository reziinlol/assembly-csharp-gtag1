using System;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000F96 RID: 3990
	public interface IGhostReactorSoakTask
	{
		// Token: 0x17000968 RID: 2408
		// (get) Token: 0x06006378 RID: 25464
		bool Complete { get; }

		// Token: 0x06006379 RID: 25465
		bool Update();

		// Token: 0x0600637A RID: 25466
		void Reset();
	}
}
