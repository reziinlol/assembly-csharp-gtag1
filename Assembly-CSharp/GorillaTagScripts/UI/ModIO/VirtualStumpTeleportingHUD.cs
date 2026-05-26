using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.UI.ModIO
{
	// Token: 0x02000F62 RID: 3938
	public class VirtualStumpTeleportingHUD : MonoBehaviour
	{
		// Token: 0x0600621D RID: 25117 RVA: 0x001FAC24 File Offset: 0x001F8E24
		public void Initialize(bool isEntering)
		{
			this.isEnteringVirtualStump = isEntering;
			if (isEntering)
			{
				string text;
				if (!LocalisationManager.TryGetKeyForCurrentLocale("VIRT_STUMP_HUD_ENTERING", out text, this.enteringVirtualStumpString))
				{
					Debug.LogError("[LOCALIZATION::VIRT_STUMP_TELEPORT_HUD] Failed to retrieve key [VIRT_STUMP_HUD_ENTERING] for locale [" + LocalisationManager.CurrentLanguage.LocaleName + "]");
				}
				this.teleportingStatusText.text = text;
				this.teleportingStatusText.gameObject.SetActive(true);
				return;
			}
			string text2;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("VIRT_STUMP_HUD_LEAVING", out text2, this.leavingVirtualStumpString))
			{
				Debug.LogError("[LOCALIZATION::VIRT_STUMP_TELEPORT_HUD] Failed to retrieve key [VIRT_STUMP_HUD_LEAVING] for locale [" + LocalisationManager.CurrentLanguage.LocaleName + "]");
			}
			this.teleportingStatusText.text = text2;
			this.teleportingStatusText.gameObject.SetActive(true);
		}

		// Token: 0x0600621E RID: 25118 RVA: 0x001FACDC File Offset: 0x001F8EDC
		private void Update()
		{
			if (Time.time - this.lastTextUpdateTime > this.textUpdateInterval)
			{
				this.lastTextUpdateTime = Time.time;
				this.IncrementProgressDots();
				this.teleportingStatusText.text = (this.isEnteringVirtualStump ? this.enteringVirtualStumpString : this.leavingVirtualStumpString);
				for (int i = 0; i < this.numProgressDots; i++)
				{
					TMP_Text tmp_Text = this.teleportingStatusText;
					tmp_Text.text += ".";
				}
			}
		}

		// Token: 0x0600621F RID: 25119 RVA: 0x001FAD5B File Offset: 0x001F8F5B
		private void IncrementProgressDots()
		{
			this.numProgressDots++;
			if (this.numProgressDots > this.maxNumProgressDots)
			{
				this.numProgressDots = 0;
			}
		}

		// Token: 0x040070EA RID: 28906
		private const string VIRT_STUMP_HUD_ENTERING_KEY = "VIRT_STUMP_HUD_ENTERING";

		// Token: 0x040070EB RID: 28907
		private const string VIRT_STUMP_HUD_LEAVING_KEY = "VIRT_STUMP_HUD_LEAVING";

		// Token: 0x040070EC RID: 28908
		[SerializeField]
		private string enteringVirtualStumpString = "Now Entering the Virtual Stump";

		// Token: 0x040070ED RID: 28909
		[SerializeField]
		private string leavingVirtualStumpString = "Now Leaving the Virtual Stump";

		// Token: 0x040070EE RID: 28910
		[SerializeField]
		private TMP_Text teleportingStatusText;

		// Token: 0x040070EF RID: 28911
		[SerializeField]
		private int maxNumProgressDots = 3;

		// Token: 0x040070F0 RID: 28912
		[SerializeField]
		private float textUpdateInterval = 0.5f;

		// Token: 0x040070F1 RID: 28913
		private float lastTextUpdateTime;

		// Token: 0x040070F2 RID: 28914
		private int numProgressDots;

		// Token: 0x040070F3 RID: 28915
		private bool isEnteringVirtualStump;
	}
}
