using System;
using System.Collections;
using UnityEngine;

namespace GorillaTag
{
	// Token: 0x0200116E RID: 4462
	public class CustomMapTestingScript : GorillaPressableButton
	{
		// Token: 0x06007103 RID: 28931 RVA: 0x0024F408 File Offset: 0x0024D608
		public override void ButtonActivation()
		{
			base.ButtonActivation();
			base.StartCoroutine(this.ButtonPressed_Local());
		}

		// Token: 0x06007104 RID: 28932 RVA: 0x0024F41D File Offset: 0x0024D61D
		private IEnumerator ButtonPressed_Local()
		{
			this.isOn = true;
			this.UpdateColor();
			yield return new WaitForSeconds(this.debounceTime);
			this.isOn = false;
			this.UpdateColor();
			yield break;
		}
	}
}
