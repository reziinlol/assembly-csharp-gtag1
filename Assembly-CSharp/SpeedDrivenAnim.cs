using System;
using UnityEngine;

// Token: 0x020002F4 RID: 756
public class SpeedDrivenAnim : MonoBehaviour
{
	// Token: 0x06001348 RID: 4936 RVA: 0x00065DF6 File Offset: 0x00063FF6
	private void Start()
	{
		this.velocityEstimator = base.GetComponent<GorillaVelocityEstimator>();
		this.animator = base.GetComponent<Animator>();
		this.keyHash = Animator.StringToHash(this.animKey);
	}

	// Token: 0x06001349 RID: 4937 RVA: 0x00065E24 File Offset: 0x00064024
	private void Update()
	{
		float target = Mathf.InverseLerp(this.speed0, this.speed1, this.velocityEstimator.linearVelocity.magnitude);
		this.currentBlend = Mathf.MoveTowards(this.currentBlend, target, this.maxChangePerSecond * Time.deltaTime);
		this.animator.SetFloat(this.keyHash, this.currentBlend);
	}

	// Token: 0x04001794 RID: 6036
	[SerializeField]
	private float speed0;

	// Token: 0x04001795 RID: 6037
	[SerializeField]
	private float speed1 = 1f;

	// Token: 0x04001796 RID: 6038
	[SerializeField]
	private float maxChangePerSecond = 1f;

	// Token: 0x04001797 RID: 6039
	[SerializeField]
	private string animKey = "speed";

	// Token: 0x04001798 RID: 6040
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x04001799 RID: 6041
	private Animator animator;

	// Token: 0x0400179A RID: 6042
	private int keyHash;

	// Token: 0x0400179B RID: 6043
	private float currentBlend;
}
