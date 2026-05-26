using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002C2 RID: 706
public class EqualizerAnim : MonoBehaviour
{
	// Token: 0x06001238 RID: 4664 RVA: 0x00061841 File Offset: 0x0005FA41
	private void Start()
	{
		this.inputColorHash = Shader.PropertyToID(this.inputColorProperty);
	}

	// Token: 0x06001239 RID: 4665 RVA: 0x00061854 File Offset: 0x0005FA54
	private void Update()
	{
		if (EqualizerAnim.thisFrame == Time.frameCount)
		{
			if (EqualizerAnim.materialsUpdatedThisFrame.Contains(this.material))
			{
				return;
			}
		}
		else
		{
			EqualizerAnim.thisFrame = Time.frameCount;
			EqualizerAnim.materialsUpdatedThisFrame.Clear();
		}
		float time = Time.time % this.loopDuration;
		this.material.SetColor(this.inputColorHash, new Color(this.redCurve.Evaluate(time), this.greenCurve.Evaluate(time), this.blueCurve.Evaluate(time)));
		EqualizerAnim.materialsUpdatedThisFrame.Add(this.material);
	}

	// Token: 0x04001613 RID: 5651
	[SerializeField]
	private AnimationCurve redCurve;

	// Token: 0x04001614 RID: 5652
	[SerializeField]
	private AnimationCurve greenCurve;

	// Token: 0x04001615 RID: 5653
	[SerializeField]
	private AnimationCurve blueCurve;

	// Token: 0x04001616 RID: 5654
	[SerializeField]
	private float loopDuration;

	// Token: 0x04001617 RID: 5655
	[SerializeField]
	private Material material;

	// Token: 0x04001618 RID: 5656
	[SerializeField]
	private string inputColorProperty;

	// Token: 0x04001619 RID: 5657
	private int inputColorHash;

	// Token: 0x0400161A RID: 5658
	private static int thisFrame;

	// Token: 0x0400161B RID: 5659
	private static HashSet<Material> materialsUpdatedThisFrame = new HashSet<Material>();
}
