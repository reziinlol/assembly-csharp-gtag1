using System;
using GorillaExtensions;
using UnityEngine;

// Token: 0x020000C9 RID: 201
public class ShadeJumpscare : MonoBehaviour
{
	// Token: 0x060004E1 RID: 1249 RVA: 0x0001B27B File Offset: 0x0001947B
	private void Awake()
	{
		this.audioSource = base.GetComponent<AudioSource>();
	}

	// Token: 0x060004E2 RID: 1250 RVA: 0x0001B289 File Offset: 0x00019489
	private void OnEnable()
	{
		this.startTime = Time.time;
		this.startAngle = Random.value * 360f;
		this.audioSource.clip = this.audioClips.GetRandomItem<AudioClip>();
		this.audioSource.GTPlay();
	}

	// Token: 0x060004E3 RID: 1251 RVA: 0x0001B2C8 File Offset: 0x000194C8
	private void Update()
	{
		float num = Time.time - this.startTime;
		float time = num / this.animationTime;
		this.shadeTransform.SetPositionAndRotation(base.transform.position + new Vector3(0f, this.shadeHeightFunction.Evaluate(time), 0f), Quaternion.Euler(0f, this.startAngle + num * this.shadeRotationSpeed, 0f));
		float num2 = this.shadeScaleFunction.Evaluate(time);
		this.shadeTransform.localScale = new Vector3(num2, num2 * this.shadeYScaleMultFunction.Evaluate(time), num2);
		this.audioSource.volume = this.soundVolumeFunction.Evaluate(time);
	}

	// Token: 0x04000574 RID: 1396
	[SerializeField]
	private Transform shadeTransform;

	// Token: 0x04000575 RID: 1397
	[SerializeField]
	private float animationTime;

	// Token: 0x04000576 RID: 1398
	[SerializeField]
	private float shadeRotationSpeed = 1f;

	// Token: 0x04000577 RID: 1399
	[SerializeField]
	private AnimationCurve shadeHeightFunction;

	// Token: 0x04000578 RID: 1400
	[SerializeField]
	private AnimationCurve shadeScaleFunction;

	// Token: 0x04000579 RID: 1401
	[SerializeField]
	private AnimationCurve shadeYScaleMultFunction;

	// Token: 0x0400057A RID: 1402
	[SerializeField]
	private AnimationCurve soundVolumeFunction;

	// Token: 0x0400057B RID: 1403
	[SerializeField]
	private AudioClip[] audioClips;

	// Token: 0x0400057C RID: 1404
	private AudioSource audioSource;

	// Token: 0x0400057D RID: 1405
	private float startTime;

	// Token: 0x0400057E RID: 1406
	private float startAngle;
}
