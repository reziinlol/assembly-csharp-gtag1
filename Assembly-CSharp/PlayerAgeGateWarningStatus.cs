using System;

// Token: 0x02000BBF RID: 3007
internal struct PlayerAgeGateWarningStatus
{
	// Token: 0x04005E66 RID: 24166
	public string header;

	// Token: 0x04005E67 RID: 24167
	public string body;

	// Token: 0x04005E68 RID: 24168
	public string leftButtonText;

	// Token: 0x04005E69 RID: 24169
	public string rightButtonText;

	// Token: 0x04005E6A RID: 24170
	public WarningButtonResult leftButtonResult;

	// Token: 0x04005E6B RID: 24171
	public WarningButtonResult rightButtonResult;

	// Token: 0x04005E6C RID: 24172
	public WarningButtonResult noWarningResult;

	// Token: 0x04005E6D RID: 24173
	public EImageVisibility showImage;

	// Token: 0x04005E6E RID: 24174
	public Action onLeftButtonPressedAction;

	// Token: 0x04005E6F RID: 24175
	public Action onRightButtonPressedAction;
}
