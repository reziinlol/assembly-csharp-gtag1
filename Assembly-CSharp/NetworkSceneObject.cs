using System;
using Fusion;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000457 RID: 1111
[RequireComponent(typeof(PhotonView))]
public class NetworkSceneObject : SimulationBehaviour
{
	// Token: 0x170002B4 RID: 692
	// (get) Token: 0x06001AA9 RID: 6825 RVA: 0x00093DC4 File Offset: 0x00091FC4
	public bool IsMine
	{
		get
		{
			return this.photonView.IsMine;
		}
	}

	// Token: 0x06001AAA RID: 6826 RVA: 0x00093DD1 File Offset: 0x00091FD1
	protected virtual void Start()
	{
		if (this.photonView == null)
		{
			this.photonView = base.GetComponent<PhotonView>();
		}
	}

	// Token: 0x06001AAB RID: 6827 RVA: 0x00093DED File Offset: 0x00091FED
	protected virtual void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
	}

	// Token: 0x06001AAC RID: 6828 RVA: 0x00093DF5 File Offset: 0x00091FF5
	protected virtual void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
	}

	// Token: 0x06001AAD RID: 6829 RVA: 0x00093E00 File Offset: 0x00092000
	private void RegisterOnRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.AddGlobal(this);
		}
	}

	// Token: 0x06001AAE RID: 6830 RVA: 0x00093E38 File Offset: 0x00092038
	private void RemoveFromRunner()
	{
		NetworkRunner runner = (NetworkSystem.Instance as NetworkSystemFusion).runner;
		if (runner != null && runner.IsRunning)
		{
			runner.RemoveGlobal(this);
		}
	}

	// Token: 0x0400253D RID: 9533
	public PhotonView photonView;
}
