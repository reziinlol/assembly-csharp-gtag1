using System;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

// Token: 0x02000B34 RID: 2868
public class KIDAgeGateConfirmation : MonoBehaviour
{
	// Token: 0x170006D4 RID: 1748
	// (get) Token: 0x0600489D RID: 18589 RVA: 0x0018428C File Offset: 0x0018248C
	private IntVariable UserAgeVar
	{
		get
		{
			if (this._userAgeVar == null)
			{
				this._userAgeVar = (this._localizedTextBody.StringReference["user-age"] as IntVariable);
				if (this._userAgeVar == null)
				{
					Debug.LogError("[Localization::KID_AGE_GATE_CONFIRMATION] Failed to get [user-age] smart variable as IntVariable");
				}
			}
			return this._userAgeVar;
		}
	}

	// Token: 0x170006D5 RID: 1749
	// (get) Token: 0x0600489E RID: 18590 RVA: 0x001842D9 File Offset: 0x001824D9
	// (set) Token: 0x0600489F RID: 18591 RVA: 0x001842E1 File Offset: 0x001824E1
	public KidAgeConfirmationResult Result { get; private set; }

	// Token: 0x060048A0 RID: 18592 RVA: 0x001842EA File Offset: 0x001824EA
	private void Start()
	{
		this.Result = KidAgeConfirmationResult.None;
	}

	// Token: 0x060048A1 RID: 18593 RVA: 0x001842F3 File Offset: 0x001824F3
	public void OnConfirm()
	{
		this.Result = KidAgeConfirmationResult.Confirm;
	}

	// Token: 0x060048A2 RID: 18594 RVA: 0x001842FC File Offset: 0x001824FC
	public void OnBack()
	{
		this.Result = KidAgeConfirmationResult.Back;
	}

	// Token: 0x060048A3 RID: 18595 RVA: 0x00184305 File Offset: 0x00182505
	public void Reset(int userAge)
	{
		this.Result = KidAgeConfirmationResult.None;
		if (this.UserAgeVar == null)
		{
			Debug.LogError("[LOCALIZATION::KID_AGE_GATE_CONFIRMATION] Unable to update [UserAgeVar] value, as it is null");
			return;
		}
		this.UserAgeVar.Value = userAge;
	}

	// Token: 0x04005B02 RID: 23298
	[Header("Localization")]
	[SerializeField]
	private LocalizedText _localizedTextBody;

	// Token: 0x04005B03 RID: 23299
	private IntVariable _userAgeVar;
}
