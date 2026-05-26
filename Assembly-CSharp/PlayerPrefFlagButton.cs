using System;
using UnityEngine;

// Token: 0x02000496 RID: 1174
public class PlayerPrefFlagButton : GorillaPressableButton
{
	// Token: 0x06001C72 RID: 7282 RVA: 0x0009A1BE File Offset: 0x000983BE
	protected override void OnEnable()
	{
		base.OnEnable();
		this.isOn = PlayerPrefFlags.Check(this.flag);
		this.UpdateColor();
	}

	// Token: 0x06001C73 RID: 7283 RVA: 0x0009A1E0 File Offset: 0x000983E0
	public override void ButtonActivation()
	{
		PlayerPrefFlagButton.ButtonMode buttonMode = this.mode;
		if (buttonMode == PlayerPrefFlagButton.ButtonMode.SET_VALUE)
		{
			PlayerPrefFlags.Set(this.flag, this.value);
			this.isOn = this.value;
			this.UpdateColor();
			return;
		}
		if (buttonMode != PlayerPrefFlagButton.ButtonMode.TOGGLE)
		{
			return;
		}
		this.isOn = PlayerPrefFlags.Flip(this.flag);
		this.UpdateColor();
	}

	// Token: 0x04002683 RID: 9859
	[SerializeField]
	private PlayerPrefFlags.Flag flag;

	// Token: 0x04002684 RID: 9860
	[SerializeField]
	private PlayerPrefFlagButton.ButtonMode mode;

	// Token: 0x04002685 RID: 9861
	[SerializeField]
	private bool value;

	// Token: 0x02000497 RID: 1175
	private enum ButtonMode
	{
		// Token: 0x04002687 RID: 9863
		SET_VALUE,
		// Token: 0x04002688 RID: 9864
		TOGGLE
	}
}
