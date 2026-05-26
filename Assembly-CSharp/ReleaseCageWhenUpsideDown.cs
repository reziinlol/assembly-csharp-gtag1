using System;
using UnityEngine;
using UnityEngine.Serialization;

// Token: 0x0200008F RID: 143
public class ReleaseCageWhenUpsideDown : MonoBehaviour
{
	// Token: 0x0600038E RID: 910 RVA: 0x00014A46 File Offset: 0x00012C46
	private void Awake()
	{
		this.cage = base.GetComponentInChildren<CrittersCage>();
	}

	// Token: 0x0600038F RID: 911 RVA: 0x00014A54 File Offset: 0x00012C54
	private void Update()
	{
		this.cage.inReleasingPosition = (Vector3.Angle(base.transform.up, Vector3.down) < this.releaseCritterThreshold);
	}

	// Token: 0x04000413 RID: 1043
	public CrittersCage cage;

	// Token: 0x04000414 RID: 1044
	[FormerlySerializedAs("dumpThreshold")]
	[FormerlySerializedAs("angle")]
	public float releaseCritterThreshold = 30f;
}
