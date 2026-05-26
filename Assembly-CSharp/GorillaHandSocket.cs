using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000863 RID: 2147
[DisallowMultipleComponent]
public class GorillaHandSocket : MonoBehaviour
{
	// Token: 0x170004F0 RID: 1264
	// (get) Token: 0x060037E8 RID: 14312 RVA: 0x00131648 File Offset: 0x0012F848
	public GorillaHandNode attachedHand
	{
		get
		{
			return this._attachedHand;
		}
	}

	// Token: 0x170004F1 RID: 1265
	// (get) Token: 0x060037E9 RID: 14313 RVA: 0x00131650 File Offset: 0x0012F850
	public bool inUse
	{
		get
		{
			return this._inUse;
		}
	}

	// Token: 0x060037EA RID: 14314 RVA: 0x00131658 File Offset: 0x0012F858
	public static bool FetchSocket(Collider collider, out GorillaHandSocket socket)
	{
		return GorillaHandSocket.gColliderToSocket.TryGetValue(collider, out socket);
	}

	// Token: 0x060037EB RID: 14315 RVA: 0x00131666 File Offset: 0x0012F866
	public bool CanAttach()
	{
		return !this._inUse && this._sinceSocketStateChange.HasElapsed(this.attachCooldown, true);
	}

	// Token: 0x060037EC RID: 14316 RVA: 0x00131684 File Offset: 0x0012F884
	public void Attach(GorillaHandNode hand)
	{
		if (!this.CanAttach())
		{
			return;
		}
		if (hand == null)
		{
			return;
		}
		hand.attachedToSocket = this;
		this._attachedHand = hand;
		this._inUse = true;
		this.OnHandAttach();
	}

	// Token: 0x060037ED RID: 14317 RVA: 0x001316B4 File Offset: 0x0012F8B4
	public void Detach()
	{
		GorillaHandNode gorillaHandNode;
		this.Detach(out gorillaHandNode);
	}

	// Token: 0x060037EE RID: 14318 RVA: 0x001316CC File Offset: 0x0012F8CC
	public void Detach(out GorillaHandNode hand)
	{
		if (this._inUse)
		{
			this._inUse = false;
		}
		if (this._attachedHand == null)
		{
			hand = null;
			return;
		}
		hand = this._attachedHand;
		hand.attachedToSocket = null;
		this._attachedHand = null;
		this.OnHandDetach();
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x060037EF RID: 14319 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnHandAttach()
	{
	}

	// Token: 0x060037F0 RID: 14320 RVA: 0x000028C5 File Offset: 0x00000AC5
	protected virtual void OnHandDetach()
	{
	}

	// Token: 0x060037F1 RID: 14321 RVA: 0x00131722 File Offset: 0x0012F922
	protected virtual void OnUpdateAttached()
	{
		this._attachedHand.transform.position = base.transform.position;
	}

	// Token: 0x060037F2 RID: 14322 RVA: 0x0013173F File Offset: 0x0012F93F
	private void OnEnable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.TryAdd(this.collider, this);
	}

	// Token: 0x060037F3 RID: 14323 RVA: 0x00131762 File Offset: 0x0012F962
	private void OnDisable()
	{
		if (this.collider == null)
		{
			return;
		}
		GorillaHandSocket.gColliderToSocket.Remove(this.collider);
	}

	// Token: 0x060037F4 RID: 14324 RVA: 0x00131784 File Offset: 0x0012F984
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060037F5 RID: 14325 RVA: 0x0013178C File Offset: 0x0012F98C
	private void FixedUpdate()
	{
		if (!this._inUse)
		{
			return;
		}
		if (!this._attachedHand)
		{
			return;
		}
		this.OnUpdateAttached();
	}

	// Token: 0x060037F6 RID: 14326 RVA: 0x001317AC File Offset: 0x0012F9AC
	private void Setup()
	{
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(UnityTag.GorillaHandSocket);
		base.gameObject.SetLayer(UnityLayer.GorillaHandSocket);
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
		this._sinceSocketStateChange = TimeSince.Now();
	}

	// Token: 0x040047D4 RID: 18388
	public Collider collider;

	// Token: 0x040047D5 RID: 18389
	public float attachCooldown = 0.5f;

	// Token: 0x040047D6 RID: 18390
	public HandSocketConstraint constraint;

	// Token: 0x040047D7 RID: 18391
	[NonSerialized]
	private GorillaHandNode _attachedHand;

	// Token: 0x040047D8 RID: 18392
	[NonSerialized]
	private bool _inUse;

	// Token: 0x040047D9 RID: 18393
	[NonSerialized]
	private TimeSince _sinceSocketStateChange;

	// Token: 0x040047DA RID: 18394
	private static readonly Dictionary<Collider, GorillaHandSocket> gColliderToSocket = new Dictionary<Collider, GorillaHandSocket>(64);
}
