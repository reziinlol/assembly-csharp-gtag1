using System;
using UnityEngine;

// Token: 0x020003CE RID: 974
public class GreyZoneActivator : MonoBehaviour
{
	// Token: 0x06001736 RID: 5942 RVA: 0x00085FFC File Offset: 0x000841FC
	private void OnEnable()
	{
		if (this.activateOnEnable)
		{
			this.Activate();
		}
	}

	// Token: 0x06001737 RID: 5943 RVA: 0x0008600C File Offset: 0x0008420C
	private void OnDisable()
	{
		if (this.deactivateOnDisable)
		{
			this.Deactivate();
		}
	}

	// Token: 0x06001738 RID: 5944 RVA: 0x0008601C File Offset: 0x0008421C
	public void Activate()
	{
		GreyZoneManager.Instance.LocalSimpleActivation(true, this.gMultiplier);
	}

	// Token: 0x06001739 RID: 5945 RVA: 0x00086031 File Offset: 0x00084231
	public void ActivateWithG(float g)
	{
		GreyZoneManager.Instance.LocalSimpleActivation(true, g);
	}

	// Token: 0x0600173A RID: 5946 RVA: 0x00086041 File Offset: 0x00084241
	public void Deactivate()
	{
		GreyZoneManager.Instance.LocalSimpleActivation(false, 1f);
	}

	// Token: 0x0400226F RID: 8815
	[SerializeField]
	private bool activateOnEnable;

	// Token: 0x04002270 RID: 8816
	[SerializeField]
	private bool deactivateOnDisable;

	// Token: 0x04002271 RID: 8817
	[Range(-5f, 5f)]
	[SerializeField]
	private float gMultiplier = 1f;
}
