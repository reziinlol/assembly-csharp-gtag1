using System;
using GorillaExtensions;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x02000122 RID: 290
public class SIUpgradeXformOffsetter : MonoBehaviour
{
	// Token: 0x0600072B RID: 1835 RVA: 0x00028C14 File Offset: 0x00026E14
	protected void Awake()
	{
		if (this.m_superInfectionGadget == null)
		{
			Debug.LogError("[SIUpgradeXformOffsetter]  ERROR!!!  Awake: Disabling component because `m_superInfectionGadget` is null. Path=" + base.transform.GetPathQ(), this);
			base.enabled = false;
			return;
		}
		foreach (SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp siupgradeXformOffsetOp in this.m_upgradeXformOffsetOps)
		{
			if (!(siupgradeXformOffsetOp.xform != null) && !(siupgradeXformOffsetOp.targetXform != null))
			{
				Debug.LogError("[SIUpgradeXformOffsetter]  ERROR!!!  Awake: Disabling component because null reference in `m_upgradeXformOffsetOps` array. Path=" + base.transform.GetPathQ(), this);
				base.enabled = false;
				return;
			}
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x00028CAE File Offset: 0x00026EAE
	protected void OnEnable()
	{
		SIGadget superInfectionGadget = this.m_superInfectionGadget;
		superInfectionGadget.OnPostRefreshVisuals = (Action<SIUpgradeSet>)Delegate.Combine(superInfectionGadget.OnPostRefreshVisuals, new Action<SIUpgradeSet>(this._HandleGadgetOnPostRefreshVisuals));
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x00028CD7 File Offset: 0x00026ED7
	protected void OnDisable()
	{
		SIGadget superInfectionGadget = this.m_superInfectionGadget;
		superInfectionGadget.OnPostRefreshVisuals = (Action<SIUpgradeSet>)Delegate.Remove(superInfectionGadget.OnPostRefreshVisuals, new Action<SIUpgradeSet>(this._HandleGadgetOnPostRefreshVisuals));
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x00028D00 File Offset: 0x00026F00
	private void _HandleGadgetOnPostRefreshVisuals(SIUpgradeSet upgradeSet)
	{
		for (int i = 0; i < this.m_upgradeXformOffsetOps.Length; i++)
		{
			SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp siupgradeXformOffsetOp = this.m_upgradeXformOffsetOps[i];
			if (upgradeSet.Contains(siupgradeXformOffsetOp.upgradeType))
			{
				siupgradeXformOffsetOp.xform.SetLocalPositionAndRotation(siupgradeXformOffsetOp.targetXform.localPosition, siupgradeXformOffsetOp.targetXform.localRotation);
				siupgradeXformOffsetOp.xform.localScale = siupgradeXformOffsetOp.targetXform.localScale;
			}
		}
	}

	// Token: 0x04000970 RID: 2416
	private const string preLog = "[SIUpgradeXformOffsetter]  ";

	// Token: 0x04000971 RID: 2417
	private const string preErr = "[SIUpgradeXformOffsetter]  ERROR!!!  ";

	// Token: 0x04000972 RID: 2418
	[SerializeField]
	private SIGadget m_superInfectionGadget;

	// Token: 0x04000973 RID: 2419
	[SerializeField]
	private SIUpgradeXformOffsetter.SIUpgradeXformOffsetOp[] m_upgradeXformOffsetOps;

	// Token: 0x02000123 RID: 291
	[Serializable]
	public struct SIUpgradeXformOffsetOp
	{
		// Token: 0x04000974 RID: 2420
		public SIUpgradeType upgradeType;

		// Token: 0x04000975 RID: 2421
		public Transform xform;

		// Token: 0x04000976 RID: 2422
		[FormerlySerializedAs("newParent")]
		public Transform targetXform;
	}
}
