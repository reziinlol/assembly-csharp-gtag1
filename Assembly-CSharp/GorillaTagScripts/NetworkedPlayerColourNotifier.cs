using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F09 RID: 3849
	public static class NetworkedPlayerColourNotifier
	{
		// Token: 0x06005FF1 RID: 24561 RVA: 0x001EF2BB File Offset: 0x001ED4BB
		static NetworkedPlayerColourNotifier()
		{
			RoomSystem.PlayerJoinedEvent += new Action<NetPlayer>(NetworkedPlayerColourNotifier.OnPlayerJoinedRoom);
			RoomSystem.JoinedRoomEvent += new Action(NetworkedPlayerColourNotifier.OnJoinedRoom);
		}

		// Token: 0x06005FF2 RID: 24562 RVA: 0x001EF2F3 File Offset: 0x001ED4F3
		public static void SetLocalRigReference(RigContainer rig)
		{
			NetworkedPlayerColourNotifier.m_localRigContainer = rig;
			NetworkedPlayerColourNotifier.m_localRig = rig.Rig;
			NetworkedPlayerColourNotifier.m_localRig.OnColorChanged += NetworkedPlayerColourNotifier.OnLocalColourChanged;
			NetworkedPlayerColourNotifier.m_netColourDirty = false;
		}

		// Token: 0x06005FF3 RID: 24563 RVA: 0x001EF324 File Offset: 0x001ED524
		public static void NotifyOthers()
		{
			if (!RoomSystem.JoinedRoom || NetworkedPlayerColourNotifier.m_localRigContainer.netView.IsNull())
			{
				return;
			}
			Color playerColor = NetworkedPlayerColourNotifier.m_localRig.playerColor;
			float r = playerColor.r;
			float g = playerColor.g;
			float b = playerColor.b;
			NetworkedPlayerColourNotifier.m_localRigContainer.netView.SendRPC("RPC_InitializeNoobMaterial", RpcTarget.Others, new object[]
			{
				r,
				g,
				b
			});
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x001EF39E File Offset: 0x001ED59E
		private static void OnLocalColourChanged(Color color)
		{
			if (!RoomSystem.JoinedRoom)
			{
				return;
			}
			NetworkedPlayerColourNotifier.m_netColourDirty = (NetworkedPlayerColourNotifier.m_initialNetColour != color);
		}

		// Token: 0x06005FF5 RID: 24565 RVA: 0x001EF3B8 File Offset: 0x001ED5B8
		private static void OnPlayerJoinedRoom(NetPlayer player)
		{
			if (NetworkedPlayerColourNotifier.m_netColourDirty && NetworkedPlayerColourNotifier.m_localRigContainer.netView.IsNotNull())
			{
				Color playerColor = NetworkedPlayerColourNotifier.m_localRig.playerColor;
				float r = playerColor.r;
				float g = playerColor.g;
				float b = playerColor.b;
				NetworkedPlayerColourNotifier.m_localRigContainer.netView.SendRPC("RPC_InitializeNoobMaterial", player, new object[]
				{
					r,
					g,
					b
				});
			}
		}

		// Token: 0x06005FF6 RID: 24566 RVA: 0x001EF431 File Offset: 0x001ED631
		private static void OnJoinedRoom()
		{
			NetworkedPlayerColourNotifier.m_initialNetColour = NetworkedPlayerColourNotifier.m_localRig.playerColor;
			NetworkedPlayerColourNotifier.m_netColourDirty = false;
		}

		// Token: 0x04006E8B RID: 28299
		private static RigContainer m_localRigContainer;

		// Token: 0x04006E8C RID: 28300
		private static VRRig m_localRig;

		// Token: 0x04006E8D RID: 28301
		private static Color m_initialNetColour;

		// Token: 0x04006E8E RID: 28302
		private static bool m_netColourDirty;
	}
}
