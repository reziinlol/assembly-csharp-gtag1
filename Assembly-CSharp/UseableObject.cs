using System;
using GorillaExtensions;
using GorillaTag;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x0200059E RID: 1438
[RequireComponent(typeof(UseableObjectEvents))]
public class UseableObject : TransferrableObject
{
	// Token: 0x170003CE RID: 974
	// (get) Token: 0x0600246C RID: 9324 RVA: 0x000C3BF7 File Offset: 0x000C1DF7
	public bool isMidUse
	{
		get
		{
			return this._isMidUse;
		}
	}

	// Token: 0x170003CF RID: 975
	// (get) Token: 0x0600246D RID: 9325 RVA: 0x000C3BFF File Offset: 0x000C1DFF
	public float useTimeElapsed
	{
		get
		{
			return this._useTimeElapsed;
		}
	}

	// Token: 0x170003D0 RID: 976
	// (get) Token: 0x0600246E RID: 9326 RVA: 0x000C3C07 File Offset: 0x000C1E07
	public bool justUsed
	{
		get
		{
			if (!this._justUsed)
			{
				return false;
			}
			this._justUsed = false;
			return true;
		}
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x000C3C1B File Offset: 0x000C1E1B
	protected override void Awake()
	{
		base.Awake();
		this._events = base.gameObject.GetOrAddComponent<UseableObjectEvents>();
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x000C3C34 File Offset: 0x000C1E34
	internal override void OnEnable()
	{
		base.OnEnable();
		UseableObjectEvents events = this._events;
		VRRig myOnlineRig = base.myOnlineRig;
		NetPlayer player;
		if ((player = ((myOnlineRig != null) ? myOnlineRig.creator : null)) == null)
		{
			VRRig myRig = base.myRig;
			player = ((myRig != null) ? myRig.creator : null);
		}
		events.Init(player);
		this._events.Activate += this.OnObjectActivated;
		this._events.Deactivate += this.OnObjectDeactivated;
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x000C3CBE File Offset: 0x000C1EBE
	internal override void OnDisable()
	{
		base.OnDisable();
		Object.Destroy(this._events);
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x000C3CD1 File Offset: 0x000C1ED1
	private void OnObjectActivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x000C3CD1 File Offset: 0x000C1ED1
	private void OnObjectDeactivated(int sender, int target, object[] args, PhotonMessageInfoWrapped info)
	{
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x000C3CD7 File Offset: 0x000C1ED7
	public override void TriggeredLateUpdate()
	{
		base.TriggeredLateUpdate();
		if (this._isMidUse)
		{
			this._useTimeElapsed += Time.deltaTime;
		}
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x000C3CFC File Offset: 0x000C1EFC
	public override void OnActivate()
	{
		base.OnActivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onActivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._useTimeElapsed = 0f;
			this._isMidUse = true;
		}
		if (this._raiseActivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent activate = events.Activate;
			if (activate == null)
			{
				return;
			}
			activate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x000C3D64 File Offset: 0x000C1F64
	public override void OnDeactivate()
	{
		base.OnDeactivate();
		if (this.IsMyItem())
		{
			UnityEvent unityEvent = this.onDeactivateLocal;
			if (unityEvent != null)
			{
				unityEvent.Invoke();
			}
			this._isMidUse = false;
			this._justUsed = true;
		}
		if (this._raiseDeactivate)
		{
			UseableObjectEvents events = this._events;
			if (events == null)
			{
				return;
			}
			PhotonEvent deactivate = events.Deactivate;
			if (deactivate == null)
			{
				return;
			}
			deactivate.RaiseAll(Array.Empty<object>());
		}
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x000C3DC5 File Offset: 0x000C1FC5
	public override bool CanActivate()
	{
		return !this.disableActivation;
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x000C3DD0 File Offset: 0x000C1FD0
	public override bool CanDeactivate()
	{
		return !this.disableDeactivation;
	}

	// Token: 0x04002FD4 RID: 12244
	[DebugOption]
	public bool disableActivation;

	// Token: 0x04002FD5 RID: 12245
	[DebugOption]
	public bool disableDeactivation;

	// Token: 0x04002FD6 RID: 12246
	[SerializeField]
	private UseableObjectEvents _events;

	// Token: 0x04002FD7 RID: 12247
	[SerializeField]
	private bool _raiseActivate = true;

	// Token: 0x04002FD8 RID: 12248
	[SerializeField]
	private bool _raiseDeactivate = true;

	// Token: 0x04002FD9 RID: 12249
	[NonSerialized]
	private DateTime _lastActivate;

	// Token: 0x04002FDA RID: 12250
	[NonSerialized]
	private DateTime _lastDeactivate;

	// Token: 0x04002FDB RID: 12251
	[NonSerialized]
	private bool _isMidUse;

	// Token: 0x04002FDC RID: 12252
	[NonSerialized]
	private float _useTimeElapsed;

	// Token: 0x04002FDD RID: 12253
	[NonSerialized]
	private bool _justUsed;

	// Token: 0x04002FDE RID: 12254
	[NonSerialized]
	private int tempHandPos;

	// Token: 0x04002FDF RID: 12255
	public UnityEvent onActivateLocal;

	// Token: 0x04002FE0 RID: 12256
	public UnityEvent onDeactivateLocal;
}
