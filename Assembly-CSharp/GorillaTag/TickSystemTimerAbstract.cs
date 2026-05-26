using System;
using System.Runtime.CompilerServices;

namespace GorillaTag
{
	// Token: 0x02001172 RID: 4466
	[Serializable]
	internal abstract class TickSystemTimerAbstract : CoolDownHelper, ITickSystemPre
	{
		// Token: 0x17000AD3 RID: 2771
		// (get) Token: 0x0600711E RID: 28958 RVA: 0x0024FA0C File Offset: 0x0024DC0C
		// (set) Token: 0x0600711F RID: 28959 RVA: 0x0024FA14 File Offset: 0x0024DC14
		bool ITickSystemPre.PreTickRunning
		{
			get
			{
				return this.registered;
			}
			set
			{
				this.registered = value;
			}
		}

		// Token: 0x17000AD4 RID: 2772
		// (get) Token: 0x06007120 RID: 28960 RVA: 0x0024FA0C File Offset: 0x0024DC0C
		public bool Running
		{
			get
			{
				return this.registered;
			}
		}

		// Token: 0x06007121 RID: 28961 RVA: 0x0024FA1D File Offset: 0x0024DC1D
		protected TickSystemTimerAbstract()
		{
		}

		// Token: 0x06007122 RID: 28962 RVA: 0x0024FA25 File Offset: 0x0024DC25
		protected TickSystemTimerAbstract(float cd) : base(cd)
		{
		}

		// Token: 0x06007123 RID: 28963 RVA: 0x0024FA2E File Offset: 0x0024DC2E
		public override void Start()
		{
			base.Start();
			TickSystem<object>.AddPreTickCallback(this);
		}

		// Token: 0x06007124 RID: 28964 RVA: 0x0024FA3C File Offset: 0x0024DC3C
		public override void Stop()
		{
			base.Stop();
			TickSystem<object>.RemovePreTickCallback(this);
		}

		// Token: 0x06007125 RID: 28965 RVA: 0x0024FA4A File Offset: 0x0024DC4A
		public override void OnCheckPass()
		{
			this.OnTimedEvent();
		}

		// Token: 0x06007126 RID: 28966
		public abstract void OnTimedEvent();

		// Token: 0x06007127 RID: 28967 RVA: 0x0024FA52 File Offset: 0x0024DC52
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		void ITickSystemPre.PreTick()
		{
			base.CheckCooldown();
		}

		// Token: 0x04008131 RID: 33073
		[NonSerialized]
		internal bool registered;
	}
}
