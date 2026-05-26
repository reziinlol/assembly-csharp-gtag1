using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004FA RID: 1274
public class BetaButton : GorillaPressableButton
{
	// Token: 0x06001FF8 RID: 8184 RVA: 0x000AC130 File Offset: 0x000AA330
	public override void ButtonActivation()
	{
		base.ButtonActivation();
		this.count++;
		base.StartCoroutine(this.ButtonColorUpdate());
		if (this.count >= 10)
		{
			this.betaParent.SetActive(false);
			PlayerPrefs.SetString("CheckedBox2", "true");
			PlayerPrefs.Save();
		}
	}

	// Token: 0x06001FF9 RID: 8185 RVA: 0x000AC188 File Offset: 0x000AA388
	private IEnumerator ButtonColorUpdate()
	{
		this.buttonRenderer.material = this.pressedMaterial;
		yield return new WaitForSeconds(this.buttonFadeTime);
		this.buttonRenderer.material = this.unpressedMaterial;
		yield break;
	}

	// Token: 0x04002ABF RID: 10943
	public GameObject betaParent;

	// Token: 0x04002AC0 RID: 10944
	public int count;

	// Token: 0x04002AC1 RID: 10945
	public float buttonFadeTime = 0.25f;

	// Token: 0x04002AC2 RID: 10946
	public Text messageText;
}
