using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000142 RID: 322
public class SIHandScannerTrigger : MonoBehaviour, IClickable
{
	// Token: 0x0600080B RID: 2059 RVA: 0x0002C180 File Offset: 0x0002A380
	private void Awake()
	{
		if (this.parentScanner == null)
		{
			this.parentScanner = base.GetComponentInParent<SIHandScanner>();
		}
	}

	// Token: 0x0600080C RID: 2060 RVA: 0x0002C19C File Offset: 0x0002A39C
	private void OnTriggerEnter(Collider other)
	{
		SIScannableHand component = other.GetComponent<SIScannableHand>();
		if (component == null)
		{
			return;
		}
		this.OnPlayerScanned(component.parentPlayer);
	}

	// Token: 0x0600080D RID: 2061 RVA: 0x0002C1C6 File Offset: 0x0002A3C6
	private void OnPlayerScanned(SIPlayer player)
	{
		this.parentScanner.HandScanned(player);
		this.onHandScanned.Invoke();
	}

	// Token: 0x0600080E RID: 2062 RVA: 0x0002C1DF File Offset: 0x0002A3DF
	public void Click(bool leftHand = false)
	{
		this.OnPlayerScanned(VRRig.LocalRig.GetComponent<SIPlayer>());
	}

	// Token: 0x04000A2C RID: 2604
	public SIHandScanner parentScanner;

	// Token: 0x04000A2D RID: 2605
	public UnityEvent onHandScanned;
}
