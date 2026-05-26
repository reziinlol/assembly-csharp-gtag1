using System;
using Liv.Lck.GorillaTag;

namespace Docking
{
	// Token: 0x0200131C RID: 4892
	public class LivCameraDock : Dock
	{
		// Token: 0x06007B4D RID: 31565 RVA: 0x0028434B File Offset: 0x0028254B
		private void Reset()
		{
			this.cameraSettings.fov = 80f;
		}

		// Token: 0x06007B4E RID: 31566 RVA: 0x00284360 File Offset: 0x00282560
		private void OnValidate()
		{
			if (this.cameraSettings.forceFov && (this.cameraSettings.fov < 30f || this.cameraSettings.fov > 110f))
			{
				this.cameraSettings.fov = 80f;
			}
		}

		// Token: 0x04008CB3 RID: 36019
		public GtCameraDockSettings cameraSettings;
	}
}
