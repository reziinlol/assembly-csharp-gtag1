using System;
using UnityEngine;

// Token: 0x0200008D RID: 141
public class PeriodicNoiseGenerator : MonoBehaviour
{
	// Token: 0x06000389 RID: 905 RVA: 0x00014951 File Offset: 0x00012B51
	private void Awake()
	{
		this.noiseActor = base.GetComponentInParent<CrittersLoudNoise>();
		this.lastTime = Time.time;
		this.mR = base.GetComponentInChildren<MeshRenderer>();
	}

	// Token: 0x0600038A RID: 906 RVA: 0x00014978 File Offset: 0x00012B78
	private void Update()
	{
		if (!CrittersManager.instance.LocalAuthority())
		{
			return;
		}
		if (Time.time > this.lastTime + this.sleepDuration)
		{
			this.lastTime = Time.time + this.randomDuration * Random.value;
			this.noiseActor.SetTimeEnabled();
			this.noiseActor.soundEnabled = true;
			this.mR.sharedMaterial = this.solid;
		}
		if (!this.noiseActor.soundEnabled && this.mR.sharedMaterial != this.transparent)
		{
			this.mR.sharedMaterial = this.transparent;
		}
	}

	// Token: 0x0400040A RID: 1034
	public float sleepDuration;

	// Token: 0x0400040B RID: 1035
	public float randomDuration;

	// Token: 0x0400040C RID: 1036
	public float lastTime;

	// Token: 0x0400040D RID: 1037
	private CrittersLoudNoise noiseActor;

	// Token: 0x0400040E RID: 1038
	public Material transparent;

	// Token: 0x0400040F RID: 1039
	public Material solid;

	// Token: 0x04000410 RID: 1040
	private MeshRenderer mR;
}
