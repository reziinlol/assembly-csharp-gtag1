using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200046B RID: 1131
public class NetworkWrapper : MonoBehaviour
{
	// Token: 0x06001B7E RID: 7038 RVA: 0x00094F83 File Offset: 0x00093183
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void AutoInstantiate()
	{
		Object.DontDestroyOnLoad(Object.Instantiate<GameObject>(Resources.Load<GameObject>("P_NetworkWrapper")));
	}

	// Token: 0x06001B7F RID: 7039 RVA: 0x00094F9C File Offset: 0x0009319C
	private void Awake()
	{
		if (this.titleRef != null)
		{
			this.titleRef.text = "PUN";
		}
		this.activeNetworkSystem = base.gameObject.AddComponent<NetworkSystemPUN>();
		this.activeNetworkSystem.AddVoiceSettings(this.VoiceSettings);
		this.activeNetworkSystem.config = this.netSysConfig;
		this.activeNetworkSystem.regionNames = this.networkRegionNames;
		this.activeNetworkSystem.OnPlayerJoined += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnPlayerLeft += this.UpdatePlayerCountWrapper;
		this.activeNetworkSystem.OnMultiplayerStarted += this.UpdatePlayerCount;
		this.activeNetworkSystem.OnReturnedToSinglePlayer += this.UpdatePlayerCount;
		Debug.Log("<color=green>initialize Network System</color>");
		this.activeNetworkSystem.Initialise();
	}

	// Token: 0x06001B80 RID: 7040 RVA: 0x000950A8 File Offset: 0x000932A8
	private void UpdatePlayerCountWrapper(NetPlayer player)
	{
		this.UpdatePlayerCount();
	}

	// Token: 0x06001B81 RID: 7041 RVA: 0x000950B0 File Offset: 0x000932B0
	private void UpdatePlayerCount()
	{
		if (this.playerCountTextRef == null)
		{
			return;
		}
		if (!this.activeNetworkSystem.IsOnline)
		{
			this.playerCountTextRef.text = string.Format("0/{0}", this.netSysConfig.MaxPlayerCount);
			Debug.Log("Player count updated");
			return;
		}
		Debug.Log("Player count not updated");
		this.playerCountTextRef.text = string.Format("{0}/{1}", this.activeNetworkSystem.AllNetPlayers.Length, this.netSysConfig.MaxPlayerCount);
	}

	// Token: 0x040025A0 RID: 9632
	[HideInInspector]
	public NetworkSystem activeNetworkSystem;

	// Token: 0x040025A1 RID: 9633
	public Text titleRef;

	// Token: 0x040025A2 RID: 9634
	[Header("NetSys settings")]
	public NetworkSystemConfig netSysConfig;

	// Token: 0x040025A3 RID: 9635
	public string[] networkRegionNames;

	// Token: 0x040025A4 RID: 9636
	public string[] devNetworkRegionNames;

	// Token: 0x040025A5 RID: 9637
	[Header("Debug output refs")]
	public Text stateTextRef;

	// Token: 0x040025A6 RID: 9638
	public Text playerCountTextRef;

	// Token: 0x040025A7 RID: 9639
	[SerializeField]
	private SO_NetworkVoiceSettings VoiceSettings;

	// Token: 0x040025A8 RID: 9640
	private const string WrapperResourcePath = "P_NetworkWrapper";
}
