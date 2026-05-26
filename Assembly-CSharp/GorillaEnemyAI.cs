using System;
using ExitGames.Client.Photon;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000A21 RID: 2593
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class GorillaEnemyAI : MonoBehaviourPun, IPunObservable, IInRoomCallbacks
{
	// Token: 0x06004257 RID: 16983 RVA: 0x001626D0 File Offset: 0x001608D0
	private void Start()
	{
		this.agent = base.GetComponent<NavMeshAgent>();
		this.r = base.GetComponent<Rigidbody>();
		this.r.useGravity = true;
		if (!base.photonView.IsMine)
		{
			this.agent.enabled = false;
			this.r.isKinematic = true;
		}
	}

	// Token: 0x06004258 RID: 16984 RVA: 0x00162728 File Offset: 0x00160928
	void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.IsWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.eulerAngles);
			return;
		}
		Vector3 vector = (Vector3)stream.ReceiveNext();
		ref this.targetPosition.SetValueSafe(vector);
		vector = (Vector3)stream.ReceiveNext();
		ref this.targetRotation.SetValueSafe(vector);
	}

	// Token: 0x06004259 RID: 16985 RVA: 0x0016279C File Offset: 0x0016099C
	private void Update()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.FindClosestPlayer();
			if (this.playerTransform != null)
			{
				this.agent.destination = this.playerTransform.position;
			}
			base.transform.LookAt(new Vector3(this.playerTransform.transform.position.x, base.transform.position.y, this.playerTransform.position.z));
			this.r.linearVelocity *= 0.99f;
			return;
		}
		base.transform.position = Vector3.Lerp(base.transform.position, this.targetPosition, this.lerpValue);
		base.transform.eulerAngles = Vector3.Lerp(base.transform.eulerAngles, this.targetRotation, this.lerpValue);
	}

	// Token: 0x0600425A RID: 16986 RVA: 0x0016288C File Offset: 0x00160A8C
	private void FindClosestPlayer()
	{
		VRRig[] array = Object.FindObjectsByType<VRRig>(FindObjectsSortMode.None);
		VRRig vrrig = null;
		float num = 100000f;
		foreach (VRRig vrrig2 in array)
		{
			Vector3 vector = vrrig2.transform.position - base.transform.position;
			if (vector.magnitude < num)
			{
				vrrig = vrrig2;
				num = vector.magnitude;
			}
		}
		this.playerTransform = vrrig.transform;
	}

	// Token: 0x0600425B RID: 16987 RVA: 0x001628FE File Offset: 0x00160AFE
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == 19)
		{
			PhotonNetwork.Destroy(base.photonView);
		}
	}

	// Token: 0x0600425C RID: 16988 RVA: 0x0016291A File Offset: 0x00160B1A
	void IInRoomCallbacks.OnMasterClientSwitched(Player newMasterClient)
	{
		if (PhotonNetwork.IsMasterClient)
		{
			this.agent.enabled = true;
			this.r.isKinematic = false;
		}
	}

	// Token: 0x0600425D RID: 16989 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnPlayerEnteredRoom(Player newPlayer)
	{
	}

	// Token: 0x0600425E RID: 16990 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnPlayerLeftRoom(Player otherPlayer)
	{
	}

	// Token: 0x0600425F RID: 16991 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
	{
	}

	// Token: 0x06004260 RID: 16992 RVA: 0x000028C5 File Offset: 0x00000AC5
	void IInRoomCallbacks.OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
	{
	}

	// Token: 0x0400542F RID: 21551
	public Transform playerTransform;

	// Token: 0x04005430 RID: 21552
	private NavMeshAgent agent;

	// Token: 0x04005431 RID: 21553
	private Rigidbody r;

	// Token: 0x04005432 RID: 21554
	private Vector3 targetPosition;

	// Token: 0x04005433 RID: 21555
	private Vector3 targetRotation;

	// Token: 0x04005434 RID: 21556
	public float lerpValue;
}
