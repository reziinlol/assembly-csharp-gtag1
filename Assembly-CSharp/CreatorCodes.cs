using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x02000506 RID: 1286
public static class CreatorCodes
{
	// Token: 0x0600201E RID: 8222 RVA: 0x000ACA20 File Offset: 0x000AAC20
	public static string getCurrentCreatorCode(string id)
	{
		if (id.IsNullOrEmpty())
		{
			return string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode == null)
		{
			return string.Empty;
		}
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			return string.Empty;
		}
		return CreatorCodes.data.currentCreatorCode[id];
	}

	// Token: 0x0600201F RID: 8223 RVA: 0x000ACA75 File Offset: 0x000AAC75
	public static CreatorCodes.CreatorCodeStatus getCurrentCreatorCodeStatus(string id)
	{
		if (id == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (CreatorCodes.creatorCodeStatus == null)
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		if (!CreatorCodes.creatorCodeStatus.ContainsKey(id))
		{
			return CreatorCodes.CreatorCodeStatus.Empty;
		}
		return CreatorCodes.creatorCodeStatus[id];
	}

	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06002020 RID: 8224 RVA: 0x000ACAA0 File Offset: 0x000AACA0
	// (remove) Token: 0x06002021 RID: 8225 RVA: 0x000ACAD4 File Offset: 0x000AACD4
	public static event Action<string> OnCreatorCodeChangedEvent;

	// Token: 0x1400004C RID: 76
	// (add) Token: 0x06002022 RID: 8226 RVA: 0x000ACB08 File Offset: 0x000AAD08
	// (remove) Token: 0x06002023 RID: 8227 RVA: 0x000ACB3C File Offset: 0x000AAD3C
	public static event Action InitializedEvent;

	// Token: 0x1400004D RID: 77
	// (add) Token: 0x06002024 RID: 8228 RVA: 0x000ACB70 File Offset: 0x000AAD70
	// (remove) Token: 0x06002025 RID: 8229 RVA: 0x000ACBA4 File Offset: 0x000AADA4
	public static event Action<string, string, NexusGroupId> OnCreatorCodeValidEvent;

	// Token: 0x1400004E RID: 78
	// (add) Token: 0x06002026 RID: 8230 RVA: 0x000ACBD8 File Offset: 0x000AADD8
	// (remove) Token: 0x06002027 RID: 8231 RVA: 0x000ACC0C File Offset: 0x000AAE0C
	public static event Action<string> OnCreatorCodeFailureEvent;

	// Token: 0x06002028 RID: 8232 RVA: 0x000ACC3F File Offset: 0x000AAE3F
	public static void Initialize()
	{
		CreatorCodes.ValidatedCreatorCode = new Dictionary<string, NexusManager.MemberCode>();
		CreatorCodes.creatorCodeStatus = new Dictionary<string, CreatorCodes.CreatorCodeStatus>();
		CreatorCodes.LoadData();
		CreatorCodes.Intialized = true;
		Action initializedEvent = CreatorCodes.InitializedEvent;
		if (initializedEvent == null)
		{
			return;
		}
		initializedEvent();
	}

	// Token: 0x06002029 RID: 8233 RVA: 0x000ACC70 File Offset: 0x000AAE70
	public static void DeleteCharacter(string id)
	{
		if (CreatorCodes.data.currentCreatorCode.ContainsKey(id) && CreatorCodes.data.currentCreatorCode[id].Length > 0)
		{
			CreatorCodes.data.currentCreatorCode[id] = CreatorCodes.data.currentCreatorCode[id].Substring(0, CreatorCodes.data.currentCreatorCode[id].Length - 1);
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = ((CreatorCodes.data.currentCreatorCode[id].Length == 0) ? CreatorCodes.CreatorCodeStatus.Empty : CreatorCodes.CreatorCodeStatus.Unchecked);
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent(id);
		}
	}

	// Token: 0x0600202A RID: 8234 RVA: 0x000ACD2C File Offset: 0x000AAF2C
	public static void AppendKey(string id, string input)
	{
		if (!CreatorCodes.data.currentCreatorCode.ContainsKey(id))
		{
			CreatorCodes.data.currentCreatorCode[id] = string.Empty;
		}
		if (CreatorCodes.data.currentCreatorCode[id].Length < 10)
		{
			Dictionary<string, string> currentCreatorCode = CreatorCodes.data.currentCreatorCode;
			currentCreatorCode[id] += input;
			CreatorCodes.ValidatedCreatorCode[id] = null;
			CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Unchecked;
			Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
			if (onCreatorCodeChangedEvent == null)
			{
				return;
			}
			onCreatorCodeChangedEvent(id);
		}
	}

	// Token: 0x0600202B RID: 8235 RVA: 0x000ACDC4 File Offset: 0x000AAFC4
	public static void ResetCreatorCode(string id)
	{
		Debug.Log("Resetting creator code");
		CreatorCodes.data.currentCreatorCode[id] = "";
		CreatorCodes.creatorCodeStatus[id] = CreatorCodes.CreatorCodeStatus.Empty;
		CreatorCodes.supportedMember = default(Member);
		CreatorCodes.ValidatedCreatorCode[id] = null;
		CreatorCodes.SaveData();
		Action<string> onCreatorCodeChangedEvent = CreatorCodes.OnCreatorCodeChangedEvent;
		if (onCreatorCodeChangedEvent == null)
		{
			return;
		}
		onCreatorCodeChangedEvent(id);
	}

	// Token: 0x0600202C RID: 8236 RVA: 0x000ACE28 File Offset: 0x000AB028
	public static Task<NexusManager.MemberCode> CheckValidationCoroutineJIT(string terminalId, string code, NexusGroupId[] group)
	{
		CreatorCodes.<CheckValidationCoroutineJIT>d__27 <CheckValidationCoroutineJIT>d__;
		<CheckValidationCoroutineJIT>d__.<>t__builder = AsyncTaskMethodBuilder<NexusManager.MemberCode>.Create();
		<CheckValidationCoroutineJIT>d__.terminalId = terminalId;
		<CheckValidationCoroutineJIT>d__.code = code;
		<CheckValidationCoroutineJIT>d__.group = group;
		<CheckValidationCoroutineJIT>d__.<>1__state = -1;
		<CheckValidationCoroutineJIT>d__.<>t__builder.Start<CreatorCodes.<CheckValidationCoroutineJIT>d__27>(ref <CheckValidationCoroutineJIT>d__);
		return <CheckValidationCoroutineJIT>d__.<>t__builder.Task;
	}

	// Token: 0x0600202D RID: 8237 RVA: 0x000ACE7B File Offset: 0x000AB07B
	private static void SaveData()
	{
		PlayerPrefs.SetString("CreatorCodes_Store", JsonConvert.SerializeObject(CreatorCodes.data));
	}

	// Token: 0x0600202E RID: 8238 RVA: 0x000ACE94 File Offset: 0x000AB094
	private static void LoadData()
	{
		string @string = PlayerPrefs.GetString("CreatorCodes_Store", string.Empty);
		if (@string.Length == 0)
		{
			return;
		}
		CreatorCodes.data = JsonConvert.DeserializeObject<CreatorCodes.CreatorCodesData>(@string);
		foreach (string key in CreatorCodes.data.currentCreatorCode.Keys)
		{
			if (CreatorCodes.data.codeFirstUsedTime.ContainsKey(key) && DateTime.UtcNow.Subtract(CreatorCodes.data.codeFirstUsedTime[key]).Days > 14)
			{
				CreatorCodes.data.currentCreatorCode[key] = string.Empty;
			}
		}
	}

	// Token: 0x04002AEC RID: 10988
	private const int MAX_CODE_LENGTH = 10;

	// Token: 0x04002AED RID: 10989
	private const string PLAYER_PREF_KEY = "CreatorCodes_Store";

	// Token: 0x04002AEE RID: 10990
	private const int DAYS_TO_STORE_CODE = 14;

	// Token: 0x04002AEF RID: 10991
	private static CreatorCodes.CreatorCodesData data = new CreatorCodes.CreatorCodesData();

	// Token: 0x04002AF0 RID: 10992
	private static Dictionary<string, NexusManager.MemberCode> ValidatedCreatorCode;

	// Token: 0x04002AF1 RID: 10993
	private static Dictionary<string, CreatorCodes.CreatorCodeStatus> creatorCodeStatus;

	// Token: 0x04002AF6 RID: 10998
	public static bool Intialized = false;

	// Token: 0x04002AF7 RID: 10999
	public static Member supportedMember;

	// Token: 0x02000507 RID: 1287
	public enum CreatorCodeStatus
	{
		// Token: 0x04002AF9 RID: 11001
		Empty,
		// Token: 0x04002AFA RID: 11002
		Unchecked,
		// Token: 0x04002AFB RID: 11003
		Validating,
		// Token: 0x04002AFC RID: 11004
		Valid
	}

	// Token: 0x02000508 RID: 1288
	[Serializable]
	private class CreatorCodesData
	{
		// Token: 0x04002AFD RID: 11005
		public Dictionary<string, string> currentCreatorCode = new Dictionary<string, string>();

		// Token: 0x04002AFE RID: 11006
		public Dictionary<string, DateTime> codeFirstUsedTime = new Dictionary<string, DateTime>();
	}
}
