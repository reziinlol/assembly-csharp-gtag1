using System;
using System.Collections.Generic;
using GorillaLocomotion;
using UnityEngine;

// Token: 0x0200090F RID: 2319
public class SizeManager : MonoBehaviour
{
	// Token: 0x1700057A RID: 1402
	// (get) Token: 0x06003C9C RID: 15516 RVA: 0x00149FB0 File Offset: 0x001481B0
	public float currentScale
	{
		get
		{
			if (this.targetRig != null)
			{
				return this.targetRig.ScaleMultiplier;
			}
			if (this.targetPlayer != null)
			{
				return this.targetPlayer.ScaleMultiplier;
			}
			return 1f;
		}
	}

	// Token: 0x1700057B RID: 1403
	// (get) Token: 0x06003C9D RID: 15517 RVA: 0x00149FEB File Offset: 0x001481EB
	// (set) Token: 0x06003C9E RID: 15518 RVA: 0x0014A020 File Offset: 0x00148220
	public int currentSizeLayerMaskValue
	{
		get
		{
			if (this.targetPlayer)
			{
				return this.targetPlayer.sizeLayerMask;
			}
			if (this.targetRig)
			{
				return this.targetRig.SizeLayerMask;
			}
			return 1;
		}
		set
		{
			if (this.targetPlayer)
			{
				this.targetPlayer.sizeLayerMask = value;
				if (this.targetRig != null)
				{
					this.targetRig.SizeLayerMask = value;
					return;
				}
			}
			else if (this.targetRig)
			{
				this.targetRig.SizeLayerMask = value;
			}
		}
	}

	// Token: 0x06003C9F RID: 15519 RVA: 0x0014A07A File Offset: 0x0014827A
	private void OnDisable()
	{
		this.touchingChangers.Clear();
		this.currentSizeLayerMaskValue = 1;
		SizeManagerManager.UnregisterSM(this);
	}

	// Token: 0x06003CA0 RID: 15520 RVA: 0x0014A094 File Offset: 0x00148294
	private void OnEnable()
	{
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06003CA1 RID: 15521 RVA: 0x0014A09C File Offset: 0x0014829C
	private void CollectLineRenderers(GameObject obj)
	{
		this.lineRenderers = obj.GetComponentsInChildren<LineRenderer>(true);
		int num = this.lineRenderers.Length;
		foreach (LineRenderer lineRenderer in this.lineRenderers)
		{
			this.initLineScalar.Add(lineRenderer.widthMultiplier);
		}
	}

	// Token: 0x06003CA2 RID: 15522 RVA: 0x0014A0EC File Offset: 0x001482EC
	public void BuildInitialize()
	{
		this.rate = 650f;
		if (this.targetRig != null)
		{
			this.CollectLineRenderers(this.targetRig.gameObject);
		}
		else if (this.targetPlayer != null)
		{
			this.CollectLineRenderers(GorillaTagger.Instance.offlineVRRig.gameObject);
		}
		this.mainCameraTransform = Camera.main.transform;
		if (this.targetPlayer != null)
		{
			this.myType = SizeManager.SizeChangerType.LocalOffline;
		}
		else if (this.targetRig != null && !this.targetRig.isOfflineVRRig && this.targetRig.netView != null && this.targetRig.netView.Owner != NetworkSystem.Instance.LocalPlayer)
		{
			this.myType = SizeManager.SizeChangerType.OtherOnline;
		}
		else
		{
			this.myType = SizeManager.SizeChangerType.LocalOnline;
		}
		this.buildInitialized = true;
	}

	// Token: 0x06003CA3 RID: 15523 RVA: 0x0014A1D0 File Offset: 0x001483D0
	private void Awake()
	{
		if (!this.buildInitialized)
		{
			this.BuildInitialize();
		}
		SizeManagerManager.RegisterSM(this);
	}

	// Token: 0x06003CA4 RID: 15524 RVA: 0x0014A1E8 File Offset: 0x001483E8
	public void InvokeFixedUpdate()
	{
		float num = 1f;
		SizeChanger sizeChanger = this.ControllingChanger(this.targetRig.transform);
		switch (this.myType)
		{
		case SizeManager.SizeChangerType.LocalOffline:
			num = this.ScaleFromChanger(sizeChanger, this.mainCameraTransform, Time.fixedDeltaTime);
			this.targetPlayer.SetScaleMultiplier((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.LocalOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		case SizeManager.SizeChangerType.OtherOnline:
			num = this.ScaleFromChanger(sizeChanger, this.targetRig.transform, Time.fixedDeltaTime);
			this.targetRig.ScaleMultiplier = ((num == 1f) ? this.SizeOverTime(num, 0.33f, Time.fixedDeltaTime) : num);
			break;
		}
		if (num != this.lastScale)
		{
			for (int i = 0; i < this.lineRenderers.Length; i++)
			{
				this.lineRenderers[i].widthMultiplier = num * this.initLineScalar[i];
			}
			Vector3 scaleCenter;
			if (sizeChanger != null && sizeChanger.TryGetScaleCenterPoint(out scaleCenter))
			{
				if (this.myType == SizeManager.SizeChangerType.LocalOffline)
				{
					this.targetPlayer.ScaleAwayFromPoint(this.lastScale, num, scaleCenter);
				}
				else if (this.myType == SizeManager.SizeChangerType.LocalOnline)
				{
					GTPlayer.Instance.ScaleAwayFromPoint(this.lastScale, num, scaleCenter);
				}
			}
			if (this.myType == SizeManager.SizeChangerType.LocalOffline)
			{
				this.CheckSizeChangeEvents(num);
			}
		}
		this.lastScale = num;
	}

	// Token: 0x06003CA5 RID: 15525 RVA: 0x0014A388 File Offset: 0x00148588
	private SizeChanger ControllingChanger(Transform t)
	{
		for (int i = this.touchingChangers.Count - 1; i >= 0; i--)
		{
			SizeChanger sizeChanger = this.touchingChangers[i];
			if (!(sizeChanger == null) && sizeChanger.gameObject.activeInHierarchy && (sizeChanger.SizeLayerMask & this.currentSizeLayerMaskValue) != 0 && (sizeChanger.alwaysControlWhenEntered || (sizeChanger.ClosestPoint(t.position) - t.position).magnitude < this.magnitudeThreshold))
			{
				return sizeChanger;
			}
		}
		return null;
	}

	// Token: 0x06003CA6 RID: 15526 RVA: 0x0014A414 File Offset: 0x00148614
	private float ScaleFromChanger(SizeChanger sC, Transform t, float deltaTime)
	{
		if (sC == null)
		{
			return 1f;
		}
		switch (sC.MyType)
		{
		case SizeChanger.ChangerType.Static:
			return this.SizeOverTime(sC.MinScale, sC.StaticEasing, deltaTime);
		case SizeChanger.ChangerType.Continuous:
		{
			Vector3 vector = Vector3.Project(t.position - sC.StartPos.position, sC.EndPos.position - sC.StartPos.position);
			return Mathf.Clamp(sC.MaxScale - vector.magnitude / (sC.StartPos.position - sC.EndPos.position).magnitude * (sC.MaxScale - sC.MinScale), sC.MinScale, sC.MaxScale);
		}
		case SizeChanger.ChangerType.Radius:
		{
			float value = Vector3.Distance(t.position, sC.StartPos.position);
			float t2 = Mathf.InverseLerp(sC.startRadius, sC.endRadius, value);
			return Mathf.Lerp(sC.MinScale, sC.MaxScale, t2);
		}
		default:
			return 1f;
		}
	}

	// Token: 0x06003CA7 RID: 15527 RVA: 0x0014A52E File Offset: 0x0014872E
	private float SizeOverTime(float targetSize, float easing, float deltaTime)
	{
		if (easing <= 0f || Mathf.Abs(this.targetRig.ScaleMultiplier - targetSize) < 0.05f)
		{
			return targetSize;
		}
		return Mathf.MoveTowards(this.targetRig.ScaleMultiplier, targetSize, deltaTime / easing);
	}

	// Token: 0x06003CA8 RID: 15528 RVA: 0x0014A568 File Offset: 0x00148768
	private void CheckSizeChangeEvents(float newSize)
	{
		if (newSize < this.smallThreshold)
		{
			if (!this.isSmall)
			{
				this.isSmall = true;
				this.isLarge = false;
				PlayerGameEvents.MiscEvent("SizeSmall", 1);
				return;
			}
		}
		else if (newSize > this.largeThreshold)
		{
			if (!this.isLarge)
			{
				this.isLarge = true;
				this.isSmall = false;
				PlayerGameEvents.MiscEvent("SizeLarge", 1);
				return;
			}
		}
		else
		{
			this.isLarge = false;
			this.isSmall = false;
		}
	}

	// Token: 0x04004D45 RID: 19781
	public List<SizeChanger> touchingChangers;

	// Token: 0x04004D46 RID: 19782
	private LineRenderer[] lineRenderers;

	// Token: 0x04004D47 RID: 19783
	private List<float> initLineScalar = new List<float>();

	// Token: 0x04004D48 RID: 19784
	public VRRig targetRig;

	// Token: 0x04004D49 RID: 19785
	public GTPlayer targetPlayer;

	// Token: 0x04004D4A RID: 19786
	public float magnitudeThreshold = 0.01f;

	// Token: 0x04004D4B RID: 19787
	public float rate = 650f;

	// Token: 0x04004D4C RID: 19788
	public Transform mainCameraTransform;

	// Token: 0x04004D4D RID: 19789
	public SizeManager.SizeChangerType myType;

	// Token: 0x04004D4E RID: 19790
	public float lastScale;

	// Token: 0x04004D4F RID: 19791
	private bool buildInitialized;

	// Token: 0x04004D50 RID: 19792
	private const float returnToNormalEasing = 0.33f;

	// Token: 0x04004D51 RID: 19793
	private float smallThreshold = 0.6f;

	// Token: 0x04004D52 RID: 19794
	private float largeThreshold = 1.5f;

	// Token: 0x04004D53 RID: 19795
	private bool isSmall;

	// Token: 0x04004D54 RID: 19796
	private bool isLarge;

	// Token: 0x02000910 RID: 2320
	public enum SizeChangerType
	{
		// Token: 0x04004D56 RID: 19798
		LocalOffline,
		// Token: 0x04004D57 RID: 19799
		LocalOnline,
		// Token: 0x04004D58 RID: 19800
		OtherOnline
	}
}
