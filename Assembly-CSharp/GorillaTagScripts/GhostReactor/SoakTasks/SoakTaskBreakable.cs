using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000F97 RID: 3991
	public sealed class SoakTaskBreakable : IGhostReactorSoakTask
	{
		// Token: 0x17000969 RID: 2409
		// (get) Token: 0x0600637B RID: 25467 RVA: 0x001FFCC3 File Offset: 0x001FDEC3
		// (set) Token: 0x0600637C RID: 25468 RVA: 0x001FFCCB File Offset: 0x001FDECB
		public bool Complete { get; private set; }

		// Token: 0x0600637D RID: 25469 RVA: 0x001FFCD4 File Offset: 0x001FDED4
		public SoakTaskBreakable(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x0600637E RID: 25470 RVA: 0x001FFCE4 File Offset: 0x001FDEE4
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._breakable != null && this._breakable.GetComponent<GRBreakable>().BrokenLocal)
			{
				Debug.Log(string.Format("soak breakable {0} is broken", this._breakable.id.index));
				this._breakable = null;
				this._nextHitTime = null;
				this.Complete = true;
			}
			else
			{
				if (this._breakable == null)
				{
					using (List<GameEntity>.Enumerator enumerator = managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							GameEntity gameEntity = enumerator.Current;
							if (!(gameEntity == null) && !gameEntity.IsHeld())
							{
								GRBreakable component = gameEntity.gameObject.GetComponent<GRBreakable>();
								if (component != null && !component.BrokenLocal)
								{
									this._breakable = gameEntity;
									this._nextHitTime = new float?(Time.time + 0.1f);
									break;
								}
							}
						}
						return true;
					}
				}
				if (this._breakable != null)
				{
					float? nextHitTime = this._nextHitTime;
					if (nextHitTime != null)
					{
						float valueOrDefault = nextHitTime.GetValueOrDefault();
						if (Time.time >= valueOrDefault)
						{
							Debug.Log(string.Format("soak hit breakable {0}", this._breakable.id.index));
							GameHitData hit = new GameHitData
							{
								hitEntityId = this._breakable.id,
								hitByEntityId = this._breakable.id,
								hitTypeId = 0,
								hitEntityPosition = Vector3.zero,
								hitPosition = Vector3.zero,
								hitImpulse = Vector3.zero,
								hitAmount = 1
							};
							managerForZone.RequestHit(hit);
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600637F RID: 25471 RVA: 0x001FFEEC File Offset: 0x001FE0EC
		public void Reset()
		{
			this._breakable = null;
			this._nextHitTime = null;
			this.Complete = false;
		}

		// Token: 0x04007224 RID: 29220
		public const float TIME_BETWEEN_HITS = 0.1f;

		// Token: 0x04007225 RID: 29221
		private readonly GRPlayer _grPlayer;

		// Token: 0x04007226 RID: 29222
		private GameEntity _breakable;

		// Token: 0x04007227 RID: 29223
		private float? _nextHitTime;
	}
}
