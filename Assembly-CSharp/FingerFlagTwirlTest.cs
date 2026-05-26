using System;
using UnityEngine;

// Token: 0x020001F7 RID: 503
public class FingerFlagTwirlTest : MonoBehaviour
{
	// Token: 0x06000D37 RID: 3383 RVA: 0x00048444 File Offset: 0x00046644
	protected void FixedUpdate()
	{
		this.animTimes += Time.deltaTime * this.rotAnimDurations;
		this.animTimes.x = this.animTimes.x % 1f;
		this.animTimes.y = this.animTimes.y % 1f;
		this.animTimes.z = this.animTimes.z % 1f;
		base.transform.localRotation = Quaternion.Euler(this.rotXAnimCurve.Evaluate(this.animTimes.x) * this.rotAnimAmplitudes.x, this.rotYAnimCurve.Evaluate(this.animTimes.y) * this.rotAnimAmplitudes.y, this.rotZAnimCurve.Evaluate(this.animTimes.z) * this.rotAnimAmplitudes.z);
	}

	// Token: 0x04000FCA RID: 4042
	public Vector3 rotAnimDurations = new Vector3(0.2f, 0.1f, 0.5f);

	// Token: 0x04000FCB RID: 4043
	public Vector3 rotAnimAmplitudes = Vector3.one * 360f;

	// Token: 0x04000FCC RID: 4044
	public AnimationCurve rotXAnimCurve;

	// Token: 0x04000FCD RID: 4045
	public AnimationCurve rotYAnimCurve;

	// Token: 0x04000FCE RID: 4046
	public AnimationCurve rotZAnimCurve;

	// Token: 0x04000FCF RID: 4047
	private Vector3 animTimes = Vector3.zero;
}
