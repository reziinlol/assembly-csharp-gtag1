using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000355 RID: 853
public class LocalActivateOnDateRange : MonoBehaviour
{
	// Token: 0x060014E0 RID: 5344 RVA: 0x0006F2E8 File Offset: 0x0006D4E8
	private void Awake()
	{
		GameObject[] array = this.gameObjectsToActivate;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
	}

	// Token: 0x060014E1 RID: 5345 RVA: 0x0006F313 File Offset: 0x0006D513
	private void OnEnable()
	{
		this.InitActiveTimes();
	}

	// Token: 0x060014E2 RID: 5346 RVA: 0x0006F31C File Offset: 0x0006D51C
	private void InitActiveTimes()
	{
		this.activationTime = new DateTime(this.activationYear, this.activationMonth, this.activationDay, this.activationHour, this.activationMinute, this.activationSecond, DateTimeKind.Utc);
		this.deactivationTime = new DateTime(this.deactivationYear, this.deactivationMonth, this.deactivationDay, this.deactivationHour, this.deactivationMinute, this.deactivationSecond, DateTimeKind.Utc);
	}

	// Token: 0x060014E3 RID: 5347 RVA: 0x0006F38C File Offset: 0x0006D58C
	private void LateUpdate()
	{
		DateTime utcNow = DateTime.UtcNow;
		this.dbgTimeUntilActivation = (this.activationTime - utcNow).TotalSeconds;
		this.dbgTimeUntilDeactivation = (this.deactivationTime - utcNow).TotalSeconds;
		bool flag = utcNow >= this.activationTime && utcNow <= this.deactivationTime;
		if (flag != this.isActive)
		{
			GameObject[] array = this.gameObjectsToActivate;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(flag);
			}
			this.isActive = flag;
		}
	}

	// Token: 0x040019B1 RID: 6577
	[Header("Activation Date and Time (UTC)")]
	public int activationYear = 2023;

	// Token: 0x040019B2 RID: 6578
	public int activationMonth = 4;

	// Token: 0x040019B3 RID: 6579
	public int activationDay = 1;

	// Token: 0x040019B4 RID: 6580
	public int activationHour = 7;

	// Token: 0x040019B5 RID: 6581
	public int activationMinute;

	// Token: 0x040019B6 RID: 6582
	public int activationSecond;

	// Token: 0x040019B7 RID: 6583
	[Header("Deactivation Date and Time (UTC)")]
	public int deactivationYear = 2023;

	// Token: 0x040019B8 RID: 6584
	public int deactivationMonth = 4;

	// Token: 0x040019B9 RID: 6585
	public int deactivationDay = 2;

	// Token: 0x040019BA RID: 6586
	public int deactivationHour = 7;

	// Token: 0x040019BB RID: 6587
	public int deactivationMinute;

	// Token: 0x040019BC RID: 6588
	public int deactivationSecond;

	// Token: 0x040019BD RID: 6589
	public GameObject[] gameObjectsToActivate;

	// Token: 0x040019BE RID: 6590
	private bool isActive;

	// Token: 0x040019BF RID: 6591
	private DateTime activationTime;

	// Token: 0x040019C0 RID: 6592
	private DateTime deactivationTime;

	// Token: 0x040019C1 RID: 6593
	[DebugReadout]
	public double dbgTimeUntilActivation;

	// Token: 0x040019C2 RID: 6594
	[DebugReadout]
	public double dbgTimeUntilDeactivation;
}
