using System;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ECC RID: 3788
	[RequireComponent(typeof(Collider))]
	public class MovingSurface : MonoBehaviour
	{
		// Token: 0x06005D49 RID: 23881 RVA: 0x001D9123 File Offset: 0x001D7323
		private void Start()
		{
			MovingSurfaceManager.instance == null;
			MovingSurfaceManager.instance.RegisterMovingSurface(this);
		}

		// Token: 0x06005D4A RID: 23882 RVA: 0x001D913C File Offset: 0x001D733C
		private void OnDestroy()
		{
			if (MovingSurfaceManager.instance != null)
			{
				MovingSurfaceManager.instance.UnregisterMovingSurface(this);
			}
		}

		// Token: 0x06005D4B RID: 23883 RVA: 0x001D9156 File Offset: 0x001D7356
		public int GetID()
		{
			return this.uniqueId;
		}

		// Token: 0x06005D4C RID: 23884 RVA: 0x001D915E File Offset: 0x001D735E
		public void CopySettings(MovingSurfaceSettings movingSurfaceSettings)
		{
			this.uniqueId = movingSurfaceSettings.uniqueId;
		}

		// Token: 0x04006BD3 RID: 27603
		[SerializeField]
		private int uniqueId = -1;
	}
}
