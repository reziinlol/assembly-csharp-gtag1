using System;
using System.Text;
using GorillaNetworking;
using TMPro;
using UnityEngine;

// Token: 0x0200082B RID: 2091
public class GRToolStatusWatch : MonoBehaviour, IGameEntityComponent
{
	// Token: 0x060035B4 RID: 13748 RVA: 0x00129710 File Offset: 0x00127910
	public void OnEntityInit()
	{
		if (this.gameEntity == null)
		{
			this.gameEntity = base.GetComponent<GameEntity>();
		}
		this.UpdateVisuals();
		this.progression = this.gameEntity.manager.GetComponent<GhostReactorManager>().reactor.toolProgression;
		GameEntity gameEntity = this.gameEntity;
		gameEntity.OnSnapped = (Action)Delegate.Combine(gameEntity.OnSnapped, new Action(this.UpdateSnappedPlayer));
		GameEntity gameEntity2 = this.gameEntity;
		gameEntity2.OnUnsnapped = (Action)Delegate.Combine(gameEntity2.OnUnsnapped, new Action(this.RemoveSnappedPlayer));
	}

	// Token: 0x060035B5 RID: 13749 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityDestroy()
	{
	}

	// Token: 0x060035B6 RID: 13750 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEntityStateChange(long prevState, long newState)
	{
	}

	// Token: 0x060035B7 RID: 13751 RVA: 0x001297AC File Offset: 0x001279AC
	public void UpdateSnappedPlayer()
	{
		this.currentPlayer = GRPlayer.Get(this.gameEntity.snappedByActorNumber);
		this.lastKills = -1;
		this.lastCredits = -1;
		this.lastJuice = -1;
		this.lastGrade = -1;
		if (this.currentPlayer == GRPlayer.GetLocal())
		{
			this.state = GRToolStatusWatch.WatchState.SnappedLocal;
		}
		else
		{
			this.state = GRToolStatusWatch.WatchState.SnappedRemote;
		}
		this.disabledText.text = "LEAVE ME ALONE!\n\nTHIS IS ONLY FOR MY OWNER!!!";
		this.UpdateVisuals();
	}

	// Token: 0x060035B8 RID: 13752 RVA: 0x00129823 File Offset: 0x00127A23
	public void RemoveSnappedPlayer()
	{
		this.currentPlayer = null;
		this.state = GRToolStatusWatch.WatchState.Dropped;
		this.disabledText.text = "LOW POWER\n\nPUT ME ON";
		this.UpdateVisuals();
	}

	// Token: 0x060035B9 RID: 13753 RVA: 0x00129849 File Offset: 0x00127A49
	private void Update()
	{
		if (this.currentPlayer == null)
		{
			return;
		}
		this.UpdateVisuals();
	}

	// Token: 0x060035BA RID: 13754 RVA: 0x00129860 File Offset: 0x00127A60
	private void UpdateVisuals()
	{
		bool flag = this.state == GRToolStatusWatch.WatchState.SnappedLocal || this.state == GRToolStatusWatch.WatchState.SnappedRemote;
		if (this.disabledVisuals.activeSelf == flag)
		{
			this.disabledVisuals.SetActive(!flag);
		}
		if (this.enabledVisuals.activeSelf != flag)
		{
			this.enabledVisuals.SetActive(flag);
		}
		if (this.state != GRToolStatusWatch.WatchState.SnappedLocal)
		{
			return;
		}
		if (this.visibleHP != this.currentPlayer.Hp / 100)
		{
			this.visibleHP = this.currentPlayer.Hp / 100;
			for (int i = 0; i < this.healthHearts.Length; i++)
			{
				if (this.healthHearts[i].activeSelf != i < this.visibleHP)
				{
					this.healthHearts[i].SetActive(i < this.visibleHP);
				}
			}
		}
		if (this.visibleShield != this.currentPlayer.ShieldHp / 100)
		{
			this.visibleShield = this.currentPlayer.ShieldHp / 100;
			if (this.shieldSymbol.activeSelf != this.visibleShield > 0)
			{
				this.shieldSymbol.SetActive(this.visibleShield > 0);
			}
		}
		this.gimbaledCompass.LookAt(this.homeBase, Vector3.up);
		int num = (int)this.currentPlayer.synchronizedSessionStats[5];
		int shiftCredits = this.currentPlayer.ShiftCredits;
		int numberOfResearchPoints = this.progression.GetNumberOfResearchPoints();
		ValueTuple<int, int, int, int> gradePointDetails = GhostReactorProgression.GetGradePointDetails(this.currentPlayer.CurrentProgression.redeemedPoints);
		int item = gradePointDetails.Item1;
		int item2 = gradePointDetails.Item2;
		if (num == this.lastKills && shiftCredits == this.lastCredits && numberOfResearchPoints == this.lastJuice && item2 == this.lastGrade)
		{
			return;
		}
		this.sb.Clear();
		this.sb.Append(num);
		this.sb.Append("\n\n");
		this.sb.Append(numberOfResearchPoints);
		this.sb.Append("\n\n");
		this.sb.Append(shiftCredits);
		this.sb.Append("\n\n\n");
		this.sb.Append(GhostReactorProgression.GetTitleNameFromLevel(item)[0]);
		this.sb.Append(item2);
		this.statsText.text = this.sb.ToString();
		this.lastKills = num;
		this.lastCredits = shiftCredits;
		this.lastJuice = numberOfResearchPoints;
		this.lastGrade = item2;
	}

	// Token: 0x04004658 RID: 18008
	public GameEntity gameEntity;

	// Token: 0x04004659 RID: 18009
	private GRPlayer currentPlayer;

	// Token: 0x0400465A RID: 18010
	private int visibleHP;

	// Token: 0x0400465B RID: 18011
	private int visibleShield;

	// Token: 0x0400465C RID: 18012
	public GameObject disabledVisuals;

	// Token: 0x0400465D RID: 18013
	public GameObject enabledVisuals;

	// Token: 0x0400465E RID: 18014
	public GameObject[] healthHearts;

	// Token: 0x0400465F RID: 18015
	public GameObject shieldSymbol;

	// Token: 0x04004660 RID: 18016
	public Vector3 homeBase;

	// Token: 0x04004661 RID: 18017
	public Transform gimbaledCompass;

	// Token: 0x04004662 RID: 18018
	public TextMeshPro statsText;

	// Token: 0x04004663 RID: 18019
	public TextMeshPro disabledText;

	// Token: 0x04004664 RID: 18020
	private int lastKills;

	// Token: 0x04004665 RID: 18021
	private int lastCredits;

	// Token: 0x04004666 RID: 18022
	private int lastJuice;

	// Token: 0x04004667 RID: 18023
	private int lastGrade;

	// Token: 0x04004668 RID: 18024
	private StringBuilder sb = new StringBuilder();

	// Token: 0x04004669 RID: 18025
	private GRToolStatusWatch.WatchState state;

	// Token: 0x0400466A RID: 18026
	private GRToolProgressionManager progression;

	// Token: 0x0200082C RID: 2092
	private enum WatchState
	{
		// Token: 0x0400466C RID: 18028
		Dropped,
		// Token: 0x0400466D RID: 18029
		SnappedLocal,
		// Token: 0x0400466E RID: 18030
		SnappedRemote
	}
}
