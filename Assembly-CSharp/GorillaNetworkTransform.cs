using System;
using System.Runtime.InteropServices;
using Fusion;
using GorillaExtensions;
using Photon.Pun;
using UnityEngine;

// Token: 0x02000830 RID: 2096
[NetworkBehaviourWeaved(15)]
internal class GorillaNetworkTransform : NetworkComponent, ITickSystemTick
{
	// Token: 0x170004B1 RID: 1201
	// (get) Token: 0x060035D8 RID: 13784 RVA: 0x0012A3B0 File Offset: 0x001285B0
	public bool RespectOwnership
	{
		get
		{
			return this.respectOwnership;
		}
	}

	// Token: 0x170004B2 RID: 1202
	// (get) Token: 0x060035D9 RID: 13785 RVA: 0x0012A3B8 File Offset: 0x001285B8
	// (set) Token: 0x060035DA RID: 13786 RVA: 0x0012A3C0 File Offset: 0x001285C0
	public bool TickRunning { get; set; }

	// Token: 0x170004B3 RID: 1203
	// (get) Token: 0x060035DB RID: 13787 RVA: 0x0012A3C9 File Offset: 0x001285C9
	// (set) Token: 0x060035DC RID: 13788 RVA: 0x0012A3F3 File Offset: 0x001285F3
	[Networked]
	[NetworkedWeaved(0, 15)]
	private unsafe GorillaNetworkTransform.NetTransformData data
	{
		get
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			return *(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0);
		}
		set
		{
			if (this.Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing GorillaNetworkTransform.data. Networked properties can only be accessed when Spawned() has been called.");
			}
			*(GorillaNetworkTransform.NetTransformData*)(this.Ptr + 0) = value;
		}
	}

	// Token: 0x060035DD RID: 13789 RVA: 0x0012A420 File Offset: 0x00128620
	public new void Awake()
	{
		this.m_StoredPosition = base.transform.localPosition;
		this.m_NetworkPosition = Vector3.zero;
		this.m_NetworkScale = Vector3.zero;
		this.m_NetworkRotation = Quaternion.identity;
		this.maxDistanceSquare = this.maxDistance * this.maxDistance;
	}

	// Token: 0x060035DE RID: 13790 RVA: 0x0012A474 File Offset: 0x00128674
	private new void OnEnable()
	{
		NetworkBehaviourUtils.InternalOnEnable(this);
		this.m_firstTake = true;
		if (this.clampToSpawn)
		{
			this.clampOriginPoint = (this.m_UseLocal ? base.transform.localPosition : base.transform.position);
		}
		TickSystem<object>.AddTickCallback(this);
	}

	// Token: 0x060035DF RID: 13791 RVA: 0x0012A4C2 File Offset: 0x001286C2
	private new void OnDisable()
	{
		NetworkBehaviourUtils.InternalOnDisable(this);
		TickSystem<object>.RemoveTickCallback(this);
	}

	// Token: 0x060035E0 RID: 13792 RVA: 0x0012A4D0 File Offset: 0x001286D0
	public void Tick()
	{
		if (!base.IsLocallyOwned)
		{
			if (this.m_UseLocal)
			{
				base.transform.SetLocalPositionAndRotation(Vector3.MoveTowards(base.transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
				return;
			}
			base.transform.SetPositionAndRotation(Vector3.MoveTowards(base.transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)NetworkSystem.Instance.TickRate), Quaternion.RotateTowards(base.transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)NetworkSystem.Instance.TickRate));
		}
	}

	// Token: 0x060035E1 RID: 13793 RVA: 0x0012A5C0 File Offset: 0x001287C0
	public override void WriteDataFusion()
	{
		GorillaNetworkTransform.NetTransformData data = this.SharedWrite();
		double sentTime = NetworkSystem.Instance.SimTick / 1000.0;
		data.SentTime = sentTime;
		this.data = data;
	}

	// Token: 0x060035E2 RID: 13794 RVA: 0x0012A5FA File Offset: 0x001287FA
	public override void ReadDataFusion()
	{
		this.SharedRead(this.data);
	}

	// Token: 0x060035E3 RID: 13795 RVA: 0x0012A608 File Offset: 0x00128808
	protected override void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData netTransformData = this.SharedWrite();
		if (this.m_SynchronizePosition)
		{
			stream.SendNext(netTransformData.position);
			stream.SendNext(netTransformData.velocity);
		}
		if (this.m_SynchronizeRotation)
		{
			stream.SendNext(netTransformData.rotation);
		}
		if (this.m_SynchronizeScale)
		{
			stream.SendNext(netTransformData.scale);
		}
	}

	// Token: 0x060035E4 RID: 13796 RVA: 0x0012A69C File Offset: 0x0012889C
	protected override void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		NetPlayer player = NetworkSystem.Instance.GetPlayer(info.Sender);
		if (this.respectOwnership && player != base.Owner)
		{
			return;
		}
		GorillaNetworkTransform.NetTransformData data = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			data.position = (Vector3)stream.ReceiveNext();
			data.velocity = (Vector3)stream.ReceiveNext();
		}
		if (this.m_SynchronizeRotation)
		{
			data.rotation = (Quaternion)stream.ReceiveNext();
		}
		if (this.m_SynchronizeScale)
		{
			data.scale = (Vector3)stream.ReceiveNext();
		}
		data.SentTime = (double)((float)info.SentServerTime);
		this.SharedRead(data);
	}

	// Token: 0x060035E5 RID: 13797 RVA: 0x0012A74C File Offset: 0x0012894C
	private void SharedRead(GorillaNetworkTransform.NetTransformData data)
	{
		if (this.m_SynchronizePosition)
		{
			ref this.m_NetworkPosition.SetValueSafe(data.position);
			ref this.m_Velocity.SetValueSafe(data.velocity);
			if (this.clampDistanceFromSpawn && Vector3.SqrMagnitude(this.clampOriginPoint - this.m_NetworkPosition) > this.maxDistanceSquare)
			{
				this.m_NetworkPosition = this.clampOriginPoint + this.m_Velocity.normalized * this.maxDistance;
				this.m_Velocity = Vector3.zero;
			}
			if (this.m_firstTake)
			{
				if (this.m_UseLocal)
				{
					base.transform.localPosition = this.m_NetworkPosition;
				}
				else
				{
					base.transform.position = this.m_NetworkPosition;
				}
				this.m_Distance = 0f;
			}
			else
			{
				float d = Mathf.Abs((float)(NetworkSystem.Instance.SimTime - data.SentTime));
				this.m_NetworkPosition += this.m_Velocity * d;
				if (this.m_UseLocal)
				{
					this.m_Distance = Vector3.Distance(base.transform.localPosition, this.m_NetworkPosition);
				}
				else
				{
					this.m_Distance = Vector3.Distance(base.transform.position, this.m_NetworkPosition);
				}
			}
		}
		if (this.m_SynchronizeRotation)
		{
			ref this.m_NetworkRotation.SetValueSafe(data.rotation);
			if (this.m_firstTake)
			{
				this.m_Angle = 0f;
				if (this.m_UseLocal)
				{
					base.transform.localRotation = this.m_NetworkRotation;
				}
				else
				{
					base.transform.rotation = this.m_NetworkRotation;
				}
			}
			else if (this.m_UseLocal)
			{
				this.m_Angle = Quaternion.Angle(base.transform.localRotation, this.m_NetworkRotation);
			}
			else
			{
				this.m_Angle = Quaternion.Angle(base.transform.rotation, this.m_NetworkRotation);
			}
		}
		if (this.m_SynchronizeScale)
		{
			ref this.m_NetworkScale.SetValueSafe(data.scale);
			base.transform.localScale = this.m_NetworkScale;
		}
		if (this.m_firstTake)
		{
			this.m_firstTake = false;
		}
	}

	// Token: 0x060035E6 RID: 13798 RVA: 0x0012A974 File Offset: 0x00128B74
	private GorillaNetworkTransform.NetTransformData SharedWrite()
	{
		GorillaNetworkTransform.NetTransformData result = default(GorillaNetworkTransform.NetTransformData);
		if (this.m_SynchronizePosition)
		{
			if (this.m_UseLocal)
			{
				this.m_Velocity = base.transform.localPosition - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.localPosition;
				result.position = base.transform.localPosition;
				result.velocity = this.m_Velocity;
			}
			else
			{
				this.m_Velocity = base.transform.position - this.m_StoredPosition;
				this.m_StoredPosition = base.transform.position;
				result.position = base.transform.position;
				result.velocity = this.m_Velocity;
			}
		}
		if (this.m_SynchronizeRotation)
		{
			if (this.m_UseLocal)
			{
				result.rotation = base.transform.localRotation;
			}
			else
			{
				result.rotation = base.transform.rotation;
			}
		}
		if (this.m_SynchronizeScale)
		{
			result.scale = base.transform.localScale;
		}
		return result;
	}

	// Token: 0x060035E7 RID: 13799 RVA: 0x0012AA87 File Offset: 0x00128C87
	public void GTAddition_DoTeleport()
	{
		this.m_firstTake = true;
	}

	// Token: 0x060035E9 RID: 13801 RVA: 0x0012AABF File Offset: 0x00128CBF
	[WeaverGenerated]
	public override void CopyBackingFieldsToState(bool A_1)
	{
		base.CopyBackingFieldsToState(A_1);
		this.data = this._data;
	}

	// Token: 0x060035EA RID: 13802 RVA: 0x0012AAD7 File Offset: 0x00128CD7
	[WeaverGenerated]
	public override void CopyStateToBackingFields()
	{
		base.CopyStateToBackingFields();
		this._data = this.data;
	}

	// Token: 0x04004679 RID: 18041
	[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
	public bool m_UseLocal;

	// Token: 0x0400467A RID: 18042
	[SerializeField]
	private bool respectOwnership;

	// Token: 0x0400467B RID: 18043
	[SerializeField]
	private bool clampDistanceFromSpawn = true;

	// Token: 0x0400467C RID: 18044
	[SerializeField]
	private float maxDistance = 100f;

	// Token: 0x0400467D RID: 18045
	private float maxDistanceSquare;

	// Token: 0x0400467E RID: 18046
	[SerializeField]
	private bool clampToSpawn = true;

	// Token: 0x0400467F RID: 18047
	[Tooltip("Use this if clampToSpawn is false, to set the center point to check the synced position against")]
	[SerializeField]
	private Vector3 clampOriginPoint;

	// Token: 0x04004680 RID: 18048
	public bool m_SynchronizePosition = true;

	// Token: 0x04004681 RID: 18049
	public bool m_SynchronizeRotation = true;

	// Token: 0x04004682 RID: 18050
	public bool m_SynchronizeScale;

	// Token: 0x04004683 RID: 18051
	private float m_Distance;

	// Token: 0x04004684 RID: 18052
	private float m_Angle;

	// Token: 0x04004685 RID: 18053
	private Vector3 m_Velocity;

	// Token: 0x04004686 RID: 18054
	private Vector3 m_NetworkPosition;

	// Token: 0x04004687 RID: 18055
	private Vector3 m_StoredPosition;

	// Token: 0x04004688 RID: 18056
	private Vector3 m_NetworkScale;

	// Token: 0x04004689 RID: 18057
	private Quaternion m_NetworkRotation;

	// Token: 0x0400468A RID: 18058
	private bool m_firstTake;

	// Token: 0x0400468C RID: 18060
	[WeaverGenerated]
	[DefaultForProperty("data", 0, 15)]
	[DrawIf("IsEditorWritable", true, CompareOperator.Equal, DrawIfMode.ReadOnly)]
	private GorillaNetworkTransform.NetTransformData _data;

	// Token: 0x02000831 RID: 2097
	[NetworkStructWeaved(15)]
	[StructLayout(LayoutKind.Explicit, Size = 60)]
	private struct NetTransformData : INetworkStruct
	{
		// Token: 0x0400468D RID: 18061
		[FieldOffset(0)]
		public Vector3 position;

		// Token: 0x0400468E RID: 18062
		[FieldOffset(12)]
		public Vector3 velocity;

		// Token: 0x0400468F RID: 18063
		[FieldOffset(24)]
		public Quaternion rotation;

		// Token: 0x04004690 RID: 18064
		[FieldOffset(40)]
		public Vector3 scale;

		// Token: 0x04004691 RID: 18065
		[FieldOffset(52)]
		public double SentTime;
	}
}
