using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fusion;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;
using GorillaTag;
using Photon.Pun;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;

namespace GorillaTagScripts
{
	// Token: 0x02000F05 RID: 3845
	public class FriendshipGroupDetection : NetworkSceneObject, ITickSystemTick
	{
		// Token: 0x17000911 RID: 2321
		// (get) Token: 0x06005F9F RID: 24479 RVA: 0x001EC53C File Offset: 0x001EA73C
		// (set) Token: 0x06005FA0 RID: 24480 RVA: 0x001EC543 File Offset: 0x001EA743
		public static FriendshipGroupDetection Instance { get; private set; }

		// Token: 0x17000912 RID: 2322
		// (get) Token: 0x06005FA1 RID: 24481 RVA: 0x001EC54B File Offset: 0x001EA74B
		// (set) Token: 0x06005FA2 RID: 24482 RVA: 0x001EC553 File Offset: 0x001EA753
		public List<Color> myBeadColors { get; private set; } = new List<Color>();

		// Token: 0x17000913 RID: 2323
		// (get) Token: 0x06005FA3 RID: 24483 RVA: 0x001EC55C File Offset: 0x001EA75C
		// (set) Token: 0x06005FA4 RID: 24484 RVA: 0x001EC564 File Offset: 0x001EA764
		public Color myBraceletColor { get; private set; }

		// Token: 0x17000914 RID: 2324
		// (get) Token: 0x06005FA5 RID: 24485 RVA: 0x001EC56D File Offset: 0x001EA76D
		// (set) Token: 0x06005FA6 RID: 24486 RVA: 0x001EC575 File Offset: 0x001EA775
		public int MyBraceletSelfIndex { get; private set; }

		// Token: 0x17000915 RID: 2325
		// (get) Token: 0x06005FA7 RID: 24487 RVA: 0x001EC57E File Offset: 0x001EA77E
		public List<string> PartyMemberIDs
		{
			get
			{
				return this.myPartyMemberIDs;
			}
		}

		// Token: 0x17000916 RID: 2326
		// (get) Token: 0x06005FA8 RID: 24488 RVA: 0x001EC586 File Offset: 0x001EA786
		public bool IsInParty
		{
			get
			{
				return this.myPartyMemberIDs != null;
			}
		}

		// Token: 0x17000917 RID: 2327
		// (get) Token: 0x06005FA9 RID: 24489 RVA: 0x001EC591 File Offset: 0x001EA791
		// (set) Token: 0x06005FAA RID: 24490 RVA: 0x001EC599 File Offset: 0x001EA799
		public GroupJoinZoneAB partyZone { get; private set; }

		// Token: 0x17000918 RID: 2328
		// (get) Token: 0x06005FAB RID: 24491 RVA: 0x001EC5A2 File Offset: 0x001EA7A2
		// (set) Token: 0x06005FAC RID: 24492 RVA: 0x001EC5AA File Offset: 0x001EA7AA
		public bool TickRunning { get; set; }

		// Token: 0x06005FAD RID: 24493 RVA: 0x001EC5B4 File Offset: 0x001EA7B4
		private void Awake()
		{
			FriendshipGroupDetection.Instance = this;
			if (this.friendshipBubble)
			{
				this.particleSystem = this.friendshipBubble.GetComponent<ParticleSystem>();
				this.audioSource = this.friendshipBubble.GetComponent<AudioSource>();
			}
			NetworkSystem.Instance.OnPlayerJoined += this.OnPlayerJoinedRoom;
		}

		// Token: 0x06005FAE RID: 24494 RVA: 0x000DE8C3 File Offset: 0x000DCAC3
		private new void OnEnable()
		{
			NetworkBehaviourUtils.InternalOnEnable(this);
			base.OnEnable();
			TickSystem<object>.AddTickCallback(this);
		}

		// Token: 0x06005FAF RID: 24495 RVA: 0x000DE8D7 File Offset: 0x000DCAD7
		private new void OnDisable()
		{
			NetworkBehaviourUtils.InternalOnDisable(this);
			base.OnDisable();
			TickSystem<object>.RemoveTickCallback(this);
		}

		// Token: 0x06005FB0 RID: 24496 RVA: 0x001EC618 File Offset: 0x001EA818
		private void OnPlayerJoinedRoom(NetPlayer joiningPlayer)
		{
			if (!this.IsInParty)
			{
				return;
			}
			bool flag = (int)NetworkSystem.Instance.CurrentRoom.MaxPlayers == NetworkSystem.Instance.RoomPlayerCount;
			Debug.Log(string.Concat(new string[]
			{
				"[FriendshipGroupDetection::OnPlayerJoinedRoom] JoiningPlayer: ",
				joiningPlayer.NickName,
				", ",
				joiningPlayer.UserId,
				" ",
				string.Format("| IsLocal: {0} | Room Full: {1}", joiningPlayer.IsLocal, flag)
			}));
			if (joiningPlayer.IsLocal)
			{
				this.lastJoinedRoomTime = (double)Time.time;
				if (!flag)
				{
					Debug.Log("[FriendshipGroupDetection::OnPlayerJoinedRoom] Delaying PartyRefresh...");
					this.wantsPartyRefreshPostJoin = true;
					return;
				}
			}
			if (flag)
			{
				this.RefreshPartyMembers();
			}
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x001EC6D4 File Offset: 0x001EA8D4
		public void AddGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Add(callback);
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x001EC6E2 File Offset: 0x001EA8E2
		public void RemoveGroupZoneCallback(Action<GroupJoinZoneAB> callback)
		{
			this.groupZoneCallbacks.Remove(callback);
		}

		// Token: 0x06005FB3 RID: 24499 RVA: 0x001EC6F1 File Offset: 0x001EA8F1
		public bool IsInMyGroup(string userID)
		{
			return this.myPartyMemberIDs != null && this.myPartyMemberIDs.Contains(userID);
		}

		// Token: 0x06005FB4 RID: 24500 RVA: 0x001EC70C File Offset: 0x001EA90C
		public bool AnyPartyMembersOutsideFriendCollider()
		{
			if (!this.IsInParty)
			{
				return false;
			}
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				if (rigContainer.Rig.IsLocalPartyMember && !GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(rigContainer.Creator.UserId))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x17000919 RID: 2329
		// (get) Token: 0x06005FB5 RID: 24501 RVA: 0x001EC794 File Offset: 0x001EA994
		// (set) Token: 0x06005FB6 RID: 24502 RVA: 0x001EC79C File Offset: 0x001EA99C
		public bool DidJoinLeftHanded { get; private set; }

		// Token: 0x06005FB7 RID: 24503 RVA: 0x001EC7A8 File Offset: 0x001EA9A8
		public void Tick()
		{
			using (FriendshipGroupDetection.profiler_Tick.Auto())
			{
				if (this.wantsPartyRefreshPostJoin && this.lastJoinedRoomTime + this.joinedRoomRefreshPartyDelay < (double)Time.time)
				{
					this.RefreshPartyMembers();
				}
				if (this.wantsPartyRefreshPostFollowFailed && this.lastFailedToFollowPartyTime + this.failedToFollowRefreshPartyDelay < (double)Time.time)
				{
					this.RefreshPartyMembers();
				}
				List<int> list = this.playersInProvisionalGroup;
				List<int> list2 = this.playersInProvisionalGroup;
				List<int> list3 = this.tempIntList;
				this.tempIntList = list2;
				this.playersInProvisionalGroup = list3;
				Vector3 position;
				this.UpdateProvisionalGroup(out position);
				if (this.playersInProvisionalGroup.Count > 0)
				{
					this.friendshipBubble.transform.position = position;
				}
				bool flag = false;
				if (list.Count == this.playersInProvisionalGroup.Count)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i] != this.playersInProvisionalGroup[i])
						{
							flag = true;
							break;
						}
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					this.groupCreateAfterTimestamp = Time.time + this.groupTime;
					this.amFirstProvisionalPlayer = (this.playersInProvisionalGroup.Count > 0 && this.playersInProvisionalGroup[0] == NetworkSystem.Instance.LocalPlayer.ActorNumber);
					if (this.playersInProvisionalGroup.Count > 0 && !this.amFirstProvisionalPlayer)
					{
						List<int> list4 = this.tempIntList;
						list4.Clear();
						NetPlayer netPlayer = null;
						foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
						{
							VRRig rig = rigContainer.Rig;
							if (rig.creator.ActorNumber == this.playersInProvisionalGroup[0])
							{
								netPlayer = rig.creator;
								if (rig.IsLocalPartyMember)
								{
									list4.Clear();
									break;
								}
							}
							else if (rig.IsLocalPartyMember)
							{
								list4.Add(rig.creator.ActorNumber);
							}
						}
						if (list4.Count > 0)
						{
							this.photonView.RPC("NotifyPartyMerging", netPlayer.GetPlayerRef(), new object[]
							{
								list4.ToArray()
							});
						}
						else
						{
							this.photonView.RPC("NotifyNoPartyToMerge", netPlayer.GetPlayerRef(), Array.Empty<object>());
						}
					}
					if (this.playersInProvisionalGroup.Count == 0)
					{
						if (Time.time > this.suppressPartyCreationUntilTimestamp && this.playEffectsAfterTimestamp == 0f)
						{
							this.audioSource.GTStop();
							this.audioSource.GTPlayOneShot(this.fistBumpInterruptedAudio, 1f);
						}
						this.particleSystem.Stop();
						this.playEffectsAfterTimestamp = 0f;
					}
					else
					{
						this.playEffectsAfterTimestamp = Time.time + this.playEffectsDelay;
					}
				}
				else if (this.playEffectsAfterTimestamp > 0f && Time.time > this.playEffectsAfterTimestamp)
				{
					this.audioSource.time = 0f;
					this.audioSource.GTPlay();
					this.particleSystem.Play();
					this.playEffectsAfterTimestamp = 0f;
				}
				else if (this.playersInProvisionalGroup.Count > 0 && Time.time > this.groupCreateAfterTimestamp && this.amFirstProvisionalPlayer)
				{
					List<int> list5 = this.tempIntList;
					list5.Clear();
					list5.AddRange(this.playersInProvisionalGroup);
					int num = 0;
					if (this.IsInParty)
					{
						foreach (RigContainer rigContainer2 in VRRigCache.ActiveRigContainers)
						{
							VRRig rig2 = rigContainer2.Rig;
							if (rig2.IsLocalPartyMember)
							{
								list5.Add(rig2.creator.ActorNumber);
								num++;
							}
						}
					}
					int num2 = 0;
					foreach (int key in this.playersInProvisionalGroup)
					{
						int[] collection;
						if (this.partyMergeIDs.TryGetValue(key, out collection))
						{
							list5.AddRange(collection);
							num2++;
						}
					}
					list5.Sort();
					int[] memberIDs = list5.Distinct<int>().ToArray<int>();
					this.myBraceletColor = GTColor.RandomHSV(this.braceletRandomColorHSVRanges);
					this.SendPartyFormedRPC(FriendshipGroupDetection.PackColor(this.myBraceletColor), memberIDs, false);
					this.groupCreateAfterTimestamp = Time.time + this.cooldownAfterCreatingGroup;
				}
				if (this.myPartyMemberIDs != null)
				{
					this.UpdateWarningSigns();
				}
			}
		}

		// Token: 0x06005FB8 RID: 24504 RVA: 0x001ECC84 File Offset: 0x001EAE84
		private void UpdateProvisionalGroup(out Vector3 midpoint)
		{
			using (FriendshipGroupDetection.profiler_updateProvisionalGroup.Auto())
			{
				this.playersInProvisionalGroup.Clear();
				bool willJoinLeftHanded;
				VRMap makingFist = VRRig.LocalRig.GetMakingFist(this.debug, out willJoinLeftHanded);
				if (makingFist == null || !NetworkSystem.Instance.InRoom || VRRig.LocalRig.leftHandLink.IsLinkActive() || VRRig.LocalRig.rightHandLink.IsLinkActive() || VRRigCache.ActiveRigs.Count == 0 || Time.time < this.suppressPartyCreationUntilTimestamp || (GorillaGameModes.GameMode.ActiveGameMode != null && !GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(NetworkSystem.Instance.LocalPlayer)))
				{
					midpoint = Vector3.zero;
				}
				else
				{
					this.WillJoinLeftHanded = willJoinLeftHanded;
					this.playersToPropagateFrom.Clear();
					this.provisionalGroupUsingLeftHands.Clear();
					this.playersMakingFists.Clear();
					int actorNumber = NetworkSystem.Instance.LocalPlayer.ActorNumber;
					int num = -1;
					foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
					{
						VRRig rig = rigContainer.Rig;
						bool isLeftHand;
						VRMap makingFist2 = rig.GetMakingFist(this.debug, out isLeftHand);
						if (makingFist2 != null && !rig.leftHandLink.IsLinkActive() && !rig.rightHandLink.IsLinkActive() && (!GorillaGameModes.GameMode.ActiveGameMode.IsNotNull() || GorillaGameModes.GameMode.ActiveGameMode.CanJoinFrienship(rig.OwningNetPlayer)))
						{
							FriendshipGroupDetection.PlayerFist item = new FriendshipGroupDetection.PlayerFist
							{
								actorNumber = rig.creator.ActorNumber,
								position = makingFist2.rigTarget.position,
								isLeftHand = isLeftHand
							};
							if (rig.isOfflineVRRig)
							{
								num = this.playersMakingFists.Count;
							}
							this.playersMakingFists.Add(item);
						}
					}
					if (this.playersMakingFists.Count <= 1 || num == -1)
					{
						midpoint = Vector3.zero;
					}
					else
					{
						this.playersToPropagateFrom.Enqueue(this.playersMakingFists[num]);
						this.playersInProvisionalGroup.Add(actorNumber);
						midpoint = makingFist.rigTarget.position;
						int num2 = 1 << num;
						FriendshipGroupDetection.PlayerFist playerFist;
						while (this.playersToPropagateFrom.TryDequeue(out playerFist))
						{
							for (int i = 0; i < this.playersMakingFists.Count; i++)
							{
								if ((num2 & 1 << i) == 0)
								{
									FriendshipGroupDetection.PlayerFist playerFist2 = this.playersMakingFists[i];
									if ((playerFist.position - playerFist2.position).IsShorterThan(this.detectionRadius))
									{
										int index = ~this.playersInProvisionalGroup.BinarySearch(playerFist2.actorNumber);
										num2 |= 1 << i;
										this.playersInProvisionalGroup.Insert(index, playerFist2.actorNumber);
										if (playerFist2.isLeftHand)
										{
											this.provisionalGroupUsingLeftHands.Add(playerFist2.actorNumber);
										}
										this.playersToPropagateFrom.Enqueue(playerFist2);
										midpoint += playerFist2.position;
									}
								}
							}
						}
						if (this.playersInProvisionalGroup.Count == 1)
						{
							this.playersInProvisionalGroup.Clear();
						}
						if (this.playersInProvisionalGroup.Count > 0)
						{
							midpoint /= (float)this.playersInProvisionalGroup.Count;
						}
					}
				}
			}
		}

		// Token: 0x06005FB9 RID: 24505 RVA: 0x001ED038 File Offset: 0x001EB238
		private void UpdateWarningSigns()
		{
			ZoneEntityBSP zoneEntity = GorillaTagger.Instance.offlineVRRig.zoneEntity;
			GTZone currentRoomZone = PhotonNetworkController.Instance.CurrentRoomZone;
			GroupJoinZoneAB groupJoinZoneAB = 0;
			if (this.myPartyMemberIDs != null)
			{
				foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
				{
					VRRig rig = rigContainer.Rig;
					if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
					{
						groupJoinZoneAB |= rig.zoneEntity.GroupZone;
					}
				}
			}
			if (groupJoinZoneAB != this.partyZone)
			{
				this.debugStr.Clear();
				foreach (RigContainer rigContainer2 in VRRigCache.ActiveRigContainers)
				{
					VRRig rig2 = rigContainer2.Rig;
					if (rig2.IsLocalPartyMember && !rig2.isOfflineVRRig)
					{
						this.debugStr.Append(string.Format("{0} in {1};", rig2.playerNameVisible, rig2.zoneEntity.GroupZone));
					}
				}
				this.partyZone = groupJoinZoneAB;
				foreach (Action<GroupJoinZoneAB> action in this.groupZoneCallbacks)
				{
					action(this.partyZone);
				}
			}
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x001ED1B0 File Offset: 0x001EB3B0
		[PunRPC]
		private void NotifyNoPartyToMerge(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "NotifyNoPartyToMerge");
			if (info.Sender == null || this.partyMergeIDs == null)
			{
				return;
			}
			this.partyMergeIDs.Remove(info.Sender.ActorNumber);
		}

		// Token: 0x06005FBB RID: 24507 RVA: 0x001ED1E8 File Offset: 0x001EB3E8
		[Rpc]
		private unsafe static void RPC_NotifyNoPartyToMerge(NetworkRunner runner, RpcInfo info = default(RpcInfo))
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
					if (SimulationMessage.CanAllocateUserPayload(num))
					{
						if (runner.HasAnyActiveConnections())
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr->Offset = num2 * 8;
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
						info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
						goto IL_10;
					}
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)", num);
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.partyMergeIDs.Remove(info.Source.PlayerId);
		}

		// Token: 0x06005FBC RID: 24508 RVA: 0x001ED2E9 File Offset: 0x001EB4E9
		[PunRPC]
		private void NotifyPartyMerging(int[] memberIDs, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "NotifyPartyMerging");
			if (memberIDs == null)
			{
				return;
			}
			if (memberIDs.Length > 10)
			{
				return;
			}
			this.partyMergeIDs[info.Sender.ActorNumber] = memberIDs;
		}

		// Token: 0x06005FBD RID: 24509 RVA: 0x001ED31C File Offset: 0x001EB51C
		[Rpc]
		private unsafe static void RPC_NotifyPartyMerging(NetworkRunner runner, [RpcTarget] PlayerRef playerRef, int[] memberIDs, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(playerRef);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(playerRef, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)"));
							int num2 = 8;
							*(int*)(ptr2 + num2) = memberIDs.Length;
							num2 += 4;
							num2 = (Native.CopyFromArray<int>((void*)(ptr2 + num2), memberIDs) + 3 & -4) + num2;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(playerRef);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			if (memberIDs.Length > 10)
			{
				return;
			}
			FriendshipGroupDetection.Instance.partyMergeIDs[info.Source.PlayerId] = memberIDs;
		}

		// Token: 0x06005FBE RID: 24510 RVA: 0x001ED4A8 File Offset: 0x001EB6A8
		public void SendAboutToGroupJoin()
		{
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				Debug.Log(string.Concat(new string[]
				{
					"Sending group join to ",
					VRRigCache.ActiveRigContainers.Count.ToString(),
					" players. Party member:",
					rig.OwningNetPlayer.NickName,
					"Is offline rig",
					rig.isOfflineVRRig.ToString()
				}));
				if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
				{
					this.photonView.RPC("PartyMemberIsAboutToGroupJoin", rig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x001ED584 File Offset: 0x001EB784
		[PunRPC]
		private void PartyMemberIsAboutToGroupJoin(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PartyMemberIsAboutToGroupJoin");
			this.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x001ED5A0 File Offset: 0x001EB7A0
		[Rpc]
		private unsafe static void RPC_PartyMemberIsAboutToGroupJoin(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(targetPlayer);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.PartMemberIsAboutToGroupJoinWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x001ED6C0 File Offset: 0x001EB8C0
		private void PartMemberIsAboutToGroupJoinWrapped(PhotonMessageInfoWrapped wrappedInfo)
		{
			float time = Time.time;
			float num = this.aboutToGroupJoin_CooldownUntilTimestamp;
			if (wrappedInfo.senderID < NetworkSystem.Instance.LocalPlayer.ActorNumber)
			{
				this.aboutToGroupJoin_CooldownUntilTimestamp = Time.time + 5f;
				if (this.myPartyMembersHash.Contains(wrappedInfo.Sender.UserId))
				{
					PhotonNetworkController.Instance.DeferJoining(2f);
				}
			}
		}

		// Token: 0x06005FC2 RID: 24514 RVA: 0x001ED72C File Offset: 0x001EB92C
		private void SendPartyFormedRPC(short braceletColor, int[] memberIDs, bool forceDebug)
		{
			string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
			foreach (VRRig vrrig in VRRigCache.ActiveRigs)
			{
				if (this.playersInProvisionalGroup.BinarySearch(vrrig.creator.ActorNumber) >= 0)
				{
					this.photonView.RPC("PartyFormedSuccessfully", vrrig.Creator.GetPlayerRef(), new object[]
					{
						text,
						braceletColor,
						memberIDs,
						forceDebug
					});
				}
			}
		}

		// Token: 0x06005FC3 RID: 24515 RVA: 0x001ED7EC File Offset: 0x001EB9EC
		[Rpc]
		private unsafe static void RPC_PartyFormedSuccessfully(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3 & -4);
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						num += 4;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), partyGameMode) + 3 & -4) + num2;
							*(short*)(ptr2 + num2) = braceletColor;
							num2 += (2 + 3 & -4);
							*(int*)(ptr2 + num2) = memberIDs.Length;
							num2 += 4;
							num2 = (Native.CopyFromArray<int>((void*)(ptr2 + num2), memberIDs) + 3 & -4) + num2;
							ReadWriteUtilsForWeaver.WriteBoolean((int*)(ptr2 + num2), forceDebug);
							num2 += 4;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(targetPlayer);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			MonkeAgent.IncrementRPCCall(info, "PartyFormedSuccessfully");
			FriendshipGroupDetection.Instance.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FC4 RID: 24516 RVA: 0x001EDA09 File Offset: 0x001EBC09
		[PunRPC]
		private void PartyFormedSuccessfully(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PartyFormedSuccessfully");
			this.PartyFormedSuccesfullyWrapped(partyGameMode, braceletColor, memberIDs, forceDebug, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x001EDA2C File Offset: 0x001EBC2C
		private void PartyFormedSuccesfullyWrapped(string partyGameMode, short braceletColor, int[] memberIDs, bool forceDebug, PhotonMessageInfoWrapped info)
		{
			if (memberIDs == null || memberIDs.Length > 10 || !memberIDs.Contains(info.Sender.ActorNumber) || this.playersInProvisionalGroup.IndexOf(info.Sender.ActorNumber) != 0 || Mathf.Abs(this.groupCreateAfterTimestamp - Time.time) > this.m_maxGroupJoinTimeDifference || !GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			if (this.IsInParty)
			{
				string text = Enum.Parse<GameModeType>(GorillaComputer.instance.currentGameMode.Value, true).ToString();
				foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
				{
					VRRig rig = rigContainer.Rig;
					if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
					{
						this.photonView.RPC("AddPartyMembers", rig.Creator.GetPlayerRef(), new object[]
						{
							text,
							braceletColor,
							memberIDs
						});
					}
				}
			}
			this.suppressPartyCreationUntilTimestamp = Time.time + this.cooldownAfterCreatingGroup;
			this.DidJoinLeftHanded = this.WillJoinLeftHanded;
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x001EDB6C File Offset: 0x001EBD6C
		[PunRPC]
		private void AddPartyMembers(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfo info)
		{
			this.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FC7 RID: 24519 RVA: 0x001EDB80 File Offset: 0x001EBD80
		[Rpc]
		private unsafe static void RPC_AddPartyMembers(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, string partyGameMode, short braceletColor, int[] memberIDs, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(partyGameMode) + 3 & -4);
						num += 4;
						num += (memberIDs.Length * 4 + 4 + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), partyGameMode) + 3 & -4) + num2;
							*(short*)(ptr2 + num2) = braceletColor;
							num2 += (2 + 3 & -4);
							*(int*)(ptr2 + num2) = memberIDs.Length;
							num2 += 4;
							num2 = (Native.CopyFromArray<int>((void*)(ptr2 + num2), memberIDs) + 3 & -4) + num2;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(rpcTarget);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.AddPartyMembersWrapped(partyGameMode, braceletColor, memberIDs, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FC8 RID: 24520 RVA: 0x001EDD64 File Offset: 0x001EBF64
		private void AddPartyMembersWrapped(string partyGameMode, short braceletColor, int[] memberIDs, PhotonMessageInfoWrapped infoWrapped)
		{
			MonkeAgent.IncrementRPCCall(infoWrapped, "AddPartyMembersWrapped");
			if (!this.IsInParty || memberIDs == null || memberIDs.Length > 10 || !this.myPartyMembersHash.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)) || !GorillaGameModes.GameMode.IsValidGameMode(partyGameMode))
			{
				return;
			}
			this.SetNewParty(partyGameMode, braceletColor, memberIDs);
		}

		// Token: 0x06005FC9 RID: 24521 RVA: 0x001EDDC0 File Offset: 0x001EBFC0
		private void SetNewParty(string partyGameMode, short braceletColor, int[] memberIDs)
		{
			GorillaComputer.instance.SetGameModeWithoutButton(partyGameMode);
			this.myPartyMemberIDs = new List<string>();
			FriendshipGroupDetection.userIdLookup.Clear();
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				FriendshipGroupDetection.userIdLookup.Add(rigContainer.Creator.ActorNumber, rigContainer.Creator.UserId);
			}
			foreach (int key in memberIDs)
			{
				string item;
				if (FriendshipGroupDetection.userIdLookup.TryGetValue(key, out item))
				{
					this.myPartyMemberIDs.Add(item);
				}
			}
			this.myBraceletColor = FriendshipGroupDetection.UnpackColor(braceletColor);
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
			this.OnPartyMembershipChanged();
			PlayerGameEvents.MiscEvent("FriendshipGroupJoined", 1);
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x001EDEB4 File Offset: 0x001EC0B4
		public void LeaveParty()
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember && !rig.isOfflineVRRig)
				{
					this.photonView.RPC("PlayerLeftParty", rig.Creator.GetPlayerRef(), Array.Empty<object>());
				}
			}
			this.myPartyMemberIDs = null;
			this.OnPartyMembershipChanged();
			PhotonNetworkController.Instance.ClearDeferredJoin();
			GorillaTagger.Instance.StartVibration(false, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005FCB RID: 24523 RVA: 0x001EDF68 File Offset: 0x001EC168
		public void OnFailedToFollowParty()
		{
			if (!this.IsInParty)
			{
				return;
			}
			this.lastFailedToFollowPartyTime = (double)Time.time;
			this.wantsPartyRefreshPostFollowFailed = true;
		}

		// Token: 0x06005FCC RID: 24524 RVA: 0x001EDF88 File Offset: 0x001EC188
		public void RefreshPartyMembers()
		{
			if (this.myPartyMemberIDs.IsNullOrEmpty<string>())
			{
				return;
			}
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] refreshing...");
			List<string> list = new List<string>(this.myPartyMemberIDs);
			Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] found " + string.Format("{0} current players in Room...", NetworkSystem.Instance.AllNetPlayers.Length));
			for (int i = 0; i < NetworkSystem.Instance.AllNetPlayers.Length; i++)
			{
				if (NetworkSystem.Instance.AllNetPlayers[i] != null)
				{
					list.Remove(NetworkSystem.Instance.AllNetPlayers[i].UserId);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				Debug.Log("[FriendshipGroupDetection::RefreshPartyMembers] removing missing player " + list[j] + " from party...");
				this.PlayerIDLeftParty(list[j]);
			}
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x001EE068 File Offset: 0x001EC268
		[Rpc]
		private unsafe static void RPC_PlayerLeftParty(NetworkRunner runner, [RpcTarget] PlayerRef player, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(player);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(player, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(player);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			MonkeAgent.IncrementRPCCall(info, "PlayerLeftParty");
			FriendshipGroupDetection.Instance.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FCE RID: 24526 RVA: 0x001EE198 File Offset: 0x001EC398
		[PunRPC]
		private void PlayerLeftParty(PhotonMessageInfo info)
		{
			MonkeAgent.IncrementRPCCall(info, "PlayerLeftParty");
			this.PlayerLeftPartyWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FCF RID: 24527 RVA: 0x001EE1B4 File Offset: 0x001EC3B4
		private void PlayerLeftPartyWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(infoWrapped.Sender.UserId))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005FD0 RID: 24528 RVA: 0x001EE21C File Offset: 0x001EC41C
		private void PlayerIDLeftParty(string userID)
		{
			if (this.myPartyMemberIDs == null)
			{
				return;
			}
			if (!this.myPartyMemberIDs.Remove(userID))
			{
				return;
			}
			if (this.myPartyMemberIDs.Count <= 1)
			{
				this.myPartyMemberIDs = null;
			}
			this.OnPartyMembershipChanged();
			GorillaTagger.Instance.StartVibration(this.DidJoinLeftHanded, this.hapticStrength, this.hapticDuration);
		}

		// Token: 0x06005FD1 RID: 24529 RVA: 0x001EE278 File Offset: 0x001EC478
		public void SendVerifyPartyMember(NetPlayer player)
		{
			this.photonView.RPC("VerifyPartyMember", player.GetPlayerRef(), Array.Empty<object>());
		}

		// Token: 0x06005FD2 RID: 24530 RVA: 0x001EE295 File Offset: 0x001EC495
		[PunRPC]
		private void VerifyPartyMember(PhotonMessageInfo info)
		{
			this.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FD3 RID: 24531 RVA: 0x001EE2A4 File Offset: 0x001EC4A4
		[Rpc]
		private unsafe static void RPC_VerifyPartyMember(NetworkRunner runner, [RpcTarget] PlayerRef rpcTarget, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(rpcTarget);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(rpcTarget, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)"));
							int num2 = 8;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(rpcTarget);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.VerifyPartyMemberWrapped(new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FD4 RID: 24532 RVA: 0x001EE3C4 File Offset: 0x001EC5C4
		private void VerifyPartyMemberWrapped(PhotonMessageInfoWrapped infoWrapped)
		{
			MonkeAgent.IncrementRPCCall(infoWrapped, "VerifyPartyMemberWrapped");
			RigContainer rigContainer;
			if (!VRRigCache.Instance.TryGetVrrig(infoWrapped.Sender, out rigContainer) || !FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 15, infoWrapped.SentServerTime))
			{
				return;
			}
			if (this.myPartyMemberIDs == null || !this.myPartyMemberIDs.Contains(NetworkSystem.Instance.GetUserID(infoWrapped.senderID)))
			{
				this.photonView.RPC("PlayerLeftParty", infoWrapped.Sender.GetPlayerRef(), Array.Empty<object>());
			}
		}

		// Token: 0x06005FD5 RID: 24533 RVA: 0x001EE454 File Offset: 0x001EC654
		public void SendRequestPartyGameMode(string gameMode)
		{
			int num = int.MaxValue;
			NetPlayer netPlayer = null;
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember && rig.creator.ActorNumber < num)
				{
					netPlayer = rig.creator;
					num = rig.creator.ActorNumber;
				}
			}
			if (netPlayer != null)
			{
				this.photonView.RPC("RequestPartyGameMode", netPlayer.GetPlayerRef(), new object[]
				{
					gameMode
				});
			}
		}

		// Token: 0x06005FD6 RID: 24534 RVA: 0x001EE4F4 File Offset: 0x001EC6F4
		[Rpc]
		private unsafe static void RPC_RequestPartyGameMode(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), gameMode) + 3 & -4) + num2;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(targetPlayer);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FD7 RID: 24535 RVA: 0x001EE652 File Offset: 0x001EC852
		[PunRPC]
		private void RequestPartyGameMode(string gameMode, PhotonMessageInfo info)
		{
			this.RequestPartyGameModeWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FD8 RID: 24536 RVA: 0x001EE664 File Offset: 0x001EC864
		private void RequestPartyGameModeWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "RequestPartyGameModeWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember)
				{
					this.photonView.RPC("NotifyPartyGameModeChanged", rig.creator.GetPlayerRef(), new object[]
					{
						gameMode
					});
				}
			}
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x001EE70C File Offset: 0x001EC90C
		[Rpc]
		private unsafe static void RPC_NotifyPartyGameModeChanged(NetworkRunner runner, [RpcTarget] PlayerRef targetPlayer, string gameMode, RpcInfo info = default(RpcInfo))
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
					RpcTargetStatus rpcTargetStatus = runner.GetRpcTargetStatus(targetPlayer);
					if (rpcTargetStatus == RpcTargetStatus.Unreachable)
					{
						NetworkBehaviourUtils.NotifyRpcTargetUnreachable(targetPlayer, "System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)");
					}
					else
					{
						if (rpcTargetStatus == RpcTargetStatus.Self)
						{
							info = RpcInfo.FromLocal(runner, RpcChannel.Reliable, RpcHostMode.SourceIsServer);
							goto IL_10;
						}
						int num = 8;
						num += (ReadWriteUtilsForWeaver.GetByteCountUtf8NoHash(gameMode) + 3 & -4);
						if (!SimulationMessage.CanAllocateUserPayload(num))
						{
							NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)", num);
						}
						else
						{
							SimulationMessage* ptr = SimulationMessage.Allocate(runner.Simulation, num);
							byte* ptr2 = (byte*)(ptr + 28 / sizeof(SimulationMessage));
							*(RpcHeader*)ptr2 = RpcHeader.Create(NetworkBehaviourUtils.GetRpcStaticIndexOrThrow("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)"));
							int num2 = 8;
							num2 = (ReadWriteUtilsForWeaver.WriteStringUtf8NoHash((void*)(ptr2 + num2), gameMode) + 3 & -4) + num2;
							ptr->Offset = num2 * 8;
							ptr->SetTarget(targetPlayer);
							ptr->SetStatic();
							runner.SendRpc(ptr);
						}
					}
				}
				return;
			}
			IL_10:
			FriendshipGroupDetection.Instance.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FDA RID: 24538 RVA: 0x001EE86A File Offset: 0x001ECA6A
		[PunRPC]
		private void NotifyPartyGameModeChanged(string gameMode, PhotonMessageInfo info)
		{
			this.NotifyPartyGameModeChangedWrapped(gameMode, new PhotonMessageInfoWrapped(info));
		}

		// Token: 0x06005FDB RID: 24539 RVA: 0x001EE879 File Offset: 0x001ECA79
		private void NotifyPartyGameModeChangedWrapped(string gameMode, PhotonMessageInfoWrapped info)
		{
			MonkeAgent.IncrementRPCCall(info, "NotifyPartyGameModeChangedWrapped");
			if (!this.IsInParty || !this.IsInMyGroup(info.Sender.UserId) || !GorillaGameModes.GameMode.IsValidGameMode(gameMode))
			{
				return;
			}
			GorillaComputer.instance.SetGameModeWithoutButton(gameMode);
		}

		// Token: 0x06005FDC RID: 24540 RVA: 0x001EE8B8 File Offset: 0x001ECAB8
		private void OnPartyMembershipChanged()
		{
			this.myPartyMembersHash.Clear();
			if (this.myPartyMemberIDs != null)
			{
				foreach (string item in this.myPartyMemberIDs)
				{
					this.myPartyMembersHash.Add(item);
				}
			}
			this.myBeadColors.Clear();
			FriendshipGroupDetection.tempColorLookup.Clear();
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				rig.ClearPartyMemberStatus();
				if (rig.IsLocalPartyMember)
				{
					FriendshipGroupDetection.tempColorLookup.Add(rig.Creator.UserId, rig.playerColor);
				}
			}
			this.MyBraceletSelfIndex = 0;
			if (this.myPartyMemberIDs != null)
			{
				using (List<string>.Enumerator enumerator = this.myPartyMemberIDs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string text = enumerator.Current;
						Color item2;
						if (FriendshipGroupDetection.tempColorLookup.TryGetValue(text, out item2))
						{
							if (text == PhotonNetwork.LocalPlayer.UserId)
							{
								this.MyBraceletSelfIndex = this.myBeadColors.Count;
							}
							this.myBeadColors.Add(item2);
						}
					}
					goto IL_160;
				}
			}
			GorillaComputer.instance.SetGameModeWithoutButton(GorillaComputer.instance.lastPressedGameMode);
			this.wantsPartyRefreshPostJoin = false;
			this.wantsPartyRefreshPostFollowFailed = false;
			IL_160:
			this.myBeadColors.Add(this.myBraceletColor);
			GorillaTagger.Instance.offlineVRRig.UpdateFriendshipBracelet();
			this.UpdateWarningSigns();
		}

		// Token: 0x06005FDD RID: 24541 RVA: 0x001EEA74 File Offset: 0x001ECC74
		public bool IsPartyWithinCollider(GorillaFriendCollider friendCollider)
		{
			foreach (RigContainer rigContainer in VRRigCache.ActiveRigContainers)
			{
				VRRig rig = rigContainer.Rig;
				if (rig.IsLocalPartyMember && !rig.isOfflineVRRig && !friendCollider.playerIDsCurrentlyTouching.Contains(rig.Creator.UserId))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06005FDE RID: 24542 RVA: 0x001EEAF0 File Offset: 0x001ECCF0
		public static short PackColor(Color col)
		{
			return (short)(Mathf.RoundToInt(col.r * 9f) + Mathf.RoundToInt(col.g * 9f) * 10 + Mathf.RoundToInt(col.b * 9f) * 100);
		}

		// Token: 0x06005FDF RID: 24543 RVA: 0x001EEB30 File Offset: 0x001ECD30
		public static Color UnpackColor(short data)
		{
			return new Color
			{
				r = (float)(data % 10) / 9f,
				g = (float)(data / 10 % 10) / 9f,
				b = (float)(data / 100 % 10) / 9f
			};
		}

		// Token: 0x06005FE2 RID: 24546 RVA: 0x001EECA8 File Offset: 0x001ECEA8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyNoPartyToMerge(Fusion.NetworkRunner,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyNoPartyToMerge@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyNoPartyToMerge(runner, info);
		}

		// Token: 0x06005FE3 RID: 24547 RVA: 0x001EECEC File Offset: 0x001ECEEC
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyMerging(Fusion.NetworkRunner,Fusion.PlayerRef,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyMerging@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message->Target;
			int[] array = new int[*(int*)(ptr + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(ptr + num)) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyMerging(runner, target, array, info);
		}

		// Token: 0x06005FE4 RID: 24548 RVA: 0x001EED80 File Offset: 0x001ECF80
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyMemberIsAboutToGroupJoin(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyMemberIsAboutToGroupJoin@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			PlayerRef target = message->Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyMemberIsAboutToGroupJoin(runner, target, info);
		}

		// Token: 0x06005FE5 RID: 24549 RVA: 0x001EEDD0 File Offset: 0x001ECFD0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PartyFormedSuccessfully(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],System.Boolean,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PartyFormedSuccessfully@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message->Target;
			string partyGameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), out partyGameMode) + 3 & -4) + num;
			short num2 = *(short*)(ptr + num);
			num += (2 + 3 & -4);
			short braceletColor = num2;
			int[] array = new int[*(int*)(ptr + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(ptr + num)) + 3 & -4) + num;
			bool flag = ReadWriteUtilsForWeaver.ReadBoolean((int*)(ptr + num));
			num += 4;
			bool forceDebug = flag;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PartyFormedSuccessfully(runner, target, partyGameMode, braceletColor, array, forceDebug, info);
		}

		// Token: 0x06005FE6 RID: 24550 RVA: 0x001EEED0 File Offset: 0x001ED0D0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_AddPartyMembers(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,System.Int16,System.Int32[],Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_AddPartyMembers@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message->Target;
			string partyGameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), out partyGameMode) + 3 & -4) + num;
			short num2 = *(short*)(ptr + num);
			num += (2 + 3 & -4);
			short braceletColor = num2;
			int[] array = new int[*(int*)(ptr + num)];
			num += 4;
			num = (Native.CopyToArray<int>(array, (void*)(ptr + num)) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_AddPartyMembers(runner, target, partyGameMode, braceletColor, array, info);
		}

		// Token: 0x06005FE7 RID: 24551 RVA: 0x001EEFB0 File Offset: 0x001ED1B0
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_PlayerLeftParty(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_PlayerLeftParty@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			PlayerRef target = message->Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_PlayerLeftParty(runner, target, info);
		}

		// Token: 0x06005FE8 RID: 24552 RVA: 0x001EF000 File Offset: 0x001ED200
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_VerifyPartyMember(Fusion.NetworkRunner,Fusion.PlayerRef,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_VerifyPartyMember@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			PlayerRef target = message->Target;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_VerifyPartyMember(runner, target, info);
		}

		// Token: 0x06005FE9 RID: 24553 RVA: 0x001EF050 File Offset: 0x001ED250
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_RequestPartyGameMode(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_RequestPartyGameMode@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message->Target;
			string gameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), out gameMode) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_RequestPartyGameMode(runner, target, gameMode, info);
		}

		// Token: 0x06005FEA RID: 24554 RVA: 0x001EF0C8 File Offset: 0x001ED2C8
		[NetworkRpcStaticWeavedInvoker("System.Void GorillaTagScripts.FriendshipGroupDetection::RPC_NotifyPartyGameModeChanged(Fusion.NetworkRunner,Fusion.PlayerRef,System.String,Fusion.RpcInfo)")]
		[Preserve]
		[WeaverGenerated]
		protected unsafe static void RPC_NotifyPartyGameModeChanged@Invoker(NetworkRunner runner, SimulationMessage* message)
		{
			byte* ptr = (byte*)(message + 28 / sizeof(SimulationMessage));
			int num = 8;
			PlayerRef target = message->Target;
			string gameMode;
			num = (ReadWriteUtilsForWeaver.ReadStringUtf8NoHash((void*)(ptr + num), out gameMode) + 3 & -4) + num;
			RpcInfo info = RpcInfo.FromMessage(runner, message, RpcHostMode.SourceIsServer);
			NetworkBehaviourUtils.InvokeRpc = true;
			FriendshipGroupDetection.RPC_NotifyPartyGameModeChanged(runner, target, gameMode, info);
		}

		// Token: 0x04006E53 RID: 28243
		[SerializeField]
		private float detectionRadius = 0.5f;

		// Token: 0x04006E54 RID: 28244
		[SerializeField]
		private float groupTime = 5f;

		// Token: 0x04006E55 RID: 28245
		[SerializeField]
		private float cooldownAfterCreatingGroup = 5f;

		// Token: 0x04006E56 RID: 28246
		[SerializeField]
		private float hapticStrength = 1.5f;

		// Token: 0x04006E57 RID: 28247
		[SerializeField]
		private float hapticDuration = 2f;

		// Token: 0x04006E58 RID: 28248
		[SerializeField]
		private double joinedRoomRefreshPartyDelay = 30.0;

		// Token: 0x04006E59 RID: 28249
		[SerializeField]
		private double failedToFollowRefreshPartyDelay = 30.0;

		// Token: 0x04006E5A RID: 28250
		public bool debug;

		// Token: 0x04006E5B RID: 28251
		public double offset = 0.5;

		// Token: 0x04006E5C RID: 28252
		[SerializeField]
		private float m_maxGroupJoinTimeDifference = 1f;

		// Token: 0x04006E5D RID: 28253
		private List<string> myPartyMemberIDs;

		// Token: 0x04006E5E RID: 28254
		private HashSet<string> myPartyMembersHash = new HashSet<string>();

		// Token: 0x04006E63 RID: 28259
		private List<Action<GroupJoinZoneAB>> groupZoneCallbacks = new List<Action<GroupJoinZoneAB>>();

		// Token: 0x04006E64 RID: 28260
		[SerializeField]
		private GTColor.HSVRanges braceletRandomColorHSVRanges;

		// Token: 0x04006E65 RID: 28261
		public GameObject friendshipBubble;

		// Token: 0x04006E66 RID: 28262
		public AudioClip fistBumpInterruptedAudio;

		// Token: 0x04006E67 RID: 28263
		private ParticleSystem particleSystem;

		// Token: 0x04006E68 RID: 28264
		private AudioSource audioSource;

		// Token: 0x04006E69 RID: 28265
		private double lastJoinedRoomTime;

		// Token: 0x04006E6A RID: 28266
		private bool wantsPartyRefreshPostJoin;

		// Token: 0x04006E6B RID: 28267
		private double lastFailedToFollowPartyTime;

		// Token: 0x04006E6C RID: 28268
		private bool wantsPartyRefreshPostFollowFailed;

		// Token: 0x04006E6E RID: 28270
		private Queue<FriendshipGroupDetection.PlayerFist> playersToPropagateFrom = new Queue<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x04006E6F RID: 28271
		private List<int> playersInProvisionalGroup = new List<int>();

		// Token: 0x04006E70 RID: 28272
		private List<int> provisionalGroupUsingLeftHands = new List<int>();

		// Token: 0x04006E71 RID: 28273
		private List<int> tempIntList = new List<int>();

		// Token: 0x04006E72 RID: 28274
		private bool amFirstProvisionalPlayer;

		// Token: 0x04006E73 RID: 28275
		private Dictionary<int, int[]> partyMergeIDs = new Dictionary<int, int[]>();

		// Token: 0x04006E74 RID: 28276
		private float groupCreateAfterTimestamp;

		// Token: 0x04006E75 RID: 28277
		private float playEffectsAfterTimestamp;

		// Token: 0x04006E76 RID: 28278
		[SerializeField]
		private float playEffectsDelay;

		// Token: 0x04006E77 RID: 28279
		private float suppressPartyCreationUntilTimestamp;

		// Token: 0x04006E79 RID: 28281
		private bool WillJoinLeftHanded;

		// Token: 0x04006E7A RID: 28282
		private static readonly ProfilerMarker profiler_Tick = new ProfilerMarker("GT/FriendshipGroupDetection.Tick");

		// Token: 0x04006E7B RID: 28283
		private List<FriendshipGroupDetection.PlayerFist> playersMakingFists = new List<FriendshipGroupDetection.PlayerFist>();

		// Token: 0x04006E7C RID: 28284
		private static readonly ProfilerMarker profiler_updateProvisionalGroup = new ProfilerMarker("GT/FriendshipGroupDetection.UpdateProvisionalGroup");

		// Token: 0x04006E7D RID: 28285
		private StringBuilder debugStr = new StringBuilder();

		// Token: 0x04006E7E RID: 28286
		private float aboutToGroupJoin_CooldownUntilTimestamp;

		// Token: 0x04006E7F RID: 28287
		private static Dictionary<int, string> userIdLookup = new Dictionary<int, string>();

		// Token: 0x04006E80 RID: 28288
		private static Dictionary<string, Color> tempColorLookup = new Dictionary<string, Color>();

		// Token: 0x02000F06 RID: 3846
		private struct PlayerFist
		{
			// Token: 0x04006E81 RID: 28289
			public int actorNumber;

			// Token: 0x04006E82 RID: 28290
			public Vector3 position;

			// Token: 0x04006E83 RID: 28291
			public bool isLeftHand;
		}
	}
}
