using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class TempMask : MonoBehaviour
{
	// Token: 0x06000BEA RID: 3050 RVA: 0x00041060 File Offset: 0x0003F260
	private void Awake()
	{
		this.dayOn = new DateTime(this.year, this.month, this.day);
		this.myRig = base.GetComponentInParent<VRRig>();
		if (this.myRig != null && this.myRig.netView.IsMine && !this.myRig.isOfflineVRRig)
		{
			Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x06000BEB RID: 3051 RVA: 0x000410CE File Offset: 0x0003F2CE
	private void OnEnable()
	{
		base.StartCoroutine(this.MaskOnDuringDate());
	}

	// Token: 0x06000BEC RID: 3052 RVA: 0x00005511 File Offset: 0x00003711
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06000BED RID: 3053 RVA: 0x000410DD File Offset: 0x0003F2DD
	private IEnumerator MaskOnDuringDate()
	{
		for (;;)
		{
			if (GorillaComputer.instance != null && GorillaComputer.instance.startupMillis != 0L)
			{
				this.myDate = new DateTime(GorillaComputer.instance.startupMillis * 10000L + (long)(Time.realtimeSinceStartup * 1000f * 10000f)).Subtract(TimeSpan.FromHours(7.0));
				if (this.myDate.DayOfYear == this.dayOn.DayOfYear)
				{
					if (!this.myRenderer.enabled)
					{
						this.myRenderer.enabled = true;
					}
				}
				else if (this.myRenderer.enabled)
				{
					this.myRenderer.enabled = false;
				}
			}
			yield return new WaitForSeconds(1f);
		}
		yield break;
	}

	// Token: 0x04000E8B RID: 3723
	public int year;

	// Token: 0x04000E8C RID: 3724
	public int month;

	// Token: 0x04000E8D RID: 3725
	public int day;

	// Token: 0x04000E8E RID: 3726
	public DateTime dayOn;

	// Token: 0x04000E8F RID: 3727
	public MeshRenderer myRenderer;

	// Token: 0x04000E90 RID: 3728
	private DateTime myDate;

	// Token: 0x04000E91 RID: 3729
	private VRRig myRig;
}
