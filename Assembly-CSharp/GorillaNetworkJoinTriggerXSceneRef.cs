using System;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000C80 RID: 3200
public class GorillaNetworkJoinTriggerXSceneRef : MonoBehaviour
{
	// Token: 0x06004F77 RID: 20343 RVA: 0x001A53CE File Offset: 0x001A35CE
	protected void Awake()
	{
		if (this.m_joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this._joinTrigger) && this._joinTrigger != null)
		{
			return;
		}
		this.m_joinTriggerRef.AddCallbackOnLoad(new Action(this._OnTargetSceneLoaded));
	}

	// Token: 0x06004F78 RID: 20344 RVA: 0x001A5409 File Offset: 0x001A3609
	protected void OnDestroy()
	{
		this.m_joinTriggerRef.RemoveCallbackOnLoad(new Action(this._OnTargetSceneLoaded));
	}

	// Token: 0x06004F79 RID: 20345 RVA: 0x001A5422 File Offset: 0x001A3622
	private void _OnTargetSceneLoaded()
	{
		this.m_joinTriggerRef.TryResolve<GorillaNetworkJoinTrigger>(out this._joinTrigger);
	}

	// Token: 0x06004F7A RID: 20346 RVA: 0x001A5436 File Offset: 0x001A3636
	public void SubsPublicJoin()
	{
		if (this._joinTrigger != null)
		{
			this._joinTrigger.SubsPublicJoin();
		}
	}

	// Token: 0x04006133 RID: 24883
	[FormerlySerializedAs("joinTriggerRef")]
	[SerializeField]
	private XSceneRef m_joinTriggerRef;

	// Token: 0x04006134 RID: 24884
	private GorillaNetworkJoinTrigger _joinTrigger;
}
