using System;
using GorillaNetworking;
using GorillaTagScripts.VirtualStumpCustomMaps;
using Modio.Mods;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AA2 RID: 2722
public class CustomMapsTerminal : MonoBehaviour
{
	// Token: 0x17000665 RID: 1637
	// (get) Token: 0x06004582 RID: 17794 RVA: 0x0017803E File Offset: 0x0017623E
	public static int LocalPlayerID
	{
		get
		{
			return NetworkSystem.Instance.LocalPlayer.ActorNumber;
		}
	}

	// Token: 0x17000666 RID: 1638
	// (get) Token: 0x06004583 RID: 17795 RVA: 0x0017804F File Offset: 0x0017624F
	public static long LocalModDetailsID
	{
		get
		{
			return CustomMapsTerminal.localModDetailsID;
		}
	}

	// Token: 0x17000667 RID: 1639
	// (get) Token: 0x06004584 RID: 17796 RVA: 0x00178056 File Offset: 0x00176256
	public static int CurrentScreen
	{
		get
		{
			return (int)CustomMapsTerminal.localCurrentScreen;
		}
	}

	// Token: 0x17000668 RID: 1640
	// (get) Token: 0x06004585 RID: 17797 RVA: 0x0017805D File Offset: 0x0017625D
	public static bool IsDriver
	{
		get
		{
			return CustomMapsTerminal.localDriverID == CustomMapsTerminal.LocalPlayerID;
		}
	}

	// Token: 0x06004586 RID: 17798 RVA: 0x0017806B File Offset: 0x0017626B
	private void Awake()
	{
		CustomMapsTerminal.instance = this;
		CustomMapsTerminal.hasInstance = true;
	}

	// Token: 0x06004587 RID: 17799 RVA: 0x0017807C File Offset: 0x0017627C
	private void Start()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
		this.controlAccessScreen.Show();
		this.detailsAccessScreen.Show();
		this.modListScreen.Hide();
		this.modDetailsScreen.Hide();
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted += this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer += this.OnReturnedToSinglePlayer;
	}

	// Token: 0x06004588 RID: 17800 RVA: 0x00178138 File Offset: 0x00176338
	private void OnDestroy()
	{
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		NetworkSystem.Instance.OnMultiplayerStarted -= this.OnJoinedRoom;
		NetworkSystem.Instance.OnReturnedToSinglePlayer -= this.OnReturnedToSinglePlayer;
	}

	// Token: 0x06004589 RID: 17801 RVA: 0x001781B4 File Offset: 0x001763B4
	public static void ShowDetailsScreen(Mod mod)
	{
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		CustomMapsTerminal.localModDetailsID = mod.Id;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Hide();
		CustomMapsTerminal.instance.detailsAccessScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.SetModProfile(mod);
		CustomMapsTerminal.instance.modDisplayScreen.Show();
		CustomMapsTerminal.instance.modDisplayScreen.SetModProfile(mod);
		CustomMapsTerminal.instance.modSearchScreen.Hide();
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x0600458A RID: 17802 RVA: 0x00178260 File Offset: 0x00176460
	public static void ReturnFromDetailsScreen()
	{
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.previousScreen;
		if (screenType == CustomMapsTerminal.ScreenType.ModDetails || screenType == CustomMapsTerminal.ScreenType.Invalid || screenType == CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
		}
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Show();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.SearchMods:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.ReturnFromDetailsScreen();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x0600458B RID: 17803 RVA: 0x001783DC File Offset: 0x001765DC
	public static void ShowSearchScreen()
	{
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.SearchMods;
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Hide();
		CustomMapsTerminal.instance.detailsAccessScreen.SetDetailsScreenForDriver();
		CustomMapsTerminal.instance.detailsAccessScreen.Show();
		CustomMapsTerminal.instance.modDetailsScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Hide();
		CustomMapsTerminal.instance.modSearchScreen.Show();
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x0600458C RID: 17804 RVA: 0x00178468 File Offset: 0x00176668
	public static void ReturnFromSearchScreen()
	{
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.previousScreen;
		if (screenType == CustomMapsTerminal.ScreenType.ModDetails || screenType == CustomMapsTerminal.ScreenType.Invalid || screenType == CustomMapsTerminal.ScreenType.TerminalControlPrompt || screenType == CustomMapsTerminal.ScreenType.SearchMods)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
		}
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			CustomMapsTerminal.instance.modListScreen.Hide();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Show();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			CustomMapsTerminal.instance.modListScreen.Show();
			CustomMapsTerminal.instance.modSearchScreen.Hide();
			CustomMapsTerminal.instance.modDetailsScreen.Hide();
			CustomMapsTerminal.instance.modDisplayScreen.Hide();
			CustomMapsTerminal.instance.controlAccessScreen.Hide();
			CustomMapsTerminal.instance.detailsAccessScreen.Show();
			break;
		}
		CustomMapsTerminal.SendTerminalStatus();
	}

	// Token: 0x0600458D RID: 17805 RVA: 0x00178586 File Offset: 0x00176786
	public static void SendTerminalStatus()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.instance.mapTerminalNetworkObject.SendTerminalStatus();
	}

	// Token: 0x0600458E RID: 17806 RVA: 0x0017859F File Offset: 0x0017679F
	public static void ResetTerminalControl()
	{
		CustomMapsTerminal.localDriverID = -2;
		CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
		CustomMapsTerminal.ShowTerminalControlScreen();
	}

	// Token: 0x0600458F RID: 17807 RVA: 0x001785BC File Offset: 0x001767BC
	public static void HandleTerminalControlStatusChangeRequest(bool lockedStatus, int playerID)
	{
		if (lockedStatus && playerID == -2)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			if (!lockedStatus)
			{
				return;
			}
		}
		else if (CustomMapsTerminal.localDriverID != playerID)
		{
			return;
		}
		CustomMapsTerminal.SetTerminalControlStatus(lockedStatus, playerID, true);
	}

	// Token: 0x06004590 RID: 17808 RVA: 0x001785E8 File Offset: 0x001767E8
	public static void SetTerminalControlStatus(bool isLocked, int driverID = -2, bool sendRPC = false)
	{
		GTDev.Log<string>(string.Format("[CustomMapsTerminal::SetTerminalControlStatus] isLocked: {0} | driverID: {1} | playerId {2} | sendRPC: {3}", new object[]
		{
			isLocked,
			driverID,
			CustomMapsTerminal.LocalPlayerID,
			sendRPC
		}), null);
		if (isLocked)
		{
			CustomMapsTerminal.localDriverID = driverID;
			CustomMapsTerminal.instance.terminalControlButton.LockTerminalControl();
			if (CustomMapsTerminal.IsDriver)
			{
				CustomMapsTerminal.HideTerminalControlScreens();
			}
			else
			{
				CustomMapsTerminal.ShowTerminalControlScreen();
			}
		}
		else
		{
			CustomMapsTerminal.localDriverID = -2;
			CustomMapsTerminal.instance.terminalControlButton.UnlockTerminalControl();
			CustomMapsTerminal.ShowTerminalControlScreen();
		}
		if (sendRPC && NetworkSystem.Instance.IsMasterClient)
		{
			CustomMapsTerminal.instance.mapTerminalNetworkObject.SetTerminalControlStatus(isLocked, CustomMapsTerminal.localDriverID);
		}
	}

	// Token: 0x06004591 RID: 17809 RVA: 0x001786A0 File Offset: 0x001768A0
	public static void UpdateFromDriver(int currentScreen, long modDetailsID, int driverID)
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		CustomMapsTerminal.localDriverID = driverID;
		CustomMapsTerminal.cachedModDetailsID = modDetailsID;
		CustomMapsTerminal.localModDetailsID = modDetailsID;
		CustomMapsTerminal.cachedCurrentScreen = (CustomMapsTerminal.ScreenType)currentScreen;
		CustomMapsTerminal.localCurrentScreen = (CustomMapsTerminal.ScreenType)currentScreen;
		Debug.Log(string.Format("[CustomMapsTerminal::UpdateFromDriver] currentScreen {0} modDetailsID {1}", CustomMapsTerminal.localCurrentScreen, CustomMapsTerminal.localModDetailsID));
		if (CustomMapsTerminal.localDriverID != -2)
		{
			CustomMapsTerminal.RefreshDriverNickName();
		}
		CustomMapsTerminal.ScreenType screenType = CustomMapsTerminal.localCurrentScreen;
		if (screenType <= CustomMapsTerminal.ScreenType.SearchMods)
		{
			CustomMapsTerminal.ShowTerminalControlScreen();
			return;
		}
		if (screenType != CustomMapsTerminal.ScreenType.ModDetails)
		{
			return;
		}
		CustomMapsTerminal.ShowTerminalControlScreen();
		if (CustomMapsTerminal.localModDetailsID <= 0L)
		{
			return;
		}
		CustomMapsTerminal.instance.detailsAccessScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Show();
		CustomMapsTerminal.instance.modDisplayScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
	}

	// Token: 0x06004592 RID: 17810 RVA: 0x0017875C File Offset: 0x0017695C
	private void UpdateControlScreenForDriver()
	{
		GTDev.Log<string>(string.Format("[CustomMapsTerminal::UpdateScreenToMatchStatus] driverID: {0} ", CustomMapsTerminal.localDriverID) + string.Format("| currentScreen: {0} ", CustomMapsTerminal.localCurrentScreen) + string.Format("| previousScreen: {0} ", CustomMapsTerminal.previousScreen), null);
		switch (CustomMapsTerminal.localCurrentScreen)
		{
		case CustomMapsTerminal.ScreenType.TerminalControlPrompt:
			return;
		case CustomMapsTerminal.ScreenType.AvailableMods:
		case CustomMapsTerminal.ScreenType.InstalledMods:
		case CustomMapsTerminal.ScreenType.FavoriteMods:
		case CustomMapsTerminal.ScreenType.SubscribedMods:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Hide();
			this.detailsAccessScreen.SetDetailsScreenForDriver();
			this.detailsAccessScreen.Show();
			this.modListScreen.Show();
			this.modDetailsScreen.Hide();
			this.modDisplayScreen.Hide();
			return;
		case CustomMapsTerminal.ScreenType.SearchMods:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Show();
			this.detailsAccessScreen.SetDetailsScreenForDriver();
			this.detailsAccessScreen.Show();
			this.modListScreen.Hide();
			this.modDetailsScreen.Hide();
			this.modDisplayScreen.Hide();
			return;
		case CustomMapsTerminal.ScreenType.ModDetails:
			this.controlAccessScreen.Hide();
			this.modSearchScreen.Hide();
			this.detailsAccessScreen.Hide();
			this.modListScreen.Hide();
			this.modDetailsScreen.Show();
			this.modDetailsScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
			this.modDisplayScreen.Show();
			this.modDisplayScreen.RetrieveModFromModIO(CustomMapsTerminal.localModDetailsID, false, null);
			return;
		default:
			return;
		}
	}

	// Token: 0x06004593 RID: 17811 RVA: 0x001788DC File Offset: 0x00176ADC
	private void ValidateLocalStatus()
	{
		if (CustomMapsTerminal.localDriverID == -2)
		{
			return;
		}
		if (CustomMapLoader.IsMapLoaded())
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapLoader.LoadedMapModId;
			CustomMapsTerminal.SendTerminalStatus();
			return;
		}
		if (CustomMapManager.IsLoading())
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapManager.LoadingMapId;
			CustomMapsTerminal.SendTerminalStatus();
			return;
		}
		if (CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
			CustomMapsTerminal.localModDetailsID = CustomMapManager.GetRoomMapId()._id;
			CustomMapsTerminal.SendTerminalStatus();
		}
	}

	// Token: 0x06004594 RID: 17812 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnModIOLoggedIn()
	{
	}

	// Token: 0x06004595 RID: 17813 RVA: 0x0017895D File Offset: 0x00176B5D
	private void OnModIOLoggedOut()
	{
		if (CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
		{
			if (this.modListScreen.isActiveAndEnabled)
			{
				this.modListScreen.SwapListDisplay(CustomMapsListScreen.ListScreenState.AvailableMods, false);
			}
			else
			{
				CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			}
		}
		if (CustomMapsTerminal.previousScreen == CustomMapsTerminal.ScreenType.SubscribedMods)
		{
			CustomMapsTerminal.previousScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
	}

	// Token: 0x06004596 RID: 17814 RVA: 0x00178998 File Offset: 0x00176B98
	public void HandleTerminalControlButtonPressed()
	{
		if (!NetworkSystem.Instance.InRoom)
		{
			CustomMapsTerminal.SetTerminalControlStatus(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID, false);
			return;
		}
		if (CustomMapsTerminal.localDriverID != -2 && !CustomMapsTerminal.IsDriver)
		{
			return;
		}
		if (this.mapTerminalNetworkObject.HasAuthority)
		{
			CustomMapsTerminal.HandleTerminalControlStatusChangeRequest(!this.terminalControlButton.IsLocked, CustomMapsTerminal.LocalPlayerID);
			return;
		}
		this.mapTerminalNetworkObject.RequestTerminalControlStatusChange(!this.terminalControlButton.IsLocked);
	}

	// Token: 0x06004597 RID: 17815 RVA: 0x00178A1C File Offset: 0x00176C1C
	private static void ShowTerminalControlScreen()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (CustomMapsTerminal.localDriverID == -2)
		{
			CustomMapsTerminal.instance.controlAccessScreen.Reset();
			CustomMapsTerminal.instance.detailsAccessScreen.Reset();
		}
		else
		{
			CustomMapsTerminal.instance.controlAccessScreen.SetDriverName();
			CustomMapsTerminal.instance.detailsAccessScreen.SetDriverName();
		}
		CustomMapsTerminal.instance.modListScreen.Hide();
		CustomMapsTerminal.instance.modDetailsScreen.Hide();
		CustomMapsTerminal.instance.modDisplayScreen.Hide();
		CustomMapsTerminal.instance.controlAccessScreen.Show();
		CustomMapsTerminal.instance.detailsAccessScreen.Show();
		CustomMapsTerminal.instance.modSearchScreen.Hide();
		CustomMapsTerminal.previousScreen = CustomMapsTerminal.localCurrentScreen;
		CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.TerminalControlPrompt;
	}

	// Token: 0x06004598 RID: 17816 RVA: 0x00178AE4 File Offset: 0x00176CE4
	private static void HideTerminalControlScreens()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (CustomMapsTerminal.localCurrentScreen != CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			return;
		}
		if (CustomMapsTerminal.previousScreen > CustomMapsTerminal.ScreenType.TerminalControlPrompt)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.previousScreen;
			if ((CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.SubscribedMods || CustomMapsTerminal.localCurrentScreen == CustomMapsTerminal.ScreenType.FavoriteMods) && !ModIOManager.IsLoggedIn())
			{
				CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
			}
		}
		else if (CustomMapLoader.IsMapLoaded() || CustomMapManager.IsLoading() || CustomMapManager.GetRoomMapId() != ModId.Null)
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.ModDetails;
		}
		else
		{
			CustomMapsTerminal.localCurrentScreen = CustomMapsTerminal.ScreenType.AvailableMods;
		}
		CustomMapsTerminal.instance.UpdateControlScreenForDriver();
	}

	// Token: 0x06004599 RID: 17817 RVA: 0x00178B69 File Offset: 0x00176D69
	public static void RequestDriverNickNameRefresh()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		if (!CustomMapsTerminal.IsDriver)
		{
			return;
		}
		CustomMapsTerminal.RefreshDriverNickName();
		CustomMapsTerminal.instance.mapTerminalNetworkObject.RefreshDriverNickName();
	}

	// Token: 0x0600459A RID: 17818 RVA: 0x00178B90 File Offset: 0x00176D90
	public static void RefreshDriverNickName()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return;
		}
		bool flag = KIDManager.HasPermissionToUseFeature(EKIDFeatures.Custom_Nametags);
		CustomMapsTerminal.instance.terminalControllerLabelText.gameObject.SetActive(true);
		if (NetworkSystem.Instance.InRoom)
		{
			NetPlayer netPlayerByID = NetworkSystem.Instance.GetNetPlayerByID(CustomMapsTerminal.localDriverID);
			CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.DefaultName;
			if (GorillaComputer.instance.NametagsEnabled && flag)
			{
				RigContainer rigContainer;
				if (netPlayerByID.IsLocal)
				{
					CustomMapsTerminal.instance.terminalControllerText.text = netPlayerByID.NickName;
				}
				else if (VRRigCache.Instance.TryGetVrrig(netPlayerByID, out rigContainer))
				{
					CustomMapsTerminal.instance.terminalControllerText.text = rigContainer.Rig.playerNameVisible;
				}
			}
		}
		else
		{
			CustomMapsTerminal.instance.terminalControllerText.text = ((GorillaComputer.instance.NametagsEnabled && flag) ? NetworkSystem.Instance.LocalPlayer.NickName : NetworkSystem.Instance.LocalPlayer.DefaultName);
		}
		CustomMapsTerminal.instance.terminalControllerText.gameObject.SetActive(true);
		CustomMapsTerminal.instance.modListScreen.RefreshDriverNickname(CustomMapsTerminal.instance.terminalControllerText.text);
	}

	// Token: 0x0600459B RID: 17819 RVA: 0x00178CC4 File Offset: 0x00176EC4
	private void OnReturnedToSinglePlayer()
	{
		if (CustomMapsTerminal.localDriverID != CustomMapsTerminal.cachedLocalPlayerID)
		{
			CustomMapsTerminal.ResetTerminalControl();
		}
		else
		{
			CustomMapsTerminal.localDriverID = CustomMapsTerminal.LocalPlayerID;
		}
		CustomMapsTerminal.cachedLocalPlayerID = -1;
	}

	// Token: 0x0600459C RID: 17820 RVA: 0x00178CE9 File Offset: 0x00176EE9
	private void OnJoinedRoom()
	{
		CustomMapsTerminal.cachedLocalPlayerID = CustomMapsTerminal.LocalPlayerID;
		CustomMapsTerminal.ResetTerminalControl();
	}

	// Token: 0x0600459D RID: 17821 RVA: 0x00178CFA File Offset: 0x00176EFA
	public static bool IsLocked()
	{
		return CustomMapsTerminal.localDriverID != -2;
	}

	// Token: 0x0600459E RID: 17822 RVA: 0x00178D08 File Offset: 0x00176F08
	public static int GetDriverID()
	{
		return CustomMapsTerminal.localDriverID;
	}

	// Token: 0x0600459F RID: 17823 RVA: 0x00178D0F File Offset: 0x00176F0F
	public static string GetDriverNickname()
	{
		if (!CustomMapsTerminal.hasInstance)
		{
			return "";
		}
		return CustomMapsTerminal.instance.terminalControllerText.text;
	}

	// Token: 0x040057FA RID: 22522
	[SerializeField]
	private CustomMapsAccessScreen controlAccessScreen;

	// Token: 0x040057FB RID: 22523
	[SerializeField]
	private CustomMapsAccessScreen detailsAccessScreen;

	// Token: 0x040057FC RID: 22524
	[SerializeField]
	private CustomMapsListScreen modListScreen;

	// Token: 0x040057FD RID: 22525
	[SerializeField]
	private CustomMapsDetailsScreen modDetailsScreen;

	// Token: 0x040057FE RID: 22526
	[SerializeField]
	private CustomMapsDisplayScreen modDisplayScreen;

	// Token: 0x040057FF RID: 22527
	[SerializeField]
	private CustomMapsSearchScreen modSearchScreen;

	// Token: 0x04005800 RID: 22528
	[SerializeField]
	private VirtualStumpSerializer mapTerminalNetworkObject;

	// Token: 0x04005801 RID: 22529
	[SerializeField]
	private CustomMapsTerminalControlButton terminalControlButton;

	// Token: 0x04005802 RID: 22530
	[SerializeField]
	private TMP_Text terminalControllerLabelText;

	// Token: 0x04005803 RID: 22531
	[SerializeField]
	private TMP_Text terminalControllerText;

	// Token: 0x04005804 RID: 22532
	public const int NO_DRIVER_ID = -2;

	// Token: 0x04005805 RID: 22533
	private static CustomMapsTerminal instance;

	// Token: 0x04005806 RID: 22534
	private static bool hasInstance;

	// Token: 0x04005807 RID: 22535
	private static long localModDetailsID = -1L;

	// Token: 0x04005808 RID: 22536
	private static long cachedModDetailsID = -1L;

	// Token: 0x04005809 RID: 22537
	private static int localDriverID = -1;

	// Token: 0x0400580A RID: 22538
	private static int cachedLocalPlayerID = -1;

	// Token: 0x0400580B RID: 22539
	private static CustomMapsTerminal.ScreenType localCurrentScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x0400580C RID: 22540
	private static CustomMapsTerminal.ScreenType cachedCurrentScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x0400580D RID: 22541
	private static CustomMapsTerminal.ScreenType previousScreen = CustomMapsTerminal.ScreenType.Invalid;

	// Token: 0x02000AA3 RID: 2723
	public enum ScreenType
	{
		// Token: 0x0400580F RID: 22543
		Invalid = -1,
		// Token: 0x04005810 RID: 22544
		TerminalControlPrompt,
		// Token: 0x04005811 RID: 22545
		AvailableMods,
		// Token: 0x04005812 RID: 22546
		InstalledMods,
		// Token: 0x04005813 RID: 22547
		FavoriteMods,
		// Token: 0x04005814 RID: 22548
		SubscribedMods,
		// Token: 0x04005815 RID: 22549
		SearchMods,
		// Token: 0x04005816 RID: 22550
		ModDetails
	}
}
