using System;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;

// Token: 0x02000A92 RID: 2706
public class CustomMapsKeyToggleButton : CustomMapsKeyButton
{
	// Token: 0x06004511 RID: 17681 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void PressButtonColourUpdate()
	{
	}

	// Token: 0x06004512 RID: 17682 RVA: 0x00174F14 File Offset: 0x00173114
	public void SetButtonStatus(bool newIsPressed)
	{
		if (this.isPressed == newIsPressed)
		{
			return;
		}
		this.isPressed = newIsPressed;
		this.propBlock.SetColor("_BaseColor", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.propBlock.SetColor("_Color", this.isPressed ? this.ButtonColorSettings.PressedColor : this.ButtonColorSettings.UnpressedColor);
		this.ButtonRenderer.SetPropertyBlock(this.propBlock);
	}

	// Token: 0x0400574B RID: 22347
	private bool isPressed;
}
