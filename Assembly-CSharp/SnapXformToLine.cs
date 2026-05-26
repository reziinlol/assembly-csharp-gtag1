using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009D1 RID: 2513
public class SnapXformToLine : MonoBehaviour
{
	// Token: 0x170005F5 RID: 1525
	// (get) Token: 0x06004059 RID: 16473 RVA: 0x00157EF3 File Offset: 0x001560F3
	public Vector3 linePoint
	{
		get
		{
			return this._closest;
		}
	}

	// Token: 0x170005F6 RID: 1526
	// (get) Token: 0x0600405A RID: 16474 RVA: 0x00157EFB File Offset: 0x001560FB
	public float linearDistance
	{
		get
		{
			return this._linear;
		}
	}

	// Token: 0x0600405B RID: 16475 RVA: 0x00157F03 File Offset: 0x00156103
	public void SnapTarget(bool applyToXform = true)
	{
		this.Snap(this.target, true);
	}

	// Token: 0x0600405C RID: 16476 RVA: 0x00157F12 File Offset: 0x00156112
	public void SnapTarget(Vector3 point)
	{
		if (this.target)
		{
			this.target.position = this.GetSnappedPoint(this.target.position);
		}
	}

	// Token: 0x0600405D RID: 16477 RVA: 0x00157F40 File Offset: 0x00156140
	public void SnapTargetLinear(float t)
	{
		if (this.target && this.from && this.to)
		{
			this.target.position = Vector3.Lerp(this.from.position, this.to.position, t);
		}
	}

	// Token: 0x0600405E RID: 16478 RVA: 0x00157F9B File Offset: 0x0015619B
	public Vector3 GetSnappedPoint(Transform t)
	{
		return this.GetSnappedPoint(t.position);
	}

	// Token: 0x0600405F RID: 16479 RVA: 0x00157FAC File Offset: 0x001561AC
	public Vector3 GetSnappedPoint(Vector3 point)
	{
		if (!this.apply)
		{
			return point;
		}
		if (!this.from || !this.to)
		{
			return point;
		}
		return SnapXformToLine.GetClosestPointOnLine(point, this.from.position, this.to.position);
	}

	// Token: 0x06004060 RID: 16480 RVA: 0x00157FFC File Offset: 0x001561FC
	public void Snap(Transform xform, bool applyToXform = true)
	{
		if (!this.apply || !xform || !this.from || !this.to)
		{
			return;
		}
		Vector3 position = xform.position;
		Vector3 position2 = this.from.position;
		Vector3 position3 = this.to.position;
		Vector3 closestPointOnLine = SnapXformToLine.GetClosestPointOnLine(position, position2, position3);
		float num = Vector3.Distance(position2, position3);
		float num2 = Vector3.Distance(closestPointOnLine, position2);
		Vector3 closest = this._closest;
		Vector3 vector = closestPointOnLine;
		float linear = this._linear;
		float num3 = Mathf.Approximately(num, 0f) ? 0f : (num2 / (num + Mathf.Epsilon));
		this._closest = vector;
		this._linear = num3;
		if (this.output)
		{
			IRangedVariable<float> asT = this.output.AsT;
			asT.Set(asT.Min + this._linear * asT.Range);
		}
		if (applyToXform)
		{
			xform.position = this._closest;
			if (!Mathf.Approximately(closest.x, vector.x) || !Mathf.Approximately(closest.y, vector.y) || !Mathf.Approximately(closest.z, vector.z))
			{
				UnityEvent<Vector3> unityEvent = this.onPositionChanged;
				if (unityEvent != null)
				{
					unityEvent.Invoke(this._closest);
				}
			}
			if (!Mathf.Approximately(linear, num3))
			{
				UnityEvent<float> unityEvent2 = this.onLinearDistanceChanged;
				if (unityEvent2 != null)
				{
					unityEvent2.Invoke(this._linear);
				}
			}
			if (this.snapOrientation)
			{
				xform.forward = (position3 - position2).normalized;
				xform.up = Vector3.Lerp(this.from.up.normalized, this.to.up.normalized, this._linear);
			}
		}
	}

	// Token: 0x06004061 RID: 16481 RVA: 0x001581C2 File Offset: 0x001563C2
	private void OnDisable()
	{
		if (this.resetOnDisable)
		{
			this.SnapTargetLinear(0f);
		}
	}

	// Token: 0x06004062 RID: 16482 RVA: 0x001581D7 File Offset: 0x001563D7
	private void LateUpdate()
	{
		this.SnapTarget(true);
	}

	// Token: 0x06004063 RID: 16483 RVA: 0x001581E0 File Offset: 0x001563E0
	private static Vector3 GetClosestPointOnLine(Vector3 p, Vector3 a, Vector3 b)
	{
		Vector3 lhs = p - a;
		Vector3 vector = b - a;
		float sqrMagnitude = vector.sqrMagnitude;
		float d = Mathf.Clamp(Vector3.Dot(lhs, vector) / sqrMagnitude, 0f, 1f);
		return a + vector * d;
	}

	// Token: 0x040050E4 RID: 20708
	public bool apply = true;

	// Token: 0x040050E5 RID: 20709
	public bool snapOrientation = true;

	// Token: 0x040050E6 RID: 20710
	public bool resetOnDisable = true;

	// Token: 0x040050E7 RID: 20711
	[Space]
	public Transform target;

	// Token: 0x040050E8 RID: 20712
	[Space]
	public Transform from;

	// Token: 0x040050E9 RID: 20713
	public Transform to;

	// Token: 0x040050EA RID: 20714
	private Vector3 _closest;

	// Token: 0x040050EB RID: 20715
	private float _linear;

	// Token: 0x040050EC RID: 20716
	public Ref<IRangedVariable<float>> output;

	// Token: 0x040050ED RID: 20717
	public UnityEvent<float> onLinearDistanceChanged;

	// Token: 0x040050EE RID: 20718
	public UnityEvent<Vector3> onPositionChanged;
}
