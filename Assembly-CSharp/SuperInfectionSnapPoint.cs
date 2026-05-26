using System;
using GorillaExtensions;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x020006ED RID: 1773
public class SuperInfectionSnapPoint : MonoBehaviour
{
	// Token: 0x06002CB4 RID: 11444 RVA: 0x000F1C44 File Offset: 0x000EFE44
	public void Initialize()
	{
		VRRig componentInParent = base.GetComponentInParent<VRRig>(true);
		if (componentInParent == null)
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  Expected a VRRig to be in parent hierarchy. Path=\"" + base.transform.GetPathQ() + "\"");
		}
		Transform[] boneXforms;
		string str;
		if (!GTHardCodedBones.TryGetBoneXforms(componentInParent, out boneXforms, out str))
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  Could not get bone transforms: " + str);
		}
		if (this.overrideParentTransform != null)
		{
			this.parentTransform = this.overrideParentTransform;
		}
		else if (!GTHardCodedBones.TryGetBoneXform(boneXforms, this.parentBone.Bone, out this.parentTransform))
		{
			throw new NullReferenceException("[SuperInfectionSnapPoint]  ERROR!!!  " + string.Format("Could not find bone Transform `{0}`.", this.parentBone));
		}
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (this.parentTransform != null)
		{
			base.transform.SetParent(this.parentTransform, false);
		}
		base.transform.localPosition = localPosition;
		base.transform.localEulerAngles = localEulerAngles;
	}

	// Token: 0x06002CB5 RID: 11445 RVA: 0x000F1D47 File Offset: 0x000EFF47
	public void Clear()
	{
		this.Unsnapped();
	}

	// Token: 0x06002CB6 RID: 11446 RVA: 0x000F1D50 File Offset: 0x000EFF50
	public void Snapped(GameEntity entity)
	{
		this.snappedEntity = entity;
		GameSnappable gameSnappable;
		if (this.snappedEntity.TryGetComponent<GameSnappable>(out gameSnappable))
		{
			gameSnappable.snappedToJoint = this;
			return;
		}
		Debug.LogError(string.Format("Snapped: entity {0} has no GameSnappable!?", this.snappedEntity));
	}

	// Token: 0x06002CB7 RID: 11447 RVA: 0x000F1D90 File Offset: 0x000EFF90
	public void Unsnapped()
	{
		GameSnappable gameSnappable;
		if (this.snappedEntity && this.snappedEntity.TryGetComponent<GameSnappable>(out gameSnappable))
		{
			gameSnappable.snappedToJoint = null;
		}
		else
		{
			Debug.LogError(string.Format("Unsnapped: entity {0} has no GameSnappable!?", this.snappedEntity));
		}
		this.snappedEntity = null;
	}

	// Token: 0x06002CB8 RID: 11448 RVA: 0x000F1DDE File Offset: 0x000EFFDE
	public bool HasSnapped()
	{
		return this.snappedEntity != null;
	}

	// Token: 0x06002CB9 RID: 11449 RVA: 0x000F1DEC File Offset: 0x000EFFEC
	public GameEntity GetSnappedEntity()
	{
		return this.snappedEntity;
	}

	// Token: 0x04003921 RID: 14625
	private const string preLog = "[SuperInfectionSnapPoint]  ";

	// Token: 0x04003922 RID: 14626
	private const string preErr = "[SuperInfectionSnapPoint]  ERROR!!!  ";

	// Token: 0x04003923 RID: 14627
	public GamePlayer playerForPoint;

	// Token: 0x04003924 RID: 14628
	public SnapJointType jointType;

	// Token: 0x04003925 RID: 14629
	public GTHardCodedBones.SturdyEBone parentBone;

	// Token: 0x04003926 RID: 14630
	public Transform overrideParentTransform;

	// Token: 0x04003927 RID: 14631
	private Transform parentTransform;

	// Token: 0x04003928 RID: 14632
	public bool canSnapOverride;

	// Token: 0x04003929 RID: 14633
	public float snapPointRadius;

	// Token: 0x0400392A RID: 14634
	private GameEntity snappedEntity;
}
