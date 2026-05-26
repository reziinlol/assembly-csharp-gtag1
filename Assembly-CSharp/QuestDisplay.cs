using System;
using TMPro;
using UnityEngine;

// Token: 0x0200025D RID: 605
public class QuestDisplay : MonoBehaviour
{
	// Token: 0x17000194 RID: 404
	// (get) Token: 0x06001038 RID: 4152 RVA: 0x00056F36 File Offset: 0x00055136
	public bool IsChanged
	{
		get
		{
			return this.quest.lastChange > this._lastUpdate;
		}
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x00056F4C File Offset: 0x0005514C
	public void UpdateDisplay()
	{
		this.text.text = this.quest.GetTextDescription();
		if (this.quest.isQuestComplete)
		{
			this.progressDisplay.SetVisible(false);
		}
		else if (this.quest.requiredOccurenceCount > 1)
		{
			this.progressDisplay.SetProgress(this.quest.occurenceCount, this.quest.requiredOccurenceCount);
			this.progressDisplay.SetVisible(true);
		}
		else
		{
			this.progressDisplay.SetVisible(false);
		}
		this.UpdateCompletionIndicator();
		this._lastUpdate = Time.frameCount;
	}

	// Token: 0x0600103A RID: 4154 RVA: 0x00056FE4 File Offset: 0x000551E4
	private void UpdateCompletionIndicator()
	{
		bool isQuestComplete = this.quest.isQuestComplete;
		bool flag = !isQuestComplete && this.quest.requiredOccurenceCount == 1;
		this.dailyIncompleteIndicator.SetActive(this.quest.isDailyQuest && flag);
		this.dailyCompleteIndicator.SetActive(this.quest.isDailyQuest && isQuestComplete);
		this.weeklyIncompleteIndicator.SetActive(!this.quest.isDailyQuest && flag);
		this.weeklyCompleteIndicator.SetActive(!this.quest.isDailyQuest && isQuestComplete);
	}

	// Token: 0x04001361 RID: 4961
	[SerializeField]
	private ProgressDisplay progressDisplay;

	// Token: 0x04001362 RID: 4962
	[SerializeField]
	private TMP_Text text;

	// Token: 0x04001363 RID: 4963
	[SerializeField]
	private TMP_Text statusText;

	// Token: 0x04001364 RID: 4964
	[SerializeField]
	private GameObject dailyIncompleteIndicator;

	// Token: 0x04001365 RID: 4965
	[SerializeField]
	private GameObject dailyCompleteIndicator;

	// Token: 0x04001366 RID: 4966
	[SerializeField]
	private GameObject weeklyIncompleteIndicator;

	// Token: 0x04001367 RID: 4967
	[SerializeField]
	private GameObject weeklyCompleteIndicator;

	// Token: 0x04001368 RID: 4968
	[NonSerialized]
	public RotatingQuest quest;

	// Token: 0x04001369 RID: 4969
	private int _lastUpdate = -1;
}
