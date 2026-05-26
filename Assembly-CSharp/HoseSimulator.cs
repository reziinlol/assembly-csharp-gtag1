using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020002DE RID: 734
public class HoseSimulator : MonoBehaviour, ISpawnable
{
	// Token: 0x170001D6 RID: 470
	// (get) Token: 0x060012AE RID: 4782 RVA: 0x00063598 File Offset: 0x00061798
	// (set) Token: 0x060012AF RID: 4783 RVA: 0x000635A0 File Offset: 0x000617A0
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x170001D7 RID: 471
	// (get) Token: 0x060012B0 RID: 4784 RVA: 0x000635A9 File Offset: 0x000617A9
	// (set) Token: 0x060012B1 RID: 4785 RVA: 0x000635B1 File Offset: 0x000617B1
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060012B2 RID: 4786 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060012B3 RID: 4787 RVA: 0x000635BC File Offset: 0x000617BC
	void ISpawnable.OnSpawn(VRRig rig)
	{
		this.anchors = rig.cosmeticReferences.Get(this.startAnchorRef).GetComponent<HoseSimulatorAnchors>();
		if (this.skinnedMeshRenderer != null)
		{
			Bounds localBounds = this.skinnedMeshRenderer.localBounds;
			localBounds.extents = this.localBoundsOverride;
			this.skinnedMeshRenderer.localBounds = localBounds;
		}
		this.hoseSectionLengths = new float[this.hoseBones.Length - 1];
		this.hoseBonePositions = new Vector3[this.hoseBones.Length];
		this.hoseBoneVelocities = new Vector3[this.hoseBones.Length];
		for (int i = 0; i < this.hoseSectionLengths.Length; i++)
		{
			float num = 1f;
			this.hoseSectionLengths[i] = num;
			this.totalHoseLength += num;
		}
	}

	// Token: 0x060012B4 RID: 4788 RVA: 0x00063684 File Offset: 0x00061884
	private void LateUpdate()
	{
		if (this.myHoldable.InLeftHand())
		{
			this.isLeftHanded = true;
		}
		else if (this.myHoldable.InRightHand())
		{
			this.isLeftHanded = false;
		}
		for (int i = 0; i < this.miscBones.Length; i++)
		{
			Transform transform = this.isLeftHanded ? this.anchors.miscAnchorsLeft[i] : this.anchors.miscAnchorsRight[i];
			this.miscBones[i].transform.position = transform.position;
			this.miscBones[i].transform.rotation = transform.rotation;
		}
		this.startAnchor = (this.isLeftHanded ? this.anchors.leftAnchorPoint : this.anchors.rightAnchorPoint);
		float x = this.myHoldable.transform.lossyScale.x;
		float num = 0f;
		Vector3 position = this.startAnchor.position;
		Vector3 ctrl = position + this.startAnchor.forward * this.startStiffness * x;
		Vector3 position2 = this.endAnchor.position;
		Vector3 ctrl2 = position2 - this.endAnchor.forward * this.endStiffness * x;
		for (int j = 0; j < this.hoseBones.Length; j++)
		{
			float num2 = num / this.totalHoseLength;
			Vector3 vector = BezierUtils.BezierSolve(num2, position, ctrl, ctrl2, position2);
			Vector3 a = BezierUtils.BezierSolve(num2 + 0.1f, position, ctrl, ctrl2, position2);
			if (this.firstUpdate)
			{
				this.hoseBones[j].transform.position = vector;
				this.hoseBonePositions[j] = vector;
				this.hoseBoneVelocities[j] = Vector3.zero;
			}
			else
			{
				this.hoseBoneVelocities[j] *= this.damping;
				this.hoseBonePositions[j] += this.hoseBoneVelocities[j] * Time.deltaTime;
				float num3 = this.hoseBoneMaxDisplacement[j] * x;
				if ((vector - this.hoseBonePositions[j]).IsLongerThan(num3))
				{
					Vector3 vector2 = vector + (this.hoseBonePositions[j] - vector).normalized * num3;
					this.hoseBoneVelocities[j] += (vector2 - this.hoseBonePositions[j]) / Time.deltaTime;
					this.hoseBonePositions[j] = vector2;
				}
				this.hoseBones[j].transform.position = this.hoseBonePositions[j];
			}
			this.hoseBones[j].transform.rotation = Quaternion.LookRotation(a - vector, this.endAnchor.transform.up);
			if (j < this.hoseSectionLengths.Length)
			{
				num += this.hoseSectionLengths[j];
			}
		}
		this.firstUpdate = false;
	}

	// Token: 0x060012B5 RID: 4789 RVA: 0x000639C2 File Offset: 0x00061BC2
	private void OnDrawGizmosSelected()
	{
		if (this.hoseBonePositions != null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawLineStrip(this.hoseBonePositions, false);
		}
	}

	// Token: 0x040016CF RID: 5839
	[SerializeField]
	private SkinnedMeshRenderer skinnedMeshRenderer;

	// Token: 0x040016D0 RID: 5840
	[SerializeField]
	private Vector3 localBoundsOverride;

	// Token: 0x040016D1 RID: 5841
	[SerializeField]
	private Transform[] miscBones;

	// Token: 0x040016D2 RID: 5842
	[SerializeField]
	private Transform[] hoseBones;

	// Token: 0x040016D3 RID: 5843
	[SerializeField]
	private float[] hoseBoneMaxDisplacement;

	// Token: 0x040016D4 RID: 5844
	[SerializeField]
	private CosmeticRefID startAnchorRef;

	// Token: 0x040016D5 RID: 5845
	private Transform startAnchor;

	// Token: 0x040016D6 RID: 5846
	[SerializeField]
	private float startStiffness = 0.5f;

	// Token: 0x040016D7 RID: 5847
	[SerializeField]
	private Transform endAnchor;

	// Token: 0x040016D8 RID: 5848
	[SerializeField]
	private float endStiffness = 0.5f;

	// Token: 0x040016D9 RID: 5849
	private Vector3[] hoseBonePositions;

	// Token: 0x040016DA RID: 5850
	private Vector3[] hoseBoneVelocities;

	// Token: 0x040016DB RID: 5851
	[SerializeField]
	private float damping = 0.97f;

	// Token: 0x040016DC RID: 5852
	private float[] hoseSectionLengths;

	// Token: 0x040016DD RID: 5853
	private float totalHoseLength;

	// Token: 0x040016DE RID: 5854
	private bool firstUpdate = true;

	// Token: 0x040016DF RID: 5855
	private HoseSimulatorAnchors anchors;

	// Token: 0x040016E0 RID: 5856
	[SerializeField]
	private TransferrableObject myHoldable;

	// Token: 0x040016E1 RID: 5857
	private bool isLeftHanded;
}
