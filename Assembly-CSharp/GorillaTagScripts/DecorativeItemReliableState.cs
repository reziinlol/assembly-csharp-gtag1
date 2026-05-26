using System;
using GorillaExtensions;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000EFD RID: 3837
	public class DecorativeItemReliableState : MonoBehaviour, IPunObservable
	{
		// Token: 0x06005F5B RID: 24411 RVA: 0x001EAF2C File Offset: 0x001E912C
		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				stream.SendNext(this.isSnapped);
				stream.SendNext(this.snapPosition);
				stream.SendNext(this.respawnPosition);
				stream.SendNext(this.respawnRotation);
				return;
			}
			this.isSnapped = (bool)stream.ReceiveNext();
			this.snapPosition = (Vector3)stream.ReceiveNext();
			this.respawnPosition = (Vector3)stream.ReceiveNext();
			this.respawnRotation = (Quaternion)stream.ReceiveNext();
			float num = 10000f;
			if (!this.snapPosition.IsValid(num))
			{
				this.snapPosition = Vector3.zero;
			}
			num = 10000f;
			if (!this.respawnPosition.IsValid(num))
			{
				this.respawnPosition = Vector3.zero;
			}
			if (!this.respawnRotation.IsValid())
			{
				this.respawnRotation = quaternion.identity;
			}
		}

		// Token: 0x04006E1A RID: 28186
		public bool isSnapped;

		// Token: 0x04006E1B RID: 28187
		public Vector3 snapPosition = Vector3.zero;

		// Token: 0x04006E1C RID: 28188
		public Vector3 respawnPosition = Vector3.zero;

		// Token: 0x04006E1D RID: 28189
		public Quaternion respawnRotation = Quaternion.identity;
	}
}
