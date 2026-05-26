using System;
using GorillaNetworking;
using UnityEngine;

// Token: 0x0200066D RID: 1645
public abstract class CosmeticCritterHoldable : MonoBehaviour
{
	// Token: 0x17000420 RID: 1056
	// (get) Token: 0x06002924 RID: 10532 RVA: 0x000DE767 File Offset: 0x000DC967
	// (set) Token: 0x06002925 RID: 10533 RVA: 0x000DE76F File Offset: 0x000DC96F
	public int OwnerID { get; private set; }

	// Token: 0x17000421 RID: 1057
	// (get) Token: 0x06002926 RID: 10534 RVA: 0x000DE778 File Offset: 0x000DC978
	public bool IsLocal
	{
		get
		{
			return this.transferrableObject.IsLocalObject();
		}
	}

	// Token: 0x06002927 RID: 10535 RVA: 0x000DE785 File Offset: 0x000DC985
	public bool OwningPlayerMatches(PhotonMessageInfoWrapped info)
	{
		return this.transferrableObject.targetRig.creator == info.Sender;
	}

	// Token: 0x06002928 RID: 10536 RVA: 0x000DE79F File Offset: 0x000DC99F
	protected virtual CallLimiter CreateCallLimiter()
	{
		return new CallLimiter(10, 2f, 0.5f);
	}

	// Token: 0x06002929 RID: 10537 RVA: 0x000DE7B2 File Offset: 0x000DC9B2
	public void ResetCallLimiter()
	{
		this.callLimiter.Reset();
	}

	// Token: 0x0600292A RID: 10538 RVA: 0x000DE7C0 File Offset: 0x000DC9C0
	private void TrySetID()
	{
		if (this.IsLocal)
		{
			PlayFabAuthenticator instance = PlayFabAuthenticator.instance;
			if (instance != null)
			{
				string playFabPlayerId = instance.GetPlayFabPlayerId();
				Type type = base.GetType();
				this.OwnerID = (playFabPlayerId + ((type != null) ? type.ToString() : null)).GetStaticHash();
				return;
			}
		}
		else if (this.transferrableObject.targetRig != null && this.transferrableObject.targetRig.creator != null)
		{
			string userId = this.transferrableObject.targetRig.creator.UserId;
			Type type2 = base.GetType();
			this.OwnerID = (userId + ((type2 != null) ? type2.ToString() : null)).GetStaticHash();
		}
	}

	// Token: 0x0600292B RID: 10539 RVA: 0x000DE86E File Offset: 0x000DCA6E
	protected virtual void Awake()
	{
		this.transferrableObject = base.GetComponentInParent<TransferrableObject>();
		this.callLimiter = this.CreateCallLimiter();
		if (this.IsLocal)
		{
			CosmeticCritterManager.Instance.RegisterLocalHoldable(this);
		}
	}

	// Token: 0x0600292C RID: 10540 RVA: 0x000DE89B File Offset: 0x000DCA9B
	protected virtual void OnEnable()
	{
		this.TrySetID();
	}

	// Token: 0x0600292D RID: 10541 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnDisable()
	{
	}

	// Token: 0x0400359F RID: 13727
	protected TransferrableObject transferrableObject;

	// Token: 0x040035A1 RID: 13729
	protected CallLimiter callLimiter;
}
