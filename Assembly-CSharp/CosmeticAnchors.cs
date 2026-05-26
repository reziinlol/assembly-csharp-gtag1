using System;
using GorillaExtensions;
using GorillaNetworking;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200051C RID: 1308
public class CosmeticAnchors : MonoBehaviour, ISpawnable
{
	// Token: 0x17000387 RID: 903
	// (get) Token: 0x060020C4 RID: 8388 RVA: 0x000AF64C File Offset: 0x000AD84C
	// (set) Token: 0x060020C5 RID: 8389 RVA: 0x000AF654 File Offset: 0x000AD854
	bool ISpawnable.IsSpawned { get; set; }

	// Token: 0x17000388 RID: 904
	// (get) Token: 0x060020C6 RID: 8390 RVA: 0x000AF65D File Offset: 0x000AD85D
	// (set) Token: 0x060020C7 RID: 8391 RVA: 0x000AF665 File Offset: 0x000AD865
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060020C8 RID: 8392 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnSpawn(VRRig rig)
	{
	}

	// Token: 0x060020C9 RID: 8393 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060020CA RID: 8394 RVA: 0x000AF670 File Offset: 0x000AD870
	private void AssignAnchorToPath(ref GameObject anchorGObjRef, string path)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		Transform transform;
		if (!base.transform.TryFindByPath(path, out transform, false))
		{
			this.vrRig = base.GetComponentInParent<VRRig>(true);
			if (this.vrRig && this.vrRig.isOfflineVRRig)
			{
				Debug.LogError("CosmeticAnchors: Could not find path: \"" + path + "\".\nPath to this component: " + base.transform.GetPathQ(), this);
			}
			return;
		}
		anchorGObjRef = transform.gameObject;
	}

	// Token: 0x060020CB RID: 8395 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnEnable()
	{
	}

	// Token: 0x060020CC RID: 8396 RVA: 0x000028C5 File Offset: 0x00000AC5
	private void OnDisable()
	{
	}

	// Token: 0x060020CD RID: 8397 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void TryUpdate()
	{
	}

	// Token: 0x060020CE RID: 8398 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void EnableAnchor(bool enable)
	{
	}

	// Token: 0x060020CF RID: 8399 RVA: 0x000AF6E8 File Offset: 0x000AD8E8
	private void SetHuntComputerAnchor(bool enable)
	{
		Transform huntComputer = this.anchorOverrides.HuntComputer;
		if (!GorillaTagger.Instance.offlineVRRig.huntComputer.activeSelf || !enable)
		{
			huntComputer.parent = this.anchorOverrides.HuntDefaultAnchor;
		}
		else
		{
			huntComputer.parent = this.huntComputerAnchor.transform;
		}
		huntComputer.transform.localPosition = Vector3.zero;
		huntComputer.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060020D0 RID: 8400 RVA: 0x000AF760 File Offset: 0x000AD960
	private void SetBuilderWatchAnchor(bool enable)
	{
		Transform builderWatch = this.anchorOverrides.BuilderWatch;
		if (!GorillaTagger.Instance.offlineVRRig.builderResizeWatch.activeSelf || !enable)
		{
			builderWatch.parent = this.anchorOverrides.BuilderWatchAnchor;
		}
		else
		{
			builderWatch.parent = this.builderWatchAnchor.transform;
		}
		builderWatch.transform.localPosition = Vector3.zero;
		builderWatch.transform.localRotation = Quaternion.identity;
	}

	// Token: 0x060020D1 RID: 8401 RVA: 0x000AF7D8 File Offset: 0x000AD9D8
	private void SetCustomAnchor(Transform target, bool enable, GameObject overrideAnchor, Transform defaultAnchor)
	{
		Transform transform = (enable && overrideAnchor != null) ? overrideAnchor.transform : defaultAnchor;
		if (target != null && target.parent != transform)
		{
			target.parent = transform;
			target.transform.localPosition = Vector3.zero;
			target.transform.localRotation = Quaternion.identity;
			target.transform.localScale = Vector3.one;
		}
	}

	// Token: 0x060020D2 RID: 8402 RVA: 0x000AF84C File Offset: 0x000ADA4C
	public Transform GetPositionAnchor(TransferrableObject.PositionState pos)
	{
		if (pos != TransferrableObject.PositionState.OnLeftArm)
		{
			if (pos != TransferrableObject.PositionState.OnRightArm)
			{
				if (pos != TransferrableObject.PositionState.OnChest)
				{
					return null;
				}
				if (!this.chestAnchor)
				{
					return null;
				}
				return this.chestAnchor.transform;
			}
			else
			{
				if (!this.rightArmAnchor)
				{
					return null;
				}
				return this.rightArmAnchor.transform;
			}
		}
		else
		{
			if (!this.leftArmAnchor)
			{
				return null;
			}
			return this.leftArmAnchor.transform;
		}
	}

	// Token: 0x060020D3 RID: 8403 RVA: 0x000AF8BA File Offset: 0x000ADABA
	public Transform GetNameAnchor()
	{
		if (!this.nameAnchor)
		{
			return null;
		}
		return this.nameAnchor.transform;
	}

	// Token: 0x060020D4 RID: 8404 RVA: 0x000AF8D6 File Offset: 0x000ADAD6
	public bool AffectedByHunt()
	{
		return this.huntComputerAnchor != null;
	}

	// Token: 0x060020D5 RID: 8405 RVA: 0x000AF8E4 File Offset: 0x000ADAE4
	public bool AffectedByBuilder()
	{
		return this.builderWatchAnchor != null;
	}

	// Token: 0x04002B6D RID: 11117
	[SerializeField]
	private bool deprecatedWarning = true;

	// Token: 0x04002B6E RID: 11118
	[SerializeField]
	protected GameObject nameAnchor;

	// Token: 0x04002B6F RID: 11119
	[SerializeField]
	protected string nameAnchor_path;

	// Token: 0x04002B70 RID: 11120
	[SerializeField]
	protected GameObject leftArmAnchor;

	// Token: 0x04002B71 RID: 11121
	[SerializeField]
	protected string leftArmAnchor_path;

	// Token: 0x04002B72 RID: 11122
	[SerializeField]
	protected GameObject rightArmAnchor;

	// Token: 0x04002B73 RID: 11123
	[SerializeField]
	protected string rightArmAnchor_path;

	// Token: 0x04002B74 RID: 11124
	[SerializeField]
	protected GameObject chestAnchor;

	// Token: 0x04002B75 RID: 11125
	[SerializeField]
	protected string chestAnchor_path;

	// Token: 0x04002B76 RID: 11126
	[SerializeField]
	protected GameObject huntComputerAnchor;

	// Token: 0x04002B77 RID: 11127
	[SerializeField]
	protected string huntComputerAnchor_path;

	// Token: 0x04002B78 RID: 11128
	[SerializeField]
	protected GameObject builderWatchAnchor;

	// Token: 0x04002B79 RID: 11129
	[SerializeField]
	protected string builderWatchAnchor_path;

	// Token: 0x04002B7A RID: 11130
	[SerializeField]
	protected GameObject friendshipBraceletLeftOverride;

	// Token: 0x04002B7B RID: 11131
	[SerializeField]
	protected string friendshipBraceletLeftOverride_path;

	// Token: 0x04002B7C RID: 11132
	[SerializeField]
	protected GameObject friendshipBraceletRightOverride;

	// Token: 0x04002B7D RID: 11133
	[SerializeField]
	protected string friendshipBraceletRightOverride_path;

	// Token: 0x04002B7E RID: 11134
	[SerializeField]
	protected GameObject badgeAnchor;

	// Token: 0x04002B7F RID: 11135
	[SerializeField]
	protected string badgeAnchor_path;

	// Token: 0x04002B80 RID: 11136
	[SerializeField]
	public CosmeticsController.CosmeticSlots slot;

	// Token: 0x04002B81 RID: 11137
	private VRRig vrRig;

	// Token: 0x04002B82 RID: 11138
	private VRRigAnchorOverrides anchorOverrides;

	// Token: 0x04002B83 RID: 11139
	private bool anchorEnabled;

	// Token: 0x04002B84 RID: 11140
	private static GTLogErrorLimiter k_debugLogError_anchorOverridesNull = new GTLogErrorLimiter("The array `anchorOverrides` was null. Is the cosmetic getting initialized properly? ", 10, "\n- ");
}
