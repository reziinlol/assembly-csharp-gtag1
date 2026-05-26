using System;
using System.Collections.Generic;
using GorillaExtensions;
using GorillaLocomotion;
using GorillaTag.CosmeticSystem;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000532 RID: 1330
public class OneStringGuitar : TransferrableObject
{
	// Token: 0x06002192 RID: 8594 RVA: 0x000B2B84 File Offset: 0x000B0D84
	public override Matrix4x4 GetDefaultTransformationMatrix()
	{
		return Matrix4x4.identity;
	}

	// Token: 0x06002193 RID: 8595 RVA: 0x000B2B8C File Offset: 0x000B0D8C
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.chestColliderLeft = this._GetChestColliderByPath(rig, "rig/body_pivot/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformLeft");
		this.chestColliderRight = this._GetChestColliderByPath(rig, "rig/body_pivot/Old Cosmetics Body/OneStringGuitarStick/Center/BaseTransformRight");
		this.currentChestCollider = this.chestColliderLeft;
		Transform[] array;
		string str;
		if (!GTHardCodedBones.TryGetBoneXforms(rig, out array, out str))
		{
			Debug.LogError("OneStringGuitar: Error getting bone Transforms: " + str, this);
			return;
		}
		this.parentHandLeft = array[9];
		this.parentHandRight = array[27];
		this.parentHand = this.parentHandRight;
		this.leftHandIndicator = GorillaTagger.Instance.leftHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.rightHandIndicator = GorillaTagger.Instance.rightHandTriggerCollider.GetComponent<GorillaTriggerColliderHandIndicator>();
		this.sphereRadius = this.leftHandIndicator.GetComponent<SphereCollider>().radius;
		this.itemState = TransferrableObject.ItemStates.State0;
		this.nullHit = default(RaycastHit);
		this.strumList.Add(this.strumCollider);
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.startingLeftChestOffset = this.chestOffsetLeft;
		this.startingRightChestOffset = this.chestOffsetRight;
		this.startingUnsnapDistance = this.unsnapDistance;
		this.selfInstrumentIndex = rig.AssignInstrumentToInstrumentSelfOnly(this);
		for (int i = 0; i < this.frets.Length; i++)
		{
			this.fretsList.Add(this.frets[i]);
		}
	}

	// Token: 0x06002194 RID: 8596 RVA: 0x000B2CD4 File Offset: 0x000B0ED4
	private Collider _GetChestColliderByPath(VRRig vrRig, string chestColliderLeftPath)
	{
		Transform transform;
		if (!vrRig.transform.TryFindByExactPath(chestColliderLeftPath, out transform))
		{
			Debug.LogError("DEACTIVATING! do you move this without updating the script? could not find this transform: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		Collider component = transform.GetComponent<Collider>();
		if (!component)
		{
			Debug.LogError("DEACTIVATING! found transform but couldn't find collider at path: \"" + chestColliderLeftPath + "\"");
			base.gameObject.SetActive(false);
		}
		return component;
	}

	// Token: 0x06002195 RID: 8597 RVA: 0x000B2D44 File Offset: 0x000B0F44
	internal override void OnEnable()
	{
		base.OnEnable();
		if (this.currentState == TransferrableObject.PositionState.InLeftHand)
		{
			this.fretHandIndicator = this.leftHandIndicator;
			this.strumHandIndicator = this.rightHandIndicator;
		}
		else
		{
			this.fretHandIndicator = this.rightHandIndicator;
			this.strumHandIndicator = this.leftHandIndicator;
		}
		if (base.IsLocalObject())
		{
			this.parentHand = GTPlayer.Instance.GetHandFollower(this.currentState == TransferrableObject.PositionState.InLeftHand);
		}
		this.initOffset = Vector3.zero;
		this.initRotation = Quaternion.identity;
	}

	// Token: 0x06002196 RID: 8598 RVA: 0x000B2DC9 File Offset: 0x000B0FC9
	internal override void OnDisable()
	{
		base.OnDisable();
		this.angleSnapped = false;
		this.positionSnapped = false;
		this.lastState = OneStringGuitar.GuitarStates.Club;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06002197 RID: 8599 RVA: 0x000B2DED File Offset: 0x000B0FED
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		if (!base.OnRelease(zoneReleased, releasingHand))
		{
			return false;
		}
		if (!this.CanDeactivate())
		{
			return false;
		}
		if (base.InHand())
		{
			return false;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
		return true;
	}

	// Token: 0x06002198 RID: 8600 RVA: 0x000B2E18 File Offset: 0x000B1018
	protected override void LateUpdateShared()
	{
		base.LateUpdateShared();
		if (this.lastState != (OneStringGuitar.GuitarStates)this.itemState)
		{
			this.angleSnapped = false;
			this.positionSnapped = false;
		}
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			Vector3 positionTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startPositionLeft : this.startPositionRight;
			Quaternion rotationTarget = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.startQuatLeft : this.startQuatRight;
			this.UpdateNonPlayingPosition(positionTarget, rotationTarget);
		}
		else if (this.itemState == TransferrableObject.ItemStates.State1)
		{
			Vector3 positionTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripPositionLeft : this.reverseGripPositionRight;
			Quaternion rotationTarget2 = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.reverseGripQuatLeft : this.reverseGripQuatRight;
			this.UpdateNonPlayingPosition(positionTarget2, rotationTarget2);
			if (this.IsMyItem() && (this.chestTouch.transform.position - this.currentChestCollider.transform.position).magnitude < this.snapDistance)
			{
				this.itemState = TransferrableObject.ItemStates.State2;
				this.angleSnapped = false;
				this.positionSnapped = false;
				this.currentChestCollider.gameObject.SetActive(true);
			}
		}
		else if (this.itemState == TransferrableObject.ItemStates.State2)
		{
			Quaternion rhs = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.holdingOffsetRotationLeft : this.holdingOffsetRotationRight;
			Vector3 point = (this.currentState == TransferrableObject.PositionState.InLeftHand) ? this.chestOffsetLeft : this.chestOffsetRight;
			Quaternion quaternion = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * rhs;
			if (!this.angleSnapped && Quaternion.Angle(base.transform.rotation, quaternion) > this.angleLerpSnap)
			{
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, quaternion, this.lerpValue);
			}
			else
			{
				this.angleSnapped = true;
				base.transform.rotation = quaternion;
			}
			Vector3 vector = this.currentChestCollider.transform.position + base.transform.rotation * point;
			if (!this.positionSnapped && (base.transform.position - vector).magnitude > this.vectorLerpSnap)
			{
				base.transform.position = Vector3.Lerp(base.transform.position, this.currentChestCollider.transform.position + base.transform.rotation * point, this.lerpValue);
			}
			else
			{
				this.positionSnapped = true;
				base.transform.position = vector;
			}
			if (this.currentState == TransferrableObject.PositionState.InRightHand)
			{
				this.parentHand = this.parentHandRight;
			}
			else
			{
				this.parentHand = this.parentHandLeft;
			}
			if (this.IsMyItem())
			{
				this.unsnapDistance = this.startingUnsnapDistance * base.myRig.transform.localScale.x;
				if (this.currentState == TransferrableObject.PositionState.InRightHand)
				{
					this.chestOffsetRight = Vector3.Scale(this.startingRightChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderRight;
					this.fretHandIndicator = this.rightHandIndicator;
					this.strumHandIndicator = this.leftHandIndicator;
				}
				else
				{
					this.chestOffsetLeft = Vector3.Scale(this.startingLeftChestOffset, base.myRig.transform.localScale);
					this.currentChestCollider = this.chestColliderLeft;
					this.fretHandIndicator = this.leftHandIndicator;
					this.strumHandIndicator = this.rightHandIndicator;
				}
				if (this.Unsnap())
				{
					this.itemState = TransferrableObject.ItemStates.State1;
					this.angleSnapped = false;
					this.positionSnapped = false;
					if (this.currentState == TransferrableObject.PositionState.InLeftHand)
					{
						EquipmentInteractor.instance.wasLeftGrabPressed = true;
					}
					else
					{
						EquipmentInteractor.instance.wasRightGrabPressed = true;
					}
					this.currentChestCollider.gameObject.SetActive(false);
				}
				else
				{
					if (!this.handIn)
					{
						this.CheckFretFinger(this.fretHandIndicator.transform);
						HitChecker.CheckHandHit(ref this.collidersHitCount, this.interactableMask, this.sphereRadius, ref this.nullHit, ref this.raycastHits, ref this.raycastHitList, ref this.spherecastSweep, ref this.strumHandIndicator);
						if (this.collidersHitCount > 0)
						{
							int i = 0;
							while (i < this.collidersHitCount)
							{
								if (this.raycastHits[i].collider != null && this.strumCollider == this.raycastHits[i].collider)
								{
									GorillaTagger.Instance.StartVibration(this.strumHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
									this.PlayNote(this.currentFretIndex, Mathf.Max(Mathf.Min(1f, this.strumHandIndicator.currentVelocity.magnitude / this.maxVelocity) * this.maxVolume, this.minVolume));
									if (!NetworkSystem.Instance.InRoom || this.selfInstrumentIndex <= -1)
									{
										break;
									}
									NetworkView myVRRig = GorillaTagger.Instance.myVRRig;
									if (myVRRig == null)
									{
										break;
									}
									myVRRig.SendRPC("RPC_PlaySelfOnlyInstrument", RpcTarget.Others, new object[]
									{
										this.selfInstrumentIndex,
										this.currentFretIndex,
										this.audioSource.volume
									});
									break;
								}
								else
								{
									i++;
								}
							}
						}
					}
					this.handIn = HitChecker.CheckHandIn(ref this.anyHit, ref this.collidersHit, this.sphereRadius * base.transform.lossyScale.x, this.interactableMask, ref this.strumHandIndicator, ref this.strumList);
				}
			}
		}
		this.lastState = (OneStringGuitar.GuitarStates)this.itemState;
	}

	// Token: 0x06002199 RID: 8601 RVA: 0x000B33D4 File Offset: 0x000B15D4
	public override void PlayNote(int note, float volume)
	{
		this.audioSource.time = 0.005f;
		this.audioSource.clip = this.audioClips[note];
		this.audioSource.volume = volume;
		this.audioSource.GTPlay();
		base.PlayNote(note, volume);
	}

	// Token: 0x0600219A RID: 8602 RVA: 0x000B3424 File Offset: 0x000B1624
	private bool Unsnap()
	{
		return (this.parentHand.position - this.chestTouch.position).magnitude > this.unsnapDistance;
	}

	// Token: 0x0600219B RID: 8603 RVA: 0x000B345C File Offset: 0x000B165C
	private void CheckFretFinger(Transform finger)
	{
		for (int i = 0; i < this.collidersHit.Length; i++)
		{
			this.collidersHit[i] = null;
		}
		this.collidersHitCount = Physics.OverlapSphereNonAlloc(finger.position, this.sphereRadius, this.collidersHit, this.interactableMask, QueryTriggerInteraction.Collide);
		this.currentFretIndex = 5;
		if (this.collidersHitCount > 0)
		{
			for (int j = 0; j < this.collidersHit.Length; j++)
			{
				if (this.fretsList.Contains(this.collidersHit[j]))
				{
					this.currentFretIndex = this.fretsList.IndexOf(this.collidersHit[j]);
					if (this.currentFretIndex != this.lastFretIndex)
					{
						GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
					}
					this.lastFretIndex = this.currentFretIndex;
					return;
				}
			}
			return;
		}
		if (this.lastFretIndex != -1)
		{
			GorillaTagger.Instance.StartVibration(this.fretHandIndicator.isLeftHand, GorillaTagger.Instance.tapHapticStrength / 6f, GorillaTagger.Instance.tapHapticDuration);
		}
		this.lastFretIndex = -1;
	}

	// Token: 0x0600219C RID: 8604 RVA: 0x000B3590 File Offset: 0x000B1790
	public void UpdateNonPlayingPosition(Vector3 positionTarget, Quaternion rotationTarget)
	{
		if (!this.angleSnapped)
		{
			if (Quaternion.Angle(rotationTarget, base.transform.localRotation) < this.angleLerpSnap)
			{
				this.angleSnapped = true;
				base.transform.localRotation = rotationTarget;
			}
			else
			{
				base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, rotationTarget, this.lerpValue);
			}
		}
		if (!this.positionSnapped)
		{
			if ((base.transform.localPosition - positionTarget).magnitude < this.vectorLerpSnap)
			{
				this.positionSnapped = true;
				base.transform.localPosition = positionTarget;
				return;
			}
			base.transform.localPosition = Vector3.Lerp(base.transform.localPosition, positionTarget, this.lerpValue);
		}
	}

	// Token: 0x0600219D RID: 8605 RVA: 0x000B3654 File Offset: 0x000B1854
	public override bool CanDeactivate()
	{
		return !base.gameObject.activeSelf || this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x0600219E RID: 8606 RVA: 0x000B3677 File Offset: 0x000B1877
	public override bool CanActivate()
	{
		return this.itemState == TransferrableObject.ItemStates.State0 || this.itemState == TransferrableObject.ItemStates.State1;
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000B368D File Offset: 0x000B188D
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.itemState == TransferrableObject.ItemStates.State0)
		{
			this.itemState = TransferrableObject.ItemStates.State1;
			return;
		}
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000B36B0 File Offset: 0x000B18B0
	public void GenerateVectorOffsetLeft()
	{
		this.chestOffsetLeft = base.transform.position - this.chestColliderLeft.transform.position;
		this.holdingOffsetRotationLeft = Quaternion.LookRotation(base.transform.position - this.chestColliderLeft.transform.position);
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000B3710 File Offset: 0x000B1910
	public void GenerateVectorOffsetRight()
	{
		this.chestOffsetRight = base.transform.position - this.chestColliderRight.transform.position;
		this.holdingOffsetRotationRight = Quaternion.LookRotation(base.transform.position - this.chestColliderRight.transform.position);
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000B376E File Offset: 0x000B196E
	public void GenerateReverseGripOffsetLeft()
	{
		this.reverseGripPositionLeft = base.transform.localPosition;
		this.reverseGripQuatLeft = base.transform.localRotation;
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000B3792 File Offset: 0x000B1992
	public void GenerateClubOffsetLeft()
	{
		this.startPositionLeft = base.transform.localPosition;
		this.startQuatLeft = base.transform.localRotation;
	}

	// Token: 0x060021A4 RID: 8612 RVA: 0x000B37B6 File Offset: 0x000B19B6
	public void GenerateReverseGripOffsetRight()
	{
		this.reverseGripPositionRight = base.transform.localPosition;
		this.reverseGripQuatRight = base.transform.localRotation;
	}

	// Token: 0x060021A5 RID: 8613 RVA: 0x000B37DA File Offset: 0x000B19DA
	public void GenerateClubOffsetRight()
	{
		this.startPositionRight = base.transform.localPosition;
		this.startQuatRight = base.transform.localRotation;
	}

	// Token: 0x060021A6 RID: 8614 RVA: 0x000B37FE File Offset: 0x000B19FE
	public void TestClubPositionRight()
	{
		base.transform.localPosition = this.startPositionRight;
		base.transform.localRotation = this.startQuatRight;
	}

	// Token: 0x060021A7 RID: 8615 RVA: 0x000B3822 File Offset: 0x000B1A22
	public void TestReverseGripPositionRight()
	{
		base.transform.localPosition = this.reverseGripPositionRight;
		base.transform.localRotation = this.reverseGripQuatRight;
	}

	// Token: 0x060021A8 RID: 8616 RVA: 0x000B3848 File Offset: 0x000B1A48
	public void TestPlayingPositionRight()
	{
		base.transform.rotation = Quaternion.LookRotation(this.parentHand.position - this.currentChestCollider.transform.position) * this.holdingOffsetRotationRight;
		base.transform.position = this.chestColliderRight.transform.position + base.transform.rotation * this.chestOffsetRight;
	}

	// Token: 0x04002C3C RID: 11324
	public Vector3 chestOffsetLeft;

	// Token: 0x04002C3D RID: 11325
	public Vector3 chestOffsetRight;

	// Token: 0x04002C3E RID: 11326
	public Quaternion holdingOffsetRotationLeft;

	// Token: 0x04002C3F RID: 11327
	public Quaternion holdingOffsetRotationRight;

	// Token: 0x04002C40 RID: 11328
	public Quaternion chestRotationOffset;

	// Token: 0x04002C41 RID: 11329
	[NonSerialized]
	public Collider currentChestCollider;

	// Token: 0x04002C42 RID: 11330
	[NonSerialized]
	public Collider chestColliderLeft;

	// Token: 0x04002C43 RID: 11331
	[NonSerialized]
	public Collider chestColliderRight;

	// Token: 0x04002C44 RID: 11332
	public float lerpValue = 0.25f;

	// Token: 0x04002C45 RID: 11333
	public AudioSource audioSource;

	// Token: 0x04002C46 RID: 11334
	private Transform parentHand;

	// Token: 0x04002C47 RID: 11335
	private Transform parentHandLeft;

	// Token: 0x04002C48 RID: 11336
	private Transform parentHandRight;

	// Token: 0x04002C49 RID: 11337
	public float unsnapDistance;

	// Token: 0x04002C4A RID: 11338
	public float snapDistance;

	// Token: 0x04002C4B RID: 11339
	public Vector3 startPositionLeft;

	// Token: 0x04002C4C RID: 11340
	public Quaternion startQuatLeft;

	// Token: 0x04002C4D RID: 11341
	public Vector3 reverseGripPositionLeft;

	// Token: 0x04002C4E RID: 11342
	public Quaternion reverseGripQuatLeft;

	// Token: 0x04002C4F RID: 11343
	public Vector3 startPositionRight;

	// Token: 0x04002C50 RID: 11344
	public Quaternion startQuatRight;

	// Token: 0x04002C51 RID: 11345
	public Vector3 reverseGripPositionRight;

	// Token: 0x04002C52 RID: 11346
	public Quaternion reverseGripQuatRight;

	// Token: 0x04002C53 RID: 11347
	public float angleLerpSnap = 1f;

	// Token: 0x04002C54 RID: 11348
	public float vectorLerpSnap = 0.01f;

	// Token: 0x04002C55 RID: 11349
	private bool angleSnapped;

	// Token: 0x04002C56 RID: 11350
	private bool positionSnapped;

	// Token: 0x04002C57 RID: 11351
	public Transform chestTouch;

	// Token: 0x04002C58 RID: 11352
	private int collidersHitCount;

	// Token: 0x04002C59 RID: 11353
	private Collider[] collidersHit = new Collider[20];

	// Token: 0x04002C5A RID: 11354
	private RaycastHit[] raycastHits = new RaycastHit[20];

	// Token: 0x04002C5B RID: 11355
	private List<RaycastHit> raycastHitList = new List<RaycastHit>();

	// Token: 0x04002C5C RID: 11356
	private RaycastHit nullHit;

	// Token: 0x04002C5D RID: 11357
	public Collider[] collidersToBeIn;

	// Token: 0x04002C5E RID: 11358
	public LayerMask interactableMask;

	// Token: 0x04002C5F RID: 11359
	public int currentFretIndex;

	// Token: 0x04002C60 RID: 11360
	public int lastFretIndex;

	// Token: 0x04002C61 RID: 11361
	public Collider[] frets;

	// Token: 0x04002C62 RID: 11362
	private List<Collider> fretsList = new List<Collider>();

	// Token: 0x04002C63 RID: 11363
	public AudioClip[] audioClips;

	// Token: 0x04002C64 RID: 11364
	private GorillaTriggerColliderHandIndicator leftHandIndicator;

	// Token: 0x04002C65 RID: 11365
	private GorillaTriggerColliderHandIndicator rightHandIndicator;

	// Token: 0x04002C66 RID: 11366
	private GorillaTriggerColliderHandIndicator fretHandIndicator;

	// Token: 0x04002C67 RID: 11367
	private GorillaTriggerColliderHandIndicator strumHandIndicator;

	// Token: 0x04002C68 RID: 11368
	private float sphereRadius;

	// Token: 0x04002C69 RID: 11369
	private bool anyHit;

	// Token: 0x04002C6A RID: 11370
	private bool handIn;

	// Token: 0x04002C6B RID: 11371
	private Vector3 spherecastSweep;

	// Token: 0x04002C6C RID: 11372
	public Collider strumCollider;

	// Token: 0x04002C6D RID: 11373
	public float maxVolume = 1f;

	// Token: 0x04002C6E RID: 11374
	public float minVolume = 0.05f;

	// Token: 0x04002C6F RID: 11375
	public float maxVelocity = 2f;

	// Token: 0x04002C70 RID: 11376
	private List<Collider> strumList = new List<Collider>();

	// Token: 0x04002C71 RID: 11377
	private int selfInstrumentIndex = -1;

	// Token: 0x04002C72 RID: 11378
	private OneStringGuitar.GuitarStates lastState;

	// Token: 0x04002C73 RID: 11379
	private Vector3 startingLeftChestOffset;

	// Token: 0x04002C74 RID: 11380
	private Vector3 startingRightChestOffset;

	// Token: 0x04002C75 RID: 11381
	private float startingUnsnapDistance;

	// Token: 0x02000533 RID: 1331
	private enum GuitarStates
	{
		// Token: 0x04002C77 RID: 11383
		Club = 1,
		// Token: 0x04002C78 RID: 11384
		HeldReverseGrip,
		// Token: 0x04002C79 RID: 11385
		Playing = 4
	}
}
