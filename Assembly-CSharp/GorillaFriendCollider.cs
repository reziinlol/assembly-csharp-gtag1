using System;
using System.Collections.Generic;
using GorillaNetworking;
using GTMathUtil;
using Unity.Profiling;
using UnityEngine;

// Token: 0x02000C7F RID: 3199
public class GorillaFriendCollider : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x06004F6E RID: 20334 RVA: 0x001A50FA File Offset: 0x001A32FA
	public void Awake()
	{
		this.thisCapsule = base.GetComponent<CapsuleCollider>();
		this.thisBox = base.GetComponent<BoxCollider>();
		if (!GorillaFriendCollider.updateAdded)
		{
			GorillaFriendCollider.updateAdded = true;
			VRRigCache.OnActiveRigsChanged += GorillaFriendCollider.UpdateActiveRigs;
			GorillaFriendCollider.UpdateActiveRigs();
		}
	}

	// Token: 0x06004F6F RID: 20335 RVA: 0x001A5137 File Offset: 0x001A3337
	private static void UpdateActiveRigs()
	{
		VRRigCache.Instance.GetActiveRigs(GorillaFriendCollider.playerRigs);
	}

	// Token: 0x06004F70 RID: 20336 RVA: 0x00018E08 File Offset: 0x00017008
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004F71 RID: 20337 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x06004F72 RID: 20338 RVA: 0x001A5148 File Offset: 0x001A3348
	private void AddUserID(in string userID)
	{
		if (this.playerIDsCurrentlyTouching.Contains(userID))
		{
			return;
		}
		this.playerIDsCurrentlyTouching.Add(userID);
	}

	// Token: 0x06004F73 RID: 20339 RVA: 0x001A5168 File Offset: 0x001A3368
	public void SliceUpdate()
	{
		using (GorillaFriendCollider.profiler_SliceUpdate.Auto())
		{
			if (NetworkSystem.Instance.InRoom || this.runCheckWhileNotInRoom)
			{
				this.RefreshPlayersWithinBounds();
			}
		}
	}

	// Token: 0x06004F74 RID: 20340 RVA: 0x001A51C0 File Offset: 0x001A33C0
	public void RefreshPlayersWithinBounds()
	{
		this.playerIDsCurrentlyTouching.Clear();
		for (int i = 0; i < GorillaFriendCollider.playerRigs.Count; i++)
		{
			float y = GorillaFriendCollider.playerRigs[i].bodyTransform.transform.position.y;
			bool flag = !this.applyCapsuleYLimits || (y >= this.capsuleColliderYLimits.x && y <= this.capsuleColliderYLimits.y);
			bool flag2 = (this.thisBox != null && WithinBounds.PointWithinBoxColliderBounds(GorillaFriendCollider.playerRigs[i].rigContainer.SpeakerHead.position, this.thisBox)) || (this.thisBox == null && this.thisCapsule != null && WithinBounds.PointWithinCapsuleColliderBounds(GorillaFriendCollider.playerRigs[i].rigContainer.SpeakerHead.position, this.thisCapsule));
			if (flag && flag2)
			{
				this.playerIDsCurrentlyTouching.Add(GorillaFriendCollider.playerRigs[i].isLocal ? NetworkSystem.Instance.LocalPlayer.UserId : GorillaFriendCollider.playerRigs[i].creator.UserId);
			}
		}
		if (NetworkSystem.Instance.InRoom && NetworkSystem.Instance.LocalPlayer != null && this.playerIDsCurrentlyTouching.Contains(NetworkSystem.Instance.LocalPlayer.UserId) && GorillaComputer.instance.friendJoinCollider != this)
		{
			GorillaComputer.instance.allowedMapsToJoin = this.myAllowedMapsToJoin;
			GorillaComputer.instance.friendJoinCollider = this;
			GorillaComputer.instance.UpdateScreen();
		}
	}

	// Token: 0x04006126 RID: 24870
	public List<string> playerIDsCurrentlyTouching = new List<string>();

	// Token: 0x04006127 RID: 24871
	private CapsuleCollider thisCapsule;

	// Token: 0x04006128 RID: 24872
	private BoxCollider thisBox;

	// Token: 0x04006129 RID: 24873
	[Tooltip("If using a capsule collider, the player position can be checked against these minimum and maximum Y limits (world position) to make it behave more like a cylinder check")]
	public bool applyCapsuleYLimits;

	// Token: 0x0400612A RID: 24874
	[Tooltip("If the player's Y world position is lower than Limits.x or higher than Limits.y, they will not be considered \"Inside\" the friend collider")]
	public Vector2 capsuleColliderYLimits = Vector2.zero;

	// Token: 0x0400612B RID: 24875
	public bool runCheckWhileNotInRoom;

	// Token: 0x0400612C RID: 24876
	public string[] myAllowedMapsToJoin;

	// Token: 0x0400612D RID: 24877
	private readonly Collider[] overlapColliders = new Collider[20];

	// Token: 0x0400612E RID: 24878
	public bool manualRefreshOnly;

	// Token: 0x0400612F RID: 24879
	private float _nextUpdateTime = -1f;

	// Token: 0x04006130 RID: 24880
	private static List<VRRig> playerRigs = new List<VRRig>();

	// Token: 0x04006131 RID: 24881
	private static bool updateAdded = false;

	// Token: 0x04006132 RID: 24882
	private static readonly ProfilerMarker profiler_SliceUpdate = new ProfilerMarker("GT/FriendCollider.SliceUpdate");
}
