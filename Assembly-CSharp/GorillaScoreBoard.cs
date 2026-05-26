using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000889 RID: 2185
public class GorillaScoreBoard : MonoBehaviour
{
	// Token: 0x17000511 RID: 1297
	// (get) Token: 0x06003911 RID: 14609 RVA: 0x00137A80 File Offset: 0x00135C80
	// (set) Token: 0x06003912 RID: 14610 RVA: 0x00137A97 File Offset: 0x00135C97
	public bool IsDirty
	{
		get
		{
			return this._isDirty || string.IsNullOrEmpty(this.initialGameMode);
		}
		set
		{
			this._isDirty = value;
		}
	}

	// Token: 0x06003913 RID: 14611 RVA: 0x00137AA0 File Offset: 0x00135CA0
	public void SetSleepState(bool awake)
	{
		this.boardText.enabled = awake;
		this.buttonText.enabled = awake;
		if (this.linesParent != null)
		{
			this.linesParent.SetActive(awake);
		}
	}

	// Token: 0x06003914 RID: 14612 RVA: 0x00137AD4 File Offset: 0x00135CD4
	private string GetBeginningString()
	{
		string text = string.Format(" ({0})", 10);
		if (NetworkSystem.Instance.SessionIsSubscription)
		{
			text = string.Format(" ({0})", 20);
		}
		return string.Concat(new string[]
		{
			"ROOM ID: ",
			NetworkSystem.Instance.SessionIsPrivate ? "-PRIVATE- GAME: " : (NetworkSystem.Instance.RoomName + "   GAME: "),
			this.RoomType(),
			text,
			"\n  PLAYER     COLOR  MUTE   REPORT"
		});
	}

	// Token: 0x06003915 RID: 14613 RVA: 0x00137B64 File Offset: 0x00135D64
	private string RoomType()
	{
		this.initialGameMode = RoomSystem.RoomGameMode;
		this.gmNames = GameMode.gameModeNames;
		this.gmName = "ERROR";
		int count = this.gmNames.Count;
		int num = this.initialGameMode.LastIndexOf('|');
		if (num >= 0)
		{
			this.tempGmName = this.initialGameMode.Substring(num + 1);
			for (int i = 0; i < count; i++)
			{
				if (this.tempGmName == this.gmNames[i])
				{
					this.gmName = this.tempGmName;
					break;
				}
			}
		}
		else
		{
			for (int j = 0; j < count; j++)
			{
				this.tempGmName = this.gmNames[j];
				if (this.initialGameMode.Contains(this.tempGmName))
				{
					this.gmName = this.tempGmName;
					break;
				}
			}
		}
		return this.gmName;
	}

	// Token: 0x06003916 RID: 14614 RVA: 0x00137C40 File Offset: 0x00135E40
	public void RedrawPlayerLines()
	{
		this.stringBuilder.Clear();
		this.stringBuilder.Append(this.GetBeginningString());
		this.buttonStringBuilder.Clear();
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		int num = 0;
		for (int i = 0; i < this.lines.Count; i++)
		{
			if (this.lines[i].gameObject.activeInHierarchy)
			{
				num++;
			}
		}
		if (num > 10)
		{
			this.linesParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
			this.linesParent.transform.localPosition = new Vector3(0f, this.bigRoomYOffset, 0f);
			this.textsParent.transform.localScale = new Vector3(1f, 0.5f, 1f);
		}
		else
		{
			this.linesParent.transform.localScale = Vector3.one;
			this.linesParent.transform.localPosition = Vector3.zero;
			this.textsParent.transform.localScale = Vector3.one;
		}
		for (int j = 0; j < this.lines.Count; j++)
		{
			try
			{
				if (this.lines[j].gameObject.activeInHierarchy)
				{
					this.linesRTs[j].localPosition = new Vector3(0f, (float)(this.startingYValue - this.lineHeight * j), 0f);
					if (this.lines[j].linePlayer != null && this.lines[j].linePlayer.InRoom)
					{
						this.stringBuilder.Append("\n ");
						SubscriptionManager.SubscriptionDetails subscriptionDetails = SubscriptionManager.GetSubscriptionDetails(this.lines[j].linePlayer);
						if (subscriptionDetails.active && subscriptionDetails.tier > 0)
						{
							this.stringBuilder.Append("<color=#ffc600>");
						}
						else
						{
							this.stringBuilder.Append("<color=#ffffff>");
						}
						this.stringBuilder.Append(flag ? this.lines[j].playerNameVisible : this.lines[j].linePlayer.DefaultName);
						this.stringBuilder.Append("</color>");
						if (this.lines[j].linePlayer != NetworkSystem.Instance.LocalPlayer)
						{
							if (this.lines[j].reportButton.isActiveAndEnabled)
							{
								this.buttonStringBuilder.Append("MUTE                                REPORT\n");
							}
							else
							{
								this.buttonStringBuilder.Append("MUTE                HATE SPEECH    TOXICITY     CHEATING       CANCEL\n");
							}
						}
						else
						{
							this.buttonStringBuilder.Append("\n");
						}
					}
				}
			}
			catch
			{
			}
		}
		this.boardText.text = this.stringBuilder.ToString();
		this.buttonText.text = this.buttonStringBuilder.ToString();
		this._isDirty = false;
	}

	// Token: 0x06003917 RID: 14615 RVA: 0x00137F6C File Offset: 0x0013616C
	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => Utils.IsASCIILetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text.Substring(0, 12);
			}
			text = text.ToUpper();
		}
		return text;
	}

	// Token: 0x06003918 RID: 14616 RVA: 0x00137FCC File Offset: 0x001361CC
	private void Start()
	{
		this.linesRTs.Clear();
		for (int i = 0; i < this.lines.Count; i++)
		{
			this.linesRTs.Add(this.lines[i].GetComponent<RectTransform>());
		}
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
	}

	// Token: 0x06003919 RID: 14617 RVA: 0x0013801C File Offset: 0x0013621C
	private void OnEnable()
	{
		GorillaScoreboardTotalUpdater.RegisterScoreboard(this);
		this._isDirty = true;
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnSubscriptionData, new Action(this.SetDirty));
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnSubscriptionData, new Action(this.SetDirty));
	}

	// Token: 0x0600391A RID: 14618 RVA: 0x00138076 File Offset: 0x00136276
	private void OnDisable()
	{
		GorillaScoreboardTotalUpdater.UnregisterScoreboard(this);
		SubscriptionManager.OnSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnSubscriptionData, new Action(this.SetDirty));
	}

	// Token: 0x0600391B RID: 14619 RVA: 0x0013809E File Offset: 0x0013629E
	private void SetDirty()
	{
		this._isDirty = true;
	}

	// Token: 0x04004912 RID: 18706
	public GameObject scoreBoardLinePrefab;

	// Token: 0x04004913 RID: 18707
	public int startingYValue;

	// Token: 0x04004914 RID: 18708
	public int lineHeight;

	// Token: 0x04004915 RID: 18709
	public bool includeMMR;

	// Token: 0x04004916 RID: 18710
	public bool isActive;

	// Token: 0x04004917 RID: 18711
	public GameObject linesParent;

	// Token: 0x04004918 RID: 18712
	public float bigRoomYOffset = 32.5f;

	// Token: 0x04004919 RID: 18713
	[SerializeField]
	public List<GorillaPlayerScoreboardLine> lines;

	// Token: 0x0400491A RID: 18714
	private List<RectTransform> linesRTs = new List<RectTransform>();

	// Token: 0x0400491B RID: 18715
	public GameObject textsParent;

	// Token: 0x0400491C RID: 18716
	public TextMeshPro boardText;

	// Token: 0x0400491D RID: 18717
	public TextMeshPro buttonText;

	// Token: 0x0400491E RID: 18718
	public bool needsUpdate;

	// Token: 0x0400491F RID: 18719
	public TextMeshPro notInRoomText;

	// Token: 0x04004920 RID: 18720
	public string initialGameMode;

	// Token: 0x04004921 RID: 18721
	private string tempGmName;

	// Token: 0x04004922 RID: 18722
	private string gmName;

	// Token: 0x04004923 RID: 18723
	private const string error = "ERROR";

	// Token: 0x04004924 RID: 18724
	private List<string> gmNames;

	// Token: 0x04004925 RID: 18725
	private bool _isDirty = true;

	// Token: 0x04004926 RID: 18726
	private StringBuilder stringBuilder = new StringBuilder(220);

	// Token: 0x04004927 RID: 18727
	private StringBuilder buttonStringBuilder = new StringBuilder(720);
}
