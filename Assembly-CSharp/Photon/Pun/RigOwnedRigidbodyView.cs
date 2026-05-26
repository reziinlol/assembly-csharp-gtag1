using System;
using GorillaExtensions;
using UnityEngine;

namespace Photon.Pun
{
	// Token: 0x02000EB2 RID: 3762
	[RequireComponent(typeof(Rigidbody))]
	public class RigOwnedRigidbodyView : MonoBehaviourPun, IPunObservable
	{
		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x06005C77 RID: 23671 RVA: 0x001D5624 File Offset: 0x001D3824
		// (set) Token: 0x06005C78 RID: 23672 RVA: 0x001D562C File Offset: 0x001D382C
		public bool IsMine { get; private set; }

		// Token: 0x06005C79 RID: 23673 RVA: 0x001D5635 File Offset: 0x001D3835
		public void SetIsMine(bool isMine)
		{
			this.IsMine = isMine;
		}

		// Token: 0x06005C7A RID: 23674 RVA: 0x001D563E File Offset: 0x001D383E
		public void Awake()
		{
			this.m_Body = base.GetComponent<Rigidbody>();
			this.m_NetworkPosition = default(Vector3);
			this.m_NetworkRotation = default(Quaternion);
		}

		// Token: 0x06005C7B RID: 23675 RVA: 0x001D5664 File Offset: 0x001D3864
		public void FixedUpdate()
		{
			if (!this.IsMine)
			{
				this.m_Body.position = Vector3.MoveTowards(this.m_Body.position, this.m_NetworkPosition, this.m_Distance * (1f / (float)PhotonNetwork.SerializationRate));
				this.m_Body.rotation = Quaternion.RotateTowards(this.m_Body.rotation, this.m_NetworkRotation, this.m_Angle * (1f / (float)PhotonNetwork.SerializationRate));
			}
		}

		// Token: 0x06005C7C RID: 23676 RVA: 0x001D56E4 File Offset: 0x001D38E4
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (info.Sender != info.photonView.Owner)
			{
				return;
			}
			try
			{
				if (stream.IsWriting)
				{
					stream.SendNext(this.m_Body.position);
					stream.SendNext(this.m_Body.rotation);
					if (this.m_SynchronizeVelocity)
					{
						stream.SendNext(this.m_Body.linearVelocity);
					}
					if (this.m_SynchronizeAngularVelocity)
					{
						stream.SendNext(this.m_Body.angularVelocity);
					}
					stream.SendNext(this.m_Body.IsSleeping());
				}
				else
				{
					Vector3 vector = (Vector3)stream.ReceiveNext();
					ref this.m_NetworkPosition.SetValueSafe(vector);
					Quaternion quaternion = (Quaternion)stream.ReceiveNext();
					ref this.m_NetworkRotation.SetValueSafe(quaternion);
					if (this.m_TeleportEnabled && Vector3.Distance(this.m_Body.position, this.m_NetworkPosition) > this.m_TeleportIfDistanceGreaterThan)
					{
						this.m_Body.position = this.m_NetworkPosition;
					}
					if (this.m_SynchronizeVelocity || this.m_SynchronizeAngularVelocity)
					{
						float d = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
						if (this.m_SynchronizeVelocity)
						{
							Vector3 linearVelocity = (Vector3)stream.ReceiveNext();
							float num = 10000f;
							if (!linearVelocity.IsValid(num))
							{
								linearVelocity = Vector3.zero;
							}
							if (!this.m_Body.isKinematic)
							{
								this.m_Body.linearVelocity = linearVelocity;
							}
							this.m_NetworkPosition += this.m_Body.linearVelocity * d;
							this.m_Distance = Vector3.Distance(this.m_Body.position, this.m_NetworkPosition);
						}
						if (this.m_SynchronizeAngularVelocity)
						{
							Vector3 angularVelocity = (Vector3)stream.ReceiveNext();
							float num = 10000f;
							if (!angularVelocity.IsValid(num))
							{
								angularVelocity = Vector3.zero;
							}
							this.m_Body.angularVelocity = angularVelocity;
							this.m_NetworkRotation = Quaternion.Euler(this.m_Body.angularVelocity * d) * this.m_NetworkRotation;
							this.m_Angle = Quaternion.Angle(this.m_Body.rotation, this.m_NetworkRotation);
						}
					}
					if ((bool)stream.ReceiveNext())
					{
						this.m_Body.Sleep();
					}
				}
			}
			catch
			{
			}
		}

		// Token: 0x04006AD5 RID: 27349
		private float m_Distance;

		// Token: 0x04006AD6 RID: 27350
		private float m_Angle;

		// Token: 0x04006AD7 RID: 27351
		private Rigidbody m_Body;

		// Token: 0x04006AD8 RID: 27352
		private Vector3 m_NetworkPosition;

		// Token: 0x04006AD9 RID: 27353
		private Quaternion m_NetworkRotation;

		// Token: 0x04006ADA RID: 27354
		public bool m_SynchronizeVelocity = true;

		// Token: 0x04006ADB RID: 27355
		public bool m_SynchronizeAngularVelocity;

		// Token: 0x04006ADC RID: 27356
		public bool m_TeleportEnabled;

		// Token: 0x04006ADD RID: 27357
		public float m_TeleportIfDistanceGreaterThan = 3f;
	}
}
