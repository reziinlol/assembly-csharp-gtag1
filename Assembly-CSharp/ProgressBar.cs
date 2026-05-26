using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200093F RID: 2367
public class ProgressBar : MonoBehaviour
{
	// Token: 0x06003E0D RID: 15885 RVA: 0x0014F1C0 File Offset: 0x0014D3C0
	public void UpdateProgress(float newFill)
	{
		bool flag = newFill > 1f;
		this._fillAmount = Mathf.Clamp(newFill, 0f, 1f);
		this.fillImage.fillAmount = this._fillAmount;
		if (this.useColors)
		{
			if (flag)
			{
				this.fillImage.color = this.overCapacity;
				return;
			}
			if (Mathf.Approximately(this._fillAmount, 1f))
			{
				this.fillImage.color = this.atCapacity;
				return;
			}
			this.fillImage.color = this.underCapacity;
		}
	}

	// Token: 0x04004E42 RID: 20034
	[SerializeField]
	private Image fillImage;

	// Token: 0x04004E43 RID: 20035
	[SerializeField]
	private bool useColors;

	// Token: 0x04004E44 RID: 20036
	[SerializeField]
	private Color underCapacity = Color.green;

	// Token: 0x04004E45 RID: 20037
	[SerializeField]
	private Color overCapacity = Color.red;

	// Token: 0x04004E46 RID: 20038
	[SerializeField]
	private Color atCapacity = Color.yellow;

	// Token: 0x04004E47 RID: 20039
	private float _fillAmount;
}
