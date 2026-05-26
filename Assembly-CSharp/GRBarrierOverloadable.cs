using System;
using UnityEngine;

// Token: 0x02000746 RID: 1862
public class GRBarrierOverloadable : MonoBehaviour
{
	// Token: 0x06002F39 RID: 12089 RVA: 0x00101475 File Offset: 0x000FF675
	private void OnEnable()
	{
		this.tool.OnEnergyChange += this.OnEnergyChange;
		this.gameEntity.OnStateChanged += this.OnEntityStateChanged;
	}

	// Token: 0x06002F3A RID: 12090 RVA: 0x001014A8 File Offset: 0x000FF6A8
	private void OnEnergyChange(GRTool tool, int energyChange, GameEntityId chargingEntityId)
	{
		if (this.state == GRBarrierOverloadable.State.Active && tool.energy >= tool.GetEnergyMax())
		{
			this.SetState(GRBarrierOverloadable.State.Destroyed);
			if (this.gameEntity.IsAuthority())
			{
				this.gameEntity.RequestState(this.gameEntity.id, 1L);
			}
		}
	}

	// Token: 0x06002F3B RID: 12091 RVA: 0x001014F7 File Offset: 0x000FF6F7
	private void OnEntityStateChanged(long prevState, long nextState)
	{
		if (!this.gameEntity.IsAuthority())
		{
			this.SetState((GRBarrierOverloadable.State)nextState);
		}
	}

	// Token: 0x06002F3C RID: 12092 RVA: 0x00101510 File Offset: 0x000FF710
	public void SetState(GRBarrierOverloadable.State newState)
	{
		if (this.state != newState)
		{
			this.state = newState;
			GRBarrierOverloadable.State state = this.state;
			if (state == GRBarrierOverloadable.State.Active)
			{
				this.meshRenderer.enabled = true;
				this.collider.enabled = true;
				return;
			}
			if (state != GRBarrierOverloadable.State.Destroyed)
			{
				return;
			}
			this.audioSource.Play();
			this.meshRenderer.enabled = false;
			this.collider.enabled = false;
		}
	}

	// Token: 0x04003C95 RID: 15509
	public GRTool tool;

	// Token: 0x04003C96 RID: 15510
	public GameEntity gameEntity;

	// Token: 0x04003C97 RID: 15511
	public AudioSource audioSource;

	// Token: 0x04003C98 RID: 15512
	public MeshRenderer meshRenderer;

	// Token: 0x04003C99 RID: 15513
	public Collider collider;

	// Token: 0x04003C9A RID: 15514
	private GRBarrierOverloadable.State state;

	// Token: 0x02000747 RID: 1863
	public enum State
	{
		// Token: 0x04003C9C RID: 15516
		Active,
		// Token: 0x04003C9D RID: 15517
		Destroyed
	}
}
