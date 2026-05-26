using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaTag;
using Photon.Pun;
using TMPro;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class SIQuestBoard : MonoBehaviour, IGorillaSliceableSimple
{
	// Token: 0x060008C7 RID: 2247 RVA: 0x0002FDF8 File Offset: 0x0002DFF8
	public void WriteDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			stream.SendNext(this.questDisplays[i].activePlayerActorNumber);
		}
	}

	// Token: 0x060008C8 RID: 2248 RVA: 0x0002FE38 File Offset: 0x0002E038
	public void ReadDataPUN(PhotonStream stream, PhotonMessageInfo info)
	{
		for (int i = 0; i < this.questDisplays.Count; i++)
		{
			this.questDisplays[i].activePlayerActorNumber = (int)stream.ReceiveNext();
		}
	}

	// Token: 0x060008C9 RID: 2249 RVA: 0x0002FE77 File Offset: 0x0002E077
	public void GrantBonusPointProgress()
	{
		if (!this.bounds.Contains(GTPlayer.Instance.HeadCenterPosition))
		{
			return;
		}
		SIPlayer.LocalPlayer.GetBonusProgress(this.superInfection.siManager);
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x0002FEA8 File Offset: 0x0002E0A8
	void IGorillaSliceableSimple.SliceUpdate()
	{
		if (this.superInfection.siManager.gameEntityManager.IsAuthority())
		{
			this.AuthorityUpdateScreenAssignments();
		}
		DateTime utcNow = DateTime.UtcNow;
		DateTime dateTime = utcNow.Date + SIProgression.Instance.CROSSOVER_TIME_OF_DAY;
		if (dateTime < utcNow)
		{
			dateTime = dateTime.AddDays(1.0);
		}
		TimeSpan timeSpan = dateTime - utcNow;
		GTTime.TryUpdateTimeText(this.timeToNewQuests, timeSpan, SIQuestBoard._timeToNewQuests_chars, 15, ref SIQuestBoard._lastTotalSeconds);
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x0002FF2C File Offset: 0x0002E12C
	private void AuthorityUpdateScreenAssignments()
	{
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		NetPlayer[] allNetPlayers = NetworkSystem.Instance.AllNetPlayers;
		for (int i = 0; i < allNetPlayers.Length; i++)
		{
			list.Add(allNetPlayers[i].ActorNumber);
		}
		for (int j = 0; j < this.questDisplays.Count; j++)
		{
			int activePlayerActorNumber = this.questDisplays[j].activePlayerActorNumber;
			if (activePlayerActorNumber != -1)
			{
				if (!list.Contains(activePlayerActorNumber))
				{
					this.questDisplays[j].activePlayerActorNumber = -1;
				}
				else if (!list2.Contains(activePlayerActorNumber))
				{
					list2.Add(activePlayerActorNumber);
				}
			}
		}
		for (int k = 0; k < allNetPlayers.Length; k++)
		{
			int actorNumber = allNetPlayers[k].ActorNumber;
			if (!list2.Contains(actorNumber))
			{
				for (int l = 0; l < this.questDisplays.Count; l++)
				{
					if (this.questDisplays[l].activePlayerActorNumber == -1)
					{
						this.questDisplays[l].activePlayerActorNumber = actorNumber;
						break;
					}
				}
			}
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x0003003C File Offset: 0x0002E23C
	public void OnEnable()
	{
		GorillaSlicerSimpleManager.RegisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
		if (this.bonusPointArea.gameObject.activeSelf)
		{
			this.bounds = this.bonusPointArea.bounds;
			this.bonusPointArea.gameObject.SetActive(false);
		}
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00018E11 File Offset: 0x00017011
	public void OnDisable()
	{
		GorillaSlicerSimpleManager.UnregisterSliceable(this, GorillaSlicerSimpleManager.UpdateStep.Update);
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void ForceCompleteQuest(int index)
	{
	}

	// Token: 0x060008CF RID: 2255 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void CheatAddPoints(int points)
	{
	}

	// Token: 0x060008D0 RID: 2256 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void CheatAddBonusPoints(int points)
	{
	}

	// Token: 0x060008D1 RID: 2257 RVA: 0x0003007C File Offset: 0x0002E27C
	public void CheatRoomFXDurationPlus()
	{
		if (this.currentDuration < SIQuestBoard.RoomFXDurationState._120seconds)
		{
			this.currentDuration++;
		}
		this.RoomFXDurationReadout.text = string.Format("{0}secs", this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D2 RID: 2258 RVA: 0x000300CC File Offset: 0x0002E2CC
	public void CheatRoomFXDurationMinus()
	{
		if (this.currentDuration > SIQuestBoard.RoomFXDurationState._15seconds)
		{
			this.currentDuration--;
		}
		this.RoomFXDurationReadout.text = string.Format("{0}secs", this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D3 RID: 2259 RVA: 0x0003011B File Offset: 0x0002E31B
	public void CheatRoomFX_Underwater()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.Underwater, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D4 RID: 2260 RVA: 0x00030135 File Offset: 0x0002E335
	public void CheatRoomFX_LunarMode()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.LunarMode, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D5 RID: 2261 RVA: 0x0003014F File Offset: 0x0002E34F
	public void CheatRoomFX_ConstLowG()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.ConstLowG, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D6 RID: 2262 RVA: 0x00030169 File Offset: 0x0002E369
	public void CheatRoomFX_Bouncy()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.Bouncy, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D7 RID: 2263 RVA: 0x00030183 File Offset: 0x0002E383
	public void CheatRoomFX_Supercharge()
	{
		this.StartRoomFX(SuperInfectionManager.RoomFXType.Supercharge, this.roomFXDurations[this.currentDuration]);
	}

	// Token: 0x060008D8 RID: 2264 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void StartRoomFX(SuperInfectionManager.RoomFXType fxType, float duration)
	{
	}

	// Token: 0x04000AE2 RID: 2786
	public SuperInfection superInfection;

	// Token: 0x04000AE3 RID: 2787
	public List<SIUIPlayerQuestDisplay> questDisplays;

	// Token: 0x04000AE4 RID: 2788
	public BoxCollider bonusPointArea;

	// Token: 0x04000AE5 RID: 2789
	public Bounds bounds;

	// Token: 0x04000AE6 RID: 2790
	public ParticleSystem celebrateParticle;

	// Token: 0x04000AE7 RID: 2791
	public TextMeshProUGUI timeToNewQuests;

	// Token: 0x04000AE8 RID: 2792
	private static readonly char[] _timeToNewQuests_chars = "NEW QUESTS IN: ??:??:??".ToCharArray();

	// Token: 0x04000AE9 RID: 2793
	private const int _timeToNewQuests_index = 15;

	// Token: 0x04000AEA RID: 2794
	private static int _lastTotalSeconds;

	// Token: 0x04000AEB RID: 2795
	private Dictionary<SIQuestBoard.RoomFXDurationState, float> roomFXDurations = new Dictionary<SIQuestBoard.RoomFXDurationState, float>
	{
		{
			SIQuestBoard.RoomFXDurationState._15seconds,
			15f
		},
		{
			SIQuestBoard.RoomFXDurationState._30seconds,
			30f
		},
		{
			SIQuestBoard.RoomFXDurationState._60seconds,
			60f
		},
		{
			SIQuestBoard.RoomFXDurationState._90seconds,
			90f
		},
		{
			SIQuestBoard.RoomFXDurationState._120seconds,
			120f
		}
	};

	// Token: 0x04000AEC RID: 2796
	private SIQuestBoard.RoomFXDurationState currentDuration = SIQuestBoard.RoomFXDurationState._30seconds;

	// Token: 0x04000AED RID: 2797
	[SerializeField]
	private TextMeshPro RoomFXDurationReadout;

	// Token: 0x0200014F RID: 335
	private enum RoomFXDurationState
	{
		// Token: 0x04000AEF RID: 2799
		_15seconds,
		// Token: 0x04000AF0 RID: 2800
		_30seconds,
		// Token: 0x04000AF1 RID: 2801
		_60seconds,
		// Token: 0x04000AF2 RID: 2802
		_90seconds,
		// Token: 0x04000AF3 RID: 2803
		_120seconds
	}
}
