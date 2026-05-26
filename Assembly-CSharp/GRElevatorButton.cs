using System;
using UnityEngine;

// Token: 0x02000763 RID: 1891
public class GRElevatorButton : MonoBehaviour
{
	// Token: 0x06002FEC RID: 12268 RVA: 0x00104098 File Offset: 0x00102298
	private void Awake()
	{
		if (this.disableDelayed == null)
		{
			this.disableDelayed = this.buttonLit.GetComponent<DisableGameObjectDelayed>();
		}
		if (this.tempLight)
		{
			this.disableDelayed.enabled = false;
			return;
		}
		this.disableDelayed.delayTime = this.litUpTime;
	}

	// Token: 0x06002FED RID: 12269 RVA: 0x001040EA File Offset: 0x001022EA
	public void Pressed()
	{
		this.buttonLit.SetActive(true);
	}

	// Token: 0x06002FEE RID: 12270 RVA: 0x001040F8 File Offset: 0x001022F8
	public void Depressed()
	{
		this.buttonLit.SetActive(false);
	}

	// Token: 0x04003D7A RID: 15738
	public GRElevator.ButtonType buttonType;

	// Token: 0x04003D7B RID: 15739
	public GameObject buttonLit;

	// Token: 0x04003D7C RID: 15740
	public float litUpTime;

	// Token: 0x04003D7D RID: 15741
	public DisableGameObjectDelayed disableDelayed;

	// Token: 0x04003D7E RID: 15742
	public bool tempLight;
}
