using System;
using System.Collections.Generic;
using GameObjectScheduling;
using GorillaNetworking;
using UnityEngine;

namespace GorillaGameModes
{
	// Token: 0x02000EAE RID: 3758
	[CreateAssetMenu(fileName = "New Game Mode Zone Map", menuName = "Game Settings/Game Mode Zone Map", order = 2)]
	public class GameModeZoneMapping : ScriptableObject
	{
		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x06005C6E RID: 23662 RVA: 0x001D5155 File Offset: 0x001D3355
		public HashSet<GameModeType> AllModes
		{
			get
			{
				this.Init();
				return this.allModes;
			}
		}

		// Token: 0x06005C6F RID: 23663 RVA: 0x001D5164 File Offset: 0x001D3364
		private void Init()
		{
			if (this.allModes != null)
			{
				return;
			}
			this.allModes = new HashSet<GameModeType>();
			for (int i = 0; i < this.defaultGameModes.Length; i++)
			{
				this.allModes.Add(this.defaultGameModes[i]);
			}
			this.bigRoomZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			this.publicZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			this.privateZoneGameModesLookup = new Dictionary<GTZone, HashSet<GameModeType>>();
			for (int j = 0; j < this.zoneGameModes.Length; j++)
			{
				for (int k = 0; k < this.zoneGameModes[j].zone.Length; k++)
				{
					this.publicZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					for (int l = 0; l < this.zoneGameModes[j].modes.Length; l++)
					{
						if (!this.allModes.Contains(this.zoneGameModes[j].modes[l]))
						{
							this.allModes.Add(this.zoneGameModes[j].modes[l]);
						}
					}
					if (this.zoneGameModes[j].privateModes.Length != 0)
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].privateModes));
						for (int m = 0; m < this.zoneGameModes[j].privateModes.Length; m++)
						{
							if (!this.allModes.Contains(this.zoneGameModes[j].privateModes[m]))
							{
								this.allModes.Add(this.zoneGameModes[j].privateModes[m]);
							}
						}
					}
					else
					{
						this.privateZoneGameModesLookup.Add(this.zoneGameModes[j].zone[k], new HashSet<GameModeType>(this.zoneGameModes[j].modes));
					}
				}
			}
			this.modeNameLookup = new Dictionary<GameModeType, string>();
			for (int n = 0; n < this.gameModeNameOverrides.Length; n++)
			{
				this.modeNameLookup.Add(this.gameModeNameOverrides[n].mode, this.gameModeNameOverrides[n].displayName);
			}
			this.isNewLookup = new HashSet<GameModeType>(this.newThisUpdate);
			this.gameModeTypeCountdownsLookup = new Dictionary<GameModeType, CountdownTextDate>();
			for (int num = 0; num < this.gameModeTypeCountdowns.Length; num++)
			{
				this.gameModeTypeCountdownsLookup.Add(this.gameModeTypeCountdowns[num].mode, this.gameModeTypeCountdowns[num].countdownTextDate);
			}
		}

		// Token: 0x06005C70 RID: 23664 RVA: 0x001D5434 File Offset: 0x001D3634
		public HashSet<GameModeType> GetModesForZone(GTZone zone, bool isPrivate)
		{
			this.Init();
			if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
			{
				return this.privateZoneGameModesLookup[zone];
			}
			if (this.publicZoneGameModesLookup.ContainsKey(zone))
			{
				return this.publicZoneGameModesLookup[zone];
			}
			return new HashSet<GameModeType>(this.defaultGameModes);
		}

		// Token: 0x06005C71 RID: 23665 RVA: 0x001D548C File Offset: 0x001D368C
		public bool IsBigRoomMode(GameModeType gameModeType)
		{
			for (int i = 0; i < this.bigRoomGameModes.Length; i++)
			{
				if (this.bigRoomGameModes[i] == gameModeType)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06005C72 RID: 23666 RVA: 0x001D54BA File Offset: 0x001D36BA
		internal string GetModeName(GameModeType mode)
		{
			this.Init();
			if (this.modeNameLookup.ContainsKey(mode))
			{
				return this.modeNameLookup[mode];
			}
			return mode.ToString().ToUpper();
		}

		// Token: 0x06005C73 RID: 23667 RVA: 0x001D54EF File Offset: 0x001D36EF
		internal bool IsNew(GameModeType mode)
		{
			this.Init();
			return this.isNewLookup.Contains(mode);
		}

		// Token: 0x06005C74 RID: 23668 RVA: 0x001D5503 File Offset: 0x001D3703
		internal CountdownTextDate GetCountdown(GameModeType mode)
		{
			this.Init();
			if (this.gameModeTypeCountdownsLookup.ContainsKey(mode))
			{
				return this.gameModeTypeCountdownsLookup[mode];
			}
			return null;
		}

		// Token: 0x06005C75 RID: 23669 RVA: 0x001D5528 File Offset: 0x001D3728
		internal GameModeType VerifyModeForZone(GTZone zone, GameModeType mode, bool isPrivate)
		{
			if (GorillaComputer.instance.IsPlayerInVirtualStump())
			{
				zone = GTZone.customMaps;
			}
			if (zone == GTZone.none)
			{
				if (this.allModes.Contains(mode))
				{
					return mode;
				}
				return GameModeType.Casual;
			}
			else
			{
				bool flag = PlayerPrefFlags.Check(PlayerPrefFlags.Flag.GAME_MODE_SELECTOR_IS_SUPER);
				if (!flag)
				{
					if (mode == GameModeType.SuperCasual)
					{
						mode = GameModeType.Casual;
					}
					else if (mode == GameModeType.SuperInfect)
					{
						mode = GameModeType.Infection;
					}
				}
				HashSet<GameModeType> hashSet;
				if (isPrivate && this.privateZoneGameModesLookup.ContainsKey(zone))
				{
					hashSet = this.privateZoneGameModesLookup[zone];
				}
				else if (this.publicZoneGameModesLookup.ContainsKey(zone))
				{
					hashSet = this.publicZoneGameModesLookup[zone];
				}
				else
				{
					hashSet = new HashSet<GameModeType>(this.defaultGameModes);
				}
				if (hashSet.Contains(mode))
				{
					return mode;
				}
				GameModeType result = GameModeType.Casual;
				foreach (GameModeType gameModeType in hashSet)
				{
					if (flag || (gameModeType != GameModeType.SuperCasual && gameModeType != GameModeType.SuperInfect))
					{
						return gameModeType;
					}
				}
				return result;
			}
		}

		// Token: 0x04006AC0 RID: 27328
		[SerializeField]
		[TextArea(4, 40)]
		private string notes;

		// Token: 0x04006AC1 RID: 27329
		[SerializeField]
		private GameModeNameOverrides[] gameModeNameOverrides;

		// Token: 0x04006AC2 RID: 27330
		[SerializeField]
		private GameModeType[] defaultGameModes;

		// Token: 0x04006AC3 RID: 27331
		[SerializeField]
		private GameModeType[] bigRoomGameModes;

		// Token: 0x04006AC4 RID: 27332
		[SerializeField]
		private ZoneGameModes[] zoneGameModes;

		// Token: 0x04006AC5 RID: 27333
		[SerializeField]
		private GameModeTypeCountdown[] gameModeTypeCountdowns;

		// Token: 0x04006AC6 RID: 27334
		[SerializeField]
		private GameModeType[] newThisUpdate;

		// Token: 0x04006AC7 RID: 27335
		private Dictionary<GTZone, HashSet<GameModeType>> bigRoomZoneGameModesLookup;

		// Token: 0x04006AC8 RID: 27336
		private Dictionary<GTZone, HashSet<GameModeType>> publicZoneGameModesLookup;

		// Token: 0x04006AC9 RID: 27337
		private Dictionary<GTZone, HashSet<GameModeType>> privateZoneGameModesLookup;

		// Token: 0x04006ACA RID: 27338
		private Dictionary<GameModeType, string> modeNameLookup;

		// Token: 0x04006ACB RID: 27339
		private HashSet<GameModeType> isNewLookup;

		// Token: 0x04006ACC RID: 27340
		private Dictionary<GameModeType, CountdownTextDate> gameModeTypeCountdownsLookup;

		// Token: 0x04006ACD RID: 27341
		private HashSet<GameModeType> allModes;
	}
}
