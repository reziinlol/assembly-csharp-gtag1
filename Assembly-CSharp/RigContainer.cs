using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GorillaNetworking;
using GorillaTag.Audio;
using Newtonsoft.Json;
using Photon.Voice.PUN;
using PlayFab;
using PlayFab.CloudScriptModels;
using UnityEngine;

// Token: 0x0200083C RID: 2108
[RequireComponent(typeof(VRRig), typeof(VRRigReliableState))]
public class RigContainer : MonoBehaviour
{
	// Token: 0x170004BB RID: 1211
	// (get) Token: 0x06003637 RID: 13879 RVA: 0x0012B5DD File Offset: 0x001297DD
	// (set) Token: 0x06003638 RID: 13880 RVA: 0x0012B5E5 File Offset: 0x001297E5
	public bool Initialized { get; private set; }

	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x06003639 RID: 13881 RVA: 0x0012B5EE File Offset: 0x001297EE
	public VRRig Rig
	{
		get
		{
			return this.vrrig;
		}
	}

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x0600363A RID: 13882 RVA: 0x0012B5F6 File Offset: 0x001297F6
	public VRRigReliableState ReliableState
	{
		get
		{
			return this.reliableState;
		}
	}

	// Token: 0x170004BE RID: 1214
	// (get) Token: 0x0600363B RID: 13883 RVA: 0x0012B5FE File Offset: 0x001297FE
	public Transform SpeakerHead
	{
		get
		{
			return this.speakerHead;
		}
	}

	// Token: 0x170004BF RID: 1215
	// (get) Token: 0x0600363C RID: 13884 RVA: 0x0012B606 File Offset: 0x00129806
	public AudioSource ReplacementVoiceSource
	{
		get
		{
			return this.replacementVoiceSource;
		}
	}

	// Token: 0x170004C0 RID: 1216
	// (get) Token: 0x0600363D RID: 13885 RVA: 0x0012B60E File Offset: 0x0012980E
	public List<LoudSpeakerNetwork> LoudSpeakerNetworks
	{
		get
		{
			return this.loudSpeakerNetworks;
		}
	}

	// Token: 0x170004C1 RID: 1217
	// (get) Token: 0x0600363E RID: 13886 RVA: 0x0012B616 File Offset: 0x00129816
	public LCKSocialCameraFollower LckCococamFollower
	{
		get
		{
			return this.m_lckCococamFollower;
		}
	}

	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x0600363F RID: 13887 RVA: 0x0012B61E File Offset: 0x0012981E
	public LCKSocialCameraFollower LCKTabletFollower
	{
		get
		{
			return this.m_lckTablet;
		}
	}

	// Token: 0x170004C3 RID: 1219
	// (get) Token: 0x06003640 RID: 13888 RVA: 0x0012B626 File Offset: 0x00129826
	// (set) Token: 0x06003641 RID: 13889 RVA: 0x0012B62E File Offset: 0x0012982E
	public PhotonVoiceView Voice
	{
		get
		{
			return this.voiceView;
		}
		set
		{
			if (value == this.voiceView)
			{
				return;
			}
			if (this.voiceView != null)
			{
				this.voiceView.SpeakerInUse.enabled = false;
			}
			this.voiceView = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170004C4 RID: 1220
	// (get) Token: 0x06003642 RID: 13890 RVA: 0x0012B66B File Offset: 0x0012986B
	public NetworkView netView
	{
		get
		{
			return this.vrrig.netView;
		}
	}

	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x06003643 RID: 13891 RVA: 0x0012B678 File Offset: 0x00129878
	public int CachedNetViewID
	{
		get
		{
			return this.m_cachedNetViewID;
		}
	}

	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x06003644 RID: 13892 RVA: 0x0012B680 File Offset: 0x00129880
	// (set) Token: 0x06003645 RID: 13893 RVA: 0x0012B68B File Offset: 0x0012988B
	public bool Muted
	{
		get
		{
			return !this.enableVoice;
		}
		set
		{
			this.enableVoice = !value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170004C7 RID: 1223
	// (get) Token: 0x06003646 RID: 13894 RVA: 0x0012B69D File Offset: 0x0012989D
	// (set) Token: 0x06003647 RID: 13895 RVA: 0x0012B6AA File Offset: 0x001298AA
	public NetPlayer Creator
	{
		get
		{
			return this.vrrig.creator;
		}
		set
		{
			if (this.vrrig.isOfflineVRRig || (this.vrrig.creator != null && this.vrrig.creator.InRoom))
			{
				return;
			}
			this.vrrig.creator = value;
		}
	}

	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x06003648 RID: 13896 RVA: 0x0012B6E5 File Offset: 0x001298E5
	// (set) Token: 0x06003649 RID: 13897 RVA: 0x0012B6ED File Offset: 0x001298ED
	public bool ForceMute
	{
		get
		{
			return this.forceMute;
		}
		set
		{
			this.forceMute = value;
			this.RefreshVoiceChat();
		}
	}

	// Token: 0x170004C9 RID: 1225
	// (get) Token: 0x0600364A RID: 13898 RVA: 0x0012B6FC File Offset: 0x001298FC
	public SphereCollider HeadCollider
	{
		get
		{
			return this.headCollider;
		}
	}

	// Token: 0x170004CA RID: 1226
	// (get) Token: 0x0600364B RID: 13899 RVA: 0x0012B704 File Offset: 0x00129904
	public CapsuleCollider BodyCollider
	{
		get
		{
			return this.bodyCollider;
		}
	}

	// Token: 0x170004CB RID: 1227
	// (get) Token: 0x0600364C RID: 13900 RVA: 0x0012B70C File Offset: 0x0012990C
	public VRRigEvents RigEvents
	{
		get
		{
			return this.rigEvents;
		}
	}

	// Token: 0x0600364D RID: 13901 RVA: 0x0012B714 File Offset: 0x00129914
	public bool GetIsPlayerAutoMuted()
	{
		return this.bPlayerAutoMuted;
	}

	// Token: 0x0600364E RID: 13902 RVA: 0x0012B71C File Offset: 0x0012991C
	public void UpdateAutomuteLevel(string autoMuteLevel)
	{
		if (autoMuteLevel.Equals("LOW", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 1;
		}
		else if (autoMuteLevel.Equals("HIGH", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 0;
		}
		else if (autoMuteLevel.Equals("ERROR", StringComparison.OrdinalIgnoreCase))
		{
			this.playerChatQuality = 2;
		}
		else
		{
			this.playerChatQuality = 2;
		}
		this.RefreshVoiceChat();
	}

	// Token: 0x0600364F RID: 13903 RVA: 0x0012B77B File Offset: 0x0012997B
	private void Awake()
	{
		this.loudSpeakerNetworks = new List<LoudSpeakerNetwork>();
	}

	// Token: 0x06003650 RID: 13904 RVA: 0x0012B788 File Offset: 0x00129988
	private void Start()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.LocalPlayer;
			RoomSystem.JoinedRoomEvent += new Action(this.OnMultiPlayerStarted);
			RoomSystem.LeftRoomEvent += new Action(this.OnReturnedToSinglePlayer);
		}
		else
		{
			this.rigEvents.enableEvent += this.RigPostEnable;
		}
		this.Rig.rigContainer = this;
	}

	// Token: 0x06003651 RID: 13905 RVA: 0x0012B81D File Offset: 0x00129A1D
	private void RigPostEnable(RigContainer _)
	{
		this.vrrig.UpdateName();
	}

	// Token: 0x06003652 RID: 13906 RVA: 0x0012B82A File Offset: 0x00129A2A
	private void OnMultiPlayerStarted()
	{
		if (this.Rig.isOfflineVRRig)
		{
			this.vrrig.creator = NetworkSystem.Instance.GetLocalPlayer();
		}
	}

	// Token: 0x06003653 RID: 13907 RVA: 0x0012B84E File Offset: 0x00129A4E
	private void OnReturnedToSinglePlayer()
	{
		if (this.Rig.isOfflineVRRig)
		{
			RigContainer.CancelAutomuteRequest();
		}
	}

	// Token: 0x06003654 RID: 13908 RVA: 0x0012B864 File Offset: 0x00129A64
	private void OnDisable()
	{
		this.Initialized = false;
		this.enableVoice = true;
		this.voiceView = null;
		base.gameObject.transform.localPosition = Vector3.zero;
		base.gameObject.transform.localRotation = Quaternion.identity;
		this.vrrig.syncPos = base.gameObject.transform.position;
		this.vrrig.syncRotation = base.gameObject.transform.rotation;
		this.forceMute = false;
	}

	// Token: 0x06003655 RID: 13909 RVA: 0x0012B8ED File Offset: 0x00129AED
	internal void InitializeNetwork(NetworkView netView, PhotonVoiceView voiceView, VRRigSerializer vrRigSerializer)
	{
		if (!netView || !voiceView)
		{
			return;
		}
		this.InitializeNetwork_Shared(netView, vrRigSerializer);
		this.Voice = voiceView;
		this.vrrig.voiceAudio = voiceView.SpeakerInUse.GetComponent<AudioSource>();
	}

	// Token: 0x06003656 RID: 13910 RVA: 0x0012B928 File Offset: 0x00129B28
	private void InitializeNetwork_Shared(NetworkView netView, VRRigSerializer vrRigSerializer)
	{
		if (this.vrrig.netView)
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent creating multiple vrrigs", this.Creator.UserId, this.Creator.NickName);
			if (this.vrrig.netView.IsMine)
			{
				NetworkSystem.Instance.NetDestroy(this.vrrig.gameObject);
			}
			else
			{
				this.vrrig.netView.gameObject.SetActive(false);
			}
		}
		this.vrrig.netView = netView;
		this.vrrig.rigSerializer = vrRigSerializer;
		this.vrrig.OwningNetPlayer = NetworkSystem.Instance.GetPlayer(NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject));
		this.m_cachedNetViewID = netView.ViewID;
		if (!this.Initialized)
		{
			this.vrrig.NetInitialize();
			if (GorillaGameManager.instance != null && NetworkSystem.Instance.IsMasterClient)
			{
				int owningPlayerID = NetworkSystem.Instance.GetOwningPlayerID(vrRigSerializer.gameObject);
				bool playerTutorialCompletion = NetworkSystem.Instance.GetPlayerTutorialCompletion(owningPlayerID);
				GorillaGameManager.instance.NewVRRig(netView.Owner, netView.ViewID, playerTutorialCompletion);
			}
			bool isLocal = this.vrrig.OwningNetPlayer.IsLocal;
			if (!this.vrrig.isOfflineVRRig && this.vrrig.InitializedCosmetics)
			{
				netView.SendRPC("RPC_RequestCosmetics", netView.Owner, Array.Empty<object>());
			}
		}
		this.Initialized = true;
		if (!this.vrrig.isOfflineVRRig)
		{
			base.StartCoroutine(RigContainer.QueueAutomute(this.Creator));
		}
	}

	// Token: 0x06003657 RID: 13911 RVA: 0x0012BAC0 File Offset: 0x00129CC0
	private static IEnumerator QueueAutomute(NetPlayer player)
	{
		RigContainer.playersToCheckAutomute.Add(player);
		if (!RigContainer.automuteQueued)
		{
			RigContainer.automuteQueued = true;
			yield return new WaitForSecondsRealtime(1f);
			while (RigContainer.waitingForAutomuteCallback)
			{
				yield return null;
			}
			RigContainer.automuteQueued = false;
			RigContainer.RequestAutomuteSettings();
		}
		yield break;
	}

	// Token: 0x06003658 RID: 13912 RVA: 0x0012BAD0 File Offset: 0x00129CD0
	private static void RequestAutomuteSettings()
	{
		if (RigContainer.playersToCheckAutomute.Count == 0)
		{
			return;
		}
		RigContainer.waitingForAutomuteCallback = true;
		RigContainer.playersToCheckAutomute.RemoveAll((NetPlayer player) => player == null);
		RigContainer.requestedAutomutePlayers = new List<NetPlayer>(RigContainer.playersToCheckAutomute);
		RigContainer.playersToCheckAutomute.Clear();
		string[] value = (from x in RigContainer.requestedAutomutePlayers
		select x.UserId).ToArray<string>();
		foreach (NetPlayer netPlayer in RigContainer.requestedAutomutePlayers)
		{
		}
		ExecuteFunctionRequest executeFunctionRequest = new ExecuteFunctionRequest();
		executeFunctionRequest.Entity = new EntityKey
		{
			Id = PlayFabSettings.staticPlayer.EntityId,
			Type = PlayFabSettings.staticPlayer.EntityType
		};
		executeFunctionRequest.FunctionName = "ShouldUserAutomutePlayer";
		executeFunctionRequest.FunctionParameter = string.Join(",", value);
		PlayFabCloudScriptAPI.ExecuteFunction(executeFunctionRequest, delegate(ExecuteFunctionResult result)
		{
			Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(result.FunctionResult.ToString());
			if (dictionary == null)
			{
				using (List<NetPlayer>.Enumerator enumerator2 = RigContainer.requestedAutomutePlayers.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						NetPlayer netPlayer2 = enumerator2.Current;
						if (netPlayer2 != null)
						{
							RigContainer.ReceiveAutomuteSettings(netPlayer2, "none");
						}
					}
					goto IL_A6;
				}
			}
			foreach (NetPlayer netPlayer3 in RigContainer.requestedAutomutePlayers)
			{
				if (netPlayer3 != null)
				{
					string score;
					if (dictionary.TryGetValue(netPlayer3.UserId, out score))
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, score);
					}
					else
					{
						RigContainer.ReceiveAutomuteSettings(netPlayer3, "none");
					}
				}
			}
			IL_A6:
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, delegate(PlayFabError error)
		{
			foreach (NetPlayer player in RigContainer.requestedAutomutePlayers)
			{
				RigContainer.ReceiveAutomuteSettings(player, "ERROR");
			}
			RigContainer.requestedAutomutePlayers.Clear();
			RigContainer.waitingForAutomuteCallback = false;
		}, null, null);
	}

	// Token: 0x06003659 RID: 13913 RVA: 0x0012BC34 File Offset: 0x00129E34
	private static void CancelAutomuteRequest()
	{
		RigContainer.playersToCheckAutomute.Clear();
		RigContainer.automuteQueued = false;
		if (RigContainer.requestedAutomutePlayers != null)
		{
			RigContainer.requestedAutomutePlayers.Clear();
		}
		RigContainer.waitingForAutomuteCallback = false;
	}

	// Token: 0x0600365A RID: 13914 RVA: 0x0012BC60 File Offset: 0x00129E60
	private static void ReceiveAutomuteSettings(NetPlayer player, string score)
	{
		RigContainer rigContainer;
		VRRigCache.Instance.TryGetVrrig(player, out rigContainer);
		if (rigContainer != null)
		{
			rigContainer.UpdateAutomuteLevel(score);
		}
	}

	// Token: 0x0600365B RID: 13915 RVA: 0x0012BC8C File Offset: 0x00129E8C
	private void ProcessAutomute()
	{
		int @int = PlayerPrefs.GetInt("autoMute", 1);
		this.bPlayerAutoMuted = (!this.hasManualMute && this.playerChatQuality < @int);
	}

	// Token: 0x0600365C RID: 13916 RVA: 0x0012BCC0 File Offset: 0x00129EC0
	public void RefreshVoiceChat()
	{
		if (this.Voice == null)
		{
			return;
		}
		this.ProcessAutomute();
		this.Voice.SpeakerInUse.enabled = (!this.forceMute && this.enableVoice && !this.bPlayerAutoMuted && GorillaComputer.instance.voiceChatOn == "TRUE");
		this.replacementVoiceSource.mute = (this.forceMute || !this.enableVoice || this.bPlayerAutoMuted || GorillaComputer.instance.voiceChatOn == "OFF");
	}

	// Token: 0x0600365D RID: 13917 RVA: 0x0012BD5F File Offset: 0x00129F5F
	public void AddLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		if (this.loudSpeakerNetworks.Contains(network))
		{
			return;
		}
		this.loudSpeakerNetworks.Add(network);
	}

	// Token: 0x0600365E RID: 13918 RVA: 0x0012BD7C File Offset: 0x00129F7C
	public void RemoveLoudSpeakerNetwork(LoudSpeakerNetwork network)
	{
		this.loudSpeakerNetworks.Remove(network);
	}

	// Token: 0x0600365F RID: 13919 RVA: 0x0012BD8C File Offset: 0x00129F8C
	public static void RefreshAllRigVoices()
	{
		RigContainer.staticTempRC = null;
		if (!NetworkSystem.Instance.InRoom || VRRigCache.Instance == null)
		{
			return;
		}
		foreach (NetPlayer targetPlayer in NetworkSystem.Instance.AllNetPlayers)
		{
			if (VRRigCache.Instance.TryGetVrrig(targetPlayer, out RigContainer.staticTempRC))
			{
				RigContainer.staticTempRC.RefreshVoiceChat();
			}
		}
	}

	// Token: 0x040046B4 RID: 18100
	[SerializeField]
	private VRRig vrrig;

	// Token: 0x040046B5 RID: 18101
	[SerializeField]
	private VRRigReliableState reliableState;

	// Token: 0x040046B6 RID: 18102
	[SerializeField]
	private Transform speakerHead;

	// Token: 0x040046B7 RID: 18103
	[SerializeField]
	private AudioSource replacementVoiceSource;

	// Token: 0x040046B8 RID: 18104
	private List<LoudSpeakerNetwork> loudSpeakerNetworks;

	// Token: 0x040046B9 RID: 18105
	[SerializeField]
	private LCKSocialCameraFollower m_lckCococamFollower;

	// Token: 0x040046BA RID: 18106
	[SerializeField]
	private LCKSocialCameraFollower m_lckTablet;

	// Token: 0x040046BB RID: 18107
	private PhotonVoiceView voiceView;

	// Token: 0x040046BC RID: 18108
	private int m_cachedNetViewID;

	// Token: 0x040046BD RID: 18109
	private bool enableVoice = true;

	// Token: 0x040046BE RID: 18110
	private bool forceMute;

	// Token: 0x040046BF RID: 18111
	[SerializeField]
	private SphereCollider headCollider;

	// Token: 0x040046C0 RID: 18112
	[SerializeField]
	private CapsuleCollider bodyCollider;

	// Token: 0x040046C1 RID: 18113
	[SerializeField]
	private VRRigEvents rigEvents;

	// Token: 0x040046C2 RID: 18114
	public bool hasManualMute;

	// Token: 0x040046C3 RID: 18115
	private bool bPlayerAutoMuted;

	// Token: 0x040046C4 RID: 18116
	public int playerChatQuality = 2;

	// Token: 0x040046C5 RID: 18117
	private static List<NetPlayer> playersToCheckAutomute = new List<NetPlayer>();

	// Token: 0x040046C6 RID: 18118
	private static bool automuteQueued = false;

	// Token: 0x040046C7 RID: 18119
	private static List<NetPlayer> requestedAutomutePlayers;

	// Token: 0x040046C8 RID: 18120
	private static bool waitingForAutomuteCallback = false;

	// Token: 0x040046C9 RID: 18121
	private static RigContainer staticTempRC;
}
