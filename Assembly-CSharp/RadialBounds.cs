using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020009B9 RID: 2489
public class RadialBounds : MonoBehaviour
{
	// Token: 0x170005D8 RID: 1496
	// (get) Token: 0x06003FB5 RID: 16309 RVA: 0x00154ABA File Offset: 0x00152CBA
	// (set) Token: 0x06003FB6 RID: 16310 RVA: 0x00154AC2 File Offset: 0x00152CC2
	public Vector3 localCenter
	{
		get
		{
			return this._localCenter;
		}
		set
		{
			this._localCenter = value;
		}
	}

	// Token: 0x170005D9 RID: 1497
	// (get) Token: 0x06003FB7 RID: 16311 RVA: 0x00154ACB File Offset: 0x00152CCB
	// (set) Token: 0x06003FB8 RID: 16312 RVA: 0x00154AD3 File Offset: 0x00152CD3
	public float localRadius
	{
		get
		{
			return this._localRadius;
		}
		set
		{
			this._localRadius = value;
		}
	}

	// Token: 0x170005DA RID: 1498
	// (get) Token: 0x06003FB9 RID: 16313 RVA: 0x00154ADC File Offset: 0x00152CDC
	public Vector3 center
	{
		get
		{
			return base.transform.TransformPoint(this._localCenter);
		}
	}

	// Token: 0x170005DB RID: 1499
	// (get) Token: 0x06003FBA RID: 16314 RVA: 0x00154AEF File Offset: 0x00152CEF
	public float radius
	{
		get
		{
			return MathUtils.GetScaledRadius(this._localRadius, base.transform.lossyScale);
		}
	}

	// Token: 0x04005025 RID: 20517
	[SerializeField]
	private Vector3 _localCenter;

	// Token: 0x04005026 RID: 20518
	[SerializeField]
	private float _localRadius = 1f;

	// Token: 0x04005027 RID: 20519
	[Space]
	public UnityEvent<RadialBounds> onOverlapEnter;

	// Token: 0x04005028 RID: 20520
	public UnityEvent<RadialBounds> onOverlapExit;

	// Token: 0x04005029 RID: 20521
	public UnityEvent<RadialBounds, float> onOverlapStay;
}
