using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

// Token: 0x02000124 RID: 292
public class ThrownGadget : MonoBehaviour
{
	// Token: 0x1400000F RID: 15
	// (add) Token: 0x06000730 RID: 1840 RVA: 0x00028D74 File Offset: 0x00026F74
	// (remove) Token: 0x06000731 RID: 1841 RVA: 0x00028DAC File Offset: 0x00026FAC
	public event Action OnActivated;

	// Token: 0x14000010 RID: 16
	// (add) Token: 0x06000732 RID: 1842 RVA: 0x00028DE4 File Offset: 0x00026FE4
	// (remove) Token: 0x06000733 RID: 1843 RVA: 0x00028E1C File Offset: 0x0002701C
	public event Action OnThrown;

	// Token: 0x14000011 RID: 17
	// (add) Token: 0x06000734 RID: 1844 RVA: 0x00028E54 File Offset: 0x00027054
	// (remove) Token: 0x06000735 RID: 1845 RVA: 0x00028E8C File Offset: 0x0002708C
	public event Action OnHitSurface;

	// Token: 0x06000736 RID: 1846 RVA: 0x00028EC1 File Offset: 0x000270C1
	private void OnEnable()
	{
		this.isHeldLocal = false;
		this.lastThrowerLocal = false;
	}

	// Token: 0x06000737 RID: 1847 RVA: 0x00028ED1 File Offset: 0x000270D1
	public bool IsHeld()
	{
		return this.gameEntity.heldByActorNumber != -1;
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x00028EE4 File Offset: 0x000270E4
	public bool IsHeldLocal()
	{
		return this.gameEntity.heldByActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00028EFD File Offset: 0x000270FD
	public bool IsHeldByAnother()
	{
		return this.IsHeld() && !this.IsHeldLocal();
	}

	// Token: 0x0600073A RID: 1850 RVA: 0x00028F14 File Offset: 0x00027114
	private bool IsButtonHeld()
	{
		if (!this.IsHeldLocal())
		{
			return false;
		}
		GamePlayer gamePlayer;
		if (!GamePlayer.TryGetGamePlayer(this.gameEntity.heldByActorNumber, out gamePlayer))
		{
			return false;
		}
		if (gamePlayer == null)
		{
			return false;
		}
		int num = gamePlayer.FindHandIndex(this.gameEntity.id);
		return num != -1 && ControllerInputPoller.TriggerFloat(GamePlayer.IsLeftHand(num) ? XRNode.LeftHand : XRNode.RightHand) > 0.25f;
	}

	// Token: 0x0600073B RID: 1851 RVA: 0x00028F7C File Offset: 0x0002717C
	public void Update()
	{
		bool flag = this.IsHeldLocal();
		if (flag)
		{
			this.lastThrowerLocal = true;
			this.UpdateActivation();
		}
		else if (this.isHeldLocal)
		{
			Action onThrown = this.OnThrown;
			if (onThrown != null)
			{
				onThrown();
			}
		}
		else if (this.IsHeldByAnother())
		{
			this.lastThrowerLocal = false;
		}
		this.isHeldLocal = flag;
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x00028FD4 File Offset: 0x000271D4
	private void UpdateActivation()
	{
		bool flag = this.IsButtonHeld();
		if (!this.activationButtonLastInput && flag)
		{
			Action onActivated = this.OnActivated;
			if (onActivated != null)
			{
				onActivated();
			}
		}
		this.activationButtonLastInput = flag;
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0002900D File Offset: 0x0002720D
	public void OnCollisionEnter(Collision collision)
	{
		if (this.lastThrowerLocal)
		{
			Action onHitSurface = this.OnHitSurface;
			if (onHitSurface == null)
			{
				return;
			}
			onHitSurface();
		}
	}

	// Token: 0x04000977 RID: 2423
	public GameEntity gameEntity;

	// Token: 0x0400097B RID: 2427
	private bool isHeldLocal;

	// Token: 0x0400097C RID: 2428
	private bool lastThrowerLocal;

	// Token: 0x0400097D RID: 2429
	private bool activationButtonLastInput;
}
