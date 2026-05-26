using System;
using System.Collections.Generic;
using GorillaNetworking;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200081C RID: 2076
public class GRUIEmployeeTerminal : MonoBehaviour
{
	// Token: 0x06003550 RID: 13648 RVA: 0x00126EF8 File Offset: 0x001250F8
	public void Setup()
	{
		this.signupButton.onPressButton.AddListener(new UnityAction(this.OnSignup));
		PlayFab.ClientModels.GetUserDataRequest getUserDataRequest = new PlayFab.ClientModels.GetUserDataRequest();
		getUserDataRequest.PlayFabId = PlayFabAuthenticator.instance.GetPlayFabPlayerId();
		getUserDataRequest.Keys = new List<string>
		{
			"GRData"
		};
		this.isSigningUp = true;
		PlayFabClientAPI.GetUserData(getUserDataRequest, new Action<GetUserDataResult>(this.OnGetUserDataInitialState), new Action<PlayFabError>(this.OnGetUserDataInitialStateFail), null, null);
		this.Refresh();
	}

	// Token: 0x06003551 RID: 13649 RVA: 0x00126F7C File Offset: 0x0012517C
	public void OnSignup()
	{
		if (this.isSigningUp || this.isEmployee)
		{
			return;
		}
		UpdateUserDataRequest request = new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string>
			{
				{
					"GRData",
					"Now we have data"
				}
			}
		};
		if (!PlayFabClientAPI.IsClientLoggedIn())
		{
			if (PlayFabAuthenticator.instance != null)
			{
				PlayFabAuthenticator.instance.AuthenticateWithPlayFab();
			}
			return;
		}
		this.isSigningUp = true;
		PlayFabClientAPI.UpdateUserData(request, new Action<UpdateUserDataResult>(this.OnSaveTableSuccess), new Action<PlayFabError>(this.OnSaveTableFailure), null, null);
	}

	// Token: 0x06003552 RID: 13650 RVA: 0x00127005 File Offset: 0x00125205
	public Transform GetSpawnMarker()
	{
		return this.spawnMarker;
	}

	// Token: 0x06003553 RID: 13651 RVA: 0x00127010 File Offset: 0x00125210
	public void Refresh()
	{
		if (this.isSigningUp)
		{
			this.signupButtonText.text = "APPLYING";
			return;
		}
		if (this.isEmployee)
		{
			this.signupButtonText.text = "HIRED";
			return;
		}
		this.signupButtonText.text = "APPLY";
	}

	// Token: 0x06003554 RID: 13652 RVA: 0x00127060 File Offset: 0x00125260
	private void OnGetUserDataInitialState(GetUserDataResult result)
	{
		UserDataRecord userDataRecord;
		if (result.Data.TryGetValue("GRData", out userDataRecord))
		{
			string value = userDataRecord.Value;
			this.isEmployee = true;
		}
		else
		{
			this.isEmployee = false;
		}
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06003555 RID: 13653 RVA: 0x001270A5 File Offset: 0x001252A5
	private void OnGetUserDataInitialStateFail(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06003556 RID: 13654 RVA: 0x001270BB File Offset: 0x001252BB
	private void OnSaveTableSuccess(UpdateUserDataResult result)
	{
		this.isEmployee = true;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x06003557 RID: 13655 RVA: 0x001270A5 File Offset: 0x001252A5
	private void OnSaveTableFailure(PlayFabError error)
	{
		this.isEmployee = false;
		this.isSigningUp = false;
		this.Refresh();
	}

	// Token: 0x040045DB RID: 17883
	[SerializeField]
	private GorillaPressableButton signupButton;

	// Token: 0x040045DC RID: 17884
	[SerializeField]
	private TMP_Text signupButtonText;

	// Token: 0x040045DD RID: 17885
	[SerializeField]
	private Transform spawnMarker;

	// Token: 0x040045DE RID: 17886
	[SerializeField]
	private GRUIStationEmployeeBadges badgeStation;

	// Token: 0x040045DF RID: 17887
	private int entityTypeId;

	// Token: 0x040045E0 RID: 17888
	private bool isEmployee;

	// Token: 0x040045E1 RID: 17889
	private bool isSigningUp;

	// Token: 0x040045E2 RID: 17890
	private const string GR_DATA_KEY = "GRData";
}
