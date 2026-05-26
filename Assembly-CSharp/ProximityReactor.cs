using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AEA RID: 2794
public class ProximityReactor : MonoBehaviour
{
	// Token: 0x170006A1 RID: 1697
	// (get) Token: 0x0600473D RID: 18237 RVA: 0x001801DD File Offset: 0x0017E3DD
	public float proximityRange
	{
		get
		{
			return this.proximityMax - this.proximityMin;
		}
	}

	// Token: 0x170006A2 RID: 1698
	// (get) Token: 0x0600473E RID: 18238 RVA: 0x001801EC File Offset: 0x0017E3EC
	public float distance
	{
		get
		{
			return this._distance;
		}
	}

	// Token: 0x170006A3 RID: 1699
	// (get) Token: 0x0600473F RID: 18239 RVA: 0x001801F4 File Offset: 0x0017E3F4
	public float distanceLinear
	{
		get
		{
			return this._distanceLinear;
		}
	}

	// Token: 0x06004740 RID: 18240 RVA: 0x001801FC File Offset: 0x0017E3FC
	public void SetRigFrom()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.from = componentInParent.transform;
		}
	}

	// Token: 0x06004741 RID: 18241 RVA: 0x00180228 File Offset: 0x0017E428
	public void SetRigTo()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent != null)
		{
			this.to = componentInParent.transform;
		}
	}

	// Token: 0x06004742 RID: 18242 RVA: 0x00180252 File Offset: 0x0017E452
	public void SetTransformFrom(Transform t)
	{
		this.from = t;
	}

	// Token: 0x06004743 RID: 18243 RVA: 0x0018025B File Offset: 0x0017E45B
	public void SetTransformTo(Transform t)
	{
		this.to = t;
	}

	// Token: 0x06004744 RID: 18244 RVA: 0x00180264 File Offset: 0x0017E464
	private void Setup()
	{
		this._distance = 0f;
		this._distanceLinear = 0f;
	}

	// Token: 0x06004745 RID: 18245 RVA: 0x0018027C File Offset: 0x0017E47C
	private void OnEnable()
	{
		this.Setup();
	}

	// Token: 0x06004746 RID: 18246 RVA: 0x00180284 File Offset: 0x0017E484
	private void Update()
	{
		if (!this.from || !this.to)
		{
			this._distance = 0f;
			this._distanceLinear = 0f;
			return;
		}
		Vector3 position = this.from.position;
		float magnitude = (this.to.position - position).magnitude;
		if (!this._distance.Approx(magnitude, 1E-06f))
		{
			UnityEvent<float> unityEvent = this.onProximityChanged;
			if (unityEvent != null)
			{
				unityEvent.Invoke(magnitude);
			}
		}
		this._distance = magnitude;
		float num = this.proximityRange.Approx0(1E-06f) ? 0f : MathUtils.LinearUnclamped(magnitude, this.proximityMin, this.proximityMax, 0f, 1f);
		if (!this._distanceLinear.Approx(num, 1E-06f))
		{
			UnityEvent<float> unityEvent2 = this.onProximityChangedLinear;
			if (unityEvent2 != null)
			{
				unityEvent2.Invoke(num);
			}
		}
		this._distanceLinear = num;
		if (this._distanceLinear < 0f)
		{
			UnityEvent<float> unityEvent3 = this.onBelowMinProximity;
			if (unityEvent3 != null)
			{
				unityEvent3.Invoke(magnitude);
			}
		}
		if (this._distanceLinear > 1f)
		{
			UnityEvent<float> unityEvent4 = this.onAboveMaxProximity;
			if (unityEvent4 == null)
			{
				return;
			}
			unityEvent4.Invoke(magnitude);
		}
	}

	// Token: 0x040059B3 RID: 22963
	public Transform from;

	// Token: 0x040059B4 RID: 22964
	public Transform to;

	// Token: 0x040059B5 RID: 22965
	[Space]
	public float proximityMin;

	// Token: 0x040059B6 RID: 22966
	public float proximityMax = 1f;

	// Token: 0x040059B7 RID: 22967
	[Space]
	[NonSerialized]
	private float _distance;

	// Token: 0x040059B8 RID: 22968
	[NonSerialized]
	private float _distanceLinear;

	// Token: 0x040059B9 RID: 22969
	[Space]
	public UnityEvent<float> onProximityChanged;

	// Token: 0x040059BA RID: 22970
	public UnityEvent<float> onProximityChangedLinear;

	// Token: 0x040059BB RID: 22971
	[Space]
	public UnityEvent<float> onBelowMinProximity;

	// Token: 0x040059BC RID: 22972
	public UnityEvent<float> onAboveMaxProximity;
}
