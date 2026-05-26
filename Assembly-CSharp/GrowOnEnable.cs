using System;
using UnityEngine;

// Token: 0x02000131 RID: 305
public class GrowOnEnable : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06000794 RID: 1940 RVA: 0x0002A05B File Offset: 0x0002825B
	private void Awake()
	{
		this._targetScale = base.transform.localScale;
	}

	// Token: 0x06000795 RID: 1941 RVA: 0x0002A06E File Offset: 0x0002826E
	private void OnEnable()
	{
		this._lerpVal = 0f;
		this._curve = AnimationCurves.GetCurveForEase(this.easeType);
		this.UpdateScale();
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0002A098 File Offset: 0x00028298
	private void OnDisable()
	{
		base.transform.localScale = this._targetScale;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06000797 RID: 1943 RVA: 0x0002A0B1 File Offset: 0x000282B1
	// (set) Token: 0x06000798 RID: 1944 RVA: 0x0002A0B9 File Offset: 0x000282B9
	public bool TickRunning { get; set; }

	// Token: 0x06000799 RID: 1945 RVA: 0x0002A0C2 File Offset: 0x000282C2
	public void Tick()
	{
		this._lerpVal = Mathf.Clamp01(this._lerpVal + Time.deltaTime / this.growDuration);
		this.UpdateScale();
		if (this._lerpVal >= 1f)
		{
			TickSystem<object>.RemoveTickCallback(this);
		}
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0002A0FB File Offset: 0x000282FB
	private void UpdateScale()
	{
		base.transform.localScale = this._targetScale * this._curve.Evaluate(this._lerpVal);
	}

	// Token: 0x040009B3 RID: 2483
	[SerializeField]
	private float growDuration = 1f;

	// Token: 0x040009B4 RID: 2484
	[SerializeField]
	private AnimationCurves.EaseType easeType = AnimationCurves.EaseType.EaseOutBack;

	// Token: 0x040009B5 RID: 2485
	private AnimationCurve _curve;

	// Token: 0x040009B6 RID: 2486
	private Vector3 _targetScale;

	// Token: 0x040009B7 RID: 2487
	private float _lerpVal;
}
