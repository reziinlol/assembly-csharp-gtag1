using System;
using TMPro;
using UnityEngine;

// Token: 0x0200058F RID: 1423
public class RandomizeLabel : MonoBehaviour
{
	// Token: 0x06002410 RID: 9232 RVA: 0x000C1AC6 File Offset: 0x000BFCC6
	public void Randomize()
	{
		this.strings.distinct = this.distinct;
		this.label.text = this.strings.NextItem();
	}

	// Token: 0x04002F50 RID: 12112
	public TMP_Text label;

	// Token: 0x04002F51 RID: 12113
	public RandomStrings strings;

	// Token: 0x04002F52 RID: 12114
	public bool distinct;
}
