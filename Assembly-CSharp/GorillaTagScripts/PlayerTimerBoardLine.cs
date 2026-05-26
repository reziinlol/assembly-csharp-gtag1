using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTagScripts
{
	// Token: 0x02000F1F RID: 3871
	public class PlayerTimerBoardLine : MonoBehaviour
	{
		// Token: 0x060060AD RID: 24749 RVA: 0x001F2367 File Offset: 0x001F0567
		public void ResetData()
		{
			this.linePlayer = null;
			this.currentNickname = string.Empty;
			this.playerTimeStr = string.Empty;
			this.playerTimeSeconds = 0f;
		}

		// Token: 0x060060AE RID: 24750 RVA: 0x001F2394 File Offset: 0x001F0594
		public void SetLineData(NetPlayer netPlayer)
		{
			if (!netPlayer.InRoom || netPlayer == this.linePlayer)
			{
				return;
			}
			this.linePlayer = netPlayer;
			RigContainer rigContainer;
			if (VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
			{
				this.rigContainer = rigContainer;
				this.playerVRRig = rigContainer.Rig;
			}
			this.InitializeLine();
		}

		// Token: 0x060060AF RID: 24751 RVA: 0x001F23E2 File Offset: 0x001F05E2
		public void InitializeLine()
		{
			this.currentNickname = string.Empty;
			this.UpdatePlayerText();
			this.UpdateTimeText();
		}

		// Token: 0x060060B0 RID: 24752 RVA: 0x001F23FC File Offset: 0x001F05FC
		public void UpdateLine()
		{
			if (this.linePlayer != null)
			{
				if (this.playerNameVisible != this.playerVRRig.playerNameVisible)
				{
					this.UpdatePlayerText();
					this.parentBoard.IsDirty = true;
				}
				string value = this.playerTimeStr;
				this.UpdateTimeText();
				if (!this.playerTimeStr.Equals(value))
				{
					this.parentBoard.IsDirty = true;
				}
			}
		}

		// Token: 0x060060B1 RID: 24753 RVA: 0x001F2464 File Offset: 0x001F0664
		private void UpdatePlayerText()
		{
			try
			{
				if (this.rigContainer.IsNull() || this.playerVRRig.IsNull())
				{
					this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
					this.currentNickname = this.linePlayer.NickName;
				}
				else if (this.rigContainer.Initialized)
				{
					this.playerNameVisible = this.playerVRRig.playerNameVisible;
				}
				else if (this.currentNickname.IsNullOrEmpty() || GorillaComputer.instance.friendJoinCollider.playerIDsCurrentlyTouching.Contains(this.linePlayer.UserId))
				{
					this.playerNameVisible = this.NormalizeName(this.linePlayer.NickName != this.currentNickname, this.linePlayer.NickName);
				}
			}
			catch (Exception)
			{
				this.playerNameVisible = this.linePlayer.DefaultName;
				MonkeAgent.instance.SendReport("NmError", this.linePlayer.UserId, this.linePlayer.NickName);
			}
		}

		// Token: 0x060060B2 RID: 24754 RVA: 0x001F2598 File Offset: 0x001F0798
		private void UpdateTimeText()
		{
			if (this.linePlayer == null || !(PlayerTimerManager.instance != null))
			{
				this.playerTimeStr = "--:--:--";
				return;
			}
			this.playerTimeSeconds = PlayerTimerManager.instance.GetLastDurationForPlayer(this.linePlayer.ActorNumber);
			if (this.playerTimeSeconds > 0f)
			{
				this.playerTimeStr = TimeSpan.FromSeconds((double)this.playerTimeSeconds).ToString("mm\\:ss\\:ff");
				return;
			}
			this.playerTimeStr = "--:--:--";
		}

		// Token: 0x060060B3 RID: 24755 RVA: 0x001F261C File Offset: 0x001F081C
		public string NormalizeName(bool doIt, string text)
		{
			if (doIt)
			{
				if (GorillaComputer.instance.CheckAutoBanListForName(text))
				{
					text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
					if (text.Length > 12)
					{
						text = text.Substring(0, 12);
					}
					text = text.ToUpper();
				}
				else
				{
					text = "BADGORILLA";
					MonkeAgent.instance.SendReport("evading the name ban", this.linePlayer.UserId, this.linePlayer.NickName);
				}
			}
			return text;
		}

		// Token: 0x060060B4 RID: 24756 RVA: 0x001F26C0 File Offset: 0x001F08C0
		public static int CompareByTotalTime(PlayerTimerBoardLine lineA, PlayerTimerBoardLine lineB)
		{
			if (lineA.playerTimeSeconds > 0f && lineB.playerTimeSeconds > 0f)
			{
				return lineA.playerTimeSeconds.CompareTo(lineB.playerTimeSeconds);
			}
			if (lineA.playerTimeSeconds <= 0f)
			{
				return 1;
			}
			if (lineB.playerTimeSeconds <= 0f)
			{
				return -1;
			}
			return 0;
		}

		// Token: 0x04006F47 RID: 28487
		public string playerNameVisible;

		// Token: 0x04006F48 RID: 28488
		public string playerTimeStr;

		// Token: 0x04006F49 RID: 28489
		private float playerTimeSeconds;

		// Token: 0x04006F4A RID: 28490
		public NetPlayer linePlayer;

		// Token: 0x04006F4B RID: 28491
		public VRRig playerVRRig;

		// Token: 0x04006F4C RID: 28492
		public PlayerTimerBoard parentBoard;

		// Token: 0x04006F4D RID: 28493
		internal RigContainer rigContainer;

		// Token: 0x04006F4E RID: 28494
		private string currentNickname;
	}
}
