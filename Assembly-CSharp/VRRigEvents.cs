using System;
using GorillaTag;
using UnityEngine;

// Token: 0x020004F5 RID: 1269
[RequireComponent(typeof(RigContainer))]
public class VRRigEvents : MonoBehaviour, IPreDisable
{
	// Token: 0x06001FCC RID: 8140 RVA: 0x000AB35A File Offset: 0x000A955A
	public void PreDisable()
	{
		DelegateListProcessor<RigContainer> delegateListProcessor = this.disableEvent;
		if (delegateListProcessor == null)
		{
			return;
		}
		delegateListProcessor.InvokeSafe(this.rigRef);
	}

	// Token: 0x06001FCD RID: 8141 RVA: 0x000AB372 File Offset: 0x000A9572
	public void SendPostEnableEvent()
	{
		DelegateListProcessor<RigContainer> delegateListProcessor = this.enableEvent;
		if (delegateListProcessor == null)
		{
			return;
		}
		delegateListProcessor.InvokeSafe(this.rigRef);
	}

	// Token: 0x04002A8A RID: 10890
	[SerializeField]
	private RigContainer rigRef;

	// Token: 0x04002A8B RID: 10891
	public DelegateListProcessor<RigContainer> disableEvent = new DelegateListProcessor<RigContainer>(5);

	// Token: 0x04002A8C RID: 10892
	public DelegateListProcessor<RigContainer> enableEvent = new DelegateListProcessor<RigContainer>(5);
}
