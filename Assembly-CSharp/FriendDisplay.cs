using System;
using System.Collections.Generic;
using GorillaLocomotion;
using GorillaNetworking;
using GorillaTagScripts;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000C72 RID: 3186
public class FriendDisplay : MonoBehaviour
{
	// Token: 0x17000766 RID: 1894
	// (get) Token: 0x06004EFA RID: 20218 RVA: 0x001A1F89 File Offset: 0x001A0189
	// (set) Token: 0x06004EFB RID: 20219 RVA: 0x001A1F90 File Offset: 0x001A0190
	public static int ConfiguredVimPageCount { get; private set; } = 0;

	// Token: 0x17000767 RID: 1895
	// (get) Token: 0x06004EFC RID: 20220 RVA: 0x001A1F98 File Offset: 0x001A0198
	// (set) Token: 0x06004EFD RID: 20221 RVA: 0x001A1F9F File Offset: 0x001A019F
	public static int ConfiguredFreeExtraPageCount { get; private set; } = 1;

	// Token: 0x17000768 RID: 1896
	// (get) Token: 0x06004EFE RID: 20222 RVA: 0x001A1FA7 File Offset: 0x001A01A7
	private int totalPages
	{
		get
		{
			return 1 + this.freeExtraPageCount + this.vimPageCount;
		}
	}

	// Token: 0x17000769 RID: 1897
	// (get) Token: 0x06004EFF RID: 20223 RVA: 0x001A1FB8 File Offset: 0x001A01B8
	public int TotalCapacity
	{
		get
		{
			return 9 + (this.freeExtraPageCount + this.vimPageCount) * 9;
		}
	}

	// Token: 0x1700076A RID: 1898
	// (get) Token: 0x06004F00 RID: 20224 RVA: 0x001A1FCD File Offset: 0x001A01CD
	public int VIMTotalCapacity
	{
		get
		{
			return this.vimPageCount * 9;
		}
	}

	// Token: 0x1700076B RID: 1899
	// (get) Token: 0x06004F01 RID: 20225 RVA: 0x001A1FD8 File Offset: 0x001A01D8
	public int FreeExtraTotalCapacity
	{
		get
		{
			return this.freeExtraPageCount * 9;
		}
	}

	// Token: 0x1700076C RID: 1900
	// (get) Token: 0x06004F02 RID: 20226 RVA: 0x001A1FE3 File Offset: 0x001A01E3
	public int VimPageCount
	{
		get
		{
			return this.vimPageCount;
		}
	}

	// Token: 0x1700076D RID: 1901
	// (get) Token: 0x06004F03 RID: 20227 RVA: 0x001A1FEB File Offset: 0x001A01EB
	public bool InRemoveMode
	{
		get
		{
			return this.inRemoveMode;
		}
	}

	// Token: 0x06004F04 RID: 20228 RVA: 0x001A1FF3 File Offset: 0x001A01F3
	private void Awake()
	{
		FriendDisplay.ConfiguredVimPageCount = this.vimPageCount;
		FriendDisplay.ConfiguredFreeExtraPageCount = this.freeExtraPageCount;
	}

	// Token: 0x06004F05 RID: 20229 RVA: 0x001A200C File Offset: 0x001A020C
	private void Start()
	{
		this.InitFriendCards();
		this.InitLocalPlayerCard();
		this.UpdateLocalPlayerPrivacyButtons();
		this.triggerNotifier.TriggerEnterEvent += this.TriggerEntered;
		this.triggerNotifier.TriggerExitEvent += this.TriggerExited;
		NetworkSystem.Instance.OnJoinedRoomEvent += this.OnJoinedRoom;
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Combine(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnLocalSubscriptionChanged));
	}

	// Token: 0x06004F06 RID: 20230 RVA: 0x001A209C File Offset: 0x001A029C
	private void OnDestroy()
	{
		if (NetworkSystem.Instance != null)
		{
			NetworkSystem.Instance.OnJoinedRoomEvent -= this.OnJoinedRoom;
		}
		if (this.triggerNotifier != null)
		{
			this.triggerNotifier.TriggerEnterEvent -= this.TriggerEntered;
			this.triggerNotifier.TriggerExitEvent -= this.TriggerExited;
		}
		SubscriptionManager.OnLocalSubscriptionData = (Action)Delegate.Remove(SubscriptionManager.OnLocalSubscriptionData, new Action(this.OnLocalSubscriptionChanged));
	}

	// Token: 0x06004F07 RID: 20231 RVA: 0x001A2133 File Offset: 0x001A0333
	private void OnLocalSubscriptionChanged()
	{
		if (!this.localPlayerAtDisplay)
		{
			return;
		}
		this.GoToFriendPage(this._currentPage);
	}

	// Token: 0x06004F08 RID: 20232 RVA: 0x001A214C File Offset: 0x001A034C
	public void TriggerEntered(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh += this.OnGetFriendsReceived;
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
			this.localPlayerAtDisplay = true;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x06004F09 RID: 20233 RVA: 0x001A21AC File Offset: 0x001A03AC
	public void TriggerExited(TriggerEventNotifier notifier, Collider other)
	{
		if (other == GTPlayer.Instance.headCollider)
		{
			FriendSystem.Instance.OnFriendListRefresh -= this.OnGetFriendsReceived;
			this.ClearFriendCards();
			this.ClearLocalPlayerCard();
			this.ClearPageButtons();
			this.localPlayerAtDisplay = false;
			if (this.InRemoveMode)
			{
				this.ToggleRemoveFriendMode();
			}
		}
	}

	// Token: 0x06004F0A RID: 20234 RVA: 0x001A220A File Offset: 0x001A040A
	private void OnJoinedRoom()
	{
		this.Refresh();
	}

	// Token: 0x06004F0B RID: 20235 RVA: 0x001A2212 File Offset: 0x001A0412
	private void Refresh()
	{
		if (this.localPlayerAtDisplay)
		{
			FriendSystem.Instance.RefreshFriendsList();
			this.PopulateLocalPlayerCard();
		}
	}

	// Token: 0x06004F0C RID: 20236 RVA: 0x001A222E File Offset: 0x001A042E
	public void LocalPlayerFullyVisiblePress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Visible);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06004F0D RID: 20237 RVA: 0x001A2249 File Offset: 0x001A0449
	public void LocalPlayerPublicOnlyPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.PublicOnly);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06004F0E RID: 20238 RVA: 0x001A2264 File Offset: 0x001A0464
	public void LocalPlayerFullyHiddenPress()
	{
		FriendSystem.Instance.SetLocalPlayerPrivacy(FriendSystem.PlayerPrivacy.Hidden);
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
	}

	// Token: 0x06004F0F RID: 20239 RVA: 0x001A2280 File Offset: 0x001A0480
	private void UpdateLocalPlayerPrivacyButtons()
	{
		FriendSystem.PlayerPrivacy localPlayerPrivacy = FriendSystem.Instance.LocalPlayerPrivacy;
		this.SetButtonAppearance(this._localPlayerFullyVisibleButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Visible);
		this.SetButtonAppearance(this._localPlayerPublicOnlyButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly);
		this.SetButtonAppearance(this._localPlayerFullyHiddenButton, localPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden);
	}

	// Token: 0x06004F10 RID: 20240 RVA: 0x001A22CC File Offset: 0x001A04CC
	private void UpdatePageButtons(int selectedPage)
	{
		int count = FriendBackendController.Instance.FriendsList.Count;
		bool flag = SubscriptionManager.IsLocalSubscribed();
		int num = 1 + this.freeExtraPageCount;
		int num2 = 9 + this.freeExtraPageCount * 9;
		bool flag2 = this.freeExtraPageCount > 0;
		int num3 = Mathf.Min(this.totalPages, this.PageButtons.Length);
		if (!flag2)
		{
			for (int i = num; i < num3; i++)
			{
				int num4 = i - num;
				bool flag3 = count > num2 + num4 * 9;
				if (flag || flag3)
				{
					flag2 = true;
					break;
				}
			}
		}
		for (int j = 0; j < this.PageButtons.Length; j++)
		{
			bool flag4;
			if (j >= num3)
			{
				flag4 = false;
			}
			else if (j == 0)
			{
				flag4 = flag2;
			}
			else if (j < num)
			{
				flag4 = true;
			}
			else
			{
				int num5 = j - num;
				bool flag5 = count > num2 + num5 * 9;
				flag4 = (flag || flag5);
			}
			if (flag4)
			{
				this.SetPageButtonAppearance(this.PageButtons[j], (j == selectedPage) ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
			}
			else
			{
				this.HidePageButton(this.PageButtons[j]);
			}
		}
	}

	// Token: 0x06004F11 RID: 20241 RVA: 0x001A23D4 File Offset: 0x001A05D4
	private void SetButtonAppearance(MeshRenderer buttonRenderer, bool active)
	{
		this.SetButtonAppearance(buttonRenderer, active ? FriendDisplay.ButtonState.Active : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06004F12 RID: 20242 RVA: 0x001A23E4 File Offset: 0x001A05E4
	private void SetButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		Material[] sharedMaterials;
		switch (state)
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
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
	}

	// Token: 0x06004F13 RID: 20243 RVA: 0x001A243C File Offset: 0x001A063C
	private void ClearPageButtons()
	{
		for (int i = 0; i < this.PageButtons.Length; i++)
		{
			this.HidePageButton(this.PageButtons[i]);
		}
	}

	// Token: 0x06004F14 RID: 20244 RVA: 0x001A246C File Offset: 0x001A066C
	private void HidePageButton(MeshRenderer buttonRenderer)
	{
		buttonRenderer.enabled = false;
		buttonRenderer.GetComponent<BoxCollider>().enabled = false;
		buttonRenderer.transform.localPosition = new Vector3(buttonRenderer.transform.localPosition.x, buttonRenderer.transform.localPosition.y, this.pageButtonInactiveZPos);
	}

	// Token: 0x06004F15 RID: 20245 RVA: 0x001A24C4 File Offset: 0x001A06C4
	private void SetPageButtonAppearance(MeshRenderer buttonRenderer, FriendDisplay.ButtonState state)
	{
		buttonRenderer.enabled = true;
		buttonRenderer.GetComponent<BoxCollider>().enabled = true;
		Material[] sharedMaterials;
		switch (state)
		{
		case FriendDisplay.ButtonState.Default:
			sharedMaterials = this._pageButtonDefaultMaterials;
			break;
		case FriendDisplay.ButtonState.Active:
			sharedMaterials = this._pageButtonActiveMaterials;
			break;
		case FriendDisplay.ButtonState.Alert:
			sharedMaterials = this._pageButtonAlerttMaterials;
			break;
		default:
			throw new ArgumentOutOfRangeException("state", state, null);
		}
		buttonRenderer.sharedMaterials = sharedMaterials;
		Vector3 localPosition = buttonRenderer.transform.localPosition;
		buttonRenderer.transform.localPosition = new Vector3(localPosition.x, localPosition.y, this.pageButtonActiveZPos);
	}

	// Token: 0x06004F16 RID: 20246 RVA: 0x001A255C File Offset: 0x001A075C
	public void ToggleRemoveFriendMode()
	{
		this.inRemoveMode = !this.inRemoveMode;
		FriendCard[] array = this.friendCards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetRemoveEnabled(this.inRemoveMode);
		}
		this.SetButtonAppearance(this._removeFriendButton, this.inRemoveMode ? FriendDisplay.ButtonState.Alert : FriendDisplay.ButtonState.Default);
	}

	// Token: 0x06004F17 RID: 20247 RVA: 0x001A25B4 File Offset: 0x001A07B4
	private void InitFriendCards()
	{
		float num = this.gridWidth / (float)this.gridDimension;
		float num2 = this.gridHeight / (float)this.gridDimension;
		Vector3 right = this.gridRoot.right;
		Vector3 a = -this.gridRoot.up;
		Vector3 a2 = this.gridRoot.position - right * (this.gridWidth * 0.5f - num * 0.5f) - a * (this.gridHeight * 0.5f - num2 * 0.5f);
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < this.gridDimension; i++)
		{
			for (int j = 0; j < this.gridDimension; j++)
			{
				FriendCard friendCard = this.friendCards[num4];
				friendCard.gameObject.SetActive(true);
				friendCard.transform.localScale = Vector3.one * (num / friendCard.Width);
				friendCard.transform.position = a2 + right * num * (float)j + a * num2 * (float)i;
				friendCard.transform.rotation = this.gridRoot.transform.rotation;
				friendCard.Init(this);
				friendCard.SetButton(this._friendCardButtons[num3++], this._buttonDefaultMaterials, this._buttonActiveMaterials, this._buttonAlertMaterials, this._friendCardButtonText[num4]);
				friendCard.SetEmpty();
				num4++;
			}
		}
	}

	// Token: 0x06004F18 RID: 20248 RVA: 0x001A2754 File Offset: 0x001A0954
	public void RandomizeFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].Randomize();
		}
	}

	// Token: 0x06004F19 RID: 20249 RVA: 0x001A2784 File Offset: 0x001A0984
	private void ClearFriendCards()
	{
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
	}

	// Token: 0x06004F1A RID: 20250 RVA: 0x001A27B1 File Offset: 0x001A09B1
	public void OnGetFriendsReceived(List<FriendBackendController.Friend> friendsList)
	{
		this.UpdateLocalPlayerPrivacyButtons();
		this.PopulateLocalPlayerCard();
		this.GoToFriendPage(this._currentPage);
	}

	// Token: 0x06004F1B RID: 20251 RVA: 0x001A27CC File Offset: 0x001A09CC
	public void GoToFriendPage(int currentPage)
	{
		int num = Mathf.Min(this.totalPages, this.PageButtons.Length);
		if (currentPage < 0 || currentPage >= num)
		{
			currentPage = 0;
		}
		this._currentPage = currentPage;
		this.UpdatePageButtons(currentPage);
		for (int i = 0; i < this.friendCards.Length; i++)
		{
			this.friendCards[i].SetEmpty();
		}
		List<FriendBackendController.Friend> friendsList = FriendBackendController.Instance.FriendsList;
		int num2 = currentPage * this.cardsPerPage;
		int num3 = 9 + this.freeExtraPageCount * 9;
		for (int j = 0; j < this.friendCards.Length; j++)
		{
			int num4 = num2 + j;
			bool flag = num4 >= num3;
			if (num4 < friendsList.Count)
			{
				this.friendCards[j].Populate(friendsList[num4], flag);
			}
			else
			{
				this.friendCards[j].SetEmpty(flag);
			}
		}
	}

	// Token: 0x06004F1C RID: 20252 RVA: 0x001A28A9 File Offset: 0x001A0AA9
	private void InitLocalPlayerCard()
	{
		this._localPlayerCard.Init(this);
		this.ClearLocalPlayerCard();
	}

	// Token: 0x06004F1D RID: 20253 RVA: 0x001A28C0 File Offset: 0x001A0AC0
	private void PopulateLocalPlayerCard()
	{
		string zone = PhotonNetworkController.Instance.CurrentRoomZone.GetName<GTZone>().ToUpper();
		this._localPlayerCard.SetName(NetworkSystem.Instance.LocalPlayer.NickName.ToUpper());
		if (!PhotonNetwork.InRoom || string.IsNullOrEmpty(NetworkSystem.Instance.RoomName) || NetworkSystem.Instance.RoomName.Length <= 0)
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		bool flag = NetworkSystem.Instance.RoomName[0] == '@';
		bool flag2 = !NetworkSystem.Instance.SessionIsPrivate;
		if (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.Hidden || (FriendSystem.Instance.LocalPlayerPrivacy == FriendSystem.PlayerPrivacy.PublicOnly && !flag2))
		{
			this._localPlayerCard.SetRoom("OFFLINE");
			this._localPlayerCard.SetZone("");
			return;
		}
		if (flag)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.Substring(1).ToUpper());
			this._localPlayerCard.SetZone("CUSTOM");
			return;
		}
		if (!flag2)
		{
			this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
			this._localPlayerCard.SetZone("PRIVATE");
			return;
		}
		this._localPlayerCard.SetRoom(NetworkSystem.Instance.RoomName.ToUpper());
		this._localPlayerCard.SetZone(zone);
	}

	// Token: 0x06004F1E RID: 20254 RVA: 0x001A2A4D File Offset: 0x001A0C4D
	private void ClearLocalPlayerCard()
	{
		this._localPlayerCard.SetEmpty();
	}

	// Token: 0x06004F1F RID: 20255 RVA: 0x001A2A5C File Offset: 0x001A0C5C
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.white;
		float num = this.gridWidth * 0.5f;
		float num2 = this.gridHeight * 0.5f;
		float num3 = num;
		float num4 = num2;
		Vector3 a = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, num4, 0f);
		Vector3 vector = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, num4, 0f);
		Vector3 vector2 = this.gridRoot.position + this.gridRoot.rotation * new Vector3(-num3, -num4, 0f);
		Vector3 b = this.gridRoot.position + this.gridRoot.rotation * new Vector3(num3, -num4, 0f);
		for (int i = 0; i <= this.gridDimension; i++)
		{
			float t = (float)i / (float)this.gridDimension;
			Vector3 from = Vector3.Lerp(a, vector, t);
			Vector3 to = Vector3.Lerp(vector2, b, t);
			Gizmos.DrawLine(from, to);
			Vector3 from2 = Vector3.Lerp(a, vector2, t);
			Vector3 to2 = Vector3.Lerp(vector, b, t);
			Gizmos.DrawLine(from2, to2);
		}
	}

	// Token: 0x040060C3 RID: 24771
	[FormerlySerializedAs("gridCenter")]
	[SerializeField]
	private FriendCard[] friendCards = new FriendCard[9];

	// Token: 0x040060C4 RID: 24772
	[SerializeField]
	private Transform gridRoot;

	// Token: 0x040060C5 RID: 24773
	[SerializeField]
	private float gridWidth = 2f;

	// Token: 0x040060C6 RID: 24774
	[SerializeField]
	private float gridHeight = 1f;

	// Token: 0x040060C7 RID: 24775
	[SerializeField]
	private int gridDimension = 3;

	// Token: 0x040060C8 RID: 24776
	[SerializeField]
	private TriggerEventNotifier triggerNotifier;

	// Token: 0x040060C9 RID: 24777
	[FormerlySerializedAs("_joinButtons")]
	[Header("Buttons")]
	[SerializeField]
	private GorillaPressableDelayButton[] _friendCardButtons;

	// Token: 0x040060CA RID: 24778
	[SerializeField]
	private TextMeshProUGUI[] _friendCardButtonText;

	// Token: 0x040060CB RID: 24779
	[SerializeField]
	private MeshRenderer _localPlayerFullyVisibleButton;

	// Token: 0x040060CC RID: 24780
	[SerializeField]
	private MeshRenderer _localPlayerPublicOnlyButton;

	// Token: 0x040060CD RID: 24781
	[SerializeField]
	private MeshRenderer _localPlayerFullyHiddenButton;

	// Token: 0x040060CE RID: 24782
	[SerializeField]
	private MeshRenderer _removeFriendButton;

	// Token: 0x040060CF RID: 24783
	[SerializeField]
	private FriendCard _localPlayerCard;

	// Token: 0x040060D0 RID: 24784
	[SerializeField]
	private MeshRenderer[] PageButtons;

	// Token: 0x040060D1 RID: 24785
	[SerializeField]
	private Material[] _buttonDefaultMaterials;

	// Token: 0x040060D2 RID: 24786
	[SerializeField]
	private Material[] _buttonActiveMaterials;

	// Token: 0x040060D3 RID: 24787
	[SerializeField]
	private Material[] _buttonAlertMaterials;

	// Token: 0x040060D4 RID: 24788
	[SerializeField]
	private Material[] _pageButtonDefaultMaterials;

	// Token: 0x040060D5 RID: 24789
	[SerializeField]
	private Material[] _pageButtonActiveMaterials;

	// Token: 0x040060D6 RID: 24790
	[SerializeField]
	private Material[] _pageButtonAlerttMaterials;

	// Token: 0x040060D7 RID: 24791
	public const int PageCapacity = 9;

	// Token: 0x040060D8 RID: 24792
	public const int VIMPageCapacity = 9;

	// Token: 0x040060DB RID: 24795
	[SerializeField]
	private int freeExtraPageCount = 1;

	// Token: 0x040060DC RID: 24796
	[SerializeField]
	private int vimPageCount;

	// Token: 0x040060DD RID: 24797
	private int cardsPerPage = 9;

	// Token: 0x040060DE RID: 24798
	[SerializeField]
	private float pageButtonInactiveZPos;

	// Token: 0x040060DF RID: 24799
	[SerializeField]
	private float pageButtonActiveZPos;

	// Token: 0x040060E0 RID: 24800
	private MeshRenderer[] _joinButtonRenderers;

	// Token: 0x040060E1 RID: 24801
	private bool inRemoveMode;

	// Token: 0x040060E2 RID: 24802
	private bool localPlayerAtDisplay;

	// Token: 0x040060E3 RID: 24803
	private int _currentPage;

	// Token: 0x02000C73 RID: 3187
	public enum ButtonState
	{
		// Token: 0x040060E5 RID: 24805
		Default,
		// Token: 0x040060E6 RID: 24806
		Active,
		// Token: 0x040060E7 RID: 24807
		Alert
	}
}
