using System;
using UnityEngine;

// Token: 0x02000BB0 RID: 2992
public class KIDUI_RestrictedAccessScreen : MonoBehaviour
{
	// Token: 0x06004B2A RID: 19242 RVA: 0x00191EF8 File Offset: 0x001900F8
	public void ShowRestrictedAccessScreen(SessionStatus? sessionStatus)
	{
		base.gameObject.SetActive(true);
		this._pendingStatusIndicator.SetActive(false);
		this._prohibitedStatusIndicator.SetActive(false);
		if (sessionStatus == null)
		{
			return;
		}
		if (sessionStatus != null)
		{
			switch (sessionStatus.GetValueOrDefault())
			{
			case SessionStatus.PASS:
			case SessionStatus.CHALLENGE:
			case SessionStatus.CHALLENGE_SESSION_UPGRADE:
				break;
			case SessionStatus.PROHIBITED:
				this._prohibitedStatusIndicator.SetActive(true);
				return;
			case SessionStatus.PENDING_AGE_APPEAL:
				this._pendingStatusIndicator.SetActive(true);
				break;
			default:
				return;
			}
		}
	}

	// Token: 0x06004B2B RID: 19243 RVA: 0x00191F78 File Offset: 0x00190178
	public void OnChangeAgePressed()
	{
		PrivateUIRoom.RemoveUI(base.transform);
		base.gameObject.SetActive(false);
		this._ageAppealScreen.ShowAgeAppealScreen();
	}

	// Token: 0x06004B2C RID: 19244 RVA: 0x00189D14 File Offset: 0x00187F14
	public void OnDisable()
	{
		KIDAudioManager instance = KIDAudioManager.Instance;
		if (instance == null)
		{
			return;
		}
		instance.PlaySoundWithDelay(KIDAudioManager.KIDSoundType.PageTransition);
	}

	// Token: 0x04005E20 RID: 24096
	[SerializeField]
	private KIDAgeAppeal _ageAppealScreen;

	// Token: 0x04005E21 RID: 24097
	[SerializeField]
	private GameObject _pendingStatusIndicator;

	// Token: 0x04005E22 RID: 24098
	[SerializeField]
	private GameObject _prohibitedStatusIndicator;
}
