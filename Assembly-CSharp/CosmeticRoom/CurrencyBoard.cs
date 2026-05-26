using System;
using GorillaExtensions;
using GorillaNetworking;
using TMPro;
using UnityEngine;

namespace CosmeticRoom
{
	// Token: 0x02000FF7 RID: 4087
	public class CurrencyBoard : MonoBehaviour
	{
		// Token: 0x06006636 RID: 26166 RVA: 0x0020F41C File Offset: 0x0020D61C
		public void OnEnable()
		{
			CosmeticsController.instance.AddCurrencyBoard(this);
		}

		// Token: 0x06006637 RID: 26167 RVA: 0x0020F42B File Offset: 0x0020D62B
		public void OnDisable()
		{
			CosmeticsController.instance.RemoveCurrencyBoard(this);
		}

		// Token: 0x06006638 RID: 26168 RVA: 0x0020F43C File Offset: 0x0020D63C
		public void UpdateCurrencyBoard(bool checkedDaily, bool gotDaily, int currencyBalance, int secTilTomorrow)
		{
			if (this.dailyRocksTextTMP.IsNotNull())
			{
				this.dailyRocksTextTMP.text = (checkedDaily ? (gotDaily ? "SUCCESSFULLY GOT DAILY ROCKS!" : "WAITING TO GET DAILY ROCKS...") : "CHECKING DAILY ROCKS...");
			}
			if (this.currencyBoardTextTMP.IsNotNull())
			{
				this.currencyBoardTextTMP.text = string.Concat(new string[]
				{
					currencyBalance.ToString(),
					"\n\n",
					(secTilTomorrow / 3600).ToString(),
					" HR, ",
					(secTilTomorrow % 3600 / 60).ToString(),
					"MIN"
				});
			}
		}

		// Token: 0x040075C2 RID: 30146
		public TMP_Text dailyRocksTextTMP;

		// Token: 0x040075C3 RID: 30147
		public TMP_Text currencyBoardTextTMP;
	}
}
