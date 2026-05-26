using System;
using Liv.Lck.GorillaTag;
using UnityEngine;

namespace Docking
{
	// Token: 0x0200131D RID: 4893
	public class LivCameraDockable : Dockable
	{
		// Token: 0x06007B50 RID: 31568 RVA: 0x002843B8 File Offset: 0x002825B8
		protected override void OnTriggerEnter(Collider other)
		{
			LivCameraDock livCameraDock;
			if (other.TryGetComponent<LivCameraDock>(out livCameraDock))
			{
				this.livDock = livCameraDock;
				this.potentialDock = other.transform;
			}
		}

		// Token: 0x06007B51 RID: 31569 RVA: 0x002843E2 File Offset: 0x002825E2
		protected override void OnTriggerExit(Collider other)
		{
			if (this.livDock != null && other.transform == this.potentialDock.transform)
			{
				this.potentialDock = null;
				this.livDock = null;
			}
		}

		// Token: 0x06007B52 RID: 31570 RVA: 0x00284418 File Offset: 0x00282618
		public override void Dock()
		{
			base.Dock();
			if (this.livDock == null)
			{
				return;
			}
			GTLckController gtlckController = base.GetComponent<GTLckController>() ?? base.GetComponentInParent<GTLckController>();
			if (gtlckController != null)
			{
				gtlckController.ApplyCameraSettings(this.livDock.cameraSettings);
			}
			this.livDock = null;
		}

		// Token: 0x04008CB4 RID: 36020
		private LivCameraDock livDock;
	}
}
