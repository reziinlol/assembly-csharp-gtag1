using System;
using UnityEngine;

// Token: 0x0200084E RID: 2126
public class GorillaCaveCrystal : Tappable
{
	// Token: 0x0600371B RID: 14107 RVA: 0x0012E96C File Offset: 0x0012CB6C
	private void Awake()
	{
		if (this.tapScript == null)
		{
			this.tapScript = base.GetComponent<TapInnerGlow>();
		}
	}

	// Token: 0x0600371C RID: 14108 RVA: 0x0012E988 File Offset: 0x0012CB88
	public override void OnTapLocal(float tapStrength, float tapTime, PhotonMessageInfoWrapped info)
	{
		this._tapStrength = tapStrength;
		this.AnimateCrystal();
	}

	// Token: 0x0600371D RID: 14109 RVA: 0x0012E997 File Offset: 0x0012CB97
	private void AnimateCrystal()
	{
		if (this.tapScript)
		{
			this.tapScript.Tap();
		}
	}

	// Token: 0x0400472A RID: 18218
	public bool overrideSoundAndMaterial;

	// Token: 0x0400472B RID: 18219
	public CrystalOctave octave;

	// Token: 0x0400472C RID: 18220
	public CrystalNote note;

	// Token: 0x0400472D RID: 18221
	[SerializeField]
	private MeshRenderer _crystalRenderer;

	// Token: 0x0400472E RID: 18222
	public TapInnerGlow tapScript;

	// Token: 0x0400472F RID: 18223
	[HideInInspector]
	public GorillaCaveCrystalVisuals visuals;

	// Token: 0x04004730 RID: 18224
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpInCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04004731 RID: 18225
	[HideInInspector]
	[SerializeField]
	private AnimationCurve _lerpOutCurve = AnimationCurve.Constant(0f, 1f, 1f);

	// Token: 0x04004732 RID: 18226
	[HideInInspector]
	[SerializeField]
	private bool _animating;

	// Token: 0x04004733 RID: 18227
	[HideInInspector]
	[SerializeField]
	[Range(0f, 1f)]
	private float _tapStrength = 1f;

	// Token: 0x04004734 RID: 18228
	[NonSerialized]
	private TimeSince _timeSinceLastTap;
}
