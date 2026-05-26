using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

// Token: 0x02000264 RID: 612
[Serializable]
public class RotatingQuest
{
	// Token: 0x17000199 RID: 409
	// (get) Token: 0x06001057 RID: 4183 RVA: 0x00057326 File Offset: 0x00055526
	[JsonIgnore]
	public bool IsMovementQuest
	{
		get
		{
			return this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance;
		}
	}

	// Token: 0x1700019A RID: 410
	// (get) Token: 0x06001058 RID: 4184 RVA: 0x0005733D File Offset: 0x0005553D
	// (set) Token: 0x06001059 RID: 4185 RVA: 0x00057345 File Offset: 0x00055545
	[JsonIgnore]
	public GTZone RequiredZone { get; private set; } = GTZone.none;

	// Token: 0x0600105A RID: 4186 RVA: 0x0005734E File Offset: 0x0005554E
	public void SetRequiredZone()
	{
		this.RequiredZone = ((this.requiredZones.Count > 0) ? this.requiredZones[Random.Range(0, this.requiredZones.Count)] : GTZone.none);
	}

	// Token: 0x0600105B RID: 4187 RVA: 0x00057384 File Offset: 0x00055584
	public void AddEventListener()
	{
		if (this.isQuestComplete)
		{
			return;
		}
		switch (this.questType)
		{
		case QuestType.gameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger += this.OnGameEventOccurence;
			return;
		case QuestType.gameModeRound:
			PlayerGameEvents.OnGameModeCompleteRound += this.OnGameEventOccurence;
			return;
		case QuestType.grabObject:
			PlayerGameEvents.OnGrabbedObject += this.OnGameEventOccurence;
			return;
		case QuestType.dropObject:
			PlayerGameEvents.OnDroppedObject += this.OnGameEventOccurence;
			return;
		case QuestType.eatObject:
			PlayerGameEvents.OnEatObject += this.OnGameEventOccurence;
			return;
		case QuestType.tapObject:
			PlayerGameEvents.OnTapObject += this.OnGameEventOccurence;
			return;
		case QuestType.launchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile += this.OnGameEventOccurence;
			return;
		case QuestType.moveDistance:
			PlayerGameEvents.OnPlayerMoved += this.OnGameMoveEvent;
			return;
		case QuestType.swimDistance:
			PlayerGameEvents.OnPlayerSwam += this.OnGameMoveEvent;
			return;
		case QuestType.triggerHandEffect:
			PlayerGameEvents.OnTriggerHandEffect += this.OnGameEventOccurence;
			return;
		case QuestType.enterLocation:
			PlayerGameEvents.OnEnterLocation += this.OnGameEventOccurence;
			return;
		case QuestType.misc:
			PlayerGameEvents.OnMiscEvent += this.OnGameEventOccurence;
			return;
		case QuestType.critter:
			PlayerGameEvents.OnCritterEvent += this.OnGameEventOccurence;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600105C RID: 4188 RVA: 0x000574C8 File Offset: 0x000556C8
	public void RemoveEventListener()
	{
		switch (this.questType)
		{
		case QuestType.gameModeObjective:
			PlayerGameEvents.OnGameModeObjectiveTrigger -= this.OnGameEventOccurence;
			return;
		case QuestType.gameModeRound:
			PlayerGameEvents.OnGameModeCompleteRound -= this.OnGameEventOccurence;
			return;
		case QuestType.grabObject:
			PlayerGameEvents.OnGrabbedObject -= this.OnGameEventOccurence;
			return;
		case QuestType.dropObject:
			PlayerGameEvents.OnDroppedObject -= this.OnGameEventOccurence;
			return;
		case QuestType.eatObject:
			PlayerGameEvents.OnEatObject -= this.OnGameEventOccurence;
			return;
		case QuestType.tapObject:
			PlayerGameEvents.OnTapObject -= this.OnGameEventOccurence;
			return;
		case QuestType.launchedProjectile:
			PlayerGameEvents.OnLaunchedProjectile -= this.OnGameEventOccurence;
			return;
		case QuestType.moveDistance:
			PlayerGameEvents.OnPlayerMoved -= this.OnGameMoveEvent;
			return;
		case QuestType.swimDistance:
			PlayerGameEvents.OnPlayerSwam -= this.OnGameMoveEvent;
			return;
		case QuestType.triggerHandEffect:
			PlayerGameEvents.OnTriggerHandEffect -= this.OnGameEventOccurence;
			return;
		case QuestType.enterLocation:
			PlayerGameEvents.OnEnterLocation -= this.OnGameEventOccurence;
			return;
		case QuestType.misc:
			PlayerGameEvents.OnMiscEvent -= this.OnGameEventOccurence;
			return;
		case QuestType.critter:
			PlayerGameEvents.OnCritterEvent -= this.OnGameEventOccurence;
			return;
		default:
			return;
		}
	}

	// Token: 0x0600105D RID: 4189 RVA: 0x00057604 File Offset: 0x00055804
	public void ApplySavedProgress(int progress)
	{
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			this.moveDistance = (float)progress;
			this.occurenceCount = Mathf.FloorToInt(this.moveDistance);
			this.isQuestComplete = (this.occurenceCount >= this.requiredOccurenceCount);
			return;
		}
		this.occurenceCount = progress;
		this.isQuestComplete = (this.occurenceCount >= this.requiredOccurenceCount);
	}

	// Token: 0x0600105E RID: 4190 RVA: 0x00057673 File Offset: 0x00055873
	public int GetProgress()
	{
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			return Mathf.FloorToInt(this.moveDistance);
		}
		return this.occurenceCount;
	}

	// Token: 0x0600105F RID: 4191 RVA: 0x0005769A File Offset: 0x0005589A
	private void OnGameEventOccurence(string eventName)
	{
		this.OnGameEventOccurence(eventName, 1);
	}

	// Token: 0x06001060 RID: 4192 RVA: 0x000576A4 File Offset: 0x000558A4
	private void OnGameEventOccurence(string eventName, int count)
	{
		if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
		{
			return;
		}
		string.IsNullOrEmpty(this.questOccurenceFilter);
		if (eventName.StartsWith(this.questOccurenceFilter))
		{
			this.SetProgress(this.occurenceCount + count);
		}
	}

	// Token: 0x06001061 RID: 4193 RVA: 0x000576F4 File Offset: 0x000558F4
	private void OnGameMoveEvent(float distance, float speed)
	{
		if (this.RequiredZone != GTZone.none && !ZoneManagement.IsInZone(this.RequiredZone))
		{
			return;
		}
		if (!(this.questOccurenceFilter == "maxSpeed"))
		{
			this.moveDistance += distance;
			this.SetProgress(Mathf.FloorToInt(this.moveDistance));
			return;
		}
		if (speed <= this.moveDistance)
		{
			return;
		}
		this.moveDistance = speed;
		this.SetProgress(Mathf.FloorToInt(this.moveDistance));
	}

	// Token: 0x06001062 RID: 4194 RVA: 0x00057770 File Offset: 0x00055970
	private void SetProgress(int progress)
	{
		if (this.isQuestComplete)
		{
			return;
		}
		if (this.occurenceCount == progress)
		{
			return;
		}
		this.lastChange = Time.frameCount;
		this.occurenceCount = progress;
		if (this.questType == QuestType.moveDistance || this.questType == QuestType.swimDistance)
		{
			this.moveDistance = (float)progress;
		}
		if (this.occurenceCount >= this.requiredOccurenceCount)
		{
			this.Complete();
		}
		this.questManager.HandleQuestProgressChanged(false);
	}

	// Token: 0x06001063 RID: 4195 RVA: 0x000577DD File Offset: 0x000559DD
	private void Complete()
	{
		if (this.isQuestComplete)
		{
			return;
		}
		this.isQuestComplete = true;
		this.RemoveEventListener();
		this.questManager.HandleQuestCompleted(this.questID);
	}

	// Token: 0x06001064 RID: 4196 RVA: 0x00057806 File Offset: 0x00055A06
	public string GetTextDescription()
	{
		return this.<GetTextDescription>g__GetActionName|32_0().ToUpper() + this.<GetTextDescription>g__GetLocationText|32_1().ToUpper();
	}

	// Token: 0x06001065 RID: 4197 RVA: 0x00057823 File Offset: 0x00055A23
	public string GetProgressText()
	{
		if (!this.isQuestComplete)
		{
			return string.Format("{0}/{1}", this.occurenceCount, this.requiredOccurenceCount);
		}
		return "[DONE]";
	}

	// Token: 0x06001067 RID: 4199 RVA: 0x00057880 File Offset: 0x00055A80
	[CompilerGenerated]
	private string <GetTextDescription>g__GetActionName|32_0()
	{
		switch (this.questType)
		{
		case QuestType.none:
			return "[UNDEFINED]";
		case QuestType.gameModeObjective:
			return this.questName;
		case QuestType.gameModeRound:
			return this.questName;
		case QuestType.grabObject:
			return this.questName;
		case QuestType.dropObject:
			return this.questName;
		case QuestType.eatObject:
			return this.questName;
		case QuestType.launchedProjectile:
			return this.questName;
		case QuestType.moveDistance:
			return this.questName;
		case QuestType.swimDistance:
			return this.questName;
		case QuestType.triggerHandEffect:
			return this.questName;
		case QuestType.enterLocation:
			return this.questName;
		case QuestType.misc:
			return this.questName;
		}
		return this.questName;
	}

	// Token: 0x06001068 RID: 4200 RVA: 0x00057943 File Offset: 0x00055B43
	[CompilerGenerated]
	private string <GetTextDescription>g__GetLocationText|32_1()
	{
		if (this.RequiredZone == GTZone.none)
		{
			return "";
		}
		return string.Format(" IN {0}", this.RequiredZone);
	}

	// Token: 0x0400138E RID: 5006
	public bool disable;

	// Token: 0x0400138F RID: 5007
	public int questID;

	// Token: 0x04001390 RID: 5008
	public float weight = 1f;

	// Token: 0x04001391 RID: 5009
	public QuestCategory category;

	// Token: 0x04001392 RID: 5010
	public string questName = "UNNAMED QUEST";

	// Token: 0x04001393 RID: 5011
	public QuestType questType;

	// Token: 0x04001394 RID: 5012
	public string questOccurenceFilter;

	// Token: 0x04001395 RID: 5013
	public int requiredOccurenceCount = 1;

	// Token: 0x04001396 RID: 5014
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public List<GTZone> requiredZones;

	// Token: 0x04001397 RID: 5015
	[Space]
	[NonSerialized]
	public bool isQuestActive;

	// Token: 0x04001398 RID: 5016
	[NonSerialized]
	public bool isQuestComplete;

	// Token: 0x04001399 RID: 5017
	[NonSerialized]
	public bool isDailyQuest;

	// Token: 0x0400139A RID: 5018
	[NonSerialized]
	public int lastChange;

	// Token: 0x0400139C RID: 5020
	[NonSerialized]
	public int occurenceCount;

	// Token: 0x0400139D RID: 5021
	private float moveDistance;

	// Token: 0x0400139E RID: 5022
	[NonSerialized]
	public GorillaQuestManager questManager;
}
