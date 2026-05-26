using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x020004AE RID: 1198
public class InteractionPoint : MonoBehaviour, ISpawnable, IBuildValidation
{
	// Token: 0x17000319 RID: 793
	// (get) Token: 0x06001D2F RID: 7471 RVA: 0x0009E08F File Offset: 0x0009C28F
	// (set) Token: 0x06001D30 RID: 7472 RVA: 0x0009E097 File Offset: 0x0009C297
	public bool ignoreLeftHand { get; private set; }

	// Token: 0x1700031A RID: 794
	// (get) Token: 0x06001D31 RID: 7473 RVA: 0x0009E0A0 File Offset: 0x0009C2A0
	// (set) Token: 0x06001D32 RID: 7474 RVA: 0x0009E0A8 File Offset: 0x0009C2A8
	public bool ignoreRightHand { get; private set; }

	// Token: 0x1700031B RID: 795
	// (get) Token: 0x06001D33 RID: 7475 RVA: 0x0009E0B1 File Offset: 0x0009C2B1
	public IHoldableObject Holdable
	{
		get
		{
			return this.parentHoldable;
		}
	}

	// Token: 0x1700031C RID: 796
	// (get) Token: 0x06001D34 RID: 7476 RVA: 0x0009E0B9 File Offset: 0x0009C2B9
	// (set) Token: 0x06001D35 RID: 7477 RVA: 0x0009E0C1 File Offset: 0x0009C2C1
	public bool IsSpawned { get; set; }

	// Token: 0x1700031D RID: 797
	// (get) Token: 0x06001D36 RID: 7478 RVA: 0x0009E0CA File Offset: 0x0009C2CA
	// (set) Token: 0x06001D37 RID: 7479 RVA: 0x0009E0D2 File Offset: 0x0009C2D2
	public ECosmeticSelectSide CosmeticSelectedSide { get; set; }

	// Token: 0x06001D38 RID: 7480 RVA: 0x0009E0DC File Offset: 0x0009C2DC
	public void OnSpawn(VRRig rig)
	{
		if (!this.IsSpawned)
		{
			this.IsSpawned = true;
		}
		this.interactor = EquipmentInteractor.instance;
		this.myCollider = base.GetComponent<Collider>();
		if (this.parentHoldableObject != null)
		{
			this.parentHoldable = this.parentHoldableObject.GetComponent<IHoldableObject>();
		}
		else
		{
			this.parentHoldable = base.GetComponentInParent<IHoldableObject>(true);
			this.parentHoldableObject = this.parentHoldable.gameObject;
		}
		if (this.parentHoldable == null)
		{
			if (this.parentHoldableObject == null)
			{
				Debug.LogError("InteractionPoint: Disabling because expected field `parentHoldableObject` is null. Path=" + base.transform.GetPathQ());
				base.enabled = false;
				return;
			}
			Debug.LogError("InteractionPoint: Disabling because `parentHoldableObject` does not have a IHoldableObject component. Path=" + base.transform.GetPathQ());
		}
		TransferrableObject transferrableObject = this.parentHoldable as TransferrableObject;
		this.forLocalPlayer = (transferrableObject == null || transferrableObject.IsLocalObject() || transferrableObject.isSceneObject || transferrableObject.canDrop);
	}

	// Token: 0x06001D39 RID: 7481 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x06001D3A RID: 7482 RVA: 0x0009E1D6 File Offset: 0x0009C3D6
	private void Awake()
	{
		if (this.isNonSpawnedObject)
		{
			this.OnSpawn(null);
		}
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x0009E1E7 File Offset: 0x0009C3E7
	private void OnEnable()
	{
		this.wasInLeft = false;
		this.wasInRight = false;
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x0009E1F7 File Offset: 0x0009C3F7
	public void OnDisable()
	{
		if (!this.forLocalPlayer || this.interactor == null)
		{
			return;
		}
		this.interactor.InteractionPointDisabled(this);
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x0009E21C File Offset: 0x0009C41C
	protected void LateUpdate()
	{
		if (!this.IsSpawned)
		{
			return;
		}
		if (!this.forLocalPlayer)
		{
			base.enabled = false;
			if (this.myCollider.IsNotNull())
			{
				this.myCollider.enabled = false;
			}
			return;
		}
		if (this.interactor == null)
		{
			this.interactor = EquipmentInteractor.instance;
			return;
		}
		if (this.interactionRadius > 0f || this.myCollider != null)
		{
			if (!this.ignoreLeftHand && this.OverlapCheck(this.interactor.leftHand.transform.position) != this.wasInLeft)
			{
				if (!this.wasInLeft && !this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Add(this);
					this.wasInLeft = true;
				}
				else if (this.wasInLeft && this.interactor.overlapInteractionPointsLeft.Contains(this))
				{
					this.interactor.overlapInteractionPointsLeft.Remove(this);
					this.wasInLeft = false;
				}
			}
			if (!this.ignoreRightHand && this.OverlapCheck(this.interactor.rightHand.transform.position) != this.wasInRight)
			{
				if (!this.wasInRight && !this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Add(this);
					this.wasInRight = true;
					return;
				}
				if (this.wasInRight && this.interactor.overlapInteractionPointsRight.Contains(this))
				{
					this.interactor.overlapInteractionPointsRight.Remove(this);
					this.wasInRight = false;
				}
			}
		}
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x0009E3C4 File Offset: 0x0009C5C4
	public bool OverlapCheck(Vector3 point)
	{
		if (this.interactionRadius > 0f)
		{
			return (base.transform.position - point).IsShorterThan(this.interactionRadius * base.transform.lossyScale);
		}
		return this.myCollider != null && this.myCollider.bounds.Contains(point);
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x00023994 File Offset: 0x00021B94
	public bool BuildValidationCheck()
	{
		return true;
	}

	// Token: 0x04002765 RID: 10085
	[SerializeField]
	[FormerlySerializedAs("parentTransferrableObject")]
	public GameObject parentHoldableObject;

	// Token: 0x04002766 RID: 10086
	private IHoldableObject parentHoldable;

	// Token: 0x04002769 RID: 10089
	[SerializeField]
	private bool isNonSpawnedObject;

	// Token: 0x0400276A RID: 10090
	[SerializeField]
	private float interactionRadius;

	// Token: 0x0400276B RID: 10091
	public Collider myCollider;

	// Token: 0x0400276C RID: 10092
	public EquipmentInteractor interactor;

	// Token: 0x0400276D RID: 10093
	public bool wasInLeft;

	// Token: 0x0400276E RID: 10094
	public bool wasInRight;

	// Token: 0x0400276F RID: 10095
	public bool forLocalPlayer;
}
