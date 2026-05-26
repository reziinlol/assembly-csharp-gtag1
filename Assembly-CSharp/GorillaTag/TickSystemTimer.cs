using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02001173 RID: 4467
	[Serializable]
	internal class TickSystemTimer : TickSystemTimerAbstract
	{
		// Token: 0x06007128 RID: 28968 RVA: 0x0024F3BE File Offset: 0x0024D5BE
		public TickSystemTimer()
		{
		}

		// Token: 0x06007129 RID: 28969 RVA: 0x0024FA5B File Offset: 0x0024DC5B
		public TickSystemTimer(float cd) : base(cd)
		{
		}

		// Token: 0x0600712A RID: 28970 RVA: 0x0024FA64 File Offset: 0x0024DC64
		public TickSystemTimer(float cd, Action cb) : base(cd)
		{
			this.callback = cb;
		}

		// Token: 0x0600712B RID: 28971 RVA: 0x0024FA74 File Offset: 0x0024DC74
		public TickSystemTimer(Action cb)
		{
			this.callback = cb;
		}

		// Token: 0x0600712C RID: 28972 RVA: 0x0024FA83 File Offset: 0x0024DC83
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void OnTimedEvent()
		{
			Action action = this.callback;
			if (action == null)
			{
				return;
			}
			action();
		}

		// Token: 0x04008132 RID: 33074
		public Action callback;
	}
}
