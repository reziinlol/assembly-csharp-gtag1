using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000739 RID: 1849
[Serializable]
public class GRAbilitySummon : GRAbilityBase
{
	// Token: 0x06002EF6 RID: 12022 RVA: 0x000FF2B0 File Offset: 0x000FD4B0
	public override void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		base.Setup(agent, anim, audioSource, root, head, lineOfSight);
	}

	// Token: 0x06002EF7 RID: 12023 RVA: 0x00100650 File Offset: 0x000FE850
	protected override void OnStart()
	{
		this.lastAnimIndex = AbilityHelperFunctions.RandomRangeUnique(0, this.animData.Count, this.lastAnimIndex);
		this.duration = this.animData[this.lastAnimIndex].duration;
		this.chargeTime = this.animData[this.lastAnimIndex].eventTime;
		this.PlayAnim(this.animData[this.lastAnimIndex].animName, 0.1f, this.animSpeed);
		this.state = GRAbilitySummon.State.Charge;
		this.summonSound.Play(this.audioSource);
		this.spawnedCount = 0;
		this.agent.SetStopped(true);
		this.agent.SetSpeed(1f);
		if (this.fxStartSummon != null)
		{
			this.fxStartSummon.SetActive(false);
			this.fxStartSummon.SetActive(true);
		}
	}

	// Token: 0x06002EF8 RID: 12024 RVA: 0x0010073A File Offset: 0x000FE93A
	protected override void OnStop()
	{
		this.lookAtTarget = null;
		this.agent.SetStopped(false);
	}

	// Token: 0x06002EF9 RID: 12025 RVA: 0x0010074F File Offset: 0x000FE94F
	public void SetLookAtTarget(Transform transform)
	{
		this.lookAtTarget = transform;
	}

	// Token: 0x06002EFA RID: 12026 RVA: 0x00100758 File Offset: 0x000FE958
	protected override void OnThink(float dt)
	{
		this.UpdateState(dt);
	}

	// Token: 0x06002EFB RID: 12027 RVA: 0x00100761 File Offset: 0x000FE961
	protected override void OnUpdateShared(float dt)
	{
		if (this.lookAtTarget != null)
		{
			GameAgent.UpdateFacingTarget(this.root, this.agent.navAgent, this.lookAtTarget, 360f);
		}
	}

	// Token: 0x06002EFC RID: 12028 RVA: 0x00100794 File Offset: 0x000FE994
	private void UpdateState(float dt)
	{
		double num = Time.timeAsDouble - this.startTime;
		switch (this.state)
		{
		case GRAbilitySummon.State.Charge:
			if (num > (double)this.chargeTime)
			{
				this.SetState(GRAbilitySummon.State.Spawn);
				return;
			}
			break;
		case GRAbilitySummon.State.Spawn:
			if (!this.spawned)
			{
				this.spawned = this.DoSpawn();
			}
			if (this.spawned && num > (double)this.duration)
			{
				this.SetState(GRAbilitySummon.State.Done);
				this.spawned = false;
			}
			break;
		case GRAbilitySummon.State.Done:
			break;
		default:
			return;
		}
	}

	// Token: 0x06002EFD RID: 12029 RVA: 0x0010080E File Offset: 0x000FEA0E
	private void SetState(GRAbilitySummon.State newState)
	{
		GRAbilitySummon.State state = this.state;
		this.state = newState;
		switch (newState)
		{
		default:
			return;
		}
	}

	// Token: 0x06002EFE RID: 12030 RVA: 0x00100830 File Offset: 0x000FEA30
	private Vector3? GetSpawnLocation()
	{
		if (this.summonMarkers != null && this.summonMarkers.Count > 0)
		{
			int index = Random.Range(0, this.summonMarkers.Count);
			if (this.summonMarkers[index] != null)
			{
				return new Vector3?(this.summonMarkers[index].transform.position);
			}
		}
		Vector3 position = this.root.position;
		float num = Random.Range(-this.summonConeAngle / 2f, this.summonConeAngle / 2f);
		int i = 0;
		while (i < 5)
		{
			Vector3 a = Quaternion.Euler(0f, num, 0f) * this.root.forward;
			Vector3 vector = position + a * this.desiredSpawnDistance;
			NavMeshHit navMeshHit;
			if (!NavMesh.Raycast(position, vector, out navMeshHit, this.walkableArea))
			{
				goto IL_126;
			}
			if (navMeshHit.distance >= this.minSpawnDistance)
			{
				vector = navMeshHit.position + Vector3.up * this.spawnHeight;
				goto IL_126;
			}
			num += 15f;
			if (num > this.summonConeAngle / 2f)
			{
				this.summonConeAngle = -this.summonConeAngle / 2f;
			}
			IL_151:
			i++;
			continue;
			IL_126:
			RaycastHit raycastHit;
			if (!Physics.Raycast(vector, Vector3.down, out raycastHit) || raycastHit.collider.gameObject.GetComponent<GRHazardousMaterial>() == null)
			{
				return new Vector3?(vector);
			}
			goto IL_151;
		}
		return null;
	}

	// Token: 0x06002EFF RID: 12031 RVA: 0x001009A3 File Offset: 0x000FEBA3
	public bool ForceSpawn()
	{
		return this.DoSpawn();
	}

	// Token: 0x06002F00 RID: 12032 RVA: 0x001009AC File Offset: 0x000FEBAC
	private bool DoSpawn()
	{
		Vector3? spawnLocation = this.GetSpawnLocation();
		if (spawnLocation != null)
		{
			if (this.entity.IsAuthority())
			{
				Quaternion identity = Quaternion.identity;
				GhostReactorManager.Get(this.entity).gameEntityManager.RequestCreateItem(this.entityPrefabToSpawn.name.GetStaticHash(), spawnLocation.Value, identity, 0L, this.entity.id);
				this.spawnedCount++;
			}
			if (this.audioSource != null)
			{
				this.audioSource.PlayOneShot(this.summonSpawnAudioClip);
			}
			if (this.fxOnSpawn != null)
			{
				this.fxOnSpawn.SetActive(false);
				this.fxOnSpawn.SetActive(true);
			}
			return true;
		}
		return false;
	}

	// Token: 0x06002F01 RID: 12033 RVA: 0x00100A70 File Offset: 0x000FEC70
	public override bool IsDone()
	{
		return this.state == GRAbilitySummon.State.Done;
	}

	// Token: 0x06002F02 RID: 12034 RVA: 0x00100A7B File Offset: 0x000FEC7B
	public override bool IsCoolDownOver()
	{
		return base.IsCoolDownOver(this.coolDown);
	}

	// Token: 0x06002F03 RID: 12035 RVA: 0x00100A89 File Offset: 0x000FEC89
	public override float GetRange()
	{
		return this.range;
	}

	// Token: 0x04003C32 RID: 15410
	private int lastAnimIndex = -1;

	// Token: 0x04003C33 RID: 15411
	public GameEntity entityPrefabToSpawn;

	// Token: 0x04003C34 RID: 15412
	public List<AnimationData> animData;

	// Token: 0x04003C35 RID: 15413
	private float animSpeed = 1f;

	// Token: 0x04003C36 RID: 15414
	public float coolDown;

	// Token: 0x04003C37 RID: 15415
	public float range;

	// Token: 0x04003C38 RID: 15416
	public float chargeTime = 3f;

	// Token: 0x04003C39 RID: 15417
	public float duration = 3f;

	// Token: 0x04003C3A RID: 15418
	public float desiredSpawnDistance = 3f;

	// Token: 0x04003C3B RID: 15419
	public float minSpawnDistance = 1f;

	// Token: 0x04003C3C RID: 15420
	public float spawnHeight = 1f;

	// Token: 0x04003C3D RID: 15421
	public float summonConeAngle = 120f;

	// Token: 0x04003C3E RID: 15422
	private bool spawned;

	// Token: 0x04003C3F RID: 15423
	public AudioClip summonSpawnAudioClip;

	// Token: 0x04003C40 RID: 15424
	public GameObject fxStartSummon;

	// Token: 0x04003C41 RID: 15425
	public GameObject fxOnSpawn;

	// Token: 0x04003C42 RID: 15426
	public AbilitySound summonSound;

	// Token: 0x04003C43 RID: 15427
	private int spawnedCount;

	// Token: 0x04003C44 RID: 15428
	public Transform lookAtTarget;

	// Token: 0x04003C45 RID: 15429
	public List<GRAbilitySummon.SummonMarker> summonMarkers;

	// Token: 0x04003C46 RID: 15430
	private GRAbilitySummon.State state;

	// Token: 0x0200073A RID: 1850
	[Serializable]
	public class SummonMarker
	{
		// Token: 0x04003C47 RID: 15431
		public Transform transform;
	}

	// Token: 0x0200073B RID: 1851
	private enum State
	{
		// Token: 0x04003C49 RID: 15433
		Charge,
		// Token: 0x04003C4A RID: 15434
		Spawn,
		// Token: 0x04003C4B RID: 15435
		Done
	}
}
