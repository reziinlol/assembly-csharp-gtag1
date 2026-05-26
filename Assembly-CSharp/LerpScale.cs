using System;
using UnityEngine;

// Token: 0x02000ADB RID: 2779
public class LerpScale : LerpComponent
{
	// Token: 0x060046F0 RID: 18160 RVA: 0x0017F018 File Offset: 0x0017D218
	protected override void OnLerp(float t)
	{
		this.current = Vector3.Lerp(this.start, this.end, this.scaleCurve.Evaluate(t));
		if (this.target)
		{
			this.target.localScale = this.current;
		}
	}

	// Token: 0x04005943 RID: 22851
	[Space]
	public Transform target;

	// Token: 0x04005944 RID: 22852
	[Space]
	public Vector3 start = Vector3.one;

	// Token: 0x04005945 RID: 22853
	public Vector3 end = Vector3.one;

	// Token: 0x04005946 RID: 22854
	public Vector3 current;

	// Token: 0x04005947 RID: 22855
	[SerializeField]
	private AnimationCurve scaleCurve = AnimationCurves.EaseInOutBounce;
}
