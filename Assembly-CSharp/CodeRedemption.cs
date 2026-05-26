using System;
using System.Collections;
using GorillaNetworking;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x0200050E RID: 1294
public class CodeRedemption : MonoBehaviour
{
	// Token: 0x06002050 RID: 8272 RVA: 0x000AD74E File Offset: 0x000AB94E
	public void Awake()
	{
		if (CodeRedemption.Instance == null)
		{
			CodeRedemption.Instance = this;
			return;
		}
		if (CodeRedemption.Instance != this)
		{
			Object.Destroy(this);
		}
	}

	// Token: 0x06002051 RID: 8273 RVA: 0x000AD780 File Offset: 0x000AB980
	public void HandleCodeRedemption(string code)
	{
		string text = JsonConvert.SerializeObject(new CodeRedemption.CodeRedemptionRequest
		{
			itemGUID = code,
			playFabID = PlayFabAuthenticator.instance.GetPlayFabPlayerId(),
			playFabSessionTicket = PlayFabAuthenticator.instance.GetPlayFabSessionTicket(),
			mothershipId = MothershipClientContext.MothershipId,
			mothershipToken = MothershipClientContext.Token,
			mothershipEnvId = MothershipClientApiUnity.EnvironmentId
		});
		Debug.Log("[CodeRedemption] Web Request body: \n" + text);
		base.StartCoroutine(CodeRedemption.ProcessWebRequest(PlayFabAuthenticatorSettings.HpPromoApiBaseUrl + "/api/ConsumeCodeItem", text, "application/json", new Action<UnityWebRequest>(this.OnCodeRedemptionResponse)));
	}

	// Token: 0x06002052 RID: 8274 RVA: 0x000AD824 File Offset: 0x000ABA24
	private void OnCodeRedemptionResponse(UnityWebRequest completedRequest)
	{
		if (completedRequest.result != UnityWebRequest.Result.Success)
		{
			Debug.LogError("[CodeRedemption] Web Request failed: " + completedRequest.error + "\nDetails: " + completedRequest.downloadHandler.text);
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		string text = string.Empty;
		try
		{
			CodeRedemption.CodeRedemptionResponse codeRedemptionResponse = JsonConvert.DeserializeObject<CodeRedemption.CodeRedemptionResponse>(completedRequest.downloadHandler.text);
			if (codeRedemptionResponse.result.Contains("AlreadyRedeemed", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log("[CodeRedemption] Code has already been redeemed!");
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.AlreadyUsed;
				return;
			}
			if (codeRedemptionResponse.result.Contains("TooEarly", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log(string.Format("[CodeRedemption] Code is not redeemable until {0}!", codeRedemptionResponse.startTime));
				GorillaComputer.instance.RedemptionRestrictionTime = codeRedemptionResponse.startTime;
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.TooEarly;
				return;
			}
			if (codeRedemptionResponse.result.Contains("TooLate", StringComparison.OrdinalIgnoreCase))
			{
				Debug.Log(string.Format("[CodeRedemption] Code expired at {0}!", codeRedemptionResponse.endTime));
				GorillaComputer.instance.RedemptionRestrictionTime = codeRedemptionResponse.endTime;
				GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.TooLate;
				return;
			}
			text = codeRedemptionResponse.playFabItemName;
		}
		catch (Exception ex)
		{
			string str = "[CodeRedemption] Error parsing JSON response: ";
			Exception ex2 = ex;
			Debug.LogError(str + ((ex2 != null) ? ex2.ToString() : null));
			GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Invalid;
			return;
		}
		Debug.Log("[CodeRedemption] Item successfully granted, processing external unlock...");
		GorillaComputer.instance.RedemptionStatus = GorillaComputer.RedemptionResult.Success;
		GorillaComputer.instance.RedemptionCode = "";
		base.StartCoroutine(this.CheckProcessExternalUnlock(new string[]
		{
			text
		}, true, true, true));
	}

	// Token: 0x06002053 RID: 8275 RVA: 0x000AD9DC File Offset: 0x000ABBDC
	private IEnumerator CheckProcessExternalUnlock(string[] itemIDs, bool autoEquip, bool isLeftHand, bool destroyOnFinish)
	{
		Debug.Log("[CodeRedemption] Checking if we can process external cosmetic unlock...");
		while (!CosmeticsController.instance.allCosmeticsDict_isInitialized)
		{
			yield return null;
		}
		Debug.Log("[CodeRedemption] Cosmetics initialized, proceeding to process external unlock...");
		foreach (string itemID in itemIDs)
		{
			CosmeticsController.instance.ProcessExternalUnlock(itemID, autoEquip, isLeftHand);
		}
		yield break;
	}

	// Token: 0x06002054 RID: 8276 RVA: 0x000AD9F9 File Offset: 0x000ABBF9
	private static IEnumerator ProcessWebRequest(string url, string data, string contentType, Action<UnityWebRequest> callback)
	{
		UnityWebRequest request = UnityWebRequest.Post(url, data, contentType);
		yield return request.SendWebRequest();
		callback(request);
		yield break;
	}

	// Token: 0x04002B18 RID: 11032
	public static volatile CodeRedemption Instance;

	// Token: 0x04002B19 RID: 11033
	private const string HiddenPathCollabEndpoint = "/api/ConsumeCodeItem";

	// Token: 0x0200050F RID: 1295
	[Serializable]
	private class CodeRedemptionRequest
	{
		// Token: 0x04002B1A RID: 11034
		public string itemGUID;

		// Token: 0x04002B1B RID: 11035
		public string playFabID;

		// Token: 0x04002B1C RID: 11036
		public string playFabSessionTicket;

		// Token: 0x04002B1D RID: 11037
		public string mothershipId;

		// Token: 0x04002B1E RID: 11038
		public string mothershipToken;

		// Token: 0x04002B1F RID: 11039
		public string mothershipEnvId;
	}

	// Token: 0x02000510 RID: 1296
	[Serializable]
	private class CodeRedemptionResponse
	{
		// Token: 0x04002B20 RID: 11040
		public string result;

		// Token: 0x04002B21 RID: 11041
		public string itemID;

		// Token: 0x04002B22 RID: 11042
		public string playFabItemName;

		// Token: 0x04002B23 RID: 11043
		public DateTimeOffset? startTime;

		// Token: 0x04002B24 RID: 11044
		public DateTimeOffset? endTime;
	}
}
