using System;
using BoingKit;
using UnityEngine;

// Token: 0x020009DE RID: 2526
public class SyncRigidBodyToMovement : MonoBehaviour
{
	// Token: 0x060040A4 RID: 16548 RVA: 0x00159A12 File Offset: 0x00157C12
	private void Awake()
	{
		this.targetParent = this.targetRigidbody.transform.parent;
		this.targetRigidbody.transform.parent = null;
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x060040A5 RID: 16549 RVA: 0x00159A4C File Offset: 0x00157C4C
	private void OnEnable()
	{
		this.targetRigidbody.gameObject.SetActive(true);
		this.targetRigidbody.transform.position = base.transform.position;
		this.targetRigidbody.transform.rotation = base.transform.rotation;
	}

	// Token: 0x060040A6 RID: 16550 RVA: 0x00159AA0 File Offset: 0x00157CA0
	private void OnDisable()
	{
		this.targetRigidbody.gameObject.SetActive(false);
	}

	// Token: 0x060040A7 RID: 16551 RVA: 0x00159AB4 File Offset: 0x00157CB4
	private void FixedUpdate()
	{
		this.targetRigidbody.linearVelocity = (base.transform.position - this.targetRigidbody.position) / Time.fixedDeltaTime;
		this.targetRigidbody.angularVelocity = QuaternionUtil.ToAngularVector(Quaternion.Inverse(this.targetRigidbody.rotation) * base.transform.rotation) / Time.fixedDeltaTime;
	}

	// Token: 0x0400513D RID: 20797
	[SerializeField]
	private Rigidbody targetRigidbody;

	// Token: 0x0400513E RID: 20798
	private Transform targetParent;
}
