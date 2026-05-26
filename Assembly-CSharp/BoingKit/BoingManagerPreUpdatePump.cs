using System;
using UnityEngine;

namespace BoingKit
{
	// Token: 0x0200136F RID: 4975
	public class BoingManagerPreUpdatePump : MonoBehaviour
	{
		// Token: 0x06007D49 RID: 32073 RVA: 0x00290C0B File Offset: 0x0028EE0B
		private void FixedUpdate()
		{
			this.TryPump();
		}

		// Token: 0x06007D4A RID: 32074 RVA: 0x00290C0B File Offset: 0x0028EE0B
		private void Update()
		{
			this.TryPump();
		}

		// Token: 0x06007D4B RID: 32075 RVA: 0x00290C13 File Offset: 0x0028EE13
		private void TryPump()
		{
			if (this.m_lastPumpedFrame >= Time.frameCount)
			{
				return;
			}
			if (this.m_lastPumpedFrame >= 0)
			{
				this.DoPump();
			}
			this.m_lastPumpedFrame = Time.frameCount;
		}

		// Token: 0x06007D4C RID: 32076 RVA: 0x00290C3D File Offset: 0x0028EE3D
		private void DoPump()
		{
			BoingManager.RestoreBehaviors();
			BoingManager.RestoreReactors();
			BoingManager.RestoreBones();
			BoingManager.DispatchReactorFieldCompute();
		}

		// Token: 0x04008E79 RID: 36473
		private int m_lastPumpedFrame = -1;
	}
}
