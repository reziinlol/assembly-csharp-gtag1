using System;

// Token: 0x02000261 RID: 609
public interface GorillaQuestManager
{
	// Token: 0x06001050 RID: 4176
	void LoadQuestsFromJson(string jsonString);

	// Token: 0x06001051 RID: 4177
	void LoadQuestProgress();

	// Token: 0x06001052 RID: 4178
	void SaveQuestProgress();

	// Token: 0x06001053 RID: 4179
	void SetupAllQuestEventListeners();

	// Token: 0x06001054 RID: 4180
	void ClearAllQuestEventListeners();

	// Token: 0x06001055 RID: 4181
	void HandleQuestProgressChanged(bool initialLoad);

	// Token: 0x06001056 RID: 4182
	void HandleQuestCompleted(int questID);
}
