using System;
using System.Collections;
using GorillaNetworking;
using UnityEngine;

// Token: 0x02000577 RID: 1399
public class WardrobeFunctionButton : GorillaPressableButton
{
	// Token: 0x0600238B RID: 9099 RVA: 0x000BFBD6 File Offset: 0x000BDDD6
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		CosmeticsController.instance.PressWardrobeFunctionButton(this.function);
		base.StartCoroutine(this.ButtonColorUpdate());
	}

	// Token: 0x0600238C RID: 9100 RVA: 0x000028C5 File Offset: 0x00000AC5
	public override void UpdateColor()
	{
	}

	// Token: 0x0600238D RID: 9101 RVA: 0x000BFBFD File Offset: 0x000BDDFD
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002EB5 RID: 11957
	public string function;

	// Token: 0x04002EB6 RID: 11958
	public float buttonFadeTime = 0.25f;
}
