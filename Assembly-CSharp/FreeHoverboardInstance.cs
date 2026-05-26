using System;
using UnityEngine;

// Token: 0x020008E7 RID: 2279
public class FreeHoverboardInstance : MonoBehaviour
{
	// Token: 0x17000550 RID: 1360
	// (get) Token: 0x06003B9E RID: 15262 RVA: 0x001468FC File Offset: 0x00144AFC
	// (set) Token: 0x06003B9F RID: 15263 RVA: 0x00146904 File Offset: 0x00144B04
	public Rigidbody Rigidbody { get; private set; }

	// Token: 0x17000551 RID: 1361
	// (get) Token: 0x06003BA0 RID: 15264 RVA: 0x0014690D File Offset: 0x00144B0D
	// (set) Token: 0x06003BA1 RID: 15265 RVA: 0x00146915 File Offset: 0x00144B15
	public Color boardColor { get; private set; }

	// Token: 0x06003BA2 RID: 15266 RVA: 0x00146920 File Offset: 0x00144B20
	private void Awake()
	{
		this.Rigidbody = base.GetComponent<Rigidbody>();
		Material[] sharedMaterials = this.boardMesh.sharedMaterials;
		this.colorMaterial = new Material(sharedMaterials[1]);
		sharedMaterials[1] = this.colorMaterial;
		this.boardMesh.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06003BA3 RID: 15267 RVA: 0x00146968 File Offset: 0x00144B68
	public void SetColor(Color col)
	{
		this.colorMaterial.color = col;
		this.boardColor = col;
	}

	// Token: 0x06003BA4 RID: 15268 RVA: 0x00146980 File Offset: 0x00144B80
	private void Update()
	{
		RaycastHit raycastHit;
		if (Physics.SphereCast(new Ray(base.transform.TransformPoint(this.sphereCastCenter), base.transform.TransformVector(Vector3.down)), this.sphereCastRadius, out raycastHit, 1f, this.hoverRaycastMask.value))
		{
			this.hasHoverPoint = true;
			this.hoverPoint = raycastHit.point;
			this.hoverNormal = raycastHit.normal;
			return;
		}
		this.hasHoverPoint = false;
	}

	// Token: 0x06003BA5 RID: 15269 RVA: 0x001469FC File Offset: 0x00144BFC
	private void FixedUpdate()
	{
		if (this.hasHoverPoint)
		{
			float num = Vector3.Dot(base.transform.TransformPoint(this.sphereCastCenter) - this.hoverPoint, this.hoverNormal);
			if (num < this.hoverHeight)
			{
				base.transform.position += this.hoverNormal * (this.hoverHeight - num);
				this.Rigidbody.linearVelocity = Vector3.ProjectOnPlane(this.Rigidbody.linearVelocity, this.hoverNormal);
				Vector3 point = Quaternion.Inverse(base.transform.rotation) * this.Rigidbody.angularVelocity;
				point.x *= this.avelocityDragWhileHovering;
				point.z *= this.avelocityDragWhileHovering;
				this.Rigidbody.angularVelocity = base.transform.rotation * point;
				base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(base.transform.forward, this.hoverNormal), this.hoverNormal), this.hoverRotationLerp);
			}
		}
	}

	// Token: 0x04004C35 RID: 19509
	public int ownerActorNumber;

	// Token: 0x04004C36 RID: 19510
	public int boardIndex;

	// Token: 0x04004C37 RID: 19511
	[SerializeField]
	private Vector3 sphereCastCenter;

	// Token: 0x04004C38 RID: 19512
	[SerializeField]
	private float sphereCastRadius;

	// Token: 0x04004C39 RID: 19513
	[SerializeField]
	private LayerMask hoverRaycastMask;

	// Token: 0x04004C3A RID: 19514
	[SerializeField]
	private float hoverHeight;

	// Token: 0x04004C3B RID: 19515
	[SerializeField]
	private float hoverRotationLerp;

	// Token: 0x04004C3C RID: 19516
	[SerializeField]
	private float avelocityDragWhileHovering;

	// Token: 0x04004C3D RID: 19517
	[SerializeField]
	private MeshRenderer boardMesh;

	// Token: 0x04004C3F RID: 19519
	private Material colorMaterial;

	// Token: 0x04004C40 RID: 19520
	private bool hasHoverPoint;

	// Token: 0x04004C41 RID: 19521
	private Vector3 hoverPoint;

	// Token: 0x04004C42 RID: 19522
	private Vector3 hoverNormal;
}
