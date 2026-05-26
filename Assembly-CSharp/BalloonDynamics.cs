using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020004A1 RID: 1185
public class BalloonDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x06001CB5 RID: 7349 RVA: 0x0009BA70 File Offset: 0x00099C70
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x06001CB6 RID: 7350 RVA: 0x0009BABC File Offset: 0x00099CBC
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x06001CB7 RID: 7351 RVA: 0x0009BAE8 File Offset: 0x00099CE8
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x06001CB8 RID: 7352 RVA: 0x0009BB38 File Offset: 0x00099D38
	private void ApplyBouyancyForce()
	{
		float num = this.bouyancyActualHeight + Mathf.Sin(Time.time) * this.varianceMaxheight;
		float num2 = (num - base.transform.position.y) / num;
		float y = this.bouyancyForce * num2 * this.balloonScale;
		this.rb.AddForce(new Vector3(0f, y, 0f) * this.rb.mass, ForceMode.Force);
	}

	// Token: 0x06001CB9 RID: 7353 RVA: 0x0009BBB0 File Offset: 0x00099DB0
	private void ApplyUpRightForce()
	{
		Vector3 torque = Vector3.Cross(base.transform.up, Vector3.up) * this.upRightTorque * this.balloonScale;
		this.rb.AddTorque(torque);
	}

	// Token: 0x06001CBA RID: 7354 RVA: 0x0009BBF8 File Offset: 0x00099DF8
	private void ApplyAntiSpinForce()
	{
		Vector3 vector = this.rb.transform.InverseTransformDirection(this.rb.angularVelocity);
		this.rb.AddRelativeTorque(0f, -vector.y * this.antiSpinTorque, 0f);
	}

	// Token: 0x06001CBB RID: 7355 RVA: 0x0009BC44 File Offset: 0x00099E44
	private void ApplyAirResistance()
	{
		this.rb.linearVelocity *= 1f - this.airResistance;
	}

	// Token: 0x06001CBC RID: 7356 RVA: 0x0009BC68 File Offset: 0x00099E68
	private void ApplyDistanceConstraint()
	{
		this.knot.transform.position - base.transform.position;
		Vector3 vector = this.grabPt.transform.position - this.knot.transform.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		float num = this.stringLength * this.balloonScale;
		if (magnitude > num)
		{
			Vector3 vector2 = Vector3.Dot(this.knotRb.linearVelocity, normalized) * normalized;
			float num2 = magnitude - num;
			float num3 = num2 / Time.fixedDeltaTime;
			if (vector2.magnitude < num3)
			{
				float b = num3 - vector2.magnitude;
				float num4 = Mathf.Clamp01(num2 / this.stringStretch);
				Vector3 a = Mathf.Lerp(0f, b, num4 * num4) * normalized * this.stringStrength;
				this.rb.AddForceAtPosition(a * this.rb.mass, this.knot.transform.position, ForceMode.Impulse);
			}
		}
	}

	// Token: 0x06001CBD RID: 7357 RVA: 0x0009BD84 File Offset: 0x00099F84
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		bool flag = !this.enableDynamics && enable;
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!kinematic && flag)
			{
				this.rb.linearVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001CBE RID: 7358 RVA: 0x0009BDFF File Offset: 0x00099FFF
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.enableDistanceConstraints = enable;
		this.balloonScale = scale;
	}

	// Token: 0x1700030D RID: 781
	// (get) Token: 0x06001CBF RID: 7359 RVA: 0x0009BE0F File Offset: 0x0009A00F
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001CC0 RID: 7360 RVA: 0x0009BE2C File Offset: 0x0009A02C
	private void FixedUpdate()
	{
		if (this.enableDynamics && !this.rb.isKinematic)
		{
			this.ApplyBouyancyForce();
			if (this.antiSpinTorque > 0f)
			{
				this.ApplyAntiSpinForce();
			}
			this.ApplyUpRightForce();
			this.ApplyAirResistance();
			if (this.enableDistanceConstraints)
			{
				this.ApplyDistanceConstraint();
			}
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			this.rb.linearVelocity = linearVelocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
		}
	}

	// Token: 0x06001CC1 RID: 7361 RVA: 0x00002AF8 File Offset: 0x00000CF8
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001CC2 RID: 7362 RVA: 0x0009BEBF File Offset: 0x0009A0BF
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001CC3 RID: 7363 RVA: 0x0009BEC8 File Offset: 0x0009A0C8
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		if (!other.gameObject.IsOnLayer(UnityLayer.GorillaHand))
		{
			return;
		}
		if (!this.rb)
		{
			return;
		}
		transferOwnership = true;
		TransformFollow component = other.gameObject.GetComponent<TransformFollow>();
		if (!component)
		{
			return;
		}
		Vector3 a = (component.transform.position - component.prevPos) / Time.deltaTime;
		force = a * this.bopSpeed;
		force = Mathf.Min(this.maximumVelocity, force.magnitude) * force.normalized * this.balloonScale;
		if (this.bopSpeedCap > 0f && force.IsLongerThan(this.bopSpeedCap))
		{
			force = force.normalized * this.bopSpeedCap;
		}
		collisionPt = other.ClosestPointOnBounds(base.transform.position);
		this.rb.AddForceAtPosition(force * this.rb.mass, collisionPt, ForceMode.Impulse);
		if (this.balloonBopSource != null)
		{
			this.balloonBopSource.GTPlay();
		}
		GorillaTriggerColliderHandIndicator component2 = other.GetComponent<GorillaTriggerColliderHandIndicator>();
		if (component2 != null)
		{
			float amplitude = GorillaTagger.Instance.tapHapticStrength / 4f;
			float fixedDeltaTime = Time.fixedDeltaTime;
			GorillaTagger.Instance.StartVibration(component2.isLeftHand, amplitude, fixedDeltaTime);
		}
	}

	// Token: 0x06001CC4 RID: 7364 RVA: 0x00023994 File Offset: 0x00021B94
	public bool ReturnStep()
	{
		return true;
	}

	// Token: 0x040026D6 RID: 9942
	private Rigidbody rb;

	// Token: 0x040026D7 RID: 9943
	private Collider balloonCollider;

	// Token: 0x040026D8 RID: 9944
	private Bounds bounds;

	// Token: 0x040026D9 RID: 9945
	public float bouyancyForce = 1f;

	// Token: 0x040026DA RID: 9946
	public float bouyancyMinHeight = 10f;

	// Token: 0x040026DB RID: 9947
	public float bouyancyMaxHeight = 20f;

	// Token: 0x040026DC RID: 9948
	private float bouyancyActualHeight = 20f;

	// Token: 0x040026DD RID: 9949
	public float varianceMaxheight = 5f;

	// Token: 0x040026DE RID: 9950
	public float airResistance = 0.01f;

	// Token: 0x040026DF RID: 9951
	public GameObject knot;

	// Token: 0x040026E0 RID: 9952
	private Rigidbody knotRb;

	// Token: 0x040026E1 RID: 9953
	public Transform grabPt;

	// Token: 0x040026E2 RID: 9954
	private Transform grabPtInitParent;

	// Token: 0x040026E3 RID: 9955
	public float stringLength = 2f;

	// Token: 0x040026E4 RID: 9956
	public float stringStrength = 0.9f;

	// Token: 0x040026E5 RID: 9957
	public float stringStretch = 0.1f;

	// Token: 0x040026E6 RID: 9958
	public float maximumVelocity = 2f;

	// Token: 0x040026E7 RID: 9959
	public float upRightTorque = 1f;

	// Token: 0x040026E8 RID: 9960
	public float antiSpinTorque;

	// Token: 0x040026E9 RID: 9961
	private bool enableDynamics;

	// Token: 0x040026EA RID: 9962
	private bool enableDistanceConstraints;

	// Token: 0x040026EB RID: 9963
	public float balloonScale = 1f;

	// Token: 0x040026EC RID: 9964
	public float bopSpeed = 1f;

	// Token: 0x040026ED RID: 9965
	public float bopSpeedCap;

	// Token: 0x040026EE RID: 9966
	[SerializeField]
	private AudioSource balloonBopSource;
}
