using System;
using UnityEngine;
using UnityEngine.Events;

namespace GorillaTagScripts
{
	// Token: 0x02000ECA RID: 3786
	public class AttachPoint : MonoBehaviour
	{
		// Token: 0x06005D32 RID: 23858 RVA: 0x001D8DFD File Offset: 0x001D6FFD
		private void Start()
		{
			base.transform.parent.parent = null;
		}

		// Token: 0x06005D33 RID: 23859 RVA: 0x001D8E10 File Offset: 0x001D7010
		private void OnTriggerEnter(Collider other)
		{
			if (this.attachPoint.childCount == 0)
			{
				this.UpdateHookState(false);
			}
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || componentInParent.InHand())
			{
				return;
			}
			if (this.IsHooked())
			{
				return;
			}
			this.UpdateHookState(true);
			componentInParent.SnapItem(true, this.attachPoint.position);
		}

		// Token: 0x06005D34 RID: 23860 RVA: 0x001D8E6C File Offset: 0x001D706C
		private void OnTriggerExit(Collider other)
		{
			DecorativeItem componentInParent = other.GetComponentInParent<DecorativeItem>();
			if (componentInParent == null || !componentInParent.InHand())
			{
				return;
			}
			this.UpdateHookState(false);
			componentInParent.SnapItem(false, Vector3.zero);
		}

		// Token: 0x06005D35 RID: 23861 RVA: 0x001D8EA5 File Offset: 0x001D70A5
		private void UpdateHookState(bool isHooked)
		{
			this.SetIsHook(isHooked);
		}

		// Token: 0x06005D36 RID: 23862 RVA: 0x001D8EAE File Offset: 0x001D70AE
		internal void SetIsHook(bool isHooked)
		{
			this.isHooked = isHooked;
			UnityAction unityAction = this.onHookedChanged;
			if (unityAction == null)
			{
				return;
			}
			unityAction();
		}

		// Token: 0x06005D37 RID: 23863 RVA: 0x001D8EC7 File Offset: 0x001D70C7
		public bool IsHooked()
		{
			return this.isHooked || this.attachPoint.childCount != 0;
		}

		// Token: 0x04006BC7 RID: 27591
		public Transform attachPoint;

		// Token: 0x04006BC8 RID: 27592
		public UnityAction onHookedChanged;

		// Token: 0x04006BC9 RID: 27593
		private bool isHooked;

		// Token: 0x04006BCA RID: 27594
		private bool wasHooked;

		// Token: 0x04006BCB RID: 27595
		public bool inForest;
	}
}
