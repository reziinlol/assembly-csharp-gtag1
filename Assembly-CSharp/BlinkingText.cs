using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020001C2 RID: 450
public class BlinkingText : MonoBehaviour
{
	// Token: 0x06000BF5 RID: 3061 RVA: 0x0004120C File Offset: 0x0003F40C
	private void Awake()
	{
		this.textComponent = base.GetComponent<Text>();
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x0004121C File Offset: 0x0003F41C
	private void Update()
	{
		if (this.isOn && Time.time > this.lastTime + this.cycleTime * this.dutyCycle)
		{
			this.isOn = false;
			this.textComponent.enabled = false;
			return;
		}
		if (!this.isOn && Time.time > this.lastTime + this.cycleTime)
		{
			this.lastTime = Time.time;
			this.isOn = true;
			this.textComponent.enabled = true;
		}
	}

	// Token: 0x04000E95 RID: 3733
	public float cycleTime;

	// Token: 0x04000E96 RID: 3734
	public float dutyCycle;

	// Token: 0x04000E97 RID: 3735
	private bool isOn;

	// Token: 0x04000E98 RID: 3736
	private float lastTime;

	// Token: 0x04000E99 RID: 3737
	private Text textComponent;
}
