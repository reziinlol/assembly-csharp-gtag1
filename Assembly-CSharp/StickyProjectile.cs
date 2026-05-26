using System;
using GorillaLocomotion.Swimming;
using GorillaTag.Cosmetics;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000592 RID: 1426
public class StickyProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x06002418 RID: 9240 RVA: 0x000C1C18 File Offset: 0x000BFE18
	private void Awake()
	{
		this.stickyPart.GetLocalPositionAndRotation(out this.stickyPartLocalPosition, out this.stickyPartLocalRotation);
		this.stickyPartLocalScale = this.stickyPart.localScale;
		this.headZoneInversePosition = this.INVERSE_HEAD_ROTATION * this.headZonePosition;
		this.headZoneInverseLocalPosition = this.INVERSE_HEAD_ROTATION * this.localHeadZonePosition;
		this.rb = base.GetComponent<Rigidbody>();
		this.rbwi = base.GetComponent<RigidbodyWaterInteraction>();
		this.collider = base.GetComponent<Collider>();
		this.pcc = base.GetComponent<PlayerColoredCosmetic>();
		this.triggerLayer = LayerMask.NameToLayer("Gorilla Tag Collider");
		UnityEvent onReset = this.OnReset;
		if (onReset == null)
		{
			return;
		}
		onReset.Invoke();
	}

	// Token: 0x06002419 RID: 9241 RVA: 0x000C1CCC File Offset: 0x000BFECC
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		UnityEvent onLaunch = this.OnLaunch;
		if (onLaunch != null)
		{
			onLaunch.Invoke();
		}
		this.stickyPart.SetParent(base.transform, false);
		this.stickyPart.SetLocalPositionAndRotation(this.stickyPartLocalPosition, this.stickyPartLocalRotation);
		this.stickyPart.localScale = this.stickyPartLocalScale;
		base.transform.SetPositionAndRotation(startPosition, startRotation);
		base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		this.rb.isKinematic = false;
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.rb.linearVelocity = velocity;
		if (this.faceVelocityWhileAirborne)
		{
			TickSystem<object>.AddTickCallback(this);
			this.rb.angularVelocity = Vector3.zero;
		}
		else
		{
			this.rb.angularVelocity = Random.onUnitSphere * Random.Range(this.launchRandomSpinSpeedMinMax.x, this.launchRandomSpinSpeedMinMax.y);
		}
		this.rbwi.enabled = true;
		this.collider.enabled = true;
		if (this.pcc != null)
		{
			this.pcc.UpdateColor(ownerRig.playerColor);
		}
	}

	// Token: 0x0600241A RID: 9242 RVA: 0x000C1E08 File Offset: 0x000C0008
	private void StickTo(Transform otherTransform, Vector3 position, Quaternion rotation)
	{
		this.stickyPart.parent = otherTransform;
		this.stickyPart.SetPositionAndRotation(position + rotation * this.stickyPartLocalPosition, rotation * this.stickyPartLocalRotation);
		this.rb.isKinematic = true;
		this.rbwi.enabled = false;
		this.collider.enabled = false;
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000C1E70 File Offset: 0x000C0070
	private void OnCollisionEnter(Collision collision)
	{
		TickSystem<object>.RemoveTickCallback(this);
		ContactPoint contact = collision.GetContact(0);
		this.StickTo(collision.transform, contact.point, this.alignToHitNormal ? Quaternion.LookRotation(contact.normal, Random.onUnitSphere) : base.transform.rotation);
		this.stickEvents.InvokeAll(StickyProjectile.StickFlags.Wall, false);
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000C1ED4 File Offset: 0x000C00D4
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != this.triggerLayer)
		{
			return;
		}
		TickSystem<object>.RemoveTickCallback(this);
		Vector3 vector = Time.fixedDeltaTime * 2f * this.rb.linearVelocity;
		Vector3 vector2 = base.transform.position - vector;
		Vector3 vector3;
		Quaternion rotation;
		if (this.alignToHitNormal)
		{
			float magnitude = vector.magnitude;
			RaycastHit raycastHit;
			other.Raycast(new Ray(vector2, vector / magnitude), out raycastHit, 2f * magnitude);
			vector3 = raycastHit.point;
			rotation = Quaternion.LookRotation(raycastHit.normal, Random.onUnitSphere);
		}
		else
		{
			vector3 = other.ClosestPoint(vector2);
			rotation = base.transform.rotation;
		}
		VRRig componentInParent = other.GetComponentInParent<VRRig>();
		if (componentInParent != null)
		{
			if (this.headZoneRadius > 0f && string.Equals(other.name, "SpeakerHeadCollider"))
			{
				Vector3 b;
				Quaternion quaternion;
				other.transform.GetPositionAndRotation(out b, out quaternion);
				Vector3 vector4 = quaternion * this.headZoneInversePosition + b;
				if ((vector3 - vector4).magnitude <= this.headZoneRadius * componentInParent.scaleFactor)
				{
					if (componentInParent.isOfflineVRRig)
					{
						this.StickTo(other.transform, quaternion * this.headZoneInverseLocalPosition + b, quaternion * this.INVERSE_HEAD_ROTATION);
						this.stickyPart.localScale *= this.scaleOnLocalHeadZone;
						this.stickEvents.InvokeAll(StickyProjectile.StickFlags.LocalHeadZone, false);
						return;
					}
					this.StickTo(other.transform, vector4, quaternion * this.INVERSE_HEAD_ROTATION);
					this.stickEvents.InvokeAll(StickyProjectile.StickFlags.RemoteHeadZone, false);
					return;
				}
				else if (componentInParent.isOfflineVRRig)
				{
					this.stickyPart.localScale *= this.scaleOnLocalHead;
				}
			}
			this.stickEvents.InvokeAll(componentInParent.isOfflineVRRig ? StickyProjectile.StickFlags.LocalPlayer : StickyProjectile.StickFlags.RemotePlayer, false);
		}
		else
		{
			this.stickEvents.InvokeAll(StickyProjectile.StickFlags.Wall, false);
		}
		this.StickTo(other.transform, vector3, rotation);
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000C20F1 File Offset: 0x000C02F1
	private void OnEnable()
	{
		this.stickyPart.gameObject.SetActive(true);
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000C2104 File Offset: 0x000C0304
	private void OnDisable()
	{
		this.stickyPart.gameObject.SetActive(false);
		UnityEvent onReset = this.OnReset;
		if (onReset == null)
		{
			return;
		}
		onReset.Invoke();
	}

	// Token: 0x170003C7 RID: 967
	// (get) Token: 0x0600241F RID: 9247 RVA: 0x000C2127 File Offset: 0x000C0327
	// (set) Token: 0x06002420 RID: 9248 RVA: 0x000C212F File Offset: 0x000C032F
	public bool TickRunning { get; set; }

	// Token: 0x06002421 RID: 9249 RVA: 0x000C2138 File Offset: 0x000C0338
	public void Tick()
	{
		this.rb.rotation = Quaternion.LookRotation(this.rb.linearVelocity);
	}

	// Token: 0x04002F5C RID: 12124
	[SerializeField]
	private Transform stickyPart;

	// Token: 0x04002F5D RID: 12125
	[Tooltip("Align the positive Z direction of this object to the rigidbody's velocity.")]
	[SerializeField]
	private bool faceVelocityWhileAirborne;

	// Token: 0x04002F5E RID: 12126
	[Tooltip("Set the rigidbody's angular velocity to a random unit Vector3, multiplied by a random value in this range.")]
	[SerializeField]
	private Vector2 launchRandomSpinSpeedMinMax = new Vector2(90f, 360f);

	// Token: 0x04002F5F RID: 12127
	[Tooltip("When enabled, the positive Z direction will face away from whatever surface the projectile hit. When disabled, it will keep its original rotation.")]
	[SerializeField]
	private bool alignToHitNormal = true;

	// Token: 0x04002F60 RID: 12128
	[Space]
	[SerializeField]
	public UnityEvent OnReset;

	// Token: 0x04002F61 RID: 12129
	[SerializeField]
	public UnityEvent OnLaunch;

	// Token: 0x04002F62 RID: 12130
	[Tooltip("Scale the 'Sticky Part' by this value when hitting the local player's head. Usually used to prevent things from obscuring your vision too much.")]
	[SerializeField]
	private float scaleOnLocalHead = 0.7f;

	// Token: 0x04002F63 RID: 12131
	[Tooltip("The radius of the head zone. Can be set to 0 to disable head zone functionality.")]
	[SerializeField]
	private float headZoneRadius = 0.15f;

	// Token: 0x04002F64 RID: 12132
	[Tooltip("The local origin of the head zone, relative to the player rig's head transform. When a shot hits inside the zone, the 'Sticky Part' will be moved to this position relative to the hit player's head.")]
	[SerializeField]
	private Vector3 headZonePosition = new Vector3(0f, 0.02f, 0.17f);

	// Token: 0x04002F65 RID: 12133
	[Tooltip("Scale the 'Sticky Part' by this value when hitting the local player's head zone. Can override 'Scale On Local Head' in case you want it to appear larger for emphasis.")]
	[SerializeField]
	private float scaleOnLocalHeadZone = 1f;

	// Token: 0x04002F66 RID: 12134
	[Tooltip("When a shot hits inside a remote player's head zone, it will be moved to the 'Head Zone Relative Position'. For the local player, it will instead be moved here. This DOES NOT AFFECT the actual origin of the head zone for hit-detection purposes, it is purely visual after-the-fact.")]
	[SerializeField]
	private Vector3 localHeadZonePosition = new Vector3(0f, 0.05f, 0.2f);

	// Token: 0x04002F67 RID: 12135
	[SerializeField]
	private FlagEvents<StickyProjectile.StickFlags> stickEvents;

	// Token: 0x04002F68 RID: 12136
	private readonly Quaternion INVERSE_HEAD_ROTATION = Quaternion.Inverse(Quaternion.Euler(0f, 270f, 252.3229f));

	// Token: 0x04002F69 RID: 12137
	private Vector3 headZoneInversePosition;

	// Token: 0x04002F6A RID: 12138
	private Vector3 headZoneInverseLocalPosition;

	// Token: 0x04002F6B RID: 12139
	private Vector3 stickyPartLocalPosition;

	// Token: 0x04002F6C RID: 12140
	private Quaternion stickyPartLocalRotation;

	// Token: 0x04002F6D RID: 12141
	private Vector3 stickyPartLocalScale;

	// Token: 0x04002F6E RID: 12142
	private Rigidbody rb;

	// Token: 0x04002F6F RID: 12143
	private RigidbodyWaterInteraction rbwi;

	// Token: 0x04002F70 RID: 12144
	private Collider collider;

	// Token: 0x04002F71 RID: 12145
	private PlayerColoredCosmetic pcc;

	// Token: 0x04002F72 RID: 12146
	private int triggerLayer;

	// Token: 0x02000593 RID: 1427
	[Flags]
	public enum StickFlags
	{
		// Token: 0x04002F75 RID: 12149
		Wall = 1,
		// Token: 0x04002F76 RID: 12150
		LocalPlayer = 2,
		// Token: 0x04002F77 RID: 12151
		RemotePlayer = 4,
		// Token: 0x04002F78 RID: 12152
		LocalHeadZone = 8,
		// Token: 0x04002F79 RID: 12153
		RemoteHeadZone = 16
	}
}
