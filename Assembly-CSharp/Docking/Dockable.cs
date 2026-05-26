using System;
using UnityEngine;

namespace Docking
{
	// Token: 0x0200131B RID: 4891
	public class Dockable : MonoBehaviour
	{
		// Token: 0x06007B49 RID: 31561 RVA: 0x002842BC File Offset: 0x002824BC
		protected virtual void OnTriggerEnter(Collider other)
		{
			Dock dock;
			if (other.TryGetComponent<Dock>(out dock))
			{
				this.potentialDock = other.transform;
			}
		}

		// Token: 0x06007B4A RID: 31562 RVA: 0x002842DF File Offset: 0x002824DF
		protected virtual void OnTriggerExit(Collider other)
		{
			if (this.potentialDock == other.transform)
			{
				this.potentialDock = null;
			}
		}

		// Token: 0x06007B4B RID: 31563 RVA: 0x002842FC File Offset: 0x002824FC
		public virtual void Dock()
		{
			if (this.potentialDock == null)
			{
				return;
			}
			base.transform.position = this.potentialDock.position;
			base.transform.rotation = this.potentialDock.rotation;
			this.potentialDock = null;
		}

		// Token: 0x04008CB2 RID: 36018
		protected Transform potentialDock;
	}
}
