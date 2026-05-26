using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000908 RID: 2312
public class SizeChanger : GorillaTriggerBox
{
	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x06003C67 RID: 15463 RVA: 0x00149824 File Offset: 0x00147A24
	public int SizeLayerMask
	{
		get
		{
			int num = 0;
			if (this.affectLayerA)
			{
				num |= 1;
			}
			if (this.affectLayerB)
			{
				num |= 2;
			}
			if (this.affectLayerC)
			{
				num |= 4;
			}
			if (this.affectLayerD)
			{
				num |= 8;
			}
			return num;
		}
	}

	// Token: 0x17000572 RID: 1394
	// (get) Token: 0x06003C68 RID: 15464 RVA: 0x00149864 File Offset: 0x00147A64
	public SizeChanger.ChangerType MyType
	{
		get
		{
			return this.myType;
		}
	}

	// Token: 0x17000573 RID: 1395
	// (get) Token: 0x06003C69 RID: 15465 RVA: 0x0014986C File Offset: 0x00147A6C
	public float MaxScale
	{
		get
		{
			return this.maxScale;
		}
	}

	// Token: 0x17000574 RID: 1396
	// (get) Token: 0x06003C6A RID: 15466 RVA: 0x00149874 File Offset: 0x00147A74
	public float MinScale
	{
		get
		{
			return this.minScale;
		}
	}

	// Token: 0x17000575 RID: 1397
	// (get) Token: 0x06003C6B RID: 15467 RVA: 0x0014987C File Offset: 0x00147A7C
	public Transform StartPos
	{
		get
		{
			return this.startPos;
		}
	}

	// Token: 0x17000576 RID: 1398
	// (get) Token: 0x06003C6C RID: 15468 RVA: 0x00149884 File Offset: 0x00147A84
	public Transform EndPos
	{
		get
		{
			return this.endPos;
		}
	}

	// Token: 0x17000577 RID: 1399
	// (get) Token: 0x06003C6D RID: 15469 RVA: 0x0014988C File Offset: 0x00147A8C
	public float StaticEasing
	{
		get
		{
			return this.staticEasing;
		}
	}

	// Token: 0x06003C6E RID: 15470 RVA: 0x00149894 File Offset: 0x00147A94
	private void Awake()
	{
		this.minScale = Mathf.Max(this.minScale, 0.01f);
		this.myCollider = base.GetComponent<Collider>();
	}

	// Token: 0x06003C6F RID: 15471 RVA: 0x001498B8 File Offset: 0x00147AB8
	public void OnEnable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter += this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit += this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x06003C70 RID: 15472 RVA: 0x00149934 File Offset: 0x00147B34
	public void OnDisable()
	{
		if (this.enterTrigger)
		{
			this.enterTrigger.OnEnter -= this.OnTriggerEnter;
		}
		if (this.exitTrigger)
		{
			this.exitTrigger.OnExit -= this.OnTriggerExit;
		}
		if (this.exitOnEnterTrigger)
		{
			this.exitOnEnterTrigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x06003C71 RID: 15473 RVA: 0x001499AD File Offset: 0x00147BAD
	public void AddEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerEnter;
		}
	}

	// Token: 0x06003C72 RID: 15474 RVA: 0x001499C9 File Offset: 0x00147BC9
	public void RemoveEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerEnter;
		}
	}

	// Token: 0x06003C73 RID: 15475 RVA: 0x001499E5 File Offset: 0x00147BE5
	public void AddExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter += this.OnTriggerExit;
		}
	}

	// Token: 0x06003C74 RID: 15476 RVA: 0x00149A01 File Offset: 0x00147C01
	public void RemoveExitOnEnterTrigger(SizeChangerTrigger trigger)
	{
		if (trigger)
		{
			trigger.OnEnter -= this.OnTriggerExit;
		}
	}

	// Token: 0x06003C75 RID: 15477 RVA: 0x00149A20 File Offset: 0x00147C20
	public void OnTriggerEnter(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.acceptRig(component);
	}

	// Token: 0x06003C76 RID: 15478 RVA: 0x00149A5D File Offset: 0x00147C5D
	public void acceptRig(VRRig rig)
	{
		if (!rig.sizeManager.touchingChangers.Contains(this))
		{
			rig.sizeManager.touchingChangers.Add(this);
		}
		UnityAction onEnter = this.OnEnter;
		if (onEnter == null)
		{
			return;
		}
		onEnter();
	}

	// Token: 0x06003C77 RID: 15479 RVA: 0x00149A94 File Offset: 0x00147C94
	public void OnTriggerExit(Collider other)
	{
		if (!other.GetComponent<SphereCollider>())
		{
			return;
		}
		VRRig component = other.attachedRigidbody.gameObject.GetComponent<VRRig>();
		if (component == null)
		{
			return;
		}
		this.unacceptRig(component);
	}

	// Token: 0x06003C78 RID: 15480 RVA: 0x00149AD1 File Offset: 0x00147CD1
	public void unacceptRig(VRRig rig)
	{
		rig.sizeManager.touchingChangers.Remove(this);
		UnityAction onExit = this.OnExit;
		if (onExit == null)
		{
			return;
		}
		onExit();
	}

	// Token: 0x06003C79 RID: 15481 RVA: 0x00149AF8 File Offset: 0x00147CF8
	public Vector3 ClosestPoint(Vector3 position)
	{
		if (this.enterTrigger && this.exitTrigger)
		{
			Vector3 vector = this.enterTrigger.ClosestPoint(position);
			Vector3 vector2 = this.exitTrigger.ClosestPoint(position);
			if (Vector3.Distance(position, vector) >= Vector3.Distance(position, vector2))
			{
				return vector2;
			}
			return vector;
		}
		else
		{
			if (this.myCollider)
			{
				return this.myCollider.ClosestPoint(position);
			}
			return position;
		}
	}

	// Token: 0x06003C7A RID: 15482 RVA: 0x00149B68 File Offset: 0x00147D68
	public void SetScaleCenterPoint(Transform centerPoint)
	{
		this.scaleAwayFromPoint = centerPoint;
	}

	// Token: 0x06003C7B RID: 15483 RVA: 0x00149B71 File Offset: 0x00147D71
	public bool TryGetScaleCenterPoint(out Vector3 centerPoint)
	{
		if (this.scaleAwayFromPoint != null)
		{
			centerPoint = this.scaleAwayFromPoint.position;
			return true;
		}
		centerPoint = Vector3.zero;
		return false;
	}

	// Token: 0x04004D12 RID: 19730
	[SerializeField]
	private SizeChanger.ChangerType myType;

	// Token: 0x04004D13 RID: 19731
	[SerializeField]
	private float staticEasing;

	// Token: 0x04004D14 RID: 19732
	[SerializeField]
	private float maxScale;

	// Token: 0x04004D15 RID: 19733
	[SerializeField]
	private float minScale;

	// Token: 0x04004D16 RID: 19734
	private Collider myCollider;

	// Token: 0x04004D17 RID: 19735
	[SerializeField]
	private Transform startPos;

	// Token: 0x04004D18 RID: 19736
	[SerializeField]
	private Transform endPos;

	// Token: 0x04004D19 RID: 19737
	[SerializeField]
	private SizeChangerTrigger enterTrigger;

	// Token: 0x04004D1A RID: 19738
	[SerializeField]
	private SizeChangerTrigger exitTrigger;

	// Token: 0x04004D1B RID: 19739
	[SerializeField]
	private Transform scaleAwayFromPoint;

	// Token: 0x04004D1C RID: 19740
	[SerializeField]
	private SizeChangerTrigger exitOnEnterTrigger;

	// Token: 0x04004D1D RID: 19741
	public bool alwaysControlWhenEntered;

	// Token: 0x04004D1E RID: 19742
	public int priority;

	// Token: 0x04004D1F RID: 19743
	public bool aprilFoolsEnabled;

	// Token: 0x04004D20 RID: 19744
	public float startRadius;

	// Token: 0x04004D21 RID: 19745
	public float endRadius;

	// Token: 0x04004D22 RID: 19746
	public bool affectLayerA = true;

	// Token: 0x04004D23 RID: 19747
	public bool affectLayerB = true;

	// Token: 0x04004D24 RID: 19748
	public bool affectLayerC = true;

	// Token: 0x04004D25 RID: 19749
	public bool affectLayerD = true;

	// Token: 0x04004D26 RID: 19750
	public UnityAction OnExit;

	// Token: 0x04004D27 RID: 19751
	public UnityAction OnEnter;

	// Token: 0x04004D28 RID: 19752
	private HashSet<VRRig> unregisteredPresentRigs;

	// Token: 0x02000909 RID: 2313
	public enum ChangerType
	{
		// Token: 0x04004D2A RID: 19754
		Static,
		// Token: 0x04004D2B RID: 19755
		Continuous,
		// Token: 0x04004D2C RID: 19756
		Radius
	}
}
