using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaLocomotion;
using GorillaLocomotion.Climbing;
using GorillaLocomotion.Gameplay;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.Cosmetics;
using GorillaTag.CosmeticSystem;
using GorillaTagScripts;
using KID.Model;
using Newtonsoft.Json;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using PlayFab;
using PlayFab.ClientModels;
using TagEffects;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Token: 0x020004EB RID: 1259
public class VRRig : MonoBehaviour, IWrappedSerializable, INetworkStruct, IPreDisable, IUserCosmeticsCallback, IGorillaSliceableSimple, ITickSystemPost, IEyeScannable
{
	// Token: 0x06001E8F RID: 7823 RVA: 0x000A2F9C File Offset: 0x000A119C
	private void CosmeticsV2_Awake()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Combine(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
	}

	// Token: 0x06001E90 RID: 7824 RVA: 0x000A2FBE File Offset: 0x000A11BE
	internal void Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics()
	{
		CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs = (Action)Delegate.Remove(CosmeticsV2Spawner_Dirty.OnPostInstantiateAllPrefabs, new Action(this.Handle_CosmeticsV2_OnPostInstantiateAllPrefabs_DoEnableAllCosmetics));
		this.CheckForEarlyAccess();
		this.SetCosmeticsActive(false);
	}

	// Token: 0x17000333 RID: 819
	// (get) Token: 0x06001E91 RID: 7825 RVA: 0x000A2FED File Offset: 0x000A11ED
	// (set) Token: 0x06001E92 RID: 7826 RVA: 0x000A2FFA File Offset: 0x000A11FA
	public Vector3 syncPos
	{
		get
		{
			return this.netSyncPos.CurrentSyncTarget;
		}
		set
		{
			this.netSyncPos.SetNewSyncTarget(value);
		}
	}

	// Token: 0x17000334 RID: 820
	// (get) Token: 0x06001E93 RID: 7827 RVA: 0x000A3008 File Offset: 0x000A1208
	public Material myDefaultSkinMaterialInstance
	{
		get
		{
			return this.bodyRenderer.myDefaultSkinMaterialInstance;
		}
	}

	// Token: 0x17000335 RID: 821
	// (get) Token: 0x06001E94 RID: 7828 RVA: 0x000A3015 File Offset: 0x000A1215
	public List<GameObject> cosmetics
	{
		get
		{
			return CosmeticsV2Spawner_Dirty.RigDataForRig(this).vrRig_cosmetics;
		}
	}

	// Token: 0x17000336 RID: 822
	// (get) Token: 0x06001E95 RID: 7829 RVA: 0x000A3022 File Offset: 0x000A1222
	public List<GameObject> overrideCosmetics
	{
		get
		{
			return CosmeticsV2Spawner_Dirty.RigDataForRig(this).vrRig_override;
		}
	}

	// Token: 0x06001E96 RID: 7830 RVA: 0x000A302F File Offset: 0x000A122F
	internal void SetTaggedBy(VRRig taggingRig)
	{
		this.taggedById = taggingRig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x06001E97 RID: 7831 RVA: 0x000A3042 File Offset: 0x000A1242
	public int CheckCosmeticAge(string pfID)
	{
		if (this._playerOwnedCosmeticsAge.ContainsKey(pfID))
		{
			return this._playerOwnedCosmeticsAge[pfID];
		}
		return 0;
	}

	// Token: 0x17000337 RID: 823
	// (get) Token: 0x06001E98 RID: 7832 RVA: 0x000A3060 File Offset: 0x000A1260
	public HashSet<string> TemporaryCosmetics
	{
		get
		{
			return this._temporaryCosmetics;
		}
	}

	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06001E99 RID: 7833 RVA: 0x000A3068 File Offset: 0x000A1268
	// (set) Token: 0x06001E9A RID: 7834 RVA: 0x000A3070 File Offset: 0x000A1270
	internal bool InitializedCosmetics
	{
		get
		{
			return this.initializedCosmetics;
		}
		set
		{
			this.initializedCosmetics = value;
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06001E9B RID: 7835 RVA: 0x000A3079 File Offset: 0x000A1279
	// (set) Token: 0x06001E9C RID: 7836 RVA: 0x000A3081 File Offset: 0x000A1281
	public CosmeticRefRegistry cosmeticReferences { get; private set; }

	// Token: 0x06001E9D RID: 7837 RVA: 0x000A308A File Offset: 0x000A128A
	public void SetVoiceShiftCosmeticsDirty()
	{
		this.voiceShiftCosmeticsDirty = true;
	}

	// Token: 0x06001E9E RID: 7838 RVA: 0x000A3093 File Offset: 0x000A1293
	public void BreakHandLinks()
	{
		this.leftHandLink.BreakLink();
		this.rightHandLink.BreakLink();
	}

	// Token: 0x06001E9F RID: 7839 RVA: 0x000A30AB File Offset: 0x000A12AB
	public bool IsInHandHoldChainWithOtherPlayer(int otherPlayer)
	{
		return TakeMyHand_HandLink.IsHandInChainWithOtherPlayer(this.leftHandLink, otherPlayer) || TakeMyHand_HandLink.IsHandInChainWithOtherPlayer(this.rightHandLink, otherPlayer);
	}

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x06001EA0 RID: 7840 RVA: 0x000A30C9 File Offset: 0x000A12C9
	// (set) Token: 0x06001EA1 RID: 7841 RVA: 0x000A30D1 File Offset: 0x000A12D1
	public float LastTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x06001EA2 RID: 7842 RVA: 0x000A30DA File Offset: 0x000A12DA
	// (set) Token: 0x06001EA3 RID: 7843 RVA: 0x000A30E2 File Offset: 0x000A12E2
	public float LastHandTouchedGroundAtNetworkTime { get; private set; }

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001EA4 RID: 7844 RVA: 0x000A30EB File Offset: 0x000A12EB
	public bool HasBracelet
	{
		get
		{
			return this.reliableState.HasBracelet;
		}
	}

	// Token: 0x06001EA5 RID: 7845 RVA: 0x000A30F8 File Offset: 0x000A12F8
	public Vector3 GetMouthPosition()
	{
		return this.MouthPosition.position;
	}

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001EA6 RID: 7846 RVA: 0x000A3105 File Offset: 0x000A1305
	// (set) Token: 0x06001EA7 RID: 7847 RVA: 0x000A310D File Offset: 0x000A130D
	public GorillaSkin CurrentCosmeticSkin { get; set; }

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001EA8 RID: 7848 RVA: 0x000A3116 File Offset: 0x000A1316
	// (set) Token: 0x06001EA9 RID: 7849 RVA: 0x000A311E File Offset: 0x000A131E
	public GorillaSkin CurrentModeSkin { get; set; }

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001EAA RID: 7850 RVA: 0x000A3127 File Offset: 0x000A1327
	// (set) Token: 0x06001EAB RID: 7851 RVA: 0x000A312F File Offset: 0x000A132F
	public GorillaSkin TemporaryEffectSkin { get; set; }

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06001EAC RID: 7852 RVA: 0x000A3138 File Offset: 0x000A1338
	// (set) Token: 0x06001EAD RID: 7853 RVA: 0x000A3140 File Offset: 0x000A1340
	public bool PostTickRunning { get; set; }

	// Token: 0x06001EAE RID: 7854 RVA: 0x000A3149 File Offset: 0x000A1349
	public VRRig.PartyMemberStatus GetPartyMemberStatus()
	{
		if (this.partyMemberStatus == VRRig.PartyMemberStatus.NeedsUpdate)
		{
			this.partyMemberStatus = (FriendshipGroupDetection.Instance.IsInMyGroup(this.creator.UserId) ? VRRig.PartyMemberStatus.InLocalParty : VRRig.PartyMemberStatus.NotInLocalParty);
		}
		return this.partyMemberStatus;
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06001EAF RID: 7855 RVA: 0x000A317A File Offset: 0x000A137A
	public bool IsLocalPartyMember
	{
		get
		{
			return this.GetPartyMemberStatus() != VRRig.PartyMemberStatus.NotInLocalParty;
		}
	}

	// Token: 0x06001EB0 RID: 7856 RVA: 0x000A3188 File Offset: 0x000A1388
	public void ClearPartyMemberStatus()
	{
		this.partyMemberStatus = VRRig.PartyMemberStatus.NeedsUpdate;
	}

	// Token: 0x06001EB1 RID: 7857 RVA: 0x000A3191 File Offset: 0x000A1391
	public int ActiveTransferrableObjectIndex(int idx)
	{
		return this.reliableState.activeTransferrableObjectIndex[idx];
	}

	// Token: 0x06001EB2 RID: 7858 RVA: 0x000A31A0 File Offset: 0x000A13A0
	public int ActiveTransferrableObjectIndexLength()
	{
		return this.reliableState.activeTransferrableObjectIndex.Length;
	}

	// Token: 0x06001EB3 RID: 7859 RVA: 0x000A31AF File Offset: 0x000A13AF
	public void SetActiveTransferrableObjectIndex(int idx, int v)
	{
		if (this.reliableState.activeTransferrableObjectIndex[idx] != v)
		{
			this.reliableState.activeTransferrableObjectIndex[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EB4 RID: 7860 RVA: 0x000A31DA File Offset: 0x000A13DA
	public TransferrableObject.PositionState TransferrablePosStates(int idx)
	{
		return this.reliableState.transferrablePosStates[idx];
	}

	// Token: 0x06001EB5 RID: 7861 RVA: 0x000A31E9 File Offset: 0x000A13E9
	public void SetTransferrablePosStates(int idx, TransferrableObject.PositionState v)
	{
		if (this.reliableState.transferrablePosStates[idx] != v)
		{
			this.reliableState.transferrablePosStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EB6 RID: 7862 RVA: 0x000A3214 File Offset: 0x000A1414
	public TransferrableObject.ItemStates TransferrableItemStates(int idx)
	{
		return this.reliableState.transferrableItemStates[idx];
	}

	// Token: 0x06001EB7 RID: 7863 RVA: 0x000A3223 File Offset: 0x000A1423
	public void SetTransferrableItemStates(int idx, TransferrableObject.ItemStates v)
	{
		if (this.reliableState.transferrableItemStates[idx] != v)
		{
			this.reliableState.transferrableItemStates[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EB8 RID: 7864 RVA: 0x000A324E File Offset: 0x000A144E
	public void SetTransferrableDockPosition(int idx, BodyDockPositions.DropPositions v)
	{
		if (this.reliableState.transferableDockPositions[idx] != v)
		{
			this.reliableState.transferableDockPositions[idx] = v;
			this.reliableState.SetIsDirty();
		}
	}

	// Token: 0x06001EB9 RID: 7865 RVA: 0x000A3279 File Offset: 0x000A1479
	public BodyDockPositions.DropPositions TransferrableDockPosition(int idx)
	{
		return this.reliableState.transferableDockPositions[idx];
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06001EBA RID: 7866 RVA: 0x000A3288 File Offset: 0x000A1488
	// (set) Token: 0x06001EBB RID: 7867 RVA: 0x000A3295 File Offset: 0x000A1495
	public int WearablePackedStates
	{
		get
		{
			return this.reliableState.wearablesPackedStates;
		}
		set
		{
			if (this.reliableState.wearablesPackedStates != value)
			{
				this.reliableState.wearablesPackedStates = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000343 RID: 835
	// (get) Token: 0x06001EBC RID: 7868 RVA: 0x000A32BC File Offset: 0x000A14BC
	// (set) Token: 0x06001EBD RID: 7869 RVA: 0x000A32C9 File Offset: 0x000A14C9
	public int LeftThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.lThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.lThrowableProjectileIndex != value)
			{
				this.reliableState.lThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000344 RID: 836
	// (get) Token: 0x06001EBE RID: 7870 RVA: 0x000A32F0 File Offset: 0x000A14F0
	// (set) Token: 0x06001EBF RID: 7871 RVA: 0x000A32FD File Offset: 0x000A14FD
	public int RightThrowableProjectileIndex
	{
		get
		{
			return this.reliableState.rThrowableProjectileIndex;
		}
		set
		{
			if (this.reliableState.rThrowableProjectileIndex != value)
			{
				this.reliableState.rThrowableProjectileIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001EC0 RID: 7872 RVA: 0x000A3324 File Offset: 0x000A1524
	// (set) Token: 0x06001EC1 RID: 7873 RVA: 0x000A3331 File Offset: 0x000A1531
	public Color32 LeftThrowableProjectileColor
	{
		get
		{
			return this.reliableState.lThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.lThrowableProjectileColor.Equals(value))
			{
				this.reliableState.lThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000346 RID: 838
	// (get) Token: 0x06001EC2 RID: 7874 RVA: 0x000A335D File Offset: 0x000A155D
	// (set) Token: 0x06001EC3 RID: 7875 RVA: 0x000A336A File Offset: 0x000A156A
	public Color32 RightThrowableProjectileColor
	{
		get
		{
			return this.reliableState.rThrowableProjectileColor;
		}
		set
		{
			if (!this.reliableState.rThrowableProjectileColor.Equals(value))
			{
				this.reliableState.rThrowableProjectileColor = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001EC4 RID: 7876 RVA: 0x000A3396 File Offset: 0x000A1596
	public Color32 GetThrowableProjectileColor(bool isLeftHand)
	{
		if (!isLeftHand)
		{
			return this.RightThrowableProjectileColor;
		}
		return this.LeftThrowableProjectileColor;
	}

	// Token: 0x06001EC5 RID: 7877 RVA: 0x000A33A8 File Offset: 0x000A15A8
	public void SetThrowableProjectileColor(bool isLeftHand, Color32 color)
	{
		if (isLeftHand)
		{
			this.LeftThrowableProjectileColor = color;
			return;
		}
		this.RightThrowableProjectileColor = color;
	}

	// Token: 0x06001EC6 RID: 7878 RVA: 0x000A33BC File Offset: 0x000A15BC
	public void SetRandomThrowableModelIndex(int randModelIndex)
	{
		this.RandomThrowableIndex = randModelIndex;
	}

	// Token: 0x06001EC7 RID: 7879 RVA: 0x000A33C5 File Offset: 0x000A15C5
	public int GetRandomThrowableModelIndex()
	{
		return this.RandomThrowableIndex;
	}

	// Token: 0x17000347 RID: 839
	// (get) Token: 0x06001EC8 RID: 7880 RVA: 0x000A33CD File Offset: 0x000A15CD
	// (set) Token: 0x06001EC9 RID: 7881 RVA: 0x000A33DA File Offset: 0x000A15DA
	private int RandomThrowableIndex
	{
		get
		{
			return this.reliableState.randomThrowableIndex;
		}
		set
		{
			if (this.reliableState.randomThrowableIndex != value)
			{
				this.reliableState.randomThrowableIndex = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000348 RID: 840
	// (get) Token: 0x06001ECA RID: 7882 RVA: 0x000A3401 File Offset: 0x000A1601
	// (set) Token: 0x06001ECB RID: 7883 RVA: 0x000A340E File Offset: 0x000A160E
	public bool IsMicEnabled
	{
		get
		{
			return this.reliableState.isMicEnabled;
		}
		set
		{
			if (this.reliableState.isMicEnabled != value)
			{
				this.reliableState.isMicEnabled = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x17000349 RID: 841
	// (get) Token: 0x06001ECC RID: 7884 RVA: 0x000A3435 File Offset: 0x000A1635
	// (set) Token: 0x06001ECD RID: 7885 RVA: 0x000A3442 File Offset: 0x000A1642
	public int SizeLayerMask
	{
		get
		{
			return this.reliableState.sizeLayerMask;
		}
		set
		{
			if (this.reliableState.sizeLayerMask != value)
			{
				this.reliableState.sizeLayerMask = value;
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x1700034A RID: 842
	// (get) Token: 0x06001ECE RID: 7886 RVA: 0x000A3469 File Offset: 0x000A1669
	public float scaleFactor
	{
		get
		{
			return this.scaleMultiplier * this.nativeScale;
		}
	}

	// Token: 0x1700034B RID: 843
	// (get) Token: 0x06001ECF RID: 7887 RVA: 0x000A3478 File Offset: 0x000A1678
	// (set) Token: 0x06001ED0 RID: 7888 RVA: 0x000A3480 File Offset: 0x000A1680
	public float ScaleMultiplier
	{
		get
		{
			return this.scaleMultiplier;
		}
		set
		{
			this.scaleMultiplier = value;
		}
	}

	// Token: 0x1700034C RID: 844
	// (get) Token: 0x06001ED1 RID: 7889 RVA: 0x000A3489 File Offset: 0x000A1689
	// (set) Token: 0x06001ED2 RID: 7890 RVA: 0x000A3491 File Offset: 0x000A1691
	public float NativeScale
	{
		get
		{
			return this.nativeScale;
		}
		set
		{
			this.nativeScale = value;
		}
	}

	// Token: 0x1700034D RID: 845
	// (get) Token: 0x06001ED3 RID: 7891 RVA: 0x000A349A File Offset: 0x000A169A
	public NetPlayer Creator
	{
		get
		{
			return this.creator;
		}
	}

	// Token: 0x1700034E RID: 846
	// (get) Token: 0x06001ED4 RID: 7892 RVA: 0x000A34A2 File Offset: 0x000A16A2
	internal bool Initialized
	{
		get
		{
			return this.initialized;
		}
	}

	// Token: 0x1700034F RID: 847
	// (get) Token: 0x06001ED5 RID: 7893 RVA: 0x000A34AA File Offset: 0x000A16AA
	// (set) Token: 0x06001ED6 RID: 7894 RVA: 0x000A34B2 File Offset: 0x000A16B2
	public float SpeakingLoudness
	{
		get
		{
			return this.speakingLoudness;
		}
		set
		{
			this.speakingLoudness = value;
		}
	}

	// Token: 0x17000350 RID: 848
	// (get) Token: 0x06001ED7 RID: 7895 RVA: 0x000A34BB File Offset: 0x000A16BB
	internal HandEffectContext LeftHandEffect
	{
		get
		{
			return this._leftHandEffect;
		}
	}

	// Token: 0x17000351 RID: 849
	// (get) Token: 0x06001ED8 RID: 7896 RVA: 0x000A34C3 File Offset: 0x000A16C3
	internal HandEffectContext RightHandEffect
	{
		get
		{
			return this._rightHandEffect;
		}
	}

	// Token: 0x17000352 RID: 850
	// (get) Token: 0x06001ED9 RID: 7897 RVA: 0x000A34CB File Offset: 0x000A16CB
	internal HandEffectContext ExtraLeftHandEffect
	{
		get
		{
			return this._extraLeftHandEffect;
		}
	}

	// Token: 0x17000353 RID: 851
	// (get) Token: 0x06001EDA RID: 7898 RVA: 0x000A34D3 File Offset: 0x000A16D3
	internal HandEffectContext ExtraRightHandEffect
	{
		get
		{
			return this._extraRightHandEffect;
		}
	}

	// Token: 0x17000354 RID: 852
	// (get) Token: 0x06001EDB RID: 7899 RVA: 0x000A34DB File Offset: 0x000A16DB
	public GamePlayer GamePlayerRef
	{
		get
		{
			if (this._gamePlayerRef == null)
			{
				this._gamePlayerRef = base.GetComponent<GamePlayer>();
			}
			return this._gamePlayerRef;
		}
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x000A3500 File Offset: 0x000A1700
	public void BuildInitialize()
	{
		this.fxSettings = Object.Instantiate<FXSystemSettings>(this.sharedFXSettings);
		this.fxSettings.forLocalRig = this.isOfflineVRRig;
		this.lastPosition = base.transform.position;
		if (!this.isOfflineVRRig)
		{
			base.transform.parent = null;
		}
		SizeManager component = base.GetComponent<SizeManager>();
		if (component != null)
		{
			component.BuildInitialize();
		}
		this.myMouthFlap = base.GetComponent<GorillaMouthFlap>();
		this.mySpeakerLoudness = base.GetComponent<GorillaSpeakerLoudness>();
		if (this.myReplacementVoice == null)
		{
			this.myReplacementVoice = base.GetComponentInChildren<ReplacementVoice>();
		}
		this.myEyeExpressions = base.GetComponent<GorillaEyeExpressions>();
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000A35A4 File Offset: 0x000A17A4
	private void Awake()
	{
		this.cosmeticsObjectRegistry = new CosmeticItemRegistry(this);
		this.CosmeticsV2_Awake();
		PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
		instance.OnSafetyUpdate = (Action<bool>)Delegate.Combine(instance.OnSafetyUpdate, new Action<bool>(this.UpdateNameSafeAccount));
		if (this.isOfflineVRRig)
		{
			VRRig.gLocalRig = this;
			this.BuildInitialize();
		}
		this.SharedStart();
	}

	// Token: 0x06001EDE RID: 7902 RVA: 0x000A3608 File Offset: 0x000A1808
	private void ApplyColorCode()
	{
		float defaultValue = 0f;
		float @float = PlayerPrefs.GetFloat("redValue", defaultValue);
		float float2 = PlayerPrefs.GetFloat("greenValue", defaultValue);
		float float3 = PlayerPrefs.GetFloat("blueValue", defaultValue);
		GorillaTagger.Instance.UpdateColor(@float, float2, float3);
	}

	// Token: 0x06001EDF RID: 7903 RVA: 0x000A364C File Offset: 0x000A184C
	private void SharedStart()
	{
		if (this.isInitialized)
		{
			return;
		}
		this.lastScaleFactor = this.scaleFactor;
		this.isInitialized = true;
		this.myBodyDockPositions = base.GetComponent<BodyDockPositions>();
		this.reliableState.SharedStart(this.isOfflineVRRig, this.myBodyDockPositions);
		this.bodyRenderer.SharedStart();
		this.initialized = false;
		if (this.isOfflineVRRig)
		{
			if (CosmeticsController.hasInstance && CosmeticsController.instance.v2_allCosmeticsInfoAssetRef_isLoaded)
			{
				CosmeticsController.instance.currentWornSet.LoadFromPlayerPreferences(CosmeticsController.instance);
			}
			if (Application.platform == RuntimePlatform.Android && this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.initialized = true;
		}
		else if (!this.isOfflineVRRig)
		{
			if (this.spectatorSkin != null)
			{
				Object.Destroy(this.spectatorSkin);
			}
			this.head.syncPos = -this.headBodyOffset;
		}
		GorillaSkin.ShowActiveSkin(this);
		base.Invoke("ApplyColorCode", 1f);
		List<Material> m = new List<Material>();
		this.mainSkin.GetSharedMaterials(m);
		this.layerChanger = base.GetComponent<LayerChanger>();
		if (this.layerChanger != null)
		{
			this.layerChanger.InitializeLayers(base.transform);
		}
		this.frozenEffectMinY = this.frozenEffect.transform.localScale.y;
		this.frozenEffectMinHorizontalScale = this.frozenEffect.transform.localScale.x;
		this.rightIndex.Initialize();
		this.rightMiddle.Initialize();
		this.rightThumb.Initialize();
		this.leftIndex.Initialize();
		this.leftMiddle.Initialize();
		this.leftThumb.Initialize();
		this.cachedRenderTransformPos = this.renderTransform.localPosition;
	}

	// Token: 0x06001EE0 RID: 7904 RVA: 0x000A3820 File Offset: 0x000A1A20
	public void SliceUpdate()
	{
		float time = Time.time;
		if (this._nextUpdateTime < 0f)
		{
			this._nextUpdateTime = time + 1f;
			return;
		}
		if (time < this._nextUpdateTime)
		{
			return;
		}
		this._nextUpdateTime = time + 1f;
		if (RoomSystem.JoinedRoom && NetworkSystem.Instance.IsMasterClient && GorillaGameModes.GameMode.ActiveNetworkHandler.IsNull())
		{
			GorillaGameModes.GameMode.LoadGameModeFromProperty();
		}
	}

	// Token: 0x06001EE1 RID: 7905 RVA: 0x000A388C File Offset: 0x000A1A8C
	public bool IsItemAllowed(string itemName)
	{
		if (itemName == "Slingshot")
		{
			return NetworkSystem.Instance.InRoom && GorillaGameManager.instance is GorillaPaintbrawlManager;
		}
		if (BuilderSetManager.instance.GetStarterSetsConcat().Contains(itemName))
		{
			return true;
		}
		if (this._playerOwnedCosmetics.Contains(itemName) || PlayerCosmeticsSystem.IsTemporaryCosmeticAllowed(this, itemName))
		{
			return true;
		}
		bool canTryOn = CosmeticsController.instance.GetItemFromDict(itemName).canTryOn;
		return this.inTryOnRoom && canTryOn;
	}

	// Token: 0x06001EE2 RID: 7906 RVA: 0x000A3911 File Offset: 0x000A1B11
	public void ApplyLocalTrajectoryOverride(Vector3 overrideVelocity)
	{
		this.LocalTrajectoryOverrideBlend = 1f;
		this.LocalTrajectoryOverridePosition = base.transform.position;
		this.LocalTrajectoryOverrideVelocity = overrideVelocity;
	}

	// Token: 0x06001EE3 RID: 7907 RVA: 0x000A3936 File Offset: 0x000A1B36
	public bool IsLocalTrajectoryOverrideActive()
	{
		return this.LocalTrajectoryOverrideBlend > 0f;
	}

	// Token: 0x06001EE4 RID: 7908 RVA: 0x000A3945 File Offset: 0x000A1B45
	public void ApplyLocalGrabOverride(bool isBody, bool isLeftHand, Transform grabbingHand)
	{
		this.localOverrideIsBody = isBody;
		this.localOverrideIsLeftHand = isLeftHand;
		this.localOverrideGrabbingHand = grabbingHand;
		this.localGrabOverrideBlend = 1f;
	}

	// Token: 0x06001EE5 RID: 7909 RVA: 0x000A3967 File Offset: 0x000A1B67
	public void ClearLocalGrabOverride()
	{
		this.localGrabOverrideBlend = -1f;
	}

	// Token: 0x06001EE6 RID: 7910 RVA: 0x000A3974 File Offset: 0x000A1B74
	public void RemoteRigUpdate()
	{
		if (this.scaleFactor != this.lastScaleFactor)
		{
			this.ScaleUpdate();
		}
		if (this.voiceAudio != null)
		{
			float? num = null;
			float? num2 = null;
			if (this.IsHaunted)
			{
				num = new float?(this.HauntedVoicePitch);
			}
			else if (this.UsingHauntedRing)
			{
				num = new float?(this.HauntedRingVoicePitch);
			}
			else
			{
				if (this.voiceShiftCosmeticsDirty)
				{
					this.cosmeticPitchShift = 0f;
					this.cosmeticVolumeShift = 0f;
					this.anyShiftedVoiceCosmetic = false;
					int num3 = 0;
					int num4 = 0;
					for (int i = 0; i < this.VoiceShiftCosmetics.Count; i++)
					{
						VoiceShiftCosmetic voiceShiftCosmetic = this.VoiceShiftCosmetics[i];
						if (voiceShiftCosmetic.IsShifted)
						{
							this.anyShiftedVoiceCosmetic = true;
							if (voiceShiftCosmetic.ModifyPitch)
							{
								this.cosmeticPitchShift += voiceShiftCosmetic.Pitch;
								num3++;
							}
							if (voiceShiftCosmetic.ModifyVolume)
							{
								this.cosmeticVolumeShift += voiceShiftCosmetic.Volume;
								num4++;
							}
						}
					}
					this.cosmeticPitchActive = (num3 > 0);
					this.cosmeticVolumeActive = (num4 > 0);
					if (this.cosmeticPitchActive)
					{
						this.cosmeticPitchShift /= (float)num3;
					}
					if (this.cosmeticVolumeActive)
					{
						this.cosmeticVolumeShift /= (float)num4;
					}
					this.voiceShiftCosmeticsDirty = false;
				}
				if (this.anyShiftedVoiceCosmetic)
				{
					if (this.cosmeticPitchActive)
					{
						num = new float?(this.cosmeticPitchShift);
					}
					if (this.cosmeticVolumeActive)
					{
						num2 = new float?(this.cosmeticVolumeShift);
					}
				}
				else
				{
					float time = GorillaTagger.Instance.offlineVRRig.scaleFactor / this.scaleFactor;
					float num5 = this.voicePitchForRelativeScale.Evaluate(time);
					if (float.IsNaN(num5) || num5 <= 0f)
					{
						Debug.LogError("Voice pitch curve is invalid, please fix!");
					}
					else
					{
						num = new float?(num5);
					}
				}
			}
			if (num != null && !Mathf.Approximately(this.voiceAudio.pitch, num.Value))
			{
				this.voiceAudio.pitch = num.Value;
			}
			if (num2 != null && !Mathf.Approximately(this.voiceAudio.volume, num2.Value))
			{
				this.voiceAudio.volume = num2.Value;
			}
		}
		this.jobPos = base.transform.position;
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobPos = Vector3.Lerp(base.transform.position, this.SanitizeVector3(this.syncPos), this.lerpValueBody * 0.66f);
			if (this.currentRopeSwing && this.currentRopeSwingTarget)
			{
				Vector3 b;
				if (this.grabbedRopeIsLeft)
				{
					b = this.currentRopeSwingTarget.position - this.leftHandTransform.position;
				}
				else
				{
					b = this.currentRopeSwingTarget.position - this.rightHandTransform.position;
				}
				if (this.shouldLerpToRope)
				{
					this.jobPos += Vector3.Lerp(Vector3.zero, b, this.lastRopeGrabTimer * 4f);
					if (this.lastRopeGrabTimer < 1f)
					{
						this.lastRopeGrabTimer += Time.deltaTime;
					}
				}
				else
				{
					this.jobPos += b;
				}
			}
			else if (this.currentHoldParent)
			{
				Transform transform;
				if (this.grabbedRopeIsBody)
				{
					transform = this.bodyTransform;
				}
				else if (this.grabbedRopeIsLeft)
				{
					transform = this.leftHandTransform;
				}
				else
				{
					transform = this.rightHandTransform;
				}
				this.jobPos += this.currentHoldParent.TransformPoint(this.grabbedRopeOffset) - transform.position;
			}
			else if (this.mountedMonkeBlock || this.mountedMovingSurface)
			{
				Transform transform2 = this.movingSurfaceIsMonkeBlock ? this.mountedMonkeBlock.transform : this.mountedMovingSurface.transform;
				Vector3 b2 = Vector3.zero;
				Vector3 b3 = this.jobPos - base.transform.position;
				Transform transform3;
				if (this.mountedMovingSurfaceIsBody)
				{
					transform3 = this.bodyTransform;
				}
				else if (this.mountedMovingSurfaceIsLeft)
				{
					transform3 = this.leftHandTransform;
				}
				else
				{
					transform3 = this.rightHandTransform;
				}
				b2 = transform2.TransformPoint(this.mountedMonkeBlockOffset) - (transform3.position + b3);
				if (this.shouldLerpToMovingSurface)
				{
					this.lastMountedSurfaceTimer += Time.deltaTime;
					this.jobPos += Vector3.Lerp(Vector3.zero, b2, this.lastMountedSurfaceTimer * 4f);
					if (this.lastMountedSurfaceTimer * 4f >= 1f)
					{
						this.shouldLerpToMovingSurface = false;
					}
				}
				else
				{
					this.jobPos += b2;
				}
			}
			else if (this.InOverrideSubscriptionZone)
			{
				this.jobPos = this.OverrideSubscriptionZoneLocation;
			}
		}
		else
		{
			this.jobPos = this.SanitizeVector3(this.syncPos);
		}
		if (this.LocalTrajectoryOverrideBlend > 0f)
		{
			this.LocalTrajectoryOverrideBlend -= Time.deltaTime / this.LocalTrajectoryOverrideDuration;
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			Vector3 localTrajectoryOverrideVelocity;
			Vector3 localTrajectoryOverridePosition;
			if (this.LocalTestMovementCollision(this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideVelocity, out localTrajectoryOverrideVelocity, out localTrajectoryOverridePosition))
			{
				this.LocalTrajectoryOverrideVelocity = localTrajectoryOverrideVelocity;
				this.LocalTrajectoryOverridePosition = localTrajectoryOverridePosition;
			}
			else
			{
				this.LocalTrajectoryOverridePosition += this.LocalTrajectoryOverrideVelocity * Time.deltaTime;
			}
			this.LocalTrajectoryOverrideVelocity += Physics.gravity * Time.deltaTime * 0.5f;
			this.jobPos = Vector3.Lerp(this.jobPos, this.LocalTrajectoryOverridePosition, this.LocalTrajectoryOverrideBlend);
		}
		else if (this.localGrabOverrideBlend > 0f)
		{
			this.localGrabOverrideBlend -= Time.deltaTime / this.LocalGrabOverrideDuration;
			if (this.localOverrideGrabbingHand != null)
			{
				Transform transform4;
				if (this.localOverrideIsBody)
				{
					transform4 = this.bodyTransform;
				}
				else if (this.localOverrideIsLeftHand)
				{
					transform4 = this.leftHandTransform;
				}
				else
				{
					transform4 = this.rightHandTransform;
				}
				this.jobPos += this.localOverrideGrabbingHand.TransformPoint(this.grabbedRopeOffset) - transform4.position;
			}
		}
		if (Time.time > this.timeSpawned + this.doNotLerpConstant)
		{
			this.jobRotation = Quaternion.Lerp(base.transform.rotation, this.SanitizeQuaternion(this.syncRotation), this.lerpValueBody);
		}
		else
		{
			this.jobRotation = this.SanitizeQuaternion(this.syncRotation);
		}
		this.head.syncPos = base.transform.rotation * -this.headBodyOffset * this.scaleFactor;
		this.head.MapOther(this.lerpValueBody);
		this.rightHand.MapOther(this.lerpValueBody);
		this.leftHand.MapOther(this.lerpValueBody);
		this.rightIndex.MapOtherFinger((float)(this.handSync % 10) / 10f, this.lerpValueFingers);
		this.rightMiddle.MapOtherFinger((float)(this.handSync % 100) / 100f, this.lerpValueFingers);
		this.rightThumb.MapOtherFinger((float)(this.handSync % 1000) / 1000f, this.lerpValueFingers);
		this.leftIndex.MapOtherFinger((float)(this.handSync % 10000) / 10000f, this.lerpValueFingers);
		this.leftMiddle.MapOtherFinger((float)(this.handSync % 100000) / 100000f, this.lerpValueFingers);
		this.leftThumb.MapOtherFinger((float)(this.handSync % 1000000) / 1000000f, this.lerpValueFingers);
		this.leftHandHoldableStatus = this.handSync % 10000000 / 1000000;
		this.rightHandHoldableStatus = this.handSync % 100000000 / 10000000;
	}

	// Token: 0x06001EE7 RID: 7911 RVA: 0x000A41D0 File Offset: 0x000A23D0
	private void ScaleUpdate()
	{
		this.frameScale = Mathf.MoveTowards(this.lastScaleFactor, this.scaleFactor, Time.deltaTime * 4f);
		base.transform.localScale = Vector3.one * this.frameScale;
		this.lastScaleFactor = this.frameScale;
	}

	// Token: 0x06001EE8 RID: 7912 RVA: 0x000A4226 File Offset: 0x000A2426
	public void AddLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Add(action);
	}

	// Token: 0x06001EE9 RID: 7913 RVA: 0x000A4235 File Offset: 0x000A2435
	public void RemoveLateUpdateCallback(ICallBack action)
	{
		this.lateUpdateCallbacks.Remove(action);
	}

	// Token: 0x06001EEA RID: 7914 RVA: 0x000A4248 File Offset: 0x000A2448
	public void PostTick()
	{
		GTPlayer instance = GTPlayer.Instance;
		if (this.isOfflineVRRig)
		{
			if (GorillaGameManager.instance != null)
			{
				this.speedArray = GorillaGameManager.instance.LocalPlayerSpeed();
				instance.jumpMultiplier = this.speedArray[1];
				instance.maxJumpSpeed = this.speedArray[0];
			}
			else
			{
				instance.jumpMultiplier = 1.1f;
				instance.maxJumpSpeed = 6.5f;
			}
			this.nativeScale = instance.NativeScale;
			this.scaleMultiplier = instance.ScaleMultiplier;
			if (this.scaleFactor != this.lastScaleFactor)
			{
				this.ScaleUpdate();
			}
			this.syncPos = this.mainCamera.transform.position + this.headConstraint.rotation * this.head.trackingPositionOffset * this.lastScaleFactor + base.transform.rotation * this.headBodyOffset * this.lastScaleFactor;
			base.transform.SetPositionAndRotation(this.syncPos, GTPlayerTransform.BodyRotation);
			this.head.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.leftHand.MapMine(this.lastScaleFactor, this.playerOffsetTransform);
			this.rightIndex.MapMyFinger(this.lerpValueFingers);
			this.rightMiddle.MapMyFinger(this.lerpValueFingers);
			this.rightThumb.MapMyFinger(this.lerpValueFingers);
			this.leftIndex.MapMyFinger(this.lerpValueFingers);
			this.leftMiddle.MapMyFinger(this.lerpValueFingers);
			this.leftThumb.MapMyFinger(this.lerpValueFingers);
			bool isGroundedHand = instance.IsGroundedHand || instance.IsThrusterActive;
			bool isGroundedButt = instance.IsGroundedButt;
			bool isLeftGrabbing = EquipmentInteractor.instance.isLeftGrabbing;
			bool isReadyForGrabbing = isLeftGrabbing && EquipmentInteractor.instance.CanGrabLeft();
			bool isRightGrabbing = EquipmentInteractor.instance.isRightGrabbing;
			bool isReadyForGrabbing2 = isRightGrabbing && EquipmentInteractor.instance.CanGrabRight();
			this.LastTouchedGroundAtNetworkTime = instance.LastTouchedGroundAtNetworkTime;
			this.LastHandTouchedGroundAtNetworkTime = instance.LastHandTouchedGroundAtNetworkTime;
			TakeMyHand_HandLink takeMyHand_HandLink = this.leftHandLink;
			if (takeMyHand_HandLink != null)
			{
				takeMyHand_HandLink.LocalUpdate(isGroundedHand, isGroundedButt, isLeftGrabbing, isReadyForGrabbing);
			}
			TakeMyHand_HandLink takeMyHand_HandLink2 = this.rightHandLink;
			if (takeMyHand_HandLink2 != null)
			{
				takeMyHand_HandLink2.LocalUpdate(isGroundedHand, isGroundedButt, isRightGrabbing, isReadyForGrabbing2);
			}
			if (GorillaTagger.Instance.loadedDeviceName == "Oculus")
			{
				this.mainSkin.enabled = OVRManager.hasInputFocus;
			}
			this.bodyRenderer.ActiveBody.enabled = !instance.inOverlay;
			int i = this.loudnessCheckFrame - 1;
			this.loudnessCheckFrame = i;
			if (i < 0)
			{
				this.SpeakingLoudness = 0f;
				if (this.shouldSendSpeakingLoudness && this.netView)
				{
					PhotonVoiceView component = this.netView.GetComponent<PhotonVoiceView>();
					if (component && component.RecorderInUse)
					{
						MicWrapper micWrapper = component.RecorderInUse.InputSource as MicWrapper;
						if (micWrapper != null)
						{
							int num = this.replacementVoiceDetectionDelay;
							if (num > this.voiceSampleBuffer.Length)
							{
								Array.Resize<float>(ref this.voiceSampleBuffer, num);
							}
							float[] array = this.voiceSampleBuffer;
							if (micWrapper.Mic != null && micWrapper.Mic.samples >= num && micWrapper.Mic.GetData(array, micWrapper.Mic.samples - num))
							{
								float num2 = 0f;
								for (int j = 0; j < num; j++)
								{
									float num3 = Mathf.Sqrt(array[j]);
									if (num3 > num2)
									{
										num2 = num3;
									}
								}
								this.SpeakingLoudness = num2;
							}
						}
					}
				}
				this.loudnessCheckFrame = 10;
			}
			if (PhotonNetwork.InRoom && Time.time > this.nextLocalVelocityStoreTimestamp)
			{
				this.AddVelocityToQueue(base.transform.position, PhotonNetwork.Time);
				this.nextLocalVelocityStoreTimestamp = Time.time + 0.1f;
			}
		}
		if (this.leftHandLink.IsLinkActive())
		{
			VRRig myRig = this.leftHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig.inDuplicationZone && myRig.duplicationZone.IsApplyingDisplacement)
			{
				this.leftHandLink.BreakLink();
			}
			else
			{
				this.leftHandLink.VisuallySnapHandsTogether();
			}
		}
		if (this.rightHandLink.IsLinkActive())
		{
			VRRig myRig2 = this.rightHandLink.grabbedLink.myRig;
			if (this.isLocal && myRig2.inDuplicationZone && myRig2.duplicationZone.IsApplyingDisplacement)
			{
				this.rightHandLink.BreakLink();
			}
			else
			{
				this.rightHandLink.VisuallySnapHandsTogether();
			}
		}
		if (this.creator != null)
		{
			if (GorillaGameManager.instance != null)
			{
				GorillaGameManager.instance.UpdatePlayerAppearance(this);
			}
			else if (this.setMatIndex != 0)
			{
				this.ChangeMaterialLocal(0);
				this.ForceResetFrozenEffect();
			}
		}
		if (this.inDuplicationZone)
		{
			this.renderTransform.position = base.transform.position + this.duplicationZone.GetVisualOffsetForRigs(this.cachedRenderTransformPos);
		}
		if (this.frozenEffect.activeSelf)
		{
			GorillaFreezeTagManager gorillaFreezeTagManager = GorillaGameManager.instance as GorillaFreezeTagManager;
			if (gorillaFreezeTagManager != null)
			{
				this.UpdateFrozen(Time.deltaTime, gorillaFreezeTagManager.freezeDuration);
			}
		}
		if (this.TemporaryCosmeticEffects.Count > 0)
		{
			foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
			{
				if (Time.time - effect.Value.EffectStartedTime >= effect.Value.EffectDuration)
				{
					this.RemoveTemporaryCosmeticEffects(effect);
				}
			}
		}
		this.lateUpdateCallbacks.TryRunCallbacks();
	}

	// Token: 0x06001EEB RID: 7915 RVA: 0x000A4808 File Offset: 0x000A2A08
	public void UpdateFrozen(float dt, float freezeDuration)
	{
		Vector3 localScale = this.frozenEffect.transform.localScale;
		Vector3 vector = localScale;
		vector.y = Mathf.Lerp(this.frozenEffectMinY, this.frozenEffectMaxY, this.frozenTimeElapsed / freezeDuration);
		localScale = new Vector3(localScale.x, vector.y, localScale.z);
		this.frozenEffect.transform.localScale = localScale;
		this.frozenTimeElapsed += dt;
	}

	// Token: 0x06001EEC RID: 7916 RVA: 0x000A4880 File Offset: 0x000A2A80
	private void RemoveTemporaryCosmeticEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin)
		{
			bool flag;
			if (effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
			}
		}
		else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
		{
			this.DisableHitWithKnockBack(effect);
		}
		this.TemporaryCosmeticEffects.Remove(effect.Key);
	}

	// Token: 0x06001EED RID: 7917 RVA: 0x000A48F3 File Offset: 0x000A2AF3
	public void SpawnSkinEffects(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GorillaSkin.ApplyToRig(this, effect.Value.newSkin, GorillaSkin.SkinType.temporaryEffect);
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001EEE RID: 7918 RVA: 0x000A4922 File Offset: 0x000A2B22
	public void EnableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001EEF RID: 7919 RVA: 0x000A4940 File Offset: 0x000A2B40
	private void DisableHitWithKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		if (this.TemporaryCosmeticEffects.ContainsKey(effect.Key) && effect.Value.knockbackVFX)
		{
			GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.knockbackVFX, base.transform.position, true);
			if (gameObject != null)
			{
				gameObject.gameObject.transform.SetParent(base.transform);
				gameObject.gameObject.transform.localPosition = Vector3.zero;
			}
		}
	}

	// Token: 0x06001EF0 RID: 7920 RVA: 0x000A49CC File Offset: 0x000A2BCC
	public void DisableHitWithKnockBack()
	{
		foreach (KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect in this.TemporaryCosmeticEffects.ToArray<KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>>())
		{
			bool flag;
			if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.TagWithKnockback)
			{
				this.DisableHitWithKnockBack(effect);
				this.TemporaryCosmeticEffects.Remove(effect.Key);
			}
			else if (effect.Key == CosmeticEffectsOnPlayers.EFFECTTYPE.Skin && effect.Value.newSkin != null && GorillaSkin.GetActiveSkin(this, out flag) == effect.Value.newSkin)
			{
				GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.temporaryEffect);
				this.TemporaryCosmeticEffects.Remove(effect.Key);
			}
		}
	}

	// Token: 0x06001EF1 RID: 7921 RVA: 0x000A4922 File Offset: 0x000A2B22
	public void ApplyInstanceKnockBack(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001EF2 RID: 7922 RVA: 0x000A4922 File Offset: 0x000A2B22
	public void ActivateVOEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
	}

	// Token: 0x06001EF3 RID: 7923 RVA: 0x000A4A7A File Offset: 0x000A2C7A
	public bool TryGetCosmeticVoiceOverride(CosmeticEffectsOnPlayers.EFFECTTYPE key, out CosmeticEffectsOnPlayers.CosmeticEffect value)
	{
		if (this.TemporaryCosmeticEffects == null)
		{
			value = null;
			return false;
		}
		return this.TemporaryCosmeticEffects.TryGetValue(key, out value);
	}

	// Token: 0x06001EF4 RID: 7924 RVA: 0x000A4A98 File Offset: 0x000A2C98
	public void PlayCosmeticEffectSFX(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		this.TemporaryCosmeticEffects.TryAdd(effect.Key, effect.Value);
		int index = UnityEngine.Random.Range(0, effect.Value.sfxAudioClip.Count);
		this.tagSound.PlayOneShot(effect.Value.sfxAudioClip[index]);
	}

	// Token: 0x06001EF5 RID: 7925 RVA: 0x000A4AF4 File Offset: 0x000A2CF4
	public void SpawnVFXEffect(KeyValuePair<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> effect)
	{
		GameObject gameObject = ObjectPools.instance.Instantiate(effect.Value.VFXGameObject, base.transform.position, true);
		if (gameObject != null)
		{
			gameObject.gameObject.transform.SetParent(base.transform);
			gameObject.gameObject.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x17000355 RID: 853
	// (get) Token: 0x06001EF6 RID: 7926 RVA: 0x000A4B58 File Offset: 0x000A2D58
	public bool IsPlayerMeshHidden
	{
		get
		{
			return !this.mainSkin.enabled;
		}
	}

	// Token: 0x06001EF7 RID: 7927 RVA: 0x000A4B68 File Offset: 0x000A2D68
	public void SetPlayerMeshHidden(bool hide)
	{
		this.mainSkin.enabled = !hide;
		this.faceSkin.enabled = !hide;
		this.nameTagAnchor.SetActive(!hide);
		this.UpdateMatParticles(-1);
	}

	// Token: 0x06001EF8 RID: 7928 RVA: 0x000A4B9E File Offset: 0x000A2D9E
	public void SetInvisibleToLocalPlayer(bool invisible)
	{
		if (this.IsInvisibleToLocalPlayer == invisible)
		{
			return;
		}
		this.IsInvisibleToLocalPlayer = invisible;
		this.nameTagAnchor.SetActive(!invisible);
		this.UpdateFriendshipBracelet();
	}

	// Token: 0x06001EF9 RID: 7929 RVA: 0x000A4BC6 File Offset: 0x000A2DC6
	public void ChangeLayer(string layerName)
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.ChangeLayer(base.transform.parent, layerName);
		}
		GTPlayer.Instance.ChangeLayer(layerName);
	}

	// Token: 0x06001EFA RID: 7930 RVA: 0x000A4BF8 File Offset: 0x000A2DF8
	public void RestoreLayer()
	{
		if (this.layerChanger != null)
		{
			this.layerChanger.RestoreOriginalLayers();
		}
		GTPlayer.Instance.RestoreLayer();
	}

	// Token: 0x06001EFB RID: 7931 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void SetHeadBodyOffset()
	{
	}

	// Token: 0x06001EFC RID: 7932 RVA: 0x000A4C1D File Offset: 0x000A2E1D
	public void VRRigResize(float ratioVar)
	{
		this.ratio *= ratioVar;
	}

	// Token: 0x06001EFD RID: 7933 RVA: 0x000A4C30 File Offset: 0x000A2E30
	public int ReturnHandPosition()
	{
		return 0 + Mathf.FloorToInt(this.rightIndex.calcT * 9.99f) + Mathf.FloorToInt(this.rightMiddle.calcT * 9.99f) * 10 + Mathf.FloorToInt(this.rightThumb.calcT * 9.99f) * 100 + Mathf.FloorToInt(this.leftIndex.calcT * 9.99f) * 1000 + Mathf.FloorToInt(this.leftMiddle.calcT * 9.99f) * 10000 + Mathf.FloorToInt(this.leftThumb.calcT * 9.99f) * 100000 + this.leftHandHoldableStatus * 1000000 + this.rightHandHoldableStatus * 10000000;
	}

	// Token: 0x06001EFE RID: 7934 RVA: 0x000A4CFA File Offset: 0x000A2EFA
	public void OnDestroy()
	{
		if (ApplicationQuittingState.IsQuitting)
		{
			return;
		}
		if (this.currentRopeSwingTarget && this.currentRopeSwingTarget.gameObject)
		{
			Object.Destroy(this.currentRopeSwingTarget.gameObject);
		}
		this.ClearRopeData();
	}

	// Token: 0x06001EFF RID: 7935 RVA: 0x000A4D3C File Offset: 0x000A2F3C
	private InputStruct SerializeWriteShared()
	{
		if (this.myIk == null)
		{
			this.myIk = base.GetComponent<GorillaIK>();
		}
		InputStruct inputStruct = new InputStruct
		{
			headRotation = BitPackUtils.PackQuaternionForNetwork(this.head.rigTarget.localRotation),
			rotation = BitPackUtils.PackQuaternionForNetwork(base.transform.rotation),
			usingNewIK = this.ShouldUseNewIKMethod(this.myIk.usingUpdatedIK)
		};
		if (inputStruct.usingNewIK)
		{
			inputStruct.bodyRotation = BitPackUtils.PackQuaternionForNetwork(this.myIk.targetBodyRot);
			inputStruct.rightUpperArmRotation = (short)BitPackUtils.PackRelativePos16(this.myIk.rightElbowDirection, Vector3.zero, 1f);
			inputStruct.leftUpperArmRotation = (short)BitPackUtils.PackRelativePos16(this.myIk.leftElbowDirection, Vector3.zero, 1f);
		}
		inputStruct.rightHandLong = BitPackUtils.PackHandPosRotForNetwork(this.rightHand.rigTarget.localPosition, this.rightHand.rigTarget.localRotation);
		inputStruct.leftHandLong = BitPackUtils.PackHandPosRotForNetwork(this.leftHand.rigTarget.localPosition, this.leftHand.rigTarget.localRotation);
		inputStruct.position = BitPackUtils.PackWorldPosForNetwork(base.transform.position);
		inputStruct.handPosition = this.ReturnHandPosition();
		inputStruct.taggedById = (short)this.taggedById;
		int num = Mathf.Clamp(Mathf.RoundToInt(base.transform.rotation.eulerAngles.y + 360f) % 360, 0, 360);
		int num2 = Mathf.RoundToInt(Mathf.Clamp01(this.SpeakingLoudness) * 255f);
		bool flag = this.leftHandLink.IsLinkActive() || this.rightHandLink.IsLinkActive();
		GorillaGameManager activeGameMode = GorillaGameModes.GameMode.ActiveGameMode;
		bool flag2 = activeGameMode != null && activeGameMode.GameType() == GameModeType.PropHunt;
		int packedFields = num + (this.remoteUseReplacementVoice ? 512 : 0) + ((this.grabbedRopeIndex != -1) ? 1024 : 0) + (this.grabbedRopeIsPhotonView ? 2048 : 0) + (flag ? 4096 : 0) + (this.hoverboardVisual.IsHeld ? 8192 : 0) + (this.hoverboardVisual.IsLeftHanded ? 16384 : 0) + ((this.mountedMovingSurfaceId != -1) ? 32768 : 0) + (flag2 ? 65536 : 0) + (this.propHuntHandFollower.IsLeftHand ? 131072 : 0) + (this.leftHandLink.CanBeGrabbed() ? 262144 : 0) + (this.rightHandLink.CanBeGrabbed() ? 524288 : 0) + (this.leftHandLink.IsTentacleGrab ? 1048576 : 0) + (this.rightHandLink.IsTentacleGrab ? 2097152 : 0) + (this.ShowGoldNameTag ? 4194304 : 0) + (num2 << 24);
		inputStruct.packedFields = packedFields;
		inputStruct.packedCompetitiveData = this.PackCompetitiveData();
		if (this.grabbedRopeIndex != -1)
		{
			inputStruct.grabbedRopeIndex = this.grabbedRopeIndex;
			inputStruct.ropeBoneIndex = this.grabbedRopeBoneIndex;
			inputStruct.ropeGrabIsLeft = this.grabbedRopeIsLeft;
			inputStruct.ropeGrabIsBody = this.grabbedRopeIsBody;
			inputStruct.ropeGrabOffset = this.grabbedRopeOffset;
		}
		if (this.grabbedRopeIndex == -1 && this.mountedMovingSurfaceId != -1)
		{
			inputStruct.grabbedRopeIndex = this.mountedMovingSurfaceId;
			inputStruct.ropeGrabIsLeft = this.mountedMovingSurfaceIsLeft;
			inputStruct.ropeGrabIsBody = this.mountedMovingSurfaceIsBody;
			inputStruct.ropeGrabOffset = this.mountedMonkeBlockOffset;
		}
		if (this.hoverboardVisual.IsHeld)
		{
			inputStruct.hoverboardPosRot = BitPackUtils.PackHandPosRotForNetwork(this.hoverboardVisual.NominalLocalPosition, this.hoverboardVisual.NominalLocalRotation);
			inputStruct.hoverboardColor = BitPackUtils.PackColorForNetwork(this.hoverboardVisual.boardColor);
		}
		if (flag2)
		{
			inputStruct.propHuntPosRot = this.propHuntHandFollower.GetRelativePosRotLong();
		}
		if (flag)
		{
			this.leftHandLink.Write(out inputStruct.isGroundedHand, out inputStruct.isGroundedButt, out inputStruct.leftHandGrabbedActorNumber, out inputStruct.leftGrabbedHandIsLeft);
			this.rightHandLink.Write(out inputStruct.isGroundedHand, out inputStruct.isGroundedButt, out inputStruct.rightHandGrabbedActorNumber, out inputStruct.rightGrabbedHandIsLeft);
			inputStruct.lastTouchedGroundAtTime = this.LastTouchedGroundAtNetworkTime;
			inputStruct.lastHandTouchedGroundAtTime = this.LastHandTouchedGroundAtNetworkTime;
		}
		return inputStruct;
	}

	// Token: 0x06001F00 RID: 7936 RVA: 0x000A51A0 File Offset: 0x000A33A0
	private void SerializeReadShared(InputStruct data)
	{
		if (this.myIk == null)
		{
			this.myIk = base.GetComponent<GorillaIK>();
		}
		VRMap vrmap = this.head;
		Quaternion quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.headRotation);
		ref vrmap.syncRotation.SetValueSafe(quaternion);
		bool usingUpdatedIK = this.ShouldUseNewIKMethod(data.usingNewIK);
		this.myIk.usingUpdatedIK = usingUpdatedIK;
		if (this.myIk.usingUpdatedIK)
		{
			GorillaIK gorillaIK = this.myIk;
			quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.bodyRotation);
			ref gorillaIK.targetBodyRot.SetValueSafe(quaternion);
			GorillaIK gorillaIK2 = this.myIk;
			Vector3 vector = BitPackUtils.UnpackRelativePos16((ushort)data.leftUpperArmRotation, Vector3.zero, 1f, false);
			ref gorillaIK2.leftElbowDirection.SetValueSafe(vector);
			GorillaIK gorillaIK3 = this.myIk;
			vector = BitPackUtils.UnpackRelativePos16((ushort)data.rightUpperArmRotation, Vector3.zero, 1f, false);
			ref gorillaIK3.rightElbowDirection.SetValueSafe(vector);
		}
		BitPackUtils.UnpackHandPosRotFromNetwork(data.rightHandLong, out this.tempVec, out this.tempQuat);
		this.rightHand.syncPos = this.tempVec;
		ref this.rightHand.syncRotation.SetValueSafe(this.tempQuat);
		BitPackUtils.UnpackHandPosRotFromNetwork(data.leftHandLong, out this.tempVec, out this.tempQuat);
		this.leftHand.syncPos = this.tempVec;
		ref this.leftHand.syncRotation.SetValueSafe(this.tempQuat);
		this.syncPos = BitPackUtils.UnpackWorldPosFromNetwork(data.position);
		this.handSync = data.handPosition;
		int packedFields = data.packedFields;
		if (GTPlayerTransform.UseNetRotation)
		{
			quaternion = BitPackUtils.UnpackQuaternionFromNetwork(data.rotation);
			ref this.syncRotation.SetValueSafe(quaternion);
		}
		else
		{
			int num = packedFields & 511;
			this.syncRotation.eulerAngles = this.SanitizeVector3(new Vector3(0f, (float)num, 0f));
		}
		this.remoteUseReplacementVoice = ((packedFields & 512) != 0);
		if ((packedFields & 4194304) != 0 && SubscriptionManager.GetSubscriptionDetails(this).active)
		{
			this.playerText1.color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
		}
		else
		{
			this.playerText1.color = Color.white;
		}
		int num2 = packedFields >> 24 & 255;
		this.SpeakingLoudness = (float)num2 / 255f;
		this.UpdateReplacementVoice();
		this.UnpackCompetitiveData(data.packedCompetitiveData);
		this.taggedById = (int)data.taggedById;
		bool flag = (packedFields & 1024) != 0;
		this.grabbedRopeIsPhotonView = ((packedFields & 2048) != 0);
		if (flag)
		{
			this.grabbedRopeIndex = data.grabbedRopeIndex;
			this.grabbedRopeBoneIndex = data.ropeBoneIndex;
			this.grabbedRopeIsLeft = data.ropeGrabIsLeft;
			this.grabbedRopeIsBody = data.ropeGrabIsBody;
			ref this.grabbedRopeOffset.SetValueSafe(data.ropeGrabOffset);
		}
		else
		{
			this.grabbedRopeIndex = -1;
		}
		bool flag2 = (packedFields & 32768) != 0;
		if (!flag && flag2)
		{
			this.mountedMovingSurfaceId = data.grabbedRopeIndex;
			this.mountedMovingSurfaceIsLeft = data.ropeGrabIsLeft;
			this.mountedMovingSurfaceIsBody = data.ropeGrabIsBody;
			ref this.mountedMonkeBlockOffset.SetValueSafe(data.ropeGrabOffset);
			this.movingSurfaceIsMonkeBlock = data.movingSurfaceIsMonkeBlock;
		}
		else
		{
			this.mountedMovingSurfaceId = -1;
		}
		bool flag3 = (packedFields & 8192) != 0;
		bool isHeldLeftHanded = (packedFields & 16384) != 0;
		if (flag3)
		{
			Vector3 v;
			Quaternion localRotation;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.hoverboardPosRot, out v, out localRotation);
			Color boardColor = BitPackUtils.UnpackColorFromNetwork(data.hoverboardColor);
			if (localRotation.IsValid())
			{
				this.hoverboardVisual.SetIsHeld(isHeldLeftHanded, v.ClampMagnitudeSafe(1f), localRotation, boardColor);
			}
		}
		else if (this.hoverboardVisual.gameObject.activeSelf)
		{
			this.hoverboardVisual.SetNotHeld();
		}
		if ((packedFields & 65536) != 0)
		{
			bool isLeftHand = (packedFields & 131072) != 0;
			Vector3 propPos;
			Quaternion propRot;
			BitPackUtils.UnpackHandPosRotFromNetwork(data.propHuntPosRot, out propPos, out propRot);
			this.propHuntHandFollower.SetProp(isLeftHand, propPos, propRot);
		}
		if (this.grabbedRopeIsPhotonView)
		{
			this.localGrabOverrideBlend = -1f;
		}
		Vector3 position = base.transform.position;
		this.leftHandLink.Read(this.leftHand.syncPos, this.syncRotation, position, data.isGroundedHand, data.isGroundedButt, (packedFields & 262144) != 0, (packedFields & 1048576) != 0, data.leftHandGrabbedActorNumber, data.leftGrabbedHandIsLeft);
		this.rightHandLink.Read(this.rightHand.syncPos, this.syncRotation, position, data.isGroundedHand, data.isGroundedButt, (packedFields & 524288) != 0, (packedFields & 2097152) != 0, data.rightHandGrabbedActorNumber, data.rightGrabbedHandIsLeft);
		this.LastTouchedGroundAtNetworkTime = data.lastTouchedGroundAtTime;
		this.LastHandTouchedGroundAtNetworkTime = data.lastHandTouchedGroundAtTime;
		this.UpdateRopeData();
		this.UpdateMovingMonkeBlockData();
		this.AddVelocityToQueue(this.syncPos, data.serverTimeStamp);
	}

	// Token: 0x06001F01 RID: 7937 RVA: 0x000A5650 File Offset: 0x000A3850
	private bool ShouldUseNewIKMethod(bool isReceivingNewIKData)
	{
		if (this.isOfflineVRRig)
		{
			bool flag = SubscriptionManager.IsLocalSubscribed();
			bool subscriptionSettingBool = SubscriptionManager.GetSubscriptionSettingBool(SubscriptionManager.SubscriptionFeatures.IOBT);
			return flag && subscriptionSettingBool && this.myIk != null && this.myIk.usingUpdatedIK;
		}
		return isReceivingNewIKData;
	}

	// Token: 0x06001F02 RID: 7938 RVA: 0x000A5694 File Offset: 0x000A3894
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		stream.SendNext(inputStruct.headRotation);
		stream.SendNext(inputStruct.rotation);
		stream.SendNext(inputStruct.usingNewIK);
		if (inputStruct.usingNewIK)
		{
			stream.SendNext(inputStruct.bodyRotation);
			stream.SendNext(inputStruct.leftUpperArmRotation);
			stream.SendNext(inputStruct.rightUpperArmRotation);
		}
		stream.SendNext(inputStruct.rightHandLong);
		stream.SendNext(inputStruct.leftHandLong);
		stream.SendNext(inputStruct.position);
		stream.SendNext(inputStruct.handPosition);
		stream.SendNext(inputStruct.packedFields);
		stream.SendNext(inputStruct.packedCompetitiveData);
		if (this.grabbedRopeIndex != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeBoneIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
		}
		else if (this.mountedMovingSurfaceId != -1)
		{
			stream.SendNext(inputStruct.grabbedRopeIndex);
			stream.SendNext(inputStruct.ropeGrabIsLeft);
			stream.SendNext(inputStruct.ropeGrabIsBody);
			stream.SendNext(inputStruct.ropeGrabOffset);
			stream.SendNext(inputStruct.movingSurfaceIsMonkeBlock);
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			stream.SendNext(inputStruct.hoverboardPosRot);
			stream.SendNext(inputStruct.hoverboardColor);
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			stream.SendNext(inputStruct.isGroundedHand);
			stream.SendNext(inputStruct.isGroundedButt);
			stream.SendNext(inputStruct.leftHandGrabbedActorNumber);
			stream.SendNext(inputStruct.leftGrabbedHandIsLeft);
			stream.SendNext(inputStruct.rightHandGrabbedActorNumber);
			stream.SendNext(inputStruct.rightGrabbedHandIsLeft);
			stream.SendNext(inputStruct.lastTouchedGroundAtTime);
			stream.SendNext(inputStruct.lastHandTouchedGroundAtTime);
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			stream.SendNext(inputStruct.propHuntPosRot);
		}
	}

	// Token: 0x06001F03 RID: 7939 RVA: 0x000A5924 File Offset: 0x000A3B24
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		double sentServerTime = info.SentServerTime;
		InputStruct inputStruct = new InputStruct
		{
			headRotation = (int)stream.ReceiveNext(),
			rotation = (int)stream.ReceiveNext(),
			usingNewIK = (bool)stream.ReceiveNext()
		};
		if (inputStruct.usingNewIK)
		{
			inputStruct.bodyRotation = (int)stream.ReceiveNext();
			inputStruct.leftUpperArmRotation = (short)stream.ReceiveNext();
			inputStruct.rightUpperArmRotation = (short)stream.ReceiveNext();
		}
		inputStruct.rightHandLong = (long)stream.ReceiveNext();
		inputStruct.leftHandLong = (long)stream.ReceiveNext();
		inputStruct.position = (long)stream.ReceiveNext();
		inputStruct.handPosition = (int)stream.ReceiveNext();
		inputStruct.packedFields = (int)stream.ReceiveNext();
		inputStruct.packedCompetitiveData = (short)stream.ReceiveNext();
		bool flag = (inputStruct.packedFields & 1024) != 0;
		bool flag2 = (inputStruct.packedFields & 32768) != 0;
		if (flag)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeBoneIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		else if (flag2)
		{
			inputStruct.grabbedRopeIndex = (int)stream.ReceiveNext();
			inputStruct.ropeGrabIsLeft = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabIsBody = (bool)stream.ReceiveNext();
			inputStruct.ropeGrabOffset = (Vector3)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 8192) != 0)
		{
			inputStruct.hoverboardPosRot = (long)stream.ReceiveNext();
			inputStruct.hoverboardColor = (short)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 4096) != 0)
		{
			inputStruct.isGroundedHand = (bool)stream.ReceiveNext();
			inputStruct.isGroundedButt = (bool)stream.ReceiveNext();
			inputStruct.leftHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.leftGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.rightHandGrabbedActorNumber = (int)stream.ReceiveNext();
			inputStruct.rightGrabbedHandIsLeft = (bool)stream.ReceiveNext();
			inputStruct.lastTouchedGroundAtTime = (float)stream.ReceiveNext();
			inputStruct.lastHandTouchedGroundAtTime = (float)stream.ReceiveNext();
		}
		if ((inputStruct.packedFields & 65536) != 0)
		{
			inputStruct.propHuntPosRot = (long)stream.ReceiveNext();
		}
		inputStruct.serverTimeStamp = info.SentServerTime;
		this.SerializeReadShared(inputStruct);
	}

	// Token: 0x06001F04 RID: 7940 RVA: 0x000A5BF4 File Offset: 0x000A3DF4
	public object OnSerializeWrite()
	{
		InputStruct inputStruct = this.SerializeWriteShared();
		double serverTimeStamp = NetworkSystem.Instance.SimTick / 1000.0;
		inputStruct.serverTimeStamp = serverTimeStamp;
		return inputStruct;
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x000A5C30 File Offset: 0x000A3E30
	public void OnSerializeRead(object objectData)
	{
		InputStruct data = (InputStruct)objectData;
		this.SerializeReadShared(data);
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x000A5C4C File Offset: 0x000A3E4C
	private void UpdateExtrapolationTarget()
	{
		float num = (float)(NetworkSystem.Instance.SimTime - this.remoteLatestTimestamp);
		num -= 0.15f;
		num = Mathf.Clamp(num, -0.5f, 0.5f);
		this.syncPos += this.remoteVelocity * num;
		this.remoteCorrectionNeeded = this.syncPos - base.transform.position;
		if (this.remoteCorrectionNeeded.magnitude > 1.5f && this.grabbedRopeIndex <= 0)
		{
			base.transform.position = this.syncPos;
			this.remoteCorrectionNeeded = Vector3.zero;
		}
	}

	// Token: 0x06001F07 RID: 7943 RVA: 0x000A5CF8 File Offset: 0x000A3EF8
	private void UpdateRopeData()
	{
		if (this.previousGrabbedRope == this.grabbedRopeIndex && this.previousGrabbedRopeBoneIndex == this.grabbedRopeBoneIndex && this.previousGrabbedRopeWasLeft == this.grabbedRopeIsLeft && this.previousGrabbedRopeWasBody == this.grabbedRopeIsBody)
		{
			return;
		}
		this.ClearRopeData();
		if (this.grabbedRopeIndex != -1)
		{
			GorillaRopeSwing gorillaRopeSwing;
			if (this.grabbedRopeIsPhotonView)
			{
				PhotonView photonView = PhotonView.Find(this.grabbedRopeIndex);
				GorillaClimbable gorillaClimbable;
				HandHoldXSceneRef handHoldXSceneRef;
				VRRigSerializer vrrigSerializer;
				if (photonView.TryGetComponent<GorillaClimbable>(out gorillaClimbable))
				{
					this.currentHoldParent = photonView.transform;
				}
				else if (photonView.TryGetComponent<HandHoldXSceneRef>(out handHoldXSceneRef))
				{
					GameObject targetObject = handHoldXSceneRef.targetObject;
					this.currentHoldParent = ((targetObject != null) ? targetObject.transform : null);
				}
				else if (photonView && photonView.TryGetComponent<VRRigSerializer>(out vrrigSerializer))
				{
					this.currentHoldParent = ((this.grabbedRopeBoneIndex == 1) ? vrrigSerializer.VRRig.leftHandHoldsPlayer.transform : vrrigSerializer.VRRig.rightHandHoldsPlayer.transform);
				}
			}
			else if (RopeSwingManager.instance.TryGetRope(this.grabbedRopeIndex, out gorillaRopeSwing) && gorillaRopeSwing != null)
			{
				if (this.currentRopeSwingTarget == null || this.currentRopeSwingTarget.gameObject == null)
				{
					this.currentRopeSwingTarget = new GameObject("RopeSwingTarget").transform;
				}
				if (gorillaRopeSwing.AttachRemotePlayer(this.creator.ActorNumber, this.grabbedRopeBoneIndex, this.currentRopeSwingTarget, this.grabbedRopeOffset))
				{
					this.currentRopeSwing = gorillaRopeSwing;
				}
				this.lastRopeGrabTimer = 0f;
			}
		}
		else if (this.previousGrabbedRope != -1)
		{
			PhotonView photonView2 = PhotonView.Find(this.previousGrabbedRope);
			VRRigSerializer vrrigSerializer2;
			if (photonView2 && photonView2.TryGetComponent<VRRigSerializer>(out vrrigSerializer2) && vrrigSerializer2.VRRig == VRRig.LocalRig)
			{
				EquipmentInteractor.instance.ForceDropEquipment(this.bodyHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.leftHolds);
				EquipmentInteractor.instance.ForceDropEquipment(this.rightHolds);
			}
		}
		this.shouldLerpToRope = true;
		this.previousGrabbedRope = this.grabbedRopeIndex;
		this.previousGrabbedRopeBoneIndex = this.grabbedRopeBoneIndex;
		this.previousGrabbedRopeWasLeft = this.grabbedRopeIsLeft;
		this.previousGrabbedRopeWasBody = this.grabbedRopeIsBody;
	}

	// Token: 0x06001F08 RID: 7944 RVA: 0x000A5F38 File Offset: 0x000A4138
	private void UpdateMovingMonkeBlockData()
	{
		if (this.mountedMonkeBlockOffset.sqrMagnitude > 2f)
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.prevMovingSurfaceID == this.mountedMovingSurfaceId && this.movingSurfaceWasBody == this.mountedMovingSurfaceIsBody && this.movingSurfaceWasLeft == this.mountedMovingSurfaceIsLeft && this.movingSurfaceWasMonkeBlock == this.movingSurfaceIsMonkeBlock)
		{
			return;
		}
		if (this.mountedMovingSurfaceId == -1)
		{
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		else if (this.movingSurfaceIsMonkeBlock)
		{
			this.mountedMonkeBlock = null;
			BuilderTable builderTable;
			if (BuilderTable.TryGetBuilderTableForZone(this.zoneEntity.currentZone, out builderTable))
			{
				this.mountedMonkeBlock = builderTable.GetPiece(this.mountedMovingSurfaceId);
			}
			if (this.mountedMonkeBlock == null)
			{
				this.mountedMovingSurfaceId = -1;
				this.mountedMovingSurfaceIsLeft = false;
				this.mountedMovingSurfaceIsBody = false;
				this.mountedMonkeBlock = null;
				this.mountedMovingSurface = null;
			}
		}
		else if (MovingSurfaceManager.instance == null || !MovingSurfaceManager.instance.TryGetMovingSurface(this.mountedMovingSurfaceId, out this.mountedMovingSurface))
		{
			this.mountedMovingSurfaceId = -1;
			this.mountedMovingSurfaceIsLeft = false;
			this.mountedMovingSurfaceIsBody = false;
			this.mountedMonkeBlock = null;
			this.mountedMovingSurface = null;
		}
		if (this.mountedMovingSurfaceId != -1 && this.prevMovingSurfaceID == -1)
		{
			this.shouldLerpToMovingSurface = true;
			this.lastMountedSurfaceTimer = 0f;
		}
		this.prevMovingSurfaceID = this.mountedMovingSurfaceId;
		this.movingSurfaceWasLeft = this.mountedMovingSurfaceIsLeft;
		this.movingSurfaceWasBody = this.mountedMovingSurfaceIsBody;
		this.movingSurfaceWasMonkeBlock = this.movingSurfaceIsMonkeBlock;
	}

	// Token: 0x06001F09 RID: 7945 RVA: 0x000A60E4 File Offset: 0x000A42E4
	public static void AttachLocalPlayerToMovingSurface(int blockId, bool isLeft, bool isBody, Vector3 offset, bool isMonkeBlock)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = blockId;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsLeft = isLeft;
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceIsBody = isBody;
			GorillaTagger.Instance.offlineVRRig.movingSurfaceIsMonkeBlock = isMonkeBlock;
			GorillaTagger.Instance.offlineVRRig.mountedMonkeBlockOffset = offset;
		}
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000A615A File Offset: 0x000A435A
	public static void DetachLocalPlayerFromMovingSurface()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.mountedMovingSurfaceId = -1;
		}
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x000A6184 File Offset: 0x000A4384
	public static void AttachLocalPlayerToPhotonView(PhotonView view, XRNode xrNode, Vector3 offset, Vector3 velocity)
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = view.ViewID;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsLeft = (xrNode == XRNode.LeftHand);
			GorillaTagger.Instance.offlineVRRig.grabbedRopeOffset = offset;
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIsPhotonView = true;
		}
	}

	// Token: 0x06001F0C RID: 7948 RVA: 0x000A61F1 File Offset: 0x000A43F1
	public static void DetachLocalPlayerFromPhotonView()
	{
		if (GorillaTagger.hasInstance && GorillaTagger.Instance.offlineVRRig)
		{
			GorillaTagger.Instance.offlineVRRig.grabbedRopeIndex = -1;
		}
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000A621C File Offset: 0x000A441C
	private void ClearRopeData()
	{
		if (this.currentRopeSwing)
		{
			this.currentRopeSwing.DetachRemotePlayer(this.creator.ActorNumber);
		}
		if (this.currentRopeSwingTarget)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		this.currentRopeSwing = null;
		this.currentHoldParent = null;
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x000A6273 File Offset: 0x000A4473
	public void ChangeMaterial(int materialIndex, PhotonMessageInfo info)
	{
		if (info.Sender == PhotonNetwork.MasterClient)
		{
			this.ChangeMaterialLocal(materialIndex);
		}
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x000A628C File Offset: 0x000A448C
	public void UpdateFrozenEffect(bool enable)
	{
		if (this.frozenEffect != null && ((!this.frozenEffect.activeSelf && enable) || (this.frozenEffect.activeSelf && !enable)))
		{
			this.frozenEffect.SetActive(enable);
			if (enable)
			{
				this.frozenTimeElapsed = 0f;
			}
			else
			{
				Vector3 localScale = this.frozenEffect.transform.localScale;
				localScale = new Vector3(localScale.x, this.frozenEffectMinY, localScale.z);
				this.frozenEffect.transform.localScale = localScale;
			}
		}
		if (this.iceCubeLeft != null && ((!this.iceCubeLeft.activeSelf && enable) || (this.iceCubeLeft.activeSelf && !enable)))
		{
			this.iceCubeLeft.SetActive(enable);
		}
		if (this.iceCubeRight != null && ((!this.iceCubeRight.activeSelf && enable) || (this.iceCubeRight.activeSelf && !enable)))
		{
			this.iceCubeRight.SetActive(enable);
		}
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x000A6398 File Offset: 0x000A4598
	public void ForceResetFrozenEffect()
	{
		this.frozenEffect.SetActive(false);
		this.iceCubeRight.SetActive(false);
		this.iceCubeLeft.SetActive(false);
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x000A63C0 File Offset: 0x000A45C0
	public void ChangeMaterialLocal(int materialIndex)
	{
		if (this.setMatIndex == materialIndex)
		{
			return;
		}
		int arg = this.setMatIndex;
		this.setMatIndex = materialIndex;
		if (this.setMatIndex > -1 && this.setMatIndex < this.materialsToChangeTo.Length)
		{
			this.bodyRenderer.SetMaterialIndex(materialIndex);
		}
		this.UpdateMatParticles(materialIndex);
		if (materialIndex > 0 && VRRig.LocalRig != this)
		{
			this.PlayTaggedEffect();
		}
		Action<int, int> onMaterialIndexChanged = this.OnMaterialIndexChanged;
		if (onMaterialIndexChanged == null)
		{
			return;
		}
		onMaterialIndexChanged(arg, this.setMatIndex);
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x000A6440 File Offset: 0x000A4640
	public void PlayTaggedEffect()
	{
		TagEffectPack tagEffectPack = null;
		quaternion q = base.transform.rotation;
		TagEffectsLibrary.EffectType effectType = (VRRig.LocalRig == this) ? TagEffectsLibrary.EffectType.FIRST_PERSON : TagEffectsLibrary.EffectType.THIRD_PERSON;
		if (GorillaGameManager.instance != null && this.OwningNetPlayer != null)
		{
			GorillaGameManager.instance.lastTaggedActorNr.TryGetValue(this.OwningNetPlayer.ActorNumber, out this.taggedById);
		}
		NetPlayer player = NetworkSystem.Instance.GetPlayer(this.taggedById);
		RigContainer rigContainer;
		if (player != null && VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			tagEffectPack = rigContainer.Rig.CosmeticEffectPack;
			if (tagEffectPack && tagEffectPack.shouldFaceTagger && effectType == TagEffectsLibrary.EffectType.THIRD_PERSON)
			{
				q = Quaternion.LookRotation((rigContainer.Rig.transform.position - base.transform.position).normalized);
			}
		}
		TagEffectsLibrary.PlayEffect(base.transform, false, this.scaleFactor, effectType, this.CosmeticEffectPack, tagEffectPack, q);
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000A6544 File Offset: 0x000A4744
	public void ToggleMatParticles(bool enabled)
	{
		if (this.lavaParticleSystem != null)
		{
			this.ToggleParticleSystem(this.lavaParticleSystem, enabled);
		}
		if (this.rockParticleSystem != null)
		{
			this.ToggleParticleSystem(this.rockParticleSystem, enabled);
		}
		if (this.iceParticleSystem != null)
		{
			this.ToggleParticleSystem(this.iceParticleSystem, enabled);
		}
		if (this.snowFlakeParticleSystem != null)
		{
			this.ToggleParticleSystem(this.snowFlakeParticleSystem, enabled);
		}
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000A65C0 File Offset: 0x000A47C0
	private void ToggleParticleSystem(ParticleSystem ps, bool enabled)
	{
		ps.emission.enabled = enabled;
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000A65DC File Offset: 0x000A47DC
	public void UpdateMatParticles(int materialIndex)
	{
		if (this.lavaParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 2 && this.lavaParticleSystem.isStopped)
			{
				this.lavaParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.lavaParticleSystem.isPlaying)
			{
				this.lavaParticleSystem.Stop();
			}
		}
		if (this.rockParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 1 && this.rockParticleSystem.isStopped)
			{
				this.rockParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.rockParticleSystem.isPlaying)
			{
				this.rockParticleSystem.Stop();
			}
		}
		if (this.iceParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 3 && this.rockParticleSystem.isStopped)
			{
				this.iceParticleSystem.Play();
			}
			else if (!this.isOfflineVRRig && this.iceParticleSystem.isPlaying)
			{
				this.iceParticleSystem.Stop();
			}
		}
		if (this.snowFlakeParticleSystem != null)
		{
			if (!this.isOfflineVRRig && materialIndex == 14 && this.snowFlakeParticleSystem.isStopped)
			{
				this.snowFlakeParticleSystem.Play();
				return;
			}
			if (!this.isOfflineVRRig && this.snowFlakeParticleSystem.isPlaying)
			{
				this.snowFlakeParticleSystem.Stop();
			}
		}
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000A673C File Offset: 0x000A493C
	public void InitializeNoobMaterial(float red, float green, float blue, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_InitializeNoobMaterial");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		string userID = NetworkSystem.Instance.GetUserID(info.senderID);
		if (info.senderID == NetworkSystem.Instance.GetOwningPlayerID(this.rigSerializer.gameObject) && (!this.initialized || (this.initialized && GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(userID)) || (this.initialized && CosmeticWardrobeProximityDetector.IsUserNearWardrobe(info.senderID))))
		{
			this.initialized = true;
			blue = blue.ClampSafe(0f, 1f);
			red = red.ClampSafe(0f, 1f);
			green = green.ClampSafe(0f, 1f);
			this.InitializeNoobMaterialLocal(red, green, blue);
		}
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x000A6820 File Offset: 0x000A4A20
	public void InitializeNoobMaterialLocal(float red, float green, float blue)
	{
		Color color = new Color(red, green, blue);
		color.r = Mathf.Clamp(color.r, 0f, 1f);
		color.g = Mathf.Clamp(color.g, 0f, 1f);
		color.b = Mathf.Clamp(color.b, 0f, 1f);
		this.bodyRenderer.UpdateColor(color);
		this.SetColor(color);
		bool isNamePermissionEnabled = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		this.UpdateName(isNamePermissionEnabled);
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x000A68AC File Offset: 0x000A4AAC
	public void UpdateNameSafeAccount(bool isSafeAccount)
	{
		this.UpdateName(!isSafeAccount);
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x000A68B8 File Offset: 0x000A4AB8
	public void UpdateName(bool isNamePermissionEnabled)
	{
		if (!this.isOfflineVRRig && this.creator != null)
		{
			string text = (isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? this.creator.NickName : this.creator.DefaultName;
			this.playerNameVisible = this.NormalizeName(true, text);
		}
		else if (this.showName && NetworkSystem.Instance != null)
		{
			this.playerNameVisible = ((isNamePermissionEnabled && GorillaComputer.instance.NametagsEnabled) ? NetworkSystem.Instance.GetMyNickName() : NetworkSystem.Instance.GetMyDefaultName());
		}
		this.SetNameTagText(this.playerNameVisible);
		if (this.creator != null)
		{
			this.creator.SanitizedNickName = this.playerNameVisible;
		}
		Action onPlayerNameVisibleChanged = this.OnPlayerNameVisibleChanged;
		if (onPlayerNameVisibleChanged == null)
		{
			return;
		}
		onPlayerNameVisibleChanged();
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x000A6986 File Offset: 0x000A4B86
	public void SetNameTagText(string name)
	{
		this.playerNameVisible = name;
		this.playerText1.text = name;
		Action<RigContainer> onNameChanged = this.OnNameChanged;
		if (onNameChanged == null)
		{
			return;
		}
		onNameChanged(this.rigContainer);
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x000A69B4 File Offset: 0x000A4BB4
	public void UpdateName()
	{
		Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
		bool isNamePermissionEnabled = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
		this.UpdateName(isNamePermissionEnabled);
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000A69F0 File Offset: 0x000A4BF0
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			int length = text.Length;
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			int length2 = text.Length;
			if (length2 > 0 && length == length2 && GorillaComputer.instance.CheckAutoBanListForName(text))
			{
				if (text.Length > 12)
				{
					text = text.Substring(0, 12);
				}
				text = text.ToUpper();
			}
			else
			{
				text = "BADGORILLA";
			}
		}
		return text;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000A6A7D File Offset: 0x000A4C7D
	public void SetJumpLimitLocal(float maxJumpSpeed)
	{
		GTPlayer.Instance.maxJumpSpeed = maxJumpSpeed;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x000A6A8A File Offset: 0x000A4C8A
	public void SetJumpMultiplierLocal(float jumpMultiplier)
	{
		GTPlayer.Instance.jumpMultiplier = jumpMultiplier;
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x000A6A98 File Offset: 0x000A4C98
	public void RequestMaterialColor(int askingPlayerID, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RequestMaterialColor");
		Player playerRef = ((PunNetPlayer)NetworkSystem.Instance.GetPlayer(info.senderID)).PlayerRef;
		if (this.netView.IsMine)
		{
			this.netView.GetView.RPC("RPC_InitializeNoobMaterial", playerRef, new object[]
			{
				this.myDefaultSkinMaterialInstance.color.r,
				this.myDefaultSkinMaterialInstance.color.g,
				this.myDefaultSkinMaterialInstance.color.b
			});
		}
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x000A6B40 File Offset: 0x000A4D40
	public void RequestCosmetics(PhotonMessageInfoWrapped info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (this.netView.IsMine && CosmeticsController.hasInstance)
		{
			if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
			{
				this.netView.SendRPC("RPC_HideAllCosmetics", info.Sender, Array.Empty<object>());
				return;
			}
			int[] array = CosmeticsController.instance.currentWornSet.ToPackedIDArray();
			int[] array2 = CosmeticsController.instance.tryOnSet.ToPackedIDArray();
			this.netView.SendRPC("RPC_UpdateCosmeticsWithTryonPacked", player, new object[]
			{
				array,
				array2,
				false
			});
			CosmeticCollectionDisplay.GetDisplaysForRig(base.GetInstanceID(), this.scratchDisplayList);
			if (this.scratchDisplayList.Count > 0)
			{
				int num = this.scratchDisplayList.Count * 2;
				if (this.cycleStatesArray.Length != num)
				{
					this.cycleStatesArray = new int[num];
				}
				for (int i = 0; i < this.scratchDisplayList.Count; i++)
				{
					string parentPlayFabID = this.scratchDisplayList[i].ParentPlayFabID;
					this.cycleStatesArray[i * 2] = (int)(parentPlayFabID[0] - 'A' + '\u001a' * (parentPlayFabID[1] - 'A' + '\u001a' * (parentPlayFabID[2] - 'A' + '\u001a' * (parentPlayFabID[3] - 'A' + '\u001a' * (parentPlayFabID[4] - 'A')))));
					this.cycleStatesArray[i * 2 + 1] = this.scratchDisplayList[i].ActiveIndex;
				}
				this.netView.SendRPC("RPC_UpdateCosmeticsWithCollectablesPacked", player, new object[]
				{
					this.cycleStatesArray
				});
			}
		}
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x000A6CFC File Offset: 0x000A4EFC
	public void PlayTagSoundLocal(int soundIndex, float soundVolume, bool stopCurrentAudio)
	{
		if (soundIndex < 0 || soundIndex >= this.clipToPlay.Length)
		{
			return;
		}
		this.tagSound.volume = Mathf.Min(0.25f, soundVolume);
		if (stopCurrentAudio)
		{
			this.tagSound.Stop();
		}
		this.tagSound.GTPlayOneShot(this.clipToPlay[soundIndex], 1f);
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x000A6D55 File Offset: 0x000A4F55
	public void AssignDrumToMusicDrums(int drumIndex, AudioSource drum)
	{
		if (drumIndex >= 0 && drumIndex < this.musicDrums.Length && drum != null)
		{
			this.musicDrums[drumIndex] = drum;
		}
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000A6D78 File Offset: 0x000A4F78
	public void PlayDrum(int drumIndex, float drumVolume, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlayDrum");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			this.senderRig = rigContainer.Rig;
		}
		if (this.senderRig == null || this.senderRig.muted)
		{
			return;
		}
		if (drumIndex < 0 || drumIndex >= this.musicDrums.Length || (this.senderRig.transform.position - base.transform.position).sqrMagnitude > 9f || !float.IsFinite(drumVolume))
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent drum", player.UserId, player.NickName);
			return;
		}
		AudioSource audioSource = this.netView.IsMine ? GorillaTagger.Instance.offlineVRRig.musicDrums[drumIndex] : this.musicDrums[drumIndex];
		if (!audioSource.gameObject.activeInHierarchy)
		{
			return;
		}
		float instrumentVolume = GorillaComputer.instance.instrumentVolume;
		audioSource.time = 0f;
		audioSource.volume = Mathf.Max(Mathf.Min(instrumentVolume, drumVolume * instrumentVolume), 0f);
		audioSource.GTPlay();
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000A6EAC File Offset: 0x000A50AC
	public int AssignInstrumentToInstrumentSelfOnly(TransferrableObject instrument)
	{
		if (instrument == null)
		{
			return -1;
		}
		if (!this.instrumentSelfOnly.Contains(instrument))
		{
			this.instrumentSelfOnly.Add(instrument);
		}
		return this.instrumentSelfOnly.IndexOf(instrument);
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000A6EE0 File Offset: 0x000A50E0
	public void PlaySelfOnlyInstrument(int selfOnlyIndex, int noteIndex, float instrumentVol, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySelfOnlyInstrument");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner && !this.muted)
		{
			if (selfOnlyIndex >= 0 && selfOnlyIndex < this.instrumentSelfOnly.Count && float.IsFinite(instrumentVol))
			{
				if (this.instrumentSelfOnly[selfOnlyIndex].gameObject.activeSelf)
				{
					this.instrumentSelfOnly[selfOnlyIndex].PlayNote(noteIndex, Mathf.Max(Mathf.Min(GorillaComputer.instance.instrumentVolume, instrumentVol * GorillaComputer.instance.instrumentVolume), 0f) / 2f);
					return;
				}
			}
			else
			{
				MonkeAgent.instance.SendReport("inappropriate tag data being sent self only instrument", player.UserId, player.NickName);
			}
		}
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x000A6FBC File Offset: 0x000A51BC
	public void PlayHandTapLocal(int audioClipIndex, bool isLeftHand, float tapVolume)
	{
		if (audioClipIndex > -1 && audioClipIndex < GTPlayer.Instance.materialData.Count)
		{
			GTPlayer.MaterialData materialData = GTPlayer.Instance.materialData[audioClipIndex];
			AudioSource audioSource = isLeftHand ? this.leftHandPlayer : this.rightHandPlayer;
			audioSource.volume = tapVolume;
			AudioClip clip = materialData.overrideAudio ? materialData.audio : GTPlayer.Instance.materialData[0].audio;
			audioSource.GTPlayOneShot(clip, 1f);
		}
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000A7039 File Offset: 0x000A5239
	internal HandEffectContext GetHandEffect(bool isLeftHand, StiltID stiltID)
	{
		if (stiltID == StiltID.None)
		{
			if (!isLeftHand)
			{
				return this.RightHandEffect;
			}
			return this.LeftHandEffect;
		}
		else
		{
			if (!isLeftHand)
			{
				return this.ExtraRightHandEffect;
			}
			return this.ExtraLeftHandEffect;
		}
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000A7060 File Offset: 0x000A5260
	internal void SetHandEffectData(HandEffectContext effectContext, int audioClipIndex, bool isDownTap, bool isLeftHand, StiltID stiltID, float handTapVolume, float handTapSpeed, Vector3 dirFromHitToHand)
	{
		VRMap vrmap = isLeftHand ? this.leftHand : this.rightHand;
		Vector3 b = dirFromHitToHand * this.tapPointDistance * this.scaleFactor;
		if (this.isOfflineVRRig)
		{
			Vector3 b2 = vrmap.rigTarget.rotation * vrmap.trackingPositionOffset * this.scaleFactor;
			Vector3 position = (stiltID != StiltID.None) ? GTPlayer.Instance.GetHandPosition(isLeftHand, stiltID) : (vrmap.rigTarget.position - b2 + b);
			effectContext.position = position;
			effectContext.handSoundSource.transform.position = position;
		}
		else
		{
			Quaternion rotation = vrmap.rigTarget.parent.rotation * vrmap.syncRotation;
			Vector3 b3 = this.netSyncPos.GetPredictedFuture() - base.transform.position;
			Vector3 b2 = rotation * vrmap.trackingPositionOffset * this.scaleFactor;
			effectContext.position = vrmap.rigTarget.parent.TransformPoint(vrmap.netSyncPos.GetPredictedFuture()) - b2 + b + b3;
		}
		GTPlayer.MaterialData handSurfaceData = this.GetHandSurfaceData(audioClipIndex);
		HandTapOverrides handTapOverrides = isDownTap ? effectContext.DownTapOverrides : effectContext.UpTapOverrides;
		List<int> prefabHashes = effectContext.prefabHashes;
		int index = 0;
		HashWrapper hashWrapper = handTapOverrides.overrideSurfacePrefab ? handTapOverrides.surfaceTapPrefab : GTPlayer.Instance.materialDatasSO.surfaceEffects[handSurfaceData.surfaceEffectIndex];
		prefabHashes[index] = hashWrapper;
		effectContext.prefabHashes[1] = (ref handTapOverrides.overrideGamemodePrefab ? handTapOverrides.gamemodeTapPrefab : ((RoomSystem.JoinedRoom && GorillaGameModes.GameMode.ActiveGameMode.IsNotNull()) ? GorillaGameModes.GameMode.ActiveGameMode.SpecialHandFX(this.creator, this.rigContainer) : -1));
		effectContext.soundFX = (handTapOverrides.overrideSound ? handTapOverrides.tapSound : handSurfaceData.audio);
		effectContext.isDownTap = isDownTap;
		effectContext.isLeftHand = isLeftHand;
		effectContext.soundVolume = handTapVolume * this.handSpeedToVolumeModifier;
		effectContext.soundPitch = 1f;
		effectContext.speed = handTapSpeed;
		effectContext.color = this.playerColor;
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000A729C File Offset: 0x000A549C
	internal GTPlayer.MaterialData GetHandSurfaceData(int index)
	{
		List<GTPlayer.MaterialData> materialData = GTPlayer.Instance.materialData;
		GTPlayer.MaterialData materialData2;
		if (index >= 0 && index < materialData.Count)
		{
			materialData2 = materialData[index];
		}
		else
		{
			materialData2 = materialData[0];
		}
		if (!materialData2.overrideAudio)
		{
			materialData2 = materialData[0];
		}
		return materialData2;
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x000A72E4 File Offset: 0x000A54E4
	public void PlaySplashEffect(Vector3 splashPosition, Quaternion splashRotation, float splashScale, float boundingRadius, bool bigSplash, bool enteringWater, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_PlaySplashEffect");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (player == this.netView.Owner)
		{
			float num = 10000f;
			if (splashPosition.IsValid(num) && splashRotation.IsValid() && float.IsFinite(splashScale) && float.IsFinite(boundingRadius))
			{
				if ((base.transform.position - splashPosition).sqrMagnitude >= 9f)
				{
					return;
				}
				float time = Time.time;
				int num2 = -1;
				float num3 = time + 10f;
				for (int i = 0; i < this.splashEffectTimes.Length; i++)
				{
					if (this.splashEffectTimes[i] < num3)
					{
						num3 = this.splashEffectTimes[i];
						num2 = i;
					}
				}
				if (time - 0.5f > num3)
				{
					this.splashEffectTimes[num2] = time;
					boundingRadius = Mathf.Clamp(boundingRadius, 0.0001f, 0.5f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.rippleEffect, splashPosition, splashRotation, GTPlayer.Instance.waterParams.rippleEffectScale * boundingRadius * 2f, true);
					splashScale = Mathf.Clamp(splashScale, 1E-05f, 1f);
					ObjectPools.instance.Instantiate(GTPlayer.Instance.waterParams.splashEffect, splashPosition, splashRotation, splashScale, true).GetComponent<WaterSplashEffect>().PlayEffect(bigSplash, enteringWater, splashScale, null);
					return;
				}
				return;
			}
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent splash effect", player.UserId, player.NickName);
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000A7480 File Offset: 0x000A5680
	[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
	public void RPC_EnableNonCosmeticHandItem(bool enable, bool isLeftHand, RpcInfo info = default(RpcInfo))
	{
		PhotonMessageInfoWrapped photonMessageInfoWrapped = new PhotonMessageInfoWrapped(info);
		this.IncrementRPC(photonMessageInfoWrapped, "EnableNonCosmeticHandItem");
		if (photonMessageInfoWrapped.Sender == this.creator)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(photonMessageInfoWrapped.Sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", photonMessageInfoWrapped.Sender.UserId, photonMessageInfoWrapped.Sender.NickName);
		}
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x000A753C File Offset: 0x000A573C
	[PunRPC]
	public void EnableNonCosmeticHandItemRPC(bool enable, bool isLeftHand, PhotonMessageInfoWrapped info)
	{
		NetPlayer sender = info.Sender;
		this.IncrementRPC(info, "EnableNonCosmeticHandItem");
		if (sender == this.netView.Owner)
		{
			this.senderRig = GorillaGameManager.StaticFindRigForPlayer(sender);
			if (this.senderRig == null)
			{
				return;
			}
			if (isLeftHand && this.nonCosmeticLeftHandItem)
			{
				this.senderRig.nonCosmeticLeftHandItem.EnableItem(enable);
				return;
			}
			if (!isLeftHand && this.nonCosmeticRightHandItem)
			{
				this.senderRig.nonCosmeticRightHandItem.EnableItem(enable);
				return;
			}
		}
		else
		{
			MonkeAgent.instance.SendReport("inappropriate tag data being sent Enable Non Cosmetic Hand Item", info.Sender.UserId, info.Sender.NickName);
		}
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x000A75F0 File Offset: 0x000A57F0
	public bool IsMakingFistLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) > 0.25f;
		}
		return this.leftIndex.calcT > 0.25f && this.leftMiddle.calcT > 0.25f;
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x000A7648 File Offset: 0x000A5848
	public bool IsMakingFistRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) > 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) > 0.25f;
		}
		return this.rightIndex.calcT > 0.25f && this.rightMiddle.calcT > 0.25f;
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x000A76A0 File Offset: 0x000A58A0
	public bool IsMakingFiveLeft()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.LeftHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.LeftHand) < 0.25f;
		}
		return this.leftIndex.calcT < 0.25f && this.leftMiddle.calcT < 0.25f;
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x000A76F8 File Offset: 0x000A58F8
	public bool IsMakingFiveRight()
	{
		if (this.isOfflineVRRig)
		{
			return ControllerInputPoller.GripFloat(XRNode.RightHand) < 0.25f && ControllerInputPoller.TriggerFloat(XRNode.RightHand) < 0.25f;
		}
		return this.rightIndex.calcT < 0.25f && this.rightMiddle.calcT < 0.25f;
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x000A7750 File Offset: 0x000A5950
	public VRMap GetMakingFist(bool debug, out bool isLeftHand)
	{
		if (this.IsMakingFistRight())
		{
			isLeftHand = false;
			return this.rightHand;
		}
		if (this.IsMakingFistLeft())
		{
			isLeftHand = true;
			return this.leftHand;
		}
		isLeftHand = false;
		return null;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x000A777C File Offset: 0x000A597C
	public void PlayGeodeEffect(Vector3 hitPosition)
	{
		if ((base.transform.position - hitPosition).sqrMagnitude < 9f && this.geodeCrackingSound)
		{
			this.geodeCrackingSound.GTPlay();
		}
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x000A77C4 File Offset: 0x000A59C4
	public void PlayClimbSound(AudioClip clip, bool isLeftHand)
	{
		if (isLeftHand)
		{
			this.leftHandPlayer.volume = 0.1f;
			this.leftHandPlayer.clip = clip;
			this.leftHandPlayer.GTPlayOneShot(this.leftHandPlayer.clip, 1f);
			return;
		}
		this.rightHandPlayer.volume = 0.1f;
		this.rightHandPlayer.clip = clip;
		this.rightHandPlayer.GTPlayOneShot(this.rightHandPlayer.clip, 1f);
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x000A7844 File Offset: 0x000A5A44
	public void HideAllCosmetics(PhotonMessageInfo info)
	{
		this.IncrementRPC(info, "HideAllCosmetics");
		if (NetworkSystem.Instance.GetPlayer(info.Sender) == this.netView.Owner)
		{
			this.LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet.EmptySet, CosmeticsController.CosmeticSet.EmptySet, false);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent update cosmetics", info.Sender.UserId, info.Sender.NickName);
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000A78B4 File Offset: 0x000A5AB4
	public void UpdateCosmeticsWithTryon(string[] currentItems, string[] tryOnItems, bool playfx, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && currentItems.Length == 16 && tryOnItems.Length == 16)
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItems, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItems, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet, playfx);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000A7948 File Offset: 0x000A5B48
	public void UpdateCosmeticsWithTryon(int[] currentItemsPacked, int[] tryOnItemsPacked, bool playfx, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithTryon");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.Sender == this.netView.Owner && CosmeticsController.instance.ValidatePackedItems(currentItemsPacked) && CosmeticsController.instance.ValidatePackedItems(tryOnItemsPacked))
		{
			CosmeticsController.CosmeticSet newSet = new CosmeticsController.CosmeticSet(currentItemsPacked, CosmeticsController.instance);
			CosmeticsController.CosmeticSet newTryOnSet = new CosmeticsController.CosmeticSet(tryOnItemsPacked, CosmeticsController.instance);
			this.LocalUpdateCosmeticsWithTryon(newSet, newTryOnSet, playfx);
			return;
		}
		MonkeAgent.instance.SendReport("inappropriate tag data being sent update cosmetics with tryon", player.UserId, player.NickName);
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x000A79EC File Offset: 0x000A5BEC
	public void UpdateCosmeticsWithCollectables(int[] cycleStatesPacked, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_UpdateCosmeticsWithCollectablesPacked");
		if (info.Sender != this.netView.Owner || cycleStatesPacked == null || cycleStatesPacked.Length % 2 != 0 || cycleStatesPacked.Length > 64)
		{
			return;
		}
		int num = cycleStatesPacked.Length / 2;
		this.remoteCycleStates.Clear();
		char[] array = new char[]
		{
			'\0',
			'\0',
			'\0',
			'\0',
			'\0',
			'.'
		};
		for (int i = 0; i < num; i++)
		{
			int num2 = cycleStatesPacked[i * 2];
			int num3 = cycleStatesPacked[i * 2 + 1];
			if (num3 >= 0)
			{
				array[0] = (char)(65 + num2 % 26);
				array[1] = (char)(65 + num2 / 26 % 26);
				array[2] = (char)(65 + num2 / 676 % 26);
				array[3] = (char)(65 + num2 / 17576 % 26);
				array[4] = (char)(65 + num2 / 456976 % 26);
				string text = new string(array);
				this.remoteCycleStates[text] = num3;
				CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.FindForRig(base.GetInstanceID(), text);
				if (cosmeticCollectionDisplay != null)
				{
					cosmeticCollectionDisplay.SetActiveIndex(num3);
				}
			}
		}
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x000A7AEC File Offset: 0x000A5CEC
	public void SetCollectionCycleIndex(int packedParentID, int activeIndex, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "RPC_SetCollectionCycleIndex");
		if (info.Sender != this.netView.Owner)
		{
			return;
		}
		char[] array = new char[]
		{
			'\0',
			'\0',
			'\0',
			'\0',
			'\0',
			'.'
		};
		array[0] = (ushort)(65 + packedParentID % 26);
		array[1] = (ushort)(65 + packedParentID / 26 % 26);
		array[2] = (ushort)(65 + packedParentID / 676 % 26);
		array[3] = (ushort)(65 + packedParentID / 17576 % 26);
		array[4] = (ushort)(65 + packedParentID / 456976 % 26);
		string text = new string(array);
		this.remoteCycleStates[text] = activeIndex;
		CosmeticCollectionDisplay cosmeticCollectionDisplay = CosmeticCollectionDisplay.FindForRig(base.GetInstanceID(), text);
		if (cosmeticCollectionDisplay == null)
		{
			return;
		}
		cosmeticCollectionDisplay.SetActiveIndex(activeIndex);
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x000A7B9A File Offset: 0x000A5D9A
	public void LocalUpdateCosmeticsWithTryon(CosmeticsController.CosmeticSet newSet, CosmeticsController.CosmeticSet newTryOnSet, bool playfx)
	{
		this.cosmeticSet = newSet;
		this.tryOnSet = newTryOnSet;
		if (this.initializedCosmetics)
		{
			this.SetCosmeticsActive(playfx);
		}
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x000A7BBC File Offset: 0x000A5DBC
	private void CheckForEarlyAccess()
	{
		CosmeticInfoV2 info = CosmeticsController.instance.EarlyAccessSupporterPackCosmeticSO.info;
		if (this._playerOwnedCosmetics.Contains(info.playFabID))
		{
			CosmeticSO[] setCosmetics = info.setCosmetics;
			for (int i = 0; i < setCosmetics.Length; i++)
			{
				CosmeticInfoV2 info2 = setCosmetics[i].info;
				this._playerOwnedCosmetics.Add(info2.playFabID);
			}
		}
		this.InitializedCosmetics = true;
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000A7C28 File Offset: 0x000A5E28
	public void SetCosmeticsActive(bool playfx)
	{
		if (CosmeticsController.instance == null)
		{
			return;
		}
		this.prevSet.CopyItems(this.mergedSet);
		this.mergedSet.MergeSets(this.inTryOnRoom ? this.tryOnSet : null, this.cosmeticSet);
		BodyDockPositions component = base.GetComponent<BodyDockPositions>();
		this.mergedSet.ActivateCosmetics(this.prevSet, this, component, this.cosmeticsObjectRegistry);
		if (!playfx)
		{
			return;
		}
		if (this.cosmeticsActivationPS != null)
		{
			this.cosmeticsActivationPS.Play();
		}
		if (this.cosmeticsActivationSBP != null)
		{
			this.cosmeticsActivationSBP.Play();
		}
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x000A7CCE File Offset: 0x000A5ECE
	public void RefreshCosmetics()
	{
		this.mergedSet.ActivateCosmetics(this.mergedSet, this, this.myBodyDockPositions, this.cosmeticsObjectRegistry);
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x000A7CF0 File Offset: 0x000A5EF0
	public void GetCosmeticsPlayFabCatalogData()
	{
		if (CosmeticsController.instance != null)
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), delegate(GetUserInventoryResult result)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				foreach (ItemInstance itemInstance in result.Inventory)
				{
					if (!dictionary.ContainsKey(itemInstance.ItemId))
					{
						dictionary[itemInstance.ItemId] = itemInstance.ItemId;
						if (itemInstance.CatalogVersion == CosmeticsController.instance.catalog)
						{
							this.AddCosmetic(itemInstance.ItemId);
							if (itemInstance.PurchaseDate != null)
							{
								this._playerOwnedCosmeticsAge[itemInstance.ItemId] = (int)(DateTime.UtcNow - itemInstance.PurchaseDate.Value).TotalDays;
							}
						}
					}
				}
			}, delegate(PlayFabError error)
			{
				this.initializedCosmetics = true;
			}, null, null);
		}
		this.AddCosmetic("Slingshot");
		foreach (BuilderPieceSet builderPieceSet in BuilderSetManager.instance.StartPieceSets)
		{
			this.AddCosmetic(builderPieceSet.playfabID);
		}
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000A7D8C File Offset: 0x000A5F8C
	public void GenerateFingerAngleLookupTables()
	{
		this.GenerateTableIndex(ref this.leftIndex);
		this.GenerateTableIndex(ref this.rightIndex);
		this.GenerateTableMiddle(ref this.leftMiddle);
		this.GenerateTableMiddle(ref this.rightMiddle);
		this.GenerateTableThumb(ref this.leftThumb);
		this.GenerateTableThumb(ref this.rightThumb);
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x000A7DE4 File Offset: 0x000A5FE4
	private void GenerateTableThumb(ref VRMapThumb thumb)
	{
		thumb.angle1Table = new Quaternion[11];
		thumb.angle2Table = new Quaternion[11];
		for (int i = 0; i < thumb.angle1Table.Length; i++)
		{
			thumb.angle1Table[i] = Quaternion.Lerp(thumb.startingAngle1Quat, thumb.closedAngle1Quat, (float)i / 10f);
			thumb.angle2Table[i] = Quaternion.Lerp(thumb.startingAngle2Quat, thumb.closedAngle2Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000A7E74 File Offset: 0x000A6074
	private void GenerateTableIndex(ref VRMapIndex index)
	{
		index.angle1Table = new Quaternion[11];
		index.angle2Table = new Quaternion[11];
		index.angle3Table = new Quaternion[11];
		for (int i = 0; i < index.angle1Table.Length; i++)
		{
			index.angle1Table[i] = Quaternion.Lerp(index.startingAngle1Quat, index.closedAngle1Quat, (float)i / 10f);
			index.angle2Table[i] = Quaternion.Lerp(index.startingAngle2Quat, index.closedAngle2Quat, (float)i / 10f);
			index.angle3Table[i] = Quaternion.Lerp(index.startingAngle3Quat, index.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x000A7F3C File Offset: 0x000A613C
	private void GenerateTableMiddle(ref VRMapMiddle middle)
	{
		middle.angle1Table = new Quaternion[11];
		middle.angle2Table = new Quaternion[11];
		middle.angle3Table = new Quaternion[11];
		for (int i = 0; i < middle.angle1Table.Length; i++)
		{
			middle.angle1Table[i] = Quaternion.Lerp(middle.startingAngle1Quat, middle.closedAngle1Quat, (float)i / 10f);
			middle.angle2Table[i] = Quaternion.Lerp(middle.startingAngle2Quat, middle.closedAngle2Quat, (float)i / 10f);
			middle.angle3Table[i] = Quaternion.Lerp(middle.startingAngle3Quat, middle.closedAngle3Quat, (float)i / 10f);
		}
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x000A8004 File Offset: 0x000A6204
	private Quaternion SanitizeQuaternion(Quaternion quat)
	{
		if (float.IsNaN(quat.w) || float.IsNaN(quat.x) || float.IsNaN(quat.y) || float.IsNaN(quat.z) || float.IsInfinity(quat.w) || float.IsInfinity(quat.x) || float.IsInfinity(quat.y) || float.IsInfinity(quat.z))
		{
			return Quaternion.identity;
		}
		return quat;
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000A8080 File Offset: 0x000A6280
	private Vector3 SanitizeVector3(Vector3 vec)
	{
		if (float.IsNaN(vec.x) || float.IsNaN(vec.y) || float.IsNaN(vec.z) || float.IsInfinity(vec.x) || float.IsInfinity(vec.y) || float.IsInfinity(vec.z))
		{
			return Vector3.zero;
		}
		return Vector3.ClampMagnitude(vec, 5000f);
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000A80EC File Offset: 0x000A62EC
	private void IncrementRPC(PhotonMessageInfoWrapped info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			MonkeAgent.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x000A8102 File Offset: 0x000A6302
	private void IncrementRPC(PhotonMessageInfo info, string sourceCall)
	{
		if (GorillaGameManager.instance != null)
		{
			MonkeAgent.IncrementRPCCall(info, sourceCall);
		}
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x000A8118 File Offset: 0x000A6318
	private void AddVelocityToQueue(Vector3 position, double serverTime)
	{
		Vector3 velocity = Vector3.zero;
		if (this.velocityHistoryList.Count > 0)
		{
			double num = Utils.CalculateNetworkDeltaTime(this.velocityHistoryList[0].time, serverTime);
			if (num == 0.0)
			{
				return;
			}
			velocity = (position - this.lastPosition) / (float)num;
		}
		this.velocityHistoryList.Add(new VRRig.VelocityTime(velocity, serverTime));
		this.lastPosition = position;
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x000A818C File Offset: 0x000A638C
	private Vector3 ReturnVelocityAtTime(double timeToReturn)
	{
		if (this.velocityHistoryList.Count <= 1)
		{
			return Vector3.zero;
		}
		int num = 0;
		int num2 = this.velocityHistoryList.Count - 1;
		int num3 = 0;
		if (num2 == num)
		{
			return this.velocityHistoryList[num].vel;
		}
		while (num2 - num > 1 && num3 < 1000)
		{
			num3++;
			int num4 = (num2 - num) / 2;
			if (this.velocityHistoryList[num4].time > timeToReturn)
			{
				num2 = num4;
			}
			else
			{
				num = num4;
			}
		}
		float num5 = (float)(this.velocityHistoryList[num].time - timeToReturn);
		double num6 = this.velocityHistoryList[num].time - this.velocityHistoryList[num2].time;
		if (num6 == 0.0)
		{
			num6 = 0.001;
		}
		num5 /= (float)num6;
		num5 = Mathf.Clamp(num5, 0f, 1f);
		return Vector3.Lerp(this.velocityHistoryList[num].vel, this.velocityHistoryList[num2].vel, num5);
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x000A829E File Offset: 0x000A649E
	public Vector3 LatestVelocity()
	{
		if (this.velocityHistoryList.Count > 0)
		{
			return this.velocityHistoryList[0].vel;
		}
		return Vector3.zero;
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000A82C5 File Offset: 0x000A64C5
	public bool IsPositionInRange(Vector3 position, float range)
	{
		return (this.syncPos - position).IsShorterThan(range * this.scaleFactor);
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x000A82E0 File Offset: 0x000A64E0
	public bool CheckTagDistanceRollback(VRRig otherRig, float max, float timeInterval)
	{
		Vector3 a;
		Vector3 b;
		GorillaMath.LineSegClosestPoints(this.syncPos, -this.LatestVelocity() * timeInterval, otherRig.syncPos, -otherRig.LatestVelocity() * timeInterval, out a, out b);
		return Vector3.SqrMagnitude(a - b) < max * max * this.scaleFactor;
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000A833C File Offset: 0x000A653C
	public Vector3 ClampVelocityRelativeToPlayerSafe(Vector3 inVel, float max, float teleportSpeedThreshold = 100f)
	{
		max *= this.scaleFactor;
		Vector3 vector = Vector3.zero;
		ref vector.SetValueSafe(inVel);
		Vector3 vector2 = (this.velocityHistoryList.Count > 0) ? this.velocityHistoryList[0].vel : Vector3.zero;
		if (vector2.sqrMagnitude > teleportSpeedThreshold * teleportSpeedThreshold)
		{
			vector2 = Vector3.zero;
		}
		Vector3 vector3 = vector - vector2;
		vector3 = Vector3.ClampMagnitude(vector3, max);
		vector = vector2 + vector3;
		return vector;
	}

	// Token: 0x14000044 RID: 68
	// (add) Token: 0x06001F4C RID: 8012 RVA: 0x000A83B4 File Offset: 0x000A65B4
	// (remove) Token: 0x06001F4D RID: 8013 RVA: 0x000A83EC File Offset: 0x000A65EC
	public event Action<Color> OnColorChanged;

	// Token: 0x14000045 RID: 69
	// (add) Token: 0x06001F4E RID: 8014 RVA: 0x000A8424 File Offset: 0x000A6624
	// (remove) Token: 0x06001F4F RID: 8015 RVA: 0x000A845C File Offset: 0x000A665C
	public event Action OnPlayerNameVisibleChanged;

	// Token: 0x06001F50 RID: 8016 RVA: 0x000A8494 File Offset: 0x000A6694
	public void SetColor(Color color)
	{
		Action<Color> onColorChanged = this.OnColorChanged;
		if (onColorChanged != null)
		{
			onColorChanged(color);
		}
		Action<Color> action = this.onColorInitialized;
		if (action != null)
		{
			action(color);
		}
		this.onColorInitialized = delegate(Color color1)
		{
		};
		this.colorInitialized = true;
		this.playerColor = color;
		if (this.OnDataChange != null)
		{
			this.OnDataChange();
		}
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x000A850B File Offset: 0x000A670B
	public void OnColorInitialized(Action<Color> action)
	{
		if (this.colorInitialized)
		{
			action(this.playerColor);
			return;
		}
		this.onColorInitialized = (Action<Color>)Delegate.Combine(this.onColorInitialized, action);
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x000A8539 File Offset: 0x000A6739
	private void SendScoresToRoom()
	{
		if (this.netView != null && this._scoreUpdated)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.Others, new object[]
			{
				this.currentQuestScore
			});
		}
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x000A8578 File Offset: 0x000A6778
	private void SendScoresToGameModeRoom(GameModeType newGameModeType)
	{
		if (this.netView != null && this._rankedInfoUpdated && newGameModeType != GameModeType.InfectionCompetitive && !this.m_sentRankedScore)
		{
			this.m_sentRankedScore = true;
			this.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[]
			{
				this.currentRankedELO,
				this.currentRankedSubTierQuest,
				this.currentRankedSubTierPC
			});
		}
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000A85F0 File Offset: 0x000A67F0
	private void SendScoresToNewPlayer(NetPlayer player)
	{
		if (this.netView != null)
		{
			if (this._scoreUpdated)
			{
				this.netView.SendRPC("RPC_UpdateQuestScore", player, new object[]
				{
					this.currentQuestScore
				});
			}
			if (this._rankedInfoUpdated && !this.IsInRankedMode())
			{
				this.netView.SendRPC("RPC_UpdateRankedInfo", player, new object[]
				{
					this.currentRankedELO,
					this.currentRankedSubTierQuest,
					this.currentRankedSubTierPC
				});
			}
		}
	}

	// Token: 0x14000046 RID: 70
	// (add) Token: 0x06001F55 RID: 8021 RVA: 0x000A868C File Offset: 0x000A688C
	// (remove) Token: 0x06001F56 RID: 8022 RVA: 0x000A86C4 File Offset: 0x000A68C4
	public event Action<int> OnQuestScoreChanged;

	// Token: 0x06001F57 RID: 8023 RVA: 0x000A86FC File Offset: 0x000A68FC
	public void SetQuestScore(int score)
	{
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged != null)
		{
			onQuestScoreChanged(this.currentQuestScore);
		}
		if (this.netView != null)
		{
			this.netView.SendRPC("RPC_UpdateQuestScore", RpcTarget.Others, new object[]
			{
				this.currentQuestScore
			});
		}
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x000A875A File Offset: 0x000A695A
	public int GetCurrentQuestScore()
	{
		if (!this._scoreUpdated)
		{
			this.SetQuestScoreLocal(ProgressionController.TotalPoints);
		}
		return this.currentQuestScore;
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x000A8775 File Offset: 0x000A6975
	private void SetQuestScoreLocal(int score)
	{
		this.currentQuestScore = score;
		this._scoreUpdated = true;
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x000A8788 File Offset: 0x000A6988
	public void UpdateQuestScore(int score, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateQuestScore");
		NetworkSystem.Instance.GetPlayer(info.senderID);
		if (info.senderID != this.creator.ActorNumber)
		{
			return;
		}
		if (!this.updateQuestCallLimit.CheckCallTime(Time.time))
		{
			return;
		}
		if (score < this.currentQuestScore)
		{
			return;
		}
		this.SetQuestScoreLocal(score);
		Action<int> onQuestScoreChanged = this.OnQuestScoreChanged;
		if (onQuestScoreChanged == null)
		{
			return;
		}
		onQuestScoreChanged(this.currentQuestScore);
	}

	// Token: 0x14000047 RID: 71
	// (add) Token: 0x06001F5B RID: 8027 RVA: 0x000A8800 File Offset: 0x000A6A00
	// (remove) Token: 0x06001F5C RID: 8028 RVA: 0x000A8838 File Offset: 0x000A6A38
	public event Action<int, int> OnRankedSubtierChanged;

	// Token: 0x06001F5D RID: 8029 RVA: 0x000A8870 File Offset: 0x000A6A70
	public void SetRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, bool broadcastToOtherClients = true)
	{
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged(rankedSubtierQuest, rankedSubtierPC);
		}
		if (this.netView != null && broadcastToOtherClients)
		{
			this.netView.SendRPC("RPC_UpdateRankedInfo", RpcTarget.Others, new object[]
			{
				this.currentRankedELO,
				this.currentRankedSubTierQuest,
				this.currentRankedSubTierPC
			});
		}
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x000A88EB File Offset: 0x000A6AEB
	public int GetCurrentRankedSubTier(bool getPC)
	{
		if (!this._rankedInfoUpdated)
		{
			return -1;
		}
		if (!getPC)
		{
			return this.currentRankedSubTierQuest;
		}
		return this.currentRankedSubTierPC;
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000A8907 File Offset: 0x000A6B07
	private void SetRankedInfoLocal(float rankedELO, int rankedSubTierQuest, int rankedSubTierPC)
	{
		this.currentRankedELO = rankedELO;
		this.currentRankedSubTierQuest = rankedSubTierQuest;
		this.currentRankedSubTierPC = rankedSubTierPC;
		this._rankedInfoUpdated = true;
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x000A8925 File Offset: 0x000A6B25
	private bool IsInRankedMode()
	{
		return GorillaGameModes.GameMode.ActiveGameMode != null && GorillaGameModes.GameMode.ActiveGameMode.GameType() == GameModeType.InfectionCompetitive;
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x000A8944 File Offset: 0x000A6B44
	public void UpdateRankedInfo(float rankedELO, int rankedSubtierQuest, int rankedSubtierPC, PhotonMessageInfoWrapped info)
	{
		this.IncrementRPC(info, "UpdateRankedInfo");
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.senderID);
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return;
		}
		if (!rigContainer.Rig.updateRankedInfoCallLimit.CheckCallTime(Time.time) || info.senderID != this.creator.ActorNumber || !float.IsFinite(rankedELO))
		{
			return;
		}
		if (this.IsInRankedMode())
		{
			return;
		}
		if (RankedProgressionManager.Instance == null || !RankedProgressionManager.Instance.AreValuesValid(rankedELO, rankedSubtierQuest, rankedSubtierPC))
		{
			return;
		}
		this.SetRankedInfoLocal(rankedELO, rankedSubtierQuest, rankedSubtierPC);
		Action<int, int> onRankedSubtierChanged = this.OnRankedSubtierChanged;
		if (onRankedSubtierChanged != null)
		{
			onRankedSubtierChanged(rankedSubtierQuest, rankedSubtierPC);
		}
		RankedProgressionManager.Instance.HandlePlayerRankedInfoReceived(this.creator.ActorNumber, rankedELO, rankedSubtierPC);
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x000A8A14 File Offset: 0x000A6C14
	public void OnEnable()
	{
		EyeScannerMono.Register(this);
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnSubscriptionData));
		GorillaComputer.RegisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
		if (this.currentRopeSwingTarget != null)
		{
			this.currentRopeSwingTarget.SetParent(null);
		}
		if (!this.isOfflineVRRig)
		{
			PlayerCosmeticsSystem.RegisterCosmeticCallback(this.creator.ActorNumber, this);
		}
		this.bodyRenderer.SetDefaults();
		this.SetInvisibleToLocalPlayer(false);
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride += this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride += this.HandHold_HandPositionReleaseOverride;
			GorillaGameModes.GameMode.OnStartGameMode += this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent += new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.RegisterVRRig(this);
		}
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		TickSystem<object>.AddPostTickCallback(this);
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x000A8B28 File Offset: 0x000A6D28
	public void OnSubscriptionData()
	{
		if (!this.isOfflineVRRig)
		{
			return;
		}
		this.showGoldNameTag = (SubscriptionManager.IsLocalSubscribed() && PlayerPrefs.GetInt(SubscriptionManager.GetSubsFeatureKey(SubscriptionManager.SubscriptionFeatures.GoldenName)) > 0);
		if (this.showGoldNameTag)
		{
			this.playerText1.color = SubscriptionManager.SUBSCRIBER_NAME_COLOR;
			return;
		}
		this.playerText1.color = Color.white;
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x000A8B88 File Offset: 0x000A6D88
	void IPreDisable.PreDisable()
	{
		try
		{
			this.ClearRopeData();
			if (this.currentRopeSwingTarget)
			{
				this.currentRopeSwingTarget.SetParent(base.transform);
			}
			this.EnableHuntWatch(false);
			this.EnablePaintbrawlCosmetics(false);
			this.EnableSuperInfectionHands(false);
			this.ClearPartyMemberStatus();
			this._playerOwnedCosmetics.Clear();
			this._playerOwnedCosmeticsAge.Clear();
			if (this.cosmeticSet != null)
			{
				this.mergedSet.DeactivateAllCosmetcs(this.myBodyDockPositions, CosmeticsController.instance.nullItem, this.cosmeticsObjectRegistry);
				this.mergedSet.ClearSet(CosmeticsController.instance.nullItem);
				this.prevSet.ClearSet(CosmeticsController.instance.nullItem);
				this.tryOnSet.ClearSet(CosmeticsController.instance.nullItem);
				this.cosmeticSet.ClearSet(CosmeticsController.instance.nullItem);
			}
			if (!this.isOfflineVRRig)
			{
				PlayerCosmeticsSystem.RemoveCosmeticCallback(this.creator.ActorNumber);
				this.pendingCosmeticUpdate = true;
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.leftHandLink.BreakLinkTo(this.rightHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.leftHandLink);
				VRRig.LocalRig.rightHandLink.BreakLinkTo(this.rightHandLink);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000A8D08 File Offset: 0x000A6F08
	public void OnDisable()
	{
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnSubscriptionData));
		try
		{
			GorillaSkin.ApplyToRig(this, null, GorillaSkin.SkinType.gameMode);
			this.ChangeMaterialLocal(0);
			GorillaComputer.UnregisterOnNametagSettingChanged(new Action<bool>(this.UpdateName));
			this.netView = null;
			this.voiceAudio = null;
			this.muted = false;
			this.initialized = false;
			this.initializedCosmetics = false;
			this.inTryOnRoom = false;
			this.inTempCosmSpace = false;
			this.timeSpawned = 0f;
			this.setMatIndex = 0;
			this.currentCosmeticTries = 0;
			this.velocityHistoryList.Clear();
			this.netSyncPos.Reset();
			this.rightHand.netSyncPos.Reset();
			this.leftHand.netSyncPos.Reset();
			this.ForceResetFrozenEffect();
			this.nativeScale = (this.frameScale = (this.lastScaleFactor = 1f));
			base.transform.localScale = Vector3.one;
			this.currentQuestScore = 0;
			this._scoreUpdated = false;
			this.currentRankedELO = 0f;
			this.currentRankedSubTierQuest = 0;
			this.currentRankedSubTierPC = 0;
			this._rankedInfoUpdated = false;
			this.TemporaryCosmeticEffects.Clear();
			this.m_sentRankedScore = false;
			if (this.inDuplicationZone)
			{
				this.ClearDuplicationZone(this.duplicationZone);
			}
			try
			{
				CallLimitType<CallLimiter>[] callSettings = this.fxSettings.callSettings;
				for (int i = 0; i < callSettings.Length; i++)
				{
					callSettings[i].CallLimitSettings.Reset();
				}
			}
			catch
			{
				Debug.LogError("fxtype missing in fxSettings, please fix or remove this");
			}
		}
		catch (Exception)
		{
		}
		if (this.isOfflineVRRig)
		{
			HandHold.HandPositionRequestOverride -= this.HandHold_HandPositionRequestOverride;
			HandHold.HandPositionReleaseOverride -= this.HandHold_HandPositionReleaseOverride;
			GorillaGameModes.GameMode.OnStartGameMode -= this.SendScoresToGameModeRoom;
			RoomSystem.JoinedRoomEvent -= new Action(this.SendScoresToRoom);
			RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.SendScoresToNewPlayer);
		}
		else
		{
			VRRigJobManager.Instance.DeregisterVRRig(this);
		}
		EyeScannerMono.Unregister(this);
		this.creator = null;
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		TickSystem<object>.RemovePostTickCallback(this);
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x000A8F68 File Offset: 0x000A7168
	private void HandHold_HandPositionReleaseOverride(HandHold hh, bool leftHand)
	{
		if (leftHand)
		{
			this.leftHand.handholdOverrideTarget = null;
			return;
		}
		this.rightHand.handholdOverrideTarget = null;
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x000A8F86 File Offset: 0x000A7186
	private void HandHold_HandPositionRequestOverride(HandHold hh, bool leftHand, Vector3 pos)
	{
		if (leftHand)
		{
			this.leftHand.handholdOverrideTarget = hh.transform;
			this.leftHand.handholdOverrideTargetOffset = pos;
			return;
		}
		this.rightHand.handholdOverrideTarget = hh.transform;
		this.rightHand.handholdOverrideTargetOffset = pos;
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x000A8FC8 File Offset: 0x000A71C8
	public void NetInitialize()
	{
		this.timeSpawned = Time.time;
		if (NetworkSystem.Instance.InRoom)
		{
			GorillaGameManager instance = GorillaGameManager.instance;
			if (instance != null)
			{
				if (instance is GorillaHuntManager || instance.GameModeName() == "HUNT")
				{
					this.EnableHuntWatch(true);
				}
				else if (instance is GorillaPaintbrawlManager || instance.GameModeName() == "PAINTBRAWL")
				{
					this.EnablePaintbrawlCosmetics(true);
				}
			}
			else
			{
				string gameModeString = NetworkSystem.Instance.GameModeString;
				if (!gameModeString.IsNullOrEmpty())
				{
					string text = gameModeString;
					if (text.Contains("HUNT"))
					{
						this.EnableHuntWatch(true);
					}
					else if (text.Contains("PAINTBRAWL"))
					{
						this.EnablePaintbrawlCosmetics(true);
					}
				}
			}
			this.UpdateFriendshipBracelet();
			if (this.IsLocalPartyMember && !this.isOfflineVRRig)
			{
				FriendshipGroupDetection.Instance.SendVerifyPartyMember(this.creator);
			}
		}
		if (this.netView != null)
		{
			base.transform.position = this.netView.gameObject.transform.position;
			base.transform.rotation = this.netView.gameObject.transform.rotation;
		}
		try
		{
			Action action = VRRig.newPlayerJoined;
			if (action != null)
			{
				action();
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x000A9124 File Offset: 0x000A7324
	public void GrabbedByPlayer(VRRig grabbedByRig, bool grabbedBody, bool grabbedLeftHand, bool grabbedWithLeftHand)
	{
		GorillaClimbable climbable = grabbedWithLeftHand ? grabbedByRig.leftHandHoldsPlayer : grabbedByRig.rightHandHoldsPlayer;
		GorillaHandClimber gorillaHandClimber;
		if (grabbedBody)
		{
			gorillaHandClimber = EquipmentInteractor.instance.BodyClimber;
		}
		else if (grabbedLeftHand)
		{
			gorillaHandClimber = EquipmentInteractor.instance.LeftClimber;
		}
		else
		{
			gorillaHandClimber = EquipmentInteractor.instance.RightClimber;
		}
		gorillaHandClimber.SetCanRelease(false);
		GTPlayer.Instance.BeginClimbing(climbable, gorillaHandClimber, null);
		this.grabbedRopeIsBody = grabbedBody;
		this.grabbedRopeIsLeft = grabbedLeftHand;
		this.grabbedRopeIndex = grabbedByRig.netView.ViewID;
		this.grabbedRopeBoneIndex = (grabbedWithLeftHand ? 1 : 0);
		this.grabbedRopeOffset = Vector3.zero;
		this.grabbedRopeIsPhotonView = true;
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x000A91C8 File Offset: 0x000A73C8
	public void DroppedByPlayer(VRRig grabbedByRig, Vector3 throwVelocity)
	{
		GorillaClimbable currentClimbable = GTPlayer.Instance.CurrentClimbable;
		if (GTPlayer.Instance.isClimbing && (currentClimbable == grabbedByRig.leftHandHoldsPlayer || currentClimbable == grabbedByRig.rightHandHoldsPlayer))
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 20f);
			GorillaHandClimber currentClimber = GTPlayer.Instance.CurrentClimber;
			GTPlayer.Instance.EndClimbing(currentClimber, false, false);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			this.grabbedRopeIsBody = false;
			this.grabbedRopeIsLeft = false;
			this.grabbedRopeIndex = -1;
			this.grabbedRopeBoneIndex = 0;
			this.grabbedRopeOffset = Vector3.zero;
			this.grabbedRopeIsPhotonView = false;
			return;
		}
		if (VRRig.LocalRig.leftHandLink.IsLinkActive() && VRRig.LocalRig.leftHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.leftHandLink.BreakLink();
			VRRig.LocalRig.leftHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
			return;
		}
		if (VRRig.LocalRig.rightHandLink.IsLinkActive() && VRRig.LocalRig.rightHandLink.grabbedLink.myRig == grabbedByRig)
		{
			throwVelocity = Vector3.ClampMagnitude(throwVelocity, 3f);
			VRRig.LocalRig.rightHandLink.BreakLink();
			VRRig.LocalRig.rightHandLink.RejectGrabsFor(1f);
			GTPlayer.Instance.SetVelocity(throwVelocity);
		}
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x000A9338 File Offset: 0x000A7538
	public bool IsOnGround(float headCheckDistance, float handCheckDistance, out Vector3 groundNormal)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(position, Vector3.down * headCheckDistance * this.scaleFactor, instance.headCollider.radius * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		if (this.LocalCheckCollision(position2, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		if (this.LocalCheckCollision(position3, Vector3.down * handCheckDistance * this.scaleFactor, instance.minimumRaycastDistance * this.scaleFactor, out vector, out raycastHit))
		{
			groundNormal = raycastHit.normal;
			return true;
		}
		groundNormal = Vector3.up;
		return false;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x000A944C File Offset: 0x000A764C
	private bool LocalTestMovementCollision(Vector3 startPosition, Vector3 startVelocity, out Vector3 modifiedVelocity, out Vector3 finalPosition)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 vector = startVelocity * Time.deltaTime;
		finalPosition = startPosition + vector;
		modifiedVelocity = startVelocity;
		Vector3 a;
		RaycastHit raycastHit;
		bool flag = this.LocalCheckCollision(startPosition, vector, instance.headCollider.radius * this.scaleFactor, out a, out raycastHit);
		if (flag)
		{
			finalPosition = a - vector.normalized * 0.01f;
			modifiedVelocity = startVelocity - raycastHit.normal * Vector3.Dot(raycastHit.normal, startVelocity);
		}
		Vector3 position = this.leftHand.rigTarget.position;
		Vector3 a2;
		RaycastHit raycastHit2;
		bool flag2 = this.LocalCheckCollision(position, vector, instance.minimumRaycastDistance * this.scaleFactor, out a2, out raycastHit2);
		if (flag2)
		{
			finalPosition = a2 - (this.leftHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		Vector3 position2 = this.rightHand.rigTarget.position;
		Vector3 a3;
		RaycastHit raycastHit3;
		bool flag3 = this.LocalCheckCollision(position2, vector, instance.minimumRaycastDistance * this.scaleFactor, out a3, out raycastHit3);
		if (flag3)
		{
			finalPosition = a3 - (this.rightHand.rigTarget.position - startPosition) - vector.normalized * 0.01f;
			modifiedVelocity = Vector3.zero;
		}
		return flag || flag2 || flag3;
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x000A95DC File Offset: 0x000A77DC
	public void TrySweptMoveTo(Vector3 targetPosition, out bool handCollided, out bool buttCollided)
	{
		Vector3 position = base.transform.position;
		this.TrySweptOffsetMove(targetPosition - position, out handCollided, out buttCollided);
	}

	// Token: 0x06001F6E RID: 8046 RVA: 0x000A9604 File Offset: 0x000A7804
	public void TrySweptOffsetMove(Vector3 movement, out bool handCollided, out bool buttCollided)
	{
		GTPlayer instance = GTPlayer.Instance;
		Vector3 position = base.transform.position;
		Vector3 vector = position + movement;
		Vector3 startPosition = position;
		handCollided = false;
		buttCollided = false;
		Vector3 a;
		RaycastHit raycastHit;
		if (this.LocalCheckCollision(startPosition, movement, instance.headCollider.radius * this.scaleFactor, out a, out raycastHit))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = a - movement.normalized * 0.01f;
			}
			movement = vector - position;
			buttCollided = true;
		}
		Vector3 position2 = this.leftHand.rigTarget.position;
		Vector3 a2;
		RaycastHit raycastHit2;
		if (this.LocalCheckCollision(position2, movement, instance.minimumRaycastDistance * this.scaleFactor, out a2, out raycastHit2))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = a2 - (this.leftHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		Vector3 position3 = this.rightHand.rigTarget.position;
		Vector3 a3;
		RaycastHit raycastHit3;
		if (this.LocalCheckCollision(position3, movement, instance.minimumRaycastDistance * this.scaleFactor, out a3, out raycastHit3))
		{
			if (movement.IsShorterThan(0.01f))
			{
				vector = position;
			}
			else
			{
				vector = a3 - (this.rightHand.rigTarget.position - position) - movement.normalized * 0.01f;
			}
			movement = vector - position;
			handCollided = true;
		}
		base.transform.position = vector;
	}

	// Token: 0x06001F6F RID: 8047 RVA: 0x000A9794 File Offset: 0x000A7994
	private bool LocalCheckCollision(Vector3 startPosition, Vector3 movement, float radius, out Vector3 finalPosition, out RaycastHit hit)
	{
		GTPlayer instance = GTPlayer.Instance;
		finalPosition = startPosition + movement;
		RaycastHit raycastHit = default(RaycastHit);
		bool flag = false;
		Vector3 normalized = movement.normalized;
		int num = Physics.SphereCastNonAlloc(startPosition, radius, normalized, this.rayCastNonAllocColliders, movement.magnitude, instance.locomotionEnabledLayers.value);
		if (num > 0)
		{
			raycastHit = this.rayCastNonAllocColliders[0];
			for (int i = 0; i < num; i++)
			{
				if (raycastHit.distance > 0f && (!flag || this.rayCastNonAllocColliders[i].distance < raycastHit.distance))
				{
					flag = true;
					raycastHit = this.rayCastNonAllocColliders[i];
				}
			}
		}
		hit = raycastHit;
		if (flag)
		{
			finalPosition = startPosition + normalized * (raycastHit.distance - 0.01f);
			return true;
		}
		return false;
	}

	// Token: 0x06001F70 RID: 8048 RVA: 0x000A9878 File Offset: 0x000A7A78
	public void UpdateFriendshipBracelet()
	{
		bool flag = false;
		if (this.isOfflineVRRig)
		{
			bool flag2 = false;
			VRRig.PartyMemberStatus partyMemberStatus = this.GetPartyMemberStatus();
			if (partyMemberStatus != VRRig.PartyMemberStatus.InLocalParty)
			{
				if (partyMemberStatus == VRRig.PartyMemberStatus.NotInLocalParty)
				{
					flag2 = false;
					this.reliableState.isBraceletLeftHanded = false;
				}
			}
			else
			{
				flag2 = true;
				this.reliableState.isBraceletLeftHanded = (FriendshipGroupDetection.Instance.DidJoinLeftHanded && !this.huntComputer.activeSelf);
			}
			if (this.reliableState.HasBracelet != flag2 || this.reliableState.braceletBeadColors.Count != FriendshipGroupDetection.Instance.myBeadColors.Count)
			{
				this.reliableState.SetIsDirty();
				flag = (this.reliableState.HasBracelet == flag2);
			}
			this.reliableState.braceletBeadColors.Clear();
			if (flag2)
			{
				this.reliableState.braceletBeadColors.AddRange(FriendshipGroupDetection.Instance.myBeadColors);
			}
			this.reliableState.braceletSelfIndex = FriendshipGroupDetection.Instance.MyBraceletSelfIndex;
		}
		if (this.nonCosmeticLeftHandItem != null)
		{
			bool flag3 = this.reliableState.HasBracelet && this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticLeftHandItem.EnableItem(flag3);
			if (flag3)
			{
				this.friendshipBraceletLeftHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletLeftHand.PlayAppearEffects();
				}
			}
		}
		if (this.nonCosmeticRightHandItem != null)
		{
			bool flag4 = this.reliableState.HasBracelet && !this.reliableState.isBraceletLeftHanded && !this.IsInvisibleToLocalPlayer;
			this.nonCosmeticRightHandItem.EnableItem(flag4);
			if (flag4)
			{
				this.friendshipBraceletRightHand.UpdateBeads(this.reliableState.braceletBeadColors, this.reliableState.braceletSelfIndex);
				if (flag)
				{
					this.friendshipBraceletRightHand.PlayAppearEffects();
				}
			}
		}
	}

	// Token: 0x06001F71 RID: 8049 RVA: 0x000A9A48 File Offset: 0x000A7C48
	public void EnableHuntWatch(bool on)
	{
		this.huntComputer.SetActive(on);
		if (this.builderResizeWatch != null)
		{
			MeshRenderer component = this.builderResizeWatch.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.enabled = !on;
			}
		}
	}

	// Token: 0x06001F72 RID: 8050 RVA: 0x000A9A8E File Offset: 0x000A7C8E
	public void EnablePaintbrawlCosmetics(bool on)
	{
		this.paintbrawlBalloons.gameObject.SetActive(on);
	}

	// Token: 0x06001F73 RID: 8051 RVA: 0x000A9AA4 File Offset: 0x000A7CA4
	public void EnableBuilderResizeWatch(bool on)
	{
		if (this.builderResizeWatch != null && this.builderResizeWatch.activeSelf != on)
		{
			this.builderResizeWatch.SetActive(on);
			if (this.builderArmShelfLeft != null)
			{
				this.builderArmShelfLeft.gameObject.SetActive(on);
			}
			if (this.builderArmShelfRight != null)
			{
				this.builderArmShelfRight.gameObject.SetActive(on);
			}
		}
		if (this.isOfflineVRRig)
		{
			bool flag = this.reliableState.isBuilderWatchEnabled != on;
			this.reliableState.isBuilderWatchEnabled = on;
			if (flag)
			{
				this.reliableState.SetIsDirty();
			}
		}
	}

	// Token: 0x06001F74 RID: 8052 RVA: 0x000A9B49 File Offset: 0x000A7D49
	public void EnableGuardianEjectWatch(bool on)
	{
		if (this.guardianEjectWatch != null && this.guardianEjectWatch.activeSelf != on)
		{
			this.guardianEjectWatch.SetActive(on);
		}
	}

	// Token: 0x06001F75 RID: 8053 RVA: 0x000A9B73 File Offset: 0x000A7D73
	public void EnableVStumpReturnWatch(bool on)
	{
		if (this.vStumpReturnWatch != null && this.vStumpReturnWatch.activeSelf != on)
		{
			this.vStumpReturnWatch.SetActive(on);
		}
	}

	// Token: 0x06001F76 RID: 8054 RVA: 0x000A9B9D File Offset: 0x000A7D9D
	public void EnableRankedTimerWatch(bool on)
	{
		if (this.rankedTimerWatch != null && this.rankedTimerWatch.activeSelf != on)
		{
			this.rankedTimerWatch.SetActive(on);
		}
	}

	// Token: 0x06001F77 RID: 8055 RVA: 0x000A9BC7 File Offset: 0x000A7DC7
	public void EnableSuperInfectionHands(bool on)
	{
		if (this.superInfectionHand != null)
		{
			this.superInfectionHand.EnableHands(on);
		}
	}

	// Token: 0x06001F78 RID: 8056 RVA: 0x000A9BE4 File Offset: 0x000A7DE4
	private void UpdateReplacementVoice()
	{
		if (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn != "TRUE")
		{
			this.voiceAudio.mute = true;
			return;
		}
		this.voiceAudio.mute = false;
	}

	// Token: 0x06001F79 RID: 8057 RVA: 0x000A9C34 File Offset: 0x000A7E34
	public bool ShouldPlayReplacementVoice()
	{
		return this.netView && !this.netView.IsMine && !(GorillaComputer.instance.voiceChatOn == "OFF") && (this.remoteUseReplacementVoice || this.localUseReplacementVoice || GorillaComputer.instance.voiceChatOn == "FALSE") && this.SpeakingLoudness > this.replacementVoiceLoudnessThreshold;
	}

	// Token: 0x06001F7A RID: 8058 RVA: 0x000A9CAF File Offset: 0x000A7EAF
	public void SetDuplicationZone(RigDuplicationZone duplicationZone)
	{
		this.duplicationZone = duplicationZone;
		this.inDuplicationZone = (duplicationZone != null);
	}

	// Token: 0x06001F7B RID: 8059 RVA: 0x000A9CC5 File Offset: 0x000A7EC5
	public void ClearDuplicationZone(RigDuplicationZone duplicationZone)
	{
		if (this.duplicationZone == duplicationZone)
		{
			this.SetDuplicationZone(null);
			this.renderTransform.localPosition = this.cachedRenderTransformPos;
		}
	}

	// Token: 0x06001F7C RID: 8060 RVA: 0x000A9CED File Offset: 0x000A7EED
	public void ResetTimeSpawned()
	{
		this.timeSpawned = Time.time;
	}

	// Token: 0x06001F7D RID: 8061 RVA: 0x000A9CFC File Offset: 0x000A7EFC
	public void SetGooParticleSystemStatus(bool isLeftHand, bool isEnabled)
	{
		if (isLeftHand)
		{
			if (this.leftHandGooParticleSystem.gameObject.activeSelf != isEnabled)
			{
				this.leftHandGooParticleSystem.gameObject.SetActive(isEnabled);
				return;
			}
		}
		else if (this.rightHandGooParticleSystem.gameObject.activeSelf != isEnabled)
		{
			this.rightHandGooParticleSystem.gameObject.SetActive(isEnabled);
		}
	}

	// Token: 0x17000356 RID: 854
	// (get) Token: 0x06001F7E RID: 8062 RVA: 0x000A9D55 File Offset: 0x000A7F55
	// (set) Token: 0x06001F7F RID: 8063 RVA: 0x000A9D5D File Offset: 0x000A7F5D
	bool IUserCosmeticsCallback.PendingUpdate
	{
		get
		{
			return this.pendingCosmeticUpdate;
		}
		set
		{
			this.pendingCosmeticUpdate = value;
		}
	}

	// Token: 0x17000357 RID: 855
	// (get) Token: 0x06001F80 RID: 8064 RVA: 0x000A9D66 File Offset: 0x000A7F66
	// (set) Token: 0x06001F81 RID: 8065 RVA: 0x000A9D6E File Offset: 0x000A7F6E
	public bool IsFrozen { get; set; }

	// Token: 0x17000358 RID: 856
	// (get) Token: 0x06001F82 RID: 8066 RVA: 0x000A9D77 File Offset: 0x000A7F77
	// (set) Token: 0x06001F83 RID: 8067 RVA: 0x000A9D7F File Offset: 0x000A7F7F
	public bool ShowGoldNameTag
	{
		get
		{
			return this.showGoldNameTag;
		}
		private set
		{
			this.showGoldNameTag = value;
		}
	}

	// Token: 0x06001F84 RID: 8068 RVA: 0x000A9D88 File Offset: 0x000A7F88
	bool IUserCosmeticsCallback.OnGetUserCosmetics(string cosmeticsString)
	{
		if (cosmeticsString == "BANNED")
		{
			this._playerOwnedCosmetics.Clear();
			this._playerOwnedCosmeticsAge.Clear();
			return true;
		}
		Dictionary<string, ItemInstance> dictionary;
		try
		{
			dictionary = JsonConvert.DeserializeObject<Dictionary<string, ItemInstance>>(cosmeticsString);
		}
		catch (Exception ex)
		{
			string str = "Failed to deserialize cosmetics for ";
			NetPlayer netPlayer = this.creator;
			Debug.LogError(str + ((netPlayer != null) ? netPlayer.NickName : null) + ": " + ex.Message);
			dictionary = null;
		}
		if (this.currentCosmeticTries < this.cosmeticRetries && (dictionary == null || this._playerOwnedCosmetics.SetEquals(dictionary.Keys)))
		{
			this.currentCosmeticTries++;
			return false;
		}
		if (dictionary == null)
		{
			dictionary = new Dictionary<string, ItemInstance>();
		}
		this.currentCosmeticTries = 0;
		this.SaveOwnedCosmetics(dictionary);
		this.InitializedCosmetics = true;
		this.SetCosmeticsActive(false);
		this.myBodyDockPositions.RefreshTransferrableItems();
		NetworkView networkView = this.netView;
		if (networkView != null)
		{
			networkView.SendRPC("RPC_RequestCosmetics", this.creator, Array.Empty<object>());
		}
		return true;
	}

	// Token: 0x06001F85 RID: 8069 RVA: 0x000A9E8C File Offset: 0x000A808C
	private void SaveOwnedCosmetics(Dictionary<string, ItemInstance> cosmetics)
	{
		if (cosmetics.Count == 0)
		{
			return;
		}
		this._playerOwnedCosmetics.Clear();
		this._playerOwnedCosmeticsAge.Clear();
		foreach (KeyValuePair<string, ItemInstance> keyValuePair in cosmetics)
		{
			string text;
			ItemInstance itemInstance;
			keyValuePair.Deconstruct(out text, out itemInstance);
			string text2 = text;
			ItemInstance itemInstance2 = itemInstance;
			this._playerOwnedCosmetics.Add(text2);
			if (itemInstance2 != null && itemInstance2.PurchaseDate != null)
			{
				this._playerOwnedCosmeticsAge[text2] = (int)(DateTime.UtcNow - itemInstance2.PurchaseDate.Value).TotalDays;
			}
		}
		this.CheckForEarlyAccess();
	}

	// Token: 0x06001F86 RID: 8070 RVA: 0x000A9F54 File Offset: 0x000A8154
	internal void AddCosmetic(string cosmeticId)
	{
		this._playerOwnedCosmetics.Add(cosmeticId);
	}

	// Token: 0x06001F87 RID: 8071 RVA: 0x000A9F63 File Offset: 0x000A8163
	internal bool HasCosmetic(string cosmeticId)
	{
		return this._playerOwnedCosmetics.Contains(cosmeticId);
	}

	// Token: 0x06001F88 RID: 8072 RVA: 0x000A9F74 File Offset: 0x000A8174
	private short PackCompetitiveData()
	{
		if (!this.turningCompInitialized)
		{
			this.GorillaSnapTurningComp = GorillaTagger.Instance.GetComponent<GorillaSnapTurn>();
			this.turningCompInitialized = true;
		}
		this.fps = Mathf.Min(Mathf.RoundToInt(1f / Time.smoothDeltaTime), 255);
		int num = 0;
		if (this.GorillaSnapTurningComp != null)
		{
			this.turnFactor = this.GorillaSnapTurningComp.turnFactor;
			this.turnType = this.GorillaSnapTurningComp.turnType;
			string a = this.turnType;
			if (!(a == "SNAP"))
			{
				if (a == "SMOOTH")
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
			num *= 10;
			num += this.turnFactor;
		}
		return (short)(this.fps + (num << 8));
	}

	// Token: 0x06001F89 RID: 8073 RVA: 0x000AA034 File Offset: 0x000A8234
	private void UnpackCompetitiveData(short packed)
	{
		int num = 255;
		this.fps = ((int)packed & num);
		int num2 = 31;
		int num3 = packed >> 8 & num2;
		this.turnFactor = num3 % 10;
		int num4 = num3 / 10;
		if (num4 == 1)
		{
			this.turnType = "SNAP";
			return;
		}
		if (num4 != 2)
		{
			this.turnType = "NONE";
			return;
		}
		this.turnType = "SMOOTH";
	}

	// Token: 0x06001F8A RID: 8074 RVA: 0x000AA098 File Offset: 0x000A8298
	private void OnKIDSessionUpdated(bool showCustomNames, Permission.ManagedByEnum managedBy)
	{
		bool flag = (showCustomNames || managedBy == Permission.ManagedByEnum.PLAYER) && managedBy != Permission.ManagedByEnum.PROHIBITED;
		GorillaComputer.instance.SetComputerSettingsBySafety(!flag, new GorillaComputer.ComputerState[]
		{
			GorillaComputer.ComputerState.Name
		}, false);
		bool flag2 = PlayerPrefs.GetInt("nameTagsOn", -1) > 0;
		switch (managedBy)
		{
		case Permission.ManagedByEnum.PLAYER:
			flag = GorillaComputer.instance.NametagsEnabled;
			break;
		case Permission.ManagedByEnum.GUARDIAN:
			flag = (showCustomNames && flag2);
			break;
		case Permission.ManagedByEnum.PROHIBITED:
			flag = false;
			break;
		}
		this.UpdateName(flag);
		Debug.Log("[KID] On Session Update - Custom Names Permission changed - Has enabled customNames? [" + flag.ToString() + "]");
	}

	// Token: 0x17000359 RID: 857
	// (get) Token: 0x06001F8B RID: 8075 RVA: 0x000AA134 File Offset: 0x000A8334
	public static VRRig LocalRig
	{
		get
		{
			return VRRig.gLocalRig;
		}
	}

	// Token: 0x1700035A RID: 858
	// (get) Token: 0x06001F8C RID: 8076 RVA: 0x000AA13B File Offset: 0x000A833B
	public bool isLocal
	{
		get
		{
			return VRRig.gLocalRig == this;
		}
	}

	// Token: 0x1700035B RID: 859
	// (get) Token: 0x06001F8D RID: 8077 RVA: 0x00010CFB File Offset: 0x0000EEFB
	int IEyeScannable.scannableId
	{
		get
		{
			return base.gameObject.GetInstanceID();
		}
	}

	// Token: 0x1700035C RID: 860
	// (get) Token: 0x06001F8E RID: 8078 RVA: 0x000AA148 File Offset: 0x000A8348
	Vector3 IEyeScannable.Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x1700035D RID: 861
	// (get) Token: 0x06001F8F RID: 8079 RVA: 0x000AA158 File Offset: 0x000A8358
	Bounds IEyeScannable.Bounds
	{
		get
		{
			return default(Bounds);
		}
	}

	// Token: 0x1700035E RID: 862
	// (get) Token: 0x06001F90 RID: 8080 RVA: 0x000AA16E File Offset: 0x000A836E
	IList<KeyValueStringPair> IEyeScannable.Entries
	{
		get
		{
			return this.buildEntries();
		}
	}

	// Token: 0x06001F91 RID: 8081 RVA: 0x000AA178 File Offset: 0x000A8378
	private IList<KeyValueStringPair> buildEntries()
	{
		return new KeyValueStringPair[]
		{
			new KeyValueStringPair("Name", this.playerNameVisible),
			new KeyValueStringPair("Color", string.Format("{0}, {1}, {2}", Mathf.RoundToInt(this.playerColor.r * 9f), Mathf.RoundToInt(this.playerColor.g * 9f), Mathf.RoundToInt(this.playerColor.b * 9f)))
		};
	}

	// Token: 0x14000048 RID: 72
	// (add) Token: 0x06001F92 RID: 8082 RVA: 0x000AA210 File Offset: 0x000A8410
	// (remove) Token: 0x06001F93 RID: 8083 RVA: 0x000AA248 File Offset: 0x000A8448
	public event Action OnDataChange;

	// Token: 0x040028CA RID: 10442
	private bool _isListeningFor_OnPostInstantiateAllPrefabs;

	// Token: 0x040028CB RID: 10443
	[OnEnterPlay_SetNull]
	public static Action newPlayerJoined;

	// Token: 0x040028CC RID: 10444
	public VRMap head;

	// Token: 0x040028CD RID: 10445
	public VRMap rightHand;

	// Token: 0x040028CE RID: 10446
	public VRMap leftHand;

	// Token: 0x040028CF RID: 10447
	public VRMapThumb leftThumb;

	// Token: 0x040028D0 RID: 10448
	public VRMapIndex leftIndex;

	// Token: 0x040028D1 RID: 10449
	public VRMapMiddle leftMiddle;

	// Token: 0x040028D2 RID: 10450
	public VRMapThumb rightThumb;

	// Token: 0x040028D3 RID: 10451
	public VRMapIndex rightIndex;

	// Token: 0x040028D4 RID: 10452
	public VRMapMiddle rightMiddle;

	// Token: 0x040028D5 RID: 10453
	public CrittersLoudNoise leftHandNoise;

	// Token: 0x040028D6 RID: 10454
	public CrittersLoudNoise rightHandNoise;

	// Token: 0x040028D7 RID: 10455
	public CrittersLoudNoise speakingNoise;

	// Token: 0x040028D8 RID: 10456
	private int previousGrabbedRope = -1;

	// Token: 0x040028D9 RID: 10457
	private int previousGrabbedRopeBoneIndex;

	// Token: 0x040028DA RID: 10458
	private bool previousGrabbedRopeWasLeft;

	// Token: 0x040028DB RID: 10459
	private bool previousGrabbedRopeWasBody;

	// Token: 0x040028DC RID: 10460
	private GorillaRopeSwing currentRopeSwing;

	// Token: 0x040028DD RID: 10461
	private Transform currentHoldParent;

	// Token: 0x040028DE RID: 10462
	private Transform currentRopeSwingTarget;

	// Token: 0x040028DF RID: 10463
	private float lastRopeGrabTimer;

	// Token: 0x040028E0 RID: 10464
	private bool shouldLerpToRope;

	// Token: 0x040028E1 RID: 10465
	[NonSerialized]
	public int grabbedRopeIndex = -1;

	// Token: 0x040028E2 RID: 10466
	[NonSerialized]
	public int grabbedRopeBoneIndex;

	// Token: 0x040028E3 RID: 10467
	[NonSerialized]
	public bool grabbedRopeIsLeft;

	// Token: 0x040028E4 RID: 10468
	[NonSerialized]
	public bool grabbedRopeIsBody;

	// Token: 0x040028E5 RID: 10469
	[NonSerialized]
	public bool grabbedRopeIsPhotonView;

	// Token: 0x040028E6 RID: 10470
	[NonSerialized]
	public Vector3 grabbedRopeOffset = Vector3.zero;

	// Token: 0x040028E7 RID: 10471
	private int prevMovingSurfaceID = -1;

	// Token: 0x040028E8 RID: 10472
	private bool movingSurfaceWasLeft;

	// Token: 0x040028E9 RID: 10473
	private bool movingSurfaceWasBody;

	// Token: 0x040028EA RID: 10474
	private bool movingSurfaceWasMonkeBlock;

	// Token: 0x040028EB RID: 10475
	[NonSerialized]
	public int mountedMovingSurfaceId = -1;

	// Token: 0x040028EC RID: 10476
	[NonSerialized]
	private BuilderPiece mountedMonkeBlock;

	// Token: 0x040028ED RID: 10477
	[NonSerialized]
	private MovingSurface mountedMovingSurface;

	// Token: 0x040028EE RID: 10478
	[NonSerialized]
	public bool mountedMovingSurfaceIsLeft;

	// Token: 0x040028EF RID: 10479
	[NonSerialized]
	public bool mountedMovingSurfaceIsBody;

	// Token: 0x040028F0 RID: 10480
	[NonSerialized]
	public bool movingSurfaceIsMonkeBlock;

	// Token: 0x040028F1 RID: 10481
	[NonSerialized]
	public Vector3 mountedMonkeBlockOffset = Vector3.zero;

	// Token: 0x040028F2 RID: 10482
	[NonSerialized]
	public bool InOverrideSubscriptionZone;

	// Token: 0x040028F3 RID: 10483
	[NonSerialized]
	public Vector3 OverrideSubscriptionZoneLocation = Vector3.zero;

	// Token: 0x040028F4 RID: 10484
	private float lastMountedSurfaceTimer;

	// Token: 0x040028F5 RID: 10485
	private bool shouldLerpToMovingSurface;

	// Token: 0x040028F6 RID: 10486
	[Tooltip("- False in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool isOfflineVRRig;

	// Token: 0x040028F7 RID: 10487
	public GameObject mainCamera;

	// Token: 0x040028F8 RID: 10488
	public Transform playerOffsetTransform;

	// Token: 0x040028F9 RID: 10489
	public int SDKIndex;

	// Token: 0x040028FA RID: 10490
	public bool isMyPlayer;

	// Token: 0x040028FB RID: 10491
	public AudioSource leftHandPlayer;

	// Token: 0x040028FC RID: 10492
	public AudioSource rightHandPlayer;

	// Token: 0x040028FD RID: 10493
	public AudioSource tagSound;

	// Token: 0x040028FE RID: 10494
	[SerializeField]
	private float ratio;

	// Token: 0x040028FF RID: 10495
	public Transform headConstraint;

	// Token: 0x04002900 RID: 10496
	public Vector3 headBodyOffset = Vector3.zero;

	// Token: 0x04002901 RID: 10497
	public GameObject headMesh;

	// Token: 0x04002902 RID: 10498
	private NetworkVector3 netSyncPos = new NetworkVector3();

	// Token: 0x04002903 RID: 10499
	public Vector3 jobPos;

	// Token: 0x04002904 RID: 10500
	public Quaternion syncRotation;

	// Token: 0x04002905 RID: 10501
	public Quaternion jobRotation;

	// Token: 0x04002906 RID: 10502
	public AudioClip[] clipToPlay;

	// Token: 0x04002907 RID: 10503
	public AudioClip[] handTapSound;

	// Token: 0x04002908 RID: 10504
	public int setMatIndex;

	// Token: 0x04002909 RID: 10505
	public float lerpValueFingers;

	// Token: 0x0400290A RID: 10506
	public float lerpValueBody;

	// Token: 0x0400290B RID: 10507
	public GameObject backpack;

	// Token: 0x0400290C RID: 10508
	public Transform leftHandTransform;

	// Token: 0x0400290D RID: 10509
	public Transform rightHandTransform;

	// Token: 0x0400290E RID: 10510
	public Transform bodyTransform;

	// Token: 0x0400290F RID: 10511
	public SkinnedMeshRenderer mainSkin;

	// Token: 0x04002910 RID: 10512
	public GorillaSkin defaultSkin;

	// Token: 0x04002911 RID: 10513
	public MeshRenderer faceSkin;

	// Token: 0x04002912 RID: 10514
	public XRaySkeleton skeleton;

	// Token: 0x04002913 RID: 10515
	public GorillaBodyRenderer bodyRenderer;

	// Token: 0x04002914 RID: 10516
	public ZoneEntityBSP zoneEntity;

	// Token: 0x04002915 RID: 10517
	public Material scoreboardMaterial;

	// Token: 0x04002916 RID: 10518
	public GameObject spectatorSkin;

	// Token: 0x04002917 RID: 10519
	public int handSync;

	// Token: 0x04002918 RID: 10520
	public Material[] materialsToChangeTo;

	// Token: 0x04002919 RID: 10521
	public float red;

	// Token: 0x0400291A RID: 10522
	public float green;

	// Token: 0x0400291B RID: 10523
	public float blue;

	// Token: 0x0400291C RID: 10524
	public TextMeshPro playerText1;

	// Token: 0x0400291D RID: 10525
	public string playerNameVisible;

	// Token: 0x0400291E RID: 10526
	[Tooltip("- True in 'Gorilla Player Networked.prefab'.\n- True in 'Local VRRig.prefab/Local Gorilla Player'.\n- False in 'Local VRRig.prefab/Actual Gorilla'")]
	public bool showName;

	// Token: 0x0400291F RID: 10527
	public CosmeticItemRegistry cosmeticsObjectRegistry;

	// Token: 0x04002920 RID: 10528
	[NonSerialized]
	public PropHuntHandFollower propHuntHandFollower;

	// Token: 0x04002921 RID: 10529
	private int taggedById;

	// Token: 0x04002922 RID: 10530
	private readonly HashSet<string> _playerOwnedCosmetics = new HashSet<string>(50);

	// Token: 0x04002923 RID: 10531
	private readonly Dictionary<string, int> _playerOwnedCosmeticsAge = new Dictionary<string, int>(50);

	// Token: 0x04002924 RID: 10532
	private bool initializedCosmetics;

	// Token: 0x04002925 RID: 10533
	private readonly HashSet<string> _temporaryCosmetics = new HashSet<string>();

	// Token: 0x04002926 RID: 10534
	public CosmeticsController.CosmeticSet cosmeticSet;

	// Token: 0x04002927 RID: 10535
	public CosmeticsController.CosmeticSet tryOnSet;

	// Token: 0x04002928 RID: 10536
	public CosmeticsController.CosmeticSet mergedSet;

	// Token: 0x04002929 RID: 10537
	public CosmeticsController.CosmeticSet prevSet;

	// Token: 0x0400292A RID: 10538
	[NonSerialized]
	public readonly List<GameObject> activeCosmetics = new List<GameObject>(16);

	// Token: 0x0400292B RID: 10539
	private int cosmeticRetries = 2;

	// Token: 0x0400292C RID: 10540
	private int currentCosmeticTries;

	// Token: 0x0400292E RID: 10542
	public SizeManager sizeManager;

	// Token: 0x0400292F RID: 10543
	public float pitchScale = 0.3f;

	// Token: 0x04002930 RID: 10544
	public float pitchOffset = 1f;

	// Token: 0x04002931 RID: 10545
	[NonSerialized]
	public bool IsHaunted;

	// Token: 0x04002932 RID: 10546
	public float HauntedVoicePitch = 0.5f;

	// Token: 0x04002933 RID: 10547
	public float HauntedHearingVolume = 0.15f;

	// Token: 0x04002934 RID: 10548
	[NonSerialized]
	public bool UsingHauntedRing;

	// Token: 0x04002935 RID: 10549
	[NonSerialized]
	public float HauntedRingVoicePitch;

	// Token: 0x04002936 RID: 10550
	private float cosmeticPitchShift;

	// Token: 0x04002937 RID: 10551
	private float cosmeticVolumeShift;

	// Token: 0x04002938 RID: 10552
	private bool cosmeticPitchActive;

	// Token: 0x04002939 RID: 10553
	private bool cosmeticVolumeActive;

	// Token: 0x0400293A RID: 10554
	private bool anyShiftedVoiceCosmetic;

	// Token: 0x0400293B RID: 10555
	private bool voiceShiftCosmeticsDirty;

	// Token: 0x0400293C RID: 10556
	[NonSerialized]
	public List<VoiceShiftCosmetic> VoiceShiftCosmetics = new List<VoiceShiftCosmetic>();

	// Token: 0x0400293D RID: 10557
	public FriendshipBracelet friendshipBraceletLeftHand;

	// Token: 0x0400293E RID: 10558
	public NonCosmeticHandItem nonCosmeticLeftHandItem;

	// Token: 0x0400293F RID: 10559
	public FriendshipBracelet friendshipBraceletRightHand;

	// Token: 0x04002940 RID: 10560
	public NonCosmeticHandItem nonCosmeticRightHandItem;

	// Token: 0x04002941 RID: 10561
	public HoverboardVisual hoverboardVisual;

	// Token: 0x04002942 RID: 10562
	private int hoverboardEnabledCount;

	// Token: 0x04002943 RID: 10563
	public HoldableHand bodyHolds;

	// Token: 0x04002944 RID: 10564
	public HoldableHand leftHolds;

	// Token: 0x04002945 RID: 10565
	public HoldableHand rightHolds;

	// Token: 0x04002946 RID: 10566
	public GorillaClimbable leftHandHoldsPlayer;

	// Token: 0x04002947 RID: 10567
	public GorillaClimbable rightHandHoldsPlayer;

	// Token: 0x04002948 RID: 10568
	public TakeMyHand_HandLink leftHandLink;

	// Token: 0x04002949 RID: 10569
	public TakeMyHand_HandLink rightHandLink;

	// Token: 0x0400294C RID: 10572
	public GameObject nameTagAnchor;

	// Token: 0x0400294D RID: 10573
	public GameObject frozenEffect;

	// Token: 0x0400294E RID: 10574
	public GameObject iceCubeLeft;

	// Token: 0x0400294F RID: 10575
	public GameObject iceCubeRight;

	// Token: 0x04002950 RID: 10576
	public float frozenEffectMaxY;

	// Token: 0x04002951 RID: 10577
	public float frozenEffectMaxHorizontalScale = 0.8f;

	// Token: 0x04002952 RID: 10578
	public GameObject FPVEffectsParent;

	// Token: 0x04002953 RID: 10579
	public Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect> TemporaryCosmeticEffects = new Dictionary<CosmeticEffectsOnPlayers.EFFECTTYPE, CosmeticEffectsOnPlayers.CosmeticEffect>();

	// Token: 0x04002954 RID: 10580
	private float _nextUpdateTime = -1f;

	// Token: 0x04002955 RID: 10581
	public VRRigReliableState reliableState;

	// Token: 0x04002956 RID: 10582
	[SerializeField]
	private Transform MouthPosition;

	// Token: 0x0400295A RID: 10586
	internal RigContainer rigContainer;

	// Token: 0x0400295B RID: 10587
	public Action<RigContainer> OnNameChanged;

	// Token: 0x0400295C RID: 10588
	private Vector3 remoteVelocity;

	// Token: 0x0400295D RID: 10589
	private double remoteLatestTimestamp;

	// Token: 0x0400295E RID: 10590
	private Vector3 remoteCorrectionNeeded;

	// Token: 0x0400295F RID: 10591
	private const float REMOTE_CORRECTION_RATE = 5f;

	// Token: 0x04002960 RID: 10592
	private const bool USE_NEW_NETCODE = false;

	// Token: 0x04002961 RID: 10593
	private float stealthTimer;

	// Token: 0x04002962 RID: 10594
	private GorillaAmbushManager stealthManager;

	// Token: 0x04002963 RID: 10595
	private LayerChanger layerChanger;

	// Token: 0x04002964 RID: 10596
	private float frozenEffectMinY;

	// Token: 0x04002965 RID: 10597
	private float frozenEffectMinHorizontalScale;

	// Token: 0x04002966 RID: 10598
	private float frozenTimeElapsed;

	// Token: 0x04002967 RID: 10599
	public TagEffectPack CosmeticEffectPack;

	// Token: 0x04002968 RID: 10600
	private GorillaSnapTurn GorillaSnapTurningComp;

	// Token: 0x04002969 RID: 10601
	private bool turningCompInitialized;

	// Token: 0x0400296A RID: 10602
	private string turnType = "NONE";

	// Token: 0x0400296B RID: 10603
	private int turnFactor;

	// Token: 0x0400296C RID: 10604
	private int fps;

	// Token: 0x0400296E RID: 10606
	private VRRig.PartyMemberStatus partyMemberStatus;

	// Token: 0x0400296F RID: 10607
	public static readonly GTBitOps.BitWriteInfo[] WearablePackedStatesBitWriteInfos = new GTBitOps.BitWriteInfo[]
	{
		new GTBitOps.BitWriteInfo(0, 1),
		new GTBitOps.BitWriteInfo(1, 2),
		new GTBitOps.BitWriteInfo(3, 2),
		new GTBitOps.BitWriteInfo(5, 2),
		new GTBitOps.BitWriteInfo(7, 2),
		new GTBitOps.BitWriteInfo(9, 2),
		new GTBitOps.BitWriteInfo(11, 1),
		new GTBitOps.BitWriteInfo(12, 1),
		new GTBitOps.BitWriteInfo(13, 1)
	};

	// Token: 0x04002970 RID: 10608
	public bool inTryOnRoom;

	// Token: 0x04002971 RID: 10609
	public bool inTempCosmSpace;

	// Token: 0x04002972 RID: 10610
	[NonSerialized]
	public CosmeticsController.CosmeticItem[] remoteCollectables = Array.Empty<CosmeticsController.CosmeticItem>();

	// Token: 0x04002973 RID: 10611
	[NonSerialized]
	public Dictionary<string, int> remoteCycleStates = new Dictionary<string, int>();

	// Token: 0x04002974 RID: 10612
	private readonly List<CosmeticCollectionDisplay> scratchDisplayList = new List<CosmeticCollectionDisplay>();

	// Token: 0x04002975 RID: 10613
	private int[] cycleStatesArray = Array.Empty<int>();

	// Token: 0x04002976 RID: 10614
	public bool muted;

	// Token: 0x04002977 RID: 10615
	private float lastScaleFactor = 1f;

	// Token: 0x04002978 RID: 10616
	private float scaleMultiplier = 1f;

	// Token: 0x04002979 RID: 10617
	private float nativeScale = 1f;

	// Token: 0x0400297A RID: 10618
	private float timeSpawned;

	// Token: 0x0400297B RID: 10619
	public float doNotLerpConstant = 1f;

	// Token: 0x0400297C RID: 10620
	public string tempString;

	// Token: 0x0400297D RID: 10621
	internal NetPlayer creator;

	// Token: 0x0400297E RID: 10622
	private float[] speedArray;

	// Token: 0x0400297F RID: 10623
	private double handLerpValues;

	// Token: 0x04002980 RID: 10624
	private bool initialized;

	// Token: 0x04002981 RID: 10625
	[FormerlySerializedAs("battleBalloons")]
	public PaintbrawlBalloons paintbrawlBalloons;

	// Token: 0x04002982 RID: 10626
	private int tempInt;

	// Token: 0x04002983 RID: 10627
	public BodyDockPositions myBodyDockPositions;

	// Token: 0x04002984 RID: 10628
	public ParticleSystem lavaParticleSystem;

	// Token: 0x04002985 RID: 10629
	public ParticleSystem rockParticleSystem;

	// Token: 0x04002986 RID: 10630
	public ParticleSystem iceParticleSystem;

	// Token: 0x04002987 RID: 10631
	public ParticleSystem snowFlakeParticleSystem;

	// Token: 0x04002988 RID: 10632
	public ParticleSystem leftHandGooParticleSystem;

	// Token: 0x04002989 RID: 10633
	public ParticleSystem rightHandGooParticleSystem;

	// Token: 0x0400298A RID: 10634
	public string tempItemName;

	// Token: 0x0400298B RID: 10635
	public CosmeticsController.CosmeticItem tempItem;

	// Token: 0x0400298C RID: 10636
	public string tempItemId;

	// Token: 0x0400298D RID: 10637
	public int tempItemCost;

	// Token: 0x0400298E RID: 10638
	public int leftHandHoldableStatus;

	// Token: 0x0400298F RID: 10639
	public int rightHandHoldableStatus;

	// Token: 0x04002990 RID: 10640
	[Tooltip("This has to match the drumsAS array in DrumsItem.cs.")]
	[SerializeReference]
	public AudioSource[] musicDrums;

	// Token: 0x04002991 RID: 10641
	private List<TransferrableObject> instrumentSelfOnly = new List<TransferrableObject>();

	// Token: 0x04002992 RID: 10642
	public AudioSource geodeCrackingSound;

	// Token: 0x04002993 RID: 10643
	public float bonkTime;

	// Token: 0x04002994 RID: 10644
	public float bonkCooldown = 2f;

	// Token: 0x04002995 RID: 10645
	private VRRig tempVRRig;

	// Token: 0x04002996 RID: 10646
	public GameObject huntComputer;

	// Token: 0x04002997 RID: 10647
	public GameObject builderResizeWatch;

	// Token: 0x04002998 RID: 10648
	public BuilderArmShelf builderArmShelfLeft;

	// Token: 0x04002999 RID: 10649
	public BuilderArmShelf builderArmShelfRight;

	// Token: 0x0400299A RID: 10650
	public GameObject guardianEjectWatch;

	// Token: 0x0400299B RID: 10651
	public GameObject vStumpReturnWatch;

	// Token: 0x0400299C RID: 10652
	public GameObject rankedTimerWatch;

	// Token: 0x0400299D RID: 10653
	public SuperInfectionHandDisplay superInfectionHand;

	// Token: 0x0400299E RID: 10654
	public ProjectileWeapon projectileWeapon;

	// Token: 0x0400299F RID: 10655
	private PhotonVoiceView myPhotonVoiceView;

	// Token: 0x040029A0 RID: 10656
	private VRRig senderRig;

	// Token: 0x040029A1 RID: 10657
	private bool isInitialized;

	// Token: 0x040029A2 RID: 10658
	private CircularBuffer<VRRig.VelocityTime> velocityHistoryList = new CircularBuffer<VRRig.VelocityTime>(200);

	// Token: 0x040029A3 RID: 10659
	public int velocityHistoryMaxLength = 200;

	// Token: 0x040029A4 RID: 10660
	private Vector3 lastPosition;

	// Token: 0x040029A5 RID: 10661
	public const int splashLimitCount = 4;

	// Token: 0x040029A6 RID: 10662
	public const float splashLimitCooldown = 0.5f;

	// Token: 0x040029A7 RID: 10663
	private float[] splashEffectTimes = new float[4];

	// Token: 0x040029A8 RID: 10664
	internal AudioSource voiceAudio;

	// Token: 0x040029A9 RID: 10665
	public bool remoteUseReplacementVoice;

	// Token: 0x040029AA RID: 10666
	public bool localUseReplacementVoice;

	// Token: 0x040029AB RID: 10667
	private MicWrapper currentMicWrapper;

	// Token: 0x040029AC RID: 10668
	private IAudioDesc audioDesc;

	// Token: 0x040029AD RID: 10669
	private float speakingLoudness;

	// Token: 0x040029AE RID: 10670
	public bool shouldSendSpeakingLoudness = true;

	// Token: 0x040029AF RID: 10671
	public float replacementVoiceLoudnessThreshold = 0.05f;

	// Token: 0x040029B0 RID: 10672
	public int replacementVoiceDetectionDelay = 128;

	// Token: 0x040029B1 RID: 10673
	private GorillaMouthFlap myMouthFlap;

	// Token: 0x040029B2 RID: 10674
	private GorillaSpeakerLoudness mySpeakerLoudness;

	// Token: 0x040029B3 RID: 10675
	public ReplacementVoice myReplacementVoice;

	// Token: 0x040029B4 RID: 10676
	private GorillaEyeExpressions myEyeExpressions;

	// Token: 0x040029B5 RID: 10677
	[SerializeField]
	internal NetworkView netView;

	// Token: 0x040029B6 RID: 10678
	[SerializeField]
	internal VRRigSerializer rigSerializer;

	// Token: 0x040029B7 RID: 10679
	[Obsolete("Deprecated, this is unreliable, use Creator", false)]
	public NetPlayer OwningNetPlayer;

	// Token: 0x040029B8 RID: 10680
	[SerializeField]
	private FXSystemSettings sharedFXSettings;

	// Token: 0x040029B9 RID: 10681
	[NonSerialized]
	public FXSystemSettings fxSettings;

	// Token: 0x040029BA RID: 10682
	[SerializeField]
	private float tapPointDistance = 0.035f;

	// Token: 0x040029BB RID: 10683
	[SerializeField]
	private float handSpeedToVolumeModifier = 0.05f;

	// Token: 0x040029BC RID: 10684
	[SerializeField]
	private HandEffectContext _leftHandEffect;

	// Token: 0x040029BD RID: 10685
	[SerializeField]
	private HandEffectContext _rightHandEffect;

	// Token: 0x040029BE RID: 10686
	[SerializeField]
	private HandEffectContext _extraLeftHandEffect;

	// Token: 0x040029BF RID: 10687
	[SerializeField]
	private HandEffectContext _extraRightHandEffect;

	// Token: 0x040029C0 RID: 10688
	[SerializeField]
	private Transform renderTransform;

	// Token: 0x040029C1 RID: 10689
	private GamePlayer _gamePlayerRef;

	// Token: 0x040029C2 RID: 10690
	private bool playerWasHaunted;

	// Token: 0x040029C3 RID: 10691
	private float nonHauntedVolume;

	// Token: 0x040029C4 RID: 10692
	[SerializeField]
	private AnimationCurve voicePitchForRelativeScale;

	// Token: 0x040029C5 RID: 10693
	private Vector3 LocalTrajectoryOverridePosition;

	// Token: 0x040029C6 RID: 10694
	private Vector3 LocalTrajectoryOverrideVelocity;

	// Token: 0x040029C7 RID: 10695
	private float LocalTrajectoryOverrideBlend;

	// Token: 0x040029C8 RID: 10696
	[SerializeField]
	private float LocalTrajectoryOverrideDuration = 1f;

	// Token: 0x040029C9 RID: 10697
	private bool localOverrideIsBody;

	// Token: 0x040029CA RID: 10698
	private bool localOverrideIsLeftHand;

	// Token: 0x040029CB RID: 10699
	private Transform localOverrideGrabbingHand;

	// Token: 0x040029CC RID: 10700
	private float localGrabOverrideBlend;

	// Token: 0x040029CD RID: 10701
	[SerializeField]
	private float LocalGrabOverrideDuration = 0.25f;

	// Token: 0x040029CE RID: 10702
	private float[] voiceSampleBuffer = new float[128];

	// Token: 0x040029CF RID: 10703
	private const int CHECK_LOUDNESS_FREQ_FRAMES = 10;

	// Token: 0x040029D0 RID: 10704
	private CallbackContainer<ICallBack> lateUpdateCallbacks = new CallbackContainer<ICallBack>(5);

	// Token: 0x040029D1 RID: 10705
	private float nextLocalVelocityStoreTimestamp;

	// Token: 0x040029D2 RID: 10706
	private bool IsInvisibleToLocalPlayer;

	// Token: 0x040029D3 RID: 10707
	private const int remoteUseReplacementVoice_BIT = 512;

	// Token: 0x040029D4 RID: 10708
	private const int grabbedRope_BIT = 1024;

	// Token: 0x040029D5 RID: 10709
	private const int grabbedRopeIsPhotonView_BIT = 2048;

	// Token: 0x040029D6 RID: 10710
	private const int isHoldingHandsWithPlayer_BIT = 4096;

	// Token: 0x040029D7 RID: 10711
	private const int isHoldingHoverboard_BIT = 8192;

	// Token: 0x040029D8 RID: 10712
	private const int isHoverboardLeftHanded_BIT = 16384;

	// Token: 0x040029D9 RID: 10713
	private const int isOnMovingSurface_BIT = 32768;

	// Token: 0x040029DA RID: 10714
	private const int isPropHunt_BIT = 65536;

	// Token: 0x040029DB RID: 10715
	private const int propHuntLeftHand_BIT = 131072;

	// Token: 0x040029DC RID: 10716
	private const int isLeftHandGrabbable_BIT = 262144;

	// Token: 0x040029DD RID: 10717
	private const int isRightHandGrabbable_BIT = 524288;

	// Token: 0x040029DE RID: 10718
	private const int isLeftHandTentacleHoldingHand_BIT = 1048576;

	// Token: 0x040029DF RID: 10719
	private const int isRightHandTentacleHoldingHand_BIT = 2097152;

	// Token: 0x040029E0 RID: 10720
	private const int showSubscriber_BIT = 4194304;

	// Token: 0x040029E1 RID: 10721
	private const int speakingLoudnessVal_BITSHIFT = 24;

	// Token: 0x040029E2 RID: 10722
	private GorillaIK myIk;

	// Token: 0x040029E3 RID: 10723
	private Vector3 tempVec;

	// Token: 0x040029E4 RID: 10724
	private Quaternion tempQuat;

	// Token: 0x040029E5 RID: 10725
	public Action<int, int> OnMaterialIndexChanged;

	// Token: 0x040029E6 RID: 10726
	[SerializeField]
	private ParticleSystem cosmeticsActivationPS;

	// Token: 0x040029E7 RID: 10727
	[SerializeField]
	private SoundBankPlayer cosmeticsActivationSBP;

	// Token: 0x040029E8 RID: 10728
	public Color playerColor;

	// Token: 0x040029E9 RID: 10729
	public bool colorInitialized;

	// Token: 0x040029EA RID: 10730
	private Action<Color> onColorInitialized;

	// Token: 0x040029ED RID: 10733
	private bool m_sentRankedScore;

	// Token: 0x040029EF RID: 10735
	private int currentQuestScore;

	// Token: 0x040029F0 RID: 10736
	private bool _scoreUpdated;

	// Token: 0x040029F1 RID: 10737
	private CallLimiter updateQuestCallLimit = new CallLimiter(1, 0.5f, 0.5f);

	// Token: 0x040029F3 RID: 10739
	private float currentRankedELO;

	// Token: 0x040029F4 RID: 10740
	private int currentRankedSubTierQuest;

	// Token: 0x040029F5 RID: 10741
	private int currentRankedSubTierPC;

	// Token: 0x040029F6 RID: 10742
	private bool _rankedInfoUpdated;

	// Token: 0x040029F7 RID: 10743
	internal CallLimiter updateRankedInfoCallLimit = new CallLimiter(2, 60f, 0.5f);

	// Token: 0x040029F8 RID: 10744
	public const float maxGuardianThrowVelocity = 20f;

	// Token: 0x040029F9 RID: 10745
	public const float maxRegularThrowVelocity = 3f;

	// Token: 0x040029FA RID: 10746
	private RaycastHit[] rayCastNonAllocColliders = new RaycastHit[5];

	// Token: 0x040029FB RID: 10747
	private bool inDuplicationZone;

	// Token: 0x040029FC RID: 10748
	private RigDuplicationZone duplicationZone;

	// Token: 0x040029FD RID: 10749
	private Vector3 cachedRenderTransformPos = new Vector3(0f, -1.65f, 0f);

	// Token: 0x040029FE RID: 10750
	private bool pendingCosmeticUpdate = true;

	// Token: 0x04002A00 RID: 10752
	[NonSerialized]
	private bool showGoldNameTag;

	// Token: 0x04002A01 RID: 10753
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Right = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002A02 RID: 10754
	public List<HandEffectsOverrideCosmetic> CosmeticHandEffectsOverride_Left = new List<HandEffectsOverrideCosmetic>();

	// Token: 0x04002A03 RID: 10755
	private int loudnessCheckFrame;

	// Token: 0x04002A04 RID: 10756
	private float frameScale;

	// Token: 0x04002A05 RID: 10757
	private SubscriptionManager.SubscriptionDetails subDataCache;

	// Token: 0x04002A06 RID: 10758
	private const bool SHOW_SCREENS = false;

	// Token: 0x04002A07 RID: 10759
	[OnEnterPlay_SetNull]
	private static VRRig gLocalRig;

	// Token: 0x020004EC RID: 1260
	public enum PartyMemberStatus
	{
		// Token: 0x04002A0A RID: 10762
		NeedsUpdate,
		// Token: 0x04002A0B RID: 10763
		InLocalParty,
		// Token: 0x04002A0C RID: 10764
		NotInLocalParty
	}

	// Token: 0x020004ED RID: 1261
	public enum WearablePackedStateSlots
	{
		// Token: 0x04002A0E RID: 10766
		Hat,
		// Token: 0x04002A0F RID: 10767
		LeftHand,
		// Token: 0x04002A10 RID: 10768
		RightHand,
		// Token: 0x04002A11 RID: 10769
		Face,
		// Token: 0x04002A12 RID: 10770
		Pants1,
		// Token: 0x04002A13 RID: 10771
		Pants2,
		// Token: 0x04002A14 RID: 10772
		Badge,
		// Token: 0x04002A15 RID: 10773
		Fur,
		// Token: 0x04002A16 RID: 10774
		Shirt
	}

	// Token: 0x020004EE RID: 1262
	public struct VelocityTime
	{
		// Token: 0x06001F98 RID: 8088 RVA: 0x000AA66D File Offset: 0x000A886D
		public VelocityTime(Vector3 velocity, double velTime)
		{
			this.vel = velocity;
			this.time = velTime;
		}

		// Token: 0x04002A17 RID: 10775
		public Vector3 vel;

		// Token: 0x04002A18 RID: 10776
		public double time;
	}
}
