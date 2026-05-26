using System;
using System.Collections.Generic;
using GorillaNetworking;
using UnityEngine;

// Token: 0x020007CC RID: 1996
public class GRReviveStation : MonoBehaviour
{
	// Token: 0x17000499 RID: 1177
	// (get) Token: 0x060032EB RID: 13035 RVA: 0x0011710C File Offset: 0x0011530C
	// (set) Token: 0x060032EC RID: 13036 RVA: 0x00117114 File Offset: 0x00115314
	public int Index { get; set; }

	// Token: 0x060032ED RID: 13037 RVA: 0x0011711D File Offset: 0x0011531D
	public void Init(GhostReactor reactor, int index)
	{
		this.reactor = reactor;
		this.Index = index;
	}

	// Token: 0x060032EE RID: 13038 RVA: 0x0011712D File Offset: 0x0011532D
	public void SetReviveCooldownSeconds(double seconds)
	{
		this.reviveCooldownSeconds = seconds;
	}

	// Token: 0x060032EF RID: 13039 RVA: 0x00117136 File Offset: 0x00115336
	public double GetReviveCooldownSeconds()
	{
		return this.reviveCooldownSeconds;
	}

	// Token: 0x060032F0 RID: 13040 RVA: 0x00117140 File Offset: 0x00115340
	public double CalculateRemainingReviveCooldownSeconds(int ActorNumber)
	{
		if (this.reviveCooldownSeconds == 0.0)
		{
			return 0.0;
		}
		if (this.cooldownStartTime.ContainsKey(ActorNumber))
		{
			return this.reviveCooldownSeconds - (GorillaComputer.instance.GetServerTime() - this.cooldownStartTime[ActorNumber]).TotalSeconds;
		}
		return 0.0;
	}

	// Token: 0x060032F1 RID: 13041 RVA: 0x001171AC File Offset: 0x001153AC
	public void RevivePlayer(GRPlayer player)
	{
		if (player != null)
		{
			int actorNumber = player.gamePlayer.rig.OwningNetPlayer.ActorNumber;
			this.cooldownStartTime[actorNumber] = GorillaComputer.instance.GetServerTime();
			if (player.State != GRPlayer.GRPlayerState.Alive || player.Hp < player.MaxHp)
			{
				player.OnPlayerRevive(this.reactor.grManager);
				if (this.audioSource != null)
				{
					this.audioSource.Play();
				}
				if (this.particleEffects != null)
				{
					for (int i = 0; i < this.particleEffects.Length; i++)
					{
						this.particleEffects[i].Play();
					}
				}
			}
		}
	}

	// Token: 0x060032F2 RID: 13042 RVA: 0x0011725C File Offset: 0x0011545C
	private void OnTriggerEnter(Collider collider)
	{
		Rigidbody attachedRigidbody = collider.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			VRRig component = attachedRigidbody.GetComponent<VRRig>();
			if (component != null)
			{
				GRPlayer component2 = component.GetComponent<GRPlayer>();
				if (component2 != null && (component2.State != GRPlayer.GRPlayerState.Alive || component2.Hp < component2.MaxHp))
				{
					if (!NetworkSystem.Instance.InRoom && component == VRRig.LocalRig)
					{
						this.RevivePlayer(component2);
					}
					if (this.reactor.grManager.IsAuthority() && this.CalculateRemainingReviveCooldownSeconds(component2.gamePlayer.rig.OwningNetPlayer.ActorNumber) <= 0.0)
					{
						this.reactor.grManager.RequestPlayerRevive(this, component2);
					}
				}
			}
		}
	}

	// Token: 0x0400422E RID: 16942
	public AudioSource audioSource;

	// Token: 0x0400422F RID: 16943
	public ParticleSystem[] particleEffects;

	// Token: 0x04004230 RID: 16944
	[SerializeField]
	private double reviveCooldownSeconds;

	// Token: 0x04004231 RID: 16945
	private Dictionary<int, DateTime> cooldownStartTime = new Dictionary<int, DateTime>();

	// Token: 0x04004233 RID: 16947
	private GhostReactor reactor;
}
