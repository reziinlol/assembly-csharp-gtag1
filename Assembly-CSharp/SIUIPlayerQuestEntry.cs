using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000179 RID: 377
public class SIUIPlayerQuestEntry : MonoBehaviour
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x0003563A File Offset: 0x0003383A
	private void Awake()
	{
		this.lastQuestId = -1;
		this.lastQuestProgress = -1;
	}

	// Token: 0x04000C33 RID: 3123
	public Image background;

	// Token: 0x04000C34 RID: 3124
	public SIUIProgressBar progress;

	// Token: 0x04000C35 RID: 3125
	public TextMeshProUGUI questDescription;

	// Token: 0x04000C36 RID: 3126
	public GameObject completeOverlay;

	// Token: 0x04000C37 RID: 3127
	public GameObject questInfo;

	// Token: 0x04000C38 RID: 3128
	public GameObject noQuestAvailable;

	// Token: 0x04000C39 RID: 3129
	public GameObject newQuestTag;

	// Token: 0x04000C3A RID: 3130
	public int lastQuestId;

	// Token: 0x04000C3B RID: 3131
	public int lastQuestProgress;
}
