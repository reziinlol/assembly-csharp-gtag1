using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000058 RID: 88
public class CrittersCage : CrittersActor
{
	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060001AF RID: 431 RVA: 0x0000A6D8 File Offset: 0x000088D8
	public Vector3 critterScale
	{
		get
		{
			if (this.subObjectIndex < this.critterScales.Length && this.subObjectIndex >= 0)
			{
				return this.critterScales[this.subObjectIndex];
			}
			return Vector3.one;
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060001B0 RID: 432 RVA: 0x0000A70A File Offset: 0x0000890A
	public bool CanCatch
	{
		get
		{
			return this.heldByPlayer && !this.hasCritter && !this.inReleasingPosition && this._releaseCooldownEnd <= Time.time;
		}
	}

	// Token: 0x060001B1 RID: 433 RVA: 0x0000A736 File Offset: 0x00008936
	public void SetHasCritter(bool value)
	{
		if (this.hasCritter != value && !value)
		{
			this._releaseCooldownEnd = Time.time + this.releaseCooldown;
		}
		this.hasCritter = value;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001B2 RID: 434 RVA: 0x0000A763 File Offset: 0x00008963
	public override void Initialize()
	{
		base.Initialize();
		this.hasCritter = false;
		this.heldByPlayer = false;
		this.inReleasingPosition = false;
		this.SetLidActive(true, false);
	}

	// Token: 0x060001B3 RID: 435 RVA: 0x0000A788 File Offset: 0x00008988
	private void UpdateCageVisuals()
	{
		this.SetLidActive(!this.heldByPlayer || this.hasCritter, true);
	}

	// Token: 0x060001B4 RID: 436 RVA: 0x0000A7A4 File Offset: 0x000089A4
	private void SetLidActive(bool active, bool playAudio = true)
	{
		if (active != this._lidActive && playAudio)
		{
			this.sound.GTPlayOneShot(active ? this.openSound : this.closeSound, 1f);
		}
		this.lid.SetActive(active);
		this._lidActive = active;
	}

	// Token: 0x060001B5 RID: 437 RVA: 0x0000A7F5 File Offset: 0x000089F5
	protected override void RemoteGrabbedBy(CrittersActor grabbingActor)
	{
		base.RemoteGrabbedBy(grabbingActor);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001B6 RID: 438 RVA: 0x0000A810 File Offset: 0x00008A10
	public override void GrabbedBy(CrittersActor grabbingActor, bool positionOverride = false, Quaternion localRotation = default(Quaternion), Vector3 localOffset = default(Vector3), bool disableGrabbing = false)
	{
		base.GrabbedBy(grabbingActor, positionOverride, localRotation, localOffset, disableGrabbing);
		this.heldByPlayer = grabbingActor.isOnPlayer;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x0000A831 File Offset: 0x00008A31
	public override void Released(bool keepWorldPosition, Quaternion rotation = default(Quaternion), Vector3 position = default(Vector3), Vector3 impulseVelocity = default(Vector3), Vector3 impulseAngularVelocity = default(Vector3))
	{
		base.Released(keepWorldPosition, rotation, position, impulseVelocity, impulseAngularVelocity);
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001B8 RID: 440 RVA: 0x0000A84D File Offset: 0x00008A4D
	protected override void HandleRemoteReleased()
	{
		base.HandleRemoteReleased();
		this.heldByPlayer = false;
		this.UpdateCageVisuals();
	}

	// Token: 0x060001B9 RID: 441 RVA: 0x0000A862 File Offset: 0x00008A62
	public override bool ShouldDespawn()
	{
		return base.ShouldDespawn() && !this.hasCritter;
	}

	// Token: 0x060001BA RID: 442 RVA: 0x0000A877 File Offset: 0x00008A77
	public override void SendDataByCrittersActorType(PhotonStream stream)
	{
		base.SendDataByCrittersActorType(stream);
		stream.SendNext(this.hasCritter);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x0000A894 File Offset: 0x00008A94
	public override bool UpdateSpecificActor(PhotonStream stream)
	{
		if (!base.UpdateSpecificActor(stream))
		{
			return false;
		}
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(stream.ReceiveNext(), out flag))
		{
			return false;
		}
		this.SetHasCritter(flag);
		return true;
	}

	// Token: 0x060001BC RID: 444 RVA: 0x0000A8C5 File Offset: 0x00008AC5
	public override int AddActorDataToList(ref List<object> objList)
	{
		base.AddActorDataToList(ref objList);
		objList.Add(this.hasCritter);
		return this.TotalActorDataLength();
	}

	// Token: 0x060001BD RID: 445 RVA: 0x00009EC8 File Offset: 0x000080C8
	public override int TotalActorDataLength()
	{
		return base.BaseActorDataLength() + 1;
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000A8E8 File Offset: 0x00008AE8
	public override int UpdateFromRPC(object[] data, int startingIndex)
	{
		startingIndex += base.UpdateFromRPC(data, startingIndex);
		bool flag;
		if (!CrittersManager.ValidateDataType<bool>(data[startingIndex], out flag))
		{
			return this.TotalActorDataLength();
		}
		this.SetHasCritter(flag);
		return this.TotalActorDataLength();
	}

	// Token: 0x040001E3 RID: 483
	public Transform grabPosition;

	// Token: 0x040001E4 RID: 484
	public Transform cagePosition;

	// Token: 0x040001E5 RID: 485
	public float grabDistance;

	// Token: 0x040001E6 RID: 486
	[SerializeField]
	private Vector3[] critterScales = new Vector3[]
	{
		Vector3.one
	};

	// Token: 0x040001E7 RID: 487
	[SerializeField]
	private float releaseCooldown = 0.25f;

	// Token: 0x040001E8 RID: 488
	[SerializeField]
	private AudioSource sound;

	// Token: 0x040001E9 RID: 489
	[SerializeField]
	private AudioClip openSound;

	// Token: 0x040001EA RID: 490
	[SerializeField]
	private AudioClip closeSound;

	// Token: 0x040001EB RID: 491
	public GameObject lid;

	// Token: 0x040001EC RID: 492
	[NonSerialized]
	public bool heldByPlayer;

	// Token: 0x040001ED RID: 493
	[NonSerialized]
	private bool hasCritter;

	// Token: 0x040001EE RID: 494
	[NonSerialized]
	public bool inReleasingPosition;

	// Token: 0x040001EF RID: 495
	private float _releaseCooldownEnd;

	// Token: 0x040001F0 RID: 496
	private bool _lidActive;
}
