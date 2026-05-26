using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class WeightedList<T>
{
	// Token: 0x1700003F RID: 63
	// (get) Token: 0x06000396 RID: 918 RVA: 0x00014C76 File Offset: 0x00012E76
	public int Count
	{
		get
		{
			return this.items.Count;
		}
	}

	// Token: 0x17000040 RID: 64
	// (get) Token: 0x06000397 RID: 919 RVA: 0x00014C83 File Offset: 0x00012E83
	public List<T> Items
	{
		get
		{
			return this.items;
		}
	}

	// Token: 0x06000398 RID: 920 RVA: 0x00014C8C File Offset: 0x00012E8C
	public void Add(T item, float weight)
	{
		if (weight <= 0f)
		{
			throw new ArgumentException("Weight must be greater than zero.");
		}
		this.totalWeight += weight;
		this.items.Add(item);
		this.weights.Add(weight);
		this.cumulativeWeights.Add(this.totalWeight);
	}

	// Token: 0x17000041 RID: 65
	[TupleElementNames(new string[]
	{
		"Item",
		"Weight"
	})]
	public ValueTuple<T, float> this[int index]
	{
		[return: TupleElementNames(new string[]
		{
			"Item",
			"Weight"
		})]
		get
		{
			if (index < 0 || index >= this.items.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return new ValueTuple<T, float>(this.items[index], this.weights[index]);
		}
	}

	// Token: 0x0600039A RID: 922 RVA: 0x00014D1A File Offset: 0x00012F1A
	public T GetRandomItem()
	{
		return this.items[this.GetRandomIndex()];
	}

	// Token: 0x0600039B RID: 923 RVA: 0x00014D30 File Offset: 0x00012F30
	public int GetRandomIndex()
	{
		if (this.items.Count == 0)
		{
			throw new InvalidOperationException("The list is empty.");
		}
		float item = Random.value * this.totalWeight;
		int num = this.cumulativeWeights.BinarySearch(item);
		if (num < 0)
		{
			num = ~num;
		}
		return num;
	}

	// Token: 0x0600039C RID: 924 RVA: 0x00014D78 File Offset: 0x00012F78
	public bool Remove(T item)
	{
		int num = this.items.IndexOf(item);
		if (num == -1)
		{
			return false;
		}
		this.RemoveAt(num);
		return true;
	}

	// Token: 0x0600039D RID: 925 RVA: 0x00014DA0 File Offset: 0x00012FA0
	public void RemoveAt(int index)
	{
		if (index < 0 || index >= this.items.Count)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		this.totalWeight -= this.weights[index];
		this.items.RemoveAt(index);
		this.weights.RemoveAt(index);
		this.RecalculateCumulativeWeights();
	}

	// Token: 0x0600039E RID: 926 RVA: 0x00014E04 File Offset: 0x00013004
	private void RecalculateCumulativeWeights()
	{
		this.cumulativeWeights.Clear();
		float num = 0f;
		foreach (float num2 in this.weights)
		{
			num += num2;
			this.cumulativeWeights.Add(num);
		}
		this.totalWeight = num;
	}

	// Token: 0x0600039F RID: 927 RVA: 0x00014E78 File Offset: 0x00013078
	public void Clear()
	{
		this.items.Clear();
		this.weights.Clear();
		this.cumulativeWeights.Clear();
		this.totalWeight = 0f;
	}

	// Token: 0x04000422 RID: 1058
	private List<T> items = new List<T>();

	// Token: 0x04000423 RID: 1059
	private List<float> weights = new List<float>();

	// Token: 0x04000424 RID: 1060
	private List<float> cumulativeWeights = new List<float>();

	// Token: 0x04000425 RID: 1061
	private float totalWeight;
}
