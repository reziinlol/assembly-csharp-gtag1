using System;
using UnityEngine;

// Token: 0x02000100 RID: 256
public abstract class SIGadgetGrenade : SIGadget
{
	// Token: 0x06000617 RID: 1559 RVA: 0x00022B1C File Offset: 0x00020D1C
	protected new virtual void OnEnable()
	{
		this.rb = base.GetComponent<Rigidbody>();
		this.activatedLocally = false;
		this.thrownGadget.OnActivated += this.HandleActivated;
		this.thrownGadget.OnThrown += this.HandleThrown;
		this.thrownGadget.OnHitSurface += this.HandleHitSurface;
	}

	// Token: 0x06000618 RID: 1560 RVA: 0x00022B84 File Offset: 0x00020D84
	protected new virtual void OnDisable()
	{
		this.thrownGadget.OnActivated -= this.HandleActivated;
		this.thrownGadget.OnThrown -= this.HandleThrown;
		this.thrownGadget.OnHitSurface -= this.HandleHitSurface;
	}

	// Token: 0x06000619 RID: 1561
	protected abstract void HandleActivated();

	// Token: 0x0600061A RID: 1562
	protected abstract void HandleThrown();

	// Token: 0x0600061B RID: 1563
	protected abstract void HandleHitSurface();

	// Token: 0x0600061C RID: 1564 RVA: 0x00022BDC File Offset: 0x00020DDC
	public override void OnEntityInit()
	{
		base.OnEntityInit();
		GameEntityId entityIdFromNetId = this.gameEntity.manager.GetEntityIdFromNetId((int)this.gameEntity.createData);
		this.parentEntity = this.gameEntity.manager.GetGameEntity(entityIdFromNetId);
		SIGadgetHolsterDisk component = this.parentEntity.GetComponent<SIGadgetHolsterDisk>();
		if (component != null)
		{
			component.RegisterGadget(this);
		}
	}

	// Token: 0x04000785 RID: 1925
	public Action GrenadeFinished;

	// Token: 0x04000786 RID: 1926
	public Renderer grenadeRenderer;

	// Token: 0x04000787 RID: 1927
	[SerializeField]
	protected ThrownGadget thrownGadget;

	// Token: 0x04000788 RID: 1928
	protected Rigidbody rb;

	// Token: 0x04000789 RID: 1929
	protected GameEntity parentEntity;
}
