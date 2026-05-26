using System;
using UnityEngine;

// Token: 0x02000091 RID: 145
public class SpawnSoundOnEnable : MonoBehaviour
{
	// Token: 0x06000394 RID: 916 RVA: 0x00014BC4 File Offset: 0x00012DC4
	private void OnEnable()
	{
		if (CrittersManager.instance == null || !CrittersManager.instance.LocalAuthority() || !CrittersManager.instance.LocalInZone)
		{
			return;
		}
		if (!this.triggerOnFirstEnable && !this.firstEnabledOccured)
		{
			this.firstEnabledOccured = true;
			return;
		}
		CrittersLoudNoise crittersLoudNoise = (CrittersLoudNoise)CrittersManager.instance.SpawnActor(CrittersActor.CrittersActorType.LoudNoise, this.soundSubIndex);
		if (crittersLoudNoise == null)
		{
			return;
		}
		crittersLoudNoise.MoveActor(base.transform.position, base.transform.rotation, false, true, true);
		crittersLoudNoise.SetImpulseVelocity(Vector3.zero, Vector3.zero);
	}

	// Token: 0x0400041F RID: 1055
	public int soundSubIndex = 3;

	// Token: 0x04000420 RID: 1056
	public bool triggerOnFirstEnable;

	// Token: 0x04000421 RID: 1057
	private bool firstEnabledOccured;
}
