using System;
using TMPro;
using UnityEngine;

// Token: 0x020005AA RID: 1450
public class GorillaDebugUI : MonoBehaviour
{
	// Token: 0x04003003 RID: 12291
	private readonly float Delay = 0.5f;

	// Token: 0x04003004 RID: 12292
	public GameObject parentCanvas;

	// Token: 0x04003005 RID: 12293
	public GameObject rayInteractorLeft;

	// Token: 0x04003006 RID: 12294
	public GameObject rayInteractorRight;

	// Token: 0x04003007 RID: 12295
	[SerializeField]
	private TMP_Dropdown playfabIdDropdown;

	// Token: 0x04003008 RID: 12296
	[SerializeField]
	private TMP_Dropdown roomIdDropdown;

	// Token: 0x04003009 RID: 12297
	[SerializeField]
	private TMP_Dropdown locationDropdown;

	// Token: 0x0400300A RID: 12298
	[SerializeField]
	private TMP_Dropdown playerNameDropdown;

	// Token: 0x0400300B RID: 12299
	[SerializeField]
	private TMP_Dropdown gameModeDropdown;

	// Token: 0x0400300C RID: 12300
	[SerializeField]
	private TMP_Dropdown timeOfDayDropdown;

	// Token: 0x0400300D RID: 12301
	[SerializeField]
	private TMP_Text networkStateTextBox;

	// Token: 0x0400300E RID: 12302
	[SerializeField]
	private TMP_Text gameModeTextBox;

	// Token: 0x0400300F RID: 12303
	[SerializeField]
	private TMP_Text currentRoomTextBox;

	// Token: 0x04003010 RID: 12304
	[SerializeField]
	private TMP_Text playerCountTextBox;

	// Token: 0x04003011 RID: 12305
	[SerializeField]
	private TMP_Text roomVisibilityTextBox;

	// Token: 0x04003012 RID: 12306
	[SerializeField]
	private TMP_Text timeMultiplierTextBox;

	// Token: 0x04003013 RID: 12307
	[SerializeField]
	private TMP_Text versionTextBox;
}
