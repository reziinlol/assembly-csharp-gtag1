using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200031B RID: 795
public class DevConsoleInstance : MonoBehaviour
{
	// Token: 0x060013DD RID: 5085 RVA: 0x000440BC File Offset: 0x000422BC
	private void OnEnable()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x040018A5 RID: 6309
	public GorillaDevButton[] buttons;

	// Token: 0x040018A6 RID: 6310
	public GameObject[] disableWhileActive;

	// Token: 0x040018A7 RID: 6311
	public GameObject[] enableWhileActive;

	// Token: 0x040018A8 RID: 6312
	public float maxHeight;

	// Token: 0x040018A9 RID: 6313
	public float lineHeight;

	// Token: 0x040018AA RID: 6314
	public int targetLogIndex = -1;

	// Token: 0x040018AB RID: 6315
	public int currentLogIndex;

	// Token: 0x040018AC RID: 6316
	public int expandAmount = 20;

	// Token: 0x040018AD RID: 6317
	public int expandedMessageIndex = -1;

	// Token: 0x040018AE RID: 6318
	public bool canExpand = true;

	// Token: 0x040018AF RID: 6319
	public List<DevConsole.DisplayedLogLine> logLines = new List<DevConsole.DisplayedLogLine>();

	// Token: 0x040018B0 RID: 6320
	public HashSet<LogType> selectedLogTypes = new HashSet<LogType>
	{
		LogType.Error,
		LogType.Exception,
		LogType.Log,
		LogType.Warning,
		LogType.Assert
	};

	// Token: 0x040018B1 RID: 6321
	[SerializeField]
	private GorillaDevButton[] logTypeButtons;

	// Token: 0x040018B2 RID: 6322
	[SerializeField]
	private GorillaDevButton BottomButton;

	// Token: 0x040018B3 RID: 6323
	public float lineStartHeight;

	// Token: 0x040018B4 RID: 6324
	public float lineStartZ;

	// Token: 0x040018B5 RID: 6325
	public float textStartHeight;

	// Token: 0x040018B6 RID: 6326
	public float lineStartTextWidth;

	// Token: 0x040018B7 RID: 6327
	public double textScale = 0.5;

	// Token: 0x040018B8 RID: 6328
	public bool isEnabled = true;

	// Token: 0x040018B9 RID: 6329
	[SerializeField]
	private GameObject ConsoleLineExample;
}
