using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

// Token: 0x02000286 RID: 646
public class GrabbyTentacleController : MonoBehaviour
{
	// Token: 0x0600115C RID: 4444 RVA: 0x0005D41C File Offset: 0x0005B61C
	private void OnEnable()
	{
		if (this.tentacles != null)
		{
			int num = this.tentacles.Length;
		}
		this.nextAttemptTimestamp = 0f;
		this.grabbedBefore.Clear();
		if (GrabbyTentacleNetworking.Instance != null)
		{
			GrabbyTentacleNetworking.Instance.Register(this);
			return;
		}
		Debug.LogError("[GrabbyTentacleController] No GrabbyTentacleNetworking.Instance at OnEnable — is the main scene loaded?");
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0005D473 File Offset: 0x0005B673
	private void OnDisable()
	{
		if (GrabbyTentacleNetworking.Instance != null)
		{
			GrabbyTentacleNetworking.Instance.Unregister(this);
		}
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0005D490 File Offset: 0x0005B690
	private void Update()
	{
		if (PhotonNetwork.InRoom && (!PhotonNetwork.IsMasterClient || GrabbyTentacleNetworking.Instance == null))
		{
			return;
		}
		if (this.tentacles == null || this.tentacles.Length == 0 || this.grabRegion == null)
		{
			return;
		}
		int num = 0;
		while (num < this.tentacles.Length && this.nextAttemptTimestamp <= Time.time)
		{
			TentacleTracker tentacleTracker = this.tentacles[num];
			if (!(tentacleTracker == null) && !tentacleTracker.gameObject.activeSelf)
			{
				this.nextAttemptTimestamp = Time.time + Random.Range(this.minRetryDelay, this.maxRetryDelay);
				Player player = this.PickTarget();
				if (player != null)
				{
					this.grabbedBefore.Add(player.ActorNumber);
					if (PhotonNetwork.InRoom)
					{
						GrabbyTentacleNetworking.Instance.SendGrab(num, player);
					}
					else
					{
						this.OnGrabReceived(num, VRRig.LocalRig, true);
					}
				}
			}
			num++;
		}
	}

	// Token: 0x0600115F RID: 4447 RVA: 0x0005D57C File Offset: 0x0005B77C
	private Player PickTarget()
	{
		this.candidateBuffer.Clear();
		this.freshCandidates.Clear();
		IReadOnlyList<VRRig> activeRigs = VRRigCache.ActiveRigs;
		for (int i = 0; i < activeRigs.Count; i++)
		{
			VRRig vrrig = activeRigs[i];
			if (!(vrrig == null) && vrrig.Creator != null && !vrrig.Creator.IsNull && !this.IsRigCurrentlyGrabbed(vrrig))
			{
				Vector3 vector = (vrrig.head != null && vrrig.head.rigTarget != null) ? vrrig.head.rigTarget.position : vrrig.transform.position;
				if (!(this.grabRegion.ClosestPoint(vector) != vector))
				{
					this.candidateBuffer.Add(vrrig);
					if (!this.grabbedBefore.Contains(vrrig.Creator.ActorNumber))
					{
						this.freshCandidates.Add(vrrig);
					}
				}
			}
		}
		List<VRRig> list = (this.freshCandidates.Count > 0) ? this.freshCandidates : this.candidateBuffer;
		if (list.Count == 0)
		{
			return null;
		}
		VRRig vrrig2 = list[Random.Range(0, list.Count)];
		Room currentRoom = PhotonNetwork.CurrentRoom;
		if (currentRoom == null)
		{
			return null;
		}
		return currentRoom.GetPlayer(vrrig2.Creator.ActorNumber, false);
	}

	// Token: 0x06001160 RID: 4448 RVA: 0x0005D6DC File Offset: 0x0005B8DC
	private bool IsRigCurrentlyGrabbed(VRRig rig)
	{
		for (int i = 0; i < this.tentacles.Length; i++)
		{
			TentacleTracker tentacleTracker = this.tentacles[i];
			if (tentacleTracker != null && tentacleTracker.gameObject.activeSelf && tentacleTracker.currentTargetRig == rig)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001161 RID: 4449 RVA: 0x0005D72C File Offset: 0x0005B92C
	public void OnGrabReceived(int tentacleIndex, VRRig targetRig, bool isLocalPlayer)
	{
		if (this.tentacles == null || tentacleIndex < 0 || tentacleIndex >= this.tentacles.Length)
		{
			return;
		}
		TentacleTracker tentacleTracker = this.tentacles[tentacleIndex];
		if (tentacleTracker == null)
		{
			return;
		}
		tentacleTracker.BeginGrab(targetRig, isLocalPlayer);
	}

	// Token: 0x040014BB RID: 5307
	[SerializeField]
	private TentacleTracker[] tentacles;

	// Token: 0x040014BC RID: 5308
	[SerializeField]
	private BoxCollider grabRegion;

	// Token: 0x040014BD RID: 5309
	[SerializeField]
	private float minRetryDelay = 1f;

	// Token: 0x040014BE RID: 5310
	[SerializeField]
	private float maxRetryDelay = 2f;

	// Token: 0x040014BF RID: 5311
	private float nextAttemptTimestamp;

	// Token: 0x040014C0 RID: 5312
	private readonly HashSet<int> grabbedBefore = new HashSet<int>();

	// Token: 0x040014C1 RID: 5313
	private readonly List<VRRig> candidateBuffer = new List<VRRig>(16);

	// Token: 0x040014C2 RID: 5314
	private readonly List<VRRig> freshCandidates = new List<VRRig>(16);
}
