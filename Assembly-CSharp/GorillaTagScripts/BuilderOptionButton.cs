using System;

namespace GorillaTagScripts
{
	// Token: 0x02000ED8 RID: 3800
	public class BuilderOptionButton : GorillaPressableButton
	{
		// Token: 0x06005DA8 RID: 23976 RVA: 0x001DB3DB File Offset: 0x001D95DB
		public override void Start()
		{
			base.Start();
		}

		// Token: 0x06005DA9 RID: 23977 RVA: 0x000028C5 File Offset: 0x00000AC5
		private void OnDestroy()
		{
		}

		// Token: 0x06005DAA RID: 23978 RVA: 0x001DB3E3 File Offset: 0x001D95E3
		public void Setup(Action<BuilderOptionButton, bool> onPressed)
		{
			this.onPressed = onPressed;
		}

		// Token: 0x06005DAB RID: 23979 RVA: 0x001DB3EC File Offset: 0x001D95EC
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			Action<BuilderOptionButton, bool> action = this.onPressed;
			if (action == null)
			{
				return;
			}
			action(this, isLeftHand);
		}

		// Token: 0x06005DAC RID: 23980 RVA: 0x001DB400 File Offset: 0x001D9600
		public void SetPressed(bool pressed)
		{
			this.buttonRenderer.material = (pressed ? this.pressedMaterial : this.unpressedMaterial);
		}

		// Token: 0x04006C41 RID: 27713
		private new Action<BuilderOptionButton, bool> onPressed;
	}
}
