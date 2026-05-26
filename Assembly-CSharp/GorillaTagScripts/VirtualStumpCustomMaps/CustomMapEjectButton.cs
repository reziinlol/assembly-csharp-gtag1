using System;
using System.Collections;
using GT_CustomMapSupportRuntime;
using UnityEngine;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F42 RID: 3906
	public class CustomMapEjectButton : GorillaPressableButton
	{
		// Token: 0x0600615F RID: 24927 RVA: 0x001F5F10 File Offset: 0x001F4110
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
			if (!this.processing)
			{
				this.HandleTeleport();
			}
		}

		// Token: 0x06006160 RID: 24928 RVA: 0x001F5F33 File Offset: 0x001F4133
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}

		// Token: 0x06006161 RID: 24929 RVA: 0x001F5F44 File Offset: 0x001F4144
		private void HandleTeleport()
		{
			if (this.processing)
			{
				return;
			}
			this.processing = true;
			CustomMapEjectButton.EjectType ejectType = this.ejectType;
			if (ejectType != CustomMapEjectButton.EjectType.EjectFromVirtualStump)
			{
				if (ejectType == CustomMapEjectButton.EjectType.ReturnToVirtualStump)
				{
					CustomMapManager.ReturnToVirtualStump();
					this.processing = false;
					return;
				}
			}
			else
			{
				CustomMapManager.ExitVirtualStump(new Action<bool>(this.FinishTeleport));
			}
		}

		// Token: 0x06006162 RID: 24930 RVA: 0x001F5F8D File Offset: 0x001F418D
		private void FinishTeleport(bool success = true)
		{
			if (!this.processing)
			{
				return;
			}
			this.processing = false;
		}

		// Token: 0x06006163 RID: 24931 RVA: 0x001F5F9F File Offset: 0x001F419F
		public void CopySettings(CustomMapEjectButtonSettings customMapEjectButtonSettings)
		{
			this.ejectType = (CustomMapEjectButton.EjectType)customMapEjectButtonSettings.ejectType;
		}

		// Token: 0x04007005 RID: 28677
		[SerializeField]
		private CustomMapEjectButton.EjectType ejectType;

		// Token: 0x04007006 RID: 28678
		private bool processing;

		// Token: 0x02000F43 RID: 3907
		public enum EjectType
		{
			// Token: 0x04007008 RID: 28680
			EjectFromVirtualStump,
			// Token: 0x04007009 RID: 28681
			ReturnToVirtualStump
		}
	}
}
