using System;
using System.Collections;
using System.Collections.Generic;

// Token: 0x02000D98 RID: 3480
public class UniqueQueue<T> : IEnumerable<!0>, IEnumerable
{
	// Token: 0x17000800 RID: 2048
	// (get) Token: 0x0600555F RID: 21855 RVA: 0x001BD9B3 File Offset: 0x001BBBB3
	public int Count
	{
		get
		{
			return this.queue.Count;
		}
	}

	// Token: 0x06005560 RID: 21856 RVA: 0x001BD9C0 File Offset: 0x001BBBC0
	public UniqueQueue()
	{
		this.queuedItems = new HashSet<T>();
		this.queue = new Queue<T>();
	}

	// Token: 0x06005561 RID: 21857 RVA: 0x001BD9DE File Offset: 0x001BBBDE
	public UniqueQueue(int capacity)
	{
		this.queuedItems = new HashSet<T>(capacity);
		this.queue = new Queue<T>(capacity);
	}

	// Token: 0x06005562 RID: 21858 RVA: 0x001BD9FE File Offset: 0x001BBBFE
	public void Clear()
	{
		this.queuedItems.Clear();
		this.queue.Clear();
	}

	// Token: 0x06005563 RID: 21859 RVA: 0x001BDA16 File Offset: 0x001BBC16
	public bool Enqueue(T item)
	{
		if (!this.queuedItems.Add(item))
		{
			return false;
		}
		this.queue.Enqueue(item);
		return true;
	}

	// Token: 0x06005564 RID: 21860 RVA: 0x001BDA38 File Offset: 0x001BBC38
	public T Dequeue()
	{
		T t = this.queue.Dequeue();
		this.queuedItems.Remove(t);
		return t;
	}

	// Token: 0x06005565 RID: 21861 RVA: 0x001BDA5F File Offset: 0x001BBC5F
	public bool TryDequeue(out T item)
	{
		if (this.queue.Count < 1)
		{
			item = default(T);
			return false;
		}
		item = this.Dequeue();
		return true;
	}

	// Token: 0x06005566 RID: 21862 RVA: 0x001BDA85 File Offset: 0x001BBC85
	public T Peek()
	{
		return this.queue.Peek();
	}

	// Token: 0x06005567 RID: 21863 RVA: 0x001BDA92 File Offset: 0x001BBC92
	public bool Contains(T item)
	{
		return this.queuedItems.Contains(item);
	}

	// Token: 0x06005568 RID: 21864 RVA: 0x001BDAA0 File Offset: 0x001BBCA0
	IEnumerator<T> IEnumerable<!0>.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x06005569 RID: 21865 RVA: 0x001BDAA0 File Offset: 0x001BBCA0
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.queue.GetEnumerator();
	}

	// Token: 0x040065A9 RID: 26025
	private HashSet<T> queuedItems;

	// Token: 0x040065AA RID: 26026
	private Queue<T> queue;
}
