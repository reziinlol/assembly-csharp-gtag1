using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

// Token: 0x020007D7 RID: 2007
[Serializable]
public class GRSenseNearby
{
	// Token: 0x170004A0 RID: 1184
	// (get) Token: 0x0600333D RID: 13117 RVA: 0x0011A011 File Offset: 0x00118211
	private bool BossEntityPresent
	{
		get
		{
			return GhostReactorManager.Get(this._entity).GetBossEntity() != null;
		}
	}

	// Token: 0x0600333E RID: 13118 RVA: 0x0011A029 File Offset: 0x00118229
	public void Setup(Transform headTransform, GameEntity entity)
	{
		this.rigsNearby = new List<VRRig>();
		this.headTransform = headTransform;
		this._entity = entity;
	}

	// Token: 0x0600333F RID: 13119 RVA: 0x0011A044 File Offset: 0x00118244
	public void OnHitByPlayer(int hitByActorId)
	{
		GRPlayer grplayer = GRPlayer.Get(hitByActorId);
		if (grplayer != null)
		{
			VRRig rig = grplayer.gamePlayer.rig;
			if (!this.rigsNearby.Contains(rig))
			{
				this.rigsNearby.Add(rig);
			}
		}
	}

	// Token: 0x06003340 RID: 13120 RVA: 0x0011A088 File Offset: 0x00118288
	public void UpdateNearby(List<VRRig> allRigs, GRSenseLineOfSight senseLineOfSight)
	{
		Vector3 position = this.headTransform.position;
		Vector3 forward = this.headTransform.rotation * Vector3.forward;
		this.RemoveNotNearby(position);
		this.AddNearby(position, forward, allRigs);
		this.RemoveNoLineOfSight(position, senseLineOfSight);
	}

	// Token: 0x06003341 RID: 13121 RVA: 0x0011A0CF File Offset: 0x001182CF
	public bool IsAnyoneNearby()
	{
		return !GhostReactorManager.AggroDisabled && this.rigsNearby != null && this.rigsNearby.Count > 0;
	}

	// Token: 0x06003342 RID: 13122 RVA: 0x0011A0F0 File Offset: 0x001182F0
	public bool IsAnyoneNearby(float range, bool ignoreBossEntity = false)
	{
		if (!ignoreBossEntity && this.BossEntityPresent && this.rigsNearby.Count > 0)
		{
			return true;
		}
		if (!this.IsAnyoneNearby())
		{
			return false;
		}
		Vector3 position = this.headTransform.position;
		float num = range * range;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			if (!(this.rigsNearby[i] == null) && (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude <= num)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06003343 RID: 13123 RVA: 0x0011A181 File Offset: 0x00118381
	public static Vector3 GetRigTestLocation(VRRig rig)
	{
		return rig.transform.position;
	}

	// Token: 0x06003344 RID: 13124 RVA: 0x0011A190 File Offset: 0x00118390
	public void AddNearby(Vector3 position, Vector3 forward, List<VRRig> allRigs)
	{
		if (this.BossEntityPresent)
		{
			foreach (VRRig item in allRigs)
			{
				if (!this.rigsNearby.Contains(item))
				{
					this.rigsNearby.Add(item);
				}
			}
			return;
		}
		float num = this.range * this.range;
		float num2 = Mathf.Cos(this.fov * 0.017453292f);
		for (int i = 0; i < allRigs.Count; i++)
		{
			VRRig vrrig = allRigs[i];
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if (component.State != GRPlayer.GRPlayerState.Ghost && !component.InStealthMode && !this.rigsNearby.Contains(vrrig))
			{
				Vector3 a = GRSenseNearby.GetRigTestLocation(vrrig) - position;
				float sqrMagnitude = a.sqrMagnitude;
				float num3 = this.hearingRange * this.hearingRange;
				if (sqrMagnitude >= num3)
				{
					if (sqrMagnitude >= num)
					{
						goto IL_116;
					}
					if (sqrMagnitude > 0f)
					{
						float d = Mathf.Sqrt(sqrMagnitude);
						if (Vector3.Dot(a / d, forward) < num2)
						{
							goto IL_116;
						}
					}
				}
				this.rigsNearby.Add(vrrig);
			}
			IL_116:;
		}
	}

	// Token: 0x06003345 RID: 13125 RVA: 0x0011A2D8 File Offset: 0x001184D8
	public void RemoveNotNearby(Vector3 position)
	{
		if (this.BossEntityPresent)
		{
			return;
		}
		float num = this.exitRange * this.exitRange;
		int i = 0;
		while (i < this.rigsNearby.Count)
		{
			VRRig vrrig = this.rigsNearby[i];
			if (!(vrrig != null))
			{
				goto IL_61;
			}
			GRPlayer component = vrrig.GetComponent<GRPlayer>();
			if ((GRSenseNearby.GetRigTestLocation(vrrig) - position).sqrMagnitude > num || component.State == GRPlayer.GRPlayerState.Ghost || component.InStealthMode)
			{
				goto IL_61;
			}
			IL_71:
			i++;
			continue;
			IL_61:
			this.rigsNearby.RemoveAt(i);
			i--;
			goto IL_71;
		}
	}

	// Token: 0x06003346 RID: 13126 RVA: 0x0011A368 File Offset: 0x00118568
	public void RemoveNoLineOfSight(Vector3 headPos, GRSenseLineOfSight senseLineOfSight)
	{
		if (this.BossEntityPresent)
		{
			return;
		}
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			Vector3 rigTestLocation = GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]);
			if (!senseLineOfSight.HasLineOfSight(headPos, rigTestLocation))
			{
				this.rigsNearby.RemoveAt(i);
				i--;
			}
		}
	}

	// Token: 0x06003347 RID: 13127 RVA: 0x0011A3C0 File Offset: 0x001185C0
	public VRRig PickClosest(out float outDistanceSq)
	{
		Vector3 position = this.headTransform.position;
		float num = float.MaxValue;
		VRRig result = null;
		for (int i = 0; i < this.rigsNearby.Count; i++)
		{
			float sqrMagnitude = (GRSenseNearby.GetRigTestLocation(this.rigsNearby[i]) - position).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = this.rigsNearby[i];
			}
		}
		outDistanceSq = num;
		return result;
	}

	// Token: 0x040042DA RID: 17114
	public float range;

	// Token: 0x040042DB RID: 17115
	public float hearingRange;

	// Token: 0x040042DC RID: 17116
	public float exitRange;

	// Token: 0x040042DD RID: 17117
	public float fov;

	// Token: 0x040042DE RID: 17118
	[ReadOnly]
	public List<VRRig> rigsNearby;

	// Token: 0x040042DF RID: 17119
	private Transform headTransform;

	// Token: 0x040042E0 RID: 17120
	private GameEntity _entity;
}
