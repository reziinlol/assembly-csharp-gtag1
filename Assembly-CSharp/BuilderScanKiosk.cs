using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GorillaTagScripts;
using GorillaTagScripts.Builder;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200064B RID: 1611
public class BuilderScanKiosk : MonoBehaviourTick
{
	// Token: 0x0600281C RID: 10268 RVA: 0x000D910A File Offset: 0x000D730A
	public static bool IsSaveSlotValid(int slot)
	{
		return slot >= 0 && slot < BuilderScanKiosk.NUM_SAVE_SLOTS;
	}

	// Token: 0x0600281D RID: 10269 RVA: 0x000D911C File Offset: 0x000D731C
	private void Start()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.AddListener(new UnityAction(this.OnSavePressed));
		}
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.AddListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveSuccess.AddListener(new UnityAction(this.OnSaveSuccess));
			this.targetTable.OnSaveFailure.AddListener(new UnityAction<string>(this.OnSaveFail));
			SharedBlocksManager.OnSaveTimeUpdated += this.OnSaveTimeUpdated;
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.AddListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			gorillaPressableButton.onPressed += this.OnScanButtonPressed;
		}
		this.scanTriangle = this.scanAnimation.GetComponent<MeshRenderer>();
		this.scanTriangle.enabled = false;
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.LoadPlayerPrefs();
		this.UpdateUI();
	}

	// Token: 0x0600281E RID: 10270 RVA: 0x000D9270 File Offset: 0x000D7470
	private new void OnEnable()
	{
		base.OnEnable();
		LocalisationManager.RegisterOnLanguageChanged(new Action(this.UpdateUI));
	}

	// Token: 0x0600281F RID: 10271 RVA: 0x000D9289 File Offset: 0x000D7489
	private new void OnDisable()
	{
		base.OnDisable();
		LocalisationManager.UnregisterOnLanguageChanged(new Action(this.UpdateUI));
	}

	// Token: 0x06002820 RID: 10272 RVA: 0x000D92A4 File Offset: 0x000D74A4
	private void OnDestroy()
	{
		if (this.saveButton != null)
		{
			this.saveButton.onPressButton.RemoveListener(new UnityAction(this.OnSavePressed));
		}
		SharedBlocksManager.OnSaveTimeUpdated -= this.OnSaveTimeUpdated;
		if (this.targetTable != null)
		{
			this.targetTable.OnSaveDirtyChanged.RemoveListener(new UnityAction<bool>(this.OnSaveDirtyChanged));
			this.targetTable.OnSaveFailure.RemoveListener(new UnityAction<string>(this.OnSaveFail));
		}
		if (this.noneButton != null)
		{
			this.noneButton.onPressButton.RemoveListener(new UnityAction(this.OnNoneButtonPressed));
		}
		foreach (GorillaPressableButton gorillaPressableButton in this.scanButtons)
		{
			if (!(gorillaPressableButton == null))
			{
				gorillaPressableButton.onPressed -= this.OnScanButtonPressed;
			}
		}
	}

	// Token: 0x06002821 RID: 10273 RVA: 0x000D93B8 File Offset: 0x000D75B8
	private void OnNoneButtonPressed()
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		if (this.targetTable.CurrentSaveSlot != -1)
		{
			this.targetTable.CurrentSaveSlot = -1;
			this.SavePlayerPrefs();
			this.UpdateUI();
		}
	}

	// Token: 0x06002822 RID: 10274 RVA: 0x000D940C File Offset: 0x000D760C
	private void OnScanButtonPressed(GorillaPressableButton button, bool isLeft)
	{
		if (this.targetTable == null)
		{
			return;
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		}
		int i = 0;
		while (i < this.scanButtons.Count)
		{
			if (button.Equals(this.scanButtons[i]))
			{
				if (i != this.targetTable.CurrentSaveSlot)
				{
					this.targetTable.CurrentSaveSlot = i;
					this.SavePlayerPrefs();
					this.UpdateUI();
					return;
				}
				break;
			}
			else
			{
				i++;
			}
		}
	}

	// Token: 0x06002823 RID: 10275 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDevScanPressed()
	{
	}

	// Token: 0x06002824 RID: 10276 RVA: 0x000D948C File Offset: 0x000D768C
	private void LoadPlayerPrefs()
	{
		int @int = PlayerPrefs.GetInt(BuilderScanKiosk.playerPrefKey, -1);
		this.targetTable.CurrentSaveSlot = @int;
		this.UpdateUI();
	}

	// Token: 0x06002825 RID: 10277 RVA: 0x000D94B7 File Offset: 0x000D76B7
	private void SavePlayerPrefs()
	{
		PlayerPrefs.SetInt(BuilderScanKiosk.playerPrefKey, this.targetTable.CurrentSaveSlot);
		PlayerPrefs.Save();
	}

	// Token: 0x06002826 RID: 10278 RVA: 0x000D94D4 File Offset: 0x000D76D4
	private void ToggleSaveButton(bool enabled)
	{
		if (enabled)
		{
			this.saveButton.enabled = true;
			this.saveButton.buttonRenderer.material = this.saveButton.unpressedMaterial;
			return;
		}
		this.saveButton.enabled = false;
		this.saveButton.buttonRenderer.material = this.saveButton.pressedMaterial;
	}

	// Token: 0x06002827 RID: 10279 RVA: 0x000D9534 File Offset: 0x000D7734
	public override void Tick()
	{
		if (this.isAnimating)
		{
			if (this.scanAnimation == null)
			{
				this.isAnimating = false;
			}
			else if ((double)Time.time > this.scanCompleteTime)
			{
				this.scanTriangle.enabled = false;
				this.isAnimating = false;
			}
		}
		if (this.coolingDown && (double)Time.time > this.coolDownCompleteTime)
		{
			this.coolingDown = false;
			this.UpdateUI();
		}
	}

	// Token: 0x06002828 RID: 10280 RVA: 0x000D95A4 File Offset: 0x000D77A4
	private void OnSavePressed()
	{
		if (this.targetTable == null || !this.isDirty || this.coolingDown)
		{
			return;
		}
		BuilderScanKiosk.ScannerState scannerState = this.scannerState;
		if (scannerState == BuilderScanKiosk.ScannerState.IDLE)
		{
			this.scannerState = BuilderScanKiosk.ScannerState.CONFIRMATION;
			this.UpdateUI();
			return;
		}
		if (scannerState != BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			return;
		}
		this.scannerState = BuilderScanKiosk.ScannerState.SAVING;
		if (this.scanAnimation != null)
		{
			this.scanCompleteTime = (double)(Time.time + this.scanAnimation.clip.length);
			this.scanTriangle.enabled = true;
			this.scanAnimation.Rewind();
			this.scanAnimation.Play();
		}
		if (this.soundBank != null)
		{
			this.soundBank.Play();
		}
		this.isAnimating = true;
		this.saveError = false;
		this.errorMsg = string.Empty;
		this.coolDownCompleteTime = (double)(Time.time + this.saveCooldownSeconds);
		this.coolingDown = true;
		this.UpdateUI();
		string busyStr;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY", out busyStr, "BUSY");
		string blocksErrStr;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS", out blocksErrStr, "PLEASE REMOVE BLOCKS CONNECTED OUTSIDE OF TABLE PLATFORM");
		this.targetTable.SaveTableForPlayer(busyStr, blocksErrStr);
	}

	// Token: 0x06002829 RID: 10281 RVA: 0x000D96C8 File Offset: 0x000D78C8
	private string GetSavePath()
	{
		return string.Concat(new string[]
		{
			this.GetSaveFolder(),
			Path.DirectorySeparatorChar.ToString(),
			BuilderScanKiosk.SAVE_FILE,
			"_",
			this.targetTable.CurrentSaveSlot.ToString(),
			".png"
		});
	}

	// Token: 0x0600282A RID: 10282 RVA: 0x000D9724 File Offset: 0x000D7924
	private string GetSaveFolder()
	{
		return Application.persistentDataPath + Path.DirectorySeparatorChar.ToString() + BuilderScanKiosk.SAVE_FOLDER;
	}

	// Token: 0x0600282B RID: 10283 RVA: 0x000D973F File Offset: 0x000D793F
	private void OnSaveDirtyChanged(bool dirty)
	{
		this.isDirty = dirty;
		this.UpdateUI();
	}

	// Token: 0x0600282C RID: 10284 RVA: 0x000D974E File Offset: 0x000D794E
	private void OnSaveTimeUpdated()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x0600282D RID: 10285 RVA: 0x000D974E File Offset: 0x000D794E
	private void OnSaveSuccess()
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = false;
		this.UpdateUI();
	}

	// Token: 0x0600282E RID: 10286 RVA: 0x000D9764 File Offset: 0x000D7964
	private void OnSaveFail(string errorMsg)
	{
		this.scannerState = BuilderScanKiosk.ScannerState.IDLE;
		this.saveError = true;
		this.errorMsg = errorMsg;
		this.UpdateUI();
	}

	// Token: 0x0600282F RID: 10287 RVA: 0x000D9784 File Offset: 0x000D7984
	private void UpdateUI()
	{
		this.screenText.text = this.GetTextForScreen();
		this.ToggleSaveButton(BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot) && !this.coolingDown);
		this.noneButton.buttonRenderer.material = ((!BuilderScanKiosk.IsSaveSlotValid(this.targetTable.CurrentSaveSlot)) ? this.noneButton.pressedMaterial : this.noneButton.unpressedMaterial);
		bool flag = SubscriptionManager.IsLocalSubscribed();
		for (int i = 0; i < this.scanButtons.Count; i++)
		{
			GorillaPressableButton gorillaPressableButton = this.scanButtons[i];
			if (gorillaPressableButton.isSubscriberOnlyButton && !flag)
			{
				gorillaPressableButton.buttonRenderer.material = ((gorillaPressableButton.nonSubscriberMaterial != null) ? gorillaPressableButton.nonSubscriberMaterial : gorillaPressableButton.unpressedMaterial);
			}
			else
			{
				gorillaPressableButton.buttonRenderer.material = ((this.targetTable.CurrentSaveSlot == i) ? gorillaPressableButton.pressedMaterial : gorillaPressableButton.unpressedMaterial);
			}
		}
		if (this.scannerState == BuilderScanKiosk.ScannerState.CONFIRMATION)
		{
			string text;
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON", out text, "YES UPDATE SCAN");
			this.saveButton.myTmpText.text = text;
			return;
		}
		string text2;
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON", out text2, "UPDATE SCAN");
		this.saveButton.myTmpText.text = text2;
	}

	// Token: 0x06002830 RID: 10288 RVA: 0x000D98D8 File Offset: 0x000D7AD8
	private string GetTextForScreen()
	{
		if (this.targetTable == null)
		{
			return "";
		}
		StringBuilder stringBuilder = new StringBuilder();
		string value = "";
		int currentSaveSlot = this.targetTable.CurrentSaveSlot;
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT", out value, "<b><color=red>NONE</color></b>");
			stringBuilder.Append(value);
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			stringBuilder.Append("<b><color=red>");
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out value, "SCAN ");
			stringBuilder.Append(value);
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
			SharedBlocksManager.LocalPublishInfo publishInfoForSlot = SharedBlocksManager.GetPublishInfoForSlot(currentSaveSlot);
			DateTime t = DateTime.FromBinary(publishInfoForSlot.publishTime);
			if (t > DateTime.MinValue)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL", out value, "UPDATED ");
				stringBuilder.Append(": ");
				stringBuilder.Append(value);
				stringBuilder.Append(t.ToString());
				stringBuilder.Append("\n");
			}
			if (SharedBlocksManager.IsMapIDValid(publishInfoForSlot.mapID))
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL", out value, "MAP ID: ");
				stringBuilder.Append(value);
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(0, 4));
				stringBuilder.Append("-");
				stringBuilder.Append(publishInfoForSlot.mapID.Substring(4));
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS", out value, "\nUSE THIS CODE IN THE SHARE MY BLOCKS ROOM");
				stringBuilder.Append(value);
			}
		}
		stringBuilder.Append("\n");
		switch (this.scannerState)
		{
		case BuilderScanKiosk.ScannerState.IDLE:
			if (this.saveError)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR", out value, "ERROR WHILE SCANNING: ");
				stringBuilder.Append(value);
				stringBuilder.Append(this.errorMsg);
			}
			else if (this.coolingDown)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN", out value, "COOLING DOWN...");
				stringBuilder.Append(value);
			}
			else if (!this.isDirty)
			{
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES", out value, "NO UNSAVED CHANGES");
				stringBuilder.Append(value);
			}
			break;
		case BuilderScanKiosk.ScannerState.CONFIRMATION:
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE", out value, "YOU ARE ABOUT TO REPLACE ");
			if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
			{
				stringBuilder.Append(value);
				stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
			}
			else
			{
				stringBuilder.Append(value);
				stringBuilder.Append("<b><color=red>");
				LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out value, "SCAN ");
				stringBuilder.Append(value);
				stringBuilder.Append(currentSaveSlot + 1);
				stringBuilder.Append("</color></b>");
			}
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION", out value, " ARE YOU SURE YOU WANT TO SCAN?");
			stringBuilder.Append(value);
			break;
		case BuilderScanKiosk.ScannerState.SAVING:
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING", out value, "SCANNING BUILD...");
			stringBuilder.Append(value);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		stringBuilder.Append("\n\n\n");
		LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS", out value, "CREATE A <b><color=red>NEW</color></b> PRIVATE ROOM TO LOAD ");
		stringBuilder.Append(value);
		if (!BuilderScanKiosk.IsSaveSlotValid(currentSaveSlot))
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE", out value, "<b><color=red>AN EMPTY TABLE</color></b>");
			stringBuilder.Append(value);
		}
		else if (currentSaveSlot == BuilderScanKiosk.DEV_SAVE_SLOT)
		{
			stringBuilder.Append("<b><color=red>DEV SCAN</color></b>");
		}
		else
		{
			LocalisationManager.TryGetKeyForCurrentLocale("MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL", out value, "SCAN ");
			stringBuilder.Append("<b><color=red>");
			stringBuilder.Append(value);
			stringBuilder.Append(currentSaveSlot + 1);
			stringBuilder.Append("</color></b>");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0400344F RID: 13391
	private const string MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT_KEY = "MONKE_BLOCKS_SAVE_KIOSK_NO_SAVE_SLOT";

	// Token: 0x04003450 RID: 13392
	private const string MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SCAN_LABEL";

	// Token: 0x04003451 RID: 13393
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATE_LABEL";

	// Token: 0x04003452 RID: 13394
	private const string MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL_KEY = "MONKE_BLOCKS_SAVE_KIOSK_MAP_ID_LABEL";

	// Token: 0x04003453 RID: 13395
	private const string MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_CODE_INSTRUCTIONS";

	// Token: 0x04003454 RID: 13396
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR";

	// Token: 0x04003455 RID: 13397
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BUSY";

	// Token: 0x04003456 RID: 13398
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_ERROR_BLOCKS";

	// Token: 0x04003457 RID: 13399
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_COOLDOWN";

	// Token: 0x04003458 RID: 13400
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_NO_CHANGES";

	// Token: 0x04003459 RID: 13401
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_REPLACE";

	// Token: 0x0400345A RID: 13402
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_WARNING_CONFIRMATION";

	// Token: 0x0400345B RID: 13403
	private const string MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SAVE_SAVING";

	// Token: 0x0400345C RID: 13404
	private const string MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS_KEY = "MONKE_BLOCKS_SAVE_KIOSK_LOAD_INSTRUCTIONS";

	// Token: 0x0400345D RID: 13405
	private const string MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_EMPTY_TABLE";

	// Token: 0x0400345E RID: 13406
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_NONE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_NONE";

	// Token: 0x0400345F RID: 13407
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_ONE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_ONE";

	// Token: 0x04003460 RID: 13408
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_TWO_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_TWO";

	// Token: 0x04003461 RID: 13409
	private const string MONKE_BLOCKS_SAVE_KIOSK_SLOT_THREE_KEY = "MONKE_BLOCKS_SAVE_KIOSK_SLOT_THREE";

	// Token: 0x04003462 RID: 13410
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATED_BUTTON";

	// Token: 0x04003463 RID: 13411
	private const string MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON_KEY = "MONKE_BLOCKS_SAVE_KIOSK_UPDATE_CONFIRM_BUTTON";

	// Token: 0x04003464 RID: 13412
	[SerializeField]
	private GorillaPressableButton saveButton;

	// Token: 0x04003465 RID: 13413
	[SerializeField]
	private GorillaPressableButton noneButton;

	// Token: 0x04003466 RID: 13414
	[SerializeField]
	private List<GorillaPressableButton> scanButtons;

	// Token: 0x04003467 RID: 13415
	[SerializeField]
	private BuilderTable targetTable;

	// Token: 0x04003468 RID: 13416
	[SerializeField]
	private float saveCooldownSeconds = 5f;

	// Token: 0x04003469 RID: 13417
	[SerializeField]
	private TMP_Text screenText;

	// Token: 0x0400346A RID: 13418
	[SerializeField]
	private SoundBankPlayer soundBank;

	// Token: 0x0400346B RID: 13419
	[SerializeField]
	private Animation scanAnimation;

	// Token: 0x0400346C RID: 13420
	private MeshRenderer scanTriangle;

	// Token: 0x0400346D RID: 13421
	private bool isAnimating;

	// Token: 0x0400346E RID: 13422
	private static string playerPrefKey = "BuilderSaveSlot";

	// Token: 0x0400346F RID: 13423
	private static string SAVE_FOLDER = "MonkeBlocks";

	// Token: 0x04003470 RID: 13424
	private static string SAVE_FILE = "MyBuild";

	// Token: 0x04003471 RID: 13425
	public static int NUM_SAVE_SLOTS = 5;

	// Token: 0x04003472 RID: 13426
	public static int DEV_SAVE_SLOT = -2;

	// Token: 0x04003473 RID: 13427
	private Texture2D buildCaptureTexture;

	// Token: 0x04003474 RID: 13428
	private bool isDirty;

	// Token: 0x04003475 RID: 13429
	private bool saveError;

	// Token: 0x04003476 RID: 13430
	private string errorMsg = string.Empty;

	// Token: 0x04003477 RID: 13431
	private bool coolingDown;

	// Token: 0x04003478 RID: 13432
	private double coolDownCompleteTime;

	// Token: 0x04003479 RID: 13433
	private double scanCompleteTime;

	// Token: 0x0400347A RID: 13434
	private BuilderScanKiosk.ScannerState scannerState;

	// Token: 0x0200064C RID: 1612
	private enum ScannerState
	{
		// Token: 0x0400347C RID: 13436
		IDLE,
		// Token: 0x0400347D RID: 13437
		CONFIRMATION,
		// Token: 0x0400347E RID: 13438
		SAVING
	}
}
