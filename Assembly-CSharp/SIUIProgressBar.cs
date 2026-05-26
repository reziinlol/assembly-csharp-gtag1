using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200017A RID: 378
public class SIUIProgressBar : MonoBehaviour
{
	// Token: 0x060009E8 RID: 2536 RVA: 0x0003564C File Offset: 0x0003384C
	public void UpdateFillPercent(float percentFull)
	{
		float num = this.backgroundImage.rectTransform.sizeDelta.x * (1f - 2f * this.borderPercent / 100f);
		float num2 = num * Mathf.Min(1f, percentFull);
		float x = -(num - num2) / 2f * this.progressImage.rectTransform.localScale.x;
		this.progressImage.rectTransform.sizeDelta = new Vector2(num2, this.progressImage.rectTransform.sizeDelta.y);
		this.progressImage.rectTransform.localPosition = new Vector3(x, this.progressImage.rectTransform.localPosition.y, this.progressImage.rectTransform.localPosition.z);
	}

	// Token: 0x04000C3C RID: 3132
	public Image backgroundImage;

	// Token: 0x04000C3D RID: 3133
	public Image progressImage;

	// Token: 0x04000C3E RID: 3134
	public float borderPercent;

	// Token: 0x04000C3F RID: 3135
	public TextMeshProUGUI progressText;
}
