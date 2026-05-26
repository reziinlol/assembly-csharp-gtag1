using System;
using System.Collections.Generic;
using System.Text;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005C1 RID: 1473
public class GameModePages : BasePageHandler
{
	// Token: 0x170003E7 RID: 999
	// (get) Token: 0x06002503 RID: 9475 RVA: 0x000C56C9 File Offset: 0x000C38C9
	protected override int pageSize
	{
		get
		{
			return this.buttons.Length;
		}
	}

	// Token: 0x170003E8 RID: 1000
	// (get) Token: 0x06002504 RID: 9476 RVA: 0x000C56D3 File Offset: 0x000C38D3
	protected override int entriesCount
	{
		get
		{
			return GameMode.gameModeNames.Count;
		}
	}

	// Token: 0x06002505 RID: 9477 RVA: 0x000C56E0 File Offset: 0x000C38E0
	private void Awake()
	{
		GameModePages.gameModeSelectorInstances.Add(this);
		this.buttons = base.GetComponentsInChildren<GameModeSelectButton>();
		for (int i = 0; i < this.buttons.Length; i++)
		{
			this.buttons[i].buttonIndex = i;
			this.buttons[i].selector = this;
		}
	}

	// Token: 0x06002506 RID: 9478 RVA: 0x000C5733 File Offset: 0x000C3933
	protected override void Start()
	{
		base.Start();
		base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		this.initialized = true;
	}

	// Token: 0x06002507 RID: 9479 RVA: 0x000C574D File Offset: 0x000C394D
	private void OnEnable()
	{
		if (this.initialized)
		{
			base.SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x06002508 RID: 9480 RVA: 0x000C5762 File Offset: 0x000C3962
	private void OnDestroy()
	{
		GameModePages.gameModeSelectorInstances.Remove(this);
	}

	// Token: 0x06002509 RID: 9481 RVA: 0x000C5770 File Offset: 0x000C3970
	protected override void ShowPage(int selectedPage, int startIndex, int endIndex)
	{
		GameModePages.textBuilder.Clear();
		for (int i = startIndex; i < endIndex; i++)
		{
			GameModePages.textBuilder.AppendLine(GameMode.gameModeNames[i]);
		}
		this.gameModeText.text = GameModePages.textBuilder.ToString();
		if (base.selectedIndex >= startIndex && base.selectedIndex <= endIndex)
		{
			this.UpdateAllButtons(this.currentButtonIndex);
		}
		else
		{
			this.UpdateAllButtons(-1);
		}
		int buttonsMissing = (selectedPage == base.pages - 1 && base.maxEntires > endIndex) ? (base.maxEntires - endIndex) : 0;
		this.EnableEntryButtons(buttonsMissing);
	}

	// Token: 0x0600250A RID: 9482 RVA: 0x000C580D File Offset: 0x000C3A0D
	protected override void PageEntrySelected(int pageEntry, int selectionIndex)
	{
		if (selectionIndex >= this.entriesCount)
		{
			return;
		}
		GameModePages.sharedSelectedIndex = selectionIndex;
		this.UpdateAllButtons(pageEntry);
		this.currentButtonIndex = pageEntry;
		GorillaComputer.instance.OnModeSelectButtonPress(GameMode.gameModeNames[selectionIndex], false);
	}

	// Token: 0x0600250B RID: 9483 RVA: 0x000C5848 File Offset: 0x000C3A48
	private void UpdateAllButtons(int onButton)
	{
		for (int i = 0; i < this.buttons.Length; i++)
		{
			if (i == onButton)
			{
				this.buttons[onButton].isOn = true;
				this.buttons[onButton].UpdateColor();
			}
			else if (this.buttons[i].isOn)
			{
				this.buttons[i].isOn = false;
				this.buttons[i].UpdateColor();
			}
		}
	}

	// Token: 0x0600250C RID: 9484 RVA: 0x000C58B4 File Offset: 0x000C3AB4
	private void EnableEntryButtons(int buttonsMissing)
	{
		int num = this.buttons.Length - buttonsMissing;
		int i;
		for (i = 0; i < num; i++)
		{
			this.buttons[i].gameObject.SetActive(true);
		}
		while (i < this.buttons.Length)
		{
			this.buttons[i].gameObject.SetActive(false);
			i++;
		}
	}

	// Token: 0x0600250D RID: 9485 RVA: 0x000C5910 File Offset: 0x000C3B10
	public static void SetSelectedGameModeShared(string gameMode)
	{
		GameModePages.sharedSelectedIndex = GameMode.gameModeNames.IndexOf(gameMode);
		if (GameModePages.sharedSelectedIndex < 0)
		{
			return;
		}
		for (int i = 0; i < GameModePages.gameModeSelectorInstances.Count; i++)
		{
			GameModePages.gameModeSelectorInstances[i].SelectEntryFromIndex(GameModePages.sharedSelectedIndex);
		}
	}

	// Token: 0x0400305A RID: 12378
	private int currentButtonIndex;

	// Token: 0x0400305B RID: 12379
	[SerializeField]
	private Text gameModeText;

	// Token: 0x0400305C RID: 12380
	[SerializeField]
	private GameModeSelectButton[] buttons;

	// Token: 0x0400305D RID: 12381
	private bool initialized;

	// Token: 0x0400305E RID: 12382
	private static int sharedSelectedIndex = 0;

	// Token: 0x0400305F RID: 12383
	private static StringBuilder textBuilder = new StringBuilder(50);

	// Token: 0x04003060 RID: 12384
	[OnEnterPlay_Clear]
	private static List<GameModePages> gameModeSelectorInstances = new List<GameModePages>(7);
}
