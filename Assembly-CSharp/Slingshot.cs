using System;
using GorillaLocomotion;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020004B5 RID: 1205
public class Slingshot : ProjectileWeapon
{
	// Token: 0x06001D5F RID: 7519 RVA: 0x0009EAAC File Offset: 0x0009CCAC
	private void DestroyDummyProjectile()
	{
		if (this.hasDummyProjectile)
		{
			this.dummyProjectile.transform.localScale = Vector3.one * this.dummyProjectileInitialScale;
			this.dummyProjectile.GetComponent<SphereCollider>().enabled = true;
			ObjectPools.instance.Destroy(this.dummyProjectile);
			this.dummyProjectile = null;
			this.hasDummyProjectile = false;
		}
	}

	// Token: 0x06001D60 RID: 7520 RVA: 0x0009EB10 File Offset: 0x0009CD10
	protected override void Awake()
	{
		base.Awake();
		if (this.elasticLeft)
		{
			this._elasticIntialWidthMultiplier = this.elasticLeft.widthMultiplier;
		}
	}

	// Token: 0x06001D61 RID: 7521 RVA: 0x0009EB36 File Offset: 0x0009CD36
	public override void OnSpawn(VRRig rig)
	{
		base.OnSpawn(rig);
		this.myRig = rig;
		this.OnEnable();
	}

	// Token: 0x06001D62 RID: 7522 RVA: 0x0009EB4C File Offset: 0x0009CD4C
	internal override void OnEnable()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		this.leftHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapLeft).transform;
		this.rightHandSnap = this.myRig.cosmeticReferences.Get(CosmeticRefID.SlingshotSnapRight).transform;
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
		if (this.elasticLeft)
		{
			this.elasticLeft.positionCount = 2;
		}
		if (this.elasticRight)
		{
			this.elasticRight.positionCount = 2;
		}
		this.dummyProjectile = null;
		base.OnEnable();
	}

	// Token: 0x06001D63 RID: 7523 RVA: 0x0009EBE8 File Offset: 0x0009CDE8
	internal override void OnDisable()
	{
		this.DestroyDummyProjectile();
		base.OnDisable();
	}

	// Token: 0x06001D64 RID: 7524 RVA: 0x0009EBF8 File Offset: 0x0009CDF8
	protected override void LateUpdateShared()
	{
		if (!base.IsSpawned)
		{
			return;
		}
		base.LateUpdateShared();
		float num = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 vector;
		if (this.InDrawingState())
		{
			if (!this.hasDummyProjectile)
			{
				this.dummyProjectile = ObjectPools.instance.Instantiate(this.projectilePrefab, true);
				this.hasDummyProjectile = true;
				SphereCollider component = this.dummyProjectile.GetComponent<SphereCollider>();
				component.enabled = false;
				this.dummyProjectileColliderRadius = component.radius;
				this.dummyProjectileInitialScale = this.dummyProjectile.transform.localScale.x;
				bool blueTeam;
				bool orangeTeam;
				bool flag;
				base.GetIsOnTeams(out blueTeam, out orangeTeam, out flag);
				this.dummyProjectile.GetComponent<SlingshotProjectile>().ApplyTeamModelAndColor(blueTeam, orangeTeam, flag && this.targetRig, this.targetRig ? this.targetRig.playerColor : default(Color));
			}
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(false);
			}
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(false);
			}
			float d = this.dummyProjectileInitialScale * num;
			this.dummyProjectile.transform.localScale = Vector3.one * d;
			Vector3 position = this.drawingHand.transform.position;
			Vector3 position2 = this.centerOrigin.position;
			Vector3 normalized = (position2 - position).normalized;
			float d2 = (EquipmentInteractor.instance.grabRadius - this.dummyProjectileColliderRadius) * num;
			vector = position + normalized * d2;
			this.dummyProjectile.transform.position = vector;
			this.dummyProjectile.transform.rotation = Quaternion.LookRotation(position2 - vector, Vector3.up);
			if (!this.wasStretching)
			{
				UnityEvent<bool> stretchStartShared = this.StretchStartShared;
				if (stretchStartShared != null)
				{
					stretchStartShared.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretching = true;
			}
		}
		else
		{
			this.DestroyDummyProjectile();
			if (this.disableInDraw != null)
			{
				this.disableInDraw.SetActive(true);
			}
			vector = this.centerOrigin.position;
			if (this.wasStretching)
			{
				UnityEvent<bool> stretchEndShared = this.StretchEndShared;
				if (stretchEndShared != null)
				{
					stretchEndShared.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretching = false;
			}
		}
		this.center.position = vector;
		if (!this.disableLineRenderer)
		{
			this.elasticLeftPoints[0] = this.leftArm.position;
			this.elasticLeftPoints[1] = (this.elasticRightPoints[1] = vector);
			this.elasticRightPoints[0] = this.rightArm.position;
			this.elasticLeft.SetPositions(this.elasticLeftPoints);
			this.elasticRight.SetPositions(this.elasticRightPoints);
			this.elasticLeft.widthMultiplier = this._elasticIntialWidthMultiplier * num;
			this.elasticRight.widthMultiplier = this._elasticIntialWidthMultiplier * num;
		}
		if (!NetworkSystem.Instance.InRoom && this.disableWhenNotInRoom)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06001D65 RID: 7525 RVA: 0x0009EF20 File Offset: 0x0009D120
	protected override void LateUpdateLocal()
	{
		base.LateUpdateLocal();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = EquipmentInteractor.instance.rightHand;
			}
			else
			{
				this.drawingHand = EquipmentInteractor.instance.leftHand;
			}
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), this.hapticsStrength, this.hapticsLength);
			if (!this.wasStretchingLocal)
			{
				UnityEvent<bool> stretchStartLocal = this.StretchStartLocal;
				if (stretchStartLocal != null)
				{
					stretchStartLocal.Invoke(!this.ForLeftHandSlingshot());
				}
				this.wasStretchingLocal = true;
				return;
			}
		}
		else if (this.wasStretchingLocal)
		{
			UnityEvent<bool> stretchEndLocal = this.StretchEndLocal;
			if (stretchEndLocal != null)
			{
				stretchEndLocal.Invoke(!this.ForLeftHandSlingshot());
			}
			this.wasStretchingLocal = false;
		}
	}

	// Token: 0x06001D66 RID: 7526 RVA: 0x0009EFDB File Offset: 0x0009D1DB
	protected override void LateUpdateReplicated()
	{
		base.LateUpdateReplicated();
		if (this.InDrawingState())
		{
			if (this.ForLeftHandSlingshot())
			{
				this.drawingHand = this.rightHandSnap.gameObject;
				return;
			}
			this.drawingHand = this.leftHandSnap.gameObject;
		}
	}

	// Token: 0x06001D67 RID: 7527 RVA: 0x0009F016 File Offset: 0x0009D216
	public static bool IsSlingShotEnabled()
	{
		return !(GorillaTagger.Instance == null) && !(GorillaTagger.Instance.offlineVRRig == null) && GorillaTagger.Instance.offlineVRRig.cosmeticSet.HasItemOfCategory(CosmeticsController.CosmeticCategory.Chest);
	}

	// Token: 0x06001D68 RID: 7528 RVA: 0x0009F050 File Offset: 0x0009D250
	public override void OnGrab(InteractionPoint pointGrabbed, GameObject grabbingHand)
	{
		if (!this.IsMyItem())
		{
			return;
		}
		bool flag = pointGrabbed == this.nock;
		if (flag && !base.InHand())
		{
			return;
		}
		base.OnGrab(pointGrabbed, grabbingHand);
		if (this.InDrawingState() || base.OnChest())
		{
			return;
		}
		if (flag)
		{
			if (grabbingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = true;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = true;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.itemState = TransferrableObject.ItemStates.State2;
			}
			else
			{
				this.itemState = TransferrableObject.ItemStates.State3;
			}
			this.minTimeToLaunch = Time.time + this.delayLaunchTime;
			GorillaTagger.Instance.StartVibration(!this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength / 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
		}
	}

	// Token: 0x06001D69 RID: 7529 RVA: 0x0009F12C File Offset: 0x0009D32C
	public override bool OnRelease(DropZone zoneReleased, GameObject releasingHand)
	{
		base.OnRelease(zoneReleased, releasingHand);
		if (this.InDrawingState() && releasingHand == this.drawingHand)
		{
			if (releasingHand == EquipmentInteractor.instance.leftHand)
			{
				EquipmentInteractor.instance.disableLeftGrab = false;
			}
			else
			{
				EquipmentInteractor.instance.disableRightGrab = false;
			}
			if (this.ForLeftHandSlingshot())
			{
				this.currentState = TransferrableObject.PositionState.InLeftHand;
			}
			else
			{
				this.currentState = TransferrableObject.PositionState.InRightHand;
			}
			this.itemState = TransferrableObject.ItemStates.State0;
			GorillaTagger.Instance.StartVibration(this.ForLeftHandSlingshot(), GorillaTagger.Instance.tapHapticStrength * 2f, GorillaTagger.Instance.tapHapticDuration * 1.5f);
			if (Time.time > this.minTimeToLaunch && (releasingHand.transform.position - this.centerOrigin.transform.position).sqrMagnitude > this.minDrawDistanceToRelease * this.minDrawDistanceToRelease)
			{
				base.LaunchProjectile();
			}
		}
		else
		{
			EquipmentInteractor.instance.disableLeftGrab = false;
			EquipmentInteractor.instance.disableRightGrab = false;
		}
		return true;
	}

	// Token: 0x06001D6A RID: 7530 RVA: 0x0009F244 File Offset: 0x0009D444
	public override void DropItemCleanup()
	{
		base.DropItemCleanup();
		this.currentState = TransferrableObject.PositionState.OnChest;
		this.itemState = TransferrableObject.ItemStates.State0;
	}

	// Token: 0x06001D6B RID: 7531 RVA: 0x00023994 File Offset: 0x00021B94
	public override bool AutoGrabTrue(bool leftGrabbingHand)
	{
		return true;
	}

	// Token: 0x06001D6C RID: 7532 RVA: 0x0009F25B File Offset: 0x0009D45B
	private bool ForLeftHandSlingshot()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.currentState == TransferrableObject.PositionState.InLeftHand;
	}

	// Token: 0x06001D6D RID: 7533 RVA: 0x0009F271 File Offset: 0x0009D471
	private bool InDrawingState()
	{
		return this.itemState == TransferrableObject.ItemStates.State2 || this.itemState == TransferrableObject.ItemStates.State3;
	}

	// Token: 0x06001D6E RID: 7534 RVA: 0x0009F287 File Offset: 0x0009D487
	protected override Vector3 GetLaunchPosition()
	{
		return this.dummyProjectile.transform.position;
	}

	// Token: 0x06001D6F RID: 7535 RVA: 0x0009F29C File Offset: 0x0009D49C
	protected override Vector3 GetLaunchVelocity()
	{
		float d = Mathf.Abs(base.transform.lossyScale.x);
		Vector3 a = this.centerOrigin.position - this.center.position;
		a /= d;
		Vector3 a2 = Mathf.Min(this.springConstant * this.maxDraw, a.magnitude * this.springConstant) * a.normalized * d;
		Vector3 averagedVelocity = GTPlayer.Instance.AveragedVelocity;
		return a2 + averagedVelocity;
	}

	// Token: 0x04002793 RID: 10131
	[SerializeField]
	private bool disableLineRenderer;

	// Token: 0x04002794 RID: 10132
	[FormerlySerializedAs("elastic")]
	public LineRenderer elasticLeft;

	// Token: 0x04002795 RID: 10133
	public LineRenderer elasticRight;

	// Token: 0x04002796 RID: 10134
	public Transform leftArm;

	// Token: 0x04002797 RID: 10135
	public Transform rightArm;

	// Token: 0x04002798 RID: 10136
	public Transform center;

	// Token: 0x04002799 RID: 10137
	public Transform centerOrigin;

	// Token: 0x0400279A RID: 10138
	private GameObject dummyProjectile;

	// Token: 0x0400279B RID: 10139
	public GameObject drawingHand;

	// Token: 0x0400279C RID: 10140
	public InteractionPoint nock;

	// Token: 0x0400279D RID: 10141
	public InteractionPoint grip;

	// Token: 0x0400279E RID: 10142
	public float springConstant;

	// Token: 0x0400279F RID: 10143
	public float maxDraw;

	// Token: 0x040027A0 RID: 10144
	[SerializeField]
	private GameObject disableInDraw;

	// Token: 0x040027A1 RID: 10145
	[SerializeField]
	private float minDrawDistanceToRelease;

	// Token: 0x040027A2 RID: 10146
	[Header("Stretching Haptics")]
	[Space]
	[SerializeField]
	private bool playStretchingHaptics;

	// Token: 0x040027A3 RID: 10147
	[SerializeField]
	private float hapticsStrength = 0.1f;

	// Token: 0x040027A4 RID: 10148
	[SerializeField]
	private float hapticsLength = 0.1f;

	// Token: 0x040027A5 RID: 10149
	[Header("Stretching Events")]
	[Space]
	public UnityEvent<bool> StretchStartShared;

	// Token: 0x040027A6 RID: 10150
	public UnityEvent<bool> StretchEndShared;

	// Token: 0x040027A7 RID: 10151
	[Space]
	public UnityEvent<bool> StretchStartLocal;

	// Token: 0x040027A8 RID: 10152
	public UnityEvent<bool> StretchEndLocal;

	// Token: 0x040027A9 RID: 10153
	private bool wasStretching;

	// Token: 0x040027AA RID: 10154
	private bool wasStretchingLocal;

	// Token: 0x040027AB RID: 10155
	private Transform leftHandSnap;

	// Token: 0x040027AC RID: 10156
	private Transform rightHandSnap;

	// Token: 0x040027AD RID: 10157
	public bool disableWhenNotInRoom;

	// Token: 0x040027AE RID: 10158
	private bool hasDummyProjectile;

	// Token: 0x040027AF RID: 10159
	private float delayLaunchTime = 0.07f;

	// Token: 0x040027B0 RID: 10160
	private float minTimeToLaunch = -1f;

	// Token: 0x040027B1 RID: 10161
	private float dummyProjectileColliderRadius;

	// Token: 0x040027B2 RID: 10162
	private float dummyProjectileInitialScale;

	// Token: 0x040027B3 RID: 10163
	private int projectileCount;

	// Token: 0x040027B4 RID: 10164
	private Vector3[] elasticLeftPoints = new Vector3[2];

	// Token: 0x040027B5 RID: 10165
	private Vector3[] elasticRightPoints = new Vector3[2];

	// Token: 0x040027B6 RID: 10166
	private float _elasticIntialWidthMultiplier;

	// Token: 0x040027B7 RID: 10167
	private new VRRig myRig;

	// Token: 0x020004B6 RID: 1206
	public enum SlingshotState
	{
		// Token: 0x040027B9 RID: 10169
		NoState = 1,
		// Token: 0x040027BA RID: 10170
		OnChest,
		// Token: 0x040027BB RID: 10171
		LeftHandDrawing = 4,
		// Token: 0x040027BC RID: 10172
		RightHandDrawing = 8
	}

	// Token: 0x020004B7 RID: 1207
	public enum SlingshotActions
	{
		// Token: 0x040027BE RID: 10174
		Grab,
		// Token: 0x040027BF RID: 10175
		Release
	}
}
