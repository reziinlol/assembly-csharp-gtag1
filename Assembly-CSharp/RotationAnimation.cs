using System;
using UnityEngine;

// Token: 0x020004E0 RID: 1248
public class RotationAnimation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x1700032F RID: 815
	// (get) Token: 0x06001E56 RID: 7766 RVA: 0x000A25EC File Offset: 0x000A07EC
	// (set) Token: 0x06001E57 RID: 7767 RVA: 0x000A25F4 File Offset: 0x000A07F4
	public bool TickRunning { get; set; }

	// Token: 0x06001E58 RID: 7768 RVA: 0x000A2600 File Offset: 0x000A0800
	public void Tick()
	{
		Vector3 vector = Vector3.zero;
		vector.x = this.amplitude.x * this.x.Evaluate((Time.time - this.baseTime) * this.period.x % 1f);
		vector.y = this.amplitude.y * this.y.Evaluate((Time.time - this.baseTime) * this.period.y % 1f);
		vector.z = this.amplitude.z * this.z.Evaluate((Time.time - this.baseTime) * this.period.z % 1f);
		if (this.releaseSet)
		{
			float num = this.release.Evaluate(Time.time - this.releaseTime);
			vector *= num;
			if (num < Mathf.Epsilon)
			{
				base.enabled = false;
			}
		}
		base.transform.localRotation = Quaternion.Euler(vector) * this.baseRotation;
	}

	// Token: 0x06001E59 RID: 7769 RVA: 0x000A271A File Offset: 0x000A091A
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x06001E5A RID: 7770 RVA: 0x000A272D File Offset: 0x000A092D
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.releaseSet = false;
		this.baseTime = Time.time;
	}

	// Token: 0x06001E5B RID: 7771 RVA: 0x000A2747 File Offset: 0x000A0947
	public void ReleaseToDisable()
	{
		this.releaseSet = true;
		this.releaseTime = Time.time;
	}

	// Token: 0x06001E5C RID: 7772 RVA: 0x000A275B File Offset: 0x000A095B
	public void CancelRelease()
	{
		this.releaseSet = false;
	}

	// Token: 0x06001E5D RID: 7773 RVA: 0x000A2764 File Offset: 0x000A0964
	private void OnDisable()
	{
		base.transform.localRotation = this.baseRotation;
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x04002880 RID: 10368
	[SerializeField]
	private AnimationCurve x;

	// Token: 0x04002881 RID: 10369
	[SerializeField]
	private AnimationCurve y;

	// Token: 0x04002882 RID: 10370
	[SerializeField]
	private AnimationCurve z;

	// Token: 0x04002883 RID: 10371
	[SerializeField]
	private AnimationCurve attack;

	// Token: 0x04002884 RID: 10372
	[SerializeField]
	private AnimationCurve release;

	// Token: 0x04002885 RID: 10373
	[SerializeField]
	private Vector3 amplitude = Vector3.one;

	// Token: 0x04002886 RID: 10374
	[SerializeField]
	private Vector3 period = Vector3.one;

	// Token: 0x04002887 RID: 10375
	private Quaternion baseRotation;

	// Token: 0x04002888 RID: 10376
	private float baseTime;

	// Token: 0x04002889 RID: 10377
	private float releaseTime;

	// Token: 0x0400288A RID: 10378
	private bool releaseSet;
}
