using System;
using UnityEngine;

// Token: 0x02000678 RID: 1656
public class CrossFadeAudioSources : MonoBehaviour, IRangedVariable<float>, IVariable<float>, IVariable
{
	// Token: 0x06002963 RID: 10595 RVA: 0x000DF9AD File Offset: 0x000DDBAD
	public void Play()
	{
		if (this.source1)
		{
			this.source1.Play();
		}
		if (this.source2)
		{
			this.source2.Play();
		}
	}

	// Token: 0x06002964 RID: 10596 RVA: 0x000DF9DF File Offset: 0x000DDBDF
	public void Stop()
	{
		if (this.source1)
		{
			this.source1.Stop();
		}
		if (this.source2)
		{
			this.source2.Stop();
		}
	}

	// Token: 0x06002965 RID: 10597 RVA: 0x000DFA14 File Offset: 0x000DDC14
	private void Update()
	{
		if (!this.source1 || !this.source2)
		{
			return;
		}
		float num = this._curve.Evaluate(this._lerp);
		float num2;
		if (this.tween)
		{
			num2 = MathUtils.Xlerp(this._lastT, num, Time.deltaTime, this.tweenSpeed);
		}
		else
		{
			num2 = (this.lerpByClipLength ? this._curve.Evaluate((float)this.source1.timeSamples / (float)this.source1.clip.samples) : num);
		}
		this._lastT = num2;
		this.source2.volume = num2;
		this.source1.volume = 1f - num2;
	}

	// Token: 0x06002966 RID: 10598 RVA: 0x000DFACA File Offset: 0x000DDCCA
	public float Get()
	{
		return this._lerp;
	}

	// Token: 0x06002967 RID: 10599 RVA: 0x000DFAD2 File Offset: 0x000DDCD2
	public void Set(float f)
	{
		this._lerp = Mathf.Clamp01(f);
	}

	// Token: 0x17000424 RID: 1060
	// (get) Token: 0x06002968 RID: 10600 RVA: 0x000DFAE0 File Offset: 0x000DDCE0
	// (set) Token: 0x06002969 RID: 10601 RVA: 0x000028C5 File Offset: 0x00000AC5
	public float Min
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	// Token: 0x17000425 RID: 1061
	// (get) Token: 0x0600296A RID: 10602 RVA: 0x000DFAE7 File Offset: 0x000DDCE7
	// (set) Token: 0x0600296B RID: 10603 RVA: 0x000028C5 File Offset: 0x00000AC5
	public float Max
	{
		get
		{
			return 1f;
		}
		set
		{
		}
	}

	// Token: 0x17000426 RID: 1062
	// (get) Token: 0x0600296C RID: 10604 RVA: 0x000DFAE7 File Offset: 0x000DDCE7
	public float Range
	{
		get
		{
			return 1f;
		}
	}

	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x0600296D RID: 10605 RVA: 0x000DFAEE File Offset: 0x000DDCEE
	public AnimationCurve Curve
	{
		get
		{
			return this._curve;
		}
	}

	// Token: 0x040035DF RID: 13791
	[SerializeField]
	private float _lerp;

	// Token: 0x040035E0 RID: 13792
	[SerializeField]
	private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	// Token: 0x040035E1 RID: 13793
	[Space]
	[SerializeField]
	private AudioSource source1;

	// Token: 0x040035E2 RID: 13794
	[SerializeField]
	private AudioSource source2;

	// Token: 0x040035E3 RID: 13795
	[Space]
	public bool lerpByClipLength;

	// Token: 0x040035E4 RID: 13796
	public bool tween;

	// Token: 0x040035E5 RID: 13797
	public float tweenSpeed = 16f;

	// Token: 0x040035E6 RID: 13798
	private float _lastT;
}
