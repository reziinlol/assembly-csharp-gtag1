using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000566 RID: 1382
public class CreatorCodeSmallDisplay : MonoBehaviour
{
	// Token: 0x0600231E RID: 8990 RVA: 0x000BD17A File Offset: 0x000BB37A
	private void Awake()
	{
		this.codeText.text = "CREATOR CODE: <NONE>";
		ATM_Manager.instance.smallDisplays.Add(this);
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000BD19E File Offset: 0x000BB39E
	public void SetCode(string code)
	{
		if (code == "")
		{
			this.codeText.text = "CREATOR CODE: <NONE>";
			return;
		}
		this.codeText.text = "CREATOR CODE: " + code;
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000BD1D4 File Offset: 0x000BB3D4
	public void SuccessfulPurchase(string memberName)
	{
		if (!string.IsNullOrWhiteSpace(memberName))
		{
			this.codeText.text = "SUPPORTED: " + memberName + "!";
		}
	}

	// Token: 0x04002E3F RID: 11839
	public Text codeText;

	// Token: 0x04002E40 RID: 11840
	private const string CreatorCode = "CREATOR CODE: ";

	// Token: 0x04002E41 RID: 11841
	private const string CreatorSupported = "SUPPORTED: ";

	// Token: 0x04002E42 RID: 11842
	private const string NoCreator = "<NONE>";
}
