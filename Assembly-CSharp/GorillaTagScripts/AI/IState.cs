using System;

namespace GorillaTagScripts.AI
{
	// Token: 0x02000FEB RID: 4075
	public interface IState
	{
		// Token: 0x060065E9 RID: 26089
		void Tick();

		// Token: 0x060065EA RID: 26090
		void OnEnter();

		// Token: 0x060065EB RID: 26091
		void OnExit();
	}
}
