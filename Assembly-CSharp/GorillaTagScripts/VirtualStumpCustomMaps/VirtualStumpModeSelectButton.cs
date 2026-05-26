using System;
using GorillaExtensions;
using GorillaGameModes;
using GorillaNetworking;

namespace GorillaTagScripts.VirtualStumpCustomMaps
{
	// Token: 0x02000F50 RID: 3920
	public class VirtualStumpModeSelectButton : ModeSelectButton
	{
		// Token: 0x060061D4 RID: 25044 RVA: 0x001F8F6C File Offset: 0x001F716C
		public override void ButtonActivationWithHand(bool isLeftHand)
		{
			if (this.warningScreen.ShouldShowWarning)
			{
				this.warningScreen.Show();
			}
			else
			{
				GorillaComputer.instance.SetGameModeWithoutButton(this.gameMode);
			}
			if (GorillaComputer.instance.IsPlayerInVirtualStump() && RoomSystem.JoinedRoom && NetworkSystem.Instance.LocalPlayer.IsMasterClient && NetworkSystem.Instance.SessionIsPrivate)
			{
				if (GameMode.ActiveGameMode.IsNull())
				{
					GameMode.ChangeGameMode(this.gameMode);
					return;
				}
				if (GameMode.ActiveGameMode.GameType().ToString().ToLower() != this.gameMode.ToLower())
				{
					GameMode.ChangeGameMode(this.gameMode);
				}
			}
		}
	}
}
