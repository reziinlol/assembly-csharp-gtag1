using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000F98 RID: 3992
	public sealed class SoakTaskDepositCollectibles : IGhostReactorSoakTask
	{
		// Token: 0x1700096A RID: 2410
		// (get) Token: 0x06006380 RID: 25472 RVA: 0x001FFF08 File Offset: 0x001FE108
		// (set) Token: 0x06006381 RID: 25473 RVA: 0x001FFF10 File Offset: 0x001FE110
		public bool Complete { get; private set; }

		// Token: 0x06006382 RID: 25474 RVA: 0x001FFF19 File Offset: 0x001FE119
		public SoakTaskDepositCollectibles(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x06006383 RID: 25475 RVA: 0x001FFF28 File Offset: 0x001FE128
		public bool Update()
		{
			if (this._coreDepositor == null)
			{
				GhostReactor instance = GhostReactor.instance;
				if (instance != null)
				{
					this._coreDepositor = instance.currencyDepositor;
				}
				if (this._coreDepositor == null)
				{
					return false;
				}
			}
			if (this._seedExtractorTriggerLocation == null)
			{
				GhostReactor instance2 = GhostReactor.instance;
				if (instance2 != null)
				{
					this._seedExtractorTriggerLocation = new Vector3?(instance2.seedExtractor.transform.Find("DepositorTrigger").position);
				}
				if (this._seedExtractorTriggerLocation == null)
				{
					return false;
				}
			}
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._heldEntity == null || this._depositCollectibleTime == null)
			{
				List<GameEntity> list = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>();
				GameEntity gameEntity = null;
				foreach (GameEntity gameEntity2 in list)
				{
					if (!(gameEntity2 == null) && !gameEntity2.IsHeld())
					{
						GRCollectible component = gameEntity2.gameObject.GetComponent<GRCollectible>();
						if (component != null && (component.type == ProgressionManager.CoreType.Core || component.type == ProgressionManager.CoreType.SuperCore || component.type == ProgressionManager.CoreType.ChaosSeed))
						{
							gameEntity = gameEntity2;
							break;
						}
					}
				}
				if (gameEntity != null)
				{
					Debug.Log(string.Format("Soak grabbing core {0}", gameEntity.id.index));
					managerForZone.RequestGrabEntity(gameEntity.id, true, Vector3.zero, Quaternion.identity);
					this._heldEntity = gameEntity;
					this._depositCollectibleTime = new float?(Time.time + 0.1f);
				}
			}
			else if (this._heldEntity != null)
			{
				float? depositCollectibleTime = this._depositCollectibleTime;
				if (depositCollectibleTime != null)
				{
					float valueOrDefault = depositCollectibleTime.GetValueOrDefault();
					if (Time.time >= valueOrDefault)
					{
						GRCollectible component2 = this._heldEntity.GetComponent<GRCollectible>();
						if (component2 == null)
						{
							return false;
						}
						ProgressionManager.CoreType type = component2.type;
						if (type - ProgressionManager.CoreType.Core > 1)
						{
							if (type == ProgressionManager.CoreType.ChaosSeed)
							{
								Debug.Log(string.Format("Soak depositing chaos seed {0}", this._heldEntity.id.index));
								this._heldEntity.gameObject.transform.position = this._seedExtractorTriggerLocation.Value;
							}
						}
						else
						{
							Debug.Log(string.Format("Soak depositing core {0}", this._heldEntity.id.index));
							this._heldEntity.gameObject.transform.position = this._coreDepositor.gameObject.transform.position;
						}
						this._heldEntity = null;
						this._depositCollectibleTime = null;
						this.Complete = true;
					}
				}
			}
			return true;
		}

		// Token: 0x06006384 RID: 25476 RVA: 0x00200208 File Offset: 0x001FE408
		public void Reset()
		{
			this._heldEntity = null;
			this._depositCollectibleTime = null;
			this.Complete = false;
		}

		// Token: 0x04007229 RID: 29225
		public const float TIME_TO_HOLD_COLLECTIBLE = 0.1f;

		// Token: 0x0400722A RID: 29226
		private readonly GRPlayer _grPlayer;

		// Token: 0x0400722B RID: 29227
		private GRCurrencyDepositor _coreDepositor;

		// Token: 0x0400722C RID: 29228
		private Vector3? _seedExtractorTriggerLocation;

		// Token: 0x0400722D RID: 29229
		private GameEntity _heldEntity;

		// Token: 0x0400722E RID: 29230
		private float? _depositCollectibleTime;
	}
}
