using System;
using GorillaNetworking;
using GorillaTagScripts;
using TMPro;
using UnityEngine;

// Token: 0x02000C71 RID: 3185
public class FriendCard : MonoBehaviour
{
	// Token: 0x17000761 RID: 1889
	// (get) Token: 0x06004ED9 RID: 20185 RVA: 0x001A13EC File Offset: 0x0019F5EC
	public TextMeshProUGUI NameText
	{
		get
		{
			return this.nameText;
		}
	}

	// Token: 0x17000762 RID: 1890
	// (get) Token: 0x06004EDA RID: 20186 RVA: 0x001A13F4 File Offset: 0x0019F5F4
	public TextMeshProUGUI RoomText
	{
		get
		{
			return this.roomText;
		}
	}

	// Token: 0x17000763 RID: 1891
	// (get) Token: 0x06004EDB RID: 20187 RVA: 0x001A13FC File Offset: 0x0019F5FC
	public TextMeshProUGUI ZoneText
	{
		get
		{
			return this.zoneText;
		}
	}

	// Token: 0x17000764 RID: 1892
	// (get) Token: 0x06004EDC RID: 20188 RVA: 0x001A1404 File Offset: 0x0019F604
	public float Width
	{
		get
		{
			return this.width;
		}
	}

	// Token: 0x17000765 RID: 1893
	// (get) Token: 0x06004EDD RID: 20189 RVA: 0x001A140C File Offset: 0x0019F60C
	// (set) Token: 0x06004EDE RID: 20190 RVA: 0x001A1414 File Offset: 0x0019F614
	public float Height { get; private set; } = 0.25f;

	// Token: 0x06004EDF RID: 20191 RVA: 0x001A141D File Offset: 0x0019F61D
	private void Awake()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(false);
		}
	}

	// Token: 0x06004EE0 RID: 20192 RVA: 0x001A143D File Offset: 0x0019F63D
	private void OnDestroy()
	{
		if (this._button)
		{
			this._button.onPressed -= this.OnButtonPressed;
		}
	}

	// Token: 0x06004EE1 RID: 20193 RVA: 0x001A1463 File Offset: 0x0019F663
	public void Init(FriendDisplay owner)
	{
		this.friendDisplay = owner;
	}

	// Token: 0x06004EE2 RID: 20194 RVA: 0x001A146C File Offset: 0x0019F66C
	private void UpdateComponentStates()
	{
		if (this.removeProgressBar)
		{
			this.removeProgressBar.gameObject.SetActive(this.canRemove);
		}
		bool flag = this._isVimSlot && !SubscriptionManager.IsLocalSubscribed();
		if (this.canRemove)
		{
			this.SetButtonState((this.currentFriend != null) ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
			return;
		}
		if (this.joinable && !flag)
		{
			this.SetButtonState(FriendDisplay.ButtonState.Active);
			return;
		}
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06004EE3 RID: 20195 RVA: 0x001A14E8 File Offset: 0x0019F6E8
	private void SetButtonState(FriendDisplay.ButtonState newState)
	{
		if (this._button == null)
		{
			return;
		}
		if (this._buttonState == newState)
		{
			return;
		}
		this._buttonState = newState;
		MeshRenderer buttonRenderer = this._button.buttonRenderer;
		FriendDisplay.ButtonState buttonState = this._buttonState;
		Material[] sharedMaterials;
		switch (buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			sharedMaterials = this._buttonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			sharedMaterials = this._buttonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			sharedMaterials = this._buttonAlertMaterials;
			break;
		default:
			<PrivateImplementationDetails>.ThrowSwitchExpressionException(buttonState);
			break;
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
		this._button.delayTime = (float)((this._buttonState == FriendDisplay.ButtonState.Alert) ? 3 : 0);
	}

	// Token: 0x06004EE4 RID: 20196 RVA: 0x001A1582 File Offset: 0x0019F782
	public void Populate(FriendBackendController.Friend friend)
	{
		this.Populate(friend, false);
	}

	// Token: 0x06004EE5 RID: 20197 RVA: 0x001A158C File Offset: 0x0019F78C
	public void Populate(FriendBackendController.Friend friend, bool isVimSlot)
	{
		this.SetEmpty(isVimSlot);
		if (friend != null && friend.Presence != null)
		{
			if (friend.Presence.UserName != null)
			{
				this.SetName(friend.Presence.UserName.ToUpper());
			}
			if (!string.IsNullOrEmpty(friend.Presence.RoomId) && friend.Presence.RoomId.Length > 0)
			{
				bool? isPublic = friend.Presence.IsPublic;
				bool flag = true;
				bool flag2 = isPublic.GetValueOrDefault() == flag & isPublic != null;
				bool flag3 = friend.Presence.RoomId[0] == '@';
				bool flag4 = friend.Presence.RoomId.Equals(NetworkSystem.Instance.RoomName);
				bool flag5 = false;
				if (!flag4 && flag2 && !friend.Presence.Zone.IsNullOrEmpty())
				{
					string text = friend.Presence.Zone.ToLower();
					foreach (GTZone e in ZoneManagement.instance.activeZones)
					{
						if (text.Contains(e.GetName<GTZone>().ToLower()))
						{
							flag5 = true;
						}
					}
				}
				this.joinable = (!flag3 && !flag4 && (!flag2 || flag5) && this.HasKIDPermissionToJoinPrivateRooms());
				if (flag3)
				{
					this.SetRoom(friend.Presence.RoomId.Substring(1).ToUpper());
					this.SetZone("CUSTOM");
				}
				else if (!flag2)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone("PRIVATE");
				}
				else if (friend.Presence.Zone != null)
				{
					this.SetRoom(friend.Presence.RoomId.ToUpper());
					this.SetZone(friend.Presence.Zone.ToUpper());
				}
			}
			else
			{
				this.joinable = false;
				this.SetRoom("OFFLINE");
			}
			this.currentFriend = friend;
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06004EE6 RID: 20198 RVA: 0x001A17A8 File Offset: 0x0019F9A8
	public void SetName(string friendName)
	{
		TMP_Text tmp_Text = this.nameText;
		this._friendName = friendName;
		tmp_Text.text = friendName;
	}

	// Token: 0x06004EE7 RID: 20199 RVA: 0x001A17CC File Offset: 0x0019F9CC
	public void SetRoom(string friendRoom)
	{
		TMP_Text tmp_Text = this.roomText;
		this._friendRoom = friendRoom;
		tmp_Text.text = friendRoom;
	}

	// Token: 0x06004EE8 RID: 20200 RVA: 0x001A17F0 File Offset: 0x0019F9F0
	public void SetZone(string friendZone)
	{
		TMP_Text tmp_Text = this.zoneText;
		this._friendZone = friendZone;
		tmp_Text.text = friendZone;
	}

	// Token: 0x06004EE9 RID: 20201 RVA: 0x001A1814 File Offset: 0x0019FA14
	public void Randomize()
	{
		this.SetEmpty();
		int num = Random.Range(0, this.randomNames.Length);
		this.SetName(this.randomNames[num].ToUpper());
		this.SetRoom(string.Format("{0}{1}{2}{3}", new object[]
		{
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91),
			(char)Random.Range(65, 91)
		}));
		bool flag = Random.Range(0f, 1f) > 0.5f;
		this.joinable = (flag && Random.Range(0f, 1f) > 0.5f);
		if (flag)
		{
			int num2 = Random.Range(0, 17);
			GTZone gtzone = (GTZone)num2;
			this.SetZone(gtzone.ToString().ToUpper());
		}
		else
		{
			this.SetZone(this.privateString);
		}
		this.UpdateComponentStates();
	}

	// Token: 0x06004EEA RID: 20202 RVA: 0x001A191A File Offset: 0x0019FB1A
	public void SetEmpty()
	{
		this.SetEmpty(this._isVimSlot);
	}

	// Token: 0x06004EEB RID: 20203 RVA: 0x001A1928 File Offset: 0x0019FB28
	public void SetEmpty(bool isVimSlot)
	{
		base.CancelInvoke("RestoreAfterResubscribeMessage");
		this.SetName(this.emptyString);
		this.SetRoom(this.emptyString);
		this.SetZone(this.emptyString);
		this.joinable = false;
		this.currentFriend = null;
		this._isVimSlot = isVimSlot;
		this.UpdateComponentStates();
	}

	// Token: 0x06004EEC RID: 20204 RVA: 0x001A197F File Offset: 0x0019FB7F
	public void SetRemoveEnabled(bool enabled)
	{
		this.canRemove = enabled;
		this.UpdateComponentStates();
	}

	// Token: 0x06004EED RID: 20205 RVA: 0x001A1990 File Offset: 0x0019FB90
	private void JoinButtonPressed()
	{
		if (this.joinable && this.currentFriend != null && this.currentFriend.Presence != null)
		{
			bool? isPublic = this.currentFriend.Presence.IsPublic;
			bool flag = true;
			JoinType roomJoinType = (isPublic.GetValueOrDefault() == flag & isPublic != null) ? JoinType.FriendStationPublic : JoinType.FriendStationPrivate;
			GorillaComputer.instance.roomToJoin = this._friendRoom;
			PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(this._friendRoom, roomJoinType);
			this.joinable = false;
			this.UpdateComponentStates();
		}
	}

	// Token: 0x06004EEE RID: 20206 RVA: 0x001A1A18 File Offset: 0x0019FC18
	private void RemoveFriendButtonPressed()
	{
		if (this.friendDisplay.InRemoveMode)
		{
			FriendSystem.Instance.RemoveFriend(this.currentFriend, null);
			this.SetEmpty();
		}
	}

	// Token: 0x06004EEF RID: 20207 RVA: 0x001A1A40 File Offset: 0x0019FC40
	private void OnDrawGizmosSelected()
	{
		float num = this.width * 0.5f * base.transform.lossyScale.x;
		float num2 = this.Height * 0.5f * base.transform.lossyScale.y;
		float num3 = num;
		float num4 = num2;
		Vector3 vector = base.transform.position + base.transform.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector2 = base.transform.position + base.transform.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector3 = base.transform.position + base.transform.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 vector4 = base.transform.position + base.transform.rotation * new Vector3(num3, -num4, 0f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector2);
		Gizmos.DrawLine(vector2, vector4);
		Gizmos.DrawLine(vector4, vector3);
		Gizmos.DrawLine(vector3, vector);
	}

	// Token: 0x06004EF0 RID: 20208 RVA: 0x001A1B74 File Offset: 0x0019FD74
	public void SetButton(GorillaPressableDelayButton friendCardButton, Material[] normalMaterials, Material[] activeMaterials, Material[] alertMaterials, TextMeshProUGUI buttonText)
	{
		this._button = friendCardButton;
		this._button.SetFillBar(this.removeProgressBar);
		this._button.onPressBegin += this.OnButtonPressBegin;
		this._button.onPressAbort += this.OnButtonPressAbort;
		this._button.onPressed += this.OnButtonPressed;
		this._buttonDefaultMaterials = normalMaterials;
		this._buttonActiveMaterials = activeMaterials;
		this._buttonAlertMaterials = alertMaterials;
		this._buttonText = buttonText;
		this.SetButtonState(FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06004EF1 RID: 20209 RVA: 0x001A1C03 File Offset: 0x0019FE03
	private void OnRemoveFriendBegin()
	{
		this.nameText.text = "REMOVING";
		this.roomText.text = "FRIEND";
		this.zoneText.text = this.emptyString;
	}

	// Token: 0x06004EF2 RID: 20210 RVA: 0x001A1C36 File Offset: 0x0019FE36
	private void OnRemoveFriendEnd()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x06004EF3 RID: 20211 RVA: 0x001A1C6C File Offset: 0x0019FE6C
	private void OnButtonPressBegin()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendBegin();
			break;
		default:
			return;
		}
	}

	// Token: 0x06004EF4 RID: 20212 RVA: 0x001A1C9C File Offset: 0x0019FE9C
	private void OnButtonPressAbort()
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
		case FriendDisplay.ButtonState.Active:
			break;
		case FriendDisplay.ButtonState.Alert:
			this.OnRemoveFriendEnd();
			break;
		default:
			return;
		}
	}

	// Token: 0x06004EF5 RID: 20213 RVA: 0x001A1CCC File Offset: 0x0019FECC
	private void OnButtonPressed(GorillaPressableButton button, bool isLeftHand)
	{
		switch (this._buttonState)
		{
		case FriendDisplay.ButtonState.Default:
			if (this._isVimSlot && this.currentFriend != null && !SubscriptionManager.IsLocalSubscribed())
			{
				this.ShowResubscribeMessage();
				return;
			}
			break;
		case FriendDisplay.ButtonState.Active:
			this.JoinButtonPressed();
			return;
		case FriendDisplay.ButtonState.Alert:
			this.RemoveFriendButtonPressed();
			break;
		default:
			return;
		}
	}

	// Token: 0x06004EF6 RID: 20214 RVA: 0x001A1D20 File Offset: 0x0019FF20
	private void ShowResubscribeMessage()
	{
		base.CancelInvoke("RestoreAfterResubscribeMessage");
		this.nameText.text = "RESUBSCRIBE TO UNLOCK!";
		this.roomText.text = this.emptyString;
		this.zoneText.text = this.emptyString;
		base.Invoke("RestoreAfterResubscribeMessage", 2.5f);
	}

	// Token: 0x06004EF7 RID: 20215 RVA: 0x001A1C36 File Offset: 0x0019FE36
	private void RestoreAfterResubscribeMessage()
	{
		this.nameText.text = this._friendName;
		this.roomText.text = this._friendRoom;
		this.zoneText.text = this._friendZone;
	}

	// Token: 0x06004EF8 RID: 20216 RVA: 0x001A1D7A File Offset: 0x0019FF7A
	private bool HasKIDPermissionToJoinPrivateRooms()
	{
		return !KIDManager.KidEnabled || (KIDManager.HasPermissionToUseFeature(EKIDFeatures.Groups) && KIDManager.HasPermissionToUseFeature(EKIDFeatures.Multiplayer));
	}

	// Token: 0x040060AA RID: 24746
	[SerializeField]
	private TextMeshProUGUI nameText;

	// Token: 0x040060AB RID: 24747
	[SerializeField]
	private TextMeshProUGUI roomText;

	// Token: 0x040060AC RID: 24748
	[SerializeField]
	private TextMeshProUGUI zoneText;

	// Token: 0x040060AD RID: 24749
	[SerializeField]
	private Transform removeProgressBar;

	// Token: 0x040060AE RID: 24750
	[SerializeField]
	private float width = 0.25f;

	// Token: 0x040060B0 RID: 24752
	private const string ResubscribeMessage = "RESUBSCRIBE TO UNLOCK!";

	// Token: 0x040060B1 RID: 24753
	private const float ResubscribeMessageDuration = 2.5f;

	// Token: 0x040060B2 RID: 24754
	private string emptyString = "";

	// Token: 0x040060B3 RID: 24755
	private string privateString = "PRIVATE";

	// Token: 0x040060B4 RID: 24756
	private bool joinable;

	// Token: 0x040060B5 RID: 24757
	private bool canRemove;

	// Token: 0x040060B6 RID: 24758
	private bool _isVimSlot;

	// Token: 0x040060B7 RID: 24759
	private GorillaPressableDelayButton _button;

	// Token: 0x040060B8 RID: 24760
	private TextMeshProUGUI _buttonText;

	// Token: 0x040060B9 RID: 24761
	private string _friendName;

	// Token: 0x040060BA RID: 24762
	private string _friendRoom;

	// Token: 0x040060BB RID: 24763
	private string _friendZone;

	// Token: 0x040060BC RID: 24764
	private FriendBackendController.Friend currentFriend;

	// Token: 0x040060BD RID: 24765
	private FriendDisplay friendDisplay;

	// Token: 0x040060BE RID: 24766
	private string[] randomNames = new string[]
	{
		"Veronica",
		"Roman",
		"Janiyah",
		"Dalton",
		"Bellamy",
		"Eithan",
		"Celeste",
		"Isaac",
		"Astrid",
		"Azariah",
		"Keilani",
		"Zeke",
		"Jayleen",
		"Yosef",
		"Jaylee",
		"Bodie",
		"Greta",
		"Cain",
		"Ella",
		"Everly",
		"Finnley",
		"Paisley",
		"Kaison",
		"Luna",
		"Nina",
		"Maison",
		"Monroe",
		"Ricardo",
		"Zariyah",
		"Travis",
		"Lacey",
		"Elian",
		"Frankie",
		"Otis",
		"Adele",
		"Edison",
		"Amira",
		"Ivan",
		"Raelynn",
		"Eliel",
		"Aliana",
		"Beckett",
		"Mylah",
		"Melvin",
		"Magdalena",
		"Leroy",
		"Madeleine"
	};

	// Token: 0x040060BF RID: 24767
	private FriendDisplay.ButtonState _buttonState = (FriendDisplay.ButtonState)(-1);

	// Token: 0x040060C0 RID: 24768
	private Material[] _buttonDefaultMaterials;

	// Token: 0x040060C1 RID: 24769
	private Material[] _buttonActiveMaterials;

	// Token: 0x040060C2 RID: 24770
	private Material[] _buttonAlertMaterials;
}
