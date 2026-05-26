using System;
using System.Collections;
using GorillaNetworking;
using Oculus.Platform;
using Oculus.Platform.Models;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x020005AD RID: 1453
public class DeepLinkHandler : MonoBehaviour
{
	// Token: 0x060024AA RID: 9386 RVA: 0x000C4836 File Offset: 0x000C2A36
	public void Awake()
	{
		if (DeepLinkHandler.instance == null)
		{
			DeepLinkHandler.instance = this;
			return;
		}
		if (DeepLinkHandler.instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000C4868 File Offset: 0x000C2A68
	public static void Initialize(GameObject parent)
	{
		if (DeepLinkHandler.instance == null && parent != null)
		{
			parent.AddComponent<DeepLinkHandler>();
		}
		if (DeepLinkHandler.instance == null)
		{
			return;
		}
		DeepLinkHandler.instance.RefreshLaunchDetails();
		if (DeepLinkHandler.instance.cachedLaunchDetails != null && DeepLinkHandler.instance.cachedLaunchDetails.LaunchType == LaunchType.Deeplink)
		{
			DeepLinkHandler.instance.HandleDeepLink();
			return;
		}
		Object.Destroy(DeepLinkHandler.instance);
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000C48EC File Offset: 0x000C2AEC
	private void RefreshLaunchDetails()
	{
		if (UnityEngine.Application.platform != RuntimePlatform.Android)
		{
			GTDev.Log<string>("[DeepLinkHandler::RefreshLaunchDetails] Not on Android Platform!", null);
			return;
		}
		this.cachedLaunchDetails = ApplicationLifecycle.GetLaunchDetails();
		GTDev.Log<string>(string.Concat(new string[]
		{
			"[DeepLinkHandler::RefreshLaunchDetails] LaunchType: ",
			this.cachedLaunchDetails.LaunchType.ToString(),
			"\n[DeepLinkHandler::RefreshLaunchDetails] LaunchSource: ",
			this.cachedLaunchDetails.LaunchSource,
			"\n[DeepLinkHandler::RefreshLaunchDetails] DeepLinkMessage: ",
			this.cachedLaunchDetails.DeeplinkMessage
		}), null);
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000C4977 File Offset: 0x000C2B77
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000C499C File Offset: 0x000C2B9C
	private void HandleDeepLink()
	{
		GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Handling deep link...", null);
		if (this.cachedLaunchDetails.LaunchSource.Contains("7221491444554579"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Witchblood, processing...", null);
			string text = JsonUtility.ToJson(new DeepLinkHandler.CollabRequest
			{
				itemGUID = this.cachedLaunchDetails.DeeplinkMessage,
				launchSource = this.cachedLaunchDetails.LaunchSource,
				oculusUserID = PlayFabAuthenticator.instance.userID,
				playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text, "application/json", new Action<UnityWebRequest>(this.OnWitchbloodCollabResponse)));
			return;
		}
		if (this.cachedLaunchDetails.LaunchSource.Contains("1903584373052985"))
		{
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] DeepLink received from Racoon Lagoon, processing...", null);
			string text2 = JsonUtility.ToJson(new DeepLinkHandler.CollabRequest
			{
				itemGUID = this.cachedLaunchDetails.DeeplinkMessage,
				launchSource = this.cachedLaunchDetails.LaunchSource,
				oculusUserID = PlayFabAuthenticator.instance.userID,
				playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
				playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
				mothershipId = MothershipClientContext.MothershipId,
				mothershipToken = MothershipClientContext.Token,
				mothershipEnvId = MothershipClientApiUnity.EnvironmentId
			});
			GTDev.Log<string>("[DeepLinkHandler::HandleDeepLink] Web Request body: \n" + text2, null);
			base.StartCoroutine(DeepLinkHandler.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeItem", text2, "application/json", new Action<UnityWebRequest>(this.OnRaccoonLagoonCollabResponse)));
			return;
		}
		GTDev.LogError<string>("[DeepLinkHandler::HandleDeepLink] App launched via DeepLink, but from an unknown app. App ID: " + this.cachedLaunchDetails.LaunchSource, null);
		Object.Destroy(this);
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000C4BA8 File Offset: 0x000C2DA8
	private void OnWitchbloodCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnWitchbloodCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.WitchbloodCollabCosmeticID, true, true, true));
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x000C4C38 File Offset: 0x000C2E38
	private void OnRaccoonLagoonCollabResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			GTDev.LogError<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text, null);
			Object.Destroy(this);
			return;
		}
		if (completedRequest.downloadHandler.text.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
		{
			GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item has already been redeemed!", null);
			Object.Destroy(this);
			return;
		}
		GTDev.Log<string>("[DeepLinkHandler::OnRaccoonLagoonCollabResponse] Item successfully granted, processing external unlock...", null);
		base.StartCoroutine(this.CheckProcessExternalUnlock(this.RaccoonLagoonCosmeticIDs, true, true, true));
	}

	// Token: 0x060024B1 RID: 9393 RVA: 0x000C4CC6 File Offset: 0x000C2EC6
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		GTDev.Log<string>("[DeepLinkHandler::CheckProcessExternalUnlock] Cosmetics initialized, proceeding to process external unlock...", null);
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		if (destroyOnFinish)
		{
			Object.Destroy(this);
		}
		yield return null;
		yield break;
	}

	// Token: 0x04003017 RID: 12311
	public static volatile DeepLinkHandler instance;

	// Token: 0x04003018 RID: 12312
	private LaunchDetails cachedLaunchDetails;

	// Token: 0x04003019 RID: 12313
	private const string WitchbloodAppID = "7221491444554579";

	// Token: 0x0400301A RID: 12314
	private readonly string[] WitchbloodCollabCosmeticID = new string[]
	{
		"LMAKT."
	};

	// Token: 0x0400301B RID: 12315
	private const string RaccoonLagoonAppID = "1903584373052985";

	// Token: 0x0400301C RID: 12316
	private readonly string[] RaccoonLagoonCosmeticIDs = new string[]
	{
		"LMALI.",
		"LHAGS."
	};

	// Token: 0x0400301D RID: 12317
	private const string HiddenPathCollabEndpoint = "/api/ConsumeItem";

	// Token: 0x020005AE RID: 1454
	[Serializable]
	private class CollabRequest
	{
		// Token: 0x0400301E RID: 12318
		public string itemGUID;

		// Token: 0x0400301F RID: 12319
		public string launchSource;

		// Token: 0x04003020 RID: 12320
		public string oculusUserID;

		// Token: 0x04003021 RID: 12321
		public string playFabID;

		// Token: 0x04003022 RID: 12322
		public string playFabSessionTicket;

		// Token: 0x04003023 RID: 12323
		public string mothershipId;

		// Token: 0x04003024 RID: 12324
		public string mothershipToken;

		// Token: 0x04003025 RID: 12325
		public string mothershipEnvId;
	}
}
