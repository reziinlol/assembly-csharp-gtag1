using System;
using GorillaTag.Cosmetics;
using UnityEngine;

// Token: 0x020002E5 RID: 741
public class ParachuteProjectile : MonoBehaviour, IProjectile, ITickSystemTick
{
	// Token: 0x060012D6 RID: 4822 RVA: 0x000641AE File Offset: 0x000623AE
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
	}

	// Token: 0x060012D7 RID: 4823 RVA: 0x000641BC File Offset: 0x000623BC
	private void OnEnable()
	{
		this.launched = false;
		this.landTime = 0f;
		this.launchedTime = 0f;
		this.peakTime = float.MaxValue;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (!this.TickRunning)
		{
			TickSystem<object>.AddCallbackTarget(this);
		}
	}

	// Token: 0x060012D8 RID: 4824 RVA: 0x0006421C File Offset: 0x0006241C
	private void OnDisable()
	{
		this.launched = false;
		if (this.TickRunning)
		{
			TickSystem<object>.RemoveCallbackTarget(this);
		}
	}

	// Token: 0x060012D9 RID: 4825 RVA: 0x00064234 File Offset: 0x00062434
	public void Launch(Vector3 startPosition, Quaternion startRotation, Vector3 velocity, float chargeFrac, VRRig ownerRig, int progress)
	{
		this.parachuteDeployed = false;
		this.landed = false;
		if (this.rb == null)
		{
			this.rb = base.GetComponent<Rigidbody>();
		}
		this.rb.position = startPosition;
		this.rb.rotation = startRotation;
		this.ChangeUp(Vector3.up);
		this.rb.freezeRotation = true;
		if (ownerRig == null)
		{
			base.transform.localScale = Vector3.one;
		}
		else
		{
			base.transform.localScale = Vector3.one * ownerRig.scaleFactor;
		}
		this.rb.isKinematic = false;
		this.rb.linearVelocity = velocity;
		this.rb.linearDamping = this.initialDrag;
		this.rb.angularDamping = this.initialAngularDrag;
		this.launchedTime = Time.time;
		this.monkeMeshFilter.mesh = this.launchMesh;
		this.parachute.SetActive(false);
		if (velocity.y > 0f)
		{
			this.peakTime = velocity.y / (-1f * Physics.gravity.y);
		}
		else
		{
			this.peakTime = 0f;
		}
		this.launched = true;
	}

	// Token: 0x060012DA RID: 4826 RVA: 0x00064370 File Offset: 0x00062570
	private void OnPeakReached()
	{
		this.parachuteDeployed = true;
		this.parachute.SetActive(true);
		this.monkeMeshFilter.mesh = this.parachutingMesh;
		this.ChangeUp(Vector3.up);
		this.rb.linearDamping = this.parachuteDrag;
		this.rb.angularDamping = this.parachuteAngularDrag;
	}

	// Token: 0x060012DB RID: 4827 RVA: 0x000643D0 File Offset: 0x000625D0
	private void OnLanded(Collision collision)
	{
		this.landTime = Time.time;
		this.landed = true;
		ContactPoint contact = collision.GetContact(0);
		this.rb.isKinematic = true;
		this.rb.position = contact.point + contact.normal * (this.groundOffset * base.transform.localScale.x);
		this.ChangeUp(contact.normal);
		this.monkeMeshFilter.mesh = this.landedMesh;
		this.parachute.SetActive(false);
	}

	// Token: 0x060012DC RID: 4828 RVA: 0x00064468 File Offset: 0x00062668
	private void ChangeUp(Vector3 newUp)
	{
		Vector3 forward = Vector3.Cross(this.rb.transform.right, newUp);
		if (forward.sqrMagnitude < 1E-45f)
		{
			forward = Vector3.Cross(Vector3.Cross(newUp, this.rb.transform.forward), newUp);
		}
		this.rb.rotation = Quaternion.LookRotation(forward, newUp);
	}

	// Token: 0x060012DD RID: 4829 RVA: 0x000644CC File Offset: 0x000626CC
	private void PlayImpactEffects(Vector3 position, Vector3 normal)
	{
		if (this.impactEffect != null)
		{
			Vector3 position2 = position + this.impactEffectOffset * normal;
			GameObject gameObject = ObjectPools.instance.Instantiate(this.impactEffect, position2, true);
			gameObject.transform.localScale = base.transform.localScale * this.impactEffectScaleMultiplier;
			gameObject.transform.up = normal;
		}
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060012DE RID: 4830 RVA: 0x00064548 File Offset: 0x00062748
	public void OnTriggerEvent(bool isLeft, Collider col)
	{
		if (this.parachuteDeployed)
		{
			this.PlayImpactEffects(base.transform.position, Vector3.up);
			GorillaTriggerColliderHandIndicator componentInParent = col.GetComponentInParent<GorillaTriggerColliderHandIndicator>();
			if (componentInParent != null)
			{
				float amplitude = GorillaTagger.Instance.tapHapticStrength / 2f;
				float fixedDeltaTime = Time.fixedDeltaTime;
				GorillaTagger.Instance.StartVibration(componentInParent.isLeftHand, amplitude, fixedDeltaTime);
			}
		}
	}

	// Token: 0x060012DF RID: 4831 RVA: 0x000645AC File Offset: 0x000627AC
	private void OnCollisionEnter(Collision collision)
	{
		if (!this.launched || this.landed)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		if (collision.collider.attachedRigidbody != null)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (collision.collider.gameObject.IsOnLayer(UnityLayer.GorillaThrowable))
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (!this.parachuteDeployed)
		{
			this.PlayImpactEffects(contact.point, contact.normal);
			return;
		}
		if (Vector3.Angle(contact.normal, Vector3.up) < this.groudUpThreshold)
		{
			this.OnLanded(collision);
			return;
		}
		this.PlayImpactEffects(contact.point, contact.normal);
	}

	// Token: 0x170001DC RID: 476
	// (get) Token: 0x060012E0 RID: 4832 RVA: 0x00064675 File Offset: 0x00062875
	// (set) Token: 0x060012E1 RID: 4833 RVA: 0x0006467D File Offset: 0x0006287D
	public bool TickRunning { get; set; }

	// Token: 0x060012E2 RID: 4834 RVA: 0x00064688 File Offset: 0x00062888
	public void Tick()
	{
		if (!this.parachuteDeployed && Time.time > this.launchedTime + this.parachuteDeployDelay && Time.time >= this.launchedTime + this.peakTime)
		{
			this.OnPeakReached();
		}
		if (this.landed && Time.time > this.landTime + this.destroyOnLandDelay)
		{
			this.PlayImpactEffects(base.transform.position, base.transform.up);
		}
	}

	// Token: 0x0400170C RID: 5900
	[SerializeField]
	private MeshFilter monkeMeshFilter;

	// Token: 0x0400170D RID: 5901
	[SerializeField]
	private GameObject parachute;

	// Token: 0x0400170E RID: 5902
	[SerializeField]
	private Mesh launchMesh;

	// Token: 0x0400170F RID: 5903
	[SerializeField]
	private Mesh parachutingMesh;

	// Token: 0x04001710 RID: 5904
	[SerializeField]
	private Mesh landedMesh;

	// Token: 0x04001711 RID: 5905
	[Tooltip("time to wait after launch before deploying the parachute")]
	[SerializeField]
	private float parachuteDeployDelay = 1f;

	// Token: 0x04001712 RID: 5906
	[Tooltip("time to wait after landing before destroying")]
	[SerializeField]
	private float destroyOnLandDelay = 3f;

	// Token: 0x04001713 RID: 5907
	[Tooltip("How far from the collision point should the projectile sit when landed")]
	[SerializeField]
	private float groundOffset;

	// Token: 0x04001714 RID: 5908
	[Tooltip("Acceptable angle in degrees of surface from world up to be considered the ground")]
	[SerializeField]
	private float groudUpThreshold = 45f;

	// Token: 0x04001715 RID: 5909
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialDrag;

	// Token: 0x04001716 RID: 5910
	[Tooltip("Drag before the parachute is deployed.")]
	[SerializeField]
	private float initialAngularDrag = 0.05f;

	// Token: 0x04001717 RID: 5911
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteDrag = 5f;

	// Token: 0x04001718 RID: 5912
	[Tooltip("Drag after the parachute is deployed.")]
	[SerializeField]
	private float parachuteAngularDrag = 10f;

	// Token: 0x04001719 RID: 5913
	[SerializeField]
	private GameObject impactEffect;

	// Token: 0x0400171A RID: 5914
	[SerializeField]
	private float impactEffectScaleMultiplier = 1f;

	// Token: 0x0400171B RID: 5915
	[Tooltip("Distance from the surface that the particle should spawn.")]
	[SerializeField]
	private float impactEffectOffset;

	// Token: 0x0400171C RID: 5916
	private Rigidbody rb;

	// Token: 0x0400171D RID: 5917
	private bool launched;

	// Token: 0x0400171E RID: 5918
	private float launchedTime;

	// Token: 0x0400171F RID: 5919
	private float landTime;

	// Token: 0x04001720 RID: 5920
	private float peakTime = float.MaxValue;

	// Token: 0x04001721 RID: 5921
	private bool parachuteDeployed;

	// Token: 0x04001722 RID: 5922
	private bool landed;
}
