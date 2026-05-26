using System;
using UnityEngine;

// Token: 0x020009BD RID: 2493
public class RangedFloat : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x170005DC RID: 1500
	// (get) Token: 0x06003FC7 RID: 16327 RVA: 0x00155023 File Offset: 0x00153223
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x170005DD RID: 1501
	// (get) Token: 0x06003FC8 RID: 16328 RVA: 0x0015502B File Offset: 0x0015322B
	public float Range
	{
		get
		{
			return this._max - this._min;
		}
	}

	// Token: 0x170005DE RID: 1502
	// (get) Token: 0x06003FC9 RID: 16329 RVA: 0x0015503A File Offset: 0x0015323A
	// (set) Token: 0x06003FCA RID: 16330 RVA: 0x00155042 File Offset: 0x00153242
	public float Min
	{
		get
		{
			return this._min;
		}
		set
		{
			this._min = value;
		}
	}

	// Token: 0x170005DF RID: 1503
	// (get) Token: 0x06003FCB RID: 16331 RVA: 0x0015504B File Offset: 0x0015324B
	// (set) Token: 0x06003FCC RID: 16332 RVA: 0x00155053 File Offset: 0x00153253
	public float Max
	{
		get
		{
			return this._max;
		}
		set
		{
			this._max = value;
		}
	}

	// Token: 0x170005E0 RID: 1504
	// (get) Token: 0x06003FCD RID: 16333 RVA: 0x0015505C File Offset: 0x0015325C
	// (set) Token: 0x06003FCE RID: 16334 RVA: 0x00155091 File Offset: 0x00153291
	public float normalized
	{
		get
		{
			if (!this.Range.Approx0(1E-06f))
			{
				return (this._value - this._min) / (this._max - this.Min);
			}
			return 0f;
		}
		set
		{
			this._value = this._min + Mathf.Clamp01(value) * (this._max - this._min);
		}
	}

	// Token: 0x170005E1 RID: 1505
	// (get) Token: 0x06003FCF RID: 16335 RVA: 0x001550B4 File Offset: 0x001532B4
	public float curved
	{
		get
		{
			return this._min + this._curve.Evaluate(this.normalized) * (this._max - this._min);
		}
	}

	// Token: 0x06003FD0 RID: 16336 RVA: 0x001550DC File Offset: 0x001532DC
	public float Get()
	{
		return this._value;
	}

	// Token: 0x06003FD1 RID: 16337 RVA: 0x001550E4 File Offset: 0x001532E4
	public void Set(float f)
	{
		this._value = Mathf.Clamp(f, this._min, this._max);
	}

	// Token: 0x0400503F RID: 20543
	[SerializeField]
	private float _value = 0.5f;

	// Token: 0x04005040 RID: 20544
	[SerializeField]
	private float _min;

	// Token: 0x04005041 RID: 20545
	[SerializeField]
	private float _max = 1f;

	// Token: 0x04005042 RID: 20546
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
}
