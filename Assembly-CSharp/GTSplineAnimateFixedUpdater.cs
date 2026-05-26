using System;
using Fusion;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Splines;

// Token: 0x0200034B RID: 843
[NetworkBehaviourWeaved(1)]
public class GTSplineAnimateFixedUpdater : NetworkComponent
{
	// Token: 0x060014BB RID: 5307 RVA: 0x0006EB27 File Offset: 0x0006CD27
	protected override void Awake()
	{
		base.Awake();
		this.splineAnimateRef.AddCallbackOnLoad(new Action(this.InitSplineAnimate));
		this.splineAnimateRef.AddCallbackOnUnload(new Action(this.ClearSplineAnimate));
	}

	// Token: 0x060014BC RID: 5308 RVA: 0x0006EB5D File Offset: 0x0006CD5D
	private void InitSplineAnimate()
	{
		this.isSplineLoaded = this.splineAnimateRef.TryResolve<SplineAnimate>(out this.splineAnimate);
		if (this.isSplineLoaded && this.splineAnimate != null)
		{
			this.splineAnimate.enabled = false;
		}
	}

	// Token: 0x060014BD RID: 5309 RVA: 0x0006EB98 File Offset: 0x0006CD98
	private void ClearSplineAnimate()
	{
		this.splineAnimate = null;
		this.isSplineLoaded = false;
	}

	// Token: 0x060014BE RID: 5310 RVA: 0x0006EBA8 File Offset: 0x0006CDA8
	private void FixedUpdate()
	{
		if (!base.IsMine && this.progressLerpStartTime + 1f > Time.time)
		{
			if (this.isSplineLoaded)
			{
				this.progress = Mathf.Lerp(this.progressLerpStart, this.progressLerpEnd, (Time.time - this.progressLerpStartTime) / 1f) % this.Duration;
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
				return;
			}
		}
		else
		{
			this.progress = (this.progress + Time.fixedDeltaTime) % this.Duration;
			if (this.isSplineLoaded)
			{
				this.splineAnimate.NormalizedTime = this.progress / this.Duration;
			}
		}
	}

	// Token: 0x1700020F RID: 527
	// (get) Token: 0x060014BF RID: 5311 RVA: 0x0006EC5D File Offset: 0x0006CE5D
	// (set) Token: 0x060014C0 RID: 5312 RVA: 0x0006EC83 File Offset: 0x0006CE83
	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe float Netdata
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(float*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GTSplineAnimateFixedUpdater.Netdata. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(float*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060014C1 RID: 5313 RVA: 0x0006ECAA File Offset: 0x0006CEAA
	public override void WriteDataFusion()
	{
		this.Netdata = this.progress + 1f;
	}

	// Token: 0x060014C2 RID: 5314 RVA: 0x0006ECBE File Offset: 0x0006CEBE
	public override void ReadDataFusion()
	{
		this.SharedReadData(this.Netdata);
	}

	// Token: 0x060014C3 RID: 5315 RVA: 0x0006ECCC File Offset: 0x0006CECC
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		stream.SendNext(this.progress + 1f);
	}

	// Token: 0x060014C4 RID: 5316 RVA: 0x0006ECF4 File Offset: 0x0006CEF4
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		if (!info.Sender.IsMasterClient)
		{
			return;
		}
		float incomingValue = (float)stream.ReceiveNext();
		this.SharedReadData(incomingValue);
	}

	// Token: 0x060014C5 RID: 5317 RVA: 0x0006ED24 File Offset: 0x0006CF24
	private void SharedReadData(float incomingValue)
	{
		if (float.IsNaN(incomingValue) || incomingValue > this.Duration + 1f || incomingValue < 0f)
		{
			return;
		}
		this.progressLerpEnd = incomingValue;
		if (this.progressLerpEnd < this.progress)
		{
			if (this.progress < this.Duration)
			{
				this.progressLerpEnd += this.Duration;
			}
			else
			{
				this.progress -= this.Duration;
			}
		}
		this.progressLerpStart = this.progress;
		this.progressLerpStartTime = Time.time;
	}

	// Token: 0x060014C7 RID: 5319 RVA: 0x0006EDB3 File Offset: 0x0006CFB3
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.Netdata = this._Netdata;
	}

	// Token: 0x060014C8 RID: 5320 RVA: 0x0006EDCB File Offset: 0x0006CFCB
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._Netdata = this.Netdata;
	}

	// Token: 0x04001973 RID: 6515
	[SerializeField]
	private XSceneRef splineAnimateRef;

	// Token: 0x04001974 RID: 6516
	[SerializeField]
	private float Duration;

	// Token: 0x04001975 RID: 6517
	private const float progressLerpDuration = 1f;

	// Token: 0x04001976 RID: 6518
	private SplineAnimate splineAnimate;

	// Token: 0x04001977 RID: 6519
	private bool isSplineLoaded;

	// Token: 0x04001978 RID: 6520
	private float progress;

	// Token: 0x04001979 RID: 6521
	private float progressLerpStart;

	// Token: 0x0400197A RID: 6522
	private float progressLerpEnd;

	// Token: 0x0400197B RID: 6523
	private float progressLerpStartTime;

	// Token: 0x0400197C RID: 6524
	[WeaverGenerated]
	[SerializeField]
	[DefaultForProperty("Netdata", 0, 1)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private float _Netdata;
}
