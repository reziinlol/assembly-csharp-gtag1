using System;
using GorillaLocomotion.Swimming;
using UnityEngine;

// Token: 0x020003AF RID: 943
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class WaterRippleEffect : MonoBehaviour
{
	// Token: 0x060016BB RID: 5819 RVA: 0x00084261 File Offset: 0x00082461
	private void Awake()
	{
		this.animator = base.GetComponent<Animator>();
		this.renderer = base.GetComponent<SpriteRenderer>();
		this.ripplePlaybackSpeedHash = Animator.StringToHash(this.ripplePlaybackSpeedName);
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x0008428C File Offset: 0x0008248C
	public void Destroy()
	{
		this.waterVolume = null;
		ObjectPools.instance.Destroy(base.gameObject);
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x000842A8 File Offset: 0x000824A8
	public void PlayEffect(WaterVolume volume = null)
	{
		this.waterVolume = volume;
		this.rippleStartTime = Time.time;
		this.animator.SetFloat(this.ripplePlaybackSpeedHash, this.ripplePlaybackSpeed);
		if (this.waterVolume != null && this.waterVolume.Parameters != null)
		{
			this.renderer.color = this.waterVolume.Parameters.rippleSpriteColor;
		}
		Color color = this.renderer.color;
		color.a = 1f;
		this.renderer.color = color;
	}

	// Token: 0x060016BE RID: 5822 RVA: 0x00084340 File Offset: 0x00082540
	private void Update()
	{
		if (this.waterVolume != null && !this.waterVolume.isStationary && this.waterVolume.surfacePlane != null)
		{
			Vector3 b = Vector3.Dot(base.transform.position - this.waterVolume.surfacePlane.position, this.waterVolume.surfacePlane.up) * this.waterVolume.surfacePlane.up;
			base.transform.position = base.transform.position - b;
		}
		float num = Mathf.Clamp01((Time.time - this.rippleStartTime - this.fadeOutDelay) / this.fadeOutTime);
		Color color = this.renderer.color;
		color.a = 1f - num;
		this.renderer.color = color;
		if (num >= 1f - Mathf.Epsilon)
		{
			this.Destroy();
			return;
		}
	}

	// Token: 0x040021B6 RID: 8630
	[SerializeField]
	private float ripplePlaybackSpeed = 1f;

	// Token: 0x040021B7 RID: 8631
	[SerializeField]
	private float fadeOutDelay = 0.5f;

	// Token: 0x040021B8 RID: 8632
	[SerializeField]
	private float fadeOutTime = 1f;

	// Token: 0x040021B9 RID: 8633
	private string ripplePlaybackSpeedName = "RipplePlaybackSpeed";

	// Token: 0x040021BA RID: 8634
	private int ripplePlaybackSpeedHash;

	// Token: 0x040021BB RID: 8635
	private float rippleStartTime = -1f;

	// Token: 0x040021BC RID: 8636
	private Animator animator;

	// Token: 0x040021BD RID: 8637
	private SpriteRenderer renderer;

	// Token: 0x040021BE RID: 8638
	private WaterVolume waterVolume;
}
