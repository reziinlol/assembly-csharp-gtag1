using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// Token: 0x020008D5 RID: 2261
public class GTSignalListener : MonoBehaviour
{
	// Token: 0x17000544 RID: 1348
	// (get) Token: 0x06003B1B RID: 15131 RVA: 0x00143FBF File Offset: 0x001421BF
	// (set) Token: 0x06003B1C RID: 15132 RVA: 0x00143FC7 File Offset: 0x001421C7
	public int rigActorID { get; private set; } = -1;

	// Token: 0x06003B1D RID: 15133 RVA: 0x00143FD0 File Offset: 0x001421D0
	private void Awake()
	{
		this.OnListenerAwake();
	}

	// Token: 0x06003B1E RID: 15134 RVA: 0x00143FD8 File Offset: 0x001421D8
	private void OnEnable()
	{
		this.RefreshActorID();
		this.OnListenerEnable();
		GTSignalRelay.Register(this);
	}

	// Token: 0x06003B1F RID: 15135 RVA: 0x00143FEC File Offset: 0x001421EC
	private void OnDisable()
	{
		GTSignalRelay.Unregister(this);
		this.OnListenerDisable();
	}

	// Token: 0x06003B20 RID: 15136 RVA: 0x00143FFA File Offset: 0x001421FA
	private void RefreshActorID()
	{
		this.rig = base.GetComponentInParent<VRRig>(true);
		int rigActorID;
		if (!(this.rig == null))
		{
			NetPlayer creator = this.rig.Creator;
			rigActorID = ((creator != null) ? creator.ActorNumber : -1);
		}
		else
		{
			rigActorID = -1;
		}
		this.rigActorID = rigActorID;
	}

	// Token: 0x06003B21 RID: 15137 RVA: 0x00144037 File Offset: 0x00142237
	public virtual bool IsReady()
	{
		return this._callLimits.CheckCallTime(Time.time);
	}

	// Token: 0x06003B22 RID: 15138 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnListenerAwake()
	{
	}

	// Token: 0x06003B23 RID: 15139 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnListenerEnable()
	{
	}

	// Token: 0x06003B24 RID: 15140 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnListenerDisable()
	{
	}

	// Token: 0x06003B25 RID: 15141 RVA: 0x000028C5 File Offset: 0x00000AC5
	public virtual void HandleSignalReceived(int sender, object[] args)
	{
	}

	// Token: 0x04004B75 RID: 19317
	[Space]
	public GTSignalID signal;

	// Token: 0x04004B76 RID: 19318
	[Space]
	public VRRig rig;

	// Token: 0x04004B78 RID: 19320
	[Space]
	public bool deafen;

	// Token: 0x04004B79 RID: 19321
	[FormerlySerializedAs("listenToRigOnly")]
	public bool listenToSelfOnly;

	// Token: 0x04004B7A RID: 19322
	public bool ignoreSelf;

	// Token: 0x04004B7B RID: 19323
	[Space]
	public bool callUnityEvent = true;

	// Token: 0x04004B7C RID: 19324
	[Space]
	[SerializeField]
	private CallLimiter _callLimits = new CallLimiter(10, 0.25f, 0.5f);

	// Token: 0x04004B7D RID: 19325
	[Space]
	public UnityEvent onSignalReceived;
}
