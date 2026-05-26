using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BuildSafe
{
	// Token: 0x0200100F RID: 4111
	public abstract class SceneBakeTask : MonoBehaviour
	{
		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x0600669B RID: 26267 RVA: 0x00210507 File Offset: 0x0020E707
		// (set) Token: 0x0600669C RID: 26268 RVA: 0x0021050F File Offset: 0x0020E70F
		public SceneBakeMode bakeMode
		{
			get
			{
				return this.m_bakeMode;
			}
			set
			{
				this.m_bakeMode = value;
			}
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x0600669D RID: 26269 RVA: 0x00210518 File Offset: 0x0020E718
		// (set) Token: 0x0600669E RID: 26270 RVA: 0x00210520 File Offset: 0x0020E720
		public virtual int callbackOrder
		{
			get
			{
				return this.m_callbackOrder;
			}
			set
			{
				this.m_callbackOrder = value;
			}
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x0600669F RID: 26271 RVA: 0x00210529 File Offset: 0x0020E729
		// (set) Token: 0x060066A0 RID: 26272 RVA: 0x00210531 File Offset: 0x0020E731
		public bool runIfInactive
		{
			get
			{
				return this.m_runIfInactive;
			}
			set
			{
				this.m_runIfInactive = value;
			}
		}

		// Token: 0x060066A1 RID: 26273
		[Conditional("UNITY_EDITOR")]
		public abstract void OnSceneBake(Scene scene, SceneBakeMode mode);

		// Token: 0x060066A2 RID: 26274 RVA: 0x000028C5 File Offset: 0x00000AC5
		[Conditional("UNITY_EDITOR")]
		private void ForceRun()
		{
		}

		// Token: 0x04007605 RID: 30213
		[SerializeField]
		private SceneBakeMode m_bakeMode;

		// Token: 0x04007606 RID: 30214
		[SerializeField]
		private int m_callbackOrder;

		// Token: 0x04007607 RID: 30215
		[Space]
		[SerializeField]
		private bool m_runIfInactive = true;
	}
}
