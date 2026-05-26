using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Oculus.Platform;
using Oculus.Platform.Models;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F2A RID: 3882
	public class SubscriptionManager : MonoBehaviour
	{
		// Token: 0x1700092D RID: 2349
		// (get) Token: 0x060060F0 RID: 24816 RVA: 0x001F36FA File Offset: 0x001F18FA
		public static bool LocalSubscriptionDataInitialized
		{
			get
			{
				return SubscriptionManager._localSubscriptionDataInitialized;
			}
		}

		// Token: 0x1700092E RID: 2350
		// (get) Token: 0x060060F1 RID: 24817 RVA: 0x001F3701 File Offset: 0x001F1901
		// (set) Token: 0x060060F2 RID: 24818 RVA: 0x001F3710 File Offset: 0x001F1910
		public static bool SubsOnlyMatchmaking
		{
			get
			{
				return PlayerPrefs.GetInt("subsOnlyMatchmaking") == 1;
			}
			set
			{
				PlayerPrefs.SetInt("subsOnlyMatchmaking", value ? 1 : 0);
				PlayerPrefs.Save();
			}
		}

		// Token: 0x060060F3 RID: 24819 RVA: 0x001F3728 File Offset: 0x001F1928
		public static string GetSubsFeatureKey(SubscriptionManager.SubscriptionFeatures feature)
		{
			return SubscriptionManager.SUBS_KEYS[(int)feature];
		}

		// Token: 0x060060F4 RID: 24820 RVA: 0x001F3734 File Offset: 0x001F1934
		private void Awake()
		{
			SubscriptionManager.<Awake>d__25 <Awake>d__;
			<Awake>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Awake>d__.<>4__this = this;
			<Awake>d__.<>1__state = -1;
			<Awake>d__.<>t__builder.Start<SubscriptionManager.<Awake>d__25>(ref <Awake>d__);
		}

		// Token: 0x060060F5 RID: 24821 RVA: 0x001F376B File Offset: 0x001F196B
		protected void OnEnable()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeft);
			SubscriptionManager.InitializePersonalSubscriptionData();
		}

		// Token: 0x060060F6 RID: 24822 RVA: 0x001F37A8 File Offset: 0x001F19A8
		public static void InitializePersonalSubscriptionData()
		{
			SubscriptionManager.<InitializePersonalSubscriptionData>d__33 <InitializePersonalSubscriptionData>d__;
			<InitializePersonalSubscriptionData>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<InitializePersonalSubscriptionData>d__.<>1__state = -1;
			<InitializePersonalSubscriptionData>d__.<>t__builder.Start<SubscriptionManager.<InitializePersonalSubscriptionData>d__33>(ref <InitializePersonalSubscriptionData>d__);
		}

		// Token: 0x060060F7 RID: 24823 RVA: 0x001F37D7 File Offset: 0x001F19D7
		protected void OnDisable()
		{
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoinedRoom);
			RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeft);
		}

		// Token: 0x060060F8 RID: 24824 RVA: 0x001F3810 File Offset: 0x001F1A10
		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(VRRig rig)
		{
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.rigs.ContainsKey(rig))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return SubscriptionManager.GetSubscriptionDetails(SubscriptionManager.Instance.rigs[rig]);
		}

		// Token: 0x060060F9 RID: 24825 RVA: 0x001F385C File Offset: 0x001F1A5C
		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails(NetPlayer np)
		{
			SubscriptionManager.SubscriptionDetails result;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(np, out result))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return result;
		}

		// Token: 0x060060FA RID: 24826 RVA: 0x001F3895 File Offset: 0x001F1A95
		public static bool IsPlayerSubscribed(VRRig rig)
		{
			return SubscriptionManager.GetSubscriptionDetails(rig).active;
		}

		// Token: 0x060060FB RID: 24827 RVA: 0x001F38A2 File Offset: 0x001F1AA2
		public static bool IsPlayerSubscribed(NetPlayer np)
		{
			return SubscriptionManager.GetSubscriptionDetails(np).active;
		}

		// Token: 0x060060FC RID: 24828 RVA: 0x001F38B0 File Offset: 0x001F1AB0
		public static SubscriptionManager.SubscriptionDetails GetSubscriptionDetails()
		{
			SubscriptionManager.SubscriptionDetails result;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out result))
			{
				return default(SubscriptionManager.SubscriptionDetails);
			}
			return result;
		}

		// Token: 0x060060FD RID: 24829 RVA: 0x001F38F4 File Offset: 0x001F1AF4
		public static SubscriptionManager.SubscriptionStatus LocalSubscriptionStatus()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			if (SubscriptionManager.Instance == null || !SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails))
			{
				return SubscriptionManager.SubscriptionStatus.Unknown;
			}
			if (!subscriptionDetails.active)
			{
				return SubscriptionManager.SubscriptionStatus.Inactive;
			}
			return SubscriptionManager.SubscriptionStatus.Active;
		}

		// Token: 0x060060FE RID: 24830 RVA: 0x001F3938 File Offset: 0x001F1B38
		public static SubscriptionManager.SubscriptionDetails LocalSubscriptionDetails()
		{
			return SubscriptionManager.localSubscriptionDetails;
		}

		// Token: 0x060060FF RID: 24831 RVA: 0x001F3940 File Offset: 0x001F1B40
		public static bool IsLocalSubscribed()
		{
			SubscriptionManager.SubscriptionDetails subscriptionDetails;
			return !(SubscriptionManager.Instance == null) && !(VRRig.LocalRig == null) && VRRig.LocalRig.creator != null && SubscriptionManager.Instance.subData.TryGetValue(VRRig.LocalRig.creator, out subscriptionDetails) && subscriptionDetails.active;
		}

		// Token: 0x06006100 RID: 24832 RVA: 0x001F3998 File Offset: 0x001F1B98
		public static void ForceRecheck()
		{
			SubscriptionManager.Instance.OnPlayerJoinedRoom(null);
		}

		// Token: 0x06006101 RID: 24833 RVA: 0x001F39A5 File Offset: 0x001F1BA5
		private void OnPlayerJoinedRoom(NetPlayer npl)
		{
			if (SubscriptionManager.OnSubscriptionData != null)
			{
				SubscriptionManager.OnSubscriptionData();
			}
			if (NetworkSystem.Instance.AllNetPlayers.Length > SubscriptionManager.PERF_CHANGE_ROOMSIZE)
			{
				GorillaTagger.Instance.ToggleForcedPerformanceRefresh();
				PhotonNetwork.SendRate = 20;
			}
		}

		// Token: 0x06006102 RID: 24834 RVA: 0x001F39DC File Offset: 0x001F1BDC
		private void UpdatePlayerSubsDetails(NetPlayer player, bool? isSubscribed = null, int? daysAccrued = null)
		{
			if (player == null)
			{
				return;
			}
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(player.ActorNumber, out rigContainer))
			{
				this.rigs[rigContainer.Rig] = player;
			}
			if (player != NetworkSystem.Instance.LocalPlayer)
			{
				bool flag = player == VRRig.LocalRig.creator;
			}
			bool flag2 = false;
			int daysAccrued2 = 0;
			int tier = 0;
			if (isSubscribed != null)
			{
				flag2 = isSubscribed.Value;
				tier = (flag2 ? 1 : 0);
				daysAccrued2 = daysAccrued.GetValueOrDefault();
			}
			SubscriptionManager.SubscriptionDetails value = new SubscriptionManager.SubscriptionDetails
			{
				active = flag2,
				tier = tier,
				daysAccrued = daysAccrued2
			};
			this.subData[player] = value;
		}

		// Token: 0x06006103 RID: 24835 RVA: 0x001F3A8C File Offset: 0x001F1C8C
		private void OnPlayerLeft(NetPlayer pl)
		{
			if (this.subData.ContainsKey(pl))
			{
				this.subData.Remove(pl);
			}
			NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
			if (allNetPlayers.Length <= SubscriptionManager.PERF_CHANGE_ROOMSIZE)
			{
				GorillaTagger.Instance.ToggleDefaultPerformanceRefresh();
				PhotonNetwork.SendRate = SubscriptionManager.DEFAULT_SEND_RATE;
			}
			NetPlayer lowestNetPlayer = this.GetLowestNetPlayer(allNetPlayers);
			if (lowestNetPlayer != null && lowestNetPlayer == NetworkSystem.Instance.LocalPlayer)
			{
				byte currentRoomExpectedSize = RoomSystem.GetCurrentRoomExpectedSize();
				PhotonNetwork.CurrentRoom.MaxPlayers = currentRoomExpectedSize;
			}
		}

		// Token: 0x06006104 RID: 24836 RVA: 0x001F3B08 File Offset: 0x001F1D08
		private NetPlayer GetLowestNetPlayer(NetPlayer[] players)
		{
			NetPlayer result = null;
			int num = int.MaxValue;
			for (int i = 0; i < players.Length; i++)
			{
				if (players[i].ActorNumber < num)
				{
					num = players[i].ActorNumber;
					result = players[i];
				}
			}
			return result;
		}

		// Token: 0x06006105 RID: 24837 RVA: 0x001F3B44 File Offset: 0x001F1D44
		private void OnGetViewerPurchasesStartup(Message msg)
		{
			if (msg.IsError)
			{
				if (this.attempts < 3)
				{
					this.attempts++;
					IAP.GetViewerPurchases().OnComplete(new Message<PurchaseList>.Callback(this.OnGetViewerPurchasesStartup));
				}
				return;
			}
			if (msg.GetPurchaseList() == null)
			{
				return;
			}
			if (SubscriptionManager._localSubscriptionDataInitialized)
			{
				return;
			}
			bool flag = false;
			foreach (Purchase purchase in msg.GetPurchaseList())
			{
				if (purchase.Type == ProductType.SUBSCRIPTION && purchase.Sku.Contains("fan_club"))
				{
					flag = true;
					SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
					{
						active = (DateTime.Now < purchase.ExpirationTime),
						subscriptionActiveUntilDate = purchase.ExpirationTime
					};
				}
			}
			if (!flag)
			{
				SubscriptionManager.localSubscriptionDetails = new SubscriptionManager.SubscriptionDetails
				{
					active = false
				};
			}
		}

		// Token: 0x06006106 RID: 24838 RVA: 0x001F3C3C File Offset: 0x001F1E3C
		public static void SetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature, int settingValue)
		{
			string subsFeatureKey = SubscriptionManager.GetSubsFeatureKey(feature);
			PlayerPrefs.SetInt(subsFeatureKey, settingValue);
			SubscriptionManager.subSettings[subsFeatureKey] = settingValue;
			PlayerPrefs.Save();
		}

		// Token: 0x06006107 RID: 24839 RVA: 0x001F3C68 File Offset: 0x001F1E68
		public static int GetSubscriptionSettingValue(SubscriptionManager.SubscriptionFeatures feature)
		{
			string subsFeatureKey = SubscriptionManager.GetSubsFeatureKey(feature);
			int result;
			if (SubscriptionManager.subSettings.TryGetValue(subsFeatureKey, out result))
			{
				return result;
			}
			SubscriptionManager.subSettings[subsFeatureKey] = PlayerPrefs.GetInt(subsFeatureKey, 1);
			return SubscriptionManager.subSettings[subsFeatureKey];
		}

		// Token: 0x06006108 RID: 24840 RVA: 0x001F3CAA File Offset: 0x001F1EAA
		public static bool GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures feature)
		{
			return SubscriptionManager.GetSubscriptionSettingValue(feature) >= 1;
		}

		// Token: 0x06006109 RID: 24841 RVA: 0x001F3CB8 File Offset: 0x001F1EB8
		public static bool IsSubscriptionFeatureAvailable(SubscriptionManager.SubscriptionFeatures feature)
		{
			if (feature != SubscriptionManager.SubscriptionFeatures.IOBT)
			{
				return feature != SubscriptionManager.SubscriptionFeatures.HandTracking || UnityEngine.Application.platform == RuntimePlatform.Android;
			}
			if (UnityEngine.Application.platform != RuntimePlatform.Android)
			{
				return false;
			}
			OVRPlugin.SystemHeadset systemHeadsetType = OVRPlugin.GetSystemHeadsetType();
			return systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Quest_3 || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Quest_3S || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Link_Quest_3 || systemHeadsetType == OVRPlugin.SystemHeadset.Meta_Link_Quest_3S;
		}

		// Token: 0x0600610A RID: 24842 RVA: 0x001F3D0B File Offset: 0x001F1F0B
		public static bool CheckSubscriptionFeaturePermission(SubscriptionManager.SubscriptionFeatures feature)
		{
			if (feature != SubscriptionManager.SubscriptionFeatures.IOBT)
			{
				return feature != SubscriptionManager.SubscriptionFeatures.HandTracking || OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.BodyTracking);
			}
			return OVRPermissionsRequester.IsPermissionGranted(OVRPermissionsRequester.Permission.BodyTracking);
		}

		// Token: 0x0600610B RID: 24843 RVA: 0x000028C5 File Offset: 0x00000AC5
		[RuntimeInitializeOnLoadMethod]
		private static void OnLoad()
		{
		}

		// Token: 0x0600610C RID: 24844 RVA: 0x001F3D28 File Offset: 0x001F1F28
		public static void UpdatePlayerSubscriptionData(NetPlayer player, bool isSubscribed, int daysAccrued = 0)
		{
			if (SubscriptionManager.Instance == null)
			{
				Debug.LogWarning("SubscriptionManager: Instance is null, cannot update player subscription data");
				return;
			}
			if (player == null)
			{
				Debug.LogWarning("SubscriptionManager: NetPlayer is null, cannot update subscription data");
				return;
			}
			SubscriptionManager.Instance.UpdatePlayerSubsDetails(player, new bool?(isSubscribed), new int?(daysAccrued));
			if (SubscriptionManager.OnSubscriptionData != null)
			{
				SubscriptionManager.OnSubscriptionData();
			}
		}

		// Token: 0x04006F83 RID: 28547
		public const string FAN_CLUB_BASE_SKU = "fan_club";

		// Token: 0x04006F84 RID: 28548
		public const string FAN_CLUB_STEAM_SKU = "40494";

		// Token: 0x04006F85 RID: 28549
		public const string SUBSCRIBER_NAME_COLOR_HEX = "#ffc600";

		// Token: 0x04006F86 RID: 28550
		public static Color SUBSCRIBER_NAME_COLOR = Color.gold;

		// Token: 0x04006F87 RID: 28551
		public const int PERF_SEND_RATE = 20;

		// Token: 0x04006F88 RID: 28552
		public static int DEFAULT_SEND_RATE = 30;

		// Token: 0x04006F89 RID: 28553
		public static int PERF_CHANGE_ROOMSIZE = 10;

		// Token: 0x04006F8A RID: 28554
		private static SubscriptionManager Instance;

		// Token: 0x04006F8B RID: 28555
		public static Action OnSubscriptionData;

		// Token: 0x04006F8C RID: 28556
		public static Action OnLocalSubscriptionData;

		// Token: 0x04006F8D RID: 28557
		private Dictionary<NetPlayer, SubscriptionManager.SubscriptionDetails> subData = new Dictionary<NetPlayer, SubscriptionManager.SubscriptionDetails>();

		// Token: 0x04006F8E RID: 28558
		private Dictionary<VRRig, NetPlayer> rigs = new Dictionary<VRRig, NetPlayer>();

		// Token: 0x04006F8F RID: 28559
		private static SubscriptionManager.SubscriptionDetails localSubscriptionDetails;

		// Token: 0x04006F90 RID: 28560
		private static bool _localSubscriptionDataInitialized;

		// Token: 0x04006F91 RID: 28561
		public const string SUB_PREFIX = "SMKEYPREFIX";

		// Token: 0x04006F92 RID: 28562
		public static string[] SUBS_KEYS;

		// Token: 0x04006F93 RID: 28563
		private static int maxRetries = 3;

		// Token: 0x04006F94 RID: 28564
		private int attempts;

		// Token: 0x04006F95 RID: 28565
		private static Dictionary<string, int> subSettings = new Dictionary<string, int>();

		// Token: 0x02000F2B RID: 3883
		public enum SubscriptionStatus
		{
			// Token: 0x04006F97 RID: 28567
			Active,
			// Token: 0x04006F98 RID: 28568
			Inactive,
			// Token: 0x04006F99 RID: 28569
			Unknown
		}

		// Token: 0x02000F2C RID: 3884
		public enum SubscriptionTerm
		{
			// Token: 0x04006F9B RID: 28571
			MONTHLY,
			// Token: 0x04006F9C RID: 28572
			QUARTERLY,
			// Token: 0x04006F9D RID: 28573
			SEMIANNUAL,
			// Token: 0x04006F9E RID: 28574
			ANNUAL
		}

		// Token: 0x02000F2D RID: 3885
		public enum SubscriptionFeatures
		{
			// Token: 0x04006FA0 RID: 28576
			GoldenName,
			// Token: 0x04006FA1 RID: 28577
			IOBT,
			// Token: 0x04006FA2 RID: 28578
			HandTracking,
			// Token: 0x04006FA3 RID: 28579
			SubscriptionFeatureCount
		}

		// Token: 0x02000F2E RID: 3886
		public struct SubscriptionDetails
		{
			// Token: 0x04006FA4 RID: 28580
			public bool active;

			// Token: 0x04006FA5 RID: 28581
			public int daysAccrued;

			// Token: 0x04006FA6 RID: 28582
			public bool[] subscriptionFeatureSettings;

			// Token: 0x04006FA7 RID: 28583
			public int tier;

			// Token: 0x04006FA8 RID: 28584
			public DateTime subscriptionActiveUntilDate;

			// Token: 0x04006FA9 RID: 28585
			public bool autoRenew;

			// Token: 0x04006FAA RID: 28586
			public int autoRenewMonths;
		}

		// Token: 0x02000F2F RID: 3887
		[Serializable]
		private class MothershipSubscription
		{
			// Token: 0x04006FAB RID: 28587
			public string SubscriptionId;

			// Token: 0x04006FAC RID: 28588
			public DateTimeOffset EarliestStartDate;

			// Token: 0x04006FAD RID: 28589
			public DateTimeOffset CurrentStartDate;

			// Token: 0x04006FAE RID: 28590
			public DateTimeOffset MostRecentBillingCycleStartDate;

			// Token: 0x04006FAF RID: 28591
			public DateTimeOffset MostRecentBillingCycleEndDate;

			// Token: 0x04006FB0 RID: 28592
			public int TotalLifetimeSeconds;

			// Token: 0x04006FB1 RID: 28593
			public bool IsActive;

			// Token: 0x04006FB2 RID: 28594
			public bool IsCancelling;

			// Token: 0x04006FB3 RID: 28595
			public string Sku;

			// Token: 0x04006FB4 RID: 28596
			public string PlayerId;

			// Token: 0x04006FB5 RID: 28597
			public string TrialType;

			// Token: 0x04006FB6 RID: 28598
			public string ExternalServiceName;

			// Token: 0x04006FB7 RID: 28599
			public string ExternalSubscriptionId;

			// Token: 0x04006FB8 RID: 28600
			public string SubscriptionCatalogItemId;
		}

		// Token: 0x02000F30 RID: 3888
		[Serializable]
		private class GrantedSubscriptionBenefit
		{
			// Token: 0x04006FB9 RID: 28601
			public string BenefitId;

			// Token: 0x04006FBA RID: 28602
			public DateTimeOffset GrantedTime;

			// Token: 0x04006FBB RID: 28603
			public string PlayFabItemId;
		}

		// Token: 0x02000F31 RID: 3889
		[Serializable]
		private class GetMySubscriptionsAndTheirBenefitsRequest
		{
			// Token: 0x04006FBC RID: 28604
			public bool Refresh;

			// Token: 0x04006FBD RID: 28605
			public bool? SkipBenefitsCheck;

			// Token: 0x04006FBE RID: 28606
			public bool? SkipSharedGroupDataUpdate;

			// Token: 0x04006FBF RID: 28607
			public string MothershipId;

			// Token: 0x04006FC0 RID: 28608
			public string MothershipToken;

			// Token: 0x04006FC1 RID: 28609
			public string MothershipEnvId;

			// Token: 0x04006FC2 RID: 28610
			public string MothershipDeploymentId;
		}

		// Token: 0x02000F32 RID: 3890
		[Serializable]
		private class GetMySubscriptionsAndTheirBenefitsResponse
		{
			// Token: 0x04006FC3 RID: 28611
			public List<SubscriptionManager.MothershipSubscription> Subscriptions;

			// Token: 0x04006FC4 RID: 28612
			public Dictionary<string, List<SubscriptionManager.GrantedSubscriptionBenefit>> PreviouslyGrantedBenefitsBySubscriptionSku;

			// Token: 0x04006FC5 RID: 28613
			public Dictionary<string, List<SubscriptionManager.GrantedSubscriptionBenefit>> NewlyGrantedBenefitsBySubscriptionSku;

			// Token: 0x04006FC6 RID: 28614
			public bool? SharedGroupDataUpdateSucceeded;
		}
	}
}
