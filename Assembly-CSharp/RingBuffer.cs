using System;
using System.Collections.Generic;

// Token: 0x02000AEB RID: 2795
public class RingBuffer<T>
{
	// Token: 0x170006A4 RID: 1700
	// (get) Token: 0x06004748 RID: 18248 RVA: 0x001803C6 File Offset: 0x0017E5C6
	public int Size
	{
		get
		{
			return this._size;
		}
	}

	// Token: 0x170006A5 RID: 1701
	// (get) Token: 0x06004749 RID: 18249 RVA: 0x001803CE File Offset: 0x0017E5CE
	public int Capacity
	{
		get
		{
			return this._capacity;
		}
	}

	// Token: 0x170006A6 RID: 1702
	// (get) Token: 0x0600474A RID: 18250 RVA: 0x001803D6 File Offset: 0x0017E5D6
	public bool IsFull
	{
		get
		{
			return this._size == this._capacity;
		}
	}

	// Token: 0x170006A7 RID: 1703
	// (get) Token: 0x0600474B RID: 18251 RVA: 0x001803E6 File Offset: 0x0017E5E6
	public bool IsEmpty
	{
		get
		{
			return this._size == 0;
		}
	}

	// Token: 0x0600474C RID: 18252 RVA: 0x001803F1 File Offset: 0x0017E5F1
	public RingBuffer(int capacity)
	{
		if (capacity < 1)
		{
			throw new ArgumentException("Can't be zero or negative", "capacity");
		}
		this._size = 0;
		this._capacity = capacity;
		this._items = new T[capacity];
	}

	// Token: 0x0600474D RID: 18253 RVA: 0x00180427 File Offset: 0x0017E627
	public RingBuffer(IList<T> list) : this(list.Count)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		list.CopyTo(this._items, 0);
	}

	// Token: 0x0600474E RID: 18254 RVA: 0x00180450 File Offset: 0x0017E650
	public ref T PeekFirst()
	{
		return ref this._items[this._head];
	}

	// Token: 0x0600474F RID: 18255 RVA: 0x00180463 File Offset: 0x0017E663
	public ref T PeekLast()
	{
		return ref this._items[this._tail];
	}

	// Token: 0x06004750 RID: 18256 RVA: 0x00180478 File Offset: 0x0017E678
	public bool Push(T item)
	{
		if (this._size == this._capacity)
		{
			return false;
		}
		this._items[this._tail] = item;
		this._tail = (this._tail + 1) % this._capacity;
		this._size++;
		return true;
	}

	// Token: 0x06004751 RID: 18257 RVA: 0x001804CC File Offset: 0x0017E6CC
	public T Pop()
	{
		if (this._size == 0)
		{
			return default(T);
		}
		T result = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return result;
	}

	// Token: 0x06004752 RID: 18258 RVA: 0x00180520 File Offset: 0x0017E720
	public bool TryPop(out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head];
		this._head = (this._head + 1) % this._capacity;
		this._size--;
		return true;
	}

	// Token: 0x06004753 RID: 18259 RVA: 0x00180579 File Offset: 0x0017E779
	public void Clear()
	{
		this._head = 0;
		this._tail = 0;
		this._size = 0;
		Array.Clear(this._items, 0, this._capacity);
	}

	// Token: 0x06004754 RID: 18260 RVA: 0x001805A2 File Offset: 0x0017E7A2
	public bool TryGet(int i, out T item)
	{
		if (this._size == 0)
		{
			item = default(T);
			return false;
		}
		item = this._items[this._head + i % this._size];
		return true;
	}

	// Token: 0x06004755 RID: 18261 RVA: 0x001805D6 File Offset: 0x0017E7D6
	public ArraySegment<T> AsSegment()
	{
		return new ArraySegment<T>(this._items);
	}

	// Token: 0x040059BD RID: 22973
	private T[] _items;

	// Token: 0x040059BE RID: 22974
	private int _head;

	// Token: 0x040059BF RID: 22975
	private int _tail;

	// Token: 0x040059C0 RID: 22976
	private int _size;

	// Token: 0x040059C1 RID: 22977
	private readonly int _capacity;
}
