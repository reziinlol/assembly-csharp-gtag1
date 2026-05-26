using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x0200071E RID: 1822
public class GRAbilityBase
{
	// Token: 0x06002E2F RID: 11823 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnStart()
	{
	}

	// Token: 0x06002E30 RID: 11824 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnStop()
	{
	}

	// Token: 0x06002E31 RID: 11825 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnThink(float dt)
	{
	}

	// Token: 0x06002E32 RID: 11826 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnUpdateShared(float dt)
	{
	}

	// Token: 0x06002E33 RID: 11827 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnUpdateRemote(float dt)
	{
	}

	// Token: 0x06002E34 RID: 11828 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnUpdateAuthority(float dt)
	{
	}

	// Token: 0x06002E35 RID: 11829 RVA: 0x00023994 File Offset: 0x00021B94
	public virtual bool IsCoolDownOver()
	{
		return true;
	}

	// Token: 0x06002E36 RID: 11830 RVA: 0x000FD030 File Offset: 0x000FB230
	public virtual void Setup(GameAgent agent, Animation anim, AudioSource audioSource, Transform root, Transform head, GRSenseLineOfSight lineOfSight)
	{
		this.root = root;
		this.anim = anim;
		if (anim == null)
		{
			this.animator = null;
		}
		this.agent = agent;
		this.head = head;
		this.audioSource = audioSource;
		this.lineOfSight = lineOfSight;
		this.rb = agent.GetComponent<Rigidbody>();
		this.entity = agent.GetComponent<GameEntity>();
		this.attributes = agent.GetComponent<GRAttributes>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	// Token: 0x06002E37 RID: 11831 RVA: 0x000FD0AE File Offset: 0x000FB2AE
	public void Start()
	{
		this.startTime = Time.timeAsDouble;
		this.OnStart();
	}

	// Token: 0x06002E38 RID: 11832 RVA: 0x000FD0C1 File Offset: 0x000FB2C1
	public void Stop()
	{
		this.stopTime = Time.timeAsDouble;
		this.OnStop();
	}

	// Token: 0x06002E39 RID: 11833 RVA: 0x000FD0D4 File Offset: 0x000FB2D4
	public float GetAbilityTime(double currTime)
	{
		return (float)(currTime - this.startTime);
	}

	// Token: 0x06002E3A RID: 11834 RVA: 0x00002076 File Offset: 0x00000276
	public virtual bool IsDone()
	{
		return false;
	}

	// Token: 0x06002E3B RID: 11835 RVA: 0x000FD0DF File Offset: 0x000FB2DF
	public void Think(float dt)
	{
		this.OnThink(dt);
	}

	// Token: 0x06002E3C RID: 11836 RVA: 0x000FD0E8 File Offset: 0x000FB2E8
	public void UpdateAuthority(float dt)
	{
		this.OnUpdateShared(dt);
		this.OnUpdateAuthority(dt);
	}

	// Token: 0x06002E3D RID: 11837 RVA: 0x000FD0F8 File Offset: 0x000FB2F8
	public void UpdateRemote(float dt)
	{
		this.OnUpdateShared(dt);
		this.OnUpdateRemote(dt);
	}

	// Token: 0x06002E3E RID: 11838 RVA: 0x000FD108 File Offset: 0x000FB308
	protected virtual void PlayAnim(string animName, float blendTime, float speed)
	{
		if (this.anim != null && !string.IsNullOrEmpty(animName))
		{
			if (this.anim.GetClip(animName) == null)
			{
				Debug.LogErrorFormat("Anim Clip {0} does not exist in (1)", new object[]
				{
					animName,
					this.anim
				});
				return;
			}
			this.anim[animName].speed = speed;
			this.anim.CrossFade(animName, blendTime);
		}
	}

	// Token: 0x06002E3F RID: 11839 RVA: 0x000FD17C File Offset: 0x000FB37C
	public bool IsCoolDownOver(float coolDown)
	{
		return (float)(Time.timeAsDouble - this.stopTime) > coolDown;
	}

	// Token: 0x06002E40 RID: 11840 RVA: 0x000DFAE0 File Offset: 0x000DDCE0
	public virtual float GetRange()
	{
		return 0f;
	}

	// Token: 0x04003B49 RID: 15177
	protected GameAgent agent;

	// Token: 0x04003B4A RID: 15178
	protected GameEntity entity;

	// Token: 0x04003B4B RID: 15179
	protected Animation anim;

	// Token: 0x04003B4C RID: 15180
	protected Animator animator;

	// Token: 0x04003B4D RID: 15181
	protected Transform root;

	// Token: 0x04003B4E RID: 15182
	protected Transform head;

	// Token: 0x04003B4F RID: 15183
	protected AudioSource audioSource;

	// Token: 0x04003B50 RID: 15184
	protected GRSenseLineOfSight lineOfSight;

	// Token: 0x04003B51 RID: 15185
	protected Rigidbody rb;

	// Token: 0x04003B52 RID: 15186
	protected GRAttributes attributes;

	// Token: 0x04003B53 RID: 15187
	[ReadOnly]
	public double startTime;

	// Token: 0x04003B54 RID: 15188
	[ReadOnly]
	public double stopTime;

	// Token: 0x04003B55 RID: 15189
	protected int walkableArea = -1;
}
