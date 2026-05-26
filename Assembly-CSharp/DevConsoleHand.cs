using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200031A RID: 794
public class DevConsoleHand : DevConsoleInstance
{
	// Token: 0x04001894 RID: 6292
	public List<GameObject> otherButtonsList;

	// Token: 0x04001895 RID: 6293
	public bool isStillEnabled = true;

	// Token: 0x04001896 RID: 6294
	public bool isLeftHand;

	// Token: 0x04001897 RID: 6295
	public ConsoleMode mode;

	// Token: 0x04001898 RID: 6296
	public double debugScale;

	// Token: 0x04001899 RID: 6297
	public double inspectorScale;

	// Token: 0x0400189A RID: 6298
	public double componentInspectorScale;

	// Token: 0x0400189B RID: 6299
	public List<GameObject> consoleButtons;

	// Token: 0x0400189C RID: 6300
	public List<GameObject> inspectorButtons;

	// Token: 0x0400189D RID: 6301
	public List<GameObject> componentInspectorButtons;

	// Token: 0x0400189E RID: 6302
	public GorillaDevButton consoleButton;

	// Token: 0x0400189F RID: 6303
	public GorillaDevButton inspectorButton;

	// Token: 0x040018A0 RID: 6304
	public GorillaDevButton componentInspectorButton;

	// Token: 0x040018A1 RID: 6305
	public GorillaDevButton showNonStarItems;

	// Token: 0x040018A2 RID: 6306
	public GorillaDevButton showPrivateItems;

	// Token: 0x040018A3 RID: 6307
	public Text componentInspectionText;

	// Token: 0x040018A4 RID: 6308
	public DevInspector selectedInspector;
}
