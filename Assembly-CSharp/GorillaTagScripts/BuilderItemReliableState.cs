using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000ED7 RID: 3799
	public class BuilderItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06005DA6 RID: 23974 RVA: 0x001DB288 File Offset: 0x001D9488
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.rightHandAttachPos);
				stream.SendNext(this.rightHandAttachRot);
				stream.SendNext(this.leftHandAttachPos);
				stream.SendNext(this.leftHandAttachRot);
				return;
			}
			this.rightHandAttachPos = (Vector3)stream.ReceiveNext();
			this.rightHandAttachRot = (Quaternion)stream.ReceiveNext();
			this.leftHandAttachPos = (Vector3)stream.ReceiveNext();
			this.leftHandAttachRot = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!this.rightHandAttachPos.IsValid(num))
			{
				this.rightHandAttachPos = Vector3.zero;
			}
			if (!this.rightHandAttachRot.IsValid())
			{
				this.rightHandAttachRot = quaternion.identity;
			}
			num = 10000f;
			if (!this.leftHandAttachPos.IsValid(num))
			{
				this.leftHandAttachPos = Vector3.zero;
			}
			if (!this.leftHandAttachRot.IsValid())
			{
				this.leftHandAttachRot = quaternion.identity;
			}
			this.dirty = true;
		}

		// Token: 0x04006C3C RID: 27708
		public Vector3 rightHandAttachPos = Vector3.zero;

		// Token: 0x04006C3D RID: 27709
		public Quaternion rightHandAttachRot = Quaternion.identity;

		// Token: 0x04006C3E RID: 27710
		public Vector3 leftHandAttachPos = Vector3.zero;

		// Token: 0x04006C3F RID: 27711
		public Quaternion leftHandAttachRot = Quaternion.identity;

		// Token: 0x04006C40 RID: 27712
		public bool dirty;
	}
}
