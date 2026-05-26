using System;
using System.Collections.Generic;
using GorillaExtensions;
using UnityEngine;

namespace GorillaTagScripts.GhostReactor.SoakTasks
{
	// Token: 0x02000F9A RID: 3994
	public sealed class SoakTaskHitEnemy : IGhostReactorSoakTask
	{
		// Token: 0x1700096C RID: 2412
		// (get) Token: 0x0600638A RID: 25482 RVA: 0x00200439 File Offset: 0x001FE639
		// (set) Token: 0x0600638B RID: 25483 RVA: 0x00200441 File Offset: 0x001FE641
		public bool Complete { get; private set; }

		// Token: 0x0600638C RID: 25484 RVA: 0x0020044A File Offset: 0x001FE64A
		public SoakTaskHitEnemy(GRPlayer grPlayer)
		{
			this._grPlayer = grPlayer;
		}

		// Token: 0x0600638D RID: 25485 RVA: 0x0020045C File Offset: 0x001FE65C
		public bool Update()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return false;
			}
			if (this._enemy != null && !SoakTaskHitEnemy.IsLivingEnemy(this._enemy))
			{
				Debug.Log(string.Format("soak enemy {0} is dead", this._enemy.id.index));
				this.Complete = true;
				return true;
			}
			if (this._enemy == null)
			{
				foreach (GameEntity gameEntity in managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>())
				{
					if (!(gameEntity == null) && !gameEntity.IsHeld() && !(gameEntity.GetComponent<GameAgent>() == null) && !(gameEntity.GetComponent<GameHittable>() == null) && SoakTaskHitEnemy.IsEnemy(gameEntity))
					{
						this._enemy = gameEntity;
						this._nextHitTime = new float?(Time.time + 0.1f);
						break;
					}
				}
				return this._enemy != null;
			}
			if (this._nextHitTime == null)
			{
				throw new Exception("Invalid state in HitEnemySoakTask.");
			}
			if (Time.time < this._nextHitTime.Value)
			{
				return true;
			}
			Debug.Log(string.Format("soak hitting enemy {0}", this._enemy.id.index));
			GameEntity randomTool = this.GetRandomTool();
			if (randomTool == null)
			{
				Debug.LogError("No club found for soak task hit enemy.");
				return false;
			}
			GameHitData hit = new GameHitData
			{
				hitEntityId = this._enemy.id,
				hitByEntityId = randomTool.id,
				hitTypeId = 0,
				hitEntityPosition = Vector3.zero,
				hitPosition = Vector3.zero,
				hitImpulse = Vector3.zero,
				hitAmount = 1
			};
			managerForZone.RequestHit(hit);
			this._nextHitTime = new float?(Time.time + 0.1f);
			return true;
		}

		// Token: 0x0600638E RID: 25486 RVA: 0x0020067C File Offset: 0x001FE87C
		public void Reset()
		{
			this._enemy = null;
			this._nextHitTime = null;
			this.Complete = false;
		}

		// Token: 0x0600638F RID: 25487 RVA: 0x00200698 File Offset: 0x001FE898
		private GameEntity GetRandomTool()
		{
			GameEntityManager managerForZone = GameEntityManager.GetManagerForZone(this._grPlayer.gamePlayer.rig.zoneEntity.currentZone);
			if (managerForZone == null)
			{
				return null;
			}
			foreach (GameEntity gameEntity in managerForZone.GetGameEntities().ShuffleIntoCollection<List<GameEntity>, GameEntity>())
			{
				if (!(gameEntity == null))
				{
					GRTool component = gameEntity.GetComponent<GRTool>();
					if (component != null)
					{
						GRTool.GRToolType toolType = component.toolType;
						if (toolType == GRTool.GRToolType.Club || toolType == GRTool.GRToolType.HockeyStick)
						{
							return gameEntity;
						}
					}
				}
			}
			return null;
		}

		// Token: 0x06006390 RID: 25488 RVA: 0x00200744 File Offset: 0x001FE944
		private static bool IsEnemy(GameEntity entity)
		{
			return entity.GetComponent<GREnemyChaser>() != null || entity.GetComponent<GREnemyPest>() != null || entity.GetComponent<GREnemyRanged>() != null || entity.GetComponent<GREnemySummoner>() != null || entity.GetComponent<GREnemyMonkeye>() != null;
		}

		// Token: 0x06006391 RID: 25489 RVA: 0x00200798 File Offset: 0x001FE998
		private static bool IsLivingEnemy(GameEntity entity)
		{
			if (SoakTaskHitEnemy.IsEnemy(entity))
			{
				GREnemyChaser component = entity.GetComponent<GREnemyChaser>();
				if (component == null || component.hp <= 0)
				{
					GREnemyPest component2 = entity.GetComponent<GREnemyPest>();
					if (component2 == null || component2.hp <= 0)
					{
						GREnemyRanged component3 = entity.GetComponent<GREnemyRanged>();
						if (component3 == null || component3.hp <= 0)
						{
							GREnemySummoner component4 = entity.GetComponent<GREnemySummoner>();
							if (component4 == null || component4.hp <= 0)
							{
								GREnemyMonkeye component5 = entity.GetComponent<GREnemyMonkeye>();
								return component5 != null && component5.hp > 0;
							}
						}
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x04007235 RID: 29237
		public const float TIME_BETWEEN_HITS = 0.1f;

		// Token: 0x04007236 RID: 29238
		private readonly GRPlayer _grPlayer;

		// Token: 0x04007237 RID: 29239
		private GameEntity _enemy;

		// Token: 0x04007238 RID: 29240
		private float? _nextHitTime;
	}
}
