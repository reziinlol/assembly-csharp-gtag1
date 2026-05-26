using System;
using System.Collections.Generic;
using GorillaGameModes;
using GorillaTagScripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace GorillaNetworking
{
	// Token: 0x02001060 RID: 4192
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06006941 RID: 26945 RVA: 0x002210A4 File Offset: 0x0021F2A4
		public GroupJoinZoneAB groupJoinRequiredZonesAB
		{
			get
			{
				return new GroupJoinZoneAB
				{
					a = this.groupJoinRequiredZones,
					b = this.groupJoinRequiredZonesB
				};
			}
		}

		// Token: 0x06006942 RID: 26946 RVA: 0x002210D4 File Offset: 0x0021F2D4
		private void Start()
		{
			if (this.primaryTriggerForMyZone == null)
			{
				this.primaryTriggerForMyZone = this;
			}
			if (this.primaryTriggerForMyZone == this)
			{
				GorillaComputer.instance.RegisterPrimaryJoinTrigger(this);
			}
			PhotonNetworkController.Instance.RegisterJoinTrigger(this);
			if (!this.didRegisterForCallbacks && this.ui != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x06006943 RID: 26947 RVA: 0x00221154 File Offset: 0x0021F354
		public void RegisterUI(JoinTriggerUI ui)
		{
			this.ui = ui;
			if (!this.didRegisterForCallbacks && FriendshipGroupDetection.Instance != null)
			{
				this.didRegisterForCallbacks = true;
				FriendshipGroupDetection.Instance.AddGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
			this.UpdateUI();
		}

		// Token: 0x06006944 RID: 26948 RVA: 0x002211A0 File Offset: 0x0021F3A0
		public void UnregisterUI(JoinTriggerUI ui)
		{
			this.ui = null;
		}

		// Token: 0x06006945 RID: 26949 RVA: 0x002211A9 File Offset: 0x0021F3A9
		private void OnDestroy()
		{
			if (this.didRegisterForCallbacks)
			{
				FriendshipGroupDetection.Instance.RemoveGroupZoneCallback(new Action<GroupJoinZoneAB>(this.OnGroupPositionsChanged));
			}
		}

		// Token: 0x06006946 RID: 26950 RVA: 0x002211C9 File Offset: 0x0021F3C9
		private void OnGroupPositionsChanged(GroupJoinZoneAB groupZone)
		{
			this.UpdateUI();
		}

		// Token: 0x06006947 RID: 26951 RVA: 0x002211D4 File Offset: 0x0021F3D4
		public void UpdateUI()
		{
			if (this.ui == null || NetworkSystem.Instance == null)
			{
				return;
			}
			if (GorillaScoreboardTotalUpdater.instance.offlineTextErrorString != null)
			{
				this.ui.SetState(JoinTriggerVisualState.ConnectionError, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (NetworkSystem.Instance.SessionIsPrivate)
			{
				this.ui.SetState(JoinTriggerVisualState.InPrivateRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
			{
				this.ui.SetState(JoinTriggerVisualState.AlreadyInRoom, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				this.ui.SetState(this.CanPartyJoin() ? JoinTriggerVisualState.LeaveRoomAndPartyJoin : JoinTriggerVisualState.AbandonPartyAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (!NetworkSystem.Instance.InRoom)
			{
				this.ui.SetState(JoinTriggerVisualState.NotConnectedSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			if (PhotonNetworkController.Instance.currentJoinTrigger == this.primaryTriggerForMyZone)
			{
				this.ui.SetState(JoinTriggerVisualState.ChangingGameModeSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
				return;
			}
			this.ui.SetState(JoinTriggerVisualState.LeaveRoomAndSoloJoin, new Func<string>(this.GetActiveNetworkZone), new Func<string>(this.GetDesiredNetworkZone), new Func<string>(GorillaNetworkJoinTrigger.GetActiveGameType), new Func<string>(this.GetDesiredGameTypeLocalized));
		}

		// Token: 0x06006948 RID: 26952 RVA: 0x0022141E File Offset: 0x0021F61E
		private string GetActiveNetworkZone()
		{
			return PhotonNetworkController.Instance.currentJoinTrigger.networkZone.ToUpper();
		}

		// Token: 0x06006949 RID: 26953 RVA: 0x00221436 File Offset: 0x0021F636
		private string GetDesiredNetworkZone()
		{
			return this.networkZone.ToUpper();
		}

		// Token: 0x0600694A RID: 26954 RVA: 0x00221443 File Offset: 0x0021F643
		public static string GetActiveGameType()
		{
			GorillaGameManager activeGameMode = GameMode.ActiveGameMode;
			return ((activeGameMode != null) ? activeGameMode.GameModeName() : null) ?? "";
		}

		// Token: 0x0600694B RID: 26955 RVA: 0x00221460 File Offset: 0x0021F660
		public string GetDesiredGameType()
		{
			GameModeType gameModeType;
			return GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.TryParse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true, out gameModeType) ? gameModeType : GameModeType.Casual, NetworkSystem.Instance.SessionIsPrivate).ToString();
		}

		// Token: 0x0600694C RID: 26956 RVA: 0x002214B4 File Offset: 0x0021F6B4
		public GameModeType GetDesiredGameModeType()
		{
			GameModeType gameModeType;
			return GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.TryParse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true, out gameModeType) ? gameModeType : GameModeType.Casual, NetworkSystem.Instance.SessionIsPrivate);
		}

		// Token: 0x0600694D RID: 26957 RVA: 0x002214FC File Offset: 0x0021F6FC
		public string GetDesiredGameTypeLocalized()
		{
			GameModeType gameModeType;
			return GorillaGameManager.GameModeEnumToName(GameMode.GameModeZoneMapping.VerifyModeForZone(this.zone, Enum.TryParse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true, out gameModeType) ? gameModeType : GameModeType.Casual, NetworkSystem.Instance.SessionIsPrivate));
		}

		// Token: 0x0600694E RID: 26958 RVA: 0x00221547 File Offset: 0x0021F747
		public virtual string GetFullDesiredGameModeString()
		{
			return new GameModeString
			{
				zone = this.networkZone,
				queue = GorillaComputer.instance.currentQueue,
				gameType = this.GetDesiredGameType()
			}.ToString();
		}

		// Token: 0x0600694F RID: 26959 RVA: 0x0022157D File Offset: 0x0021F77D
		public virtual bool SameZoneAsOverride()
		{
			return NetworkSystem.Instance.groupJoinOverrideGameMode.StartsWith(this.networkZone);
		}

		// Token: 0x06006950 RID: 26960 RVA: 0x00221594 File Offset: 0x0021F794
		public virtual byte GetRoomSize(bool subscribed)
		{
			return RoomSystem.GetRoomSizeForCreate(this.zone, this.GetDesiredGameModeType(), false, subscribed);
		}

		// Token: 0x06006951 RID: 26961 RVA: 0x002215A9 File Offset: 0x0021F7A9
		public bool CanPartyJoin()
		{
			return this.CanPartyJoin(FriendshipGroupDetection.Instance.partyZone);
		}

		// Token: 0x06006952 RID: 26962 RVA: 0x002215BB File Offset: 0x0021F7BB
		public bool CanPartyJoin(GroupJoinZoneAB zone)
		{
			return (this.groupJoinRequiredZonesAB & zone) == zone;
		}

		// Token: 0x06006953 RID: 26963 RVA: 0x002215D0 File Offset: 0x0021F7D0
		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			if (this.isSubsOnly)
			{
				if (SubscriptionManager.IsLocalSubscribed())
				{
					this.SubsPublicJoin();
				}
				return;
			}
			if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
			{
				Debug.Log("GorillaNetworkJoinTrigger::OnBoxTriggered - blocking join call");
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			if (NetworkSystem.Instance.groupJoinInProgress)
			{
				return;
			}
			List<ValueTuple<string, string>> list = new List<ValueTuple<string, string>>();
			foreach (AdditionalCustomProperty additionalCustomProperty in this.additionalJoinCustomProperties)
			{
				list.Add(new ValueTuple<string, string>(additionalCustomProperty.key, additionalCustomProperty.value));
			}
			if (FriendshipGroupDetection.Instance.IsInParty)
			{
				if (this.ignoredIfInParty)
				{
					return;
				}
				if (NetworkSystem.Instance.netState == NetSystemState.Connecting || NetworkSystem.Instance.netState == NetSystemState.Disconnecting || NetworkSystem.Instance.netState == NetSystemState.Initialization || NetworkSystem.Instance.netState == NetSystemState.PingRecon)
				{
					return;
				}
				if (NetworkSystem.Instance.InRoom)
				{
					if (NetworkSystem.Instance.GameModeString == this.GetFullDesiredGameModeString())
					{
						GTDev.Log<string>("JoinTrigger: Ignoring party join/leave because " + this.networkZone + " is already the game mode", null);
						return;
					}
					if (NetworkSystem.Instance.SessionIsPrivate)
					{
						GTDev.Log<string>("JoinTrigger: Ignoring party join/leave because we're in a private room", null);
						return;
					}
					if (this.SameZoneAsOverride())
					{
						GTDev.Log<string>("JoinTrigger: Ignoring party join/leave because we joined as a group, and this trigger matches the zone for the override, so there's no reason to attempt to leave", null);
						return;
					}
				}
				if (this.CanPartyJoin())
				{
					Debug.Log(string.Format("JoinTrigger: Attempting party join in 1 second! <{0}> accepts <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
					PhotonNetworkController.Instance.DeferJoining(1f);
					FriendshipGroupDetection.Instance.SendAboutToGroupJoin();
					PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.JoinWithParty, list, false);
					return;
				}
				Debug.Log(string.Format("JoinTrigger: LeaveGroup: Leaving party and will solo join, wanted <{0}> but got <{1}>", this.groupJoinRequiredZones, FriendshipGroupDetection.Instance.partyZone));
				FriendshipGroupDetection.Instance.LeaveParty();
				PhotonNetworkController.Instance.DeferJoining(1f);
			}
			else
			{
				Debug.Log("JoinTrigger: Solo join (not in a group)");
				PhotonNetworkController.Instance.ClearDeferredJoin();
			}
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo, list, false);
		}

		// Token: 0x06006954 RID: 26964 RVA: 0x002217EC File Offset: 0x0021F9EC
		public void SubsPublicJoin()
		{
			if (GorillaNetworkJoinTrigger.triggerJoinsDisabled)
			{
				Debug.Log("GorillaNetworkJoinTrigger::SubsPublicJoin - blocking join call");
				return;
			}
			GorillaComputer.instance.allowedMapsToJoin = this.myCollider.myAllowedMapsToJoin;
			PhotonNetworkController.Instance.ClearDeferredJoin();
			PhotonNetworkController.Instance.AttemptToJoinPublicRoom(this, JoinType.Solo, null, SubscriptionManager.IsLocalSubscribed());
		}

		// Token: 0x06006955 RID: 26965 RVA: 0x00221842 File Offset: 0x0021FA42
		public static void DisableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::DisableTriggerJoins] Disabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = true;
		}

		// Token: 0x06006956 RID: 26966 RVA: 0x00221854 File Offset: 0x0021FA54
		public static void EnableTriggerJoins()
		{
			Debug.Log("[GorillaNetworkJoinTrigger::EnableTriggerJoins] Enabling Trigger-based Room Joins...");
			GorillaNetworkJoinTrigger.triggerJoinsDisabled = false;
		}

		// Token: 0x04007972 RID: 31090
		public GameObject[] makeSureThisIsDisabled;

		// Token: 0x04007973 RID: 31091
		public GameObject[] makeSureThisIsEnabled;

		// Token: 0x04007974 RID: 31092
		public GTZone zone;

		// Token: 0x04007975 RID: 31093
		public GroupJoinZoneA groupJoinRequiredZones;

		// Token: 0x04007976 RID: 31094
		public GroupJoinZoneB groupJoinRequiredZonesB;

		// Token: 0x04007977 RID: 31095
		[FormerlySerializedAs("gameModeName")]
		public string networkZone;

		// Token: 0x04007978 RID: 31096
		public GorillaFriendCollider myCollider;

		// Token: 0x04007979 RID: 31097
		public GorillaNetworkJoinTrigger primaryTriggerForMyZone;

		// Token: 0x0400797A RID: 31098
		public bool ignoredIfInParty;

		// Token: 0x0400797B RID: 31099
		public bool isSubsOnly;

		// Token: 0x0400797C RID: 31100
		private JoinTriggerUI ui;

		// Token: 0x0400797D RID: 31101
		private bool didRegisterForCallbacks;

		// Token: 0x0400797E RID: 31102
		public AdditionalCustomProperty[] additionalJoinCustomProperties;

		// Token: 0x0400797F RID: 31103
		private static bool triggerJoinsDisabled;
	}
}
