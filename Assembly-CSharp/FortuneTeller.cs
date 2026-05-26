using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

// Token: 0x02000695 RID: 1685
public class FortuneTeller : MonoBehaviourPunCallbacks
{
	// Token: 0x060029EF RID: 10735 RVA: 0x000E262C File Offset: 0x000E082C
	private void Awake()
	{
		if (this.changeMaterialsInGreyZone && GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Combine(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Combine(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x060029F0 RID: 10736 RVA: 0x000E26A0 File Offset: 0x000E08A0
	private void OnDestroy()
	{
		if (GreyZoneManager.Instance != null)
		{
			GreyZoneManager instance = GreyZoneManager.Instance;
			instance.OnGreyZoneActivated = (Action)Delegate.Remove(instance.OnGreyZoneActivated, new Action(this.GreyZoneActivated));
			GreyZoneManager instance2 = GreyZoneManager.Instance;
			instance2.OnGreyZoneDeactivated = (Action)Delegate.Remove(instance2.OnGreyZoneDeactivated, new Action(this.GreyZoneDeactivated));
		}
	}

	// Token: 0x060029F1 RID: 10737 RVA: 0x000E270C File Offset: 0x000E090C
	public override void OnEnable()
	{
		base.OnEnable();
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		if (this.button)
		{
			this.button.onPressed += this.HandlePressedButton;
		}
	}

	// Token: 0x060029F2 RID: 10738 RVA: 0x000E274A File Offset: 0x000E094A
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.button)
		{
			this.button.onPressed -= this.HandlePressedButton;
		}
	}

	// Token: 0x060029F3 RID: 10739 RVA: 0x000E2776 File Offset: 0x000E0976
	private void GreyZoneActivated()
	{
		this.boothRenderer.material = this.boothGreyZoneMaterial;
		this.beardRenderer.material = this.beardGreyZoneMaterial;
		this.tellerRenderer.SetMaterials(this.tellerGreyZoneMaterials);
	}

	// Token: 0x060029F4 RID: 10740 RVA: 0x000E27AB File Offset: 0x000E09AB
	private void GreyZoneDeactivated()
	{
		this.boothRenderer.material = this.boothDefaultMaterial;
		this.beardRenderer.material = this.beardDefaultMaterial;
		this.tellerRenderer.SetMaterials(this.tellerDefaultMaterials);
	}

	// Token: 0x060029F5 RID: 10741 RVA: 0x000E27E0 File Offset: 0x000E09E0
	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		base.OnPlayerEnteredRoom(newPlayer);
		if (PhotonNetwork.InRoom && PhotonNetwork.LocalPlayer.IsMasterClient)
		{
			base.photonView.RPC("TriggerUpdateFortuneRPC", newPlayer, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x060029F6 RID: 10742 RVA: 0x000E2844 File Offset: 0x000E0A44
	public override void OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x060029F7 RID: 10743 RVA: 0x000E2844 File Offset: 0x000E0A44
	public override void OnJoinedRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.StartAttractModeMonitor();
		}
	}

	// Token: 0x060029F8 RID: 10744 RVA: 0x000E2853 File Offset: 0x000E0A53
	private void HandlePressedButton(GorillaPressableButton button, bool isLeft)
	{
		if (base.photonView.IsMine)
		{
			this.SendNewFortune();
			return;
		}
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("RequestFortuneRPC", RpcTarget.MasterClient, Array.Empty<object>());
		}
	}

	// Token: 0x060029F9 RID: 10745 RVA: 0x000E2888 File Offset: 0x000E0A88
	[PunRPC]
	private void RequestFortuneRPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "RequestFortune");
		RigContainer rigContainer;
		if (NetworkSystem.Instance.IsMasterClient && info.Sender != null && VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
		{
			CallLimitType<CallLimiter> callLimitType = rigContainer.Rig.fxSettings.callSettings[(int)this.limiterType];
			if (callLimitType.UseNetWorkTime ? callLimitType.CallLimitSettings.CheckCallServerTime(info.SentServerTime) : callLimitType.CallLimitSettings.CheckCallTime(Time.time))
			{
				this.SendNewFortune();
			}
		}
	}

	// Token: 0x060029FA RID: 10746 RVA: 0x000E2914 File Offset: 0x000E0B14
	private void SendNewFortune()
	{
		if (this.playable.time > 0.0 && this.playable.time < this.playable.duration)
		{
			return;
		}
		this.latestFortune = this.results.GetResult();
		this.UpdateFortune(this.latestFortune, true);
		if (PhotonNetwork.InRoom)
		{
			base.photonView.RPC("TriggerNewFortuneRPC", RpcTarget.Others, new object[]
			{
				(int)this.latestFortune.fortuneType,
				this.latestFortune.resultIndex
			});
		}
	}

	// Token: 0x060029FB RID: 10747 RVA: 0x000E29B4 File Offset: 0x000E0BB4
	[PunRPC]
	private void TriggerUpdateFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TriggerUpdateFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			MonkeAgent.instance.SendReport("Sent TriggerUpdateFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerUpdateFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.UpdateFortune(this.latestFortune, false);
	}

	// Token: 0x060029FC RID: 10748 RVA: 0x000E2A30 File Offset: 0x000E0C30
	[PunRPC]
	private void TriggerNewFortuneRPC(int fortuneType, int resultIndex, PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TriggerNewFortune");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			MonkeAgent.instance.SendReport("Sent TriggerNewFortune when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		if (!this.triggerNewFortuneLimiter.CheckCallTime(Time.time))
		{
			return;
		}
		this.latestFortune = new FortuneResults.FortuneResult((FortuneResults.FortuneCategoryType)fortuneType, resultIndex);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
		this.UpdateFortune(this.latestFortune, true);
	}

	// Token: 0x060029FD RID: 10749 RVA: 0x000E2ABC File Offset: 0x000E0CBC
	private void StartAttractModeMonitor()
	{
		if (this.attractModeMonitor == null)
		{
			this.attractModeMonitor = base.StartCoroutine(this.AttractModeMonitor());
		}
	}

	// Token: 0x060029FE RID: 10750 RVA: 0x000E2AD8 File Offset: 0x000E0CD8
	private IEnumerator AttractModeMonitor()
	{
		while (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
		{
			if (Time.time >= this.nextAttractAnimTimestamp)
			{
				this.SendAttractAnim();
			}
			yield return new WaitForSeconds(this.nextAttractAnimTimestamp - Time.time);
		}
		this.attractModeMonitor = null;
		yield break;
	}

	// Token: 0x060029FF RID: 10751 RVA: 0x000E2AE7 File Offset: 0x000E0CE7
	private void SendAttractAnim()
	{
		if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
		{
			base.photonView.RPC("TriggerAttractAnimRPC", RpcTarget.All, Array.Empty<object>());
		}
	}

	// Token: 0x06002A00 RID: 10752 RVA: 0x000E2B10 File Offset: 0x000E0D10
	[PunRPC]
	private void TriggerAttractAnimRPC(PhotonMessageInfo info)
	{
		MonkeAgent.IncrementRPCCall(info, "TriggerAttractAnim");
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			MonkeAgent.instance.SendReport("Sent TriggerAttractAnim when they weren't the master client", info.Sender.UserId, info.Sender.NickName);
			return;
		}
		this.animator.SetTrigger(this.trigger_attract);
		this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
	}

	// Token: 0x06002A01 RID: 10753 RVA: 0x000E2B88 File Offset: 0x000E0D88
	private void UpdateFortune(FortuneResults.FortuneResult result, bool newFortune)
	{
		if (this.results)
		{
			PlayableAsset resultFanfare = this.GetResultFanfare(result.fortuneType);
			if (resultFanfare)
			{
				this.playable.initialTime = (newFortune ? 0.0 : resultFanfare.duration);
				this.playable.Play(resultFanfare, DirectorWrapMode.Hold);
				this.animator.SetTrigger(this.trigger_prediction);
				this.nextAttractAnimTimestamp = Time.time + this.waitDurationBeforeAttractAnim;
			}
		}
	}

	// Token: 0x06002A02 RID: 10754 RVA: 0x000E2C0B File Offset: 0x000E0E0B
	public void ApplyFortuneText()
	{
		this.text.text = this.results.GetResultText(this.latestFortune).ToUpper();
	}

	// Token: 0x06002A03 RID: 10755 RVA: 0x000E2C30 File Offset: 0x000E0E30
	private PlayableAsset GetResultFanfare(FortuneResults.FortuneCategoryType fortuneType)
	{
		foreach (FortuneTeller.FortuneTellerResultFanfare fortuneTellerResultFanfare in this.resultFanfares)
		{
			if (fortuneTellerResultFanfare.type == fortuneType)
			{
				return fortuneTellerResultFanfare.fanfare;
			}
		}
		return null;
	}

	// Token: 0x040036AC RID: 13996
	[SerializeField]
	private FXType limiterType;

	// Token: 0x040036AD RID: 13997
	[SerializeField]
	private FortuneTellerButton button;

	// Token: 0x040036AE RID: 13998
	[SerializeField]
	private TextMeshPro text;

	// Token: 0x040036AF RID: 13999
	[SerializeField]
	private FortuneResults results;

	// Token: 0x040036B0 RID: 14000
	[SerializeField]
	private PlayableDirector playable;

	// Token: 0x040036B1 RID: 14001
	[SerializeField]
	private Animator animator;

	// Token: 0x040036B2 RID: 14002
	[SerializeField]
	private float waitDurationBeforeAttractAnim;

	// Token: 0x040036B3 RID: 14003
	[SerializeField]
	private FortuneTeller.FortuneTellerResultFanfare[] resultFanfares;

	// Token: 0x040036B4 RID: 14004
	[Header("Grey Zone Visuals")]
	[SerializeField]
	private bool changeMaterialsInGreyZone;

	// Token: 0x040036B5 RID: 14005
	[SerializeField]
	private MeshRenderer boothRenderer;

	// Token: 0x040036B6 RID: 14006
	[SerializeField]
	private Material boothDefaultMaterial;

	// Token: 0x040036B7 RID: 14007
	[SerializeField]
	private Material boothGreyZoneMaterial;

	// Token: 0x040036B8 RID: 14008
	[SerializeField]
	private MeshRenderer beardRenderer;

	// Token: 0x040036B9 RID: 14009
	[SerializeField]
	private Material beardDefaultMaterial;

	// Token: 0x040036BA RID: 14010
	[SerializeField]
	private Material beardGreyZoneMaterial;

	// Token: 0x040036BB RID: 14011
	[SerializeField]
	private SkinnedMeshRenderer tellerRenderer;

	// Token: 0x040036BC RID: 14012
	[SerializeField]
	private List<Material> tellerDefaultMaterials;

	// Token: 0x040036BD RID: 14013
	[SerializeField]
	private List<Material> tellerGreyZoneMaterials;

	// Token: 0x040036BE RID: 14014
	private FortuneResults.FortuneResult latestFortune;

	// Token: 0x040036BF RID: 14015
	private CallLimiter triggerNewFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x040036C0 RID: 14016
	private CallLimiter triggerUpdateFortuneLimiter = new CallLimiter(10, 1f, 0.5f);

	// Token: 0x040036C1 RID: 14017
	private AnimHashId trigger_attract = "Attract";

	// Token: 0x040036C2 RID: 14018
	private AnimHashId trigger_prediction = "Prediction";

	// Token: 0x040036C3 RID: 14019
	private float nextAttractAnimTimestamp;

	// Token: 0x040036C4 RID: 14020
	private Coroutine attractModeMonitor;

	// Token: 0x02000696 RID: 1686
	[Serializable]
	public struct FortuneTellerResultFanfare
	{
		// Token: 0x040036C5 RID: 14021
		public FortuneResults.FortuneCategoryType type;

		// Token: 0x040036C6 RID: 14022
		public PlayableAsset fanfare;
	}
}
