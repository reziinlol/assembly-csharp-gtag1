using System;
using Photon.Pun;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class CosmeticCritterCatcherShade : CosmeticCritterCatcher
{
	// Token: 0x17000059 RID: 89
	// (get) Token: 0x060004C6 RID: 1222 RVA: 0x0001A948 File Offset: 0x00018B48
	// (set) Token: 0x060004C7 RID: 1223 RVA: 0x0001A950 File Offset: 0x00018B50
	public Vector3 LastTargetPosition { get; private set; }

	// Token: 0x060004C8 RID: 1224 RVA: 0x0001A959 File Offset: 0x00018B59
	public float GetActionTimeFrac()
	{
		return this.targetHoldTime / this.maxHoldTime;
	}

	// Token: 0x060004C9 RID: 1225 RVA: 0x0001A968 File Offset: 0x00018B68
	protected override CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 0.25f, 0.5f);
	}

	// Token: 0x060004CA RID: 1226 RVA: 0x0001A97C File Offset: 0x00018B7C
	public override CosmeticCritterAction GetLocalCatchAction(CosmeticCritter critter)
	{
		if (this.heartbeatCooldown > 0.5f || (this.currentTarget != null && this.currentTarget != critter))
		{
			return CosmeticCritterAction.None;
		}
		if (critter is CosmeticCritterShadeFleeing && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 0f))
		{
			if (this.targetHoldTime >= this.minSecondsLockedToCatch && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
		else
		{
			if (!(critter is CosmeticCritterShadeHidden) || !this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 0f))
			{
				return CosmeticCritterAction.None;
			}
			if (this.targetHoldTime >= this.secondsToReveal)
			{
				return CosmeticCritterAction.RPC | CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked;
			}
			return CosmeticCritterAction.RPC | CosmeticCritterAction.ShadeHeartbeat;
		}
	}

	// Token: 0x060004CB RID: 1227 RVA: 0x0001AA48 File Offset: 0x00018C48
	public override bool ValidateRemoteCatchAction(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		if (!base.ValidateRemoteCatchAction(critter, catchAction, serverTime))
		{
			return false;
		}
		if (critter is CosmeticCritterShadeFleeing)
		{
			if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None && (critter.transform.position - this.catchOrigin.position).sqrMagnitude <= this.catchRadius * this.catchRadius + 1f && this.targetHoldTime >= this.minSecondsLockedToCatch * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.LOCKED, 2f))
			{
				return true;
			}
		}
		else if (critter is CosmeticCritterShadeHidden)
		{
			if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None && this.targetHoldTime >= this.secondsToReveal * 0.8f)
			{
				return true;
			}
			if ((catchAction & CosmeticCritterAction.ShadeHeartbeat) != CosmeticCritterAction.None && this.shadeRevealer.CritterWithinBeamThreshold(critter, ShadeRevealer.State.TRACKING, 2f))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060004CC RID: 1228 RVA: 0x0001AB1C File Offset: 0x00018D1C
	public override void OnCatch(CosmeticCritter critter, CosmeticCritterAction catchAction, double serverTime)
	{
		this.currentTarget = critter;
		float num = PhotonNetwork.InRoom ? ((float)(PhotonNetwork.Time - serverTime)) : 0f;
		this.heartbeatCooldown = 1f + num;
		this.targetHoldTime += num;
		if (!(critter is CosmeticCritterShadeFleeing))
		{
			if (critter is CosmeticCritterShadeHidden)
			{
				this.maxHoldTime = this.secondsToReveal;
				if ((catchAction & (CosmeticCritterAction.Despawn | CosmeticCritterAction.SpawnLinked)) != CosmeticCritterAction.None)
				{
					(this.optionalLinkedSpawner as CosmeticCritterSpawnerShadeFleeing).SetSpawnPosition(critter.transform.position);
					this.currentTarget = null;
					this.targetHoldTime = 0f;
				}
			}
			return;
		}
		this.maxHoldTime = this.minSecondsLockedToCatch;
		if ((catchAction & CosmeticCritterAction.Despawn) != CosmeticCritterAction.None)
		{
			this.shadeRevealer.ShadeCaught();
			this.currentTarget = null;
			this.targetHoldTime = 0f;
			return;
		}
		CosmeticCritterAction cosmeticCritterAction = catchAction & CosmeticCritterAction.ShadeHeartbeat;
	}

	// Token: 0x060004CD RID: 1229 RVA: 0x0001ABE6 File Offset: 0x00018DE6
	protected override void Awake()
	{
		base.Awake();
		this.shadeRevealer = (this.transferrableObject as ShadeRevealer);
		this.maxHoldTime = Mathf.Max(this.secondsToReveal, this.minSecondsLockedToCatch);
	}

	// Token: 0x060004CE RID: 1230 RVA: 0x0001AC18 File Offset: 0x00018E18
	protected void LateUpdate()
	{
		if (this.heartbeatCooldown > 0f)
		{
			this.heartbeatCooldown -= Time.deltaTime;
			if (this.heartbeatCooldown < 0f)
			{
				this.heartbeatCooldown = 0f;
				this.currentTarget = null;
				return;
			}
			this.targetHoldTime = Mathf.Min(this.targetHoldTime + Time.deltaTime, this.maxHoldTime);
			if (this.currentTarget is CosmeticCritterShadeFleeing)
			{
				if (!base.IsLocal || this.heartbeatCooldown > 0.4f)
				{
					this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.LOCKED);
				}
				Vector3 normalized = (this.catchOrigin.position - this.currentTarget.transform.position).normalized;
				(this.currentTarget as CosmeticCritterShadeFleeing).pullVector += this.vacuumSpeed * Time.deltaTime * normalized;
				return;
			}
			if (this.currentTarget is CosmeticCritterShadeHidden && (!base.IsLocal || this.heartbeatCooldown > 0.4f))
			{
				this.shadeRevealer.SetBestBeamState(ShadeRevealer.State.TRACKING);
				return;
			}
		}
		else if (this.targetHoldTime > 0f)
		{
			this.targetHoldTime = Mathf.Max(this.targetHoldTime - Time.deltaTime, 0f);
		}
	}

	// Token: 0x060004CF RID: 1231 RVA: 0x0001AD61 File Offset: 0x00018F61
	protected override void OnEnable()
	{
		base.OnEnable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x060004D0 RID: 1232 RVA: 0x0001AD86 File Offset: 0x00018F86
	protected override void OnDisable()
	{
		base.OnDisable();
		this.currentTarget = null;
		this.targetHoldTime = 0f;
		this.heartbeatCooldown = 1f;
	}

	// Token: 0x04000549 RID: 1353
	[SerializeField]
	private float secondsToReveal = 1f;

	// Token: 0x0400054A RID: 1354
	[SerializeField]
	private float minSecondsLockedToCatch = 1f;

	// Token: 0x0400054B RID: 1355
	[SerializeField]
	private Transform catchOrigin;

	// Token: 0x0400054C RID: 1356
	[SerializeField]
	private float catchRadius = 1f;

	// Token: 0x0400054D RID: 1357
	[SerializeField]
	private float vacuumSpeed = 3f;

	// Token: 0x0400054E RID: 1358
	private ShadeRevealer shadeRevealer;

	// Token: 0x0400054F RID: 1359
	private CosmeticCritter currentTarget;

	// Token: 0x04000550 RID: 1360
	private float targetHoldTime;

	// Token: 0x04000551 RID: 1361
	private float maxHoldTime;

	// Token: 0x04000553 RID: 1363
	private const float HEARTBEAT_DELAY = 1f;

	// Token: 0x04000554 RID: 1364
	private float heartbeatCooldown;
}
