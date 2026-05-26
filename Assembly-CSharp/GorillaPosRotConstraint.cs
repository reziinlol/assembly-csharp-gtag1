using System;
using GorillaTag.CosmeticSystem;
using UnityEngine;

// Token: 0x0200034A RID: 842
[Serializable]
public struct GorillaPosRotConstraint
{
	// Token: 0x0400196D RID: 6509
	[Tooltip("Transform that should be moved, rotated, and scaled to match the `source` Transform in world space.")]
	public Transform follower;

	// Token: 0x0400196E RID: 6510
	[Tooltip("Bone that `follower` should match. Set to `None` to assign a specific Transform within the same prefab.")]
	public GTHardCodedBones.SturdyEBone sourceGorillaBone;

	// Token: 0x0400196F RID: 6511
	[Tooltip("Transform that `follower` should match. This is overridden at runtime if `sourceGorillaBone` is not `None`. If set in inspector, then it should be only set to a child of the the prefab this component belongs to.")]
	public Transform source;

	// Token: 0x04001970 RID: 6512
	public string sourceRelativePath;

	// Token: 0x04001971 RID: 6513
	[Tooltip("Offset to be applied to the follower's position.")]
	public Vector3 positionOffset;

	// Token: 0x04001972 RID: 6514
	[Tooltip("Offset to be applied to the follower's rotation.")]
	public Quaternion rotationOffset;
}
