using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020009BC RID: 2492
public class RandomizeTest : MonoBehaviour
{
	// Token: 0x06003FC4 RID: 16324 RVA: 0x00154EC0 File Offset: 0x001530C0
	private void Start()
	{
		for (int i = 0; i < 10; i++)
		{
			this.testList.Add(i);
		}
		for (int j = 0; j < 10; j++)
		{
			this.testListArray[j] = 0;
		}
		for (int k = 0; k < this.testList.Count; k++)
		{
			this.testListArray[k] = this.testList[k];
		}
		this.RandomizeList(ref this.testList);
		for (int l = 0; l < 10; l++)
		{
			this.testListArray[l] = 0;
		}
		for (int m = 0; m < this.testList.Count; m++)
		{
			this.testListArray[m] = this.testList[m];
		}
	}

	// Token: 0x06003FC5 RID: 16325 RVA: 0x00154F78 File Offset: 0x00153178
	public void RandomizeList(ref List<int> listToRandomize)
	{
		this.randomIterator = 0;
		while (this.randomIterator < listToRandomize.Count)
		{
			this.tempRandIndex = Random.Range(this.randomIterator, listToRandomize.Count);
			this.tempRandValue = listToRandomize[this.randomIterator];
			listToRandomize[this.randomIterator] = listToRandomize[this.tempRandIndex];
			listToRandomize[this.tempRandIndex] = this.tempRandValue;
			this.randomIterator++;
		}
	}

	// Token: 0x0400503A RID: 20538
	public List<int> testList = new List<int>();

	// Token: 0x0400503B RID: 20539
	public int[] testListArray = new int[10];

	// Token: 0x0400503C RID: 20540
	public int randomIterator;

	// Token: 0x0400503D RID: 20541
	public int tempRandIndex;

	// Token: 0x0400503E RID: 20542
	public int tempRandValue;
}
