using System;
using GorillaExtensions;
using TMPro;
using UnityEngine;

// Token: 0x02000A9C RID: 2716
public class CustomMapsScreenButton : CustomMapsScreenTouchPoint
{
	// Token: 0x0600455D RID: 17757 RVA: 0x0017747E File Offset: 0x0017567E
	protected override void OnDisable()
	{
		base.OnDisable();
		if (this.isToggle)
		{
			this.SetButtonActive(this.isActive);
			return;
		}
		this.isActive = false;
	}

	// Token: 0x0600455E RID: 17758 RVA: 0x001774A2 File Offset: 0x001756A2
	public void SetButtonText(string text)
	{
		if (this.bttnText.IsNull())
		{
			return;
		}
		this.bttnText.text = text;
	}

	// Token: 0x0600455F RID: 17759 RVA: 0x001774BE File Offset: 0x001756BE
	public void SetButtonActive(bool active)
	{
		this.isActive = active;
		this.touchPointRenderer.color = (this.isActive ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x06004560 RID: 17760 RVA: 0x001774F2 File Offset: 0x001756F2
	public override void PressButtonColourUpdate()
	{
		if (!this.isToggle)
		{
			base.PressButtonColourUpdate();
			return;
		}
	}

	// Token: 0x06004561 RID: 17761 RVA: 0x00177503 File Offset: 0x00175703
	protected override void OnButtonPressedEvent()
	{
		this.isActive = !this.isActive;
	}

	// Token: 0x040057CD RID: 22477
	[SerializeField]
	private TMP_Text bttnText;

	// Token: 0x040057CE RID: 22478
	[SerializeField]
	private bool isToggle;

	// Token: 0x040057CF RID: 22479
	private bool isActive;
}
