using System;
using System.Collections.Generic;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AB4 RID: 2740
[NetworkBehaviourWeaved(1)]
public class WanderingGhost : NetworkComponent
{
	// Token: 0x06004610 RID: 17936 RVA: 0x0017AE48 File Offset: 0x00179048
	protected override void Start()
	{
		base.Start();
		this.waypointRegions = this.waypointsContainer.GetComponentsInChildren<ZoneBasedObject>();
		this.idlePassedTime = 0f;
		ThrowableSetDressing[] array = this.allFlowers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].anchor.position = this.flowerDisabledPosition;
		}
		base.Invoke("DelayedStart", 0.5f);
	}

	// Token: 0x06004611 RID: 17937 RVA: 0x0017AEAF File Offset: 0x001790AF
	private void DelayedStart()
	{
		this.PickNextWaypoint();
		base.transform.position = this.currentWaypoint._transform.position;
		this.PickNextWaypoint();
		this.ChangeState(WanderingGhost.ghostState.patrol);
	}

	// Token: 0x06004612 RID: 17938 RVA: 0x0017AEE0 File Offset: 0x001790E0
	private void LateUpdate()
	{
		this.UpdateState();
		this.hoverVelocity -= this.mrenderer.transform.localPosition * this.hoverRectifyForce * Time.deltaTime;
		this.hoverVelocity += Random.insideUnitSphere * this.hoverRandomForce * Time.deltaTime;
		this.hoverVelocity = Vector3.MoveTowards(this.hoverVelocity, Vector3.zero, this.hoverDrag * Time.deltaTime);
		this.mrenderer.transform.localPosition += this.hoverVelocity * Time.deltaTime;
	}

	// Token: 0x06004613 RID: 17939 RVA: 0x0017AFA4 File Offset: 0x001791A4
	private void PickNextWaypoint()
	{
		if (this.waypoints.Count == 0 || this.lastWaypointRegion == null || !this.lastWaypointRegion.IsLocalPlayerInZone())
		{
			ZoneBasedObject zoneBasedObject = ZoneBasedObject.SelectRandomEligible(this.waypointRegions, this.debugForceWaypointRegion);
			if (zoneBasedObject == null)
			{
				zoneBasedObject = this.lastWaypointRegion;
			}
			if (zoneBasedObject == null)
			{
				return;
			}
			this.lastWaypointRegion = zoneBasedObject;
			this.waypoints.Clear();
			foreach (object obj in zoneBasedObject.transform)
			{
				Transform transform = (Transform)obj;
				this.waypoints.Add(new WanderingGhost.Waypoint(transform.name.Contains("_v_"), transform));
			}
		}
		int index = Random.Range(0, this.waypoints.Count);
		this.currentWaypoint = this.waypoints[index];
		this.waypoints.RemoveAt(index);
	}

	// Token: 0x06004614 RID: 17940 RVA: 0x0017B0B4 File Offset: 0x001792B4
	private void Patrol()
	{
		this.idlePassedTime = 0f;
		this.mrenderer.sharedMaterial = this.scryableMaterial;
		Transform transform = this.currentWaypoint._transform;
		base.transform.position = Vector3.MoveTowards(base.transform.position, transform.position, this.patrolSpeed * Time.deltaTime);
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, Quaternion.LookRotation(transform.position - base.transform.position), 360f * Time.deltaTime);
	}

	// Token: 0x06004615 RID: 17941 RVA: 0x0017B158 File Offset: 0x00179358
	private bool MaybeHideGhost()
	{
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, this.hitColliders);
		for (int i = 0; i < num; i++)
		{
			if (this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaHand) || this.hitColliders[i].gameObject.IsOnLayer(UnityLayer.GorillaBodyCollider))
			{
				this.ChangeState(WanderingGhost.ghostState.patrol);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004616 RID: 17942 RVA: 0x0017B1C4 File Offset: 0x001793C4
	private void ChangeState(WanderingGhost.ghostState newState)
	{
		this.currentState = newState;
		this.mrenderer.sharedMaterial = ((newState == WanderingGhost.ghostState.idle) ? this.visibleMaterial : this.scryableMaterial);
		if (newState == WanderingGhost.ghostState.patrol)
		{
			this.audioSource.GTStop();
			this.audioSource.volume = this.patrolVolume;
			this.audioSource.clip = this.patrolAudio;
			this.audioSource.GTPlay();
			return;
		}
		if (newState != WanderingGhost.ghostState.idle)
		{
			return;
		}
		this.audioSource.GTStop();
		this.audioSource.volume = this.idleVolume;
		this.audioSource.GTPlayOneShot(this.appearAudio.GetRandomItem<AudioClip>(), 1f);
		if (NetworkSystem.Instance.IsMasterClient)
		{
			this.SpawnFlowerNearby();
		}
	}

	// Token: 0x06004617 RID: 17943 RVA: 0x0017B280 File Offset: 0x00179480
	private void UpdateState()
	{
		if (!NetworkSystem.Instance.IsMasterClient)
		{
			return;
		}
		WanderingGhost.ghostState ghostState = this.currentState;
		if (ghostState != WanderingGhost.ghostState.patrol)
		{
			if (ghostState != WanderingGhost.ghostState.idle)
			{
				return;
			}
			this.idlePassedTime += Time.deltaTime;
			if (this.idlePassedTime >= this.idleStayDuration || this.MaybeHideGhost())
			{
				this.PickNextWaypoint();
				this.ChangeState(WanderingGhost.ghostState.patrol);
			}
		}
		else
		{
			if (this.currentWaypoint._transform == null)
			{
				this.PickNextWaypoint();
				return;
			}
			this.Patrol();
			if (Vector3.Distance(base.transform.position, this.currentWaypoint._transform.position) < 0.2f)
			{
				if (this.currentWaypoint._visible)
				{
					this.ChangeState(WanderingGhost.ghostState.idle);
					return;
				}
				this.PickNextWaypoint();
				return;
			}
		}
	}

	// Token: 0x06004618 RID: 17944 RVA: 0x0017B344 File Offset: 0x00179544
	private void HauntObjects()
	{
		Collider[] array = new Collider[20];
		int num = Physics.OverlapSphereNonAlloc(base.transform.position, this.sphereColliderRadius, array);
		for (int i = 0; i < num; i++)
		{
			if (array[i].CompareTag("HauntedObject"))
			{
				UnityAction<GameObject> triggerHauntedObjects = this.TriggerHauntedObjects;
				if (triggerHauntedObjects != null)
				{
					triggerHauntedObjects(array[i].gameObject);
				}
			}
		}
	}

	// Token: 0x1700066D RID: 1645
	// (get) Token: 0x06004619 RID: 17945 RVA: 0x0017B3A5 File Offset: 0x001795A5
	// (set) Token: 0x0600461A RID: 17946 RVA: 0x0017B3CF File Offset: 0x001795CF
	[Networked]
	[NetworkedWeaved(0, 1)]
	private unsafe WanderingGhost.ghostState Data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return (WanderingGhost.ghostState)this.Ptr[0];
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing WanderingGhost.Data. Networked properties can only be accessed when Spawned() has been called.");
			}
			this.Ptr[0] = (int)value;
		}
	}

	// Token: 0x0600461B RID: 17947 RVA: 0x0017B3FA File Offset: 0x001795FA
	public override void WriteDataFusion()
	{
		this.Data = this.currentState;
	}

	// Token: 0x0600461C RID: 17948 RVA: 0x0017B408 File Offset: 0x00179608
	public override void ReadDataFusion()
	{
		this.ReadDataShared(this.Data);
	}

	// Token: 0x0600461D RID: 17949 RVA: 0x0017B416 File Offset: 0x00179616
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		stream.SendNext(this.currentState);
	}

	// Token: 0x0600461E RID: 17950 RVA: 0x0017B438 File Offset: 0x00179638
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (info.Sender != PhotonNetwork.MasterClient)
		{
			return;
		}
		WanderingGhost.ghostState state = (WanderingGhost.ghostState)stream.ReceiveNext();
		this.ReadDataShared(state);
	}

	// Token: 0x0600461F RID: 17951 RVA: 0x0017B466 File Offset: 0x00179666
	private void ReadDataShared(WanderingGhost.ghostState state)
	{
		WanderingGhost.ghostState ghostState = this.currentState;
		this.currentState = state;
		if (ghostState != this.currentState)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06004620 RID: 17952 RVA: 0x0017B489 File Offset: 0x00179689
	public override void OnOwnerChange(Player newOwner, Player previousOwner)
	{
		base.OnOwnerChange(newOwner, previousOwner);
		if (newOwner == PhotonNetwork.LocalPlayer)
		{
			this.ChangeState(this.currentState);
		}
	}

	// Token: 0x06004621 RID: 17953 RVA: 0x0017B4A8 File Offset: 0x001796A8
	private void SpawnFlowerNearby()
	{
		Vector3 position = base.transform.position + Vector3.down * 0.25f;
		RaycastHit raycastHit;
		if (Physics.Raycast(new Ray(base.transform.position + Random.insideUnitCircle.x0y() * this.flowerSpawnRadius, Vector3.down), out raycastHit, 3f, this.flowerGroundMask))
		{
			position = raycastHit.point;
		}
		ThrowableSetDressing throwableSetDressing = null;
		int num = 0;
		foreach (ThrowableSetDressing throwableSetDressing2 in this.allFlowers)
		{
			if (!throwableSetDressing2.InHand())
			{
				num++;
				if (Random.Range(0, num) == 0)
				{
					throwableSetDressing = throwableSetDressing2;
				}
			}
		}
		if (throwableSetDressing != null)
		{
			if (!throwableSetDressing.IsLocalOwnedWorldShareable)
			{
				throwableSetDressing.WorldShareableRequestOwnership();
			}
			throwableSetDressing.SetWillTeleport();
			throwableSetDressing.transform.position = position;
			throwableSetDressing.StartRespawnTimer(this.flowerSpawnDuration);
		}
	}

	// Token: 0x06004623 RID: 17955 RVA: 0x0017B5E8 File Offset: 0x001797E8
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Data = this._Data;
	}

	// Token: 0x06004624 RID: 17956 RVA: 0x0017B600 File Offset: 0x00179800
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Data = this.Data;
	}

	// Token: 0x04005877 RID: 22647
	public float patrolSpeed = 3f;

	// Token: 0x04005878 RID: 22648
	public float idleStayDuration = 5f;

	// Token: 0x04005879 RID: 22649
	public float sphereColliderRadius = 2f;

	// Token: 0x0400587A RID: 22650
	public ThrowableSetDressing[] allFlowers;

	// Token: 0x0400587B RID: 22651
	public Vector3 flowerDisabledPosition;

	// Token: 0x0400587C RID: 22652
	public float flowerSpawnRadius;

	// Token: 0x0400587D RID: 22653
	public float flowerSpawnDuration;

	// Token: 0x0400587E RID: 22654
	public LayerMask flowerGroundMask;

	// Token: 0x0400587F RID: 22655
	public MeshRenderer mrenderer;

	// Token: 0x04005880 RID: 22656
	public Material visibleMaterial;

	// Token: 0x04005881 RID: 22657
	public Material scryableMaterial;

	// Token: 0x04005882 RID: 22658
	public GameObject waypointsContainer;

	// Token: 0x04005883 RID: 22659
	private ZoneBasedObject[] waypointRegions;

	// Token: 0x04005884 RID: 22660
	private ZoneBasedObject lastWaypointRegion;

	// Token: 0x04005885 RID: 22661
	private List<WanderingGhost.Waypoint> waypoints = new List<WanderingGhost.Waypoint>();

	// Token: 0x04005886 RID: 22662
	private WanderingGhost.Waypoint currentWaypoint;

	// Token: 0x04005887 RID: 22663
	public string debugForceWaypointRegion;

	// Token: 0x04005888 RID: 22664
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04005889 RID: 22665
	public AudioClip[] appearAudio;

	// Token: 0x0400588A RID: 22666
	public float idleVolume;

	// Token: 0x0400588B RID: 22667
	public AudioClip patrolAudio;

	// Token: 0x0400588C RID: 22668
	public float patrolVolume;

	// Token: 0x0400588D RID: 22669
	private WanderingGhost.ghostState currentState;

	// Token: 0x0400588E RID: 22670
	private float idlePassedTime;

	// Token: 0x0400588F RID: 22671
	public UnityAction<GameObject> TriggerHauntedObjects;

	// Token: 0x04005890 RID: 22672
	private Vector3 hoverVelocity;

	// Token: 0x04005891 RID: 22673
	public float hoverRectifyForce;

	// Token: 0x04005892 RID: 22674
	public float hoverRandomForce;

	// Token: 0x04005893 RID: 22675
	public float hoverDrag;

	// Token: 0x04005894 RID: 22676
	private const int maxColliders = 10;

	// Token: 0x04005895 RID: 22677
	private Collider[] hitColliders = new Collider[10];

	// Token: 0x04005896 RID: 22678
	[WeaverGenerated]
	[DefaultForProperty("Data", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private WanderingGhost.ghostState _Data;

	// Token: 0x02000AB5 RID: 2741
	[Serializable]
	public struct Waypoint
	{
		// Token: 0x06004625 RID: 17957 RVA: 0x0017B614 File Offset: 0x00179814
		public Waypoint(bool visible, Transform tr)
		{
			this._visible = visible;
			this._transform = tr;
		}

		// Token: 0x04005897 RID: 22679
		[Tooltip("The ghost will be visible when its reached to this waypoint")]
		public bool _visible;

		// Token: 0x04005898 RID: 22680
		public Transform _transform;
	}

	// Token: 0x02000AB6 RID: 2742
	private enum ghostState
	{
		// Token: 0x0400589A RID: 22682
		patrol,
		// Token: 0x0400589B RID: 22683
		idle
	}
}
