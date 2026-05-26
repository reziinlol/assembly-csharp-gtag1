using System;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000D5F RID: 3423
internal class NetworkVector3
{
	// Token: 0x170007EF RID: 2031
	// (get) Token: 0x06005429 RID: 21545 RVA: 0x001B8152 File Offset: 0x001B6352
	public Vector3 CurrentSyncTarget
	{
		get
		{
			return this._currentSyncTarget;
		}
	}

	// Token: 0x0600542A RID: 21546 RVA: 0x001B815C File Offset: 0x001B635C
	public void SetNewSyncTarget(Vector3 newTarget)
	{
		Vector3 currentSyncTarget = this.CurrentSyncTarget;
		ref currentSyncTarget.SetValueSafe(newTarget);
		this.distanceTraveled = currentSyncTarget - this._currentSyncTarget;
		this._currentSyncTarget = currentSyncTarget;
		this.lastSetNetTime = PhotonNetwork.Time;
	}

	// Token: 0x0600542B RID: 21547 RVA: 0x001B81A0 File Offset: 0x001B63A0
	public Vector3 GetPredictedFuture()
	{
		float d = (float)(PhotonNetwork.Time - this.lastSetNetTime) * (float)PhotonNetwork.SerializationRate;
		Vector3 b = this.distanceTraveled * d;
		return this._currentSyncTarget + b;
	}

	// Token: 0x0600542C RID: 21548 RVA: 0x001B81DB File Offset: 0x001B63DB
	public void Reset()
	{
		this._currentSyncTarget = Vector3.zero;
		this.distanceTraveled = Vector3.zero;
		this.lastSetNetTime = 0.0;
	}

	// Token: 0x04006504 RID: 25860
	private double lastSetNetTime;

	// Token: 0x04006505 RID: 25861
	private Vector3 _currentSyncTarget = Vector3.zero;

	// Token: 0x04006506 RID: 25862
	private Vector3 distanceTraveled = Vector3.zero;
}
