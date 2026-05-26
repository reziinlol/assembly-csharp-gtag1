using System;
using System.Collections.Generic;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200027C RID: 636
public class PropHuntPropZone : MonoBehaviour, IDelayedExecListener
{
	// Token: 0x06001132 RID: 4402 RVA: 0x0005C63C File Offset: 0x0005A83C
	private void Awake()
	{
		this.hasBoxCollider = base.TryGetComponent<BoxCollider>(out this.boxCollider);
	}

	// Token: 0x06001133 RID: 4403 RVA: 0x0005C65B File Offset: 0x0005A85B
	private void OnEnable()
	{
		GorillaPropHuntGameManager.RegisterPropZone(this);
	}

	// Token: 0x06001134 RID: 4404 RVA: 0x0005C663 File Offset: 0x0005A863
	private void OnDisable()
	{
		this.DestroyDecoys();
		GorillaPropHuntGameManager.UnregisterPropZone(this);
	}

	// Token: 0x06001135 RID: 4405 RVA: 0x0005C674 File Offset: 0x0005A874
	public void DestroyDecoys()
	{
		foreach (PropPlacementRB propPlacementRB in this.propPlacementRBs)
		{
			if (propPlacementRB != null)
			{
				PropHuntPools.ReturnDecoyProp(propPlacementRB);
			}
		}
		this.propPlacementRBs.Clear();
	}

	// Token: 0x06001136 RID: 4406 RVA: 0x0005C6DC File Offset: 0x0005A8DC
	public void OnRoundStart()
	{
		if (!PropHuntPools.IsReady)
		{
			Debug.LogError("ERROR!!!  PropHuntPropZone: (this should never happen) props not ready to be spawned so aborting. you should only be calling this if `PropHuntPools.IsReady` is true or from the callback `PropHuntPools.OnReady`.");
		}
		this.CreateDecoys(GorillaPropHuntGameManager.instance.GetSeed());
	}

	// Token: 0x06001137 RID: 4407 RVA: 0x0005C700 File Offset: 0x0005A900
	public void CreateDecoys(int seed)
	{
		this.DestroyDecoys();
		SRand srand = new SRand(seed + this.seedOffset);
		for (int i = 0; i < this.numProps; i++)
		{
			PropPlacementRB propPlacementRB;
			if (!PropHuntPools.TryGetDecoyProp(GorillaPropHuntGameManager.instance.GetCosmeticId(srand.NextUInt()), out propPlacementRB))
			{
				return;
			}
			Vector3 position2;
			if (this.hasBoxCollider)
			{
				Vector3 position = new Vector3(srand.NextFloat(-this.boxCollider.size.x, this.boxCollider.size.x) / 2f, srand.NextFloat(-this.boxCollider.size.y, this.boxCollider.size.y) / 2f, srand.NextFloat(-this.boxCollider.size.z, this.boxCollider.size.z) / 2f);
				position2 = base.transform.TransformPoint(position);
			}
			else
			{
				position2 = base.transform.position + srand.NextPointInsideSphere(this.radius);
			}
			propPlacementRB.gameObject.SetActive(false);
			propPlacementRB.transform.SetParent(null, false);
			propPlacementRB.transform.position = position2;
			propPlacementRB.transform.rotation = Quaternion.Euler(srand.NextFloat(360f), srand.NextFloat(360f), srand.NextFloat(360f));
			propPlacementRB._placingProp.SetActive(false);
			propPlacementRB._placingProp.transform.SetParent(null, false);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		for (int j = 0; j < this.propPlacementRBs.Count; j++)
		{
			this.propPlacementRBs[j].gameObject.SetActive(true);
		}
		GTDelayedExec.Add(this, this.m_simDurationBeforeFreeze, 0);
	}

	// Token: 0x06001138 RID: 4408 RVA: 0x0005C8E4 File Offset: 0x0005AAE4
	public void OnDelayedAction(int contextId)
	{
		for (int i = 0; i < this.propPlacementRBs.Count; i++)
		{
			PropPlacementRB propPlacementRB = this.propPlacementRBs[i];
			propPlacementRB.gameObject.SetActive(false);
			Transform transform = propPlacementRB.transform;
			GameObject placingProp = propPlacementRB._placingProp;
			placingProp.transform.SetPositionAndRotation(transform.position, transform.rotation);
			placingProp.SetActive(true);
		}
	}

	// Token: 0x06001139 RID: 4409 RVA: 0x0005C948 File Offset: 0x0005AB48
	private PropPlacementRB _GetOrCreatePropPlacementObj_NoPool()
	{
		PropPlacementRB propPlacementRB;
		if (this.nextUnusedPropPlacement < this.propPlacementRBs.Count)
		{
			propPlacementRB = this.propPlacementRBs[this.nextUnusedPropPlacement];
		}
		else
		{
			propPlacementRB = Object.Instantiate<PropPlacementRB>(this.propPlacementPrefab, base.transform);
			this.propPlacementRBs.Add(propPlacementRB);
		}
		this.nextUnusedPropPlacement++;
		return propPlacementRB;
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0005C9A9 File Offset: 0x0005ABA9
	private void SpawnProp_NoPool(GTAssetRef<GameObject> item, Vector3 pos, Quaternion rot, CosmeticSO debugCosmeticSO)
	{
		this._GetOrCreatePropPlacementObj_NoPool().PlaceProp_NoPool(this, item, pos, rot, debugCosmeticSO);
	}

	// Token: 0x04001484 RID: 5252
	private const string preLog = "PropHuntPropZone: ";

	// Token: 0x04001485 RID: 5253
	private const string preLogEd = "(editor only log) PropHuntPropZone: ";

	// Token: 0x04001486 RID: 5254
	private const string preLogBeta = "(beta only log) PropHuntPropZone: ";

	// Token: 0x04001487 RID: 5255
	private const string preErr = "ERROR!!!  PropHuntPropZone: ";

	// Token: 0x04001488 RID: 5256
	private const string preErrEd = "ERROR!!!  (editor only log) PropHuntPropZone: ";

	// Token: 0x04001489 RID: 5257
	private const string preErrBeta = "ERROR!!!  (beta only log) PropHuntPropZone: ";

	// Token: 0x0400148A RID: 5258
	private const bool _k__GT_PROP_HUNT__USE_POOLING__ = true;

	// Token: 0x0400148B RID: 5259
	[SerializeField]
	private PropPlacementRB propPlacementPrefab;

	// Token: 0x0400148C RID: 5260
	[SerializeField]
	private int seedOffset;

	// Token: 0x0400148D RID: 5261
	[SerializeField]
	private float radius = 1f;

	// Token: 0x0400148E RID: 5262
	[SerializeField]
	private int numProps = 10;

	// Token: 0x0400148F RID: 5263
	[SerializeField]
	private float m_simDurationBeforeFreeze = 2f;

	// Token: 0x04001490 RID: 5264
	private BoxCollider boxCollider;

	// Token: 0x04001491 RID: 5265
	private bool hasBoxCollider;

	// Token: 0x04001492 RID: 5266
	private int nextUnusedPropPlacement;

	// Token: 0x04001493 RID: 5267
	private readonly List<PropPlacementRB> propPlacementRBs = new List<PropPlacementRB>(64);
}
