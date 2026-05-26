using System;
using UnityEngine;

// Token: 0x020000B2 RID: 178
public class GorillaPressableDelayButton : GorillaPressableButton, IGorillaSliceableSimple
{
	// Token: 0x1400000B RID: 11
	// (add) Token: 0x06000442 RID: 1090 RVA: 0x00018C38 File Offset: 0x00016E38
	// (remove) Token: 0x06000443 RID: 1091 RVA: 0x00018C70 File Offset: 0x00016E70
	public event Action onPressBegin;

	// Token: 0x1400000C RID: 12
	// (add) Token: 0x06000444 RID: 1092 RVA: 0x00018CA8 File Offset: 0x00016EA8
	// (remove) Token: 0x06000445 RID: 1093 RVA: 0x00018CE0 File Offset: 0x00016EE0
	public event Action onPressAbort;

	// Token: 0x06000446 RID: 1094 RVA: 0x00018D18 File Offset: 0x00016F18
	private void Awake()
	{
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00018D54 File Offset: 0x00016F54
	private new void OnTriggerEnter(Collider collider)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.touchTime + this.debounceTime >= Time.time)
		{
			return;
		}
		if (this.touching)
		{
			return;
		}
		if (collider.GetComponentInParent<GorillaTriggerColliderHandIndicator>() == null)
		{
			return;
		}
		this.touching = collider;
		this.pressStartTime = Time.unscaledTime;
		this.progress = 0f;
		this.UpdateFillBar();
		Action action = this.onPressBegin;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00018DCF File Offset: 0x00016FCF
	private void OnTriggerExit(Collider other)
	{
		if (other != this.touching)
		{
			return;
		}
		this.touching = null;
		this.progress = 0f;
		this.UpdateFillBar();
		Action action = this.onPressAbort;
		if (action == null)
		{
			return;
		}
		action();
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x00018E08 File Offset: 0x00017008
	public new void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00018E11 File Offset: 0x00017011
	public new void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x0600044B RID: 1099 RVA: 0x00018E1C File Offset: 0x0001701C
	public void SliceUpdate()
	{
		if (this.touching == null)
		{
			return;
		}
		float num = Time.unscaledTime - this.pressStartTime;
		this.progress = ((this.delayTime > 0f) ? Mathf.Clamp01(num / this.delayTime) : ((num > 0f) ? 1f : 0f));
		if (num > this.delayTime)
		{
			base.OnTriggerEnter(this.touching);
			this.touching = null;
			this.progress = 0f;
		}
		this.UpdateFillBar();
	}

	// Token: 0x0600044C RID: 1100 RVA: 0x00018EA8 File Offset: 0x000170A8
	public void SetFillBar(Transform newFillBar)
	{
		this.fillBar = newFillBar;
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale = (this.fillbarStartingScale = this.fillBar.localScale);
		this.UpdateFillBar();
	}

	// Token: 0x0600044D RID: 1101 RVA: 0x00018EEB File Offset: 0x000170EB
	private void UpdateFillBar()
	{
		if (this.fillBar == null)
		{
			return;
		}
		this.fillBarScale.x = this.fillbarStartingScale.x * this.progress;
		this.fillBar.localScale = this.fillBarScale;
	}

	// Token: 0x040004AD RID: 1197
	private Collider touching;

	// Token: 0x040004AE RID: 1198
	private float pressStartTime;

	// Token: 0x040004AF RID: 1199
	private float progress;

	// Token: 0x040004B0 RID: 1200
	[SerializeField]
	[Range(0.01f, 5f)]
	public float delayTime = 1f;

	// Token: 0x040004B1 RID: 1201
	[SerializeField]
	private Transform fillBar;

	// Token: 0x040004B2 RID: 1202
	private Vector3 fillbarStartingScale;

	// Token: 0x040004B3 RID: 1203
	private Vector3 fillBarScale;
}
