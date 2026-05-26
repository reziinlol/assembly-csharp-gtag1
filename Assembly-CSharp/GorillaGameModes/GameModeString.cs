using System;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000EAA RID: 3754
	public class GameModeString
	{
		// Token: 0x06005C2D RID: 23597 RVA: 0x001D4418 File Offset: 0x001D2618
		public override string ToString()
		{
			return string.Concat(new string[]
			{
				this.zone,
				";",
				this.queue,
				";",
				this.gameType,
				";",
				this.modId,
				";",
				this.modFileId
			});
		}

		// Token: 0x06005C2E RID: 23598 RVA: 0x001D4480 File Offset: 0x001D2680
		public static GameModeString FromString(string gameModeString)
		{
			string[] array = gameModeString.Split(";", StringSplitOptions.None);
			if (array.Length != 5)
			{
				Debug.LogError("[GameModeString::FromString] Invalid game mode string: " + gameModeString);
				return null;
			}
			return new GameModeString
			{
				zone = array[0],
				queue = array[1],
				gameType = array[2],
				modId = array[3],
				modFileId = array[4]
			};
		}

		// Token: 0x06005C2F RID: 23599 RVA: 0x001D44E4 File Offset: 0x001D26E4
		public static bool DoesPropertyStringContainGameMode(string propertyString, string gameMode)
		{
			return GameModeString.GameTypeFromPropertyString(propertyString).Equals(gameMode, StringComparison.Ordinal);
		}

		// Token: 0x06005C30 RID: 23600 RVA: 0x001D44F8 File Offset: 0x001D26F8
		public static ReadOnlySpan<char> GameTypeFromPropertyString(string propertyString)
		{
			if (string.IsNullOrEmpty(propertyString))
			{
				return null;
			}
			int num = propertyString.IndexOf(';');
			if (num < 0)
			{
				return null;
			}
			num = propertyString.IndexOf(';', num + 1);
			if (num < 0)
			{
				return null;
			}
			int num2 = propertyString.IndexOf(';', ++num);
			if (num2 < 0)
			{
				return null;
			}
			return propertyString.AsSpan(num, num2 - num);
		}

		// Token: 0x04006AA7 RID: 27303
		public string zone;

		// Token: 0x04006AA8 RID: 27304
		public string queue;

		// Token: 0x04006AA9 RID: 27305
		public string gameType;

		// Token: 0x04006AAA RID: 27306
		public string modId;

		// Token: 0x04006AAB RID: 27307
		public string modFileId;
	}
}
