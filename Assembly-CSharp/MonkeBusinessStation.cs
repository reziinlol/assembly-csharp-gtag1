using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameObjectScheduling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000249 RID: 585
public class MonkeBusinessStation : MonoBehaviour
{
	// Token: 0x06000F9D RID: 3997 RVA: 0x00054AB8 File Offset: 0x00052CB8
	private void OnEnable()
	{
		this.FindQuestManager();
		ProgressionController.OnQuestSelectionChanged += this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent += this.OnProgress;
		ProgressionController.RequestProgressUpdate();
		RoomSystem.OnMonkePointsRedeemedReceived = (Action<NetPlayer, int>)Delegate.Combine(RoomSystem.OnMonkePointsRedeemedReceived, new Action<NetPlayer, int>(this.OnRemotePointsRedeemed));
		RoomSystem.PlayerLeftEvent += new Action<NetPlayer>(this.OnPlayerLeftRoom);
		this.UpdateCountdownTimers();
	}

	// Token: 0x06000F9E RID: 3998 RVA: 0x00054B34 File Offset: 0x00052D34
	private void OnDisable()
	{
		ProgressionController.OnQuestSelectionChanged -= this.OnQuestSelectionChanged;
		ProgressionController.OnProgressEvent -= this.OnProgress;
		RoomSystem.OnMonkePointsRedeemedReceived = (Action<NetPlayer, int>)Delegate.Remove(RoomSystem.OnMonkePointsRedeemedReceived, new Action<NetPlayer, int>(this.OnRemotePointsRedeemed));
		RoomSystem.PlayerLeftEvent -= new Action<NetPlayer>(this.OnPlayerLeftRoom);
	}

	// Token: 0x06000F9F RID: 3999 RVA: 0x00054B9E File Offset: 0x00052D9E
	private void FindQuestManager()
	{
		if (!this._questManager)
		{
			this._questManager = Object.FindAnyObjectByType<RotatingQuestsManager>();
		}
	}

	// Token: 0x06000FA0 RID: 4000 RVA: 0x00054BB8 File Offset: 0x00052DB8
	private void UpdateCountdownTimers()
	{
		this._dailyCountdown.SetCountdownTime(this._questManager.DailyQuestCountdown);
		this._weeklyCountdown.SetCountdownTime(this._questManager.WeeklyQuestCountdown);
	}

	// Token: 0x06000FA1 RID: 4001 RVA: 0x00054BE6 File Offset: 0x00052DE6
	private void OnQuestSelectionChanged()
	{
		this.UpdateCountdownTimers();
	}

	// Token: 0x06000FA2 RID: 4002 RVA: 0x00054BEE File Offset: 0x00052DEE
	private void OnProgress()
	{
		this.UpdateQuestStatus();
		this.UpdateProgressDisplays();
	}

	// Token: 0x06000FA3 RID: 4003 RVA: 0x00054BFC File Offset: 0x00052DFC
	private void UpdateProgressDisplays()
	{
		ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
		int item = progressionData.Item1;
		int item2 = progressionData.Item2;
		this._weeklyProgress.SetProgress(item, ProgressionController.WeeklyCap);
		if (!this._isUpdatingPointCount)
		{
			this._unclaimedPoints.text = item2.ToString();
			this._claimButton.isOn = (item2 > 0);
		}
		bool flag = item2 > 0;
		this._claimablePointsObject.SetActive(flag);
		this._noClaimablePointsObject.SetActive(!flag);
		this._badgeMount.position = (flag ? this._claimablePointsBadgePosition.position : this._noClaimablePointsBadgePosition.position);
		this._claimButton.gameObject.SetActive(flag);
	}

	// Token: 0x06000FA4 RID: 4004 RVA: 0x00054CB0 File Offset: 0x00052EB0
	private void UpdateQuestStatus()
	{
		if (this._lastQuestChange >= RotatingQuestsManager.LastQuestChange)
		{
			return;
		}
		this.FindQuestManager();
		if (this._quests.Count == 0 || this._lastQuestDailyID != RotatingQuestsManager.LastQuestDailyID)
		{
			this.BuildQuestList();
		}
		foreach (QuestDisplay questDisplay in this._quests)
		{
			if (questDisplay.IsChanged)
			{
				questDisplay.UpdateDisplay();
			}
		}
		this._lastQuestChange = Time.frameCount;
		this._lastQuestDailyID = RotatingQuestsManager.LastQuestDailyID;
	}

	// Token: 0x06000FA5 RID: 4005 RVA: 0x00054D54 File Offset: 0x00052F54
	public void RedeemProgress()
	{
		if (this._claimButton.isOn)
		{
			this._isUpdatingPointCount = true;
			ValueTuple<int, int, int> progressionData = ProgressionController.GetProgressionData();
			int item = progressionData.Item2;
			int item2 = progressionData.Item3;
			this._tempUnclaimedPoints = item;
			this._tempTotalPoints = item2;
			this._claimButton.isOn = false;
			ProgressionController.RedeemProgress();
			RoomSystem.SendMonkePointsRedeemed(this._tempUnclaimedPoints);
			base.StartCoroutine(this.PerformPointRedemptionSequence());
		}
	}

	// Token: 0x06000FA6 RID: 4006 RVA: 0x00054DBE File Offset: 0x00052FBE
	private IEnumerator PerformPointRedemptionSequence()
	{
		while (this._tempUnclaimedPoints > 0)
		{
			this._tempUnclaimedPoints--;
			this._tempTotalPoints++;
			this._unclaimedPoints.text = this._tempUnclaimedPoints.ToString();
			if (this._tempUnclaimedPoints == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this._isUpdatingPointCount = false;
		this.UpdateProgressDisplays();
		yield break;
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x00054DD0 File Offset: 0x00052FD0
	private void OnRemotePointsRedeemed(NetPlayer sender, int redeemedPointCount)
	{
		if (sender == null)
		{
			return;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(sender, out rigContainer))
		{
			return;
		}
		if (!FXSystem.CheckCallSpam(rigContainer.Rig.fxSettings, 10, (double)Time.unscaledTime))
		{
			return;
		}
		Coroutine coroutine;
		if (this.perPlayerRedemptionSequence.TryGetValue(sender, out coroutine))
		{
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
			}
			this.perPlayerRedemptionSequence.Remove(sender);
		}
		if (base.gameObject.activeInHierarchy)
		{
			Coroutine value = base.StartCoroutine(this.PerformRemotePointRedemptionSequence(sender, redeemedPointCount));
			this.perPlayerRedemptionSequence.Add(sender, value);
		}
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x00054E5C File Offset: 0x0005305C
	private void OnPlayerLeftRoom(NetPlayer player)
	{
		if (player == null)
		{
			return;
		}
		Coroutine coroutine;
		if (this.perPlayerRedemptionSequence.TryGetValue(player, out coroutine))
		{
			if (coroutine != null)
			{
				base.StopCoroutine(coroutine);
			}
			this.perPlayerRedemptionSequence.Remove(player);
		}
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x00054E94 File Offset: 0x00053094
	private IEnumerator PerformRemotePointRedemptionSequence(NetPlayer player, int redeemedPointCount)
	{
		while (redeemedPointCount > 0)
		{
			int num = redeemedPointCount;
			redeemedPointCount = num - 1;
			if (redeemedPointCount == 0)
			{
				this._audioSource.PlayOneShot(this._claimPointFinalSFX);
			}
			else
			{
				this._audioSource.PlayOneShot(this._claimPointDefaultSFX);
			}
			yield return new WaitForSeconds(this._claimDelayPerPoint);
		}
		this.perPlayerRedemptionSequence.Remove(player);
		yield break;
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00054EB4 File Offset: 0x000530B4
	private void BuildQuestList()
	{
		this.DestroyQuestList();
		RotatingQuestsManager.RotatingQuestList quests = this._questManager.quests;
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup in quests.DailyQuests)
		{
			foreach (RotatingQuest rotatingQuest in rotatingQuestGroup.quests)
			{
				if (rotatingQuest.isQuestActive)
				{
					QuestDisplay questDisplay = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._dailyQuestContainer);
					questDisplay.quest = rotatingQuest;
					this._quests.Add(questDisplay);
				}
			}
		}
		foreach (RotatingQuestsManager.RotatingQuestGroup rotatingQuestGroup2 in quests.WeeklyQuests)
		{
			foreach (RotatingQuest rotatingQuest2 in rotatingQuestGroup2.quests)
			{
				if (rotatingQuest2.isQuestActive)
				{
					QuestDisplay questDisplay2 = Object.Instantiate<QuestDisplay>(this._questDisplayPrefab, this._weeklyQuestContainer);
					questDisplay2.quest = rotatingQuest2;
					this._quests.Add(questDisplay2);
				}
			}
		}
		foreach (QuestDisplay questDisplay3 in this._quests)
		{
			questDisplay3.UpdateDisplay();
		}
		if (!this._hasBuiltQuestList)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(this._questContainerParent);
			this._hasBuiltQuestList = true;
			return;
		}
		LayoutRebuilder.MarkLayoutForRebuild(this._questContainerParent);
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x00055088 File Offset: 0x00053288
	private void DestroyQuestList()
	{
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|41_0(this._dailyQuestContainer);
		MonkeBusinessStation.<DestroyQuestList>g__DestroyChildren|41_0(this._weeklyQuestContainer);
		this._quests.Clear();
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x000550D8 File Offset: 0x000532D8
	[CompilerGenerated]
	internal static void <DestroyQuestList>g__DestroyChildren|41_0(Transform parent)
	{
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Object.Destroy(parent.GetChild(i).gameObject);
		}
	}

	// Token: 0x040012D1 RID: 4817
	[SerializeField]
	private RectTransform _questContainerParent;

	// Token: 0x040012D2 RID: 4818
	[SerializeField]
	private RectTransform _dailyQuestContainer;

	// Token: 0x040012D3 RID: 4819
	[SerializeField]
	private RectTransform _weeklyQuestContainer;

	// Token: 0x040012D4 RID: 4820
	[SerializeField]
	private QuestDisplay _questDisplayPrefab;

	// Token: 0x040012D5 RID: 4821
	[SerializeField]
	private List<QuestDisplay> _quests;

	// Token: 0x040012D6 RID: 4822
	[SerializeField]
	private ProgressDisplay _weeklyProgress;

	// Token: 0x040012D7 RID: 4823
	[SerializeField]
	private TMP_Text _unclaimedPoints;

	// Token: 0x040012D8 RID: 4824
	[SerializeField]
	private GorillaPressableButton _claimButton;

	// Token: 0x040012D9 RID: 4825
	[SerializeField]
	private AudioSource _audioSource;

	// Token: 0x040012DA RID: 4826
	[SerializeField]
	private GameObject _claimablePointsObject;

	// Token: 0x040012DB RID: 4827
	[SerializeField]
	private GameObject _noClaimablePointsObject;

	// Token: 0x040012DC RID: 4828
	[SerializeField]
	private Transform _claimablePointsBadgePosition;

	// Token: 0x040012DD RID: 4829
	[SerializeField]
	private Transform _noClaimablePointsBadgePosition;

	// Token: 0x040012DE RID: 4830
	[SerializeField]
	private Transform _badgeMount;

	// Token: 0x040012DF RID: 4831
	[Space]
	[SerializeField]
	private float _claimDelayPerPoint = 0.12f;

	// Token: 0x040012E0 RID: 4832
	[SerializeField]
	private AudioClip _claimPointDefaultSFX;

	// Token: 0x040012E1 RID: 4833
	[SerializeField]
	private AudioClip _claimPointFinalSFX;

	// Token: 0x040012E2 RID: 4834
	[Header("Quest Timers")]
	[SerializeField]
	private CountdownText _dailyCountdown;

	// Token: 0x040012E3 RID: 4835
	[SerializeField]
	private CountdownText _weeklyCountdown;

	// Token: 0x040012E4 RID: 4836
	private RotatingQuestsManager _questManager;

	// Token: 0x040012E5 RID: 4837
	private int _lastQuestChange = -1;

	// Token: 0x040012E6 RID: 4838
	private int _lastQuestDailyID = -1;

	// Token: 0x040012E7 RID: 4839
	private bool _isUpdatingPointCount;

	// Token: 0x040012E8 RID: 4840
	private int _tempUnclaimedPoints;

	// Token: 0x040012E9 RID: 4841
	private int _tempTotalPoints;

	// Token: 0x040012EA RID: 4842
	private bool _hasBuiltQuestList;

	// Token: 0x040012EB RID: 4843
	private Dictionary<NetPlayer, Coroutine> perPlayerRedemptionSequence = new Dictionary<NetPlayer, Coroutine>();
}
