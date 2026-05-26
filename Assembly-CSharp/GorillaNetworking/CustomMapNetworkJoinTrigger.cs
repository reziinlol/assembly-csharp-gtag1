using System;
using GorillaGameModes;

namespace GorillaNetworking
{
	// Token: 0x0200105B RID: 4187
	public class CustomMapNetworkJoinTrigger : GorillaNetworkJoinTrigger
	{
		// Token: 0x06006929 RID: 26921 RVA: 0x00220904 File Offset: 0x0021EB04
		public override string GetFullDesiredGameModeString()
		{
			return new GameModeString
			{
				zone = this.networkZone,
				queue = GorillaComputer.instance.currentQueue,
				gameType = base.GetDesiredGameType(),
				modId = CustomMapLoader.LoadedMapModId.ToString(),
				modFileId = CustomMapLoader.LoadedMapModFileId.ToString()
			}.ToString();
		}

		// Token: 0x0600692A RID: 26922 RVA: 0x00220971 File Offset: 0x0021EB71
		public override byte GetRoomSize(bool subscribed)
		{
			return CustomMapLoader.GetRoomSizeForCurrentlyLoadedMap();
		}
	}
}
