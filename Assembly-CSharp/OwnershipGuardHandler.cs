using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

// Token: 0x02000C8A RID: 3210
internal class OwnershipGuardHandler : IPunOwnershipCallbacks
{
	// Token: 0x06004FAB RID: 20395 RVA: 0x001A6032 File Offset: 0x001A4232
	static OwnershipGuardHandler()
	{
		PhotonNetwork.AddCallbackTarget(OwnershipGuardHandler.callbackInstance);
	}

	// Token: 0x06004FAC RID: 20396 RVA: 0x001A6052 File Offset: 0x001A4252
	internal static void RegisterView(PhotonView view)
	{
		if (view == null || OwnershipGuardHandler.guardedViews.Contains(view))
		{
			return;
		}
		OwnershipGuardHandler.guardedViews.Add(view);
	}

	// Token: 0x06004FAD RID: 20397 RVA: 0x001A6078 File Offset: 0x001A4278
	internal static void RegisterViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGuardHandler.RegisterView(photonViews[i]);
		}
	}

	// Token: 0x06004FAE RID: 20398 RVA: 0x001A609D File Offset: 0x001A429D
	internal static void RemoveView(PhotonView view)
	{
		if (view == null)
		{
			return;
		}
		OwnershipGuardHandler.guardedViews.Remove(view);
	}

	// Token: 0x06004FAF RID: 20399 RVA: 0x001A60B8 File Offset: 0x001A42B8
	internal static void RemoveViews(PhotonView[] photonViews)
	{
		for (int i = 0; i < photonViews.Length; i++)
		{
			OwnershipGuardHandler.RemoveView(photonViews[i]);
		}
	}

	// Token: 0x06004FB0 RID: 20400 RVA: 0x001A60E0 File Offset: 0x001A42E0
	void IPunOwnershipCallbacks.OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
	{
		if (!OwnershipGuardHandler.guardedViews.Contains(targetView))
		{
			return;
		}
		if (targetView.IsRoomView)
		{
			if (targetView.Owner != PhotonNetwork.MasterClient)
			{
				targetView.OwnerActorNr = 0;
				targetView.ControllerActorNr = 0;
				return;
			}
		}
		else if (targetView.OwnerActorNr != targetView.CreatorActorNr || targetView.ControllerActorNr != targetView.CreatorActorNr)
		{
			targetView.OwnerActorNr = targetView.CreatorActorNr;
			targetView.ControllerActorNr = targetView.CreatorActorNr;
		}
	}

	// Token: 0x06004FB1 RID: 20401 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IPunOwnershipCallbacks.OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
	{
	}

	// Token: 0x06004FB2 RID: 20402 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IPunOwnershipCallbacks.OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
	{
	}

	// Token: 0x0400616E RID: 24942
	private static HashSet<PhotonView> guardedViews = new HashSet<PhotonView>();

	// Token: 0x0400616F RID: 24943
	private static readonly OwnershipGuardHandler callbackInstance = new OwnershipGuardHandler();
}
