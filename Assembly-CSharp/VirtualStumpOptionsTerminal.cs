using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GorillaExtensions;
using GorillaTagScripts.VirtualStumpCustomMaps.UI;
using Modio.Customizations;
using Modio.Users;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000AA6 RID: 2726
public class VirtualStumpOptionsTerminal : MonoBehaviour, IWssAuthPrompter
{
	// Token: 0x060045A8 RID: 17832 RVA: 0x00178E18 File Offset: 0x00177018
	public void Start()
	{
		this.optionList.gameObject.SetActive(true);
		this.mainScreenText.gameObject.SetActive(true);
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.AddListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.AddListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoginStarted.AddListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.AddListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserChanged.AddListener(new UnityAction<User>(this.OnModIOUserChanged));
	}

	// Token: 0x060045A9 RID: 17833 RVA: 0x00178ED4 File Offset: 0x001770D4
	public void OnDestroy()
	{
		CustomMapsKeyboard customMapsKeyboard = this.keyboard;
		if (customMapsKeyboard != null)
		{
			customMapsKeyboard.OnKeyPressed.RemoveListener(new UnityAction<CustomMapKeyboardBinding>(this.OnKeyPressed));
		}
		ModIOManager.OnModIOLoggedIn.RemoveListener(new UnityAction(this.OnModIOLoggedIn));
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoginStarted.RemoveListener(new UnityAction(this.OnModIOLoginStarted));
		ModIOManager.OnModIOLoginFailed.RemoveListener(new UnityAction<string>(this.OnModIOLoginFailed));
		ModIOManager.OnModIOUserChanged.RemoveListener(new UnityAction<User>(this.OnModIOUserChanged));
	}

	// Token: 0x060045AA RID: 17834 RVA: 0x00178F71 File Offset: 0x00177171
	public void OnEnable()
	{
		this.RefreshButtonState();
		this.UpdateOptionListForCurrentState();
		this.UpdateScreen();
	}

	// Token: 0x060045AB RID: 17835 RVA: 0x00178F88 File Offset: 0x00177188
	private void OnKeyPressed(CustomMapKeyboardBinding pressedButton)
	{
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.cachedError = null;
			this.RefreshButtonState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.up)
		{
			int num = this.currentState - VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE;
			if (num < 0)
			{
				num = 1;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		if (pressedButton == CustomMapKeyboardBinding.down)
		{
			int num2 = (int)(this.currentState + 1);
			if (num2 >= 2)
			{
				num2 = 0;
			}
			this.ChangeState((VirtualStumpOptionsTerminal.ETerminalState)num2);
			this.UpdateOptionListForCurrentState();
			this.UpdateScreen();
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.OnKeyPressed_ModIOAccount(pressedButton);
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.OnKeyPressed_RoomSize(pressedButton);
	}

	// Token: 0x060045AC RID: 17836 RVA: 0x00179024 File Offset: 0x00177224
	private void ChangeState(VirtualStumpOptionsTerminal.ETerminalState newState)
	{
		if (newState == this.currentState)
		{
			return;
		}
		this.currentState = newState;
		this.RefreshButtonState();
	}

	// Token: 0x060045AD RID: 17837 RVA: 0x00179040 File Offset: 0x00177240
	private void RefreshButtonState()
	{
		for (int i = 0; i < this.contextualButtons.Count; i++)
		{
			if (this.contextualButtons[i].IsNotNull())
			{
				this.contextualButtons[i].SetActive(false);
			}
		}
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.OKButton.SetActive(true);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			for (int j = 0; j < this.buttonsToShow_MODIO.Count; j++)
			{
				if (this.buttonsToShow_MODIO[j].IsNotNull())
				{
					this.buttonsToShow_MODIO[j].SetActive(true);
				}
			}
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		for (int k = 0; k < this.buttonsToShow_ROOMSIZE.Count; k++)
		{
			if (this.buttonsToShow_ROOMSIZE[k].IsNotNull())
			{
				this.buttonsToShow_ROOMSIZE[k].SetActive(true);
			}
		}
	}

	// Token: 0x060045AE RID: 17838 RVA: 0x00179128 File Offset: 0x00177328
	private void UpdateOptionListForCurrentState()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < 2; i++)
		{
			stringBuilder.Append(this.optionStrings[i]);
			if (i == (int)this.currentState)
			{
				stringBuilder.Append(" <-");
			}
			stringBuilder.Append("\n");
		}
		this.optionList.text = stringBuilder.ToString();
	}

	// Token: 0x060045AF RID: 17839 RVA: 0x0017918C File Offset: 0x0017738C
	private void UpdateScreen()
	{
		this.mainScreenText.text = "";
		if (!this.cachedError.IsNullOrEmpty())
		{
			this.RefreshButtonState();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.cachedError);
			TMP_Text tmp_Text = this.mainScreenText;
			string str = "<color=\"red\">";
			StringBuilder stringBuilder2 = stringBuilder;
			tmp_Text.text = str + ((stringBuilder2 != null) ? stringBuilder2.ToString() : null);
			return;
		}
		VirtualStumpOptionsTerminal.ETerminalState eterminalState = this.currentState;
		if (eterminalState == VirtualStumpOptionsTerminal.ETerminalState.MODIO_ACCOUNT)
		{
			this.mainScreenText.text = this.UpdateScreen_ModIOAccount();
			return;
		}
		if (eterminalState != VirtualStumpOptionsTerminal.ETerminalState.ROOM_SIZE)
		{
			return;
		}
		this.mainScreenText.text = this.UpdateScreen_RoomSize();
	}

	// Token: 0x060045B0 RID: 17840 RVA: 0x00179224 File Offset: 0x00177424
	private void OnModIOLoginStarted()
	{
		this.UpdateScreen();
	}

	// Token: 0x060045B1 RID: 17841 RVA: 0x0017922C File Offset: 0x0017742C
	private void OnModIOLoggedIn()
	{
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		ModIOManager.OnModIOLoggedOut.AddListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
		AssociateMotherhsipAndModIOAccountsRequest associateMotherhsipAndModIOAccountsRequest = new AssociateMotherhsipAndModIOAccountsRequest();
		associateMotherhsipAndModIOAccountsRequest.ModIOId = ModIOManager.GetCurrentUserId();
		associateMotherhsipAndModIOAccountsRequest.ModIOToken = ModIOManager.GetCurrentAuthToken();
		associateMotherhsipAndModIOAccountsRequest.MothershipEnvId = MothershipClientApiUnity.EnvironmentId;
		associateMotherhsipAndModIOAccountsRequest.MothershipPlayerId = MothershipClientContext.MothershipId;
		associateMotherhsipAndModIOAccountsRequest.MothershipToken = MothershipClientContext.Token;
		base.StartCoroutine(ModIOManager.AssociateMothershipAndModIOAccounts(associateMotherhsipAndModIOAccountsRequest, delegate(AssociateMotherhsipAndModIOAccountsResponse response)
		{
		}));
	}

	// Token: 0x060045B2 RID: 17842 RVA: 0x001792D9 File Offset: 0x001774D9
	private void OnModIOLoggedOut()
	{
		ModIOManager.OnModIOLoggedOut.RemoveListener(new UnityAction(this.OnModIOLoggedOut));
		this.processingAccountLink = false;
		this.UpdateScreen();
	}

	// Token: 0x060045B3 RID: 17843 RVA: 0x001792FE File Offset: 0x001774FE
	private void OnModIOLoginFailed(string error)
	{
		this.processingAccountLink = false;
		this.cachedError = error;
		this.UpdateScreen();
	}

	// Token: 0x060045B4 RID: 17844 RVA: 0x00179224 File Offset: 0x00177424
	private void OnModIOUserChanged(User user)
	{
		this.UpdateScreen();
	}

	// Token: 0x060045B5 RID: 17845 RVA: 0x00179314 File Offset: 0x00177514
	private void OnKeyPressed_ModIOAccount(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.option1)
		{
			this.StartAccountLinkingProcess();
		}
		if (pressedButton == CustomMapKeyboardBinding.option2)
		{
			GTDev.Log<string>(string.Format("[VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount] logout {0}", ModIOManager.IsLoggedIn()), null);
			if (ModIOManager.IsLoggedIn())
			{
				ModIOManager.LogoutFromModIO();
			}
		}
		if (pressedButton == CustomMapKeyboardBinding.option3)
		{
			GTDev.Log<string>(string.Format("[VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount] login {0}", ModIOManager.IsLoggedIn()), null);
			if (!ModIOManager.IsLoggedIn())
			{
				ModIOManager.CancelExternalAuthentication();
				try
				{
					ModIOManager.RequestPlatformLogin();
				}
				catch (Exception arg)
				{
					GTDev.Log<string>(string.Format("VirtualStumpOptionsTerminal::OnKeyPressed_ModIOAccount platform login error: {0}", arg), null);
					throw;
				}
			}
		}
	}

	// Token: 0x060045B6 RID: 17846 RVA: 0x001793B0 File Offset: 0x001775B0
	private Task StartAccountLinkingProcess()
	{
		VirtualStumpOptionsTerminal.<StartAccountLinkingProcess>d__40 <StartAccountLinkingProcess>d__;
		<StartAccountLinkingProcess>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<StartAccountLinkingProcess>d__.<>4__this = this;
		<StartAccountLinkingProcess>d__.<>1__state = -1;
		<StartAccountLinkingProcess>d__.<>t__builder.Start<VirtualStumpOptionsTerminal.<StartAccountLinkingProcess>d__40>(ref <StartAccountLinkingProcess>d__);
		return <StartAccountLinkingProcess>d__.<>t__builder.Task;
	}

	// Token: 0x060045B7 RID: 17847 RVA: 0x001793F3 File Offset: 0x001775F3
	public void ShowPrompt(string url, string code)
	{
		this.cachedLinkURL = url;
		this.cachedLinkCode = code;
		this.UpdateScreen();
	}

	// Token: 0x060045B8 RID: 17848 RVA: 0x0017940C File Offset: 0x0017760C
	private string UpdateScreen_ModIOAccount()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (ModIOManager.IsLoggedIn())
		{
			stringBuilder.Append(this.loggedInAsString + "\n");
			stringBuilder.Append("   " + ModIOManager.GetCurrentUsername() + "\n\n");
			if (ModIOManager.GetLastAuthMethod() != ModIOManager.ModIOAuthMethod.LinkedAccount)
			{
				stringBuilder.Append(this.linkAccountPromptString + "\n");
			}
			else
			{
				stringBuilder.Append(this.alreadyLinkedAccountString + "\n");
			}
		}
		else if (ModIOManager.IsLoggingIn() && !this.processingAccountLink)
		{
			stringBuilder.Append(this.loggingInString);
		}
		else if (ModIOManager.IsLoggingOut())
		{
			stringBuilder.Append(this.loggingOutString);
		}
		else if (this.processingAccountLink)
		{
			stringBuilder.Append(this.linkAccountPromptString + "\n\n");
			stringBuilder.Append(this.urlLabelString + this.cachedLinkURL + "\n");
			stringBuilder.Append(this.linkCodeLabelString + this.cachedLinkCode + "\n");
		}
		else
		{
			stringBuilder.Append(this.notLoggedInString + "\n\n");
			stringBuilder.Append(this.loginPromptString);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060045B9 RID: 17849 RVA: 0x00179554 File Offset: 0x00177754
	private void OnKeyPressed_RoomSize(CustomMapKeyboardBinding pressedButton)
	{
		if (pressedButton == CustomMapKeyboardBinding.left)
		{
			this.DecrementRoomSize();
		}
		if (pressedButton == CustomMapKeyboardBinding.right)
		{
			this.IncrementRoomSize();
		}
		this.UpdateScreen();
	}

	// Token: 0x060045BA RID: 17850 RVA: 0x00179572 File Offset: 0x00177772
	private void DecrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() - 1);
		this.UpdateScreen();
	}

	// Token: 0x060045BB RID: 17851 RVA: 0x00179587 File Offset: 0x00177787
	private void IncrementRoomSize()
	{
		RoomSystem.OverrideRoomSize(RoomSystem.GetOverridenRoomSize() + 1);
		this.UpdateScreen();
	}

	// Token: 0x060045BC RID: 17852 RVA: 0x0017959C File Offset: 0x0017779C
	private string UpdateScreen_RoomSize()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(this.roomSizeDescriptionString + "\n\n");
		stringBuilder.Append(this.roomSizeLabelString + RoomSystem.GetOverridenRoomSize().ToString());
		return stringBuilder.ToString();
	}

	// Token: 0x0400581A RID: 22554
	[SerializeField]
	private TMP_Text optionList;

	// Token: 0x0400581B RID: 22555
	[SerializeField]
	private TMP_Text mainScreenText;

	// Token: 0x0400581C RID: 22556
	[SerializeField]
	private CustomMapsKeyboard keyboard;

	// Token: 0x0400581D RID: 22557
	[SerializeField]
	private List<string> optionStrings = new List<string>
	{
		"MOD.IO",
		"ROOM SIZE"
	};

	// Token: 0x0400581E RID: 22558
	[SerializeField]
	private string loggedInAsString = "LOGGED INTO MOD.IO AS: ";

	// Token: 0x0400581F RID: 22559
	[SerializeField]
	private string notLoggedInString = "LOGGED OUT OF MOD.IO";

	// Token: 0x04005820 RID: 22560
	[SerializeField]
	private string loginPromptString = "PRESS THE 'PLATFORM LOGIN' OR 'LINK MOD.IO ACCOUNT' BUTTON TO LOGIN";

	// Token: 0x04005821 RID: 22561
	[SerializeField]
	private string loggingInString = "LOGGING IN TO MOD.IO...";

	// Token: 0x04005822 RID: 22562
	[SerializeField]
	private string loggingOutString = "LOGGING OUT OF MOD.IO...";

	// Token: 0x04005823 RID: 22563
	[SerializeField]
	private string linkAccountPromptString = "IF YOU HAVE AN EXISTING MOD.IO ACCOUNT, YOU CAN LINK IT BY PRESSING THE 'LINK MOD.IO ACCOUNT' BUTTON.";

	// Token: 0x04005824 RID: 22564
	[SerializeField]
	private string alreadyLinkedAccountString = "YOU'VE ALREADY LINKED YOUR MOD.IO ACCOUNT.";

	// Token: 0x04005825 RID: 22565
	[SerializeField]
	private string accountLinkingPromptString = "PLEASE GO TO THIS URL IN YOUR BROWSER AND LOG IN TO YOUR MOD.IO ACCOUNT. ONCE LOGGED IN, ENTER THE FOLLOWING CODE TO PROCEED: ";

	// Token: 0x04005826 RID: 22566
	[SerializeField]
	private string urlLabelString = "URL: ";

	// Token: 0x04005827 RID: 22567
	[SerializeField]
	private string linkCodeLabelString = "CODE: ";

	// Token: 0x04005828 RID: 22568
	[SerializeField]
	private string roomSizeDescriptionString = "THIS SETTING WILL CHANGE THE MAXIMUM AMOUNT OF PLAYERS ALLOWED IN PRIVATE ROOMS YOU CREATE. WHEN JOINING A PUBLIC ROOM, THE MAP YOU'VE LOADED WILL CONTROL THE ROOM SIZE.";

	// Token: 0x04005829 RID: 22569
	[SerializeField]
	private string roomSizeLabelString = "MAX PLAYERS: ";

	// Token: 0x0400582A RID: 22570
	[SerializeField]
	private GameObject OKButton;

	// Token: 0x0400582B RID: 22571
	[SerializeField]
	private List<GameObject> contextualButtons = new List<GameObject>();

	// Token: 0x0400582C RID: 22572
	[SerializeField]
	private List<GameObject> buttonsToShow_MODIO = new List<GameObject>();

	// Token: 0x0400582D RID: 22573
	[SerializeField]
	private List<GameObject> buttonsToShow_ROOMSIZE = new List<GameObject>();

	// Token: 0x0400582E RID: 22574
	private bool processingAccountLink;

	// Token: 0x0400582F RID: 22575
	private string cachedLinkURL = "";

	// Token: 0x04005830 RID: 22576
	private string cachedLinkCode = "";

	// Token: 0x04005831 RID: 22577
	private string cachedError;

	// Token: 0x04005832 RID: 22578
	private VirtualStumpOptionsTerminal.ETerminalState currentState;

	// Token: 0x02000AA7 RID: 2727
	private enum ETerminalState
	{
		// Token: 0x04005834 RID: 22580
		MODIO_ACCOUNT,
		// Token: 0x04005835 RID: 22581
		ROOM_SIZE,
		// Token: 0x04005836 RID: 22582
		NUM_STATES
	}
}
