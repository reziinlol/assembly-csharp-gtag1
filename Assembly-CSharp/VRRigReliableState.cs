using System;
using System.Collections.Generic;
using Fusion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using UnityEngine;

// Token: 0x020004F8 RID: 1272
public class VRRigReliableState : MonoBehaviour, IWrappedSerializable, INetworkStruct
{
	// Token: 0x17000376 RID: 886
	// (get) Token: 0x06001FE2 RID: 8162 RVA: 0x000AB456 File Offset: 0x000A9656
	public bool HasBracelet
	{
		get
		{
			return this.braceletBeadColors.Count > 0;
		}
	}

	// Token: 0x17000377 RID: 887
	// (get) Token: 0x06001FE3 RID: 8163 RVA: 0x000AB466 File Offset: 0x000A9666
	// (set) Token: 0x06001FE4 RID: 8164 RVA: 0x000AB46E File Offset: 0x000A966E
	public bool isDirty { get; private set; } = true;

	// Token: 0x06001FE5 RID: 8165 RVA: 0x000AB477 File Offset: 0x000A9677
	private void Awake()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Combine(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
		RoomSystem.JoinedRoomEvent += new Action(this.SetIsDirty);
	}

	// Token: 0x06001FE6 RID: 8166 RVA: 0x000AB4B4 File Offset: 0x000A96B4
	private void OnDestroy()
	{
		VRRig.newPlayerJoined = (Action)Delegate.Remove(VRRig.newPlayerJoined, new Action(this.SetIsDirty));
	}

	// Token: 0x06001FE7 RID: 8167 RVA: 0x000AB4D6 File Offset: 0x000A96D6
	public void SetIsDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x06001FE8 RID: 8168 RVA: 0x000AB4DF File Offset: 0x000A96DF
	public void SetIsNotDirty()
	{
		this.isDirty = false;
	}

	// Token: 0x06001FE9 RID: 8169 RVA: 0x000AB4E8 File Offset: 0x000A96E8
	public void SharedStart(bool isOfflineVRRig_, BodyDockPositions bDock_)
	{
		this.isOfflineVRRig = isOfflineVRRig_;
		this.bDock = bDock_;
		this.activeTransferrableObjectIndex = new int[5];
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			this.activeTransferrableObjectIndex[i] = -1;
		}
		this.transferrablePosStates = new TransferrableObject.PositionState[5];
		this.transferrableItemStates = new TransferrableObject.ItemStates[5];
		this.transferableDockPositions = new BodyDockPositions.DropPositions[5];
	}

	// Token: 0x06001FEA RID: 8170 RVA: 0x000AB550 File Offset: 0x000A9750
	public void RegisterCosmeticStateSyncTarget(VRRigReliableState.StateSyncSlots slot, ICosmeticStateSync target)
	{
		if (this.m_cosmeticStateTargets[(int)slot] != null)
		{
			Debug.LogWarning(string.Format("{0}-CosmeticStateSync: instance already registered at slot {1}, this will be overriden", "VRRigReliableState", slot));
		}
		this.m_cosmeticStateTargets[(int)slot] = target;
		if (this.bDock.myRig.isOfflineVRRig)
		{
			this.m_cosmeticStates[(int)slot] = target.StateValue;
			this.isDirty = true;
			return;
		}
		target.OnStateUpdate(this.m_cosmeticStates[(int)slot]);
	}

	// Token: 0x06001FEB RID: 8171 RVA: 0x000AB5C4 File Offset: 0x000A97C4
	public void UnRegisterCosmeticStateSyncTarget(VRRigReliableState.StateSyncSlots slot, ICosmeticStateSync target)
	{
		if (this.m_cosmeticStateTargets[(int)slot] != target)
		{
			Debug.LogWarning(string.Format("{0}-CosmeticStateSync: target is not the value stored at slot {1}, ignoring", "VRRigReliableState", slot));
			return;
		}
		this.m_cosmeticStateTargets[(int)slot] = null;
		this.m_cosmeticStates[(int)slot] = -1;
		if (this.bDock.myRig.isOfflineVRRig)
		{
			this.isDirty = true;
		}
	}

	// Token: 0x06001FEC RID: 8172 RVA: 0x000AB624 File Offset: 0x000A9824
	private void CopyStateSyncToSyncArray()
	{
		for (int i = 0; i < this.m_cosmeticStateTargets.Length; i++)
		{
			ICosmeticStateSync cosmeticStateSync = this.m_cosmeticStateTargets[i];
			int num = (cosmeticStateSync != null) ? cosmeticStateSync.StateValue : -1;
			if (num != this.m_cosmeticStates[i])
			{
				this.isDirty = true;
			}
			this.m_cosmeticStates[i] = num;
		}
	}

	// Token: 0x06001FED RID: 8173 RVA: 0x000AB674 File Offset: 0x000A9874
	public int GetCachedStateAtSlot(VRRigReliableState.StateSyncSlots slot)
	{
		if (slot < VRRigReliableState.StateSyncSlots.Hat || slot >= (VRRigReliableState.StateSyncSlots)this.m_cosmeticStates.Length)
		{
			return -1;
		}
		return this.m_cosmeticStates[(int)slot];
	}

	// Token: 0x06001FEE RID: 8174 RVA: 0x000AB69C File Offset: 0x000A989C
	void IWrappedSerializable.OnSerializeRead(object newData)
	{
		this.Data = (ReliableStateData)newData;
		long header = this.Data.Header;
		int num;
		this.SetHeader(header, out num);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & 1L << (i & 31)) != 0L)
			{
				long num2 = this.Data.TransferrableStates[i];
				this.activeTransferrableObjectIndex[i] = (int)num2;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)(num2 >> 32 & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)(num2 >> 40 & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)(num2 >> 48 & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = this.Data.WearablesPackedState;
		this.lThrowableProjectileIndex = this.Data.LThrowableProjectileIndex;
		this.rThrowableProjectileIndex = this.Data.RThrowableProjectileIndex;
		this.sizeLayerMask = this.Data.SizeLayerMask;
		this.randomThrowableIndex = this.Data.RandomThrowableIndex;
		this.braceletBeadColors.Clear();
		if (num > 0)
		{
			if (num <= 3)
			{
				int num3 = (int)this.Data.PackedBeads;
				this.braceletSelfIndex = num3 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num3, 0, num, this.braceletBeadColors);
			}
			else
			{
				long packedBeads = this.Data.PackedBeads;
				this.braceletSelfIndex = (int)(packedBeads >> 60);
				if (num <= 6)
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, num, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(packedBeads, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors(this.Data.PackedBeadsMoreThan6, 6, num, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
	}

	// Token: 0x06001FEF RID: 8175 RVA: 0x000AB878 File Offset: 0x000A9A78
	object IWrappedSerializable.OnSerializeWrite()
	{
		this.isDirty = false;
		ReliableStateData reliableStateData = default(ReliableStateData);
		long header = this.GetHeader();
		reliableStateData.Header = header;
		long[] array = this.GetTransferrableStates(header).ToArray();
		reliableStateData.TransferrableStates.CopyFrom(array, 0, array.Length);
		reliableStateData.WearablesPackedState = this.wearablesPackedStates;
		reliableStateData.LThrowableProjectileIndex = this.lThrowableProjectileIndex;
		reliableStateData.RThrowableProjectileIndex = this.rThrowableProjectileIndex;
		reliableStateData.SizeLayerMask = this.sizeLayerMask;
		reliableStateData.RandomThrowableIndex = this.randomThrowableIndex;
		if (this.braceletBeadColors.Count > 0)
		{
			long num = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num |= (long)this.braceletSelfIndex << 30;
				reliableStateData.PackedBeads = num;
			}
			else
			{
				num |= (long)this.braceletSelfIndex << 60;
				reliableStateData.PackedBeads = num;
				if (this.braceletBeadColors.Count > 6)
				{
					reliableStateData.PackedBeadsMoreThan6 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6);
				}
			}
		}
		this.Data = reliableStateData;
		return reliableStateData;
	}

	// Token: 0x06001FF0 RID: 8176 RVA: 0x000AB990 File Offset: 0x000A9B90
	void IWrappedSerializable.OnSerializeWrite(PhotonStream stream, PhotonMessageInfo info)
	{
		this.CopyStateSyncToSyncArray();
		if (!this.isDirty)
		{
			return;
		}
		this.isDirty = false;
		long header = this.GetHeader();
		stream.SendNext(header);
		foreach (long num in this.GetTransferrableStates(header))
		{
			stream.SendNext(num);
		}
		stream.SendNext(this.wearablesPackedStates);
		stream.SendNext(this.lThrowableProjectileIndex);
		stream.SendNext(this.rThrowableProjectileIndex);
		stream.SendNext(this.sizeLayerMask);
		stream.SendNext(this.randomThrowableIndex);
		foreach (int num2 in this.m_cosmeticStates)
		{
			stream.SendNext(num2);
		}
		if (this.braceletBeadColors.Count > 0)
		{
			long num3 = VRRigReliableState.PackBeadColors(this.braceletBeadColors, 0);
			if (this.braceletBeadColors.Count <= 3)
			{
				num3 |= (long)this.braceletSelfIndex << 30;
				stream.SendNext((int)num3);
				return;
			}
			num3 |= (long)this.braceletSelfIndex << 60;
			stream.SendNext(num3);
			if (this.braceletBeadColors.Count > 6)
			{
				stream.SendNext(VRRigReliableState.PackBeadColors(this.braceletBeadColors, 6));
			}
		}
	}

	// Token: 0x06001FF1 RID: 8177 RVA: 0x000ABB1C File Offset: 0x000A9D1C
	void IWrappedSerializable.OnSerializeRead(PhotonStream stream, PhotonMessageInfo info)
	{
		long num = (long)stream.ReceiveNext();
		this.isMicEnabled = ((num & 32L) != 0L);
		this.isBraceletLeftHanded = ((num & 64L) != 0L);
		this.isBuilderWatchEnabled = ((num & 128L) != 0L);
		int num2 = (int)(num >> 12) & 15;
		this.lThrowableProjectileColor.r = (byte)(num >> 16);
		this.lThrowableProjectileColor.g = (byte)(num >> 24);
		this.lThrowableProjectileColor.b = (byte)(num >> 32);
		this.rThrowableProjectileColor.r = (byte)(num >> 40);
		this.rThrowableProjectileColor.g = (byte)(num >> 48);
		this.rThrowableProjectileColor.b = (byte)(num >> 56);
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((num & 1L << (i & 31)) != 0L)
			{
				long num3 = (long)stream.ReceiveNext();
				this.activeTransferrableObjectIndex[i] = (int)num3;
				this.transferrablePosStates[i] = (TransferrableObject.PositionState)(num3 >> 32 & 255L);
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)(num3 >> 40 & 255L);
				this.transferableDockPositions[i] = (BodyDockPositions.DropPositions)(num3 >> 48 & 255L);
			}
			else
			{
				this.activeTransferrableObjectIndex[i] = -1;
				this.transferrablePosStates[i] = TransferrableObject.PositionState.None;
				this.transferrableItemStates[i] = (TransferrableObject.ItemStates)0;
				this.transferableDockPositions[i] = BodyDockPositions.DropPositions.None;
			}
		}
		this.wearablesPackedStates = (int)stream.ReceiveNext();
		this.lThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.rThrowableProjectileIndex = (int)stream.ReceiveNext();
		this.sizeLayerMask = (int)stream.ReceiveNext();
		this.randomThrowableIndex = (int)stream.ReceiveNext();
		for (int j = 0; j < this.m_cosmeticStates.Length; j++)
		{
			int num4 = (int)stream.ReceiveNext();
			this.m_cosmeticStates[j] = num4;
			ICosmeticStateSync cosmeticStateSync = this.m_cosmeticStateTargets[j];
			if (cosmeticStateSync != null)
			{
				cosmeticStateSync.OnStateUpdate(num4);
			}
		}
		this.braceletBeadColors.Clear();
		if (num2 > 0)
		{
			if (num2 <= 3)
			{
				int num5 = (int)stream.ReceiveNext();
				this.braceletSelfIndex = num5 >> 30;
				VRRigReliableState.UnpackBeadColors((long)num5, 0, num2, this.braceletBeadColors);
			}
			else
			{
				long num6 = (long)stream.ReceiveNext();
				this.braceletSelfIndex = (int)(num6 >> 60);
				if (num2 <= 6)
				{
					VRRigReliableState.UnpackBeadColors(num6, 0, num2, this.braceletBeadColors);
				}
				else
				{
					VRRigReliableState.UnpackBeadColors(num6, 0, 6, this.braceletBeadColors);
					VRRigReliableState.UnpackBeadColors((long)stream.ReceiveNext(), 6, num2, this.braceletBeadColors);
				}
			}
		}
		this.bDock.RefreshTransferrableItems();
		this.bDock.myRig.UpdateFriendshipBracelet();
		this.bDock.myRig.EnableBuilderResizeWatch(this.isBuilderWatchEnabled);
	}

	// Token: 0x06001FF2 RID: 8178 RVA: 0x000ABDCC File Offset: 0x000A9FCC
	private long GetHeader()
	{
		long num = 0L;
		if (CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
			{
				if (this.activeTransferrableObjectIndex[i] != -1 && (this.transferrablePosStates[i] == TransferrableObject.PositionState.InLeftHand || this.transferrablePosStates[i] == TransferrableObject.PositionState.InRightHand))
				{
					num |= (long)((ulong)((byte)(1 << i)));
				}
			}
		}
		else
		{
			for (int j = 0; j < this.activeTransferrableObjectIndex.Length; j++)
			{
				if (this.activeTransferrableObjectIndex[j] != -1)
				{
					num |= (long)((ulong)((byte)(1 << j)));
				}
			}
		}
		if (this.isBraceletLeftHanded)
		{
			num |= 64L;
		}
		if (this.isMicEnabled)
		{
			num |= 32L;
		}
		if (this.isBuilderWatchEnabled && !CosmeticsController.instance.isHidingCosmeticsFromRemotePlayers)
		{
			num |= 128L;
		}
		num |= ((long)this.braceletBeadColors.Count & 15L) << 12;
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.r) << 16);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.g) << 24);
		num |= (long)((long)((ulong)this.lThrowableProjectileColor.b) << 32);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.r) << 40);
		num |= (long)((long)((ulong)this.rThrowableProjectileColor.g) << 48);
		return num | (long)((long)((ulong)this.rThrowableProjectileColor.b) << 56);
	}

	// Token: 0x06001FF3 RID: 8179 RVA: 0x000ABF14 File Offset: 0x000AA114
	private void SetHeader(long header, out int numBeadsToRead)
	{
		this.isMicEnabled = ((header & 32L) != 0L);
		this.isBraceletLeftHanded = ((header & 64L) != 0L);
		numBeadsToRead = ((int)(header >> 12) & 15);
		this.lThrowableProjectileColor.r = (byte)(header >> 16);
		this.lThrowableProjectileColor.g = (byte)(header >> 24);
		this.lThrowableProjectileColor.b = (byte)(header >> 32);
		this.rThrowableProjectileColor.r = (byte)(header >> 40);
		this.rThrowableProjectileColor.g = (byte)(header >> 48);
		this.rThrowableProjectileColor.b = (byte)(header >> 56);
	}

	// Token: 0x06001FF4 RID: 8180 RVA: 0x000ABFAC File Offset: 0x000AA1AC
	private List<long> GetTransferrableStates(long header)
	{
		List<long> list = new List<long>();
		for (int i = 0; i < this.activeTransferrableObjectIndex.Length; i++)
		{
			if ((header & 1L << (i & 31)) != 0L && this.activeTransferrableObjectIndex[i] != -1)
			{
				long num = (long)((ulong)this.activeTransferrableObjectIndex[i]);
				num |= (long)this.transferrablePosStates[i] << 32;
				num |= (long)this.transferrableItemStates[i] << 40;
				num |= (long)this.transferableDockPositions[i] << 48;
				list.Add(num);
			}
		}
		return list;
	}

	// Token: 0x06001FF5 RID: 8181 RVA: 0x000AC028 File Offset: 0x000AA228
	private static long PackBeadColors(List<Color> beadColors, int fromIndex)
	{
		long num = 0L;
		int num2 = Mathf.Min(fromIndex + 6, beadColors.Count);
		int num3 = 0;
		for (int i = fromIndex; i < num2; i++)
		{
			long num4 = (long)FriendshipGroupDetection.PackColor(beadColors[i]);
			num |= num4 << num3;
			num3 += 10;
		}
		return num;
	}

	// Token: 0x06001FF6 RID: 8182 RVA: 0x000AC074 File Offset: 0x000AA274
	private static void UnpackBeadColors(long packed, int startIndex, int endIndex, List<Color> beadColorsResult)
	{
		int num = Mathf.Min(startIndex + 6, endIndex);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			short data = (short)(packed >> num2 & 1023L);
			beadColorsResult.Add(FriendshipGroupDetection.UnpackColor(data));
			num2 += 10;
		}
	}

	// Token: 0x04002A96 RID: 10902
	[NonSerialized]
	private ICosmeticStateSync[] m_cosmeticStateTargets = new ICosmeticStateSync[3];

	// Token: 0x04002A97 RID: 10903
	[NonSerialized]
	private int[] m_cosmeticStates = new int[3];

	// Token: 0x04002A98 RID: 10904
	[NonSerialized]
	public int[] activeTransferrableObjectIndex;

	// Token: 0x04002A99 RID: 10905
	[NonSerialized]
	public TransferrableObject.PositionState[] transferrablePosStates;

	// Token: 0x04002A9A RID: 10906
	[NonSerialized]
	public TransferrableObject.ItemStates[] transferrableItemStates;

	// Token: 0x04002A9B RID: 10907
	[NonSerialized]
	public BodyDockPositions.DropPositions[] transferableDockPositions;

	// Token: 0x04002A9C RID: 10908
	[NonSerialized]
	public int wearablesPackedStates;

	// Token: 0x04002A9D RID: 10909
	[NonSerialized]
	public int lThrowableProjectileIndex = -1;

	// Token: 0x04002A9E RID: 10910
	[NonSerialized]
	public int rThrowableProjectileIndex = -1;

	// Token: 0x04002A9F RID: 10911
	[NonSerialized]
	public Color32 lThrowableProjectileColor = Color.white;

	// Token: 0x04002AA0 RID: 10912
	[NonSerialized]
	public Color32 rThrowableProjectileColor = Color.white;

	// Token: 0x04002AA1 RID: 10913
	[NonSerialized]
	public int randomThrowableIndex;

	// Token: 0x04002AA2 RID: 10914
	[NonSerialized]
	public bool isMicEnabled;

	// Token: 0x04002AA3 RID: 10915
	private bool isOfflineVRRig;

	// Token: 0x04002AA4 RID: 10916
	private BodyDockPositions bDock;

	// Token: 0x04002AA5 RID: 10917
	[NonSerialized]
	public int sizeLayerMask = 1;

	// Token: 0x04002AA6 RID: 10918
	private const long IS_MIC_ENABLED_BIT = 32L;

	// Token: 0x04002AA7 RID: 10919
	private const long BRACELET_LEFTHAND_BIT = 64L;

	// Token: 0x04002AA8 RID: 10920
	private const long BUILDER_WATCH_ENABLED_BIT = 128L;

	// Token: 0x04002AA9 RID: 10921
	private const int BRACELET_NUM_BEADS_SHIFT = 12;

	// Token: 0x04002AAA RID: 10922
	private const int LPROJECTILECOLOR_R_SHIFT = 16;

	// Token: 0x04002AAB RID: 10923
	private const int LPROJECTILECOLOR_G_SHIFT = 24;

	// Token: 0x04002AAC RID: 10924
	private const int LPROJECTILECOLOR_B_SHIFT = 32;

	// Token: 0x04002AAD RID: 10925
	private const int RPROJECTILECOLOR_R_SHIFT = 40;

	// Token: 0x04002AAE RID: 10926
	private const int RPROJECTILECOLOR_G_SHIFT = 48;

	// Token: 0x04002AAF RID: 10927
	private const int RPROJECTILECOLOR_B_SHIFT = 56;

	// Token: 0x04002AB0 RID: 10928
	private const int POS_STATES_SHIFT = 32;

	// Token: 0x04002AB1 RID: 10929
	private const int ITEM_STATES_SHIFT = 40;

	// Token: 0x04002AB2 RID: 10930
	private const int DOCK_POSITIONS_SHIFT = 48;

	// Token: 0x04002AB3 RID: 10931
	private const int BRACELET_SELF_INDEX_SHIFT = 60;

	// Token: 0x04002AB4 RID: 10932
	[NonSerialized]
	public bool isBraceletLeftHanded;

	// Token: 0x04002AB5 RID: 10933
	[NonSerialized]
	public int braceletSelfIndex;

	// Token: 0x04002AB6 RID: 10934
	[NonSerialized]
	public List<Color> braceletBeadColors = new List<Color>(10);

	// Token: 0x04002AB7 RID: 10935
	[NonSerialized]
	public bool isBuilderWatchEnabled;

	// Token: 0x04002AB9 RID: 10937
	private ReliableStateData Data;

	// Token: 0x020004F9 RID: 1273
	public enum StateSyncSlots
	{
		// Token: 0x04002ABB RID: 10939
		Hat,
		// Token: 0x04002ABC RID: 10940
		Shirt,
		// Token: 0x04002ABD RID: 10941
		Face,
		// Token: 0x04002ABE RID: 10942
		Length
	}
}
