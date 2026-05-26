using System;
using UnityEngine;
using UnityEngine.AI;

// Token: 0x02000720 RID: 1824
[Serializable]
public class GRAbilityInterpolatedMovement
{
	// Token: 0x06002E4B RID: 11851 RVA: 0x000FD396 File Offset: 0x000FB596
	public void Setup(Transform root)
	{
		this.root = root;
		this.rb = root.gameObject.GetComponent<Rigidbody>();
		this.walkableArea = NavMesh.GetAreaFromName("walkable");
	}

	// Token: 0x06002E4C RID: 11852 RVA: 0x000FD3C0 File Offset: 0x000FB5C0
	public void InitFromVelocityAndDuration(Vector3 velocity, float duration)
	{
		this.velocity = velocity;
		this.duration = duration;
		float magnitude = velocity.magnitude;
	}

	// Token: 0x06002E4D RID: 11853 RVA: 0x000FD3D8 File Offset: 0x000FB5D8
	public void Start()
	{
		this.startPos = this.root.position;
		this.endPos = this.startPos + this.velocity * this.duration;
		this.endTime = Time.timeAsDouble + (double)this.duration;
		NavMeshHit navMeshHit;
		if (NavMesh.SamplePosition(this.endPos, out navMeshHit, 5f, this.walkableArea))
		{
			this.endPos = navMeshHit.position;
		}
	}

	// Token: 0x06002E4E RID: 11854 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void Stop()
	{
	}

	// Token: 0x06002E4F RID: 11855 RVA: 0x000FD452 File Offset: 0x000FB652
	public bool IsDone()
	{
		return Time.timeAsDouble >= this.endTime;
	}

	// Token: 0x06002E50 RID: 11856 RVA: 0x000FD464 File Offset: 0x000FB664
	public void Update(float dt)
	{
		Vector3 position = this.root.position;
		float num = Mathf.Clamp01(1f - (float)((this.endTime - Time.timeAsDouble) / (double)this.duration));
		GRAbilityInterpolatedMovement.InterpType interpType = this.interpolationType;
		Vector3 vector;
		if (interpType != GRAbilityInterpolatedMovement.InterpType.Linear && interpType == GRAbilityInterpolatedMovement.InterpType.EaseOut)
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, AbilityHelperFunctions.EaseOutPower(num, 2.5f));
		}
		else
		{
			vector = Vector3.Lerp(this.startPos, this.endPos, num);
		}
		vector.y = Mathf.Lerp(this.startPos.y, this.endPos.y, num * num);
		NavMeshHit navMeshHit;
		if (NavMesh.Raycast(position, vector, out navMeshHit, this.walkableArea))
		{
			vector = navMeshHit.position;
		}
		this.root.position = vector;
		if (this.rb != null)
		{
			this.rb.position = vector;
		}
	}

	// Token: 0x04003B5F RID: 15199
	public Vector3 velocity = Vector3.zero;

	// Token: 0x04003B60 RID: 15200
	private Vector3 startPos;

	// Token: 0x04003B61 RID: 15201
	private Vector3 endPos;

	// Token: 0x04003B62 RID: 15202
	public float duration;

	// Token: 0x04003B63 RID: 15203
	public double endTime;

	// Token: 0x04003B64 RID: 15204
	public float maxVelocityMagnitude = 2f;

	// Token: 0x04003B65 RID: 15205
	private Transform root;

	// Token: 0x04003B66 RID: 15206
	private Rigidbody rb;

	// Token: 0x04003B67 RID: 15207
	public GRAbilityInterpolatedMovement.InterpType interpolationType;

	// Token: 0x04003B68 RID: 15208
	private int walkableArea = -1;

	// Token: 0x02000721 RID: 1825
	public enum InterpType
	{
		// Token: 0x04003B6A RID: 15210
		Linear,
		// Token: 0x04003B6B RID: 15211
		EaseOut
	}
}
