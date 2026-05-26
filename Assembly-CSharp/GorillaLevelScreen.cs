using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000878 RID: 2168
public class GorillaLevelScreen : MonoBehaviour
{
	// Token: 0x06003888 RID: 14472 RVA: 0x00135329 File Offset: 0x00133529
	private void Awake()
	{
		if (this.myText != null)
		{
			this.startingText = this.myText.text;
		}
	}

	// Token: 0x06003889 RID: 14473 RVA: 0x0013534C File Offset: 0x0013354C
	public void UpdateText(string newText, bool setToGoodMaterial)
	{
		if (this.myText != null)
		{
			this.myText.text = newText;
		}
		Material[] materials = base.GetComponent<MeshRenderer>().materials;
		materials[0] = (setToGoodMaterial ? this.goodMaterial : this.badMaterial);
		base.GetComponent<MeshRenderer>().materials = materials;
	}

	// Token: 0x0400488B RID: 18571
	public string startingText;

	// Token: 0x0400488C RID: 18572
	public Material goodMaterial;

	// Token: 0x0400488D RID: 18573
	public Material badMaterial;

	// Token: 0x0400488E RID: 18574
	public Text myText;
}
