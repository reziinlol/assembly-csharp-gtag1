using System;
using UnityEngine;

// Token: 0x0200079E RID: 1950
public class GRFadeAndDestroyLight : MonoBehaviour
{
	// Token: 0x060031DE RID: 12766 RVA: 0x00111DE3 File Offset: 0x0010FFE3
	private void Start()
	{
		if (this.gameLight != null)
		{
			this.fadeRate = this.gameLight.light.intensity / this.TimeToFade;
		}
		this.timeSinceLastUpdate = Time.time;
	}

	// Token: 0x060031DF RID: 12767 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnEnable()
	{
	}

	// Token: 0x060031E0 RID: 12768 RVA: 0x000028C5 File Offset: 0x00000AC5
	public void OnDisable()
	{
	}

	// Token: 0x060031E1 RID: 12769 RVA: 0x00111E1C File Offset: 0x0011001C
	public void Update()
	{
		if (Time.time < this.timeSinceLastUpdate || Time.time > this.timeSinceLastUpdate + this.timeSlice)
		{
			this.timeSinceLastUpdate = Time.time;
			float num = this.gameLight.light.intensity;
			num -= this.timeSlice * this.fadeRate;
			if (num <= 0f)
			{
				base.gameObject.Destroy();
				return;
			}
			this.gameLight.light.intensity = num;
		}
	}

	// Token: 0x040040B1 RID: 16561
	public float TimeToFade = 10f;

	// Token: 0x040040B2 RID: 16562
	private float fadeRate;

	// Token: 0x040040B3 RID: 16563
	public GameLight gameLight;

	// Token: 0x040040B4 RID: 16564
	public float timeSlice = 0.1f;

	// Token: 0x040040B5 RID: 16565
	public float timeSinceLastUpdate;
}
