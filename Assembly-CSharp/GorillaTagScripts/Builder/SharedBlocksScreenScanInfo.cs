using System;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FE2 RID: 4066
	public class SharedBlocksScreenScanInfo : SharedBlocksScreen
	{
		// Token: 0x06006590 RID: 26000 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void OnUpPressed()
		{
		}

		// Token: 0x06006591 RID: 26001 RVA: 0x000028C5 File Offset: 0x00000AC5
		public override void OnDownPressed()
		{
		}

		// Token: 0x06006592 RID: 26002 RVA: 0x0020BC94 File Offset: 0x00209E94
		public override void OnSelectPressed()
		{
			this.terminal.OnLoadMapPressed();
		}

		// Token: 0x06006593 RID: 26003 RVA: 0x0020BCA1 File Offset: 0x00209EA1
		public override void Show()
		{
			base.Show();
			this.DrawScreen();
		}

		// Token: 0x06006594 RID: 26004 RVA: 0x0020BCB0 File Offset: 0x00209EB0
		private void DrawScreen()
		{
			if (this.terminal.SelectedMap == null)
			{
				this.mapIDText.text = "MAP ID: NONE";
				return;
			}
			this.mapIDText.text = "MAP ID: " + SharedBlocksTerminal.MapIDToDisplayedString(this.terminal.SelectedMap.MapID);
		}

		// Token: 0x040074CD RID: 29901
		[SerializeField]
		private TMP_Text mapIDText;
	}
}
