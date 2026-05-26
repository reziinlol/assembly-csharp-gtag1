using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000772 RID: 1906
[Serializable]
public class GRDamageFlash
{
	// Token: 0x0600303F RID: 12351 RVA: 0x001064AC File Offset: 0x001046AC
	public void Setup()
	{
		this.flashRendererDefaultMaterial = new List<Material>(this.flashRenderers.Count);
		this.stateMachine = new SimpleStateMachine<GRDamageFlash.State>();
		for (int i = 0; i < this.flashRenderers.Count; i++)
		{
			this.flashRendererDefaultMaterial.Add(this.flashRenderers[i].sharedMaterial);
		}
		this.stateMachine.Setup(GRDamageFlash.State.Idle, new Action<GRDamageFlash.State>(this.OnStateStart), new Action<GRDamageFlash.State>(this.OnStateEnd), new Action<GRDamageFlash.State>(this.OnStateUpdate));
	}

	// Token: 0x06003040 RID: 12352 RVA: 0x0010653C File Offset: 0x0010473C
	public void Play()
	{
		if (this.stateMachine.GetState() == GRDamageFlash.State.Idle)
		{
			this.stateMachine.SetState(GRDamageFlash.State.Playing, false);
		}
	}

	// Token: 0x06003041 RID: 12353 RVA: 0x00106558 File Offset: 0x00104758
	public void OnStateStart(GRDamageFlash.State state)
	{
		if (state == GRDamageFlash.State.Playing)
		{
			for (int i = 0; i < this.flashRenderers.Count; i++)
			{
				this.flashRenderers[i].material = this.flashMaterial;
			}
		}
	}

	// Token: 0x06003042 RID: 12354 RVA: 0x00106598 File Offset: 0x00104798
	public void OnStateEnd(GRDamageFlash.State state)
	{
		if (state == GRDamageFlash.State.Playing)
		{
			for (int i = 0; i < this.flashRenderers.Count; i++)
			{
				this.flashRenderers[i].material = this.flashRendererDefaultMaterial[i];
			}
		}
	}

	// Token: 0x06003043 RID: 12355 RVA: 0x001065DC File Offset: 0x001047DC
	public void OnStateUpdate(GRDamageFlash.State state)
	{
		if (state != GRDamageFlash.State.Playing)
		{
			if (state != GRDamageFlash.State.Cooldown)
			{
				return;
			}
			if (this.stateMachine.IsStateFinished(Time.timeAsDouble, this.flashCooldown))
			{
				this.stateMachine.SetState(GRDamageFlash.State.Idle, false);
			}
		}
		else if (this.stateMachine.IsStateFinished(Time.timeAsDouble, this.flashDuration))
		{
			this.stateMachine.SetState((this.flashCooldown > 0f) ? GRDamageFlash.State.Cooldown : GRDamageFlash.State.Idle, false);
			return;
		}
	}

	// Token: 0x06003044 RID: 12356 RVA: 0x0010664D File Offset: 0x0010484D
	public void Stop()
	{
		this.stateMachine.SetState(GRDamageFlash.State.Idle, false);
	}

	// Token: 0x06003045 RID: 12357 RVA: 0x0010665C File Offset: 0x0010485C
	public void Update()
	{
		this.stateMachine.Update();
	}

	// Token: 0x04003DCF RID: 15823
	public Material flashMaterial;

	// Token: 0x04003DD0 RID: 15824
	public float flashDuration = 0.1f;

	// Token: 0x04003DD1 RID: 15825
	public float flashCooldown = 0.1f;

	// Token: 0x04003DD2 RID: 15826
	public List<Renderer> flashRenderers;

	// Token: 0x04003DD3 RID: 15827
	private SimpleStateMachine<GRDamageFlash.State> stateMachine;

	// Token: 0x04003DD4 RID: 15828
	private List<Material> flashRendererDefaultMaterial;

	// Token: 0x02000773 RID: 1907
	public enum State
	{
		// Token: 0x04003DD6 RID: 15830
		Idle,
		// Token: 0x04003DD7 RID: 15831
		Playing,
		// Token: 0x04003DD8 RID: 15832
		Cooldown
	}
}
