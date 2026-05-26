using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000EB3 RID: 3763
	[HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
	public class RigOwnedTransformView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06005C7E RID: 23678 RVA: 0x001D597A File Offset: 0x001D3B7A
		// (set) Token: 0x06005C7F RID: 23679 RVA: 0x001D5982 File Offset: 0x001D3B82
		public bool IsMine { get; private set; }

		// Token: 0x06005C80 RID: 23680 RVA: 0x001D598B File Offset: 0x001D3B8B
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06005C81 RID: 23681 RVA: 0x001D5994 File Offset: 0x001D3B94
		public void Awake()
		{
			this.m_StoredPosition = base.transform.localPosition;
			this.m_NetworkPosition = Vector3.zero;
			this.m_networkScale = Vector3.one;
			this.m_NetworkRotation = Quaternion.identity;
		}

		// Token: 0x06005C82 RID: 23682 RVA: 0x001D59C8 File Offset: 0x001D3BC8
		private void Reset()
		{
			this.m_UseLocal = true;
		}

		// Token: 0x06005C83 RID: 23683 RVA: 0x001D59D1 File Offset: 0x001D3BD1
		private void OnEnable()
		{
			this.m_firstTake = true;
		}

		// Token: 0x06005C84 RID: 23684 RVA: 0x001D59DC File Offset: 0x001D3BDC
		public void Update()
		{
			Transform transform = base.transform;
			if (!this.IsMine && this.IsValid(this.m_NetworkPosition) && this.IsValid(this.m_NetworkRotation))
			{
				if (this.m_UseLocal)
				{
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					transform.localRotation = Quaternion.RotateTowards(transform.localRotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
					return;
				}
				transform.position = Vector3.MoveTowards(transform.position, this.m_NetworkPosition, this.m_Distance * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, this.m_NetworkRotation, this.m_Angle * Time.deltaTime * (float)PhotonNetwork.SerializationRate);
			}
		}

		// Token: 0x06005C85 RID: 23685 RVA: 0x001D5AD0 File Offset: 0x001D3CD0
		private bool IsValid(Vector3 v)
		{
			return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
		}

		// Token: 0x06005C86 RID: 23686 RVA: 0x001D5B30 File Offset: 0x001D3D30
		private bool IsValid(Quaternion q)
		{
			return !float.IsNaN(q.x) && !float.IsNaN(q.y) && !float.IsNaN(q.z) && !float.IsNaN(q.w) && !float.IsInfinity(q.x) && !float.IsInfinity(q.y) && !float.IsInfinity(q.z) && !float.IsInfinity(q.w);
		}

		// Token: 0x06005C87 RID: 23687 RVA: 0x001D5BA8 File Offset: 0x001D3DA8
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			try
			{
				Transform transform = base.transform;
				if (stream.IsWriting)
				{
					if (this.m_SynchronizePosition)
					{
						if (this.m_UseLocal)
						{
							this.m_Direction = transform.localPosition - this.m_StoredPosition;
							this.m_StoredPosition = transform.localPosition;
							stream.SendNext(transform.localPosition);
							stream.SendNext(this.m_Direction);
						}
						else
						{
							this.m_Direction = transform.position - this.m_StoredPosition;
							this.m_StoredPosition = transform.position;
							stream.SendNext(transform.position);
							stream.SendNext(this.m_Direction);
						}
					}
					if (this.m_SynchronizeRotation)
					{
						if (this.m_UseLocal)
						{
							stream.SendNext(transform.localRotation);
						}
						else
						{
							stream.SendNext(transform.rotation);
						}
					}
					if (this.m_SynchronizeScale)
					{
						stream.SendNext(transform.localScale);
					}
				}
				else
				{
					if (this.m_SynchronizePosition)
					{
						Vector3 vector = (Vector3)stream.ReceiveNext();
						ref this.m_NetworkPosition.SetValueSafe(vector);
						vector = (Vector3)stream.ReceiveNext();
						ref this.m_Direction.SetValueSafe(vector);
						if (this.m_firstTake)
						{
							if (this.m_UseLocal)
							{
								transform.localPosition = this.m_NetworkPosition;
							}
							else
							{
								transform.position = this.m_NetworkPosition;
							}
							this.m_Distance = 0f;
						}
						else
						{
							float d = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
							this.m_NetworkPosition += this.m_Direction * d;
							if (this.m_UseLocal)
							{
								this.m_Distance = Vector3.Distance(transform.localPosition, this.m_NetworkPosition);
							}
							else
							{
								this.m_Distance = Vector3.Distance(transform.position, this.m_NetworkPosition);
							}
						}
					}
					if (this.m_SynchronizeRotation)
					{
						Quaternion quaternion = (Quaternion)stream.ReceiveNext();
						ref this.m_NetworkRotation.SetValueSafe(quaternion);
						if (this.m_firstTake)
						{
							this.m_Angle = 0f;
							if (this.m_UseLocal)
							{
								transform.localRotation = this.m_NetworkRotation;
							}
							else
							{
								transform.rotation = this.m_NetworkRotation;
							}
						}
						else if (this.m_UseLocal)
						{
							this.m_Angle = Quaternion.Angle(transform.localRotation, this.m_NetworkRotation);
						}
						else
						{
							this.m_Angle = Quaternion.Angle(transform.rotation, this.m_NetworkRotation);
						}
					}
					if (this.m_SynchronizeScale)
					{
						Vector3 vector = (Vector3)stream.ReceiveNext();
						ref this.m_networkScale.SetValueSafe(vector);
						transform.localScale = this.m_networkScale;
					}
					if (this.m_firstTake)
					{
						this.m_firstTake = false;
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x06005C88 RID: 23688 RVA: 0x001D59D1 File Offset: 0x001D3BD1
		public void GTAddition_DoTeleport()
		{
			this.m_firstTake = true;
		}

		// Token: 0x04006ADF RID: 27359
		private float m_Distance;

		// Token: 0x04006AE0 RID: 27360
		private float m_Angle;

		// Token: 0x04006AE1 RID: 27361
		private Vector3 m_Direction;

		// Token: 0x04006AE2 RID: 27362
		private Vector3 m_NetworkPosition;

		// Token: 0x04006AE3 RID: 27363
		private Vector3 m_StoredPosition;

		// Token: 0x04006AE4 RID: 27364
		private Vector3 m_networkScale;

		// Token: 0x04006AE5 RID: 27365
		private Quaternion m_NetworkRotation;

		// Token: 0x04006AE6 RID: 27366
		public bool m_SynchronizePosition = true;

		// Token: 0x04006AE7 RID: 27367
		public bool m_SynchronizeRotation = true;

		// Token: 0x04006AE8 RID: 27368
		public bool m_SynchronizeScale;

		// Token: 0x04006AE9 RID: 27369
		[Tooltip("Indicates if localPosition and localRotation should be used. Scale ignores this setting, and always uses localScale to avoid issues with lossyScale.")]
		public bool m_UseLocal;

		// Token: 0x04006AEA RID: 27370
		private bool m_firstTake;
	}
}
