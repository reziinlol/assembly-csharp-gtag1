using System;
using GorillaNetworking;
using LitJson;
using PlayFab;
using UnityEngine;

// Token: 0x02000AFE RID: 2814
public class AnnouncementManager : MonoBehaviour
{
	// Token: 0x0600480B RID: 18443 RVA: 0x001824AA File Offset: 0x001806AA
	public bool ShowAnnouncement()
	{
		return this._showAnnouncement;
	}

	// Token: 0x170006B8 RID: 1720
	// (get) Token: 0x0600480C RID: 18444 RVA: 0x001824B2 File Offset: 0x001806B2
	// (set) Token: 0x0600480D RID: 18445 RVA: 0x001824BA File Offset: 0x001806BA
	public bool _completedSetup { get; private set; }

	// Token: 0x170006B9 RID: 1721
	// (get) Token: 0x0600480E RID: 18446 RVA: 0x001824C3 File Offset: 0x001806C3
	// (set) Token: 0x0600480F RID: 18447 RVA: 0x001824CB File Offset: 0x001806CB
	public bool _announcementActive { get; private set; }

	// Token: 0x170006BA RID: 1722
	// (get) Token: 0x06004810 RID: 18448 RVA: 0x001824D4 File Offset: 0x001806D4
	public static AnnouncementManager Instance
	{
		get
		{
			if (AnnouncementManager._instance == null)
			{
				Debug.LogError("[KID::ANNOUNCEMENT] [_instance] is NULL, does it exist in the scene?");
			}
			return AnnouncementManager._instance;
		}
	}

	// Token: 0x170006BB RID: 1723
	// (get) Token: 0x06004811 RID: 18449 RVA: 0x001824F2 File Offset: 0x001806F2
	private static string AnnouncementDPlayerPref
	{
		get
		{
			if (string.IsNullOrEmpty(AnnouncementManager._announcementIDPref))
			{
				AnnouncementManager._announcementIDPref = "announcement-id-" + PlayFabAuthenticator.instance.GetPlayFabPlayerId();
			}
			return AnnouncementManager._announcementIDPref;
		}
	}

	// Token: 0x06004812 RID: 18450 RVA: 0x00182520 File Offset: 0x00180720
	private void Awake()
	{
		if (AnnouncementManager._instance != null)
		{
			Debug.LogError("[KID::ANNOUNCEMENT] [AnnouncementManager] has already been setup, does another already exist in the scene?");
			return;
		}
		AnnouncementManager._instance = this;
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Announcement Message Box has not been set. Announcement system will not work without it");
		}
	}

	// Token: 0x06004813 RID: 18451 RVA: 0x00182558 File Offset: 0x00180758
	private void Start()
	{
		if (this._announcementMessageBox == null)
		{
			return;
		}
		this._announcementMessageBox.RightButton = "";
		this._announcementMessageBox.LeftButton = "Continue";
		PlayFabTitleDataCache.Instance.GetTitleData("AnnouncementData", new Action<string>(this.ConfigureAnnouncement), new Action<PlayFabError>(this.OnError), false);
	}

	// Token: 0x06004814 RID: 18452 RVA: 0x001825BC File Offset: 0x001807BC
	public void OnContinuePressed()
	{
		HandRayController.Instance.DisableHandRays();
		if (this._announcementMessageBox == null)
		{
			Debug.LogError("[ANNOUNCEMENT] Message Box is null, Continue Button cannot work");
			return;
		}
		PrivateUIRoom.RemoveUI(this._announcementMessageBox.transform);
		this._announcementActive = false;
		PlayerPrefs.SetString(AnnouncementManager.AnnouncementDPlayerPref, this._announcementData.AnnouncementID);
		PlayerPrefs.Save();
	}

	// Token: 0x06004815 RID: 18453 RVA: 0x0018261D File Offset: 0x0018081D
	private void OnError(PlayFabError error)
	{
		Debug.LogError("[ANNOUNCEMENT] Failed to Get Title Data for key [AnnouncementData]. Error:\n[" + error.ErrorMessage);
		this._completedSetup = true;
	}

	// Token: 0x06004816 RID: 18454 RVA: 0x0018263C File Offset: 0x0018083C
	private void ConfigureAnnouncement(string data)
	{
		this._announcementString = data;
		this._announcementData = JsonMapper.ToObject<SAnnouncementData>(this._announcementString);
		if (!bool.TryParse(this._announcementData.ShowAnnouncement, out this._showAnnouncement))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Failed to parse [ShowAnnouncement] with value [" + this._announcementData.ShowAnnouncement + "] to a bool, assuming false");
			return;
		}
		if (!this.ShowAnnouncement())
		{
			this._completedSetup = true;
			return;
		}
		if (string.IsNullOrEmpty(this._announcementData.AnnouncementID))
		{
			this._completedSetup = true;
			Debug.LogError("[ANNOUNCEMENT] Announcement Version is empty or null. Will not show announcement");
			return;
		}
		string @string = PlayerPrefs.GetString(AnnouncementManager.AnnouncementDPlayerPref, "");
		if (this._announcementData.AnnouncementID == @string)
		{
			this._completedSetup = true;
			return;
		}
		PrivateUIRoom.ForceStartOverlay(PrivateUIRoom.OverlaySource.KID, "");
		HandRayController.Instance.EnableHandRays();
		this._announcementMessageBox.Header = this._announcementData.AnnouncementTitle;
		this._announcementMessageBox.Body = this._announcementData.Message;
		this._announcementActive = true;
		PrivateUIRoom.AddUI(this._announcementMessageBox.transform);
		this._completedSetup = true;
	}

	// Token: 0x04005A2C RID: 23084
	private const string ANNOUNCEMENT_ID_PLAYERPREF_PREFIX = "announcement-id-";

	// Token: 0x04005A2D RID: 23085
	private const string ANNOUNCEMENT_TITLE_DATA_KEY = "AnnouncementData";

	// Token: 0x04005A2E RID: 23086
	private const string ANNOUNCEMENT_HEADING = "Announcement!";

	// Token: 0x04005A2F RID: 23087
	private const string ANNOUNCEMENT_BUTTON_TEXT = "Continue";

	// Token: 0x04005A30 RID: 23088
	[SerializeField]
	private MessageBox _announcementMessageBox;

	// Token: 0x04005A31 RID: 23089
	private string _announcementString = string.Empty;

	// Token: 0x04005A32 RID: 23090
	private SAnnouncementData _announcementData;

	// Token: 0x04005A33 RID: 23091
	private bool _showAnnouncement;

	// Token: 0x04005A36 RID: 23094
	private static AnnouncementManager _instance;

	// Token: 0x04005A37 RID: 23095
	private static string _announcementIDPref = "";
}
