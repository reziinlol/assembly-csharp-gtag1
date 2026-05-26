using System;
using UnityEngine;

// Token: 0x02000861 RID: 2145
public class GorillaHandNode : MonoBehaviour
{
	// Token: 0x170004ED RID: 1261
	// (get) Token: 0x060037DC RID: 14300 RVA: 0x001313F0 File Offset: 0x0012F5F0
	public bool isGripping
	{
		get
		{
			return this.PollGrip();
		}
	}

	// Token: 0x170004EE RID: 1262
	// (get) Token: 0x060037DD RID: 14301 RVA: 0x001313F8 File Offset: 0x0012F5F8
	public bool isLeftHand
	{
		get
		{
			return this._isLeftHand;
		}
	}

	// Token: 0x170004EF RID: 1263
	// (get) Token: 0x060037DE RID: 14302 RVA: 0x00131400 File Offset: 0x0012F600
	public bool isRightHand
	{
		get
		{
			return this._isRightHand;
		}
	}

	// Token: 0x060037DF RID: 14303 RVA: 0x00131408 File Offset: 0x0012F608
	private void Awake()
	{
		this.Setup();
	}

	// Token: 0x060037E0 RID: 14304 RVA: 0x00131410 File Offset: 0x0012F610
	private bool PollGrip()
	{
		if (this.rig == null)
		{
			return false;
		}
		bool flag = this.PollThumb() >= 0.25f;
		bool flag2 = this.PollIndex() >= 0.25f;
		bool flag3 = this.PollMiddle() >= 0.25f;
		return flag && flag2 && flag3;
	}

	// Token: 0x060037E1 RID: 14305 RVA: 0x00131464 File Offset: 0x0012F664
	private void Setup()
	{
		if (this.rig == null)
		{
			this.rig = base.GetComponentInParent<VRRig>();
		}
		if (this.rigidbody == null)
		{
			this.rigidbody = base.GetComponent<Rigidbody>();
		}
		if (this.collider == null)
		{
			this.collider = base.GetComponent<Collider>();
		}
		if (this.rig)
		{
			this.vrIndex = (this._isLeftHand ? this.rig.leftIndex : this.rig.rightIndex);
			this.vrThumb = (this._isLeftHand ? this.rig.leftThumb : this.rig.rightThumb);
			this.vrMiddle = (this._isLeftHand ? this.rig.leftMiddle : this.rig.rightMiddle);
		}
		this._isLeftHand = base.name.Contains("left", StringComparison.OrdinalIgnoreCase);
		this._isRightHand = base.name.Contains("right", StringComparison.OrdinalIgnoreCase);
		int num = 0;
		num |= 1024;
		num |= 2097152;
		num |= 16777216;
		base.gameObject.SetTag(this._isLeftHand ? UnityTag.GorillaHandLeft : UnityTag.GorillaHandRight);
		base.gameObject.SetLayer(UnityLayer.GorillaHand);
		this.rigidbody.includeLayers = num;
		this.rigidbody.excludeLayers = ~num;
		this.rigidbody.isKinematic = true;
		this.rigidbody.useGravity = false;
		this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		this.collider.isTrigger = true;
		this.collider.includeLayers = num;
		this.collider.excludeLayers = ~num;
	}

	// Token: 0x060037E2 RID: 14306 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnTriggerStay(Collider other)
	{
	}

	// Token: 0x060037E3 RID: 14307 RVA: 0x00131623 File Offset: 0x0012F823
	private float PollIndex()
	{
		return Mathf.Clamp01(this.vrIndex.calcT / 0.88f);
	}

	// Token: 0x060037E4 RID: 14308 RVA: 0x0013163B File Offset: 0x0012F83B
	private float PollMiddle()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x060037E5 RID: 14309 RVA: 0x0013163B File Offset: 0x0012F83B
	private float PollThumb()
	{
		return this.vrIndex.calcT;
	}

	// Token: 0x040047CA RID: 18378
	public VRRig rig;

	// Token: 0x040047CB RID: 18379
	public Collider collider;

	// Token: 0x040047CC RID: 18380
	public Rigidbody rigidbody;

	// Token: 0x040047CD RID: 18381
	[Space]
	[NonSerialized]
	public VRMapIndex vrIndex;

	// Token: 0x040047CE RID: 18382
	[NonSerialized]
	public VRMapThumb vrThumb;

	// Token: 0x040047CF RID: 18383
	[NonSerialized]
	public VRMapMiddle vrMiddle;

	// Token: 0x040047D0 RID: 18384
	[Space]
	public GorillaHandSocket attachedToSocket;

	// Token: 0x040047D1 RID: 18385
	[Space]
	[SerializeField]
	private bool _isLeftHand;

	// Token: 0x040047D2 RID: 18386
	[SerializeField]
	private bool _isRightHand;

	// Token: 0x040047D3 RID: 18387
	public bool ignoreSockets;
}
