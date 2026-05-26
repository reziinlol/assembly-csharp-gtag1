using System;
using UnityEngine;

// Token: 0x02000D80 RID: 3456
public class BezierCurve : MonoBehaviour
{
	// Token: 0x060054C1 RID: 21697 RVA: 0x001BA7D8 File Offset: 0x001B89D8
	public Vector3 GetPoint(float t)
	{
		Vector3 vector = (this.points.Length == 3) ? Bezier.GetPoint(this.points[0], this.points[1], this.points[2], t) : Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t);
		if (!this.referenceTransform)
		{
			return vector;
		}
		return this.referenceTransform.TransformPoint(vector);
	}

	// Token: 0x060054C2 RID: 21698 RVA: 0x001BA870 File Offset: 0x001B8A70
	public Vector3 GetVelocity(float t)
	{
		Vector3 vector = (this.points.Length == 3) ? Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], t) : Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t);
		if (!this.referenceTransform)
		{
			return vector;
		}
		return this.referenceTransform.TransformPoint(vector) - this.referenceTransform.position;
	}

	// Token: 0x060054C3 RID: 21699 RVA: 0x001BA918 File Offset: 0x001B8B18
	public Vector3 GetDirection(float t)
	{
		return this.GetVelocity(t).normalized;
	}

	// Token: 0x060054C4 RID: 21700 RVA: 0x001BA934 File Offset: 0x001B8B34
	public void Reset()
	{
		this.referenceTransform = base.transform;
		this.points = new Vector3[]
		{
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
	}

	// Token: 0x04006554 RID: 25940
	public Transform referenceTransform;

	// Token: 0x04006555 RID: 25941
	public Vector3[] points;
}
