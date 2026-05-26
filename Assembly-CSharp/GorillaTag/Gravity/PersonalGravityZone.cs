using System;
using GorillaLocomotion;
using UnityEngine;

namespace GorillaTag.Gravity
{
	// Token: 0x0200118E RID: 4494
	public class PersonalGravityZone : BasicGravityZone
	{
		// Token: 0x060071D5 RID: 29141 RVA: 0x001A9D25 File Offset: 0x001A7F25
		public void SetLocalPlayerGravityDirection(Vector3 direction)
		{
			GTPlayerTransform.Instance.SetPersonalGravityDirection(direction);
		}

		// Token: 0x060071D6 RID: 29142 RVA: 0x001A9D32 File Offset: 0x001A7F32
		public void SetLocalPlayerGravityDirection(Transform referenceDir)
		{
			GTPlayerTransform.Instance.SetPersonalGravityDirection(referenceDir);
		}

		// Token: 0x060071D7 RID: 29143 RVA: 0x00251242 File Offset: 0x0024F442
		protected override Vector3 GetGravityVectorAtPoint(in Vector3 worldPosition, in MonkeGravityController controller)
		{
			return controller.PersonalGravityDirection;
		}

		// Token: 0x060071D8 RID: 29144 RVA: 0x0025124B File Offset: 0x0024F44B
		private void ResetLocalPlayerIfMatch(MonkeGravityController controller)
		{
			if (controller == GTPlayerTransform.Instance)
			{
				GTPlayerTransform.Instance.SetPersonalGravityDirection(Vector3.up);
			}
		}

		// Token: 0x060071D9 RID: 29145 RVA: 0x00251269 File Offset: 0x0024F469
		protected override void OnTargetExited(MonkeGravityController target)
		{
			this.ResetLocalPlayerIfMatch(target);
		}

		// Token: 0x060071DA RID: 29146 RVA: 0x00251269 File Offset: 0x0024F469
		protected override void OnTargetFilteredOut(MonkeGravityController target)
		{
			this.ResetLocalPlayerIfMatch(target);
		}
	}
}
