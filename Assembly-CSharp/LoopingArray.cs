using System;
using GorillaTag;

// Token: 0x02000D59 RID: 3417
public class LoopingArray<T> : ObjectPoolEvents
{
	// Token: 0x170007EC RID: 2028
	// (get) Token: 0x060053F2 RID: 21490 RVA: 0x001B6D58 File Offset: 0x001B4F58
	public int Length
	{
		get
		{
			return this.m_length;
		}
	}

	// Token: 0x170007ED RID: 2029
	// (get) Token: 0x060053F3 RID: 21491 RVA: 0x001B6D60 File Offset: 0x001B4F60
	public int CurrentIndex
	{
		get
		{
			return this.m_currentIndex;
		}
	}

	// Token: 0x170007EE RID: 2030
	public T this[int index]
	{
		get
		{
			return this.m_array[index];
		}
		set
		{
			this.m_array[index] = value;
		}
	}

	// Token: 0x060053F6 RID: 21494 RVA: 0x001B6D85 File Offset: 0x001B4F85
	public LoopingArray() : this(0)
	{
	}

	// Token: 0x060053F7 RID: 21495 RVA: 0x001B6D8E File Offset: 0x001B4F8E
	public LoopingArray(int capicity)
	{
		this.m_length = capicity;
		this.m_array = new T[capicity];
		this.Clear();
	}

	// Token: 0x060053F8 RID: 21496 RVA: 0x001B6DAF File Offset: 0x001B4FAF
	public int AddAndIncrement(in T value)
	{
		int currentIndex = this.m_currentIndex;
		this.m_array[this.m_currentIndex] = value;
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		return currentIndex;
	}

	// Token: 0x060053F9 RID: 21497 RVA: 0x001B6DE3 File Offset: 0x001B4FE3
	public int IncrementAndAdd(in T value)
	{
		this.m_currentIndex = (this.m_currentIndex + 1) % this.m_length;
		this.m_array[this.m_currentIndex] = value;
		return this.m_currentIndex;
	}

	// Token: 0x060053FA RID: 21498 RVA: 0x001B6E18 File Offset: 0x001B5018
	public void Clear()
	{
		this.m_currentIndex = 0;
		for (int i = 0; i < this.m_array.Length; i++)
		{
			this.m_array[i] = default(T);
		}
	}

	// Token: 0x060053FB RID: 21499 RVA: 0x001B6E54 File Offset: 0x001B5054
	void ObjectPoolEvents.OnTaken()
	{
		this.Clear();
	}

	// Token: 0x060053FC RID: 21500 RVA: 0x000028C5 File Offset: 0x00000AC5
	void ObjectPoolEvents.OnReturned()
	{
	}

	// Token: 0x040064FB RID: 25851
	private int m_length;

	// Token: 0x040064FC RID: 25852
	private int m_currentIndex;

	// Token: 0x040064FD RID: 25853
	private T[] m_array;

	// Token: 0x02000D5A RID: 3418
	public class Pool : ObjectPool<LoopingArray<T>>
	{
		// Token: 0x060053FD RID: 21501 RVA: 0x001B6E5C File Offset: 0x001B505C
		private Pool(int amount) : base(amount)
		{
		}

		// Token: 0x060053FE RID: 21502 RVA: 0x001B6E65 File Offset: 0x001B5065
		public Pool(int size, int amount) : this(size, amount, amount)
		{
		}

		// Token: 0x060053FF RID: 21503 RVA: 0x001B6E70 File Offset: 0x001B5070
		public Pool(int size, int initialAmount, int maxAmount)
		{
			this.m_size = size;
			base.InitializePool(initialAmount, maxAmount);
		}

		// Token: 0x06005400 RID: 21504 RVA: 0x001B6E87 File Offset: 0x001B5087
		public override LoopingArray<T> CreateInstance()
		{
			return new LoopingArray<T>(this.m_size);
		}

		// Token: 0x040064FE RID: 25854
		private readonly int m_size;
	}
}
