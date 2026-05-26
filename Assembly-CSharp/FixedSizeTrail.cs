using System;
using UnityEngine;

// Token: 0x02000687 RID: 1671
[RequireComponent(typeof(LineRenderer))]
public class FixedSizeTrail : MonoBehaviour
{
	// Token: 0x17000428 RID: 1064
	// (get) Token: 0x0600299E RID: 10654 RVA: 0x000E0AAA File Offset: 0x000DECAA
	public LineRenderer renderer
	{
		get
		{
			return this._lineRenderer;
		}
	}

	// Token: 0x17000429 RID: 1065
	// (get) Token: 0x0600299F RID: 10655 RVA: 0x000E0AB2 File Offset: 0x000DECB2
	// (set) Token: 0x060029A0 RID: 10656 RVA: 0x000E0ABA File Offset: 0x000DECBA
	public float length
	{
		get
		{
			return this._length;
		}
		set
		{
			this._length = Math.Clamp(value, 0f, 128f);
		}
	}

	// Token: 0x1700042A RID: 1066
	// (get) Token: 0x060029A1 RID: 10657 RVA: 0x000E0AD2 File Offset: 0x000DECD2
	public Vector3[] points
	{
		get
		{
			return this._points;
		}
	}

	// Token: 0x060029A2 RID: 10658 RVA: 0x000E0ADA File Offset: 0x000DECDA
	private void Reset()
	{
		this.Setup();
	}

	// Token: 0x060029A3 RID: 10659 RVA: 0x000E0ADA File Offset: 0x000DECDA
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060029A4 RID: 10660 RVA: 0x000E0ADA File Offset: 0x000DECDA
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x060029A5 RID: 10661 RVA: 0x000E0AE4 File Offset: 0x000DECE4
	public void Setup()
	{
		this._transform = base.transform;
		if (this._lineRenderer == null)
		{
			this._lineRenderer = base.GetComponent<LineRenderer>();
		}
		if (!this._lineRenderer)
		{
			return;
		}
		this._lineRenderer.useWorldSpace = true;
		Vector3 position = this._transform.position;
		Vector3 forward = this._transform.forward;
		int num = this._segments + 1;
		this._points = new Vector3[num];
		float d = this._length / (float)this._segments;
		for (int i = 0; i < num; i++)
		{
			this._points[i] = position - forward * d * (float)i;
		}
		this._lineRenderer.positionCount = num;
		this._lineRenderer.SetPositions(this._points);
		this.Update();
	}

	// Token: 0x060029A6 RID: 10662 RVA: 0x000E0BC2 File Offset: 0x000DEDC2
	private void Update()
	{
		if (!this.manualUpdate)
		{
			this.Update(Time.deltaTime);
		}
	}

	// Token: 0x060029A7 RID: 10663 RVA: 0x000E0BD8 File Offset: 0x000DEDD8
	private void FixedUpdate()
	{
		if (!this.applyPhysics)
		{
			return;
		}
		float deltaTime = Time.deltaTime;
		int num = this._points.Length - 1;
		float num2 = this._length / (float)num;
		for (int i = 1; i < num; i++)
		{
			float time = (float)(i - 1) / (float)num;
			float num3 = this.gravityCurve.Evaluate(time);
			Vector3 b = this.gravity * (num3 * deltaTime);
			this._points[i] += b;
			this._points[i + 1] += b;
		}
	}

	// Token: 0x060029A8 RID: 10664 RVA: 0x000E0C7C File Offset: 0x000DEE7C
	public void Update(float dt)
	{
		float num = this._length / (float)(this._segments - 1);
		Vector3 position = this._transform.position;
		this._points[0] = position;
		float num2 = Vector3.Distance(this._points[0], this._points[1]);
		float num3 = num - num2;
		if (num2 > num)
		{
			Array.Copy(this._points, 0, this._points, 1, this._points.Length - 1);
		}
		for (int i = 0; i < this._points.Length - 1; i++)
		{
			Vector3 vector = this._points[i];
			Vector3 vector2 = this._points[i + 1] - vector;
			if (vector2.sqrMagnitude > num * num)
			{
				this._points[i + 1] = vector + vector2.normalized * num;
			}
		}
		if (num3 > 0f)
		{
			int num4 = this._points.Length - 1;
			int num5 = num4 - 1;
			Vector3 vector3 = this._points[num4] - this._points[num5];
			Vector3 a = vector3.normalized;
			if (this.applyPhysics)
			{
				Vector3 normalized = (this._points[num5] - this._points[num5 - 1]).normalized;
				a = Vector3.Lerp(a, normalized, 0.5f);
			}
			this._points[num4] = this._points[num5] + a * Math.Min(vector3.magnitude, num3);
		}
		this._lineRenderer.SetPositions(this._points);
	}

	// Token: 0x060029A9 RID: 10665 RVA: 0x000E0E34 File Offset: 0x000DF034
	private static float CalcLength(in Vector3[] positions)
	{
		float num = 0f;
		for (int i = 0; i < positions.Length - 1; i++)
		{
			num += Vector3.Distance(positions[i], positions[i + 1]);
		}
		return num;
	}

	// Token: 0x04003648 RID: 13896
	[SerializeField]
	private Transform _transform;

	// Token: 0x04003649 RID: 13897
	[SerializeField]
	private LineRenderer _lineRenderer;

	// Token: 0x0400364A RID: 13898
	[SerializeField]
	[Range(1f, 128f)]
	private int _segments = 8;

	// Token: 0x0400364B RID: 13899
	[SerializeField]
	private float _length = 8f;

	// Token: 0x0400364C RID: 13900
	public bool manualUpdate;

	// Token: 0x0400364D RID: 13901
	[Space]
	public bool applyPhysics;

	// Token: 0x0400364E RID: 13902
	public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

	// Token: 0x0400364F RID: 13903
	public AnimationCurve gravityCurve = AnimationCurves.EaseInCubic;

	// Token: 0x04003650 RID: 13904
	[Space]
	private Vector3[] _points = new Vector3[8];
}
