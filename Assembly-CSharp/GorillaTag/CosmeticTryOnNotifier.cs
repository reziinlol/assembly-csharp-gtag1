using System;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x02001158 RID: 4440
	[RequireComponent(typeof(VRRigCollection))]
	public class CosmeticTryOnNotifier : MonoBehaviour
	{
		// Token: 0x06007071 RID: 28785 RVA: 0x0024A7DC File Offset: 0x002489DC
		private void Awake()
		{
			if (!base.TryGetComponent<VRRigCollection>(out this.m_vrrigCollection))
			{
				this.m_vrrigCollection = this.AddComponent<VRRigCollection>();
			}
			VRRigCollection vrrigCollection = this.m_vrrigCollection;
			vrrigCollection.playerEnteredCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection.playerEnteredCollection, new Action<RigContainer>(this.PlayerEnteredTryOnSpace));
			VRRigCollection vrrigCollection2 = this.m_vrrigCollection;
			vrrigCollection2.playerLeftCollection = (Action<RigContainer>)Delegate.Combine(vrrigCollection2.playerLeftCollection, new Action<RigContainer>(this.PlayerLeftTryOnSpace));
		}

		// Token: 0x06007072 RID: 28786 RVA: 0x0024A854 File Offset: 0x00248A54
		private void PlayerEnteredTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(true, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			PlayerCosmeticsSystem.SetRigTemporarySpace(true, playerRig, this.unlockList.Strings);
		}

		// Token: 0x06007073 RID: 28787 RVA: 0x0024A88C File Offset: 0x00248A8C
		private void PlayerLeftTryOnSpace(RigContainer playerRig)
		{
			CosmeticTryOnNotifier.Mode mode = this.mode;
			if (mode == CosmeticTryOnNotifier.Mode.TRY_ON)
			{
				PlayerCosmeticsSystem.SetRigTryOn(false, playerRig);
				return;
			}
			if (mode != CosmeticTryOnNotifier.Mode.ENABLE_LIST)
			{
				return;
			}
			PlayerCosmeticsSystem.SetRigTemporarySpace(false, playerRig, this.unlockList.Strings);
		}

		// Token: 0x0400804E RID: 32846
		private VRRigCollection m_vrrigCollection;

		// Token: 0x0400804F RID: 32847
		[SerializeField]
		private CosmeticTryOnNotifier.Mode mode;

		// Token: 0x04008050 RID: 32848
		[SerializeField]
		private StringList unlockList;

		// Token: 0x02001159 RID: 4441
		private enum Mode
		{
			// Token: 0x04008052 RID: 32850
			TRY_ON,
			// Token: 0x04008053 RID: 32851
			ENABLE_LIST,
			// Token: 0x04008054 RID: 32852
			ENABLE_LIST_TITLEDATA
		}
	}
}
