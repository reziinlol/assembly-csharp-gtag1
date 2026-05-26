using System;
using GorillaExtensions;
using GorillaTag;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x02000349 RID: 841
public class GTPosRotConstraints : MonoBehaviour, ISpawnable
{
	// Token: 0x060014B1 RID: 5297 RVA: 0x0006E73C File Offset: 0x0006C93C
	public void Awake()
	{
		if (this._shouldCallOnSpawnDuringAwake)
		{
			VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
			if (componentInParent == null)
			{
				return;
			}
			((ISpawnable)this).OnSpawn(componentInParent);
		}
	}

	// Token: 0x1700020D RID: 525
	// (get) Token: 0x060014B2 RID: 5298 RVA: 0x0006E76C File Offset: 0x0006C96C
	// (set) Token: 0x060014B3 RID: 5299 RVA: 0x0006E774 File Offset: 0x0006C974
	public bool IsSpawned { get; set; }

	// Token: 0x1700020E RID: 526
	// (get) Token: 0x060014B4 RID: 5300 RVA: 0x0006E77D File Offset: 0x0006C97D
	// (set) Token: 0x060014B5 RID: 5301 RVA: 0x0006E785 File Offset: 0x0006C985
	ECosmeticSelectSide ISpawnable.CosmeticSelectedSide { get; set; }

	// Token: 0x060014B6 RID: 5302 RVA: 0x0006E790 File Offset: 0x0006C990
	void ISpawnable.OnSpawn(VRRig rig)
	{
		Transform[] array = Array.Empty<Transform>();
		string str;
		if (rig != null && !GTHardCodedBones.TryGetBoneXforms(rig, out array, out str))
		{
			Debug.LogError("GTPosRotConstraints: Error getting bone Transforms: " + str, this);
			return;
		}
		for (int i = 0; i < this.constraints.Length; i++)
		{
			GorillaPosRotConstraint gorillaPosRotConstraint = this.constraints[i];
			if (Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.x, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.y, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.z, 0f) && Mathf.Approximately(gorillaPosRotConstraint.rotationOffset.w, 0f))
			{
				gorillaPosRotConstraint.rotationOffset = Quaternion.identity;
			}
			if (!gorillaPosRotConstraint.follower)
			{
				Debug.LogError(string.Concat(new string[]
				{
					string.Format("{0}: Disabling component! At index {1}, Transform `follower` is ", "GTPosRotConstraints", i),
					"null. Affected component path: ",
					base.transform.GetPathQ(),
					"\n- Affected component path: ",
					base.transform.GetPathQ()
				}), this);
				base.enabled = false;
				return;
			}
			if (gorillaPosRotConstraint.sourceGorillaBone == GTHardCodedBones.EBone.None)
			{
				if (!gorillaPosRotConstraint.source)
				{
					if (string.IsNullOrEmpty(gorillaPosRotConstraint.sourceRelativePath))
					{
						Debug.LogError(string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i) + "null, not EBone, and `sourceRelativePath` is null or empty.\n- Affected component path: " + base.transform.GetPathQ(), this);
						base.enabled = false;
						return;
					}
					if (!base.transform.TryFindByPath(gorillaPosRotConstraint.sourceRelativePath, out gorillaPosRotConstraint.source, false))
					{
						Debug.LogError(string.Concat(new string[]
						{
							string.Format("{0}: Disabling component! At index {1} Transform `source` is ", "GTPosRotConstraints", i),
							"null, not EBone, and could not find by path: \"",
							gorillaPosRotConstraint.sourceRelativePath,
							"\"\n- Affected component path: ",
							base.transform.GetPathQ()
						}), this);
						base.enabled = false;
						return;
					}
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
			else
			{
				if (rig == null)
				{
					Debug.LogError("GTPosRotConstraints: Disabling component! `VRRig` could not be found in parents, but " + string.Format("bone at index {0} is set to use EBone `{1}` but without `VRRig` it cannot ", i, gorillaPosRotConstraint.sourceGorillaBone) + "be resolved.\n- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				int boneIndex = GTHardCodedBones.GetBoneIndex(gorillaPosRotConstraint.sourceGorillaBone);
				if (boneIndex <= 0)
				{
					Debug.LogError(string.Format("{0}: (should never happen) Disabling component! At index {1}, could ", "GTPosRotConstraints", i) + string.Format("not find EBone `{0}`.\n", gorillaPosRotConstraint.sourceGorillaBone) + "- Affected component path: " + base.transform.GetPathQ(), this);
					base.enabled = false;
					return;
				}
				gorillaPosRotConstraint.source = array[boneIndex];
				if (!gorillaPosRotConstraint.source)
				{
					Debug.LogError(string.Concat(new string[]
					{
						string.Format("{0}: Disabling component! At index {1}, bone {2} was ", "GTPosRotConstraints", i, gorillaPosRotConstraint.sourceGorillaBone),
						"not present in `VRRig` path: ",
						rig.transform.GetPathQ(),
						"\n- Affected component path: ",
						base.transform.GetPathQ()
					}), this);
					base.enabled = false;
					return;
				}
				this.constraints[i] = gorillaPosRotConstraint;
			}
		}
		if (base.isActiveAndEnabled && !this._registerOnEnable)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x060014B7 RID: 5303 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ISpawnable.OnDespawn()
	{
	}

	// Token: 0x060014B8 RID: 5304 RVA: 0x0006EB07 File Offset: 0x0006CD07
	protected void OnEnable()
	{
		if (this.IsSpawned || this._registerOnEnable)
		{
			GTPosRotConstraintManager.Register(this);
		}
	}

	// Token: 0x060014B9 RID: 5305 RVA: 0x0006EB1F File Offset: 0x0006CD1F
	protected void OnDisable()
	{
		GTPosRotConstraintManager.Unregister(this);
	}

	// Token: 0x04001968 RID: 6504
	[SerializeField]
	private bool _shouldCallOnSpawnDuringAwake;

	// Token: 0x04001969 RID: 6505
	[Tooltip("Used for actors that get disabled and re-enabled")]
	[SerializeField]
	private bool _registerOnEnable;

	// Token: 0x0400196A RID: 6506
	public GorillaPosRotConstraint[] constraints;
}
