using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A2 RID: 930
public class ThermalSourceVolume : MonoBehaviour
{
	// Token: 0x06001686 RID: 5766 RVA: 0x0008257E File Offset: 0x0008077E
	protected void OnEnable()
	{
		ThermalManager.Register(this);
	}

	// Token: 0x06001687 RID: 5767 RVA: 0x00082586 File Offset: 0x00080786
	protected void OnDisable()
	{
		ThermalManager.Unregister(this);
	}

	// Token: 0x04002097 RID: 8343
	[Tooltip("Temperature in celsius. Default is 20 which is room temperature.")]
	public float celsius = 20f;

	// Token: 0x04002098 RID: 8344
	public float innerRadius = 0.1f;

	// Token: 0x04002099 RID: 8345
	public float outerRadius = 1f;

	// Token: 0x0400209A RID: 8346
	[Tooltip("Exclude these thermal receivers from being impacted by this source")]
	public List<ThermalReceiver> exclusionReceivers = new List<ThermalReceiver>();
}
