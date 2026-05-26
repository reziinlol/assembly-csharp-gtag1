using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;

namespace GorillaTag
{
	// Token: 0x0200115E RID: 4446
	public static class MonkeAgentCleanup
	{
		// Token: 0x06007087 RID: 28807 RVA: 0x0024AEFC File Offset: 0x002490FC
		static MonkeAgentCleanup()
		{
			MonkeAgentCleanup.k_destroyTimer.callback = new Action(MonkeAgentCleanup.CheckDestroyQueue);
			RoomSystem.LeftRoomEvent += new Action(MonkeAgentCleanup.OnLeftRoom);
		}

		// Token: 0x06007088 RID: 28808 RVA: 0x0024AF84 File Offset: 0x00249184
		public static void RegisterForDestroy(PhotonView target)
		{
			if (MonkeAgentCleanup.k_destroyTargets.Contains(target))
			{
				return;
			}
			if (target.gameObject.activeSelf)
			{
				target.gameObject.Disable();
			}
			if (MonkeAgentCleanup.k_destroyTargets.Add(target))
			{
				MonkeAgentCleanup.k_destroyQueue.Enqueue(target);
			}
			if (!MonkeAgentCleanup.k_destroyTimer.Running && MonkeAgentCleanup.k_destroyQueue.Count > 0)
			{
				MonkeAgentCleanup.k_destroyTimer.Start();
			}
		}

		// Token: 0x06007089 RID: 28809 RVA: 0x0024AFF2 File Offset: 0x002491F2
		private static void OnLeftRoom()
		{
			MonkeAgentCleanup.k_destroyQueue.Clear();
			MonkeAgentCleanup.k_destroyTargets.Clear();
			MonkeAgentCleanup.k_destroyTimer.Stop();
		}

		// Token: 0x0600708A RID: 28810 RVA: 0x0024B014 File Offset: 0x00249214
		private static void CheckDestroyQueue()
		{
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			bool flag = RoomSystem.GetLowestActorNumberPlayer() == NetworkSystem.Instance.LocalPlayer;
			int num = 0;
			while (MonkeAgentCleanup.k_destroyQueue.Count > 0 && num < 10)
			{
				PhotonView photonView = MonkeAgentCleanup.k_destroyQueue.Dequeue();
				if (MonkeAgentCleanup.k_destroyTargets.Remove(photonView) && !photonView.IsNull())
				{
					if ((photonView.IsRoomView && flag) || photonView.IsMine)
					{
						MonkeAgentCleanup.k_cacheInfo[MonkeAgentCleanup.k_viewIdKey] = photonView.InstantiationId;
						PhotonNetwork.NetworkingClient.OpRaiseEvent(202, MonkeAgentCleanup.k_cacheInfo, MonkeAgentCleanup.k_raiseEventOptions, SendOptions.SendReliable);
					}
					PhotonNetwork.RemoveInstantiatedGO(photonView.gameObject, true);
					num++;
				}
			}
			if (MonkeAgentCleanup.k_destroyTargets.Count == 0)
			{
				MonkeAgentCleanup.k_destroyTimer.Stop();
			}
		}

		// Token: 0x0400806A RID: 32874
		private static readonly Queue<PhotonView> k_destroyQueue = new Queue<PhotonView>();

		// Token: 0x0400806B RID: 32875
		private static readonly HashSet<PhotonView> k_destroyTargets = new HashSet<PhotonView>();

		// Token: 0x0400806C RID: 32876
		private static readonly TickSystemTimer k_destroyTimer = new TickSystemTimer(1f);

		// Token: 0x0400806D RID: 32877
		private static readonly Hashtable k_cacheInfo = new Hashtable(1);

		// Token: 0x0400806E RID: 32878
		private static readonly RaiseEventOptions k_raiseEventOptions = new RaiseEventOptions
		{
			CachingOption = EventCaching.RemoveFromRoomCache
		};

		// Token: 0x0400806F RID: 32879
		private static readonly object k_viewIdKey = 7;
	}
}
