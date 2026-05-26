using System;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

// Token: 0x02000B28 RID: 2856
public class KIDAgeAppeal : MonoBehaviour
{
	// Token: 0x06004870 RID: 18544 RVA: 0x001831B0 File Offset: 0x001813B0
	public void ShowAgeAppealScreen()
	{
		this._ageSlider = base.GetComponentInChildren<AgeSliderWithProgressBar>(true);
		this._ageSlider.ControllerActive = true;
		base.gameObject.SetActive(true);
		this._inputsContainer.SetActive(true);
		this._monkeLoader.SetActive(false);
	}

	// Token: 0x06004871 RID: 18545 RVA: 0x001831F0 File Offset: 0x001813F0
	public void OnNewAgeConfirmed()
	{
		KIDAgeAppeal.<OnNewAgeConfirmed>d__6 <OnNewAgeConfirmed>d__;
		<OnNewAgeConfirmed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnNewAgeConfirmed>d__.<>4__this = this;
		<OnNewAgeConfirmed>d__.<>1__state = -1;
		<OnNewAgeConfirmed>d__.<>t__builder.Start<KIDAgeAppeal.<OnNewAgeConfirmed>d__6>(ref <OnNewAgeConfirmed>d__);
	}

	// Token: 0x04005ABE RID: 23230
	[SerializeField]
	private TMP_Text _ageText;

	// Token: 0x04005ABF RID: 23231
	[SerializeField]
	private KIDUI_AgeAppealEmailScreen _ageAppealEmailScreen;

	// Token: 0x04005AC0 RID: 23232
	[SerializeField]
	private GameObject _inputsContainer;

	// Token: 0x04005AC1 RID: 23233
	[SerializeField]
	private GameObject _monkeLoader;

	// Token: 0x04005AC2 RID: 23234
	private AgeSliderWithProgressBar _ageSlider;
}
