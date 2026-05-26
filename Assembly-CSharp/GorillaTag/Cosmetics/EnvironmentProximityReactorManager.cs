using System;
using System.Collections.Generic;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001275 RID: 4725
	public class EnvironmentProximityReactorManager : NetworkSceneObject
	{
		// Token: 0x17000B6B RID: 2923
		// (get) Token: 0x0600766B RID: 30315 RVA: 0x0026D7AD File Offset: 0x0026B9AD
		public static EnvironmentProximityReactorManager Instance
		{
			get
			{
				return EnvironmentProximityReactorManager.instance;
			}
		}

		// Token: 0x0600766C RID: 30316 RVA: 0x0026D7B4 File Offset: 0x0026B9B4
		private void Awake()
		{
			if (EnvironmentProximityReactorManager.instance != null && EnvironmentProximityReactorManager.instance != this)
			{
				GTDev.LogWarning<string>("[EnvironmentProximityReactorManager] Duplicate instance of the Environment Reactor Manager, destroying.", null);
				UnityEngine.Object.Destroy(this);
				return;
			}
			EnvironmentProximityReactorManager.instance = this;
			foreach (EnvironmentProximityReactor environmentProximityReactor in EnvironmentProximityReactorManager.registry)
			{
				if (environmentProximityReactor != null)
				{
					this.RegisterInstance(environmentProximityReactor);
				}
			}
			EnvironmentProximityReactorManager.registry.Clear();
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(this.OnPlayerJoined);
			RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeft);
			RoomSystem.LeftRoomEvent += new Action(this.OnLeftRoom);
			CosmeticsProximityReactorManager.OnCosmeticRegistered += this.OnCosmeticRegistered;
		}

		// Token: 0x0600766D RID: 30317 RVA: 0x0026D8B0 File Offset: 0x0026BAB0
		private void OnDestroy()
		{
			NetworkBehaviourUtils.InternalOnDestroy(this);
			if (EnvironmentProximityReactorManager.instance == this)
			{
				EnvironmentProximityReactorManager.instance = null;
			}
			if (NetworkSystem.Instance != null)
			{
				RoomSystem.PlayerJoinedEvent -= new Action<NetPlayer>(this.OnPlayerJoined);
				RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeft);
				RoomSystem.LeftRoomEvent -= new Action(this.OnLeftRoom);
			}
			CosmeticsProximityReactorManager.OnCosmeticRegistered -= this.OnCosmeticRegistered;
		}

		// Token: 0x0600766E RID: 30318 RVA: 0x0026D945 File Offset: 0x0026BB45
		private void OnPlayerLeft(NetPlayer player)
		{
			this.pendingEvents.Remove(player.ActorNumber);
		}

		// Token: 0x0600766F RID: 30319 RVA: 0x0026D959 File Offset: 0x0026BB59
		private void OnLeftRoom()
		{
			this.pendingEvents.Clear();
		}

		// Token: 0x06007670 RID: 30320 RVA: 0x0026D968 File Offset: 0x0026BB68
		private void OnPlayerJoined(NetPlayer newPlayer)
		{
			if (newPlayer.IsLocal || !NetworkSystem.Instance.InRoom)
			{
				return;
			}
			for (int i = 0; i < this.reactors.Count; i++)
			{
				if (this.reactors[i] != null)
				{
					this.reactors[i].SyncStateTo(newPlayer, this);
				}
			}
		}

		// Token: 0x06007671 RID: 30321 RVA: 0x0026D9C8 File Offset: 0x0026BBC8
		public void BroadcastProximityStateTo(NetPlayer target, int reactorId, int blockIndex, bool isBelow)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.photonView.RPC("ProximityStateRPC", ((PunNetPlayer)target).PlayerRef, new object[]
			{
				reactorId,
				blockIndex,
				isBelow
			});
		}

		// Token: 0x06007672 RID: 30322 RVA: 0x0026DA20 File Offset: 0x0026BC20
		private bool CheckPlayerRateLimit(NetPlayer sender)
		{
			RigContainer rigContainer;
			return VRRigCache.Instance.TryGetVrrig(sender, out rigContainer) && rigContainer.Rig.fxSettings.callSettings[24].CallLimitSettings.CheckCallTime(Time.unscaledTime);
		}

		// Token: 0x06007673 RID: 30323 RVA: 0x0026DA60 File Offset: 0x0026BC60
		private bool SenderHasValidCosmetic(int reactorId, int blockIndex, PhotonMessageInfoWrapped info)
		{
			if (CosmeticsProximityReactorManager.Instance == null)
			{
				return false;
			}
			EnvironmentProximityReactor environmentProximityReactor = null;
			for (int i = 0; i < this.reactors.Count; i++)
			{
				if (this.reactors[i] != null && this.reactors[i].reactorId == reactorId)
				{
					environmentProximityReactor = this.reactors[i];
					break;
				}
			}
			if (environmentProximityReactor == null || blockIndex >= environmentProximityReactor.blocks.Count)
			{
				return false;
			}
			EnvironmentProximityReactor.InteractionBlock interactionBlock = environmentProximityReactor.blocks[blockIndex];
			IReadOnlyList<CosmeticsProximityReactor> cosmetics = CosmeticsProximityReactorManager.Instance.Cosmetics;
			for (int j = 0; j < cosmetics.Count; j++)
			{
				CosmeticsProximityReactor cosmeticsProximityReactor = cosmetics[j];
				if (!(cosmeticsProximityReactor == null))
				{
					VRRig ownerRig = cosmeticsProximityReactor.GetOwnerRig();
					if (!(ownerRig == null) && ownerRig.Creator == info.Sender && interactionBlock.CanTriggerFrom(cosmeticsProximityReactor))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06007674 RID: 30324 RVA: 0x0026DB54 File Offset: 0x0026BD54
		private bool SenderIsInRange(int reactorId, int blockIndex, PhotonMessageInfoWrapped info)
		{
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(info.Sender, out rigContainer))
			{
				return false;
			}
			EnvironmentProximityReactor environmentProximityReactor = null;
			for (int i = 0; i < this.reactors.Count; i++)
			{
				if (this.reactors[i] != null && this.reactors[i].reactorId == reactorId)
				{
					environmentProximityReactor = this.reactors[i];
					break;
				}
			}
			if (environmentProximityReactor == null || blockIndex >= environmentProximityReactor.blocks.Count)
			{
				return false;
			}
			float range = environmentProximityReactor.blocks[blockIndex].proximityThreshold + this.distanceBuffer;
			return rigContainer.Rig.IsPositionInRange(environmentProximityReactor.transform.position, range);
		}

		// Token: 0x06007675 RID: 30325 RVA: 0x0026DC10 File Offset: 0x0026BE10
		private void OnCosmeticRegistered(CosmeticsProximityReactor cosmetic)
		{
			VRRig ownerRig = cosmetic.GetOwnerRig();
			if (ownerRig == null || ownerRig.Creator == null)
			{
				return;
			}
			int actorNumber = ownerRig.Creator.ActorNumber;
			List<EnvironmentProximityReactorManager.PendingProximityEvent> list;
			if (!this.pendingEvents.TryGetValue(actorNumber, out list))
			{
				return;
			}
			float unscaledTime = Time.unscaledTime;
			for (int i = list.Count - 1; i >= 0; i--)
			{
				EnvironmentProximityReactorManager.PendingProximityEvent pendingProximityEvent = list[i];
				if (unscaledTime - pendingProximityEvent.receivedTime > 10f)
				{
					list.RemoveAt(i);
				}
				else if (this.SenderHasValidCosmetic(pendingProximityEvent.reactorId, pendingProximityEvent.blockIndex, pendingProximityEvent.info))
				{
					if (!this.SenderIsInRange(pendingProximityEvent.reactorId, pendingProximityEvent.blockIndex, pendingProximityEvent.info))
					{
						list.RemoveAt(i);
					}
					else
					{
						this.ApplyProximityEventToReactor(pendingProximityEvent.reactorId, pendingProximityEvent.blockIndex, pendingProximityEvent.isBelow);
						list.RemoveAt(i);
					}
				}
			}
			if (list.Count == 0)
			{
				this.pendingEvents.Remove(actorNumber);
			}
		}

		// Token: 0x06007676 RID: 30326 RVA: 0x0026DD18 File Offset: 0x0026BF18
		private void Update()
		{
			if (this.pendingEvents.Count == 0)
			{
				return;
			}
			float unscaledTime = Time.unscaledTime;
			foreach (KeyValuePair<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>> keyValuePair in this.pendingEvents)
			{
				List<EnvironmentProximityReactorManager.PendingProximityEvent> value = keyValuePair.Value;
				for (int i = value.Count - 1; i >= 0; i--)
				{
					if (unscaledTime - value[i].receivedTime > 10f)
					{
						value.RemoveAt(i);
					}
				}
			}
			foreach (KeyValuePair<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>> keyValuePair2 in this.pendingEvents)
			{
				if (keyValuePair2.Value.Count == 0)
				{
					this.pendingEvents.Remove(keyValuePair2.Key);
					break;
				}
			}
		}

		// Token: 0x06007677 RID: 30327 RVA: 0x0026DE14 File Offset: 0x0026C014
		private void TryCacheProximityEvent(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfoWrapped info)
		{
			int actorNumber = info.Sender.ActorNumber;
			List<EnvironmentProximityReactorManager.PendingProximityEvent> list;
			if (!this.pendingEvents.TryGetValue(actorNumber, out list))
			{
				list = new List<EnvironmentProximityReactorManager.PendingProximityEvent>();
				this.pendingEvents[actorNumber] = list;
			}
			else if (list.Exists((EnvironmentProximityReactorManager.PendingProximityEvent e) => e.reactorId == reactorId))
			{
				return;
			}
			list.Add(new EnvironmentProximityReactorManager.PendingProximityEvent
			{
				reactorId = reactorId,
				blockIndex = blockIndex,
				isBelow = isBelow,
				info = info,
				receivedTime = Time.unscaledTime
			});
		}

		// Token: 0x06007678 RID: 30328 RVA: 0x0026DEB8 File Offset: 0x0026C0B8
		private void ApplyProximityEventToReactor(int reactorId, int blockIndex, bool isBelow)
		{
			for (int i = 0; i < this.reactors.Count; i++)
			{
				EnvironmentProximityReactor environmentProximityReactor = this.reactors[i];
				if (!(environmentProximityReactor == null) && environmentProximityReactor.reactorId == reactorId)
				{
					environmentProximityReactor.ApplySharedProximity(blockIndex, isBelow);
					return;
				}
			}
		}

		// Token: 0x06007679 RID: 30329 RVA: 0x0026DF03 File Offset: 0x0026C103
		private void RegisterInstance(EnvironmentProximityReactor reactor)
		{
			if (reactor == null)
			{
				return;
			}
			if (this.idSet.Add(reactor.reactorId))
			{
				this.reactors.Add(reactor);
			}
		}

		// Token: 0x0600767A RID: 30330 RVA: 0x0026DF2E File Offset: 0x0026C12E
		private void UnregisterInstance(EnvironmentProximityReactor reactor)
		{
			if (reactor == null)
			{
				return;
			}
			if (!this.idSet.Remove(reactor.reactorId))
			{
				return;
			}
			this.reactors.Remove(reactor);
		}

		// Token: 0x0600767B RID: 30331 RVA: 0x0026DF5B File Offset: 0x0026C15B
		public static void Register(EnvironmentProximityReactor reactor)
		{
			if (EnvironmentProximityReactorManager.instance != null)
			{
				EnvironmentProximityReactorManager.instance.RegisterInstance(reactor);
				return;
			}
			EnvironmentProximityReactorManager.registry.Add(reactor);
		}

		// Token: 0x0600767C RID: 30332 RVA: 0x0026DF82 File Offset: 0x0026C182
		public static void Unregister(EnvironmentProximityReactor reactor)
		{
			if (EnvironmentProximityReactorManager.instance != null)
			{
				EnvironmentProximityReactorManager.instance.UnregisterInstance(reactor);
				return;
			}
			EnvironmentProximityReactorManager.registry.Remove(reactor);
		}

		// Token: 0x0600767D RID: 30333 RVA: 0x0026DFAC File Offset: 0x0026C1AC
		public void BroadcastProximityState(int reactorId, int blockIndex, bool isBelow)
		{
			if (!NetworkSystem.Instance.InRoom)
			{
				return;
			}
			this.photonView.RPC("ProximityStateRPC", RpcTarget.Others, new object[]
			{
				reactorId,
				blockIndex,
				isBelow
			});
		}

		// Token: 0x0600767E RID: 30334 RVA: 0x0026DFF8 File Offset: 0x0026C1F8
		[PunRPC]
		public void ProximityStateRPC(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfo info)
		{
			this.ApplyProximityStateShared(reactorId, blockIndex, isBelow, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x0600767F RID: 30335 RVA: 0x0026E00C File Offset: 0x0026C20C
		[Rpc]
		public unsafe static void RPC_ProximityState(NetworkRunner runner, int reactorId, int blockIndex, bool isBelow, RpcInfo info = default(RpcInfo))
		{
			if (NetworkBehaviourUtils.InvokeRpc)
			{
				NetworkBehaviourUtils.InvokeRpc = false;
			}
			else
			{
				if (runner == null)
				{
					throw new ArgumentNullException("runner");
				}
				if (runner.Stage != SimulationStages.Resimulate)
				{
					int num = 8;
					num += 4;
					num += 4;
					num += 4;
					if (SimulationMessage.CanAllocateUserPayload(num))
					{
						if (runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)"));
							int num2 = 8;
							*(int*)(ptr2 + num2) = reactorId;
							num2 += 4;
							*(int*)(ptr2 + num2) = blockIndex;
							num2 += 4;
							ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), isBelow);
							num2 += 4;
							ptr->Offset = num2 * 8;
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
						info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_10;
					}
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)", num);
				}
				return;
			}
			IL_10:
			if (EnvironmentProximityReactorManager.instance == null)
			{
				return;
			}
			EnvironmentProximityReactorManager.instance.ApplyProximityStateShared(reactorId, blockIndex, isBelow, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06007680 RID: 30336 RVA: 0x0026E180 File Offset: 0x0026C380
		private void ApplyProximityStateShared(int reactorId, int blockIndex, bool isBelow, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "ApplyProximityStateShared");
			if (blockIndex < 0)
			{
				return;
			}
			if (!this.idSet.Contains(reactorId))
			{
				MonkeAgent.instance.SendReport("Sent invalid reactorId in ProximityStateRPC", info.Sender.UserId, info.Sender.NickName);
				return;
			}
			if (!this.CheckPlayerRateLimit(info.Sender))
			{
				return;
			}
			if (!this.SenderHasValidCosmetic(reactorId, blockIndex, info))
			{
				this.TryCacheProximityEvent(reactorId, blockIndex, isBelow, info);
				return;
			}
			if (!this.SenderIsInRange(reactorId, blockIndex, info))
			{
				MonkeAgent.instance.SendReport("Sent ProximityStateRPC from out of range", info.Sender.UserId, info.Sender.NickName);
				return;
			}
			this.ApplyProximityEventToReactor(reactorId, blockIndex, isBelow);
		}

		// Token: 0x06007683 RID: 30339 RVA: 0x0026E280 File Offset: 0x0026C480
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTag.Cosmetics.EnvironmentProximityReactorManager::RPC_ProximityState(Fusion.NetworkRunner,System.Int32,System.Int32,System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_ProximityState@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			int num2 = *(int*)(ptr + num);
			num += 4;
			int reactorId = num2;
			int num3 = *(int*)(ptr + num);
			num += 4;
			int blockIndex = num3;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
			num += 4;
			bool isBelow = flag;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			EnvironmentProximityReactorManager.RPC_ProximityState(runner, reactorId, blockIndex, isBelow, info);
		}

		// Token: 0x04008884 RID: 34948
		private static EnvironmentProximityReactorManager instance;

		// Token: 0x04008885 RID: 34949
		[SerializeField]
		private List<EnvironmentProximityReactor> reactors = new List<EnvironmentProximityReactor>();

		// Token: 0x04008886 RID: 34950
		private readonly HashSet<int> idSet = new HashSet<int>();

		// Token: 0x04008887 RID: 34951
		private readonly Dictionary<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>> pendingEvents = new Dictionary<int, List<EnvironmentProximityReactorManager.PendingProximityEvent>>();

		// Token: 0x04008888 RID: 34952
		private float distanceBuffer = 3f;

		// Token: 0x04008889 RID: 34953
		private const float cosmeticSyncTimeout = 10f;

		// Token: 0x0400888A RID: 34954
		private static readonly HashSet<EnvironmentProximityReactor> registry = new HashSet<EnvironmentProximityReactor>();

		// Token: 0x02001276 RID: 4726
		private struct PendingProximityEvent
		{
			// Token: 0x0400888B RID: 34955
			public int reactorId;

			// Token: 0x0400888C RID: 34956
			public int blockIndex;

			// Token: 0x0400888D RID: 34957
			public bool isBelow;

			// Token: 0x0400888E RID: 34958
			public PhotonMessageInfoWrapped info;

			// Token: 0x0400888F RID: 34959
			public float receivedTime;
		}
	}
}
