using System;
using TMPro;
using UnityEngine;

// Token: 0x02000A32 RID: 2610
public class CustomMapsTerminalControlButton : CustomMapsScreenTouchPoint
{
	// Token: 0x17000630 RID: 1584
	// (get) Token: 0x060042B9 RID: 17081 RVA: 0x00164873 File Offset: 0x00162A73
	// (set) Token: 0x060042BA RID: 17082 RVA: 0x0016487B File Offset: 0x00162A7B
	public bool IsLocked
	{
		get
		{
			return this.isLocked;
		}
		set
		{
			this.isLocked = value;
		}
	}

	// Token: 0x060042BB RID: 17083 RVA: 0x00164884 File Offset: 0x00162A84
	protected override void OnButtonPressedEvent()
	{
		GTDev.Log<string>("terminal control pressed", null);
		if (this.mapsTerminal == null)
		{
			return;
		}
		this.mapsTerminal.HandleTerminalControlButtonPressed();
	}

	// Token: 0x060042BC RID: 17084 RVA: 0x001648AB File Offset: 0x00162AAB
	public void LockTerminalControl()
	{
		if (this.IsLocked)
		{
			return;
		}
		this.IsLocked = true;
		this.PressButtonColourUpdate();
	}

	// Token: 0x060042BD RID: 17085 RVA: 0x001648C3 File Offset: 0x00162AC3
	public void UnlockTerminalControl()
	{
		if (!this.IsLocked)
		{
			return;
		}
		this.IsLocked = false;
		this.PressButtonColourUpdate();
	}

	// Token: 0x060042BE RID: 17086 RVA: 0x001648DC File Offset: 0x00162ADC
	public override void PressButtonColourUpdate()
	{
		this.bttnText.fontSize = (this.isLocked ? this.lockedFontSize : this.unlockedFontSize);
		this.bttnText.text = (this.isLocked ? this.lockedText : this.unlockedText);
		this.bttnText.color = (this.isLocked ? this.lockedTextColor : this.unlockedTextColor);
		this.touchPointRenderer.color = (this.isLocked ? this.buttonColorSettings.PressedColor : this.buttonColorSettings.UnpressedColor);
	}

	// Token: 0x040054A7 RID: 21671
	[SerializeField]
	private TMP_Text bttnText;

	// Token: 0x040054A8 RID: 21672
	[SerializeField]
	private string unlockedText = "TERMINAL AVAILABLE";

	// Token: 0x040054A9 RID: 21673
	[SerializeField]
	private string lockedText = "TERMINAL UNAVAILABLE";

	// Token: 0x040054AA RID: 21674
	[SerializeField]
	private float unlockedFontSize = 30f;

	// Token: 0x040054AB RID: 21675
	[SerializeField]
	private float lockedFontSize = 30f;

	// Token: 0x040054AC RID: 21676
	[SerializeField]
	private Color unlockedTextColor = Color.black;

	// Token: 0x040054AD RID: 21677
	[SerializeField]
	private Color lockedTextColor = Color.white;

	// Token: 0x040054AE RID: 21678
	private bool isLocked;

	// Token: 0x040054AF RID: 21679
	[SerializeField]
	private CustomMapsTerminal mapsTerminal;
}
