using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x02000581 RID: 1409
public class GumBubble : LerpComponent
{
	// Token: 0x060023AC RID: 9132 RVA: 0x000BFF75 File Offset: 0x000BE175
	private void Awake()
	{
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060023AD RID: 9133 RVA: 0x000BFF8A File Offset: 0x000BE18A
	public void InflateDelayed()
	{
		this.InflateDelayed(this._delayInflate);
	}

	// Token: 0x060023AE RID: 9134 RVA: 0x000BFF98 File Offset: 0x000BE198
	public void InflateDelayed(float delay)
	{
		if (delay < 0f)
		{
			delay = 0f;
		}
		base.Invoke("Inflate", delay);
	}

	// Token: 0x060023AF RID: 9135 RVA: 0x000BFFB8 File Offset: 0x000BE1B8
	public void Inflate()
	{
		base.gameObject.SetActive(true);
		base.enabled = true;
		if (this._animating)
		{
			return;
		}
		this._animating = true;
		this._sinceInflate = 0f;
		if (this.audioSource != null && this._sfxInflate != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxInflate, 1f);
		}
		UnityEvent unityEvent = this.onInflate;
		if (unityEvent == null)
		{
			return;
		}
		unityEvent.Invoke();
	}

	// Token: 0x060023B0 RID: 9136 RVA: 0x000C003C File Offset: 0x000BE23C
	public void Pop()
	{
		this._lerp = 0f;
		base.RenderLerp();
		if (this.audioSource != null && this._sfxPop != null)
		{
			this.audioSource.GTPlayOneShot(this._sfxPop, 1f);
		}
		UnityEvent unityEvent = this.onPop;
		if (unityEvent != null)
		{
			unityEvent.Invoke();
		}
		this._done = false;
		this._animating = false;
		base.enabled = false;
		base.gameObject.SetActive(false);
	}

	// Token: 0x060023B1 RID: 9137 RVA: 0x000C00C0 File Offset: 0x000BE2C0
	private void Update()
	{
		float t = Mathf.Clamp01(this._sinceInflate / this._lerpLength);
		this._lerp = Mathf.Lerp(0f, 1f, t);
		if (this._lerp <= 1f && !this._done)
		{
			base.RenderLerp();
			if (Mathf.Approximately(this._lerp, 1f))
			{
				this._done = true;
			}
		}
		float num = this._lerpLength + this._delayPop;
		if (this._sinceInflate >= num)
		{
			this.Pop();
		}
	}

	// Token: 0x060023B2 RID: 9138 RVA: 0x000C0154 File Offset: 0x000BE354
	protected override void OnLerp(float t)
	{
		if (!this.target)
		{
			return;
		}
		if (this._lerpCurve == null)
		{
			GTDev.LogError<string>("[GumBubble] Missing lerp curve", this, null);
			return;
		}
		this.target.localScale = this.targetScale * this._lerpCurve.Evaluate(t);
	}

	// Token: 0x04002ECE RID: 11982
	public Transform target;

	// Token: 0x04002ECF RID: 11983
	public Vector3 targetScale = Vector3.one;

	// Token: 0x04002ED0 RID: 11984
	[SerializeField]
	private AnimationCurve _lerpCurve;

	// Token: 0x04002ED1 RID: 11985
	public AudioSource audioSource;

	// Token: 0x04002ED2 RID: 11986
	[SerializeField]
	private AudioClip _sfxInflate;

	// Token: 0x04002ED3 RID: 11987
	[SerializeField]
	private AudioClip _sfxPop;

	// Token: 0x04002ED4 RID: 11988
	[SerializeField]
	private float _delayInflate = 1.16f;

	// Token: 0x04002ED5 RID: 11989
	[FormerlySerializedAs("_popDelay")]
	[SerializeField]
	private float _delayPop = 0.5f;

	// Token: 0x04002ED6 RID: 11990
	[SerializeField]
	private bool _animating;

	// Token: 0x04002ED7 RID: 11991
	public UnityEvent onPop;

	// Token: 0x04002ED8 RID: 11992
	public UnityEvent onInflate;

	// Token: 0x04002ED9 RID: 11993
	[NonSerialized]
	private bool _done;

	// Token: 0x04002EDA RID: 11994
	[NonSerialized]
	private TimeSince _sinceInflate;
}
