using System;
using UnityEngine;

// Token: 0x020004B0 RID: 1200
public class KiteDynamics : MonoBehaviour, ITetheredObjectBehavior
{
	// Token: 0x06001D48 RID: 7496 RVA: 0x0009E430 File Offset: 0x0009C630
	private void Awake()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.knotRb = this.knot.GetComponent<Rigidbody>();
		this.balloonCollider = base.GetComponent<Collider>();
		this.grabPtPosition = this.grabPt.position;
		this.grabPtInitParent = this.grabPt.transform.parent;
	}

	// Token: 0x06001D49 RID: 7497 RVA: 0x0009E48D File Offset: 0x0009C68D
	private void Start()
	{
		this.airResistance = Mathf.Clamp(this.airResistance, 0f, 1f);
		this.balloonCollider.enabled = false;
	}

	// Token: 0x06001D4A RID: 7498 RVA: 0x0009E4B8 File Offset: 0x0009C6B8
	public void ReParent()
	{
		if (this.grabPt != null)
		{
			this.grabPt.transform.parent = this.grabPtInitParent.transform;
		}
		this.bouyancyActualHeight = Random.Range(this.bouyancyMinHeight, this.bouyancyMaxHeight);
	}

	// Token: 0x06001D4B RID: 7499 RVA: 0x0009E508 File Offset: 0x0009C708
	public void EnableDynamics(bool enable, bool collider, bool kinematic)
	{
		this.enableDynamics = enable;
		if (this.balloonCollider)
		{
			this.balloonCollider.enabled = collider;
		}
		if (this.rb != null)
		{
			this.rb.isKinematic = kinematic;
			if (!enable)
			{
				this.rb.linearVelocity = Vector3.zero;
				this.rb.angularVelocity = Vector3.zero;
			}
		}
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x0009E572 File Offset: 0x0009C772
	public void EnableDistanceConstraints(bool enable, float scale = 1f)
	{
		this.rb.useGravity = !enable;
		this.balloonScale = scale;
		this.grabPtPosition = this.grabPt.position;
	}

	// Token: 0x1700031E RID: 798
	// (get) Token: 0x06001D4D RID: 7501 RVA: 0x0009E59B File Offset: 0x0009C79B
	public bool ColliderEnabled
	{
		get
		{
			return this.balloonCollider && this.balloonCollider.enabled;
		}
	}

	// Token: 0x06001D4E RID: 7502 RVA: 0x0009E5B8 File Offset: 0x0009C7B8
	private void FixedUpdate()
	{
		if (this.rb.isKinematic || this.rb.useGravity)
		{
			return;
		}
		if (this.enableDynamics)
		{
			Vector3 vector = (this.grabPt.position - this.grabPtPosition) * 100f;
			vector = Matrix4x4.Rotate(this.ctrlRotation).MultiplyVector(vector);
			this.rb.AddForce(vector, ForceMode.Force);
			Vector3 linearVelocity = this.rb.linearVelocity;
			float magnitude = linearVelocity.magnitude;
			this.rb.linearVelocity = linearVelocity.normalized * Mathf.Min(magnitude, this.maximumVelocity * this.balloonScale);
			base.transform.LookAt(base.transform.position - this.rb.linearVelocity);
		}
	}

	// Token: 0x06001D4F RID: 7503 RVA: 0x00002AF8 File Offset: 0x00000CF8
	void ITetheredObjectBehavior.DbgClear()
	{
		throw new NotImplementedException();
	}

	// Token: 0x06001D50 RID: 7504 RVA: 0x0009BEBF File Offset: 0x0009A0BF
	bool ITetheredObjectBehavior.IsEnabled()
	{
		return base.enabled;
	}

	// Token: 0x06001D51 RID: 7505 RVA: 0x0009E692 File Offset: 0x0009C892
	void ITetheredObjectBehavior.TriggerEnter(Collider other, ref Vector3 force, ref Vector3 collisionPt, ref bool transferOwnership)
	{
		transferOwnership = false;
	}

	// Token: 0x06001D52 RID: 7506 RVA: 0x0009E698 File Offset: 0x0009C898
	public bool ReturnStep()
	{
		this.rb.isKinematic = true;
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.grabPt.position, Time.deltaTime * this.returnSpeed);
		return base.transform.position == this.grabPt.position;
	}

	// Token: 0x04002772 RID: 10098
	private Rigidbody rb;

	// Token: 0x04002773 RID: 10099
	private Collider balloonCollider;

	// Token: 0x04002774 RID: 10100
	private Bounds bounds;

	// Token: 0x04002775 RID: 10101
	[SerializeField]
	private float bouyancyMinHeight = 10f;

	// Token: 0x04002776 RID: 10102
	[SerializeField]
	private float bouyancyMaxHeight = 20f;

	// Token: 0x04002777 RID: 10103
	private float bouyancyActualHeight = 20f;

	// Token: 0x04002778 RID: 10104
	[SerializeField]
	private float airResistance = 0.01f;

	// Token: 0x04002779 RID: 10105
	public GameObject knot;

	// Token: 0x0400277A RID: 10106
	private Rigidbody knotRb;

	// Token: 0x0400277B RID: 10107
	public Transform grabPt;

	// Token: 0x0400277C RID: 10108
	private Transform grabPtInitParent;

	// Token: 0x0400277D RID: 10109
	[SerializeField]
	private float maximumVelocity = 2f;

	// Token: 0x0400277E RID: 10110
	private bool enableDynamics;

	// Token: 0x0400277F RID: 10111
	[SerializeField]
	private float balloonScale = 1f;

	// Token: 0x04002780 RID: 10112
	private Vector3 grabPtPosition;

	// Token: 0x04002781 RID: 10113
	[SerializeField]
	private Quaternion ctrlRotation;

	// Token: 0x04002782 RID: 10114
	[SerializeField]
	private float returnSpeed = 50f;
}
