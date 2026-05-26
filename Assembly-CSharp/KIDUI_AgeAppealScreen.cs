using System;
using System.Threading;
using UnityEngine;

// Token: 0x02000B77 RID: 2935
public class KIDUI_AgeAppealScreen : MonoBehaviour
{
	// Token: 0x060049E0 RID: 18912 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void Awake()
	{
	}

	// Token: 0x060049E1 RID: 18913 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x060049E2 RID: 18914 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x060049E3 RID: 18915 RVA: 0x0018BA56 File Offset: 0x00189C56
	public void ShowRestrictedAccessScreen()
	{
		base.gameObject.SetActive(true);
	}

	// Token: 0x060049E4 RID: 18916 RVA: 0x000440BC File Offset: 0x000422BC
	public void OnChangeAgePressed()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04005CA4 RID: 23716
	[SerializeField]
	private KIDUIButton _changeAgeButton;

	// Token: 0x04005CA5 RID: 23717
	[SerializeField]
	private int _minimumDelay = 1000;

	// Token: 0x04005CA6 RID: 23718
	private string _submittedEmailAddress;

	// Token: 0x04005CA7 RID: 23719
	private CancellationTokenSource _cancellationTokenSource;
}
