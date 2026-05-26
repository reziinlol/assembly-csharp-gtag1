using System;
using UnityEngine;

namespace GorillaTagScripts.Builder
{
	// Token: 0x02000FE1 RID: 4065
	public class SharedBlocksScreen : MonoBehaviour
	{
		// Token: 0x06006587 RID: 25991 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void OnUpPressed()
		{
		}

		// Token: 0x06006588 RID: 25992 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void OnDownPressed()
		{
		}

		// Token: 0x06006589 RID: 25993 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void OnSelectPressed()
		{
		}

		// Token: 0x0600658A RID: 25994 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void OnDeletePressed()
		{
		}

		// Token: 0x0600658B RID: 25995 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void OnNumberPressed(int number)
		{
		}

		// Token: 0x0600658C RID: 25996 RVA: 0x000028C5 File Offset: 0x00000AC5
		public virtual void OnLetterPressed(string letter)
		{
		}

		// Token: 0x0600658D RID: 25997 RVA: 0x0020BC5E File Offset: 0x00209E5E
		public virtual void Show()
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}

		// Token: 0x0600658E RID: 25998 RVA: 0x0020BC79 File Offset: 0x00209E79
		public virtual void Hide()
		{
			if (base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x040074CB RID: 29899
		public SharedBlocksTerminal.ScreenType screenType;

		// Token: 0x040074CC RID: 29900
		public SharedBlocksTerminal terminal;
	}
}
