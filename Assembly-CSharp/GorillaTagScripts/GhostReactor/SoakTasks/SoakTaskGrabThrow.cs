using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000F99 RID: 3993
	public sealed class SoakTaskGrabThrow : IGhostReactorSoakTask
	{
		// Token: 0x1700096B RID: 2411
		// (get) Token: 0x06006385 RID: 25477 RVA: 0x00200224 File Offset: 0x001FE424
		// (set) Token: 0x06006386 RID: 25478 RVA: 0x0020022C File Offset: 0x001FE42C
		public bool Complete { get; private set; }

		// Token: 0x06006387 RID: 25479 RVA: 0x00200235 File Offset: 0x001FE435
		public SoakTaskGrabThrow(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x00200244 File Offset: 0x001FE444
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._dropEntityTime == null || this._heldEntityId == null)
			{
				List<GameEntity> list = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>();
				GameEntity gameEntity = null;
				foreach (GameEntity gameEntity2 in list)
				{
					if (!(gameEntity2 == null) && !gameEntity2.IsHeld() && gameEntity2.pickupable && !(gameEntity2.gameObject.GetComponent<GameAgent>() != null))
					{
						gameEntity = gameEntity2;
						break;
					}
				}
				if (gameEntity != null)
				{
					Debug.Log(string.Format("Soak grabbing entity {0}", gameEntity.id.index));
					managerForZone.RequestGrabEntity(gameEntity.id, true, Vector3.zero, Quaternion.identity);
					this._heldEntityId = new GameEntityId?(gameEntity.id);
					this._dropEntityTime = new float?(Time.time + 0.1f);
				}
			}
			else if (this._heldEntityId != null)
			{
				float? dropEntityTime = this._dropEntityTime;
				if (dropEntityTime != null)
				{
					float valueOrDefault = dropEntityTime.GetValueOrDefault();
					if (Time.time >= valueOrDefault)
					{
						Debug.Log(string.Format("Soak dropping entity {0}", this._heldEntityId.Value.index));
						managerForZone.RequestThrowEntity(this._heldEntityId.Value, true, Vector3.zero, Vector3.zero, Vector3.zero);
						this._heldEntityId = null;
						this._dropEntityTime = null;
						this.Complete = true;
					}
				}
			}
			return true;
		}

		// Token: 0x06006389 RID: 25481 RVA: 0x00200418 File Offset: 0x001FE618
		public void Reset()
		{
			this._heldEntityId = null;
			this._dropEntityTime = null;
			this.Complete = false;
		}

		// Token: 0x04007230 RID: 29232
		public const float TIME_TO_HOLD_ENTITY = 0.1f;

		// Token: 0x04007231 RID: 29233
		private readonly GRPlayer _grPlayer;

		// Token: 0x04007232 RID: 29234
		private GameEntityId? _heldEntityId;

		// Token: 0x04007233 RID: 29235
		private float? _dropEntityTime;
	}
}
