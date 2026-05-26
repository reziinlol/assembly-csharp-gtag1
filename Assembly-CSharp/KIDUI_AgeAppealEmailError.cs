using System;
using TMPro;
using UnityEngine;

// Token: 0x02000B92 RID: 2962
public class KIDUI_AgeAppealEmailError : MonoBehaviour
{
	// Token: 0x06004A8E RID: 19086 RVA: 0x0018E7B6 File Offset: 0x0018C9B6
	public void ShowAgeAppealEmailErrorScreen(bool hasChallenge, int newAge, string email)
	{
		this.hasChallenge = hasChallenge;
		this.newAge = newAge;
		this._emailText.text = email;
		base.gameObject.SetActive(true);
	}

	// Token: 0x06004A8F RID: 19087 RVA: 0x0018E7DE File Offset: 0x0018C9DE
	public void onBackPressed()
	{
		base.gameObject.SetActive(false);
		this._ageAppealEmailScreen.ShowAgeAppealEmailScreen(this.hasChallenge, this.newAge);
	}

	// Token: 0x04005D52 RID: 23890
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04005D53 RID: 23891
	[SerializeField]
	private TMP_Text _emailText;

	// Token: 0x04005D54 RID: 23892
	private bool hasChallenge;

	// Token: 0x04005D55 RID: 23893
	private int newAge;
}
