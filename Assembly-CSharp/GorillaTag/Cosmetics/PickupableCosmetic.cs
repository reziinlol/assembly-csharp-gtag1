using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GorillaTag.Cosmetics
{
	// Token: 0x02001235 RID: 4661
	public class PickupableCosmetic : PickupableVariant
	{
		// Token: 0x0600749A RID: 29850 RVA: 0x00262F88 File Offset: 0x00261188
		private void Awake()
		{
			this.rigOwnedPhysicsBody = base.GetComponent<RigOwnedPhysicsBody>();
			this.bodyCollider = base.GetComponent<Collider>();
		}

		// Token: 0x0600749B RID: 29851 RVA: 0x002481B3 File Offset: 0x002463B3
		private void Start()
		{
			base.enabled = false;
		}

		// Token: 0x0600749C RID: 29852 RVA: 0x00262FA2 File Offset: 0x002611A2
		private void OnEnable()
		{
			if (this.rigOwnedPhysicsBody != null)
			{
				this.rigOwnedPhysicsBody.enabled = true;
			}
		}

		// Token: 0x0600749D RID: 29853 RVA: 0x00262FBE File Offset: 0x002611BE
		private void OnDisable()
		{
			if (this.rigOwnedPhysicsBody != null)
			{
				this.rigOwnedPhysicsBody.enabled = false;
			}
		}

		// Token: 0x0600749E RID: 29854 RVA: 0x00262FDC File Offset: 0x002611DC
		protected internal override void Pickup(bool isAutoPickup = false)
		{
			if (!isAutoPickup)
			{
				UnityEvent onPickupShared = this.OnPickupShared;
				if (onPickupShared != null)
				{
					onPickupShared.Invoke();
				}
			}
			this.rb.linearVelocity = Vector3.zero;
			this.rb.isKinematic = true;
			if (this.holdableParent != null)
			{
				base.transform.parent = this.holdableParent.transform;
			}
			base.transform.localPosition = Vector3.zero;
			base.transform.localRotation = Quaternion.identity;
			base.transform.localScale = Vector3.one;
			this.scale = 1f;
			this.placedOnFloorTime = -1f;
			this.placedOnFloor = false;
			this.broken = false;
			this.brokenTime = -1f;
			if (this.isBreakable && this.transferrableParent != null && this.transferrableParent.IsLocalObject())
			{
				int num = (int)this.transferrableParent.itemState;
				num &= ~PickupableCosmetic.breakableBitmask;
				this.transferrableParent.itemState = (TransferrableObject.ItemStates)num;
				if (this.breakEffect != null && this.breakEffect.isPlaying)
				{
					this.breakEffect.Stop();
				}
			}
			this.ShowRenderers(true);
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = true;
			}
			base.enabled = false;
		}

		// Token: 0x0600749F RID: 29855 RVA: 0x0026312E File Offset: 0x0026132E
		protected internal override void DelayedPickup()
		{
			this.DelayedPickup_Internal();
		}

		// Token: 0x060074A0 RID: 29856 RVA: 0x00263138 File Offset: 0x00261338
		private void DelayedPickup_Internal()
		{
			PickupableCosmetic.<DelayedPickup_Internal>d__45 <DelayedPickup_Internal>d__;
			<DelayedPickup_Internal>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<DelayedPickup_Internal>d__.<>4__this = this;
			<DelayedPickup_Internal>d__.<>1__state = -1;
			<DelayedPickup_Internal>d__.<>t__builder.Start<PickupableCosmetic.<DelayedPickup_Internal>d__45>(ref <DelayedPickup_Internal>d__);
		}

		// Token: 0x060074A1 RID: 29857 RVA: 0x00263170 File Offset: 0x00261370
		protected internal override void Release(HoldableObject holdable, Vector3 startPosition, Vector3 velocity, float playerScale)
		{
			this.holdableParent = holdable;
			base.transform.parent = null;
			base.transform.position = startPosition;
			base.transform.localScale = Vector3.one * playerScale;
			this.rb.isKinematic = false;
			this.rb.useGravity = true;
			this.rb.linearVelocity = velocity;
			this.rb.detectCollisions = true;
			if (!this.allowPickupFromGround && this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.scale = playerScale;
			base.enabled = true;
			this.transferrableParent = (this.holdableParent as TransferrableObject);
			this.currentRayIndex = 0;
			this.frameCounter = 0;
		}

		// Token: 0x060074A2 RID: 29858 RVA: 0x00263234 File Offset: 0x00261434
		private void FixedUpdate()
		{
			if (this.isBreakable && this.broken)
			{
				if (Time.time > this.respawnDelay + this.brokenTime)
				{
					this.Pickup(false);
				}
				return;
			}
			if (this.isBreakable && this.placedOnFloor)
			{
				bool flag = (this.transferrableParent.itemState & (TransferrableObject.ItemStates)PickupableCosmetic.breakableBitmask) > (TransferrableObject.ItemStates)0;
				if (flag != this.broken && flag)
				{
					this.OnBreakReplicated();
				}
			}
			if (this.autoPickupAfterSeconds > 0f && this.placedOnFloor && Time.time - this.placedOnFloorTime > this.autoPickupAfterSeconds)
			{
				this.Pickup(true);
				ThrowablePickupableCosmetic throwablePickupableCosmetic = this.transferrableParent as ThrowablePickupableCosmetic;
				if (throwablePickupableCosmetic)
				{
					UnityEvent onReturnToDockPositionShared = throwablePickupableCosmetic.OnReturnToDockPositionShared;
					if (onReturnToDockPositionShared != null)
					{
						onReturnToDockPositionShared.Invoke();
					}
				}
			}
			if (this.autoPickupDistance > 0f && this.transferrableParent != null && (this.transferrableParent.ownerRig.transform.position - base.transform.position).IsLongerThan(this.autoPickupDistance))
			{
				this.Pickup(false);
			}
			if (!this.placedOnFloor && base.enabled)
			{
				this.frameCounter++;
				if (this.frameCounter % this.stepEveryNFrames != 0)
				{
					return;
				}
				float maxDistance = this.RaycastCheckDist * this.scale;
				int value = this.floorLayerMask.value;
				Vector3[] cachedDirections = this.GetCachedDirections(this.RaycastChecksMax);
				int num = 0;
				while (num < this.raysPerStep && this.currentRayIndex < cachedDirections.Length)
				{
					Vector3 vector = cachedDirections[this.currentRayIndex];
					this.currentRayIndex++;
					num++;
					RaycastHit hitInfo;
					if (Physics.Raycast(this.GetSafeRayOrigin(this.raycastOrigin.position, vector), vector, out hitInfo, maxDistance, value, QueryTriggerInteraction.Ignore) && (!this.dontStickToWall || Vector3.Angle(hitInfo.normal, Vector3.up) < 40f))
					{
						this.SettleBanner(hitInfo);
						UnityEvent onPlacedShared = this.OnPlacedShared;
						if (onPlacedShared != null)
						{
							onPlacedShared.Invoke();
						}
						this.placedOnFloor = true;
						this.placedOnFloorTime = Time.time;
						break;
					}
				}
				if (this.currentRayIndex >= cachedDirections.Length)
				{
					this.currentRayIndex = 0;
				}
			}
		}

		// Token: 0x060074A3 RID: 29859 RVA: 0x00263474 File Offset: 0x00261674
		private void SettleBanner(RaycastHit hitInfo)
		{
			this.rb.isKinematic = true;
			this.rb.useGravity = false;
			this.rb.detectCollisions = false;
			Vector3 normal = hitInfo.normal;
			base.transform.position = hitInfo.point + normal * this.placementOffset;
			Quaternion rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, normal).normalized, normal);
			base.transform.rotation = rotation;
		}

		// Token: 0x060074A4 RID: 29860 RVA: 0x002634FC File Offset: 0x002616FC
		private Vector3 GetFibonacciSphereDirection(int index, int total)
		{
			float f = Mathf.Acos(1f - 2f * ((float)index + 0.5f) / (float)total);
			float f2 = 3.1415927f * (1f + Mathf.Sqrt(5f)) * ((float)index + 0.5f);
			float x = Mathf.Sin(f) * Mathf.Cos(f2);
			float y = Mathf.Sin(f) * Mathf.Sin(f2);
			float z = Mathf.Cos(f);
			return new Vector3(x, y, z).normalized;
		}

		// Token: 0x060074A5 RID: 29861 RVA: 0x00263578 File Offset: 0x00261778
		private Vector3[] GetCachedDirections(int count)
		{
			if (count <= 0)
			{
				return PickupableCosmetic.tmpEmpty;
			}
			Vector3[] array;
			if (PickupableCosmetic.directionCache.TryGetValue(count, out array))
			{
				return array;
			}
			array = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = this.GetFibonacciSphereDirection(i, count);
			}
			PickupableCosmetic.directionCache[count] = array;
			return array;
		}

		// Token: 0x060074A6 RID: 29862 RVA: 0x002635D0 File Offset: 0x002617D0
		private Vector3 GetSafeRayOrigin(Vector3 rawOrigin, Vector3 dir)
		{
			float d = this.selfSkinOffset;
			if (this.bodyCollider != null)
			{
				float magnitude = this.bodyCollider.bounds.extents.magnitude;
				d = Mathf.Max(this.selfSkinOffset, magnitude * 0.05f);
			}
			return rawOrigin - dir.normalized * d;
		}

		// Token: 0x060074A7 RID: 29863 RVA: 0x00263634 File Offset: 0x00261834
		public void BreakPlaceable()
		{
			if (!this.isBreakable || !this.placedOnFloor)
			{
				return;
			}
			if (this.transferrableParent != null && this.transferrableParent.IsLocalObject())
			{
				int num = (int)this.transferrableParent.itemState;
				num |= PickupableCosmetic.breakableBitmask;
				this.transferrableParent.itemState = (TransferrableObject.ItemStates)num;
				return;
			}
			GTDev.LogError<string>("PickupableCosmetic " + base.gameObject.name + " has no TransferrableObject parent. Break effects cannot be replicated", null);
		}

		// Token: 0x060074A8 RID: 29864 RVA: 0x002636AE File Offset: 0x002618AE
		private void OnBreakReplicated()
		{
			this.PlayBreakEffects();
		}

		// Token: 0x060074A9 RID: 29865 RVA: 0x002636B8 File Offset: 0x002618B8
		protected virtual void PlayBreakEffects()
		{
			if (!this.isBreakable || !this.placedOnFloor || this.broken)
			{
				return;
			}
			this.broken = true;
			this.brokenTime = Time.time;
			if (this.breakEffect != null)
			{
				if (this.breakEffect.isPlaying)
				{
					this.breakEffect.Stop();
				}
				this.breakEffect.Play();
			}
			if (this.interactionPoint != null)
			{
				this.interactionPoint.enabled = false;
			}
			this.ShowRenderers(false);
			UnityEvent onBrokenShared = this.OnBrokenShared;
			if (onBrokenShared == null)
			{
				return;
			}
			onBrokenShared.Invoke();
		}

		// Token: 0x060074AA RID: 29866 RVA: 0x00263754 File Offset: 0x00261954
		protected virtual void ShowRenderers(bool visible)
		{
			if (this.hideOnBreak.IsNullOrEmpty<Renderer>())
			{
				return;
			}
			for (int i = 0; i < this.hideOnBreak.Length; i++)
			{
				Renderer renderer = this.hideOnBreak[i];
				if (!(renderer == null))
				{
					renderer.forceRenderingOff = !visible;
				}
			}
		}

		// Token: 0x040085EB RID: 34283
		[SerializeField]
		private InteractionPoint interactionPoint;

		// Token: 0x040085EC RID: 34284
		[SerializeField]
		private Rigidbody rb;

		// Token: 0x040085ED RID: 34285
		[SerializeField]
		private Transform raycastOrigin;

		// Token: 0x040085EE RID: 34286
		[Tooltip("Allow player to grab the placed object")]
		[SerializeField]
		private bool allowPickupFromGround = true;

		// Token: 0x040085EF RID: 34287
		[SerializeField]
		private float autoPickupAfterSeconds;

		// Token: 0x040085F0 RID: 34288
		[SerializeField]
		private float autoPickupDistance;

		// Token: 0x040085F1 RID: 34289
		[Tooltip("Amount to offset the placed object from the hit position in the hit normal direction")]
		[SerializeField]
		private float placementOffset;

		// Token: 0x040085F2 RID: 34290
		[Tooltip("Prevent sticking if the hit surface normal is not within 40 degrees of world up")]
		[SerializeField]
		private bool dontStickToWall;

		// Token: 0x040085F3 RID: 34291
		[Tooltip("Layers to raycast against for placement")]
		[SerializeField]
		private LayerMask floorLayerMask = 134218241;

		// Token: 0x040085F4 RID: 34292
		[Tooltip("The distance to check if the banner is close to the floor (from a raycast check).")]
		public float RaycastCheckDist = 0.2f;

		// Token: 0x040085F5 RID: 34293
		[Tooltip("How many checks should we attempt for a raycast.")]
		public int RaycastChecksMax = 12;

		// Token: 0x040085F6 RID: 34294
		[FormerlySerializedAs("OnPickup")]
		[Space]
		public UnityEvent OnPickupShared;

		// Token: 0x040085F7 RID: 34295
		[FormerlySerializedAs("OnPlaced")]
		public UnityEvent OnPlacedShared;

		// Token: 0x040085F8 RID: 34296
		[SerializeField]
		private bool isBreakable;

		// Token: 0x040085F9 RID: 34297
		[Tooltip("Particle system played OnBrokenShared")]
		[SerializeField]
		private ParticleSystem breakEffect;

		// Token: 0x040085FA RID: 34298
		[Tooltip("Renderers disabled OnBrokenShared and enabled OnPickupShared")]
		[SerializeField]
		private Renderer[] hideOnBreak = new Renderer[0];

		// Token: 0x040085FB RID: 34299
		[Tooltip("Time after BreakPlaceable to reset item")]
		[SerializeField]
		private float respawnDelay = 0.5f;

		// Token: 0x040085FC RID: 34300
		[FormerlySerializedAs("OnBroken")]
		[Space]
		public UnityEvent OnBrokenShared;

		// Token: 0x040085FD RID: 34301
		private static int breakableBitmask = 32;

		// Token: 0x040085FE RID: 34302
		private bool placedOnFloor;

		// Token: 0x040085FF RID: 34303
		private float placedOnFloorTime = -1f;

		// Token: 0x04008600 RID: 34304
		private bool broken;

		// Token: 0x04008601 RID: 34305
		private float brokenTime = -1f;

		// Token: 0x04008602 RID: 34306
		private VRRig cachedLocalRig;

		// Token: 0x04008603 RID: 34307
		private HoldableObject holdableParent;

		// Token: 0x04008604 RID: 34308
		private TransferrableObject transferrableParent;

		// Token: 0x04008605 RID: 34309
		private RigOwnedPhysicsBody rigOwnedPhysicsBody;

		// Token: 0x04008606 RID: 34310
		private double throwSettledTime = -1.0;

		// Token: 0x04008607 RID: 34311
		private int landingSide;

		// Token: 0x04008608 RID: 34312
		private float scale;

		// Token: 0x04008609 RID: 34313
		private Collider bodyCollider;

		// Token: 0x0400860A RID: 34314
		[Tooltip("How many directions to test per physics tick (spreads work across frames).")]
		[SerializeField]
		[Min(1f)]
		private int raysPerStep = 3;

		// Token: 0x0400860B RID: 34315
		[Tooltip("Run a raycast step only every N physics ticks (1 = every FixedUpdate).")]
		[SerializeField]
		[Min(1f)]
		private int stepEveryNFrames = 2;

		// Token: 0x0400860C RID: 34316
		[Tooltip("Small skin so rays start just outside our own collider volume.")]
		[SerializeField]
		[Range(0.005f, 0.1f)]
		private float selfSkinOffset = 0.02f;

		// Token: 0x0400860D RID: 34317
		[SerializeField]
		private bool debugPlacementRays;

		// Token: 0x0400860E RID: 34318
		private int currentRayIndex;

		// Token: 0x0400860F RID: 34319
		private int frameCounter;

		// Token: 0x04008610 RID: 34320
		private static readonly Dictionary<int, Vector3[]> directionCache = new Dictionary<int, Vector3[]>();

		// Token: 0x04008611 RID: 34321
		private static readonly Vector3[] tmpEmpty = Array.Empty<Vector3>();
	}
}
