using System;
using MathGeoLib;
using UnityEngine;

// Token: 0x02000E12 RID: 3602
[Serializable]
public struct BoundsInfo
{
	// Token: 0x17000843 RID: 2115
	// (get) Token: 0x060057B8 RID: 22456 RVA: 0x001C6A43 File Offset: 0x001C4C43
	public Vector3 sizeComputed
	{
		get
		{
			return Vector3.Scale(this.size, this.scale) * this.inflate;
		}
	}

	// Token: 0x17000844 RID: 2116
	// (get) Token: 0x060057B9 RID: 22457 RVA: 0x001C6A61 File Offset: 0x001C4C61
	public Vector3 sizeComputedAA
	{
		get
		{
			return Vector3.Scale(this.sizeAA, this.scaleAA) * this.inflateAA;
		}
	}

	// Token: 0x060057BA RID: 22458 RVA: 0x001C6A80 File Offset: 0x001C4C80
	public static BoundsInfo ComputeBounds(Vector3[] vertices)
	{
		if (vertices.Length == 0)
		{
			return default(BoundsInfo);
		}
		OrientedBoundingBox orientedBoundingBox = OrientedBoundingBox.BruteEnclosing(vertices);
		Vector4 column = orientedBoundingBox.Axis1;
		Vector4 column2 = orientedBoundingBox.Axis2;
		Vector4 column3 = orientedBoundingBox.Axis3;
		Vector4 column4 = new Vector4(0f, 0f, 0f, 1f);
		BoundsInfo result = default(BoundsInfo);
		result.center = orientedBoundingBox.Center;
		result.size = orientedBoundingBox.Extent * 2f;
		result.rotation = new Matrix4x4(column, column2, column3, column4).rotation;
		result.scale = Vector3.one;
		result.inflate = 1f;
		Bounds bounds = GeometryUtility.CalculateBounds(vertices, Matrix4x4.identity);
		result.centerAA = bounds.center;
		result.sizeAA = bounds.size;
		result.scaleAA = Vector3.one;
		result.inflateAA = 1f;
		return result;
	}

	// Token: 0x060057BB RID: 22459 RVA: 0x001C6B84 File Offset: 0x001C4D84
	public static BoxCollider CreateBoxCollider(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int hashCode3 = bounds.rotation.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2, hashCode3);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.center;
		transform.rotation = bounds.rotation;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputed;
		return boxCollider;
	}

	// Token: 0x060057BC RID: 22460 RVA: 0x001C6C30 File Offset: 0x001C4E30
	public static BoxCollider CreateBoxColliderAA(BoundsInfo bounds)
	{
		int hashCode = bounds.center.QuantizedId128().GetHashCode();
		int hashCode2 = bounds.size.QuantizedId128().GetHashCode();
		int num = StaticHash.Compute(hashCode, hashCode2);
		Transform transform = new GameObject(string.Format("BoxCollider_{0:X8}", num)).transform;
		transform.position = bounds.centerAA;
		BoxCollider boxCollider = transform.gameObject.AddComponent<BoxCollider>();
		boxCollider.size = bounds.sizeComputedAA;
		return boxCollider;
	}

	// Token: 0x04006881 RID: 26753
	public Vector3 center;

	// Token: 0x04006882 RID: 26754
	public Vector3 size;

	// Token: 0x04006883 RID: 26755
	public Quaternion rotation;

	// Token: 0x04006884 RID: 26756
	public Vector3 scale;

	// Token: 0x04006885 RID: 26757
	public float inflate;

	// Token: 0x04006886 RID: 26758
	[Space]
	public Vector3 centerAA;

	// Token: 0x04006887 RID: 26759
	public Vector3 sizeAA;

	// Token: 0x04006888 RID: 26760
	public Vector3 scaleAA;

	// Token: 0x04006889 RID: 26761
	public float inflateAA;
}
