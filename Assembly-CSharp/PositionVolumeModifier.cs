using System;
using UnityEngine;

// Token: 0x02000363 RID: 867
public class PositionVolumeModifier : MonoBehaviour
{
	// Token: 0x06001532 RID: 5426 RVA: 0x000707E5 File Offset: 0x0006E9E5
	public void OnTriggerStay(Collider other)
	{
		this.audioToMod.isModified = true;
	}

	// Token: 0x04001A05 RID: 6661
	public TimeOfDayDependentAudio audioToMod;
}
