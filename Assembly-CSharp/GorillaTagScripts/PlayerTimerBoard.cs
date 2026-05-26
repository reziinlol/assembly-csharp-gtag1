using System;
using System.Collections.Generic;
using System.Text;
using KID.Model;
using TMPro;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F1E RID: 3870
	public class PlayerTimerBoard : MonoBehaviour
	{
		// Token: 0x17000928 RID: 2344
		// (get) Token: 0x060060A3 RID: 24739 RVA: 0x001F200D File Offset: 0x001F020D
		// (set) Token: 0x060060A4 RID: 24740 RVA: 0x001F2015 File Offset: 0x001F0215
		public bool IsDirty { get; set; } = true;

		// Token: 0x060060A5 RID: 24741 RVA: 0x001F201E File Offset: 0x001F021E
		private void Start()
		{
			this.TryInit();
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x001F2026 File Offset: 0x001F0226
		private void OnEnable()
		{
			this.TryInit();
			LocalisationManager.RegisterOnLanguageChanged(new Action(this.RedrawPlayerLines));
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x001F203F File Offset: 0x001F023F
		private void TryInit()
		{
			if (this.isInitialized)
			{
				return;
			}
			if (PlayerTimerManager.instance == null)
			{
				return;
			}
			PlayerTimerManager.instance.RegisterTimerBoard(this);
			this.isInitialized = true;
		}

		// Token: 0x060060A8 RID: 24744 RVA: 0x001F206A File Offset: 0x001F026A
		private void OnDisable()
		{
			if (PlayerTimerManager.instance != null)
			{
				PlayerTimerManager.instance.UnregisterTimerBoard(this);
			}
			this.isInitialized = false;
			LocalisationManager.UnregisterOnLanguageChanged(new Action(this.RedrawPlayerLines));
		}

		// Token: 0x060060A9 RID: 24745 RVA: 0x001F209C File Offset: 0x001F029C
		public void SetSleepState(bool awake)
		{
			this.playerColumn.enabled = awake;
			this.timeColumn.enabled = awake;
			if (this.linesParent != null)
			{
				this.linesParent.SetActive(awake);
			}
		}

		// Token: 0x060060AA RID: 24746 RVA: 0x001F20D0 File Offset: 0x001F02D0
		public void SortLines()
		{
			this.lines.Sort(new Comparison<PlayerTimerBoardLine>(PlayerTimerBoardLine.CompareByTotalTime));
		}

		// Token: 0x060060AB RID: 24747 RVA: 0x001F20EC File Offset: 0x001F02EC
		public void RedrawPlayerLines()
		{
			this.stringBuilder.Clear();
			this.stringBuilderTime.Clear();
			string value;
			if (!LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER", out value, "<b><color=yellow>PLAYER</color></b>"))
			{
				Debug.LogError("[LOCALIZATION::MONKE_BLOCKS::TIMER] Failed to get key for Game Mode [MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER]");
			}
			this.stringBuilder.Append("<b><color=yellow>");
			this.stringBuilder.Append(value);
			this.stringBuilder.Append("</color></b>");
			if (!LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES", out value, "<b><color=yellow>LATEST TIME</color></b>"))
			{
				Debug.LogError("[LOCALIZATION::MONKE_BLOCKS::TIMER] Failed to get key for Game Mode [MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES]");
			}
			this.stringBuilderTime.Append("<b><color=yellow>");
			this.stringBuilderTime.Append(value);
			this.stringBuilderTime.Append("</color></b>");
			this.SortLines();
			Permission permissionDataByFeature = KIDManager.GetPermissionDataByFeature(EKIDFeatures.Custom_Nametags);
			bool flag = (permissionDataByFeature.Enabled || permissionDataByFeature.ManagedBy == Permission.ManagedByEnum.PLAYER) && permissionDataByFeature.ManagedBy != Permission.ManagedByEnum.PROHIBITED;
			for (int i = 0; i < this.lines.Count; i++)
			{
				try
				{
					if (this.lines[i].gameObject.activeInHierarchy)
					{
						this.lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * i), 0f);
						if (this.lines[i].linePlayer != null && this.lines[i].linePlayer.InRoom)
						{
							this.stringBuilder.Append("\n ");
							this.stringBuilder.Append(flag ? this.lines[i].playerNameVisible : this.lines[i].linePlayer.DefaultName);
							this.stringBuilderTime.Append("\n ");
							this.stringBuilderTime.Append(this.lines[i].playerTimeStr);
						}
					}
				}
				catch
				{
				}
			}
			this.playerColumn.text = this.stringBuilder.ToString();
			this.timeColumn.text = this.stringBuilderTime.ToString();
			this.IsDirty = false;
		}

		// Token: 0x04006F3A RID: 28474
		[SerializeField]
		private GameObject linesParent;

		// Token: 0x04006F3B RID: 28475
		public List<PlayerTimerBoardLine> lines;

		// Token: 0x04006F3C RID: 28476
		public TextMeshPro notInRoomText;

		// Token: 0x04006F3D RID: 28477
		public TextMeshPro playerColumn;

		// Token: 0x04006F3E RID: 28478
		public TextMeshPro timeColumn;

		// Token: 0x04006F3F RID: 28479
		[SerializeField]
		private int startingYValue;

		// Token: 0x04006F40 RID: 28480
		[SerializeField]
		private int lineHeight;

		// Token: 0x04006F41 RID: 28481
		private StringBuilder stringBuilder = new StringBuilder(220);

		// Token: 0x04006F42 RID: 28482
		private StringBuilder stringBuilderTime = new StringBuilder(220);

		// Token: 0x04006F43 RID: 28483
		private const string MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER_KEY = "MONKE_BLOCKS_TIMER_BOARD_COLUMN_PLAYER";

		// Token: 0x04006F44 RID: 28484
		private const string MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES_KEY = "MONKE_BLOCKS_TIMER_BOARD_COLUMN_TIMES";

		// Token: 0x04006F45 RID: 28485
		private bool isInitialized;
	}
}
