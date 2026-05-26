using System;
using UnityEngine;

// Token: 0x020002F5 RID: 757
public class SpinRotation : MonoBehaviour, ITickSystemTick
{
	// Token: 0x170001E6 RID: 486
	// (get) Token: 0x0600134B RID: 4939 RVA: 0x00065EB4 File Offset: 0x000640B4
	// (set) Token: 0x0600134C RID: 4940 RVA: 0x00065EBC File Offset: 0x000640BC
	public bool TickRunning { get; set; }

	// Token: 0x0600134D RID: 4941 RVA: 0x00065EC5 File Offset: 0x000640C5
	public void Tick()
	{
		base.transform.localRotation = Quaternion.Euler(this.rotationPerSecondEuler * (Time.time - this.baseTime)) * this.baseRotation;
	}

	// Token: 0x0600134E RID: 4942 RVA: 0x00065EF9 File Offset: 0x000640F9
	private void Awake()
	{
		this.baseRotation = base.transform.localRotation;
	}

	// Token: 0x0600134F RID: 4943 RVA: 0x00065F0C File Offset: 0x0006410C
	private void OnEnable()
	{
		TickSystem<object>.AddTickCallback(this);
		this.baseTime = Time.time;
	}

	// Token: 0x06001350 RID: 4944 RVA: 0x00019E47 File Offset: 0x00018047
	private void OnDisable()
	{
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x0400179C RID: 6044
	[SerializeField]
	private Vector3 rotationPerSecondEuler;

	// Token: 0x0400179D RID: 6045
	private Quaternion baseRotation;

	// Token: 0x0400179E RID: 6046
	private float baseTime;
}
