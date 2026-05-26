using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200114C RID: 4428
	[DefaultExecutionOrder(2000)]
	public class StaticLodGroup : MonoBehaviour, IGorillaSimpleBackgroundWorker
	{
		// Token: 0x0600703C RID: 28732 RVA: 0x00249795 File Offset: 0x00247995
		protected void OnEnable()
		{
			if (this.initialized)
			{
				StaticLodManager.SetEnabled(this.index, true);
				return;
			}
			GorillaSimpleBackgroundWorkerManager.WorkerSignup(this);
		}

		// Token: 0x0600703D RID: 28733 RVA: 0x002497B2 File Offset: 0x002479B2
		protected void OnDisable()
		{
			if (this.initialized)
			{
				StaticLodManager.SetEnabled(this.index, false);
			}
		}

		// Token: 0x0600703E RID: 28734 RVA: 0x002497C8 File Offset: 0x002479C8
		private void OnDestroy()
		{
			if (this.initialized)
			{
				StaticLodManager.Unregister(this.index);
			}
		}

		// Token: 0x0600703F RID: 28735 RVA: 0x002497DD File Offset: 0x002479DD
		public void SimpleWork()
		{
			if (this.initialized)
			{
				return;
			}
			this.index = StaticLodManager.Register(this);
			StaticLodManager.SetEnabled(this.index, true);
			this.initialized = true;
		}

		// Token: 0x04008016 RID: 32790
		public const int k_monoDefaultExecutionOrder = 2000;

		// Token: 0x04008017 RID: 32791
		private int index;

		// Token: 0x04008018 RID: 32792
		public float collisionEnableDistance = 3f;

		// Token: 0x04008019 RID: 32793
		public float uiFadeDistanceMax = 10f;

		// Token: 0x0400801A RID: 32794
		private bool initialized;
	}
}
