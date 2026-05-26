using System;
using GorillaTag;
using UnityEngine;

// Token: 0x02000594 RID: 1428
public class StopwatchCosmetic : TransferrableObject
{
	// Token: 0x170003C8 RID: 968
	// (get) Token: 0x06002423 RID: 9251 RVA: 0x000C21FB File Offset: 0x000C03FB
	public bool isActivating
	{
		get
		{
			return this._isActivating;
		}
	}

	// Token: 0x170003C9 RID: 969
	// (get) Token: 0x06002424 RID: 9252 RVA: 0x000C2203 File Offset: 0x000C0403
	public float activeTimeElapsed
	{
		get
		{
			return this._activeTimeElapsed;
		}
	}

	// Token: 0x06002425 RID: 9253 RVA: 0x000C220C File Offset: 0x000C040C
	protected override void Awake()
	{
		base.Awake();
		if (StopwatchCosmetic.gWatchToggleRPC == null)
		{
			StopwatchCosmetic.gWatchToggleRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchToggle"));
		}
		if (StopwatchCosmetic.gWatchResetRPC == null)
		{
			StopwatchCosmetic.gWatchResetRPC = new PhotonEvent(StaticHash.Compute("StopwatchCosmetic", "WatchReset"));
		}
		this._watchToggle = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchToggle);
		this._watchReset = new Action<int, int, object[], PhotonMessageInfoWrapped>(this.OnWatchReset);
	}

	// Token: 0x06002426 RID: 9254 RVA: 0x000C2290 File Offset: 0x000C0490
	internal override void OnEnable()
	{
		base.OnEnable();
		int i;
		if (!this.FetchMyViewID(out i))
		{
			this._photonID = -1;
			return;
		}
		StopwatchCosmetic.gWatchResetRPC += this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC += this._watchToggle;
		this._photonID = i.GetStaticHash();
	}

	// Token: 0x06002427 RID: 9255 RVA: 0x000C22EB File Offset: 0x000C04EB
	internal override void OnDisable()
	{
		base.OnDisable();
		StopwatchCosmetic.gWatchResetRPC -= this._watchReset;
		StopwatchCosmetic.gWatchToggleRPC -= this._watchToggle;
	}

	// Token: 0x06002428 RID: 9256 RVA: 0x000C2320 File Offset: 0x000C0520
	private void OnWatchToggle(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnWatchToggle");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		bool flag = (bool)args[1];
		int millis = (int)args[2];
		this._watchFace.SetMillisElapsed(millis, true);
		this._watchFace.WatchToggle();
	}

	// Token: 0x06002429 RID: 9257 RVA: 0x000C23A0 File Offset: 0x000C05A0
	private void OnWatchReset(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
		if (this._photonID == -1)
		{
			return;
		}
		if (info.senderID != this.ownerRig.creator.ActorNumber)
		{
			return;
		}
		if (sender != target)
		{
			return;
		}
		MonkeAgent.IncrementRPCCall(info, "OnWatchReset");
		if ((int)args[0] != this._photonID)
		{
			return;
		}
		this._watchFace.WatchReset();
	}

	// Token: 0x0600242A RID: 9258 RVA: 0x000C2400 File Offset: 0x000C0600
	private bool FetchMyViewID(out int viewID)
	{
		viewID = -1;
		NetPlayer netPlayer = (base.myOnlineRig != null) ? base.myOnlineRig.creator : ((base.myRig != null) ? ((base.myRig.creator != null) ? base.myRig.creator : NetworkSystem.Instance.LocalPlayer) : null);
		if (netPlayer == null)
		{
			return false;
		}
		RigContainer rigContainer;
		if (!VRRigCache.Instance.TryGetVrrig(netPlayer, out rigContainer))
		{
			return false;
		}
		if (rigContainer.Rig.netView == null)
		{
			return false;
		}
		viewID = rigContainer.Rig.netView.ViewID;
		return true;
	}

	// Token: 0x0600242B RID: 9259 RVA: 0x000C249F File Offset: 0x000C069F
	public bool PollActivated()
	{
		if (!this._activated)
		{
			return false;
		}
		this._activated = false;
		return true;
	}

	// Token: 0x0600242C RID: 9260 RVA: 0x000C24B4 File Offset: 0x000C06B4
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isActivating)
		{
			this._activeTimeElapsed += Time.deltaTime;
		}
		if (this._isActivating && this._activeTimeElapsed > 1f)
		{
			this._isActivating = false;
			this._watchFace.WatchReset(true);
			StopwatchCosmetic.gWatchResetRPC.RaiseOthers(new object[]
			{
				this._photonID
			});
		}
	}

	// Token: 0x0600242D RID: 9261 RVA: 0x000C2527 File Offset: 0x000C0727
	public override void OnActivate()
	{
		if (!this.CanActivate())
		{
			return;
		}
		base.OnActivate();
		if (this.IsMyItem())
		{
			this._activeTimeElapsed = 0f;
			this._isActivating = true;
		}
	}

	// Token: 0x0600242E RID: 9262 RVA: 0x000C2554 File Offset: 0x000C0754
	public override void OnDeactivate()
	{
		if (!this.CanDeactivate())
		{
			return;
		}
		base.OnDeactivate();
		if (!this.IsMyItem())
		{
			return;
		}
		this._isActivating = false;
		this._activated = true;
		this._watchFace.WatchToggle();
		StopwatchCosmetic.gWatchToggleRPC.RaiseOthers(new object[]
		{
			this._photonID,
			this._watchFace.watchActive,
			this._watchFace.millisElapsed
		});
		this._activated = false;
	}

	// Token: 0x0600242F RID: 9263 RVA: 0x000C25DD File Offset: 0x000C07DD
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06002430 RID: 9264 RVA: 0x000C25E8 File Offset: 0x000C07E8
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002F7A RID: 12154
	[SerializeField]
	private StopwatchFace _watchFace;

	// Token: 0x04002F7B RID: 12155
	[Space]
	[NonSerialized]
	private bool _isActivating;

	// Token: 0x04002F7C RID: 12156
	[NonSerialized]
	private float _activeTimeElapsed;

	// Token: 0x04002F7D RID: 12157
	[NonSerialized]
	private bool _activated;

	// Token: 0x04002F7E RID: 12158
	[Space]
	[NonSerialized]
	private int _photonID = -1;

	// Token: 0x04002F7F RID: 12159
	private static PhotonEvent gWatchToggleRPC;

	// Token: 0x04002F80 RID: 12160
	private static PhotonEvent gWatchResetRPC;

	// Token: 0x04002F81 RID: 12161
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchToggle;

	// Token: 0x04002F82 RID: 12162
	private Action<int, int, object[], PhotonMessageInfoWrapped> _watchReset;

	// Token: 0x04002F83 RID: 12163
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002F84 RID: 12164
	[DebugOption]
	public bool disableDeactivation;
}
