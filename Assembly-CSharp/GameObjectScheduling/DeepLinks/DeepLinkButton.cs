using System;
using System.Collections;
using UnityEngine;

namespace GameObjectScheduling.DeepLinks
{
	// Token: 0x02001332 RID: 4914
	public class DeepLinkButton : GorillaPressableButton
	{
		// Token: 0x06007BB1 RID: 31665 RVA: 0x0028588B File Offset: 0x00283A8B
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			this.sendingDeepLink = DeepLinkSender.SendDeepLink(this.deepLinkAppID, this.deepLinkPayload, new Action<string>(this.OnDeepLinkSent));
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x06007BB2 RID: 31666 RVA: 0x002858C3 File Offset: 0x00283AC3
		private void OnDeepLinkSent(string message)
		{
			this.sendingDeepLink = false;
			if (!this.isOn)
			{
				this.UpdateColor();
			}
		}

		// Token: 0x06007BB3 RID: 31667 RVA: 0x002858DA File Offset: 0x00283ADA
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.pressedTime);
			this.isOn = false;
			if (!this.sendingDeepLink)
			{
				this.UpdateColor();
			}
			yield break;
		}

		// Token: 0x04008D03 RID: 36099
		[SerializeField]
		private ulong deepLinkAppID;

		// Token: 0x04008D04 RID: 36100
		[SerializeField]
		private string deepLinkPayload = "";

		// Token: 0x04008D05 RID: 36101
		[SerializeField]
		private float pressedTime = 0.2f;

		// Token: 0x04008D06 RID: 36102
		private bool sendingDeepLink;
	}
}
