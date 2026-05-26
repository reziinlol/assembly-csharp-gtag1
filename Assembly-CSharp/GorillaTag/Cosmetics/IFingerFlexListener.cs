using System;

namespace GorillaTag.Cosmetics
{
	// Token: 0x0200128D RID: 4749
	public interface IFingerFlexListener
	{
		// Token: 0x060076F4 RID: 30452 RVA: 0x00023994 File Offset: 0x00021B94
		bool FingerFlexValidation(bool isLeftHand)
		{
			return true;
		}

		// Token: 0x060076F5 RID: 30453
		void OnButtonPressed(bool isLeftHand, float value);

		// Token: 0x060076F6 RID: 30454
		void OnButtonReleased(bool isLeftHand, float value);

		// Token: 0x060076F7 RID: 30455
		void OnButtonPressStayed(bool isLeftHand, float value);

		// Token: 0x0200128E RID: 4750
		public enum ComponentActivator
		{
			// Token: 0x04008939 RID: 35129
			FingerReleased,
			// Token: 0x0400893A RID: 35130
			FingerFlexed,
			// Token: 0x0400893B RID: 35131
			FingerStayed
		}
	}
}
