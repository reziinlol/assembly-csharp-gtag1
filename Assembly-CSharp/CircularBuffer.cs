using System;

// Token: 0x02000D38 RID: 3384
internal class CircularBuffer<T>
{
	// Token: 0x170007E1 RID: 2017
	// (get) Token: 0x06005354 RID: 21332 RVA: 0x001B4357 File Offset: 0x001B2557
	// (set) Token: 0x06005355 RID: 21333 RVA: 0x001B435F File Offset: 0x001B255F
	public int Count { get; private set; }

	// Token: 0x170007E2 RID: 2018
	// (get) Token: 0x06005356 RID: 21334 RVA: 0x001B4368 File Offset: 0x001B2568
	// (set) Token: 0x06005357 RID: 21335 RVA: 0x001B4370 File Offset: 0x001B2570
	public int Capacity { get; private set; }

	// Token: 0x06005358 RID: 21336 RVA: 0x001B4379 File Offset: 0x001B2579
	public CircularBuffer(int capacity)
	{
		this.backingArray = new T[capacity];
		this.Capacity = capacity;
		this.Count = 0;
	}

	// Token: 0x06005359 RID: 21337 RVA: 0x001B439C File Offset: 0x001B259C
	public void Add(T value)
	{
		this.backingArray[this.nextWriteIdx] = value;
		this.lastWriteIdx = this.nextWriteIdx;
		this.nextWriteIdx = (this.nextWriteIdx + 1) % this.Capacity;
		if (this.Count < this.Capacity)
		{
			int count = this.Count;
			this.Count = count + 1;
		}
	}

	// Token: 0x0600535A RID: 21338 RVA: 0x001B43FA File Offset: 0x001B25FA
	public void Clear()
	{
		this.Count = 0;
	}

	// Token: 0x0600535B RID: 21339 RVA: 0x001B4403 File Offset: 0x001B2603
	public T Last()
	{
		return this.backingArray[this.lastWriteIdx];
	}

	// Token: 0x170007E3 RID: 2019
	public T this[int logicalIdx]
	{
		get
		{
			if (logicalIdx < 0 || logicalIdx >= this.Count)
			{
				throw new ArgumentOutOfRangeException("logicalIdx", logicalIdx, string.Format("Out of bounds index {0} into CircularBuffer with length {1}", logicalIdx, this.Count));
			}
			int num = (this.lastWriteIdx + this.Capacity - logicalIdx) % this.Capacity;
			return this.backingArray[num];
		}
	}

	// Token: 0x04006489 RID: 25737
	private T[] backingArray;

	// Token: 0x0400648C RID: 25740
	private int nextWriteIdx;

	// Token: 0x0400648D RID: 25741
	private int lastWriteIdx;
}
