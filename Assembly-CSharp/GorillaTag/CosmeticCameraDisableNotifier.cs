using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001157 RID: 4439
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticCameraDisableNotifier : MonoBehaviour
	{
		// Token: 0x0600706D RID: 28781 RVA: 0x0024A730 File Offset: 0x00248930
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this._vrrigCollection))
			{
				this._vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this._vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this._vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x0600706E RID: 28782 RVA: 0x0024A7A5 File Offset: 0x002489A5
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = false;
			}
		}

		// Token: 0x0600706F RID: 28783 RVA: 0x0024A7C0 File Offset: 0x002489C0
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			if (playerRig.Rig.isLocal)
			{
				this._cosmeticCamera.enabled = true;
			}
		}

		// Token: 0x0400804C RID: 32844
		private VRRigCollection _vrrigCollection;

		// Token: 0x0400804D RID: 32845
		[SerializeField]
		private Camera _cosmeticCamera;
	}
}
