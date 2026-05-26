using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020002FC RID: 764
public class TriggerOnSpeed : MonoBehaviour, ITickSystemTick
{
	// Token: 0x06001379 RID: 4985 RVA: 0x000412AB File Offset: 0x0003F4AB
	private void OnEnable()
	{
		TickSystem<object>.AddCallbackTarget(this);
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x000412B3 File Offset: 0x0003F4B3
	private void OnDisable()
	{
		TickSystem<object>.RemoveCallbackTarget(this);
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x00066C30 File Offset: 0x00064E30
	public void Tick()
	{
		bool flag = this.velocityEstimator.linearVelocity.IsLongerThan(this.speedThreshold);
		if (flag != this.wasFaster)
		{
			if (flag)
			{
				this.onFaster.Invoke();
			}
			else
			{
				this.onSlower.Invoke();
			}
			this.wasFaster = flag;
		}
	}

	// Token: 0x170001EC RID: 492
	// (get) Token: 0x0600137C RID: 4988 RVA: 0x00066C7F File Offset: 0x00064E7F
	// (set) Token: 0x0600137D RID: 4989 RVA: 0x00066C87 File Offset: 0x00064E87
	public bool TickRunning { get; set; }

	// Token: 0x040017E5 RID: 6117
	[SerializeField]
	private float speedThreshold;

	// Token: 0x040017E6 RID: 6118
	[SerializeField]
	private UnityEvent onFaster;

	// Token: 0x040017E7 RID: 6119
	[SerializeField]
	private UnityEvent onSlower;

	// Token: 0x040017E8 RID: 6120
	[SerializeField]
	private GorillaVelocityEstimator velocityEstimator;

	// Token: 0x040017E9 RID: 6121
	private bool wasFaster;
}
