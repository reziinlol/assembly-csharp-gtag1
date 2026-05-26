using System;
using UnityEngine;

// Token: 0x02000DCA RID: 3530
public class LightningDispatcher : MonoBehaviour
{
	// Token: 0x1400009A RID: 154
	// (add) Token: 0x06005682 RID: 22146 RVA: 0x001C0DFC File Offset: 0x001BEFFC
	// (remove) Token: 0x06005683 RID: 22147 RVA: 0x001C0E30 File Offset: 0x001BF030
	public static event LightningDispatcher.DispatchLightningEvent RequestLightningStrike;

	// Token: 0x06005684 RID: 22148 RVA: 0x001C0E64 File Offset: 0x001BF064
	public void DispatchLightning(Vector3 p1, Vector3 p2)
	{
		if (LightningDispatcher.RequestLightningStrike != null)
		{
			LightningStrike lightningStrike = LightningDispatcher.RequestLightningStrike(p1, p2);
			float num = Mathf.Max(new float[]
			{
				base.transform.lossyScale.x,
				base.transform.lossyScale.y,
				base.transform.lossyScale.z
			});
			lightningStrike.Play(p1, p2, this.beamWidthCM * 0.01f * num, this.soundVolumeMultiplier / num, LightningStrike.rand.NextFloat(this.minDuration, this.maxDuration), this.colorOverLifetime);
		}
	}

	// Token: 0x04006664 RID: 26212
	[SerializeField]
	private float beamWidthCM = 1f;

	// Token: 0x04006665 RID: 26213
	[SerializeField]
	private float soundVolumeMultiplier = 1f;

	// Token: 0x04006666 RID: 26214
	[SerializeField]
	private float minDuration = 0.05f;

	// Token: 0x04006667 RID: 26215
	[SerializeField]
	private float maxDuration = 0.12f;

	// Token: 0x04006668 RID: 26216
	[SerializeField]
	private Gradient colorOverLifetime;

	// Token: 0x02000DCB RID: 3531
	// (Invoke) Token: 0x06005687 RID: 22151
	public delegate LightningStrike DispatchLightningEvent(Vector3 p1, Vector3 p2);
}
