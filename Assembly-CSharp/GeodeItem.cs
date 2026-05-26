using System;
using GorillaTag;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000522 RID: 1314
public class GeodeItem : TransferrableObject
{
	// Token: 0x060020F4 RID: 8436 RVA: 0x000B03E9 File Offset: 0x000AE5E9
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.hasEffectsGameObject = (this.effectsGameObject != null);
		this.effectsHaveBeenPlayed = false;
	}

	// Token: 0x060020F5 RID: 8437 RVA: 0x000B040B File Offset: 0x000AE60B
	protected override void Start()
	{
		base.Start();
		this.itemState = TransferrableObject.ItemStates.State0;
		this.prevItemState = TransferrableObject.ItemStates.State0;
		this.InitToDefault();
	}

	// Token: 0x060020F6 RID: 8438 RVA: 0x000B0427 File Offset: 0x000AE627
	public override void ResetToDefaultState()
	{
		base.ResetToDefaultState();
		this.InitToDefault();
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060020F7 RID: 8439 RVA: 0x000B043C File Offset: 0x000AE63C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		return base.OnRelease(zoneReleased, releasingHand) && this.itemState != TransferrableObject.ItemStates.State0 && !base.InHand();
	}

	// Token: 0x060020F8 RID: 8440 RVA: 0x000B0460 File Offset: 0x000AE660
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		base.OnGrab(pointGrabbed, grabbingHand);
		UnityEvent<GeodeItem> onGeodeGrabbed = this.OnGeodeGrabbed;
		if (onGeodeGrabbed == null)
		{
			return;
		}
		onGeodeGrabbed.Invoke(this);
	}

	// Token: 0x060020F9 RID: 8441 RVA: 0x000B047C File Offset: 0x000AE67C
	private void InitToDefault()
	{
		this.cooldownRemaining = 0f;
		this.effectsHaveBeenPlayed = false;
		if (this.hasEffectsGameObject)
		{
			this.effectsGameObject.SetActive(false);
		}
		this.geodeFullMesh.SetActive(true);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(false);
		}
		this.hitLastFrame = false;
	}

	// Token: 0x060020FA RID: 8442 RVA: 0x000B04E4 File Offset: 0x000AE6E4
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			this.cooldownRemaining -= Time.deltaTime;
			if (this.cooldownRemaining <= 0f)
			{
				this.itemState = TransferrableObject.ItemStates.State0;
				this.OnItemStateChanged();
			}
			return;
		}
		if (this.velocityEstimator.linearVelocity.magnitude < this.minHitVelocity)
		{
			return;
		}
		if (base.InHand())
		{
			int num = Physics.SphereCastNonAlloc(this.geodeFullMesh.transform.position, this.sphereRayRadius * Mathf.Abs(this.geodeFullMesh.transform.lossyScale.x), this.geodeFullMesh.transform.TransformDirection(Vector3.forward), this.collidersHit, this.rayCastMaxDistance, this.collisionLayerMask, QueryTriggerInteraction.Collide);
			this.hitLastFrame = (num > 0);
		}
		if (!this.hitLastFrame)
		{
			return;
		}
		if (!GorillaParent.hasInstance)
		{
			return;
		}
		UnityEvent<GeodeItem> onGeodeCracked = this.OnGeodeCracked;
		if (onGeodeCracked != null)
		{
			onGeodeCracked.Invoke(this);
		}
		this.itemState = TransferrableObject.ItemStates.State1;
		this.cooldownRemaining = this.cooldown;
		this.index = (this.randomizeGeode ? this.RandomPickCrackedGeode() : 0);
	}

	// Token: 0x060020FB RID: 8443 RVA: 0x000B060C File Offset: 0x000AE80C
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		this.currentItemState = this.itemState;
		if (this.currentItemState != this.prevItemState)
		{
			this.OnItemStateChanged();
		}
		this.prevItemState = this.currentItemState;
	}

	// Token: 0x060020FC RID: 8444 RVA: 0x000B0640 File Offset: 0x000AE840
	private void OnItemStateChanged()
	{
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.InitToDefault();
			return;
		}
		this.geodeFullMesh.SetActive(false);
		for (int i = 0; i < this.geodeCrackedMeshes.Length; i++)
		{
			this.geodeCrackedMeshes[i].SetActive(i == this.index);
		}
		RigContainer rigContainer;
		if (NetworkSystem.Instance.InRoom && GorillaGameManager.instance != null && !this.effectsHaveBeenPlayed && VRRigCache.Instance.TryGetVrrig(NetworkSystem.Instance.LocalPlayer, out rigContainer))
		{
			rigContainer.Rig.netView.SendRPC("RPC_PlayGeodeEffect", RpcTarget.All, new object[]
			{
				this.geodeFullMesh.transform.position
			});
			this.effectsHaveBeenPlayed = true;
		}
		if (!NetworkSystem.Instance.InRoom && !this.effectsHaveBeenPlayed)
		{
			if (this.audioSource)
			{
				this.audioSource.GTPlay();
			}
			this.effectsHaveBeenPlayed = true;
		}
	}

	// Token: 0x060020FD RID: 8445 RVA: 0x000B0739 File Offset: 0x000AE939
	private int RandomPickCrackedGeode()
	{
		return Random.Range(0, this.geodeCrackedMeshes.Length);
	}

	// Token: 0x04002BB8 RID: 11192
	[Tooltip("This GameObject will activate when the geode hits the ground with enough force.")]
	public GameObject effectsGameObject;

	// Token: 0x04002BB9 RID: 11193
	public LayerMask collisionLayerMask;

	// Token: 0x04002BBA RID: 11194
	[Tooltip("Used to calculate velocity of the geode.")]
	public GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04002BBB RID: 11195
	public float cooldown = 5f;

	// Token: 0x04002BBC RID: 11196
	[Tooltip("The velocity of the geode must be greater than this value to activate the effect.")]
	public float minHitVelocity = 0.2f;

	// Token: 0x04002BBD RID: 11197
	[Tooltip("Geode's full mesh before cracking")]
	public GameObject geodeFullMesh;

	// Token: 0x04002BBE RID: 11198
	[Tooltip("Geode's cracked open half different meshes, picked randomly")]
	public GameObject[] geodeCrackedMeshes;

	// Token: 0x04002BBF RID: 11199
	[Tooltip("The distance between te geode and the layer mask to detect whether it hits it")]
	public float rayCastMaxDistance = 0.2f;

	// Token: 0x04002BC0 RID: 11200
	[FormerlySerializedAs("collisionRadius")]
	public float sphereRayRadius = 0.05f;

	// Token: 0x04002BC1 RID: 11201
	[DebugReadout]
	private float cooldownRemaining;

	// Token: 0x04002BC2 RID: 11202
	[DebugReadout]
	private bool hitLastFrame;

	// Token: 0x04002BC3 RID: 11203
	[SerializeField]
	private AudioSource audioSource;

	// Token: 0x04002BC4 RID: 11204
	public bool randomizeGeode = true;

	// Token: 0x04002BC5 RID: 11205
	public UnityEvent<GeodeItem> OnGeodeCracked;

	// Token: 0x04002BC6 RID: 11206
	public UnityEvent<GeodeItem> OnGeodeGrabbed;

	// Token: 0x04002BC7 RID: 11207
	private bool hasEffectsGameObject;

	// Token: 0x04002BC8 RID: 11208
	private bool effectsHaveBeenPlayed;

	// Token: 0x04002BC9 RID: 11209
	private RaycastHit hit;

	// Token: 0x04002BCA RID: 11210
	private RaycastHit[] collidersHit = new RaycastHit[20];

	// Token: 0x04002BCB RID: 11211
	private TransferrableObject.ItemStates currentItemState;

	// Token: 0x04002BCC RID: 11212
	private TransferrableObject.ItemStates prevItemState;

	// Token: 0x04002BCD RID: 11213
	private int index;
}
