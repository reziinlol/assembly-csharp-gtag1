using System;
using System.Diagnostics;
using UnityEngine;

// Token: 0x02000AD9 RID: 2777
public abstract class LerpComponent : MonoBehaviour
{
	// Token: 0x17000694 RID: 1684
	// (get) Token: 0x060046E0 RID: 18144 RVA: 0x0017EF3C File Offset: 0x0017D13C
	// (set) Token: 0x060046E1 RID: 18145 RVA: 0x0017EF44 File Offset: 0x0017D144
	public float Lerp
	{
		get
		{
			return this._lerp;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (!Mathf.Approximately(this._lerp, num))
			{
				LerpChangedEvent onLerpChanged = this._onLerpChanged;
				if (onLerpChanged != null)
				{
					onLerpChanged.Invoke(num);
				}
			}
			this._lerp = num;
		}
	}

	// Token: 0x17000695 RID: 1685
	// (get) Token: 0x060046E2 RID: 18146 RVA: 0x0017EF7F File Offset: 0x0017D17F
	// (set) Token: 0x060046E3 RID: 18147 RVA: 0x0017EF87 File Offset: 0x0017D187
	public float LerpTime
	{
		get
		{
			return this._lerpLength;
		}
		set
		{
			this._lerpLength = ((value < 0f) ? 0f : value);
		}
	}

	// Token: 0x17000696 RID: 1686
	// (get) Token: 0x060046E4 RID: 18148 RVA: 0x00023994 File Offset: 0x00021B94
	protected virtual bool CanRender
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060046E5 RID: 18149
	protected abstract void OnLerp(float t);

	// Token: 0x060046E6 RID: 18150 RVA: 0x0017EF9F File Offset: 0x0017D19F
	protected void RenderLerp()
	{
		this.OnLerp(this._lerp);
	}

	// Token: 0x060046E7 RID: 18151 RVA: 0x0017EFB0 File Offset: 0x0017D1B0
	protected virtual int GetState()
	{
		return new ValueTuple<float, int>(this._lerp, 779562875).GetHashCode();
	}

	// Token: 0x060046E8 RID: 18152 RVA: 0x0017EFDB File Offset: 0x0017D1DB
	protected virtual void Validate()
	{
		if (this._lerpLength < 0f)
		{
			this._lerpLength = 0f;
		}
	}

	// Token: 0x060046E9 RID: 18153 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmosSelected()
	{
	}

	// Token: 0x060046EA RID: 18154 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	private void TryEditorRender(bool playModeCheck = true)
	{
	}

	// Token: 0x060046EB RID: 18155 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	private void LerpToOne()
	{
	}

	// Token: 0x060046EC RID: 18156 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	private void LerpToZero()
	{
	}

	// Token: 0x060046ED RID: 18157 RVA: 0x000028C5 File Offset: 0x00000AC5
	[Conditional("UNITY_EDITOR")]
	private void StartPreview(float lerpFrom, float lerpTo)
	{
	}

	// Token: 0x04005939 RID: 22841
	[SerializeField]
	[Range(0f, 1f)]
	protected float _lerp;

	// Token: 0x0400593A RID: 22842
	[SerializeField]
	protected float _lerpLength = 1f;

	// Token: 0x0400593B RID: 22843
	[Space]
	[SerializeField]
	protected LerpChangedEvent _onLerpChanged;

	// Token: 0x0400593C RID: 22844
	[SerializeField]
	protected bool _previewInEditor = true;

	// Token: 0x0400593D RID: 22845
	[NonSerialized]
	private bool _previewing;

	// Token: 0x0400593E RID: 22846
	[NonSerialized]
	private bool _cancelPreview;

	// Token: 0x0400593F RID: 22847
	[NonSerialized]
	private bool _rendering;

	// Token: 0x04005940 RID: 22848
	[NonSerialized]
	private int _lastState;

	// Token: 0x04005941 RID: 22849
	[NonSerialized]
	private float _prevLerpFrom;

	// Token: 0x04005942 RID: 22850
	[NonSerialized]
	private float _prevLerpTo;
}
