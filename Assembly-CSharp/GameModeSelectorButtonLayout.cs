using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GorillaGameModes;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020000A3 RID: 163
public class GameModeSelectorButtonLayout : MonoBehaviour
{
	// Token: 0x06000402 RID: 1026 RVA: 0x00017B0C File Offset: 0x00015D0C
	private void OnEnable()
	{
		this.SetupButtons();
		NetworkSystem.Instance.OnJoinedRoomEvent += this.SetupButtons;
		if (this.superToggleButton != null)
		{
			this.superToggleButton.onPressed += this._OnPressedSuperToggleButton;
		}
	}

	// Token: 0x06000403 RID: 1027 RVA: 0x00017B68 File Offset: 0x00015D68
	private void OnDisable()
	{
		NetworkSystem.Instance.OnJoinedRoomEvent -= this.SetupButtons;
		if (this.superToggleButton != null)
		{
			this.superToggleButton.onPressed -= this._OnPressedSuperToggleButton;
		}
	}

	// Token: 0x06000404 RID: 1028 RVA: 0x00017BBC File Offset: 0x00015DBC
	protected virtual void SetupButtons()
	{
		GameModeSelectorButtonLayout.<SetupButtons>d__9 <SetupButtons>d__;
		<SetupButtons>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<SetupButtons>d__.<>4__this = this;
		<SetupButtons>d__.<>1__state = -1;
		<SetupButtons>d__.<>t__builder.Start<GameModeSelectorButtonLayout.<SetupButtons>d__9>(ref <SetupButtons>d__);
	}

	// Token: 0x06000405 RID: 1029 RVA: 0x00017BF4 File Offset: 0x00015DF4
	private void _OnPressedSuperToggleButton(GorillaPressableButton btn, bool isLeftHandPress)
	{
		if (GorillaComputer.instance == null)
		{
			Debug.Log("[GT/GameModeSelectorButtonLayout]  Tried pressing SUPER button but `GorillaComputer` is not ready.", this);
			return;
		}
		if (NetworkSystem.Instance == null)
		{
			Debug.Log("[GT/GameModeSelectorButtonLayout]  Tried pressing SUPER button but `NetworkSystem` is not ready.", this);
			return;
		}
		btn.isOn = !btn.isOn;
		PlayerPrefFlags.Set(PlayerPrefFlags.Flag.GAME_MODE_SELECTOR_IS_SUPER, btn.isOn);
		this.SetupButtons();
		HashSet<GameModeType> modesForZone = GameMode.GameModeZoneMapping.GetModesForZone(this.zone, NetworkSystem.Instance.SessionIsPrivate);
		GameModeType lastPressedGameModeType = GorillaComputer.instance.lastPressedGameModeType;
		GameModeType gameModeType;
		if ((lastPressedGameModeType == GameModeType.Casual || lastPressedGameModeType == GameModeType.SuperCasual) && modesForZone.Contains(GameModeType.Casual) && modesForZone.Contains(GameModeType.SuperCasual))
		{
			gameModeType = (btn.isOn ? GameModeType.SuperCasual : GameModeType.Casual);
		}
		else if ((lastPressedGameModeType == GameModeType.Infection || lastPressedGameModeType == GameModeType.SuperInfect) && modesForZone.Contains(GameModeType.Infection) && modesForZone.Contains(GameModeType.SuperInfect))
		{
			gameModeType = (btn.isOn ? GameModeType.SuperInfect : GameModeType.Infection);
		}
		else
		{
			gameModeType = lastPressedGameModeType;
		}
		GorillaComputer.instance.OnModeSelectButtonPress(gameModeType.ToString(), isLeftHandPress);
	}

	// Token: 0x0400046B RID: 1131
	private const string preLog = "[GT/GameModeSelectorButtonLayout]  ";

	// Token: 0x0400046C RID: 1132
	private const string preErr = "ERROR!!!  ";

	// Token: 0x0400046D RID: 1133
	[SerializeField]
	protected GorillaPressableButton superToggleButton;

	// Token: 0x0400046E RID: 1134
	[SerializeField]
	protected ModeSelectButton pf_button;

	// Token: 0x0400046F RID: 1135
	[SerializeField]
	protected GTZone zone;

	// Token: 0x04000470 RID: 1136
	[SerializeField]
	protected PartyGameModeWarning warningScreen;

	// Token: 0x04000471 RID: 1137
	protected List<ModeSelectButton> currentButtons = new List<ModeSelectButton>();
}
