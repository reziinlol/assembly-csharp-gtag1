using System;
using System.Collections.Generic;
using System.Linq;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000054 RID: 84
public class CrittersBag : CrittersActor
{
	// Token: 0x060001A1 RID: 417 RVA: 0x0000A0CF File Offset: 0x000082CF
	protected override void Awake()
	{
		base.Awake();
		this.overlapColliders = new Collider[20];
		this.attachedColliders = new Dictionary<int, GameObject>();
		this.isAttachedToPlayer = false;
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000A0F6 File Offset: 0x000082F6
	public override void OnHover(bool isLeft)
	{
		if (this.isAttachedToPlayer)
		{
			GorillaTagger.Instance.StartVibration(isLeft, GorillaTagger.Instance.tapHapticStrength / 4f, GorillaTagger.Instance.tapHapticDuration * 0.5f);
			return;
		}
		base.OnHover(isLeft);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000A134 File Offset: 0x00008334
	protected override void CleanupActor()
	{
		base.CleanupActor();
		for (int i = this.attachedColliders.Count - 1; i >= 0; i--)
		{
			this.attachedColliders[this.attachedColliders.ElementAt(i).Key].gameObject.Destroy();
		}
		this.attachedColliders.Clear();
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x0000A194 File Offset: 0x00008394
	protected override void GlobalGrabbedBy(CrittersActor grabbedBy)
	{
		base.GlobalGrabbedBy(grabbedBy);
		bool flag = this.attachedToLocalPlayer;
		if (grabbedBy.IsNotNull())
		{
			CrittersAttachPoint crittersAttachPoint = grabbedBy as CrittersAttachPoint;
			if (crittersAttachPoint != null)
			{
				this.isAttachedToPlayer = true;
				this.attachedToLocalPlayer = (crittersAttachPoint.rigPlayerId == PhotonNetwork.LocalPlayer.ActorNumber);
				goto IL_4F;
			}
		}
		this.isAttachedToPlayer = false;
		this.attachedToLocalPlayer = false;
		IL_4F:
		if (this.attachedToLocalPlayer != flag)
		{
			bool flag2 = this.attachedToLocalPlayer || flag;
			this.audioSrc.transform.localPosition = Vector3.zero;
			this.audioSrc.GTPlayOneShot(this.attachedToLocalPlayer ? this.equipSound : this.unequipSound, flag2 ? 1f : 0.5f);
		}
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x0000A247 File Offset: 0x00008447
	public override void GrabbedBy(CrittersActor grabbedBy, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbedBy, positionOverride, localRotation, localOffset, disableGrabbing);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000A258 File Offset: 0x00008458
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulse = default(Vector3), Vector3 impulseRotation = default(Vector3))
	{
		if (this.parentActorId >= 0)
		{
			base.AttemptRemoveStoredObjectCollider(this.parentActorId, true);
		}
		int num = Physics.OverlapBoxNonAlloc(this.dropCube.transform.position, this.dropCube.size / 2f, this.overlapColliders, this.dropCube.transform.rotation, CrittersManager.instance.objectLayers, QueryTriggerInteraction.Collide);
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				Rigidbody attachedRigidbody = this.overlapColliders[i].attachedRigidbody;
				if (!(attachedRigidbody == null))
				{
					CrittersAttachPoint component = attachedRigidbody.GetComponent<CrittersAttachPoint>();
					if (!(component == null) && component.anchorLocation == this.anchorLocation && !(component.GetComponentInChildren<CrittersBag>() != null))
					{
						CrittersActor crittersActor;
						if (this.lastGrabbedPlayer == PhotonNetwork.LocalPlayer.ActorNumber && CrittersManager.instance.actorById.TryGetValue(this.parentActorId, out crittersActor))
						{
							CrittersGrabber crittersGrabber = crittersActor as CrittersGrabber;
							if (crittersGrabber != null)
							{
								GorillaTagger.Instance.StartVibration(crittersGrabber.isLeft, GorillaTagger.Instance.tapHapticStrength, GorillaTagger.Instance.tapHapticDuration);
							}
						}
						this.GrabbedBy(component, true, default(Quaternion), default(Vector3), false);
						return;
					}
				}
			}
		}
		base.Released(keepWorldPosition, rotation, position, impulse, impulseRotation);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x0000A3C8 File Offset: 0x000085C8
	public void AddStoredObjectCollider(CrittersActor actor)
	{
		if (this.attachedColliders.ContainsKey(actor.actorId))
		{
			if (this.attachedColliders[actor.actorId].IsNull())
			{
				this.attachedColliders[actor.actorId] = CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject;
			}
		}
		else
		{
			this.attachedColliders.Add(actor.actorId, CrittersManager.DuplicateCapsuleCollider(base.transform, actor.storeCollider).gameObject);
		}
		this.audioSrc.transform.position = actor.transform.position;
		this.audioSrc.GTPlayOneShot(this.attachSound, 1f);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000A484 File Offset: 0x00008684
	public void RemoveStoredObjectCollider(CrittersActor actor, bool playSound = true)
	{
		GameObject obj;
		if (this.attachedColliders.TryGetValue(actor.actorId, out obj))
		{
			Object.Destroy(obj);
			this.attachedColliders.Remove(actor.actorId);
		}
		if (playSound)
		{
			this.audioSrc.transform.position = actor.transform.position;
			this.audioSrc.GTPlayOneShot(this.detachSound, 1f);
		}
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0000A4F2 File Offset: 0x000086F2
	public bool IsActorValidStore(CrittersActor actor)
	{
		return this.blockAttachTypes == null || !this.blockAttachTypes.Contains(actor.crittersActorType);
	}

	// Token: 0x040001C3 RID: 451
	public AudioSource audioSrc;

	// Token: 0x040001C4 RID: 452
	public CrittersAttachPoint.AnchoredLocationTypes anchorLocation;

	// Token: 0x040001C5 RID: 453
	public Collider attachableCollider;

	// Token: 0x040001C6 RID: 454
	public BoxCollider dropCube;

	// Token: 0x040001C7 RID: 455
	private Collider[] overlapColliders;

	// Token: 0x040001C8 RID: 456
	public List<Collider> attachDisableColliders;

	// Token: 0x040001C9 RID: 457
	public Dictionary<int, GameObject> attachedColliders;

	// Token: 0x040001CA RID: 458
	[Header("Child object attachment sounds")]
	public AudioClip attachSound;

	// Token: 0x040001CB RID: 459
	public AudioClip detachSound;

	// Token: 0x040001CC RID: 460
	[Header("Monke equip sounds")]
	public AudioClip equipSound;

	// Token: 0x040001CD RID: 461
	public AudioClip unequipSound;

	// Token: 0x040001CE RID: 462
	[Header("Attachment Blocking")]
	public List<CrittersActor.CrittersActorType> blockAttachTypes;

	// Token: 0x040001CF RID: 463
	private bool isAttachedToPlayer;

	// Token: 0x040001D0 RID: 464
	private bool attachedToLocalPlayer;
}
