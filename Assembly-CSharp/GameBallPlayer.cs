using System;
using UnityEngine;

// Token: 0x020005ED RID: 1517
public class GameBallPlayer : MonoBehaviour
{
	// Token: 0x060025C3 RID: 9667 RVA: 0x000C82D4 File Offset: 0x000C64D4
	private void Awake()
	{
		this.hands = new GameBallPlayer.HandData[2];
		for (int i = 0; i < 2; i++)
		{
			this.ClearGrabbed(i);
		}
		this.teamId = -1;
	}

	// Token: 0x060025C4 RID: 9668 RVA: 0x000C8308 File Offset: 0x000C6508
	public void CleanupPlayer()
	{
		MonkeBallPlayer component = base.GetComponent<MonkeBallPlayer>();
		if (component != null)
		{
			component.currGoalZone = null;
			for (int i = 0; i < MonkeBallGame.Instance.goalZones.Count; i++)
			{
				MonkeBallGame.Instance.goalZones[i].CleanupPlayer(component);
			}
		}
	}

	// Token: 0x060025C5 RID: 9669 RVA: 0x000C835C File Offset: 0x000C655C
	public void SetGrabbed(GameBallId gameBallId, int handIndex)
	{
		if (gameBallId.IsValid())
		{
			this.ClearGrabbedIfHeld(gameBallId);
		}
		GameBallPlayer.HandData handData = this.hands[handIndex];
		handData.grabbedGameBallId = gameBallId;
		this.hands[handIndex] = handData;
	}

	// Token: 0x060025C6 RID: 9670 RVA: 0x000C839C File Offset: 0x000C659C
	public void ClearGrabbedIfHeld(GameBallId gameBallId)
	{
		for (int i = 0; i < 2; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				this.ClearGrabbed(i);
			}
		}
	}

	// Token: 0x060025C7 RID: 9671 RVA: 0x000C83D5 File Offset: 0x000C65D5
	public void ClearGrabbed(int handIndex)
	{
		this.SetGrabbed(GameBallId.Invalid, handIndex);
	}

	// Token: 0x060025C8 RID: 9672 RVA: 0x000C83E4 File Offset: 0x000C65E4
	public void ClearAllGrabbed()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			this.ClearGrabbed(i);
		}
	}

	// Token: 0x060025C9 RID: 9673 RVA: 0x000C840B File Offset: 0x000C660B
	public void SetInGoalZone(bool inZone)
	{
		if (inZone)
		{
			this.inGoalZone++;
			return;
		}
		this.inGoalZone--;
	}

	// Token: 0x060025CA RID: 9674 RVA: 0x000C8430 File Offset: 0x000C6630
	public bool IsHoldingBall()
	{
		return this.GetGameBallId().IsValid();
	}

	// Token: 0x060025CB RID: 9675 RVA: 0x000C844B File Offset: 0x000C664B
	public GameBallId GetGameBallId(int handIndex)
	{
		return this.hands[handIndex].grabbedGameBallId;
	}

	// Token: 0x060025CC RID: 9676 RVA: 0x000C8460 File Offset: 0x000C6660
	public int FindHandIndex(GameBallId gameBallId)
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId == gameBallId)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060025CD RID: 9677 RVA: 0x000C849C File Offset: 0x000C669C
	public GameBallId GetGameBallId()
	{
		for (int i = 0; i < this.hands.Length; i++)
		{
			if (this.hands[i].grabbedGameBallId.IsValid())
			{
				return this.hands[i].grabbedGameBallId;
			}
		}
		return GameBallId.Invalid;
	}

	// Token: 0x060025CE RID: 9678 RVA: 0x000C84EB File Offset: 0x000C66EB
	public bool IsLocalPlayer()
	{
		return VRRigCache.Instance.localRig.Creator.ActorNumber == this.rig.OwningNetPlayer.ActorNumber;
	}

	// Token: 0x060025CF RID: 9679 RVA: 0x000C8513 File Offset: 0x000C6713
	public static bool IsLeftHand(int handIndex)
	{
		return handIndex == 0;
	}

	// Token: 0x060025D0 RID: 9680 RVA: 0x000C8519 File Offset: 0x000C6719
	public static int GetHandIndex(bool leftHand)
	{
		if (!leftHand)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x060025D1 RID: 9681 RVA: 0x000C8524 File Offset: 0x000C6724
	public static VRRig GetRig(int actorNumber)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(actorNumber);
		RigContainer rigContainer;
		if (player == null || player.IsNull || !VRRigCache.Instance.TryGetVrrig(player, out rigContainer))
		{
			return null;
		}
		return rigContainer.Rig;
	}

	// Token: 0x060025D2 RID: 9682 RVA: 0x000C8560 File Offset: 0x000C6760
	public static GameBallPlayer GetGamePlayer(int actorNumber)
	{
		if (actorNumber < 0)
		{
			return null;
		}
		VRRig vrrig = GameBallPlayer.GetRig(actorNumber);
		if (vrrig == null)
		{
			return null;
		}
		return vrrig.GetComponent<GameBallPlayer>();
	}

	// Token: 0x060025D3 RID: 9683 RVA: 0x000C858C File Offset: 0x000C678C
	public static GameBallPlayer GetGamePlayer(Collider collider, bool bodyOnly = false)
	{
		Transform transform = collider.transform;
		while (transform != null)
		{
			GameBallPlayer component = transform.GetComponent<GameBallPlayer>();
			if (component != null)
			{
				return component;
			}
			if (bodyOnly)
			{
				break;
			}
			transform = transform.parent;
		}
		return null;
	}

	// Token: 0x04003136 RID: 12598
	public VRRig rig;

	// Token: 0x04003137 RID: 12599
	public int teamId;

	// Token: 0x04003138 RID: 12600
	private GameBallPlayer.HandData[] hands;

	// Token: 0x04003139 RID: 12601
	public const int MAX_HANDS = 2;

	// Token: 0x0400313A RID: 12602
	public const int LEFT_HAND = 0;

	// Token: 0x0400313B RID: 12603
	public const int RIGHT_HAND = 1;

	// Token: 0x0400313C RID: 12604
	private int inGoalZone;

	// Token: 0x020005EE RID: 1518
	private struct HandData
	{
		// Token: 0x0400313D RID: 12605
		public GameBallId grabbedGameBallId;
	}
}
